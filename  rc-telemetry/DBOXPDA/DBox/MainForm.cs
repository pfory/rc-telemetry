using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using Nini.Config;

namespace DBOX
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
      Check_Configs();
    }

    Data data = new Data();

    private bool isLogged = false;
    StreamWriter logFile = new StreamWriter(@"DBOX.log", true);
    int dataReceived = 0;
 
    private void comport_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
      if (!comport.IsOpen) return;

      dataReceived++;

      int bytes = comport.BytesToRead;

      // Create a byte array buffer to hold the incoming data
      byte[] buffer = new byte[bytes];

      // Read the data from the port and store it in our buffer
      comport.Read(buffer, 0, bytes);


      /*for (int i = 0; i < buffer.Length; i++)
        logFile.Write(buffer[i].ToString("X"));
      */
      data.set_buffer(buffer);
      data.decode(); 
      ddShow();

      // Show the user the incoming data in hex format
      //if (isLogged)
      //  Log(ByteArrayToHexString(buffer));
    }

    /// <summary> Log data to the terminal window. </summary>
    /// <param name="msgtype"> The type of message to be written. </param>
    /// <param name="msg"> The string containing the message to be shown. </param>
    private void Log(string msg)
    {
      //textBox1.Invoke(new EventHandler(delegate
      //{
        for (int i = 0; i < msg.Length; i++)
        {
          string s = msg[i].ToString();
          logFile.Write(s);
        }
      //}));
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
      data.airport.set_AirportID(1);
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
      labelNapetiPalubni.Invoke(new EventHandler(delegate
      {
        labelNapetiPalubni.Text = data.napeti.get_napetiPalubni().ToString("F1").Replace(',','.');
        labelNapetiPalubni.ForeColor = data.napeti.get_colorTextPalubni();
      }));
      labelNapetiClanek1.Invoke(new EventHandler(delegate
      {
        labelNapetiClanek1.Text = data.napeti.get_napeti(0).ToString("F1").Replace(',', '.');
        labelNapetiClanek1.ForeColor = data.napeti.get_colorTextClanek(0);
      }));
      labelNapetiClanek2.Invoke(new EventHandler(delegate
      {
        labelNapetiClanek2.Text = data.napeti.get_napeti(1).ToString("F1").Replace(',', '.');
        labelNapetiClanek2.ForeColor = data.napeti.get_colorTextClanek(1);
      }));
      labelNapetiClanek3.Invoke(new EventHandler(delegate
      {
        labelNapetiClanek3.Text = data.napeti.get_napeti(2).ToString("F1").Replace(',', '.');
        labelNapetiClanek3.ForeColor = data.napeti.get_colorTextClanek(2);
      }));
      labelNapetiClanek4.Invoke(new EventHandler(delegate
      {
        labelNapetiClanek4.Text = data.napeti.get_napeti(3).ToString("F1").Replace(',', '.');
        labelNapetiClanek4.ForeColor = data.napeti.get_colorTextClanek(3);
      }));
      labelNapetiClanek5.Invoke(new EventHandler(delegate
      {
        labelNapetiClanek5.Text = data.napeti.get_napeti(4).ToString("F1").Replace(',', '.');
        labelNapetiClanek5.ForeColor = data.napeti.get_colorTextClanek(4);
      }));
      //labelNapetiClanek6.Invoke(new EventHandler(delegate
      //{
      //  labelNapetiClanek6.Text = dd.napeti.get_napeti(5).ToString("F1").Replace(',', '.');
      //  labelNapetiClanek6.ForeColor = dd.napeti.get_colorTextClanek(5);
      //}));
      labelNapetiAku.Invoke(new EventHandler(delegate
      {
        labelNapetiAku.Text = data.napeti.get_napetiAku().ToString("F1").Replace(',', '.');
        labelNapetiAku.ForeColor = colorActive; //TODO
      }));

      trackBarNapetiAku.Invoke(new EventHandler(delegate
      {
        //trackBarNapetiAku.Minimum = (int)(dd.napeti.get_napetiAkuMinimal() * 10.0f);
        //trackBarNapetiAku.Maximum = (int)(dd.napeti.get_napetiAkuMaximal() * 10.0f);
        //trackBarNapetiAku.Value = dd.napeti.get_procAku();
      }));


      labelAD1.Invoke(new EventHandler(delegate
      {
        labelAD1.Text = data.get_AD1().ToString();
        labelAD1.ForeColor = colorActive; //TODO
      }));

      labelAD2.Invoke(new EventHandler(delegate
      {
        labelAD2.Text = data.get_AD2().ToString();
        labelAD2.ForeColor = colorActive; //TODO
      }));

      labelRSSI.Invoke(new EventHandler(delegate
      {
        labelRSSI.Text = data.get_RSSI().ToString();
        labelRSSI.ForeColor = colorActive; //TODO
      }));

      labelID.Invoke(new EventHandler(delegate
      {
        labelID.Text = data.model.get_ModelID().ToString("X");
        labelID.ForeColor = colorActive;
      }));

      labelLat.Invoke(new EventHandler(delegate
      {
        string t;
        t = data.gps.get_SN();
        t += data.gps.get_latS().ToString() + "°";
        t += data.gps.get_latM().ToString() + ".";
        t += data.gps.get_latDM().ToString();
        labelLat.Text = t;
        labelLat.ForeColor = colorActive; //TODO
      }));

      labelLon.Invoke(new EventHandler(delegate
      {
        string t;
        t = data.gps.get_EW();
        t += data.gps.get_lonS().ToString() + "°";
        t += data.gps.get_lonM().ToString() + ".";
        t += data.gps.get_lonDM().ToString();
        labelLon.Text = t;
        labelLon.ForeColor = colorActive; //TODO

      }));

      labelAlt.Invoke(new EventHandler(delegate
      {
        labelAlt.Text = data.gps.get_alt().ToString() + "m.n.m";
        labelAlt.ForeColor = colorActive; //TODO
      }));

      labelSpeed.Invoke(new EventHandler(delegate
      {
        labelSpeed.Text = data.gps.get_speed().ToString();
        labelSpeed.ForeColor = colorActive; //TODO
      }));

      labelSpeedMax.Invoke(new EventHandler(delegate
      {
        labelSpeedMax.Text = data.gps.get_speedMax().ToString();
      }));

      labelVyska.Invoke(new EventHandler(delegate
      {
        labelVyska.Text = data.gps.get_vyska().ToString();
        labelVyska.ForeColor = colorActive; //TODO
      }));

      labelVyskaMax.Invoke(new EventHandler(delegate
      {
        labelVyskaMax.Text = data.gps.get_vyskaMax().ToString();
      }));


      labelSatInView.Invoke(new EventHandler(delegate
      {
        labelSatInView.Text = data.gps.get_satInView().ToString() + "sat in view";
        labelSatInView.ForeColor = colorActive; //TODO
      }));

      labelAzimuth.Invoke(new EventHandler(delegate
      {
        labelAzimuth.Text = data.gps.get_azimuth().ToString() + "°";
        labelAzimuth.ForeColor = colorActive; //TODO
      }));

      labelAzimuthAbbr.Invoke(new EventHandler(delegate
      {
        labelAzimuthAbbr.Text = data.gps.get_AzimuthAbbr();
        labelAzimuthAbbr.ForeColor = colorActive; //TODO
      }));

      labelTeplota1.Invoke(new EventHandler(delegate
      {
        labelTeplota1.Text = data.teplota.get_teplota(0).ToString("F1").Replace(',', '.');
        labelTeplota1.ForeColor = colorActive; //TODO
      }));

      labelTeplota2.Invoke(new EventHandler(delegate
      {
        labelTeplota2.Text = data.teplota.get_teplota(1).ToString("F1").Replace(',', '.');
        labelTeplota2.ForeColor = colorActive; //TODO
      }));

      labelTeplota3.Invoke(new EventHandler(delegate
      {
        labelTeplota3.Text = data.teplota.get_teplota(2).ToString("F1").Replace(',', '.');
        labelTeplota3.ForeColor = colorActive; //TODO
      }));

      labelTeplota4.Invoke(new EventHandler(delegate
      {
        labelTeplota4.Text = data.teplota.get_teplota(3).ToString("F1").Replace(',', '.');
        labelTeplota4.ForeColor = colorActive; //TODO
      }));

      labelTeplotaNazev1.Invoke(new EventHandler(delegate
      {
        labelTeplotaNazev1.Text = data.model.get_TeplotaCidloName(0);
      }));

      labelTeplotaNazev2.Invoke(new EventHandler(delegate
      {
        labelTeplotaNazev2.Text = data.model.get_TeplotaCidloName(1);
      }));

      labelTeplotaNazev3.Invoke(new EventHandler(delegate
      {
        labelTeplotaNazev3.Text = data.model.get_TeplotaCidloName(2);
      }));

      labelTeplotaNazev4.Invoke(new EventHandler(delegate
      {
        labelTeplotaNazev4.Text = data.model.get_TeplotaCidloName(3);
      }));

      trackBarAD1.Invoke(new EventHandler(delegate
      {
        trackBarAD1.Value = data.get_AD1();
      }));

      trackBarAD2.Invoke(new EventHandler(delegate
      {
        trackBarAD2.Value = data.get_AD2();
      }));

      trackBarRSSI.Invoke(new EventHandler(delegate
      {
        trackBarRSSI.Value = data.get_RSSI();
      }));

      labelModelName.Invoke(new EventHandler(delegate
      {
        labelModelName.Text = data.model.get_ModelName();
        labelModelName.ForeColor = colorActive;
      }));

      labelAirportName.Invoke(new EventHandler(delegate
      {
        labelAirportName.Text = data.airport.get_AirportName();
        labelAirportName.ForeColor = colorActive;
      }));

    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      if (dataReceived > 0)
      {
        dataReceived = 0;
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

        labelData.BackColor = Color.Red;
        Color colorInactive = Color.FromArgb(40, 40, 40);
        labelNapetiPalubni.ForeColor = colorInactive;
        labelNapetiClanek1.ForeColor = colorInactive;
        labelNapetiClanek2.ForeColor = colorInactive;
        labelNapetiClanek3.ForeColor = colorInactive;
        labelNapetiClanek4.ForeColor = colorInactive;
        labelNapetiClanek5.ForeColor = colorInactive;
        labelNapetiClanek6.ForeColor = colorInactive;
        labelVyska.ForeColor = colorInactive;
        labelSpeed.ForeColor = colorInactive;
        labelTeplota1.ForeColor = colorInactive;
        labelTeplota2.ForeColor = colorInactive;
        labelTeplota3.ForeColor = colorInactive;
        labelTeplota4.ForeColor = colorInactive;
        labelNapetiAku.ForeColor = colorInactive;
        labelAzimuth.ForeColor = colorInactive;
        labelAzimuthAbbr.ForeColor = colorInactive;
        labelAD1.ForeColor = colorInactive;
        labelAD2.ForeColor = colorInactive;
        labelRSSI.ForeColor = colorInactive;
        labelSatInView.ForeColor = colorInactive;
        labelLat.ForeColor = colorInactive;
        labelLon.ForeColor = colorInactive;
        labelAlt.ForeColor = colorInactive;
        labelModelName.ForeColor = colorInactive;
        labelID.ForeColor = colorInactive;
        labelAirportName.ForeColor = colorInactive;
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

    private void button2_Click(object sender, EventArgs e)
    {
      isLogged = !isLogged;
    }

    private void button5_Click(object sender, EventArgs e)
    {
      data.gps.set_vyskaMax(0);
    }

    private void button7_Click(object sender, EventArgs e)
    {
      data.gps.set_vyskaLetiste();
      data.gps.set_vyskaMax(0);
    }

    private void button6_Click(object sender, EventArgs e)
    {
      data.gps.set_speedMax(0);
    }



    private void menuItem1_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }
  
  }
}