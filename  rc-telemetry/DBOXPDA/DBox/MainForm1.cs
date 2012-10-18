using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using Nini.Config;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System.Net.Sockets;
using InTheHand.Net.Ports;

namespace DBOX
{
  public partial class Form2 : Form
  {
    public Form2()
    {
      InitializeComponent();
      Check_Configs();
      service = cService.get_Instance();
    }


    private cService service;
    int dataReceived = 0;
    private BluetoothClient bc;  
 
    private void comport_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
      //if (cbTestData.Checked) return;
      if (!comport.IsOpen) return;

      dataReceived = 2;

      int bytes = comport.BytesToRead;
      //labelDataReceived.Text = bytes.ToString();

      // Create a byte array buffer to hold the incoming data
      byte[] buffer = new byte[bytes];

      // Read the data from the port and store it in our buffer
      comport.Read(buffer, 0, bytes);


      service.buffer = buffer;
      service.decode(); 
      //ddShow();

    }

    /// <summary> Converts an array of bytes into a formatted string of hex digits (ex: E4 CA B2)</summary>
    /// <param name="data"> The array of bytes to be translated into a string of hex digits. </param>
    /// <returns> Returns a well formatted string of hex digits with spacing. </returns>
    private string ByteArrayToHexString(byte[] data)
    {
      StringBuilder sb = new StringBuilder(data.Length * 3);
      foreach (byte b in data)
        sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
      return sb.ToString().ToUpper();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      
      foreach (string s in SerialPort.GetPortNames())
      {
        serialPorts.Items.Add(s);
      }

      IConfigSource conf = new IniConfigSource("\\DBox.ini");

      comport.PortName = conf.Configs["Port"].Get("COM Port");
      comport.BaudRate = Convert.ToInt32(conf.Configs["Port"].Get("Baud", "9600"));
      comport.DataBits = Convert.ToInt32(conf.Configs["Port"].Get("Data size", "8"));
      Parity par;

      switch (conf.Configs["Port"].Get("Parity", "None"))
      {
        case "None":
          par = Parity.None;
          break;
        case "Even":
          par = Parity.Even;
          break;
        case "Mark":
          par = Parity.Mark;
          break;
        case "Odd":
          par = Parity.Odd;
          break;
        case "Space":
          par = Parity.Space;
          break;
        default:
          par = Parity.None;
          break;
      }
      comport.Parity = par;

      if (!comport.IsOpen)
        try
        {
          comport.Open();
        }
        catch (Exception ex)
        {
          labelTextLastMessage.Text = ex.Message;
        }

      System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
      string[] names = this.GetType().Assembly.GetManifestResourceNames();
      foreach (string n in names)
        textBox1.Text += n;

      //System.IO.Stream stream = a.GetManifestResourceStream("DBox.Properties.chimes.wav");
      //SoundPlayer player = new SoundPlayer(stream);
      //player.Play();

      //SoundPlayer sPlayer = new SoundPlayer();
      //player.Stream = DBox.Properties.Resources.chimes;
      //player.Play();
      service.airport.yAirport = 1;
      service.bmp085.pPressure = 101325;
      service.bmp085.cTemperature = 235;
      service.voltage.yNumberOfCells = 5;
      service.voltage.yNumberOfActualCells = 3; //TODO doplnit dynamicke zjisteni

      BluetoothRadio radio = BluetoothRadio.PrimaryRadio;
      //if (radio != null && radio.Mode == RadioMode.PowerOff)
      //{
      //  BluetoothRadio.PrimaryRadio.Mode = RadioMode.Connectable;
      //}

      //bc = new BluetoothClient();
      //BluetoothSerialPort sp = new BluetoothSerialPort
      //BluetoothService.SerialPort = Guid.
      //getBTInfo();
    }

