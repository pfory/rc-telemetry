#define color

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using Nini.Config;



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
      uDistance = cUDistance.get_Instance();
      uPressure = cUPressure.get_Instance();
      buffer_loc = null;
      loggerLog = new cLogger("DBox.log");
      loggerGPS = new cLogger("DBox" + date.getFileName() + ".nmea");

    }

    ~cService()
    {
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
    public cUnits uDistance;
    public cUPressure uPressure;

    private cLogger loggerLog;
    private cLogger loggerGPS;

    private int predchoziZnak = 0;
    private int tt = 0;
    private int pocetZnaku = 0;
    private int pozice = 0;
    private int poziceBufferB = 0;
    private int poziceBufferUHead = 0;
    private int poziceBufferUTail = 0;
    private int delkaBloku = 0;
    private char[] UARTbuffB = new char[3];
    private char[] UARTbuffU = new char[20];
    private string bufferGPS = "";


    private byte[] buffer_loc;
    public byte[] buffer
    {
      get { return buffer_loc; }
      set { buffer_loc = value; }
    }

    public int getBufferLength()
    {
      return buffer.Length;
    }


    private int AD1;
    private int AD2;
    private int RSSI;

    private int priznak7D = 0;
    private bool startGPSSentence = false;


    public int get_AD1() { return AD1; }
    public int get_AD2() { return AD2; }
    public int get_RSSI() { return RSSI; }

    public void decode()
    {
      loggerLog.timeStart[0] = new TimeSpan(DateTime.Now.Ticks);
      loggerLog.write2Log("\n\n\n");
      loggerLog.writeTimeStamp();
      loggerLog.write2Log(" Buffer:");
      loggerLog.write2Log(buffer_loc, true);

      foreach (byte znak in buffer_loc)
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
        {       //base telemetry
          if (pocetZnaku > 0)
          {
            if (pocetZnaku == 4)
            {
              loggerLog.write2Log("\n");
              loggerLog.writeTimeStamp();
              loggerLog.write2Log(" BASE");
              AD1 = UARTbuffB[0];
              loggerLog.write2Log(" AD1=");
              loggerLog.write2Log((char)AD1);
              loggerLog.write2Log((char)AD1, true);
              AD2 = UARTbuffB[1];
              loggerLog.write2Log(" AD2=");
              loggerLog.write2Log((char)AD2);
              loggerLog.write2Log((char)AD2, true);
              RSSI = UARTbuffB[2];
              loggerLog.write2Log(" RSSI=");
              loggerLog.write2Log((char)RSSI);
              loggerLog.write2Log((char)RSSI, true);
              tt = 0;
            }
            else
            {
              if (testPriznak7D(znak) == 0)
              {
                UARTbuffB[poziceBufferB++] = (char)znak;
                pocetZnaku++;
              }
            }
          }
          else
            pocetZnaku = 1;
        }

        if (tt == 2)
        {    //user telemetry
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
              UARTbuffU[poziceBufferUTail++] = (char)znak;
              pocetZnaku++;
            } 
          }
        }
        pozice++;
        predchoziZnak = znak;

      }
      loggerLog.timeEnd[0] = new TimeSpan(DateTime.Now.Ticks);
      loggerLog.write2Log("\n");
      loggerLog.writeTimeStamp();
      loggerLog.write2Log(" Decode " + loggerLog.getTime(0).ToString() + " ticks.");

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
      loggerLog.timeStart[1] = new TimeSpan(DateTime.Now.Ticks);


      char b;
      int[] byt = new int[4];

      loggerLog.write2Log("\n");
      loggerLog.writeTimeStamp();
      loggerLog.write2Log(" UARTbuffU (");

      while (poziceBufferUTail - poziceBufferUHead > 3)
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
              continue;
            }
            else if (a >= 0x81 && a <= 0x86)
            {//napeti
              byt[1] = readBuffer();
              byt[2] = readBuffer();
              voltage.set_Voltage(byt);
              continue;
            }
            else if (a >= 0x8A && a <= 0x8D)
            {//teplota
              byt[1] = readBuffer();
              byt[2] = readBuffer();
              temperature.set_Temperature(byt);
              continue;
            }
            else if (a == 0x94)
            {//pressure
              byt[1] = readBuffer();
              byt[2] = readBuffer();
              byt[3] = readBuffer();
              bmp085.pPressure = (byt[1] << 14) | (byt[2] << 7) | byt[3];
              continue;
            }
            else if (a == 0x95)
            {//temperature
              byt[1] = readBuffer();
              byt[2] = readBuffer();
              bmp085.cTemperature = (byt[1] << 7) | byt[2];
              continue;
            }
            else if (a == 0x96)
            {//current
              byt[1] = readBuffer();
              byt[2] = readBuffer();
              continue;
            }
            else if (a == 0x96) //last
            {
              continue;
            }
            else if (a == 0xA0 || a == 0xA2)
            {
              //GPS sentence start
              startGPSSentence = true;
              bufferGPS = String.Empty;
            }
            else if (a == 0xA1 || a == 0xA3)
            {
              //GPS sentence end
              decodeGPS(bufferGPS);
              startGPSSentence = false;
            }
          }
          else //data
          {
            if (startGPSSentence)
              bufferGPS += b;
          }
        }
      }
      loggerLog.write2Log(")");
      loggerLog.timeEnd[1] = new TimeSpan(DateTime.Now.Ticks);
      loggerLog.write2Log("\n");
      loggerLog.writeTimeStamp();
      loggerLog.write2Log(" Zpracuj frontu " + loggerLog.getTime(1).ToString() + " ticks.");

    }


    private void decodeGPS(string buf)
    {
      loggerLog.timeStart[2] = new TimeSpan(DateTime.Now.Ticks);
      int crc = gps.crc(buf);
      if (crc == 0)
      {
        loggerLog.write2Log("Cannot calculate CRC " + buf + "\n");
        return;
      }

      int crc1 = int.Parse(buf.Substring(buf.LastIndexOf('*') + 1), System.Globalization.NumberStyles.AllowHexSpecifier);

      if (crc != crc1)
      {
        loggerLog.write2Log("CRC error " + buf + "\n");
        return;
      }

      loggerGPS.write2Log(buf + "\n", false);
      loggerLog.write2Log("<DecodeGPS:" + buf + ">\n");
      String[] s = buf.Split(',');
      if (s[0] == "$GPGGA")
      {
        //$GPGGA,183058.489,4943.8289,N,01323.7176,E,0,00,,366.5,M,46.4,M,,0000*74
        gps.yLatS = Convert.ToInt16(s[2].Substring(0, 2));
        gps.yLatM = (int)convertToInt(s[2].Substring(2, 2));
        gps.yLatDM = (int)convertToInt(s[2].Substring(5, 3));
        date.set_Time(s[1]);
        gps.yLonS = (int)convertToInt(s[4].Substring(0, 3));
        gps.yLonM = (int)convertToInt(s[4].Substring(3, 2));
        gps.yLonDM = (int)convertToInt(s[4].Substring(6, 3)); ;
        gps.hAlt = convertToDouble(s[9]);
        gps.ySats = (int)convertToInt(s[7]);
        gps.yFix = (short)convertToInt(s[6]);
        gps.yHDOP = (int)convertToInt(s[7]);
      }

      if (s[0] == "$GPRMC")
      {
        //GPRMC,183130.490,A,4943.8267,N,01323.7295,E,0.89,227.52,190612,,,A*6C
        date.set_Time(s[1]);
        date.set_Date(s[9]);
        gps.vSpeed = convertToDouble(s[7]);
      }
      loggerLog.timeEnd[2] = new TimeSpan(DateTime.Now.Ticks);
      loggerLog.write2Log("\n");
      loggerLog.writeTimeStamp();
      loggerLog.write2Log(" Decode GPS " + loggerLog.getTime(2).ToString() + " ticks.");
    }

    private long convertToInt(string p)
    {
      long ret = 0;
      try
      {
        if (p != string.Empty)
        {
          ret = Convert.ToInt32(p.Replace(".", ","));
          return ret;
        }
      }

      catch (Exception ex)
      {
        loggerLog.write2Log(ex.Message + ":" + p);
      }

      return ret;
    }


    private double convertToDouble(string p)
    {
      double ret = 0;
      try
      {
        if (p != string.Empty)
        {
          ret = Convert.ToDouble(p.Replace(".", ","));
          return ret;
        }
      }
      catch (Exception ex)
      {
        loggerLog.write2Log(ex.Message + ":" + p);
      }

      return ret;
    }


    private char readBuffer()
    {
      if (poziceBufferUTail - poziceBufferUHead == 0)
      {
        poziceBufferUHead = poziceBufferUTail = 0;
        return new char();
      }

      char b = UARTbuffU[poziceBufferUHead++];

      if (poziceBufferUTail >= 13)
      {
        for (int i = 0; i < poziceBufferUTail - poziceBufferUHead + 1; i++)
        {
          UARTbuffU[i] = UARTbuffU[i + poziceBufferUHead];
        }
        poziceBufferUTail = poziceBufferUTail - poziceBufferUHead;
        poziceBufferUHead = 0;
      }
      //if (poziceBufferU > 0)
      //  poziceBufferU--;


      loggerLog.write2Log(b, true);

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

    private double yAlt_loc;
    public double hAlt
    {
      get { return yAlt_loc; }
      set
      {
        yAlt_loc = value;
        cAirport Airport = cAirport.get_Instance();
        cModel Model = cModel.get_Instance();
        Model.hModel = yAlt_loc - Airport.hAirportAlt;
      }
    }

    private double vSpeed_loc;
    public double vSpeed
    {
      get { return vSpeed_loc; }
      set
      {
        vSpeed_loc = value;
        cModel Model = cModel.get_Instance();
        Model.vModel = vSpeed_loc;
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

    //-----------------------------------------------------------------------------
    //
    //  Calculate checksum for GPS sentence
    //  Returns:  data checksum
    //            0 - if sentense is longer than 83 Bytes
    //
    //-----------------------------------------------------------------------------
    public int crc(string b)
    {
      if (b == string.Empty)
        return 0;
      if (b.IndexOf('*') == -1)
        return 0;

      char[] a = b.ToCharArray();
      int ks = 0;
      for (int i = 1; i < 84; i++)
      {
        if (b.Substring(i, 1) == "*")
          return ks;

        ks ^= Convert.ToInt16(a[i]);
      }
      return 0;
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
      int nap = (b[1] << 7) | b[2];
      if (b[0] < 0x86)
        set_uCell(b[0] - 0x81, nap);
      else
        uBoard = (float)nap;
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

    public void set_uCell(int arg_clanek, int arg_napeti)
    {
      uCell_loc[arg_clanek] = (float)arg_napeti * 0.0041f; //1024 = 4.2V
      uCell_loc[arg_clanek] = (float)Math.Round((double)uCell_loc[arg_clanek], 1); //rounding to 1 decimal point
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
#if color
    public System.Drawing.Color get_colorTextClanek(int arg_clanek)
    {
      if (uCell_loc[arg_clanek] >= C_uOrange)
        return Color.Green;
      else if (uCell_loc[arg_clanek] >= C_uRed)
        return Color.Orange;
      else if (uCell_loc[arg_clanek] > 0)
        return Color.Red;
      else
        return Color.Black;
    }
 
    public Color get_colorTextPalubni()
    {
      if (uBoard_loc >= C_uOrange)
        return Color.Green;
      else if (uBoard_loc >= C_uRed)
        return Color.Orange;
      else if (uBoard_loc > 0)
        return Color.Red;
      else
        return Color.Black;
    }
#endif
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
      for (short i = 0; i < C_SensorCount; i++)
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
#if color
          IConfigSource source = new IniConfigSource("\\DBox.ini");
#endif
          for (int i = 1; i <= 10; i++)
          {
#if color
            if (tmp_id == source.Configs["Models"].Get("Model " + i.ToString() + " ID"))
            {
              yModelName_loc = source.Configs["Models"].Get("Model " + i.ToString() + " name");
              found = true;
            }
#endif
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
#if color
          IConfigSource source = new IniConfigSource("\\DBox.ini");
        if (tmp_id == source.Configs["Models"].Get("Model " + i.ToString() + " ID"))
        {
          yTemperatureSensorName_loc[cidlo] = source.Configs["Models"].Get("Model " + i.ToString() + " temperature " + (cidlo + 1).ToString() + " name");
          found = true;
        }
#endif

      }
      if (!found)
        yTemperatureSensorName_loc[cidlo] = "--------";
    }


    private string[] yTemperatureSensorName_loc = new string[4];
    public string[] yTemperatureSensorName
    {
      get { return yTemperatureSensorName_loc; }
    }

    private double hModel_loc;
    public double hModel
    {
      get { return hModel_loc; }
      set
      {
        hModel_loc = value;
        if (hModelMax < hModel_loc)
          hModelMax = hModel_loc;
      }
    }

    private double hModelMax_loc;
    public double hModelMax
    {
      get { return hModelMax_loc; }
      set { hModelMax_loc = value; }
    }

    private double vModel_loc;
    public double vModel
    {
      get { return vModel_loc; }
      set
      {
        vModel_loc = value;
        if (vModelMax < vModel_loc)
          vModelMax = vModel_loc;
      }
    }

    private double vModelMax_loc;
    public double vModelMax
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
#if color
        source = new IniConfigSource("\\DBox.ini");
#endif
      yAirportName_loc = String.Empty;
      hAirportAlt_loc = 0;
    }

    private static cAirport uniqueInstance;
    public static cAirport get_Instance()
    {
      if (uniqueInstance == null) { uniqueInstance = new cAirport(); }
      return uniqueInstance;
    }

    private double hAirportAlt_loc;
#if color
      IConfigSource source;
#endif

    public double hAirportAlt
    {
      get { return hAirportAlt_loc; }
      set
      {
        if (value == int.MinValue)
#if color  
            hAirportAlt_loc = Convert.ToUInt16(source.Configs["Airports"].Get("Airport " + yAirport_loc.ToString() + " altitude[m]"));
        else
#endif
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
#if color
          yAirportName_loc = source.Configs["Airports"].Get("Airport " + yAirport_loc.ToString() + " name");
        else
#endif
          yAirportName_loc = value;
      }
    }
  }

  public class cBMP085
  {
    cBMP085()
    {
      pPressure_loc = 0;
      cTemperature_loc = 0;
      pPressure_old = 0;
      pPressureDelta = 0;
      pPressureZero = int.MinValue;
    }

    private long pPressure_old;
    private long pPressureDelta;

    private const float OnePaToCm = 8.3f;

    private static cBMP085 uniqueInstance;
    public static cBMP085 get_Instance()
    {
      if (uniqueInstance == null) { uniqueInstance = new cBMP085(); }
      return uniqueInstance;
    }

    public long sDistance
    {
      get
      {
        //1Pa = 8,3cm
        return (long)((float)pPressureDelta * OnePaToCm);
      }
    }

    public int hVyskaBaro
    {
      get
      {
        if (pPressureZero_loc != int.MinValue)
          return (int)(((pPressureZero_loc - pPressure_loc) * OnePaToCm) / 100);
        else
          return 0;
      }
    }

    private long pPressure_loc;
    public long pPressure //Pa 101325Pa => 1013.25hPa
    {
      get { return pPressure_loc; }
      set
      {
        pPressure_loc = value;
        pPressureDelta = pPressure_old - pPressure_loc;
        pPressure_old = pPressure_loc;
        if (pPressureZero_loc == int.MinValue)
          pPressureZero = pPressure_loc;
      }
    }

    private long pPressureZero_loc;
    public long pPressureZero
    {
      set
      {
        pPressureZero_loc = value;
      }
      get
      {
        return pPressureZero_loc;
      }
    }


    public decimal pPressureDecimal
    {
      get { return (decimal)pPressure_loc / 100; }
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
    private int rok;
    private short mes;
    private short den;
    private short hod;
    private short min;
    private short sec;

    private static cDate uniqueInstance;
    public static cDate get_Instance()
    {
      if (uniqueInstance == null) { uniqueInstance = new cDate(); }
      return uniqueInstance;
    }

    public void set_Date(string arg_tDate)
    {
      if (arg_tDate == string.Empty)
        return;

      try
      {
        den = Convert.ToInt16(arg_tDate.Substring(0, 2));
        mes = Convert.ToInt16(arg_tDate.Substring(2, 2));
        rok = Convert.ToInt16(arg_tDate.Substring(4, 2)) + 2000;
      }

      catch (Exception)
      {
      }

    }

    public void set_Time(string arg_tTime)
    {
      if (arg_tTime == string.Empty)
        return;

      try
      {
        hod = Convert.ToInt16(arg_tTime.Substring(0, 2));
        min = Convert.ToInt16(arg_tTime.Substring(2, 2));
        sec = Convert.ToInt16(arg_tTime.Substring(4, 2));
      }

      catch (Exception)
      {
      }

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

    public string getFileName()
    {
      return getFileName("-");
    }

    public string getFileName(string separator)
    {
      DateTime dt = DateTime.Now;
      return dt.Year.ToString("0000") + separator + dt.Month.ToString("00") + separator + dt.Day.ToString("00")
        + separator + dt.Hour.ToString("00") + separator + dt.Minute.ToString("00") + separator + dt.Second.ToString("00");
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

  public class cLogger
  {
    public cLogger(string arg_soubor)
    {
      sw = new StreamWriter(arg_soubor, true, Encoding.ASCII);
    }

    ~cLogger()
    {
    }

    //string logFile;// = DateTime.Now.Replace(@"/", @"-").Replace(@"\", @"-") + ".log";

    //FileStream fs;
    StreamWriter sw;

    public void writeTimeStamp()
    {
      write2Log(DateTime.Now.ToLocalTime().ToLongTimeString());
    }

    public void write2Log(string s, bool hex)
    {
      foreach (char c in s) {
        write2Log(c, hex);
      }
    }

    public void write2Log(string s)
    {
      write2Log(s, false);
    }

    public void write2Log(char znak)
    {
      write2Log(znak, false);
    }

    public void write2Log(char znak, bool hex)
    {
      if (hex)
        sw.Write("<" + Convert.ToString(znak, 16).PadLeft(2, '0').PadRight(2, ' ').ToUpper() + ">");
      else
        sw.Write(znak.ToString());
    }

    public void write2Log(char[] buffer)
    {
      write2Log(buffer, false);
    }

    public void write2Log(char[] buffer, bool hex)
    {
      foreach (char c in buffer)
      {
        write2Log(c.ToString());
      }
    }

    public void write2Log(byte[] buffer)
    {
      write2Log(buffer, false);
    }

    public void write2Log(byte[] buffer, bool hex)
    {
      foreach (byte b in buffer)
      {
        write2Log((char)b, hex);
      }
    }

    private TimeSpan[] timeStart_loc = new TimeSpan[10];
    public TimeSpan[] timeStart
    {
      set { timeStart_loc = value; }
      get { return timeStart_loc; }
    }


    private TimeSpan[] timeEnd_loc = new TimeSpan[10];
    public TimeSpan[] timeEnd
    {
      set { timeEnd_loc = value; }
      get { return timeEnd_loc; }
    }

    public long getTime(int index)
    {
      long dur = timeEnd[index].Ticks - timeStart[index].Ticks;

      timeEnd[index] = TimeSpan.MinValue;
      timeStart[index] = TimeSpan.MinValue;

      return dur;
    }

    /*
    public void write2Log(byte znak)
    {
      //logFile.WriteByte(znak);
      logFile.Write((char)znak);
    }
 
    public void write2Log(char znak)
    {
      write2Log(znak);
    }
 
    public void write2Log(string retezec)
    {
      foreach (char znak in retezec)
      {
        write2Log(znak);
      }
    }*/
  }




  /*public class cLogger
  {
    public cLogger()
    {
      isLogged_loc = false;
    }
 
    ~cLogger()
    {
      isLogged = false;
    }
 
    private static cLogger uniqueInstance;
    public static cLogger get_Instance()
    {
      if (uniqueInstance == null) { uniqueInstance = new cLogger(); }
      return uniqueInstance;
    }
 
    private bool logovat = false;
 
    private bool isLogged_loc;
 
    public bool isLogged
    {
      get { return isLogged_loc; }
      set {
        isLogged_loc = value;
        if (isLogged_loc)
        {
          bool newFile = false;
          if (!File.Exists(@"DBOX.csv"))
          {
            newFile = true;
          }
          logFile = new StreamWriter(@"DBOX.csv", true, Encoding.ASCII);
 
          if (newFile)
          {
            //header
            write2Log("ID;A0-Lat deg;A1-Lat min;A2-Lat dec;A3-Lon deg;A4-Lon min;A5-Lon dec;A6-Altitude;A7-Speed;A8-Azimuth;A9-Sat in view;AA,AB-Date&Time;AC-HDOP;AD-VDOP;AE-PDOP;AF-Fix;81-Cell#1;82-Cell#2;83-Cell#3;84-Cell#4;85-Cell#5;86-Board;8A-Temp#1;8B-Temp#2;8C-Temp#3;8D-Temp#4;94-Pressure;95-Temp;96-Current;Date;Time\n");
          }
 
          //logFile.AutoFlush = false;
          logFile1 = new FileStream(@"DBOX.log", FileMode.Create, FileAccess.Write, FileShare.None);
        }
        else
        {
          if (logFile != null)
          {
            logFile.Close();
          }
          if (logFile1 != null)
          {
            logFile1.Close();
          }
        }
      }
    }
 
 
    public void write2Log(byte znak)
    {
      write2Log((char)znak);
    }
 
    public void write2Log(char znak)
    {
      if (isLogged_loc)
      {
        logFile.Write(znak);
      }
    }
 
    public void write2Log(string retezec)
    {
      if (isLogged_loc)
      {
        foreach (char znak in retezec)
        {
          write2Log(znak);
        }
      }
    }
 
    public void write2Log1(byte znak)
    {
      if (isLogged_loc)
      {
        logFile1.WriteByte(znak);
      }
    }
 
    private StreamWriter logFile;
    private FileStream logFile1;
 
 
    internal void write2Log(int[] byt)
    {
      if (byt[0] == 0x80)
      {
        logovat = true;
      }
 
      if (logovat)
      {
      }
 
    }
  }
  */


  public abstract class cUnits
  {
    public abstract string uUnits
    {
      get;
    }
    public abstract string uUnitsDeko
    {
      get;
    }
    public abstract string uUnitsHekto
    {
      get;
    }
    public abstract string uUnitsKilo
    {
      get;
    }
    public abstract string uUnitsDeci
    {
      get;
    }
    public abstract string uUnitsCenti
    {
      get;
    }
    public abstract string uUnitsMili
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
    public override string uUnitsDeko
    {
      get { return String.Empty; }
    }
    public override string uUnitsHekto
    {
      get { return String.Empty; }
    }
    public override string uUnitsKilo
    {
      get { return String.Empty; }
    }
    public override string uUnitsDeci
    {
      get { return String.Empty; }
    }
    public override string uUnitsCenti
    {
      get { return String.Empty; }
    }
    public override string uUnitsMili
    {
      get { return String.Empty; }
    }

  }

  public class cUDistance : cUnits
  {
    public cUDistance()
    {
      index_loc = 0;
      uUnits_loc[0] = "m";
      uUnitsKilo_loc[0] = "km";
      uUnitsDeci_loc[0] = "dm";
      uUnitsCenti_loc[0] = "cm";
      uUnitsMili_loc[0] = "mm";
    }

    private static cUDistance uniqueInstance;
    public static cUDistance get_Instance()
    {
      if (uniqueInstance == null) { uniqueInstance = new cUDistance(); }
      return uniqueInstance;
    }

    short index_loc;

    string[] uUnits_loc = new string[1];
    public override string uUnits
    {
      get { return uUnits_loc[index_loc]; }
    }
    public override string uUnitsDeko
    {
      get { return String.Empty; }
    }
    public override string uUnitsHekto
    {
      get { return String.Empty; }
    }
    string[] uUnitsKilo_loc = new string[1];
    public override string uUnitsKilo
    {
      get { return String.Empty; }
    }
    string[] uUnitsDeci_loc = new string[1];
    public override string uUnitsDeci
    {
      get { return String.Empty; }
    }
    string[] uUnitsCenti_loc = new string[1];
    public override string uUnitsCenti
    {
      get { return String.Empty; }
    }
    string[] uUnitsMili_loc = new string[1];
    public override string uUnitsMili
    {
      get { return String.Empty; }
    }
  }
  public class cUPressure : cUnits
  {

    public cUPressure()
    {
      index_loc = 0;
      uUnits_loc[0] = "Pa";
      uUnitsKilo_loc[0] = "kPa";
      uUnitsHekto_loc[0] = "hPa";
    }

    private static cUPressure uniqueInstance;
    public static cUPressure get_Instance()
    {
      if (uniqueInstance == null) { uniqueInstance = new cUPressure(); }
      return uniqueInstance;
    }

    short index_loc;

    string[] uUnits_loc = new string[1];
    public override string uUnits
    {
      get { return uUnits_loc[index_loc]; }
    }
    public override string uUnitsDeko
    {
      get { return String.Empty; }
    }
    string[] uUnitsHekto_loc = new string[1];
    public override string uUnitsHekto
    {
      get { return String.Empty; }
    }
    string[] uUnitsKilo_loc = new string[1];
    public override string uUnitsKilo
    {
      get { return String.Empty; }
    }
    public override string uUnitsDeci
    {
      get { return String.Empty; }
    }
    public override string uUnitsCenti
    {
      get { return String.Empty; }
    }
    public override string uUnitsMili
    {
      get { return String.Empty; }
    }
  }
}
