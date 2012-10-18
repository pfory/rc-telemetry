using System;
using System.Text;
using System.IO;

namespace OffLineLogReader
{
    class Program
    {
        //input file ex. 7E 7E FE 45 5C 5D BF 00 00 00 00 7E 7E FE 45 5C 5C C1 00 00 00 00 7E 7E FE 45 5C 5C C0 00 00 00 00 7E 7E FD 03 05 81 04 63 2B 00 63 7E 7E FE 45 5C 5B C2 00 00 00 00 7E 7E FD 03 06 82 05 10 A3 0D 34 7E 7E FE 45 5C 5C C1 00 00 00 00 7E 7E FD 03 07 83 05 18 05 37 63 7E 7E FE 45 5C 5C BF 00 00 00 00 7E 7E FE 45 5C 5C C1 00 00 00 00 7E 7E
        static FileStream stream = new FileStream("c:\\temp\\data.log", FileMode.Open, FileAccess.Read, FileShare.Read);
        //output file csv ex. A0 31 A1 2B A2 06 42 A3 0D A4 17 A5 05 37 A6 02 62 A7 01 45 A8 00 00 A9 00 AA 81 13 AB 0B 35 AC 00 63 AD 00 63 AE 00 63 AF 01 81 04 71 82 05 0C 83 06 09 84 00 00 85 00 00 86 00 00 8A 01 5A 8B 00 00 8C 00 00 8D 00 00 94 06 17 4D 95 01 6B 96 78 18 
        static StreamWriter streamW = new StreamWriter("c:\\temp\\data.csv", false);

        static char[] UARTbuffU = new char[16];
        static int poziceBufferU = 0;
        static bool zapisovat = false;
        static string inputData = String.Empty;
        static string rawData = String.Empty;
        static byte[] byte1 = new byte[1];
        static byte predchoziZnak;
        static int tt = 0, pocetZnaku = 0, pozice = 0, delkaBloku = 0, poziceBufferB = 0;
        static private char[] UARTbuffB = new char[3];
        static private int priznak7D = 0;
        static long poradiZnaku = 0;


        static void Main(string[] args)
        {
            int cnt;
            streamW.WriteLine("ID;Lat;LatS;LatDS;Lon;LonS;LonDS;Alt;Speed;Azimut;Sats;Date;Time;HDOP;VDOP;PDOP;Fix;Voltage1;Voltage2;Voltage3;Voltage4;Voltage5;VoltageBoard;Temp1;Temp2;Temp3;Temp4;Pressure;Temp;Current;InputData;RawData");

            //sync();

            while ((cnt = stream.Read(byte1, 0, 1)) > 0)
            {
                poradiZnaku++;
                if (byte1[0] == 0xFE && predchoziZnak == 0x7E)
                { //pribyte1[0] zacatku bloku zakladni telemetrie
                    tt = 1;
                    pocetZnaku = 0;
                    poziceBufferB = 0;
                }


                if (byte1[0] == 0xFD && predchoziZnak == 0x7E)
                { //pribyte1[0] zacatku bloku user telemetrie
                    tt = 2;
                    pocetZnaku = 0;
                    pozice = 0;
                    delkaBloku = 0;
                }

                if (tt == 1)
                { 	//base telemetry
                    if (pocetZnaku < 3)
                    {
                        if (testPriznak7D(byte1[0]) == 0)
                        {
                            UARTbuffB[poziceBufferB++] = (char)byte1[0];
                            pocetZnaku++;
                        }
                    }
                    if (byte1[0] == 0x7E)
                    {
                        //logger.write2Log("BASE ");
                        //AD1 = UARTbuffB[0];
                        //logger.write2Log("AD1=" + AD1.ToString());
                        //AD2 = UARTbuffB[1];
                        //logger.write2Log("AD2=" + AD2.ToString());
                        //RSSI = UARTbuffB[2];
                        //logger.write2Log("RSSI=" + RSSI.ToString());
                        tt = 0;
                    }
                }
                if (tt == 2)
                { 	//user telemetry
                    if (pozice == 1) delkaBloku = byte1[0];

                    //pozice 2 se preskakuje
                    else if (pozice > 2 && pocetZnaku < delkaBloku && delkaBloku > 0)
                    {
                        if (testPriznak7D(byte1[0]) == 0)
                        {
                            UARTbuffU[poziceBufferU++] = (char)byte1[0];
                            pocetZnaku++;
                        }
                    }

                    if (byte1[0] == 0x7E)
                    {
                        zpracujFrontu();
                        tt = 0;
                    }
                }
                pozice++;
                predchoziZnak = byte1[0];

            }
        }

