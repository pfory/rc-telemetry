using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Nini.Config;


//GPS sentence
//--------------------------------------------------------
//value           address   data1byte   data2byte data3byte 
//--------------------------------------------------------
//latitude deg    32 0xA0 10100000	0XXXXXXX
//         min    33 0xA1	10100001	00XXXXXX
//         dec    34 0xA2	10100010	00000XXX  0XXXXXXX
//longitude deg   35 0xA3	10100011	0XXXXXXX
//          min   36 0xA4	10100100	00XXXXXX
//          dec   37 0xA5	10100101	00000XXX  0XXXXXXX
//altitude    	  38 0xA6	10100110	0XXXXXXX  0XXXXXXX
//ground speed    39 0xA7	10100111	000000XX  0XXXXXXX
//azimuth         40 0xA8	10101000	000000XX  0XXXXXXX
//sat in view	    41 0xA9	10101001	0000XXXX
//date            42 0xAA	10101010	0000000X  0XXXXXXX 
//time        	  43 0xAB	10101011	0000000X  0XXXXXXX 
//--------------------------------------------------------
//                          12byte    12byte    7byte     
//      31byte                                      

//4 sensors, range -55 až +125, accuracy 1 degree, 1 sensor=8bits

//temperature sentence
//-------------------------------------------------------
//value           address    data1byte  data2byte data3byte
//-------------------------------------------------------
//1.sensor    		10 0x8A	  10001010	0000000X  0XXXXXXX
//2.sensor    		11 0x8B	  10001011	0000000X  0XXXXXXX
//3.sensor		    12 0x8C	  10001100	0000000X  0XXXXXXX
//4.sensor		    13 0x8D	  10001101	0000000X  0XXXXXXX
//-------------------------------------------------------
//                          4byte     4byte     4byte
//      12byte

//------------------
//range for single cell 0-4,2V, onboard 0-7?V send in 10bits

//voltage sentence
//-------------------------------------------------------
//value           address    data1byte data2byte data3byte
//-------------------------------------------------------
//1.cell        	01 0x81	  10000001	00000XXX  0XXXXXXX
//2.cell	        02 0x82	  10000010	00000XXX  0XXXXXXX
//3.cell	        03 0x83	  10000011	00000XXX  0XXXXXXX
//4.cell	        04 0x84	  10000100	00000XXX  0XXXXXXX
//5.cell	        05 0x85	  10000101	00000XXX  0XXXXXXX
//onboard         06 0x86	  10000110	00000XXX  0XXXXXXX
//-------------------------------------------------------
//                          6byte     6byte     6byte
//      18byte

//DOP and fix sentence
//-------------------------------------------------------
//value           address    data1byte data2byte data3byte
//-------------------------------------------------------
//HDOP		        44 0xAC	  10101100	00000XXX  0XXXXXXX
//VDOP		        45 0xAD	  10101101	00000XXX  0XXXXXXX
//PDOP		        46 0xAE	  10101110	00000XXX  0XXXXXXX
//fix		          47 0xAF	  10101111	000000XX
//-------------------------------------------------------
//                          4byte     4byte     3byte

//pressure sentence from BMP085 sensor
//-------------------------------------------------------------------------
//value           address           data1Byte data2Byte data3Byte data4Byte
//-------------------------------------------------------------------------
//pressure      	20 0x94 10010100	00000XXX  0XXXXXXX  0XXXXXXX  0XXXXXXX
//temperature     21 0x95 10010101	00000XXX  0XXXXXXX

namespace DBOX
{
  public class cService
  {
    public cService()
    {
      gps = cGPS.get_Instance();
      voltage = cVoltage.get_Instance();
      alarmNapetiClanek = new cAlarmNapetiClanek();
      temperature = cTemperature.get_Instance();
      date = new cDate();
      model = cModel.get_Instance();
      airport = cAirport.get_Instance();
      bmp085 = cBMP085.get_Instance();
      uTemperature = cUTemperature.get_Instance();

      buffer_loc = null;
    }

    private static cService uniqueInstance;
    public static cService get_Instance()
    {
      if (uniqueInstance == null) { uniqueInstance = new cService(); }
      return uniqueInstance;
    }

    public cGPS gps;
    public cVoltage voltage;
    public cAlarmNapetiClanek alarmNapetiClanek;
    public cTemperature temperature;
    public cDate date;
    public cModel model;
    public cAirport airport;
    public cBMP085 bmp085;
    public cUnits uTemperature;

