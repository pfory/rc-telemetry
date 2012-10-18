using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DekodovaniDatUART
{
  class Program
  {
    static void Main(string[] args)
    {
      BinaryReader binReader =
   new BinaryReader(File.Open(@"..\..\data.log", FileMode.Open));
      byte znak, znakPred=0;
      try
      {
        while ((znak = binReader.ReadByte()) >= 0)
        {
          //Console.Write("{0:X}", znak);
          if (znak == 0xFD && znakPred == 0x7E)
          {
            int pocet = binReader.ReadByte();
            if (pocet > 0)
            {
              Console.Write('.');
              for (int i = 0; i <= pocet; i++)
              {
                znak = binReader.ReadByte();
                if (i > 0)
                {
                  Console.Write((char)znak);
                }
              }
              Console.Write('.');
              //Console.WriteLine();
            }
          }
          znakPred = znak;
        }
      }

      catch (EndOfStreamException)
      {
        Console.WriteLine("END OF FILE");
      }
      Console.ReadLine();
    }
  };
}