        static private int testPriznak7D(int znak)
        {
            if (byte1[0] == 0x7D)
            {
                priznak7D = 1;
                return 1;
            }

            if (priznak7D == 1)
            {
                //predchozi byte1[0] byl 7D, ten jsem zahodil a tento byte1[0] xoruji
                byte1[0] ^= 0x20;
                priznak7D = 0;
            }
            return 0;
        }


        //    nTice++;
        //    if (byte20[0] == 0xFE)
        //    {
        //        //base telemetry
        //    }
        //    else if (byte20[0] == 0xFD)
        //    {
        //        //user telemetry
        //        for (short i = 0; i < byte20[1]; i++)
        //        {
        //            UARTbuffU[poziceBufferU++] = (char)byte20[i + 3];
        //        }
        //        zpracujFrontu();
        //    }
        //    else
        //    {
        //        //resync
        //        sync();
        //    }
        //}

        static private void zpracujFrontu()
        {
            //zpracovani fronty


            char b;
            int[] byt = new int[4];


            while (poziceBufferU > 5)
            {
                if ((b = readBuffer()) > 0)
                {
                    if ((b & 0x80) == 0x80)
                    { //horni byt
                        int a = b;
                        byt[0] = a;
                        if (a == 0x80)
                        {//ID
                            byt[1] = readBuffer();
                            byt[2] = readBuffer();
                            Console.WriteLine("ID={1:D} ", byt[0], decode2Byt(byt).ToString());
                            if (zapisovat)
                            {
                                streamW.Write(inputData + ";");
                                streamW.Write(rawData + ";");
                                streamW.WriteLine();
                            }
                            zapisovat = true;
                            if (zapisovat)
                            {
                                streamW.Write(decode2Byt(byt) + ";");
                                inputData = String.Empty;
                                rawData = String.Empty;
                            }
                        }
                        if (a >= 0x81 && a <= 0x86)
                        {//napeti
                            byt[1] = readBuffer();
                            byt[2] = readBuffer();
                            Console.WriteLine("Napeti{0:X2}={1:D} ", byt[0], decode2Byt(byt).ToString());
                            if (zapisovat) streamW.Write(decode2Byt(byt) + ";");
                        }
                        if (a >= 0x8A && a <= 0x8D)
                        {//teplota
                            byt[1] = readBuffer();
                            byt[2] = readBuffer();
                            if (zapisovat) streamW.Write(decode2Byt(byt) + ";");
                        }
                        if (a >= 0xA0 && a <= 0xAF)
                        { //GPS
                            //32,33,35,36,41 (A0,A1,A3,A4,A9,AF)   1 byte
                            byt[1] = readBuffer();
                            if (a == 0xA0 || a == 0xA1 || a == 0xA3 || a == 0xA4
                                || a == 0xA9 || a == 0xAF)
                            {
                                Console.WriteLine("GPS[{0:X2}]={1:D} ", byt[0], byt[1].ToString());
                                if (zapisovat) streamW.Write(byt[1] + ";");
                            }

                            //34,37,38,39,40,42,43 (A2,A5,A6,A7,A8,AA,AB,AC,AD,AE   2 byte
                            if (a == 0xA2 || a == 0xA5 || a == 0xA6 || a == 0xA7
                                || a == 0xA8 || a == 0xAA || a == 0xAB || a == 0xAC
                                || a == 0xAD || a == 0xAE || a == 0xAA || a == 0xAB)
                            {
                                byt[2] = readBuffer();
                                if (a == 0xA6)
                                {
                                    Console.WriteLine("GPS[altitude]={1:D} ", byt[0], decode2Byt(byt).ToString());
                                    if (zapisovat) streamW.Write(decode2Byt(byt) + ";");
                                }
                                else if (a == 0xA7)
                                {
                                    Console.WriteLine("GPS[speed]={1:D} ", byt[0], decode2Byt(byt).ToString());
                                    if (zapisovat) streamW.Write(decode2Byt(byt) + ";");
                                }
                                else if (a == 0xA8)
                                {
                                    Console.WriteLine("GPS[azimuth]={1:D} ", byt[0], decode2Byt(byt).ToString());
                                    if (zapisovat) streamW.Write(decode2Byt(byt) + ";");
                                }
                                else if (a == 0xA9)
                                {
                                    Console.WriteLine("GPS[sats]={1:D} ", byt[0], decode2Byt(byt).ToString());
                                    if (zapisovat) streamW.Write(decode2Byt(byt) + ";");
                                }
                                else if (a == 0xAA)
                                {
                                    Console.WriteLine("GPS[date]={1:D} ", byt[0], decode2Byt(byt).ToString());
                                    if (zapisovat) streamW.Write(unpackDate(decode2Byt(byt)) + ";");
                                }
                                else if (a == 0xAB)
                                {
                                    byt[3] = readBuffer();
                                    Console.WriteLine("GPS[time]={1:D} ", byt[0], decode3Byt(byt).ToString());
                                    if (zapisovat) streamW.Write(unpackTime(decode3Byt(byt)) + ";");
                                }
                                else
                                {
                                    Console.WriteLine("GPS[{0:X2}]={1:D} ", byt[0], decode2Byt(byt).ToString());
                                    if (zapisovat) streamW.Write(decode2Byt(byt) + ";");
                                }
                            }
                        }

                        if (a == 0x94)
                        {//pressure
                            byt[1] = readBuffer();
                            byt[2] = readBuffer();
                            byt[3] = readBuffer();
                            if (zapisovat) streamW.Write(decode2Byt(byt) + ";");
                        }
                        if (a == 0x95)
                        {//temperature
                            byt[1] = readBuffer();
                            byt[2] = readBuffer();
                            if (zapisovat) streamW.Write(decode2Byt(byt) + ";");
                        }
                        if (a == 0x96)
                        {//
                            byt[1] = readBuffer();
                            byt[2] = readBuffer();
                            if (zapisovat) streamW.Write(decode2Byt(byt) + ";");
                        }
                    }
                }
            }
        }