    private void Check_Configs()
    {
      if (!File.Exists("\\DBox.ini"))
      {
        IniConfigSource srce = new IniConfigSource();

        IConfig config = srce.AddConfig("Port");
        config.Set("COM Port", "COM2");
        config.Set("Baud", 9600);
        config.Set("Data size", 8);
        config.Set("Parity", "None");

        config = srce.AddConfig("Airports");
        config.Set("Airport 1 altitude[m]", 0);
        config.Set("Airport 1 lat", "");
        config.Set("Airport 1 lon", "");
        config.Set("Airport 1 name", "");
        config.Set("Airport 2 altitude[m]", 0);
        config.Set("Airport 2 lat", "");
        config.Set("Airport 2 lon", "");
        config.Set("Airport 2 name", "");
        config.Set("Airport 3 altitude[m]", 0);
        config.Set("Airport 3 lat", "");
        config.Set("Airport 3 lon", "");
        config.Set("Airport 3 name", "");
        config.Set("Airport 4 altitude[m]", 0);
        config.Set("Airport 4 lat", "");
        config.Set("Airport 4 lon", "");
        config.Set("Airport 4 name", "");
        config.Set("Airport 5 altitude[m]", 0);
        config.Set("Airport 5 lat", "");
        config.Set("Airport 5 lon", "");
        config.Set("Airport 5 name", "");
        config.Set("Airport 6 altitude[m]", 0);
        config.Set("Airport 6 lat", "");
        config.Set("Airport 6 lon", "");
        config.Set("Airport 6 name", "");
        config.Set("Airport 7 altitude[m]", 0);
        config.Set("Airport 7 lat", "");
        config.Set("Airport 7 lon", "");
        config.Set("Airport 7 name", "");
        config.Set("Airport 8 altitude[m]", 0);
        config.Set("Airport 8 lat", "");
        config.Set("Airport 8 lon", "");
        config.Set("Airport 8 name", "");
        config.Set("Airport 9 altitude[m]", 0);
        config.Set("Airport 9 lat", "");
        config.Set("Airport 9 lon", "");
        config.Set("Airport 10 name", "");
        config.Set("Airport 10 lat", "");
        config.Set("Airport 10 lon", "");
        config.Set("Airport 10 name", "");

        config = srce.AddConfig("Models");
        config.Set("Model 1 name", "");
        config.Set("Model 1 ID", 0);
        config.Set("Model 2 name", "");
        config.Set("Model 2 ID", 0);
        config.Set("Model 3 name", "");
        config.Set("Model 3 ID", 0);
        config.Set("Model 4 name", "");
        config.Set("Model 4 ID", 0);
        config.Set("Model 5 name", "");
        config.Set("Model 5 ID", 0);
        config.Set("Model 6 name", "");
        config.Set("Model 6 ID", 0);
        config.Set("Model 7 name", "");
        config.Set("Model 7 ID", 0);
        config.Set("Model 8 name", "");
        config.Set("Model 8 ID", 0);
        config.Set("Model 9 name", "");
        config.Set("Model 9 ID", 0);
        config.Set("Model 10 name", "");
        config.Set("Model 10 ID", 0);


        config = srce.AddConfig("Logging");
        config.Set("FilePath", "\\DBox.log");

        srce.Save("DBox.ini");

      }
    }

    private void Form1_Closing(object sender, CancelEventArgs e)
    {
      if (comport.IsOpen)
        comport.Close();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }


    //private void timer1_Tick(object sender, EventArgs e)
    //{
    //  textBox1.Text += s;
    //  s = String.Empty;
    //  label1.Text = readBytes.ToString();
    //}

    //private void button2_Click(object sender, EventArgs e)
    //{
    //  timer1.Enabled = !timer1.Enabled;
    //}

    //private void button3_Click(object sender, EventArgs e)
    //{
    //  textBox1.Text = String.Empty;
    //}

    //private void panel1_Click(object sender, EventArgs e)
    //{
    //  tabControl1.SelectedIndex = 2;
    //}

    //private void tabPage1_Click(object sender, EventArgs e)
    //{

    //}

    //private void button4_Click(object sender, EventArgs e)
    //{
    //  serialPort1.WriteLine("Testovaci retezec");
    //}