    private int predchoziZnak = 0;
    private int tt = 0;
    private int pocetZnaku = 0;
    private int pozice = 0;
    private int poziceBufferB = 0;
    private int poziceBufferU = 0;
    private int delkaBloku = 0;
    private char[] UARTbuffB = new char[3];
    private char[] UARTbuffU = new char[15];
    private string zobrazData = String.Empty;
    
    private byte[] buffer_loc;
    public byte[] buffer
    {
      get { return buffer_loc; }
      set { buffer_loc = value; }
    }

    //public string sbuffer
    //{
    //  set {
    //    char[] array = value.ToCharArray();
    //    for (int i=0; i<array.Length; i++)
    //    {
    //      buffer_loc.Concat(BitConverter.GetBytes(array[i]));
    //    }
    //  }
    //}


    private int AD1;
    private int AD2;
    private int RSSI;

    private int priznak7D = 0;

    public string get_zobrazData() { return zobrazData; }
    public void set_zobrazData(string arg_zobrazData) { zobrazData = arg_zobrazData; }

    public int get_AD1() { return AD1; }
    public int get_AD2() { return AD2; }
    public int get_RSSI() { return RSSI; }

    public void decode()
    {
      foreach (byte znak in buffer)
      {
        if (znak == 0xFE && predchoziZnak == 0x7E)
        { //priznak zacatku bloku zakladni telemetrie
          tt = 1;
          pocetZnaku = 0;
          poziceBufferB = 0;
        }


        if (znak == 0xFD && predchoziZnak == 0x7E)
        { //priznak zacatku bloku user telemetrie
          tt = 2;
          pocetZnaku = 0;
          pozice = 0;
          delkaBloku = 0;
        }

        if (tt == 1)
        { 	//base telemetry
          if (pocetZnaku < 3)
          {
            if (testPriznak7D(znak) == 0)
            {
              UARTbuffB[poziceBufferB++] = (char)znak;
              pocetZnaku++;
            }
          }
          if (znak == 0x7E)
          {
            AD1 = UARTbuffB[0];
            AD2 = UARTbuffB[1];
            RSSI = UARTbuffB[2];
          }
        }
        if (tt == 2)
        { 	//user telemetry
          if (pozice == 1) delkaBloku = znak;

          if (znak == 0x7E || (delkaBloku == pocetZnaku && delkaBloku > 0))
          {
            zpracujFrontu();
            pozice = 0;
            tt = 0;
          }

          //pozice 2 se preskakuje
          if (pozice > 2)
          {
            if (testPriznak7D(znak) == 0)
            {
              UARTbuffU[poziceBufferU++] = (char)znak;
              pocetZnaku++;
            }
          }
        }
        pozice++;
        predchoziZnak = znak;

      }
    }

    private int testPriznak7D(int znak)
    {
      if (znak == 0x7D)
      {
        priznak7D = 1;
        return 1;
      }

      if (priznak7D == 1)
      {
        //predchozi znak byl 7D, ten jsem zahodil a tento znak xoruji
        znak ^= 0x20;
        priznak7D = 0;
      }
      return 0;
    }

