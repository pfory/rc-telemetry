using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LogDecoder
{
    class Program
    {
        static FileStream stream = new FileStream("c:\\temp\\data.log", FileMode.Open, FileAccess.Read, FileShare.None);
        static byte[] byte85 = new byte[85];
        static StreamWriter streamW = new StreamWriter("c:\\temp\\data.csv", false);

        static void Main(string[] args)
        {
            int cnt = 0;

            sync();

            while ((cnt = stream.Read(byte85, 0, 85)) > 0)
            {
                for (int i=0; i<85; i++)
                    streamW.Write(byte85[i].ToString("X") + ";");
                streamW.WriteLine();
            }

            streamW.Close();
        }

        static void sync()
        {
            int cnt;
            while ((cnt = stream.Read(byte85, 0, 1)) > 0)
            {
                if (byte85[0] == 0x80)
                {
                    break;
                }
                else
                {
                    //synchronize
                }
            }
        }

    }
}