    private void ddShow()
    {
      Color colorActive = Color.White;
      Color colorActiveHeader = Color.Yellow;

      if (tabControl1.SelectedIndex == 0) {
        labelNapetiPalubni.Invoke(new EventHandler(delegate
        {
          labelNapetiPalubni.Text = service.voltage.uBoard.ToString("F1").Replace(',', '.');
          labelNapetiPalubni.ForeColor = service.voltage.get_colorTextPalubni();
        }));
        labelNapetiClanek1.Invoke(new EventHandler(delegate
        {
          labelNapetiClanek1.Text = service.voltage.uCell[0].ToString("F1").Replace(',', '.');
          labelNapetiClanek1.ForeColor = service.voltage.get_colorTextClanek(0);
        }));
        labelNapetiClanek2.Invoke(new EventHandler(delegate
        {
          labelNapetiClanek2.Text = service.voltage.uCell[1].ToString("F1").Replace(',', '.');
          labelNapetiClanek2.ForeColor = service.voltage.get_colorTextClanek(1);
        }));
        labelNapetiClanek3.Invoke(new EventHandler(delegate
        {
          labelNapetiClanek3.Text = service.voltage.uCell[2].ToString("F1").Replace(',', '.');
          labelNapetiClanek3.ForeColor = service.voltage.get_colorTextClanek(2);
        }));
        labelNapetiClanek4.Invoke(new EventHandler(delegate
        {
          labelNapetiClanek4.Text = service.voltage.uCell[3].ToString("F1").Replace(',', '.');
          labelNapetiClanek4.ForeColor = service.voltage.get_colorTextClanek(3);
        }));
        labelNapetiClanek5.Invoke(new EventHandler(delegate
        {
          labelNapetiClanek5.Text = service.voltage.uCell[4].ToString("F1").Replace(',', '.');
          labelNapetiClanek5.ForeColor = service.voltage.get_colorTextClanek(4);
        }));
        labelNapetiAku.Invoke(new EventHandler(delegate
        {
          labelNapetiAku.Text = service.voltage.get_uAku().ToString("F1").Replace(',', '.');
          labelNapetiAku.ForeColor = colorActive; //TODO
        }));

        trackBarNapetiAku.Invoke(new EventHandler(delegate
        {
          //trackBarNapetiAku.Minimum = (int)(dd.napeti.get_uAkuMinimal() * 10.0f);
          //trackBarNapetiAku.Maximum = (int)(dd.napeti.get_napetiAkuMaximal() * 10.0f);
          //trackBarNapetiAku.Value = dd.napeti.get_procAku();
        }));


        labelAD1.Invoke(new EventHandler(delegate
        {
          labelAD1.Text = service.get_AD1().ToString();
          labelAD1.ForeColor = colorActive; //TODO
        }));

        labelAD2.Invoke(new EventHandler(delegate
        {
          labelAD2.Text = service.get_AD2().ToString();
          labelAD2.ForeColor = colorActive; //TODO
        }));

        labelRSSI.Invoke(new EventHandler(delegate
        {
          labelRSSI.Text = service.get_RSSI().ToString();
          labelRSSI.ForeColor = colorActive; //TODO
        }));

        if (service.model.yModel == 0)
        {
          labelModelName.Invoke(new EventHandler(delegate
          {
            labelModelName.Text = service.model.yModel.ToString("X");
            labelModelName.ForeColor = colorActiveHeader;
          }));
        }
        else
        {
          labelModelName.Invoke(new EventHandler(delegate
          {
            labelModelName.Text = service.model.yModelName;
            labelModelName.ForeColor = colorActiveHeader;
          }));
        }

        labelLat.Invoke(new EventHandler(delegate
        {
          string t;
          t = service.gps.get_SN();
          t += service.gps.yLatS.ToString() + "°";
          t += service.gps.yLatM.ToString() + ".";
          t += service.gps.yLatDM.ToString();
          labelLat.Text = t;
          labelLat.ForeColor = colorActive; //TODO
        }));

        labelLon.Invoke(new EventHandler(delegate
        {
          string t;
          t = service.gps.get_EW();
          t += service.gps.yLonS.ToString() + "°";
          t += service.gps.yLonM.ToString() + ".";
          t += service.gps.yLonDM.ToString();
          labelLon.Text = t;
          labelLon.ForeColor = colorActive; //TODO

        }));

        labelAlt.Invoke(new EventHandler(delegate
        {
          labelAlt.Text = service.gps.hAlt.ToString() + "m.n.m";
          labelAlt.ForeColor = colorActive; //TODO
        }));

        labelSpeed.Invoke(new EventHandler(delegate
        {
          labelSpeed.Text = service.gps.vSpeed.ToString();
          labelSpeed.ForeColor = colorActive; //TODO
        }));


        labelSpeedMax.Invoke(new EventHandler(delegate
        {
          labelSpeedMax.Text = service.model.vModelMax.ToString();
        }));

        labelVyska.Invoke(new EventHandler(delegate
        {
          labelVyska.Text = service.model.hModel.ToString();
          labelVyska.ForeColor = colorActive; //TODO
        }));


        labelAltitudeMax.Invoke(new EventHandler(delegate
        {
          labelAltitudeMax.Text = service.model.hModelMax.ToString();
        }));

        labelUAltitude.Invoke(new EventHandler(delegate
        {
          labelUAltitude.Text = service.uDistance.uUnits;
        }));

        labelSatInView.Invoke(new EventHandler(delegate
        {
          labelSatInView.Text = service.gps.ySats.ToString() + "sat in view";
          labelSatInView.ForeColor = colorActive; //TODO
        }));

        labelAzimuth.Invoke(new EventHandler(delegate
        {
          labelAzimuth.Text = service.gps.yAzimuth.ToString() + "°";
          labelAzimuth.ForeColor = colorActive; //TODO
        }));

        labelAzimuthAbbr.Invoke(new EventHandler(delegate
        {
          labelAzimuthAbbr.Text = service.gps.yAzimuthAbbr;
          labelAzimuthAbbr.ForeColor = colorActive; //TODO
        }));

        labelTeplota1.Invoke(new EventHandler(delegate
        {
          labelTeplota1.Text = service.temperature.cDallasSensor[0].ToString("F1").Replace(',', '.') + service.uTemperature.uUnits;
          labelTeplota1.ForeColor = colorActive; //TODO
        }));

        labelTeplota2.Invoke(new EventHandler(delegate
        {
          labelTeplota2.Text = service.temperature.cDallasSensor[1].ToString("F1").Replace(',', '.') + service.uTemperature.uUnits;
          labelTeplota2.ForeColor = colorActive; //TODO
        }));

        labelTeplota3.Invoke(new EventHandler(delegate
        {
          labelTeplota3.Text = service.temperature.cDallasSensor[2].ToString("F1").Replace(',', '.') + service.uTemperature.uUnits;
          labelTeplota3.ForeColor = colorActive; //TODO
        }));

        labelTeplota4.Invoke(new EventHandler(delegate
        {
          labelTeplota4.Text = service.temperature.cDallasSensor[3].ToString("F1").Replace(',', '.') + service.uTemperature.uUnits;
          labelTeplota4.ForeColor = colorActive; //TODO
        }));

        labelTeplotaNazev1.Invoke(new EventHandler(delegate
        {
          labelTeplotaNazev1.Text = service.model.yTemperatureSensorName[0];
        }));

        labelTeplotaNazev2.Invoke(new EventHandler(delegate
        {
          labelTeplotaNazev2.Text = service.model.yTemperatureSensorName[1];
        }));

        labelTeplotaNazev3.Invoke(new EventHandler(delegate
        {
          labelTeplotaNazev3.Text = service.model.yTemperatureSensorName[2];
        }));

        labelTeplotaNazev4.Invoke(new EventHandler(delegate
        {
          labelTeplotaNazev4.Text = service.model.yTemperatureSensorName[3];
        }));

        trackBarAD1.Invoke(new EventHandler(delegate
        {
          trackBarAD1.Value = service.get_AD1();
        }));

        trackBarAD2.Invoke(new EventHandler(delegate
        {
          trackBarAD2.Value = service.get_AD2();
        }));

        trackBarRSSI.Invoke(new EventHandler(delegate
        {
          trackBarRSSI.Value = service.get_RSSI();
        }));


        labelAirportName.Invoke(new EventHandler(delegate
        {
          labelAirportName.Text = service.airport.yAirportName + " " + service.airport.hAirportAlt.ToString() + "m.n.m";
          labelAirportName.ForeColor = colorActiveHeader;
        }));

        //BMP085
        labelPressure.Invoke(new EventHandler(delegate
        {
          labelPressure.Text = service.bmp085.pPressureDecimal.ToString("F2") + "hPa";
          labelPressure.ForeColor = colorActive;
        }));

        labelTemperature.Invoke(new EventHandler(delegate
        {
          labelTemperature.Text = service.bmp085.cTemperature.ToString().Replace(',', '.') + service.uTemperature.uUnits;
          labelTemperature.ForeColor = colorActive;
        }));

        labelGPSDate.Invoke(new EventHandler(delegate
        {
          labelGPSDate.Text = service.date.get_datum().ToLocalTime().ToString();
          labelGPSDate.ForeColor = colorActive;
        }));

        labelDist.Invoke(new EventHandler(delegate
        {
          labelDist.Text = service.bmp085.sDistance.ToString() + "cm";
          labelDist.ForeColor = colorActive;
        }));

        labelVyskaBaro.Invoke(new EventHandler(delegate
        {
          labelVyskaBaro.Text = service.bmp085.hVyskaBaro.ToString() + "m";
          labelVyskaBaro.ForeColor = colorActive;
        }));

        labelFix.Invoke(new EventHandler(delegate
        {
          labelFix.ForeColor = Color.White;
          string tmp_yFix = String.Empty;
          if (service.gps.yFix <= 1) //no fix
          {
            tmp_yFix = "NF";
            labelFix.BackColor = Color.Red;
            labelFix.ForeColor = Color.White;
          }
          if (service.gps.yFix == 2) //2D fix
          {
            tmp_yFix = "2D";
            labelFix.BackColor = Color.Yellow;
            labelFix.ForeColor = Color.Black;
          }
          if (service.gps.yFix == 3) //3D fix
          {
            tmp_yFix = "3D";
            labelFix.BackColor = Color.Green;
            labelFix.ForeColor = Color.White;
          }
          labelFix.Text = tmp_yFix;
        }));

        labelBufferDelka.Invoke(new EventHandler(delegate
        {
          labelBufferDelka.Text = service.getBufferLength().ToString();
        }));

        labelSpeed1.Invoke(new EventHandler(delegate
        {
          labelSpeed1.Text = service.gps.vSpeed.ToString();
          labelSpeed1.ForeColor = colorActive; //TODO
        }));


      }
      else if (tabControl1.SelectedIndex == 1) {

        labelVyska1.Invoke(new EventHandler(delegate
        {
          labelVyska1.Text = service.model.hModel.ToString();
          labelVyska1.ForeColor = colorActive; //TODO
        }));

        labelSpeed1.Invoke(new EventHandler(delegate
        {
          labelSpeed1.Text = service.gps.vSpeed.ToString();
          labelSpeed1.ForeColor = colorActive; //TODO
        }));

        labelAlt1.Invoke(new EventHandler(delegate
        {
          labelAlt1.Text = service.gps.hAlt.ToString() + "m.n.m";
          labelAlt1.ForeColor = colorActive; //TODO
        }));

        labelAltitudeMax1.Invoke(new EventHandler(delegate
        {
          labelAltitudeMax1.Text = service.model.hModelMax.ToString();
        }));

        labelAzimuth1.Invoke(new EventHandler(delegate
        {
          labelAzimuth1.Text = service.gps.yAzimuth.ToString() + "°";
          labelAzimuth1.ForeColor = colorActive; //TODO
        }));

        labelAzimuthAbbr1.Invoke(new EventHandler(delegate
        {
          labelAzimuthAbbr1.Text = service.gps.yAzimuthAbbr;
          labelAzimuthAbbr1.ForeColor = colorActive; //TODO
        }));

        labelFix1.Invoke(new EventHandler(delegate
        {
          labelFix1.ForeColor = Color.White;
          string tmp_yFix = String.Empty;
          if (service.gps.yFix <= 1) //no fix
          {
            tmp_yFix = "NF";
            labelFix1.BackColor = Color.Red;
            labelFix1.ForeColor = Color.White;
          }
          if (service.gps.yFix == 2) //2D fix
          {
            tmp_yFix = "2D";
            labelFix1.BackColor = Color.Yellow;
            labelFix1.ForeColor = Color.Black;
          }
          if (service.gps.yFix == 3) //3D fix
          {
            tmp_yFix = "3D";
            labelFix1.BackColor = Color.Green;
            labelFix1.ForeColor = Color.White;
          }
          labelFix1.Text = tmp_yFix;
        }));

        labelSatInView1.Invoke(new EventHandler(delegate
        {
          labelSatInView1.Text = service.gps.ySats.ToString() + "sat in view";
          labelSatInView1.ForeColor = colorActive; //TODO
        }));

        labelLat1.Invoke(new EventHandler(delegate
        {
          string t;
          t = service.gps.get_SN();
          t += service.gps.yLatS.ToString() + "°";
          t += service.gps.yLatM.ToString() + ".";
          t += service.gps.yLatDM.ToString();
          labelLat1.Text = t;
          labelLat1.ForeColor = colorActive; //TODO
        }));

        labelLon1.Invoke(new EventHandler(delegate
        {
          string t;
          t = service.gps.get_EW();
          t += service.gps.yLonS.ToString() + "°";
          t += service.gps.yLonM.ToString() + ".";
          t += service.gps.yLonDM.ToString();
          labelLon1.Text = t;
          labelLon1.ForeColor = colorActive; //TODO
        }));

        labelGPSDate1.Invoke(new EventHandler(delegate
        {
          labelGPSDate1.Text = service.date.get_datum().ToLocalTime().ToString();
          labelGPSDate1.ForeColor = colorActive;
        }));

      }
      else if (tabControl1.SelectedIndex == 2)
      {

        labelPressure1.Invoke(new EventHandler(delegate
        {
          labelPressure1.Text = service.bmp085.pPressureDecimal.ToString("F2") + "hPa";
          labelPressure1.ForeColor = colorActive;
        }));

        labelVyskaBaro1.Invoke(new EventHandler(delegate
        {
          labelVyskaBaro1.Text = service.bmp085.hVyskaBaro.ToString();
          labelVyskaBaro1.ForeColor = colorActive;
        }));

        labelDist1.Invoke(new EventHandler(delegate
        {
          labelDist1.Text = service.bmp085.sDistance.ToString() + "cm";
          labelDist1.ForeColor = colorActive;
        }));

      }
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      //if (!bc.Connected)
      {
        //bc.Connect
      }

      dataReceived--;

      if (dataReceived > 0)
      {
        ddShow();
        if (labelData.BackColor == Color.Green)
          labelData.BackColor = Color.Black;
        else
          labelData.BackColor = Color.Green;
      }
      else
      {
        //dd.napeti.set_Napeti2Zero();
        if (labelSpeed.Text == "999") labelSpeed.Text = "---";
        if (labelVyska.Text == "9999") labelVyska.Text = "----";
        if (labelSpeed1.Text == "999") labelSpeed1.Text = "---";
        if (labelVyska1.Text == "9999") labelVyska1.Text = "----";

        labelData.BackColor = Color.Red;
        Color colorInactive = Color.FromArgb(40, 40, 40);
        labelNapetiPalubni.ForeColor = colorInactive;
        labelNapetiClanek1.ForeColor = colorInactive;
        labelNapetiClanek2.ForeColor = colorInactive;
        labelNapetiClanek3.ForeColor = colorInactive;
        labelNapetiClanek4.ForeColor = colorInactive;
        labelNapetiClanek5.ForeColor = colorInactive;
        labelTeplota1.ForeColor = colorInactive;
        labelTeplota2.ForeColor = colorInactive;
        labelTeplota3.ForeColor = colorInactive;
        labelTeplota4.ForeColor = colorInactive;
        labelNapetiAku.ForeColor = colorInactive;
        labelAD1.ForeColor = colorInactive;
        labelAD2.ForeColor = colorInactive;
        labelRSSI.ForeColor = colorInactive;
        labelModelName.ForeColor = colorInactive;
        labelAirportName.ForeColor = colorInactive;
        labelPressure.ForeColor = colorInactive;
        labelTemperature.ForeColor = colorInactive;
        labelVyska.ForeColor = colorInactive;
        labelSpeed.ForeColor = colorInactive;
        labelVyska1.ForeColor = colorInactive;
        labelSpeed1.ForeColor = colorInactive;
        labelAzimuth.ForeColor = colorInactive;
        labelAzimuthAbbr.ForeColor = colorInactive;
        labelSatInView.ForeColor = colorInactive;
        labelLat.ForeColor = colorInactive;
        labelLon.ForeColor = colorInactive;
        labelAlt.ForeColor = colorInactive;
        labelFix.ForeColor = colorInactive;
        labelFix.BackColor = Color.Black;
        labelGPSDate.ForeColor = colorInactive;
        labelDist.ForeColor = colorInactive;
        labelVyskaBaro.ForeColor = colorInactive;

        labelVyska1.ForeColor = colorInactive;
        labelSpeed1.ForeColor = colorInactive;
        labelAlt1.ForeColor = colorInactive;
        labelAltitudeMax1.ForeColor = colorInactive;
        labelAzimuth1.ForeColor = colorInactive;
        labelAzimuthAbbr1.ForeColor = colorInactive;
        labelFix1.ForeColor = colorInactive;
        labelSatInView1.ForeColor = colorInactive;
        labelLat1.ForeColor = colorInactive;
        labelLon1.ForeColor = colorInactive;
        labelGPSDate1.ForeColor = colorInactive;

        labelPressure1.ForeColor = colorInactive;
        labelVyskaBaro1.ForeColor = colorInactive;
        labelDist1.ForeColor = colorInactive;
        //ddShow();
      }
    }

