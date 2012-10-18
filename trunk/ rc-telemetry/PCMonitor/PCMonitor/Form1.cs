using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace PCMonitor
{
    public partial class Form1 : Form
    {
        private SerialPort sp;
        private decimal[] teplota=new decimal[4];
        private decimal[] napeti = new decimal[6];
        private int ID;
        private int latS, latM, latDM, lonS, lonM, lonDM;
        private int satInView, vyska, rychlost, kurz, datumK, casK;
        private int AD1, AD2, RSSI; //basic telemetry data
        private DateTime datum;
        Thread t;
        StringBuilder sb;
        private int[] koef= new int[6];
        private int[] buffer=new int[10];
        private int ukazatel = 0;

        //public delegate void ParameterizedThreadStart(object o); 

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string s in SerialPort.GetPortNames().OrderBy(g => g))
            {
                cbPort.Items.Add(s.ToString());
                if (cbPort.Items.Count > 0) cbPort.SelectedIndex = 0;
            }
        }

        private void spClose()
        {
            if (sp != null)
                if (sp.IsOpen) sp.Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            spClose();
            if (btnSelect.Text == "Close")
            {
                btnSelect.Text = "Open";
                t.Abort();
                cbPort.Enabled = true;
                timer1.Stop();
                return;
            }

            if (cbPort.SelectedItem == null) return;
            if (!cbPort.SelectedItem.ToString().StartsWith("COM")) return;
            sp = new SerialPort(cbPort.SelectedItem.ToString(), 9600, Parity.None, 8, StopBits.One);
            try
            {
                sp.Open();
            }

            catch (Exception ex) {
                tbVystup.Text = ex.Message;
                return;
            }

            btnSelect.Text = "Close";
            tbVystup.Text = "";
            sb = new StringBuilder();

            t = new Thread(zobraz);
            t.Start();
            timer1.Start();
            cbPort.Enabled = false;
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            spClose();
        }


        void zobraz()
        {
            try
            {
                int[] byt = new int[4];
                ukazatel = 0;
                while (true)
                {
                    int b = readByte();

                    if (b == 0xFE)
                    {
                        //base telemetry block
                        //ctu dalsi tri bajty
                        AD1 = readByte();
                        AD2 = readByte();
                        RSSI = readByte();
                        continue;
                    }

                    if (b == 0xFD)
                    {
                        //user telemetry data block
                        int pocetBloku = readByte();
                        //dalsi byt se preskoci
                        readByte();
                        for (int i = 0; i < pocetBloku; i++)
                        {
                            buffer[ukazatel++] = readByte();
                        }
                    }

                    if (ukazatel < 5) continue;

                    if ((b=readBuffer())==0) continue;

                    //b = buffer[0];
                    //Console.WriteLine("Byte:{0:x}",b);
                    if ((b & 0x80) == 0x80) //horni byt
                    {
                        //zjistim adresu
                        int a = (b & 0x7F);
                        byt[0] = a;
                        if (a == 0)
                        { //ID
                            byt[1] = readBuffer();
                            byt[2] = readBuffer();
                            setID(byt);
                        }
                        if (a > 0 && a < 7) //napeti
                        {
                            byt[1] = readBuffer();
                            byt[2] = readBuffer();
                            setNapeti(byt);
                        }

                        if (a > 19 && a < 26) //koeficienty pro prepocet napeti
                        {
                            byt[1] = readBuffer();
                            byt[2] = readBuffer();
                            setKoef(byt);
                        }
                        
                        if (a > 9 && a < 13) //teplota
                        {
                            byt[1] = readBuffer();
                            byt[2] = readBuffer();
                            setTeplota(byt);
                        }

                        if (a > 31 && a < 44) //GPS
                        {
                            //32,33,35,36,41 (A0,A1,A3,A4,A9)   1 byte
                            byt[1] = readBuffer();
                            //34,37,38,39,40,42,43 (A2,A5,A6,A7,A8,AA,AB   2 byte
                            if (a == 34 || a == 37 || a == 38 || a == 39 || a == 40 || a==42 || a==43) {
                                byt[2] = readBuffer();
                            }
                            //42,43 (AA,AB)             3byte
                            if (a == 42 || a == 43) {
                                byt[3] = readBuffer();
                            }
                            setGPS(byt);
                        }
                    }
                }
                //    //nactu dalsi byte
                //    l = sp.ReadByte();
                //    int v = (h & 0x07) << 7 | (l ^ 0x80);
                //    sb.Append(String.Format("H:{0:x} L:{1:x} Address:{2:x} Value:{3:x} ({3}) \r\n", h, l, a, v));
                //    //teplota
                //    if (a > 0 & a < 6) //1-5
                //        teplota[a-1] = (decimal)v / 10;

                //    //napeti
                //    int i = a - 6;
                //    if (a > 5 & a < 12) //6-10
                //    {
                //        //nacteni koeficientu, jsou prenasena jako 3 a 4 byte
                //        h = sp.ReadByte();
                //        l = sp.ReadByte();

                //        koef[i] = h << 8 | l;

                //        if (v > 10)
                //            napeti[i] = (decimal)(2.56 / 1024 * v * koef[i]/1000);
                //        else
                //            napeti[i] = 0;

                //        if (i == 5) //palubni napeti
                //            if (v > 0)
                //                napeti[5] = (decimal)(2.56 * 2) / 1024 * v;
                //            else
                //                napeti[5] = 0;
                //    }
                //}
                //pocet++;
            }
            catch (Exception e) {
                sb.Append(e.Message);
            }
            //Console.ReadLine();
        }

        private int readBuffer()
        {
            if (ukazatel == 0) return 0;
            int b = buffer[0];
            for (int i = 0; i < buffer.Length-1; i++)
            {
                buffer[i] = buffer[i+1];
            }
            if (ukazatel>0) ukazatel--;
            //sb.Append(b.ToString("X2") + " ");
            return b;
        }

        private int readByte()
        {
            int r = sp.ReadByte();
            if (r == 0x7D)
            {
                r = sp.ReadByte();
                r ^= 0x20;
            }
            return r;
        }

        private void setGPS(int[] b)
        {
            if (b[0] == 32) latS = b[1];
            if (b[0] == 33) latM = b[1];
            if (b[0] == 34) latDM = (b[1] << 7) | b[2];
            if (b[0] == 35) lonS = b[1];
            if (b[0] == 36) lonM = b[1];
            if (b[0] == 37) lonDM = (b[1] << 7) | b[2];
            if (b[0] == 38) vyska = (b[1] << 7) | b[2];
            if (b[0] == 39) rychlost = (b[1] << 7) | b[2];
            if (b[0] == 40) kurz = (b[1] << 7) | b[2];
            if (b[0] == 41) satInView = b[1];
            if (b[0] == 42)
            {
                datumK = (b[1] << 14) | (b[2] << 7) | b[3];
            }
            if (b[0] == 43) {
                casK = (b[1] << 14) | (b[2] << 7) | b[3];
                datum = dekodujDT(datumK, casK);
            }
        }


        private void setKoef(int[] b)
        {
            koef[b[0] - 20] = (b[1] << 7) | b[2];
        }

        private void setTeplota(int[] b)
        {
            teplota[b[0] - 10] = ((decimal)((b[1] << 7) | b[2])) / 10;
        }

        private void setID(int[] b)
        {
            ID = (b[1] << 7) | b[2];
        }


        private void setNapeti(int[] b)
        {
            decimal n=(b[1] << 7) | b[2];
            if (koef[b[0] - 1] != 0)
                napeti[b[0] - 1] = (decimal)2.56 / 1024 * n * koef[b[0]-1] / 1000;
            else
                napeti[b[0] - 1]=0;

        }

        DateTime dekodujDT(long datum, long casK) {
            int rok, mesic, den;
            rok = (int)datumK;
            rok = ((rok &= 0xFE00) >> 9) + 1980;
            mesic = (int)datumK;
            mesic = (mesic &= 0x1E0) >> 5;
            den = (int)datumK;
            den = den &= 0x1F;
            int hod, min, sec;
            hod = (int)casK;
            hod = (hod&=0xF800) >> 11;
            min = (int)casK;
            min = (min &= 0x7E0) >> 5;
            sec = (int)casK;
            sec = (sec &= 0x1F) * 2;

            DateTime dt;
            
            try
            {
                dt = new DateTime(rok, mesic, den, hod, min, sec);
            }
            catch (Exception e)
            {
                return DateTime.Now;
            }

            return dt;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblID.Text = ID.ToString(); 
            lbTeplota1.Text = teplota[0].ToString("F1");
            lbTeplota2.Text = teplota[1].ToString("F1");
            lbTeplota3.Text = teplota[2].ToString("F1");
            lbNapeti1.Text = napeti[0].ToString("F");
            lbNapeti2.Text = napeti[1].ToString("F");
            lbNapeti3.Text = napeti[2].ToString("F");
            lbNapeti4.Text = napeti[3].ToString("F");
            lbNapeti5.Text = napeti[4].ToString("F");
            lbNapeti6.Text = napeti[5].ToString("F");

            lblLat.Text = String.Format("{0,2:D} {1,2:D}.{2,3:D}", latS, latM, latDM);
            lblLon.Text = String.Format("{0,2:D} {1,2:D}.{2,3:D}", lonS, lonM, lonDM);

            lblDatum.Text = String.Format("{0:F}", datum);

            lblVyska.Text = String.Format("{0:F0} m n.m.", vyska);
            lblRychlost.Text = String.Format("{0:F0} km/h", rychlost);

            string znakKurz=String.Empty;
            if (kurz > 337) znakKurz="NW";
            if (kurz < 23) znakKurz="N";
            if ((kurz >= 22) & (kurz < 68)) znakKurz="NE";
            if ((kurz >= 68) & (kurz < 113)) znakKurz="E";
            if ((kurz >= 113) & (kurz < 158)) znakKurz="SE";
            if ((kurz >= 158) & (kurz < 203)) znakKurz="S";
            if ((kurz >= 203) & (kurz < 248)) znakKurz="SW";
            if ((kurz >= 248) & (kurz < 293)) znakKurz = "W";
            if ((kurz >= 293) & (kurz < 338)) znakKurz="NW";

            lblKurz.Text = String.Format("{0:F0}°({1})", kurz, znakKurz);

            lblBaseTelemetry.Text = String.Format("{0:F0} {1:F0} {2:F0}", AD1, AD2, RSSI);

            /*if (napeti[1] == 0)
                lbNapeti2.Text = (0).ToString("F");
            else
                lbNapeti2.Text = (napeti[1] - napeti[0]).ToString("F");
            if (napeti[2] == 0)
                lbNapeti3.Text = (0).ToString("F");
            else
                lbNapeti3.Text = (napeti[2] - napeti[1]).ToString("F");
            if (napeti[3] == 0)
                lbNapeti4.Text = (0).ToString("F");
            else
                lbNapeti4.Text = (napeti[3] - napeti[2]).ToString("F");
            if (napeti[4] == 0)
                lbNapeti5.Text = (0).ToString("F");
            else
                lbNapeti5.Text = (napeti[4] - napeti[3]).ToString("F");

            for (int i = 0; i < 5; i++)
            {
                if (napeti[i] > 0)
                    lbAku.Text = (napeti[i]).ToString("F");
            }

            lbNapeti6.Text = napeti[5].ToString("F");*/
            tbVystup.Text = sb.ToString();
        }

       
    }
}
