﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
  class Program
  {
    static void Main(string[] args)
    {
      byte[] buf = null;
      
      string retezec = "7E7EFD0315";
      retezec += "801F7FA900007E7EFE445A50A4000000007E7EFE445A50A4000000007E7EFD0416";
      retezec += "A071";
      retezec += "A17C00007E7EFE445A50A4000000007E7EFE435A50A3000000007E7EFE445A50A4000000007E7EFD0517";
      retezec += "A20000";
      retezec += "A300007E7EFE435A50A3000000007E7EFE445A51";
      retezec += "A4000000007E7EFE435";
      retezec += "A50A3000000007E7EFD0518A400A50000007E7EFE435A50A4000000007E7EFE445A50A4000000007E7EFE445A50A4000000007E7EFD0619";
      retezec += "A60000";
      retezec += "A700007E7EFE435A50A4000000007E7EFD051A";
      retezec += "A80000";
      retezec += "A900007E7EFE445A50A5000000007E7EFE435A50A5000000007E7EFE435A50A5000000007E7EFD061B";
      retezec += "AA0000";
      retezec += "AB00007E7EFE435A50A6000000007E7EFE445A50A4000000007E7EFE445A50A5000000007E7EFD061C";
      retezec += "AC0000";
      retezec += "AD00007E7EFE445A50A4000000007E7EFE435A50A5000000007E7EFE445A50A4000000007E7EFD051DAE0000";
      retezec += "AF00007E7EFE445A50A4000000007E7EFE445A50A3000000007E7EFE435A4FA3000000007E7EFE435A50A4000000007E7EFE435A50A3000000007E7EFD031E";
      retezec += "8103500000007E7EFE445A50A4000000007E7EFE445A50A3000000007E7EFE445A50A3000000007E7EFD031F";
      retezec += "820356A300007E7EFE435A50A4000000007E7EFE435A50A3000000007E7EFE435A50A4000000007E7EFD0300";
      retezec += "83035F0000007E7EFE435A50A3000000007E7EFE435A50A4000000007E7EFE435A50A3000000007E7EFD0301";
      retezec += "840000A700007E7EFE435A50A3000000007E7EFE445A50A4000000007E7EFD0302";
      retezec += "850000A900007E7EFE435A50A3000000007E7EFE435A51A4000000007E7EFE435A50A3000000007E7EFD0303";
      retezec += "86043";
      retezec += "8AB00007E7EFE435A50A4000000007E7EFE435A50A3000000007E7EFE435A50A3000000007E7EFD03048A016FAD00007E7EFE445A50A4000000007E7EFE445A50A2000000007E7EFE445A50A4000000007E7EFD0305";
      retezec += "8B0000AF00007E7EFE435A50A3000000007E7EFE435A50A4000000007E7EFD0206";
      retezec += "8C00500000007E7EFE445A50A3000000007E7EFD0107000356A300007E7EFE445A50A3000000007E7EFE445A51A4000000007E7EFD0308";
      retezec += "8D00000000007E7EFE445A50A2000000007E7EFE435A51A4000000007E7EFE435A51A3000000007E7EFD0609";
      retezec += "9406174D";
      retezec += "95007E7EFD0209016B000095007E7EFE435A50A4000000007E7EFE435A50A3000000007E7EFD030A";
      retezec += "967818A900007E7EFE445A51A3000000007E7EFE435A51A3000000007E7EFE445A51A3000000007E7EFD030B"; 

      for (int i=0; i < retezec.Length; i=i+2)
      {
        buf[i] = Convert.ToByte(retezec[i] + retezec[i + 1]);
      }
    }
  }
}