    private void serialPorts_SelectedIndexChanged(object sender, EventArgs e)
    {
      ComboBox cb = (ComboBox)sender;
      string port = cb.SelectedItem.ToString();
      if (comport.IsOpen)
        comport.Close();

      comport.PortName = port;

      comport.Open();
      //textBox1.Text += port;
    }


    private void button5_Click(object sender, EventArgs e)
    {
      service.model.hModelMax = 0;
      service.bmp085.pPressureZero = int.MinValue;
    }

    private void button7_Click(object sender, EventArgs e)
    {
      service.model.hModelMax = 0;
    }

    private void button6_Click(object sender, EventArgs e)
    {
      service.model.vModelMax = 0;
    }



    private void menuItem1_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    //byte[] naplnBuffer()
    //{
    //  byte[] buf = new byte[909];

    //  string retezec = "7E7EFD0315";
    //  retezec += "801F7F"
    //  retezec += "A900007E7EFE445A50A4000000007E7EFE445A50A4000000007E7EFD0416";
    //  retezec += "A031A12C";
    //  retezec += "00007E7EFE445A50A4000000007E7EFE435A50A3000000007E7EFE445A50A4000000007E7EFD0517";
    //  retezec += "A20767A30D";
    //  retezec += "007E7EFE435A50A3000000007E7EFE445A51A4000000007E7EFE435A50A3000000007E7EFD0518";
    //  retezec += "A416A50678";
    //  retezec += "007E7EFE435A50A4000000007E7EFE445A50A4000000007E7EFE445A50A4000000007E7EFD0619";
    //  retezec += "A61D4DA70301";
    //  retezec += "7E7EFE435A50A4000000007E7EFD051A";
    //  retezec += "A80240";
    //  retezec += "A90A007E7EFE445A50A5000000007E7EFE435A50A5000000007E7EFE435A50A5000000007E7EFD061B";
    //  retezec += "AA408A";
    //  retezec += "AB78B07E7EFE435A50A6000000007E7EFE445A50A4000000007E7EFE445A50A5000000007E7EFD061C";
    //  retezec += "AC024D";
    //  retezec += "AD024E7E7EFE445A50A4000000007E7EFE435A50A5000000007E7EFE445A50A4000000007E7EFD051D";
    //  retezec += "AE024F";
    //  retezec += "AF03007E7EFE445A50A4000000007E7EFE445A50A3000000007E7EFE435A4FA3000000007E7EFE435A50A4000000007E7EFE435A50A3000000007E7EFD031E";
    //  retezec += "81077F0000007E7EFE445A50A4000000007E7EFE445A50A3000000007E7EFE445A50A3000000007E7EFD031F";
    //  retezec += "82074FA300007E7EFE435A50A4000000007E7EFE435A50A3000000007E7EFE435A50A4000000007E7EFD0300";
    //  retezec += "8306550000007E7EFE435A50A3000000007E7EFE435A50A4000000007E7EFE435A50A3000000007E7EFD0301";
    //  retezec += "84063DA700007E7EFE435A50A3000000007E7EFE445A50A4000000007E7EFD0302";
    //  retezec += "850624A900007E7EFE435A50A3000000007E7EFE435A51A4000000007E7EFE435A50A3000000007E7EFD0303";
    //  retezec += "86060C0";
    //  retezec += "8AB00007E7EFE435A50A4000000007E7EFE435A50A3000000007E7EFE435A50A3000000007E7EFD03048A016FAD00007E7EFE445A50A4000000007E7EFE445A50A2000000007E7EFE445A50A4000000007E7EFD0305";
    //  retezec += "8B0000AF00007E7EFE435A50A3000000007E7EFE435A50A4000000007E7EFD0206";
    //  retezec += "8C00500000007E7EFE445A50A3000000007E7EFD0107000356A300007E7EFE445A50A3000000007E7EFE445A51A4000000007E7EFD0308";
    //  retezec += "8D00000000007E7EFE445A50A2000000007E7EFE435A51A4000000007E7EFE435A51A3000000007E7EFD0609";
    //  retezec += "9406174D";
    //  retezec += "95007E7EFD0209016B000095007E7EFE435A50A4000000007E7EFE435A50A3000000007E7EFD030A";
    //  retezec += "967818A900007E7EFE445A51A3000000007E7EFE435A51A3000000007E7EFE445A51A3000000007E7EFD030B00";
    //  int index = 0;
    //  for (int i = 0; i < retezec.Length; i += 2)
    //  {
    //    buf[index++] = (byte)(Convert.ToInt32(retezec.Substring(i, 2), 16));
    //  }