    private void zpracujFrontu()
    {
      //zpracovani fronty


      char b;
      int[] byt = new int[3];


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
              model.set_ModelID(byt);
            }
            if (a >= 0x81 && a <= 0x86)
            {//napeti
              byt[1] = readBuffer();
              byt[2] = readBuffer();
              voltage.set_Voltage(byt);
            }
            if (a >= 0x8A && a <= 0x8D)
            {//teplota
              byt[1] = readBuffer();
              byt[2] = readBuffer();
              temperature.set_Temperature(byt);
            }
            if (a >= 0xA0 && a <= 0xAF)
            { //GPS
              //32,33,35,36,41 (A0,A1,A3,A4,A9,AF)   1 byte
              byt[1] = readBuffer();
              //34,37,38,39,40,42,43 (A2,A5,A6,A7,A8,AA,AB,AC,AD,AE   2 byte
              if (a == 0xA2 || a == 0xA5 || a == 0xA6 || a == 0xA7
                  || a == 0xA8 || a == 0xAA || a == 0xAB || a == 0xAC
                  || a == 0xAD || a == 0xAE)
              {
                byt[2] = readBuffer();
              }
              gps.set_GPS(byt);

              if (a == 0xAA)
              {
                date.set_Date(byt);
              }
              if (a == 0xAB)
              {
                date.set_Time(byt);
              }

            }
          }
        }
      }
    }

    private char readBuffer()
    {
      if (poziceBufferU == 0) return new char();
      char b = UARTbuffU[0];
      for (int i = 0; i < UARTbuffU.Length - 1; i++)
      {
        UARTbuffU[i] = UARTbuffU[i + 1];
      }
      if (poziceBufferU > 0)
        poziceBufferU--;

      zobrazData += Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' ').ToUpper();
      return b;
    }
  }

  public class cGPS
  {
    public cGPS()
    {
      yLatS_loc = 0;
      yLatM_loc = 0;
      yLatDM_loc = 0;
      yLonS_loc = 0;
      yLonM_loc = 0;
      yLonDM_loc = 0;
      vSpeed_loc = 0;
      yHDOP_loc = 0;
      yVDOP_loc = 0;
      yPDOP_loc = 0;
      yFix_loc = 0;

    }

    private static cGPS uniqueInstance;
    public static cGPS get_Instance()
    {
      if (uniqueInstance == null) { uniqueInstance = new cGPS(); }
      return uniqueInstance;
    }


    public void set_GPS(int[] b)
    {
      switch (b[0])
      {
        case 0xA0:
          yLatS = b[1];
          break;
        case 0xA1:
          yLatM = b[1];
          break;
        case 0xA2:
          yLatDM = (b[1] << 7) | b[2];
          break;
        case 0xA3:
          yLonS = b[1];
          break;
        case 0xA4:
          yLonM = b[1];
          break;
        case 0xA5:
          yLonDM = (b[1] << 7) | b[2];
          break;
        case 0xA6:
          hAlt = (b[1] << 7) | b[2];
          break;
        case 0xA7:
          vSpeed = (b[1] << 7) | b[2];
          break;
        case 0xA8:
          yAzimuth = (b[1] << 7) | b[2];
          break;
        case 0xA9:
          ySats = b[1];
          break;
        case 0xAC:
          yHDOP = (b[1] << 7) | b[2];
          break;
        case 0xAD:
          yVDOP = (b[1] << 7) | b[2];
          break;
        case 0xAE:
          yPDOP = (b[1] << 7) | b[2];
          break;
        case 0xAF:
          yFix = (short)b[1];
          break;

      }
    }

    private int yLatS_loc;
    public int yLatS
    {
      get { return Math.Abs(yLatS_loc); }
      set { yLatS_loc = value; }
    }

    private int yLatM_loc;
    public int yLatM
    {
      get { return yLatM_loc; }
      set { yLatM_loc = value; }
    }

    private int yLatDM_loc;
    public int yLatDM
    {
      get { return yLatDM_loc; }
      set { yLatDM_loc = value; }
    }

    private int yLonS_loc;
    public int yLonS
    {
      get { return Math.Abs(yLonS_loc); }
      set { yLonS_loc = value; }
    }

    private int yLonM_loc;
    public int yLonM
    {
      get { return yLonM_loc; }
      set { yLonM_loc = value; }
    }

    private int yLonDM_loc;
    public int yLonDM
    {
      get { return yLonDM_loc; }
      set { yLonDM_loc = value; }
    }

    private int yAlt_loc;
    public int hAlt
    {
      get { return yAlt_loc; }
      set {
        yAlt_loc = value;
        cAirport Airport = cAirport.get_Instance();
        cModel Model = cModel.get_Instance();
        Model.hModel = yAlt_loc - Airport.hAirportAlt;
        if (Model.hModelMax < Model.hModel)
          Model.hModelMax = Model.hModel;
      }
    }

    private int vSpeed_loc;
    public int vSpeed
    {
      get { return vSpeed_loc; }
      set {
        vSpeed_loc = value;
      }
    }

    private int yAzimuth_loc;
    public int yAzimuth
    {
      get { return yAzimuth_loc; }
      set 
      { 
        yAzimuth_loc = value;
        yAzimuthAbbr = String.Empty;
      }
    }

    private int ySats_loc;
    public int ySats
    {
      get { return ySats_loc; }
      set { ySats_loc = value; }
    }

    private int yHDOP_loc;
    public int yHDOP
    {
      get { return yHDOP_loc; }
      set { yHDOP_loc = value; }
    }

    private int yVDOP_loc;
    public int yVDOP
    {
      get { return yVDOP_loc; }
      set { yVDOP_loc = value; }
    }

    private int yPDOP_loc;
    public int yPDOP
    {
      get { return yPDOP_loc; }
      set { yPDOP_loc = value; }
    }

    private short yFix_loc;
    public short yFix
    {
      get { return yFix_loc; }
      set { yFix_loc = value; }
    }

    public string get_SN()
    {
      if (yLatS_loc >= 0)
        return "N";
      else
        return "S";
    }
    public string get_EW()
    {
      if (yLonS_loc >= 0)
        return "E";
      else
        return "W";
    }

    private string yAzimuthAbbr_loc;

    public string yAzimuthAbbr
    {
      get { return yAzimuthAbbr_loc; }
      set
      {
          if (value == String.Empty)
          {
              if (yAzimuth_loc >= 0 && yAzimuth_loc < 22)
                  yAzimuthAbbr_loc = "S";
              if (yAzimuth_loc >= 22 && yAzimuth_loc < 67)
                  yAzimuthAbbr_loc = "SV";
              if (yAzimuth_loc >= 67 && yAzimuth_loc < 112)
                  yAzimuthAbbr_loc = "V";
              if (yAzimuth_loc >= 112 && yAzimuth_loc < 157)
                  yAzimuthAbbr_loc = "JV";
              if (yAzimuth_loc >= 157 && yAzimuth_loc < 202)
                  yAzimuthAbbr_loc = "J";
              if (yAzimuth_loc >= 202 && yAzimuth_loc < 247)
                  yAzimuthAbbr_loc = "JZ";
              if (yAzimuth_loc >= 247 && yAzimuth_loc < 292)
                  yAzimuthAbbr_loc = "Z";
              if (yAzimuth_loc >= 292 && yAzimuth_loc < 337)
                  yAzimuthAbbr_loc = "SZ";
              if (yAzimuth_loc >= 337)
                  yAzimuthAbbr_loc = "S";
          }
          else
          {
              yAzimuthAbbr_loc = value;
          }
      }
    }
  }

  public class cVoltage
  {
    public cVoltage()
    {
      yNumberOfCells_loc = 0;
      uBoard_loc = 0;
      yNumberOfActualCells_loc = 0;
      for (short i = 0; i < yNumberOfCells_loc; i++)
        uCell_loc[i] = 0;
    }

    private static cVoltage uniqueInstance;
    public static cVoltage get_Instance()
    {
      if (uniqueInstance == null) { uniqueInstance = new cVoltage(); }
      return uniqueInstance;
    }

    private const float C_uCellMax = 4.2f;
    private const float C_uOrange = 4.0f;
    private const float C_uRed = 3.5f;

    public void set_Voltage(int[] b)
    {
      float nap = (b[1] << 7) | b[2];
      if (b[0] < 0x86)
        set_uCell(b[0] - 0x81, nap);
      else
        uBoard = nap;
    }

    private float[] uCell_loc;

    private static short yNumberOfCells_loc;
    public short yNumberOfCells
    { 
      get { return yNumberOfCells_loc; }
      set 
      { 
        yNumberOfCells_loc = value; 
        uCell_loc = new float[yNumberOfCells_loc];
      }
    }

    public float[] uCell
    {
      get { return uCell_loc; }
    }

    public void set_uCell(int arg_clanek, float arg_napeti)
    {
      uCell_loc[arg_clanek] = arg_napeti * 0.0041f;
      uCell_loc[arg_clanek] = (float)Math.Round((double)uCell_loc[arg_clanek], 1);
    }

    private float uBoard_loc;
    public float uBoard
    {
      get { return uBoard_loc; }
      set 
      {
        uBoard_loc = value * 0.0041f;
        uBoard_loc = (float)Math.Round((double)uBoard_loc, 1);
      }
    }

    public float get_uAku()
    {
      float uAku = 0;
      for (int i = 0; i < yNumberOfCells_loc; i++)
        uAku += uCell_loc[i];
      return uAku;
    }

    //public Color get_colorTextClanek(int arg_clanek)
    //{
    //  if (uCell_loc[arg_clanek] >= C_uOrange)
    //    return Color.Green;
    //  else if (uCell_loc[arg_clanek] >= C_uRed)
    //    return Color.Orange;
    //  else if (uCell_loc[arg_clanek] > 0)
    //    return Color.Red;
    //  else
    //    return Color.Black;
    //}

    //public Color get_colorTextPalubni()
    //{
    //  if (uBoard_loc >= C_uOrange)
    //    return Color.Green;
    //  else if (uBoard_loc >= C_uRed)
    //    return Color.Orange;
    //  else if (uBoard_loc > 0)
    //    return Color.Red;
    //  else
    //    return Color.Black;
    //}

    public int get_procAku()
    {
      return (int)(get_uAkuMaximal() - get_uAku() * 10.0f);
    }

    private int yNumberOfActualCells_loc;
    public int yNumberOfActualCells
    {
      get { return yNumberOfActualCells_loc; }
      set { yNumberOfActualCells_loc = value; }
    }

    public float get_uAkuMinimal()
    {
      return yNumberOfActualCells_loc * C_uRed;
    }

    public float get_uAkuMaximal()
    {
      return yNumberOfActualCells_loc * C_uCellMax;
    }

  }

  static class AlarmSounds
  {
    private static Stream[] stream = new Stream[5];

   static AlarmSounds()
    {
      //stream[0] = Resource1.chord;
      //stream[1] = Resource1.chimes;
      //stream[2] = Resource1.ringout;
      //stream[3] = Resource1.chord;
      //stream[4] = Resource1.chord;
    }

    public static Stream getSound(int arg_typ)
    {
      if (arg_typ <= stream.Length)
        return stream[arg_typ];
      else
        return null;
    }
  }

  public class cAlarmNapetiClanek
  {
    public cAlarmNapetiClanek()
    {
      for (short i = 0; i < C_AlarmCount; i++)
      {
        uAlarmNapeti_loc[i] = 0;
        yAlarmSound_loc[i] = 0;
      }
    }

    private const short C_AlarmCount = 3;

    private float[] uAlarmNapeti_loc = new float[C_AlarmCount];
    public float[] uAlarmNapeti
    {
      get { return uAlarmNapeti_loc; }
      set { uAlarmNapeti_loc = value; }
    }

    private int[] yAlarmSound_loc = new int[C_AlarmCount];
    public int[] yAlarmSound
    {
      get { return yAlarmSound_loc; }
      set { yAlarmSound_loc = value; }
    }

    public void makeAlarm(cVoltage napeti)
    {
      bool isAlarm1 = false;
      bool isAlarm2 = false;
      bool isAlarm3 = false;

      //comparace skutecnych napeti s nastavenymi urovnemi pro alarm
      for (int i = 0; i < napeti.yNumberOfCells; i++)
      {
        if (napeti.uCell[i] > 0)
        {
          if (napeti.uCell[i] < uAlarmNapeti[0])
            isAlarm1 = true;
          if (napeti.uCell[i] < uAlarmNapeti[1])
            isAlarm2 = true;
          if (napeti.uCell[i] < uAlarmNapeti[2])
            isAlarm3 = true;
        }
      }

      if (isAlarm1 || isAlarm2 || isAlarm3)
      {
        System.Media.SoundPlayer sp = new System.Media.SoundPlayer();

        ////prioritizace
        //if (isAlarm3)
        //    //sp.Stream = AlarmSounds.getSound(0);
        //    sp.Stream = Resource1.ringout;
        //else if (isAlarm2)
        //    //sp.Stream = AlarmSounds.getSound(1);
        //    sp.Stream = Resource1.chimes;
        //else if (isAlarm1)
        //    //sp.Stream = AlarmSounds.getSound(2);
        //    sp.Stream = Resource1.chord;

        sp.Play();
      }

    }

  }

  public class cTemperature
  {
    public cTemperature()
    {
      for (short i = 0; i<C_SensorCount; i++)
        cDallasSensor_loc[i] = 0;
    }

    private const short C_SensorCount = 4;

    private static cTemperature uniqueInstance;
    public static cTemperature get_Instance()
    {
      if (uniqueInstance == null) { uniqueInstance = new cTemperature(); }
      return uniqueInstance;
    }

    public void set_Temperature(int[] b)
    {
      float tep = (b[1] << 7) | b[2];
      set_cDallasTemperature(b[0] - 0x8A, tep);
    }

    private float[] cDallasSensor_loc = new float[C_SensorCount];
    public float[] cDallasSensor
    {
      get { return cDallasSensor_loc; }
    }

    public void set_cDallasTemperature(int ySensor_arg, float cTemperature_arg)
    {
      cDallasSensor_loc[ySensor_arg] = cTemperature_arg / 10;
    }
  }

  public class cModel
  {
    public cModel()
    {
      yModel_old = 0;
      yModel_loc = 0;
      yModelName_loc = String.Empty;
      hModel_loc = 0;
      hModelMax_loc = 0;
    }

    private static cModel uniqueInstance;
    public static cModel get_Instance()
    {
      if (uniqueInstance == null) { uniqueInstance = new cModel(); }
      return uniqueInstance;
    }

    private int yModel_old;
    private int yModel_loc; //Model ID
    public int yModel
    {
      get { return yModel_loc; }
      set 
      { 
        yModel_loc = value;
        if (yModel_loc != yModel_old)
        {
          yModelName = String.Empty;
          for (int i = 0; i < 4; i++)
            set_TeplotaCidloName(i);
          yModel_old = yModel_loc;
        }
      }
    }

    public void set_ModelID(int[] b)
    {
      yModel = (b[1] << 7) | b[2];
    }

    private string yModelName_loc;
    public string yModelName
    {
      get { return yModelName_loc; }
      set
      {
        if (value == String.Empty)
        {
          string tmp_id = yModel_loc.ToString("X");
          bool found = false;

          IConfigSource source = new IniConfigSource("\\DBox.ini");
          for (int i = 1; i <= 10; i++)
          {
            if (tmp_id == source.Configs["Models"].Get("Model " + i.ToString() + " ID"))
            {
              yModelName_loc = source.Configs["Models"].Get("Model " + i.ToString() + " name");
              found = true;
            }
          }
          if (!found)
            yModelName_loc = "--------";
        }
        else
        {
          yModelName_loc = value;
        }
      }
    }


    private void set_TeplotaCidloName(int cidlo)
    {
      string tmp_id = yModel_loc.ToString("X");
      bool found = false;
      for (int i = 1; i <= 10; i++)
      {
        IConfigSource source = new IniConfigSource("\\DBox.ini");
        if (tmp_id == source.Configs["Models"].Get("Model " + i.ToString() + " ID"))
        {
          yTemperatureSensorName_loc[cidlo] = source.Configs["Models"].Get("Model " + i.ToString() + " temperature " + (cidlo+1).ToString() + " name");
          found = true;
        }
      }
      if (!found)
        yTemperatureSensorName_loc[cidlo] = "--------";
    }


    private string[] yTemperatureSensorName_loc = new string[4];
    public string[] yTemperatureSensorName
    {
      get { return yTemperatureSensorName_loc; }
    }

    private int hModel_loc;
    public int hModel
    {
      get { return hModel_loc; }
      set { hModel_loc = value; }
    }

    private int hModelMax_loc;
    public int hModelMax
    {
      get { return hModelMax_loc; }
      set { hModelMax_loc = value; }
    }

    private int vModel_loc;
    public int vModel
    {
      get { return vModel_loc; }
      set { vModel_loc = value; }
    }

    private int vModelMax_loc;
    public int vModelMax
    {
      get { return vModelMax_loc; }
      set { vModelMax_loc = value; }
    }


  }

  public class cAirport
  {
    public cAirport()
    {
      yAirport_loc = 0;
      source = new IniConfigSource("\\DBox.ini");
      yAirportName_loc = String.Empty;
      hAirportAlt_loc = 0;
    }

    private static cAirport uniqueInstance;
    public static cAirport get_Instance()
    {
      if (uniqueInstance == null) { uniqueInstance = new cAirport(); }
      return uniqueInstance;
    }

    private int hAirportAlt_loc;
    IConfigSource source;

    public int hAirportAlt
    {
      get { return hAirportAlt_loc; }
      set {
        if (value == int.MinValue)
          hAirportAlt_loc = Convert.ToUInt16(source.Configs["Airports"].Get("Airport " + yAirport_loc.ToString() + " altitude[m]"));
        else
          hAirportAlt_loc = value;
      }
    }

    private short yAirport_loc;
    public short yAirport
    {
      get { return yAirport_loc; }
      set
      {
        yAirport_loc = value;
        yAirportName = String.Empty;
        hAirportAlt = int.MinValue;
      }
    }

    private string yAirportName_loc;
    public string yAirportName
    {
      get { return yAirportName_loc; }
      set 
      {
        if (value == String.Empty)
          yAirportName_loc = source.Configs["Airports"].Get("Airport " + yAirport_loc.ToString() + " name");
        else
          yAirportName_loc = value;
      }
    }
  }

  public class cBMP085
  {
    public cBMP085() 
    {
      pPressure_loc = 0;
      cTemperature_loc = 0;
    }

    private static cBMP085 uniqueInstance;
    public static cBMP085 get_Instance()
    {
      if (uniqueInstance == null) { uniqueInstance = new cBMP085(); }
      return uniqueInstance;
    }

    private long pPressure_loc;
    public long pPressure //Pa 101325Pa => 1013.25hPa
    {
      get { return pPressure_loc; }
      set { pPressure_loc = value; }
    }

    private float cTemperature_loc;
    public float cTemperature //°C
    {
      get { return cTemperature_loc; }
      set { cTemperature_loc = value / 10; } //235 = 23.5°C
    }
  }


  public class cDate : IComparable
  {
      private short rok;
      private short mes;
      private short den;
      private short hod;
      private short min;
      private short sec;

      public void set_Date(int[] b)
      {
          unpackDate((b[1] << 7) | b[2]);
      }

      public void set_Time(int[] b)
      {
          unpackTime((b[1] << 7) | b[2]);
      }

      public void unpackDate(int d)
      {
          int p = d;
          rok = (short)(((p &= 0xFE00) >> 9) + (int)1980);
          p = d;
          mes = (short)((p &= 0x1E0) >> 5);
          p = d;
          den = (short)(p &= 0x1F);
      }

      public void unpackTime(int c)
      {
          int p = c;
          hod = (short)((p &= 0xF800) >> 11);
          p = c;
          min = (short)((p &= 0x7E0) >> 5);
          p = c;
          sec = (short)((p &= 0x1F) * 2);
      }

      public DateTime get_datum()
      {
          DateTime dt;
          try
          {
              dt = new DateTime(rok, mes, den, hod, min, sec);
          }
          catch (ArgumentOutOfRangeException)
          {
              return new DateTime();
          }

          if (!dt.IsDaylightSavingTime())
              return dt.AddHours(1);

          return dt;
      }

      override public string ToString()
      {
          return den.ToString() + "." + mes.ToString() + "." + rok.ToString() + " " + hod.ToString() + ":" + min.ToString() + ":" + sec.ToString();
      }


      #region IComparable Members

      public int CompareTo(object obj)
      {
          if (this.get_datum() > (DateTime)obj)
              return 1;
          else if
            (this.get_datum() < (DateTime)obj)
              return -1;
          else
              return 0;
      }

      #endregion
  }

  public abstract class cUnits
  {
      //public cUnits()
      //{
      //  uUnits_loc = null;
      //  uName_loc = null;
      //  index_loc = 0;
      //}

      //public cUnits(short index)
      //{
      //  index_loc = index;
      //}

      //short index_loc;
      //public virtual short index
      //{
      //  get { return index_loc; }
      //  set { index_loc = value; }
      //}

      //int uType_loc;
      //public virtual int uType
      //{
      //  get { return uType_loc; }
      //  set { uType_loc = value; }
      //}

      //string[] uName_loc;
      //public virtual string uName
      //{
      //  get { return uName_loc[index_loc]; }
      //  set { uName_loc[index_loc] = value; }
      //}

      public abstract string uUnits
      {
          get;
      }


  }

  public class cUTemperature : cUnits
  {
      public cUTemperature()
      {
          index_loc = 0;
          uUnits_loc[0] = "°C";
          uUnits_loc[1] = "°K";
      }

      private static cUTemperature uniqueInstance;
      public static cUTemperature get_Instance()
      {
          if (uniqueInstance == null) { uniqueInstance = new cUTemperature(); }
          return uniqueInstance;
      }

      short index_loc;

      string[] uUnits_loc = new string[2];
      public override string uUnits
      {
          get { return uUnits_loc[index_loc]; }
      }
      //public cUTemperature()
      //{
      //  index = 1;
      //}

      //short index;

      //string[] uUnits_loc;
      //public override string uUnits
      //{
      //  get
      //  {
      //    return uUnits_loc[index];
      //  }
      //  set
      //  {
      //    uUnits_loc[index] = value;
      //  }
      //}
  }


}

