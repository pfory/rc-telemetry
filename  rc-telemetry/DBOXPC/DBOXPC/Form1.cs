using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SpeechLib;
using DBOX;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            service = cService.get_Instance();
        }

        private cService service;

        bool isLogged = true;
        SpeechVoiceSpeakFlags svsf = SpeechVoiceSpeakFlags.SVSFlagsAsync;
        SpVoice voice = new SpVoice();
        private int byteCount = 0;

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (!comport.IsOpen) return;

            int bytes = comport.BytesToRead;
            byteCount += bytes;

            // Create a byte array buffer to hold the incoming data
            byte[] buffer = new byte[bytes];

            // Read the data from the port and store it in our buffer
            comport.Read(buffer, 0, bytes);

            service.buffer = buffer;
            ddShow();

            // Show the user the incoming data in hex format
            if (isLogged)
                Log(ByteArrayToHexString(buffer));

        }

        /// <summary> Log data to the terminal window. </summary>
        /// <param name="msgtype"> The type of message to be written. </param>
        /// <param name="msg"> The string containing the message to be shown. </param>
        private void Log(string msg)
        {
            rtfTerminal.Invoke(new EventHandler(delegate
            {
                for (int i = 0; i < msg.Length; i+=3)
                {
                    rtfTerminal.SelectionColor = Color.Black;
                    rtfTerminal.SelectionFont = new Font(rtfTerminal.SelectionFont, FontStyle.Regular);
                    string s = msg[i].ToString() + msg[i + 1].ToString();
                    if (s == "7E")
                    {
                        rtfTerminal.SelectionColor = Color.Silver;
                        rtfTerminal.SelectionFont = new Font(rtfTerminal.SelectionFont, FontStyle.Strikeout);
                    }
                    else if (s == "FD")
                    {
                        rtfTerminal.SelectionColor = Color.Orange;
                        rtfTerminal.SelectionFont = new Font(rtfTerminal.SelectionFont, FontStyle.Strikeout);
                    }
                    else
                        rtfTerminal.SelectionColor = Color.Black;

                    if (s == "A0" || s == "A1" || s == "A2" || s == "A3" || 
                        s == "A4" || s == "A5" || s == "A6" || s == "A7" || 
                        s == "A8" || s == "A9" || s == "AA" || s == "AB" ||
                        s == "AC" || s == "AD" || s == "AE" || s == "AF")
                        rtfTerminal.SelectionColor = Color.Red;


                    rtfTerminal.AppendText(s + " ");
                }
                rtfTerminal.ScrollToCaret();
            }));
        }

        /// <summary> Converts an array of bytes into a formatted string of hex digits (ex: E4 CA B2)</summary>
        /// <param name="data"> The array of bytes to be translated into a string of hex digits. </param>
        /// <returns> Returns a well formatted string of hex digits with spacing. </returns>
        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            //dataDecode(sb);
            return sb.ToString().ToUpper();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!comport.IsOpen)
            {
                comport.Open();
            }
            //service.set_buffer(new StringBuilder("S7efe445b61c4000000007e7efe445b61c4000000007e7efe445b61c3000000007e"));
            //ddShow();
            System.Media.SoundPlayer sp = new System.Media.SoundPlayer(Resource1.chimes);
            sp.Play();
            voice.Rate = -2;
            voice.Volume = 100;
            //voice.Speak("Hi Peter. How are you today?", svsf);

            service.airport.yAirport = 1;
            service.bmp085.pPressure = 101325;
            service.bmp085.cTemperature = 235;
            service.voltage.yNumberOfCells = 5;
            service.voltage.yNumberOfActualCells = 3; //TODO doplnit dynamicke zjisteni

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (comport.IsOpen)
            {
                comport.Close();
            }

        }

        private void ddShow()
        {
            service.decode();
            labelAD1.Invoke(new EventHandler(delegate
                {
                    labelAD1.Text = service.get_AD1().ToString();
                }));
            trackBarAD1.Invoke(new EventHandler(delegate
                {
                    trackBarAD1.Value = service.get_AD1();
                }));

            labelAD2.Invoke(new EventHandler(delegate
            {
                labelAD2.Text = service.get_AD2().ToString();
            }));
            trackBarAD2.Invoke(new EventHandler(delegate
            {
                trackBarAD2.Value = service.get_AD2();
            }));

            labelRSSI.Invoke(new EventHandler(delegate
            {
                labelRSSI.Text = service.get_RSSI().ToString();
            }));
            trackBarRSSI.Invoke(new EventHandler(delegate
            {
                trackBarRSSI.Value = service.get_RSSI();
                if (service.get_AD1() > 100)
                {
                    trackBarRSSI.BackColor = Color.FromKnownColor(KnownColor.Green);
                }
                else
                {
                    trackBarRSSI.BackColor = Color.FromKnownColor(KnownColor.Red);
                }
            }));

            labelID.Invoke(new EventHandler(delegate
            {
                labelID.Text = service.model.yModel.ToString("X");
            }));

            labelLat.Invoke(new EventHandler(delegate
            {
                labelLat.Text = service.gps.yLatS + "°";
                labelLat.Text += service.gps.yLatM + ".";
                labelLat.Text += service.gps.yLatDM;
            }));

            labelLat1.Invoke(new EventHandler(delegate
            {
                labelLat1.Text = service.gps.yLatS + "°";
                labelLat1.Text += service.gps.yLatM + ".";
                labelLat1.Text += service.gps.yLatDM;
            }));

            labelLon.Invoke(new EventHandler(delegate
            {
                labelLon.Text = service.gps.yLonS + "°";
                labelLon.Text += service.gps.yLonM + ".";
                labelLon.Text += service.gps.yLonDM;
            }));

            labelAlt.Invoke(new EventHandler(delegate
            {
                labelAlt.Text = service.model.hModel.ToString();
            }));

            labelSpeed.Invoke(new EventHandler(delegate
            {
                labelSpeed.Text = service.model.vModel.ToString();
            }));

            labelSpeedMax.Invoke(new EventHandler(delegate
            {
                labelSpeedMax.Text = service.model.vModelMax.ToString();
            }));

            labelVyska.Invoke(new EventHandler(delegate
            {
                labelVyska.Text = service.model.hModel.ToString();
            }));

            labelVyskaMax.Invoke(new EventHandler(delegate
            {
                labelVyskaMax.Text = service.model.hModelMax.ToString();
            }));

            labelSatInView.Invoke(new EventHandler(delegate
            {
                labelSatInView.Text = service.gps.ySats.ToString();
            }));

            labelAzimut.Invoke(new EventHandler(delegate
            {
                labelAzimut.Text = service.gps.yAzimuth.ToString();
            }));

            labelAzimut1.Invoke(new EventHandler(delegate
            {
                labelAzimut1.Text = service.gps.yAzimuth.ToString();
            }));

            labelHDOP.Invoke(new EventHandler(delegate
            {
                labelHDOP.Text = service.gps.yHDOP.ToString();
            }));

            labelVDOP.Invoke(new EventHandler(delegate
            {
                labelVDOP.Text = service.gps.yVDOP.ToString();
            }));

            labelHDOP.Invoke(new EventHandler(delegate
            {
                labelPDOP.Text = service.gps.yHDOP.ToString();
            }));

            labelFix.Invoke(new EventHandler(delegate
            {
                labelFix.Text = service.gps.yFix.ToString();
            }));

            labelNapetiCl1.Invoke(new EventHandler(delegate
            {
                labelNapetiCl1.Text = service.voltage.uCell[0].ToString();
            }));

            labelNapetiCl2.Invoke(new EventHandler(delegate
            {
                labelNapetiCl2.Text = service.voltage.uCell[1].ToString();
            }));

            labelNapetiCl3.Invoke(new EventHandler(delegate
            {
                labelNapetiCl3.Text = service.voltage.uCell[2].ToString();
            }));

            labelNapetiCl4.Invoke(new EventHandler(delegate
            {
                labelNapetiCl4.Text = service.voltage.uCell[3].ToString();
            }));

            labelNapetiCl5.Invoke(new EventHandler(delegate
            {
                labelNapetiCl5.Text = service.voltage.uCell[4].ToString();
            }));

            labelNapetiPalubni.Invoke(new EventHandler(delegate
            {
                labelNapetiPalubni.Text = service.voltage.uBoard.ToString();
            }));

            labelTeplota1.Invoke(new EventHandler(delegate
            {
                labelTeplota1.Text = service.temperature.cDallasSensor[0].ToString();
            }));

            labelTeplota2.Invoke(new EventHandler(delegate
            {
                labelTeplota2.Text = service.temperature.cDallasSensor[1].ToString();
            }));

            labelTeplota3.Invoke(new EventHandler(delegate
            {
                labelTeplota3.Text = service.temperature.cDallasSensor[2].ToString();
            }));

            labelTeplota4.Invoke(new EventHandler(delegate
            {
                labelTeplota4.Text = service.temperature.cDallasSensor[3].ToString();
            }));

            labelTlak.Invoke(new EventHandler(delegate
            {
                labelTlak.Text = service.bmp085.pPressureDecimal.ToString();
            }));

            labelTeplota.Invoke(new EventHandler(delegate
            {
                labelTeplota.Text = service.bmp085.cTemperature.ToString();
            }));

            labelProud.Invoke(new EventHandler(delegate
            {
                labelProud.Text = "???";
            }));


            labelDatum.Invoke(new EventHandler(delegate
            {
                labelDatum.Text = service.date.get_datum().ToLongDateString() + " " + service.date.get_datum().ToLocalTime().ToLongTimeString();
            }));

            label45.Invoke(new EventHandler(delegate
                {
                    label45.Text = service.date.get_casString();
                }));



            richTextBoxBuffer.Invoke(new EventHandler(delegate
            {
                if (isLogged)
                {
                    string msg = service.get_zobrazData();
                    service.set_zobrazData(String.Empty);

                    for (int i = 0; i < msg.Length; i += 3)
                    {
                        string s = msg[i].ToString() + msg[i + 1].ToString();

                        if (s == "A0" || s == "A1" || s == "A2" || s == "A3" ||
                            s == "A4" || s == "A5" || s == "A6" || s == "A7" ||
                            s == "A8" || s == "A9" || s == "AA" || s == "AB" ||
                            s == "AC" || s == "AD" || s == "AE" || s == "AF")

                            richTextBoxBuffer.SelectionColor = Color.Red;
                        else if (s == "8A" || s == "8B" || s == "8C" || s == "8D")
                            richTextBoxBuffer.SelectionColor = Color.Blue;
                        else if (s == "81" || s == "82" || s == "83" || s == "84" ||
                            s == "85" || s == "86")
                            richTextBoxBuffer.SelectionColor = Color.Orange;
                        else if (s == "94" || s == "95" || s== "96")
                            richTextBoxBuffer.SelectionColor = Color.Green;
                        else if (s == "80")
                            richTextBoxBuffer.SelectionColor = Color.Purple;
                        else
                            richTextBoxBuffer.SelectionColor = Color.Black;


                        richTextBoxBuffer.AppendText(s + " ");
                    }

                    richTextBoxBuffer.ScrollToCaret();
                }
            }));

        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            isLogged = !isLogged;
        }

        private void buttonSetVyskaLetiste_Click(object sender, EventArgs e)
        {
            service.airport.hAirportAlt = 0;
        }

        private void buttonResetMax_Click(object sender, EventArgs e)
        {
            service.model.hModelMax = 0;
            service.model.vModelMax = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            rtfTerminal.Text = "";
            richTextBoxBuffer.Text = "";
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            if (n.Value <= numericUpDownNapetiClanek2.Value)
                numericUpDownNapetiClanek2.Value = n.Value - (decimal)0.1;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            if (n.Value <= numericUpDownNapetiClanek3.Value && numericUpDownNapetiClanek3.Value>(decimal)0.0)
                numericUpDownNapetiClanek3.Value = n.Value - (decimal)0.1;
            if (n.Value >= numericUpDownNapetiClanek1.Value && numericUpDownNapetiClanek1.Value<(decimal)4.2)
                numericUpDownNapetiClanek1.Value = n.Value + (decimal)0.1;
            if (n.Value == numericUpDownNapetiClanek1.Value)
                n.Value = n.Value - (decimal)0.1;
            if (n.Value == numericUpDownNapetiClanek3.Value)
                n.Value = n.Value + (decimal)0.1;

        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            if (n.Value >= numericUpDownNapetiClanek2.Value && numericUpDownNapetiClanek2.Value<(decimal)4.2)
                numericUpDownNapetiClanek2.Value = n.Value + (decimal)0.1;

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            service.alarmNapetiClanek.makeAlarm(service.voltage);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            service.alarmNapetiClanek.uAlarmNapeti[0] = (float)numericUpDownNapetiClanek1.Value;
            service.alarmNapetiClanek.uAlarmNapeti[1] = (float)numericUpDownNapetiClanek2.Value;
            service.alarmNapetiClanek.uAlarmNapeti[2] = (float)numericUpDownNapetiClanek3.Value;
            service.alarmNapetiClanek.yAlarmSound[0] = comboBoxZvukClanek1.SelectedIndex;
            service.alarmNapetiClanek.yAlarmSound[1] = comboBoxZvukClanek2.SelectedIndex;
            service.alarmNapetiClanek.yAlarmSound[2] = comboBoxZvukClanek3.SelectedIndex;
        }

        private void button4_Click(object sender, EventArgs e)
        {
//            voice.Speak("Altitude 356 meters");
//            voice.Speak("Speed 125 kilometer per hour");
            voice.Speak("Cell #1 is too low");
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label46.Text = byteCount.ToString() + "B/s";
            byteCount = 0;
        }

    }
}