    //  return buf;

    //}

    private void button8_Click(object sender, EventArgs e)
    {
      getBTInfo();
    }

    private void button9_Click(object sender, EventArgs e)
    {
      BluetoothRadio radio = BluetoothRadio.PrimaryRadio;
      if (radio != null && radio.Mode == RadioMode.PowerOff)
      {
        BluetoothRadio.PrimaryRadio.Mode = RadioMode.Connectable;
      }
      
      bc = new BluetoothClient();
      if (!comport.IsOpen)
        comport.Open();
      //bc.Connect(new InTheHand.Net.BluetoothAddress(001111180589), BluetoothService.SerialPort);
    }


    private void getBTInfo()
    {
      BluetoothDeviceInfo[] bdi = bc.DiscoverDevices();
      label8.Text = String.Empty;
      for (int i = 0; i < bdi.Length; i++)
        label8.Text += bdi[i].DeviceAddress + " " + bdi[i].DeviceName + " " + bdi[i].Connected.ToString() + "\n";

    }

    private void button10_Click(object sender, EventArgs e)
    {
      bc.Close();
    }

    private void panelVyskaRychlost_Click(object sender, EventArgs e)
    {
      //to GPS page
      tabControl1.SelectedIndex = 1;
    }

    private void panelGPSDetail_Click(object sender, EventArgs e)
    {
      //to All page
      tabControl1.SelectedIndex = 0;
    }

    private void panelBaro_Click(object sender, EventArgs e)
    {
      //to Baro page
      tabControl1.SelectedIndex = 2;
    }

    private void panelBaroDetail_Click(object sender, EventArgs e)
    {
      //to All page
      tabControl1.SelectedIndex = 0;
    }
  }
}