        static private char readBuffer()
        {
            if (poziceBufferU == 0) return new char();
            char b = UARTbuffU[0];
            inputData += Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' ').ToUpper();
            for (int i = 0; i < UARTbuffU.Length - 1; i++)
            {
                UARTbuffU[i] = UARTbuffU[i + 1];
            }
            if (poziceBufferU > 0)
                poziceBufferU--;

            //zobrazData += Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' ').ToUpper();
            return b;
        }

        static int decode2Byt(int[] b)
        {
            return (b[1] << 7) | b[2];
        }

        static int decode3Byt(int[] b)
        {
            return (b[1] << 14 | b[2] << 7) | b[3]; //??????????????????
        }

        static string unpackDate(int d)
        {
            int p = d;
            string ret;
            ret = (((p &= 0xFE00) >> 9) + (int)1980).ToString();
            p = d;
            ret += "." + ((p &= 0x1E0) >> 5).ToString();
            p = d;
            ret += "." + (p &= 0x1F).ToString();
            return ret;
        }

        static string unpackTime(int c)
        {
            int p = c;
            string ret;
            ret = ((p &= 0xF800) >> 11).ToString();
            p = c;
            ret += ":" + ((p &= 0x7E0) >> 5).ToString();
            p = c;
            ret += ":" + ((p &= 0x1F) * 2).ToString();
            return ret;
        }

        //static void sync()
        //{
        //    int cnt;
        //    while ((cnt = stream.Read(byte20, 0, 1)) > 0)
        //    {
        //        if (byte20[0] == 0x7E && byte1[0]Old == 0x7E)
        //        {
        //            break;
        //        }
        //        else
        //        {
        //            //synchronize
        //            byte1[0]Old = byte20[0];
        //        }
        //    }
        //}
    }
}
