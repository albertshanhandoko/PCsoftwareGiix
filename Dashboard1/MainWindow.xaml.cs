﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Windows.Threading;
using Dashboard1.Library;
using System.IO;
using System.Threading;
using Dashboard1.Helper;
using Microsoft.Win32;
using System.Diagnostics;
using System.Configuration;
using System.Resources;
using System.Management;

using System.Runtime.InteropServices;

namespace Dashboard1
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        static string Folder_Path = ConfigurationManager.AppSettings["Folder_Path"] ?? "Not Found";
        static string application_name = ConfigurationManager.AppSettings["application_name"] ?? "Not Found";
        static string url_Image_Logo = Folder_Path + "dataconfig/Logo.png";
        static string RadioButtonDefault = ConfigurationManager.AppSettings["RadioBtn_Default"] ?? "Not Found";
        //private USBClass USBPort;
        public MainWindow()
        {

        
            InitializeComponent();
            Data_Initialize();
            //check_removeable(); // ini buat cek usb
            //check_model();


            Consumo consumo = new Consumo();
            DataContext = new ConsumoViewModel(consumo);

            //txt_applicationName.Text = application_name;
            if(RadioButtonDefault == "0")
            {
                RadioBtn_English.IsChecked = true;
            }
            
            else
            {
                RadioBtn_Bahasa.IsChecked = true;
            }


        }

        public void check_model()
        {
            bool check_model = false;
            ManagementObjectSearcher theSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            foreach (ManagementObject currentObject in theSearcher.Get())
            {
                ManagementObject theSerialNumberObjectQuery = new ManagementObject("Win32_PhysicalMedia.Tag='" + currentObject["DeviceID"] + "'");
            
                string model = currentObject["MODEL"].ToString();
                if (model == "General UDisk USB Device")
                {
                    check_model = true;
                }
            }
            if (check_model != true)
            {
                MessageBox.Show("Must use Vendor Device");
                System.Windows.Application.Current.Shutdown();
            }

        }
        public void check_removeable()
        {
            string path = Process.GetCurrentProcess().MainModule.FileName;
            FileInfo fileInfo = new FileInfo(path);
            string driveRoot = fileInfo.Directory.Root.Name;
            DriveInfo driveInfo = new DriveInfo(driveRoot);
            if (driveInfo.DriveType != DriveType.Removable)
            {
                MessageBox.Show("Must run from removable drive");
                Thread.Sleep(1000);
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void Data_Initialize()
        {
            var portNames = SerialPort.GetPortNames();

            // A4 and A5 page size
            ComboBox_Pagesize.Items.Clear();

            //ComboBox_Port1.Items.Clear();
            /*
            ComboBox_Port2.Items.Clear();
            ComboBox_Port3.Items.Clear();
            ComboBox_Port4.Items.Clear();
            ComboBox_Port5.Items.Clear();
            ComboBox_Port6.Items.Clear();
            //ComboBox_input_data.Items.Clear();

            //ComboBox_BaudRate1.Items.Clear();
            ComboBox_BaudRate2.Items.Clear();
            ComboBox_BaudRate3.Items.Clear();
            ComboBox_BaudRate4.Items.Clear();
            ComboBox_BaudRate5.Items.Clear();
            ComboBox_BaudRate6.Items.Clear();
            //ComboBox_Input_Baud.Items.Clear();
            
            foreach (var portname in portNames)
            {
                //ComboBox_Port1.Items.Add(portname.ToString());
                ComboBox_Port2.Items.Add(portname.ToString());
                ComboBox_Port3.Items.Add(portname.ToString());
                ComboBox_Port4.Items.Add(portname.ToString());
                ComboBox_Port5.Items.Add(portname.ToString());
                ComboBox_Port6.Items.Add(portname.ToString());
                ComboBox_Port6.Items.Add(portname.ToString());
                //ComboBox_input_data.Items.Add(portname.ToString());
            }

            List<int> ListBaudRate = new List<int>(new int[] {
                110, 300, 600, 1200, 2400, 4800, 9600, 14400
             , 19200, 38400, 57600, 115200, 128000

            });
            List<string> Combobox_Baudrate = ListBaudRate.ConvertAll<string>(delegate (int i) { return i.ToString(); });


            foreach (var Baudrate in Combobox_Baudrate)
            {
                //ComboBox_BaudRate1.Items.Add(Baudrate);
                ComboBox_BaudRate2.Items.Add(Baudrate);
                ComboBox_BaudRate3.Items.Add(Baudrate);
                ComboBox_BaudRate4.Items.Add(Baudrate);
                ComboBox_BaudRate5.Items.Add(Baudrate);
                ComboBox_BaudRate6.Items.Add(Baudrate);
                //ComboBox_Input_Baud.Items.Add(Baudrate);
            }
            */

            BitmapImage src = new BitmapImage (new Uri("pack://application:,,,/Resources/Global_Instrument_Logo.jpeg", UriKind.Absolute));
            //imgDynamic.Source = src;


            txt_compname.Text = SensorHelper_2.read_config_name();
            txt_compaddr.Text = SensorHelper_2.read_config_addr();

            //output = (val % 2 == 1) ? "Number is odd" : "Number is even";
            if (String.IsNullOrEmpty(txt_compname.Text))
            {
                txt_compname.Text = "Your Company Name";
            }
            if (String.IsNullOrEmpty(txt_compaddr.Text)){
                txt_compaddr.Text = "Your Company Address";
            }
            //ComboBox_Pagesize.Items.Add("A4");
            //ComboBox_Pagesize.Items.Add("A5");
            ComboBox_Pagesize.SelectedValuePath = "Key";
            ComboBox_Pagesize.DisplayMemberPath = "Value";
            ComboBox_Pagesize.Items.Add(new KeyValuePair<int, string>(0, "A4 Portrait"));
            ComboBox_Pagesize.Items.Add(new KeyValuePair<int, string>(1, "2 copies of report in PDF (A4)"));
            ComboBox_Pagesize.Items.Add(new KeyValuePair<int, string>(2, "\"9.5\" x \"11\" (cnt form paper)"));
            ComboBox_Pagesize.Items.Add(new KeyValuePair<int, string>(3, "\"9.5\" x \"5.5\" (cnt form paper)"));
            /*
            ComboBox_Pagesize.Items.Add("A4 Portrait");
            ComboBox_Pagesize.Items.Add("A5 Landscape");
            ComboBox_Pagesize.Items.Add("A4 Portrait Cnt Paper");
            ComboBox_Pagesize.Items.Add("A5 Landscape Cnt Paper");
            */

            /*
            1. A4 Portrait
            2. A5 Landscape
            3. A4 Portrait Cnt Paper
            4. A5 Landscape Cnt Paper
             */

            SensorHelper_2.Generate_Initial_Folder();

        }
        public static BitmapImage LoadBitmapImage(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }
        public string DisplayedImage
        {
            get { return @"C:\Logo.png"; }
        }
        
        private void Company_data_click(object sender, RoutedEventArgs e)
        {
            Company_data_window company_window = new Company_data_window();
            company_window.Show();
        }

        private void Refresh_click(object sender, RoutedEventArgs e)
        {
            Data_Initialize();
        }
        private void Button_CompLogo_click(object sender, RoutedEventArgs e)
        {
            //string addr_text = "File not found";
            string filepath;
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = false;
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            bool? result = open.ShowDialog();
            if (result == true)
            {
                filepath = open.FileName; // Stores Original Path in Textbox
                Label_Logo.Content = filepath;
                //Txt_LogoAddress.Text = filepath;
            }

        }
        private void Add_Company_Data_click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txt_compname.Text) || String.IsNullOrEmpty(txt_compaddr.Text))
            {
                MessageBox.Show("Please enter company name and address", application_name);
            }
            else
            {
                string url_name_config = Folder_Path + "/DataConfig/config_name.txt";
                string comp_name = txt_compname.Text;
                SensorHelper_2.writeTextFile(url_name_config, comp_name);
                
                string url_addr_config = Folder_Path + "/DataConfig/config_addr.txt";
                string comp_addr = txt_compaddr.Text;
                SensorHelper_2.writeTextFile(url_addr_config, comp_addr);
                
                string url_logo_config = Folder_Path + "/DataConfig/Logo.png";
                
                string filepath = Label_Logo.Content.ToString(); // Stores Original Path in Textbox

                Uri resourceUri = new Uri(filepath, UriKind.RelativeOrAbsolute);
                
                if (!String.IsNullOrEmpty(filepath))
                {    
                    ImageSource imgsource = new BitmapImage(new Uri(filepath));
                    File.Copy(filepath, url_logo_config, true);
                }
                MessageBox.Show("Company Data successfully saved", application_name);

            }
        }

        private void RadioBtn1_Indo(object sender, RoutedEventArgs e)
        {
            guide_title.Text = "Instruksi:";
            guide_step1.Text = "1. Setelah menghubungkan USB, pergi ke 'Device Manager' dan lihat di bawah 'Ports (COM & LPT)' untuk memeriksa nomor COM Port.";
            guide_step2.Text = "2. Pilih nomor COM Port untuk setiap SENSOR yang sesuai dan pilih Baud rate menjadi 600.";
            guide_step3.Text = "3. Tekan 'START' untuk masuk ke halaman SENSOR sebelum Anda mulai mengukur dengan penguji kadar air in-line.";
            guide_step4.Text = "4. Setelah setiap pengukuran selesai, tutup jendela SENSOR dan tekan 'START' lagi untuk membuka jendela baru untuk pengukuran berikutnya.";
            guide_step5.Text = "5. Silakan hubungi indo_sales@globalinstrumentsg.com untuk masalah apa pun.";
            guide_step6.Text = "* Tekan tombol refresh, apabila Serial Port tidak ditemukan.";

        }
        private void RadioBtn1_English(object sender, RoutedEventArgs e)
        {
            guide_title.Text = "How to use this application:";
            guide_step1.Text = "1. After connecting the USB, go to ‘Device Manager’ and look under ‘Ports (COM & LPT)’ to check COM Port number. ";
            guide_step2.Text = "2. Select the COM Port number for each SENSOR accordingly and select the Baud rate to be 600.";
            guide_step3.Text = "3. Press 'START' to enter SENSOR page before you start measuring with the in-line moisture tester.";
            guide_step4.Text = "4. After each measurement is finished, close the SENSOR window and press ‘START’ again to open new window for next measurement. ";
            guide_step5.Text = "5. Please contact indo_sales@globalinstrumentsg.com for any issues.";
            guide_step6.Text = "* Click refresh button, in case Serial Port can't be found.";

        }

 /*
      
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ComboBox_Port2.Text) || ComboBox_Port2.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the Sensor 2 Port", application_name);
            }

            else if (string.IsNullOrEmpty(ComboBox_BaudRate2.Text) || ComboBox_BaudRate2.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the Sensor 2 Baudrate", application_name);
            }
            else
            {
                TestPagination TestPagination_window = new TestPagination();
                /*
                string comport = ComboBox_Port2.Text;
                if (Int32.TryParse(ComboBox_BaudRate2.Text, out int BaudRate))
                {
                    Console.WriteLine(BaudRate);
                }
                SerialPort mySerialPort = new SerialPort(comport);
                if (mySerialPort.IsOpen)
                {
                    Console.WriteLine("Port is open");
                    mySerialPort.Close();
                }
                mySerialPort.BaudRate = BaudRate;
                mySerialPort.Parity = Parity.None;
                mySerialPort.StopBits = StopBits.One;
                mySerialPort.DataBits = 8;
                mySerialPort.Handshake = Handshake.None;
                mySerialPort.RtsEnable = true;
                mySerialPort.Encoding = ASCIIEncoding.ASCII;
                try
                {
                    mySerialPort.Open();
                    Thread.Sleep(100);
                    mySerialPort.Close();
                    Thread.Sleep(100);
                    Report_Window_2 Result_Window = new Report_Window_2();
                    
                    Result_Window.Show();
                }
                catch (Exception error)//(Exception e)
                {
                    MessageBox.Show("Port failed to be opened", application_name);
                    Console.WriteLine(error.Message);
                }
            
            }

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ComboBox_Port3.Text) || ComboBox_Port3.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the Sensor 3 Port", application_name);
            }

            else if (string.IsNullOrEmpty(ComboBox_BaudRate3.Text) || ComboBox_BaudRate3.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the Sensor 3 Baudrate", application_name);
            }
            else
            {
                string comport = ComboBox_Port3.Text;
                if (Int32.TryParse(ComboBox_BaudRate3.Text, out int BaudRate))
                {
                    Console.WriteLine(BaudRate);
                }
                SerialPort mySerialPort = new SerialPort(comport);
                if (mySerialPort.IsOpen)
                {
                    Console.WriteLine("Port is open");
                    mySerialPort.Close();
                }
                mySerialPort.BaudRate = BaudRate;
                mySerialPort.Parity = Parity.None;
                mySerialPort.StopBits = StopBits.One;
                mySerialPort.DataBits = 8;
                mySerialPort.Handshake = Handshake.None;
                mySerialPort.RtsEnable = true;
                mySerialPort.Encoding = ASCIIEncoding.ASCII;
                try
                {
                    mySerialPort.Open();
                    Thread.Sleep(100);
                    mySerialPort.Close();
                    Thread.Sleep(100);
                    Report_Window_3 Result_Window = new Report_Window_3();
                    Result_Window.Show();
                }
                catch (Exception error)//(Exception e)
                {
                    MessageBox.Show("Port failed to be opened", application_name);
                    Console.WriteLine(error.Message);
                }
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ComboBox_Port4.Text) || ComboBox_Port4.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the Sensor 4 Port", application_name);
            }

            else if (string.IsNullOrEmpty(ComboBox_BaudRate4.Text) || ComboBox_BaudRate4.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the Sensor 4 Baudrate", application_name);
            }
            else
            {
                string comport = ComboBox_Port4.Text;
                if (Int32.TryParse(ComboBox_BaudRate4.Text, out int BaudRate))
                {
                    Console.WriteLine(BaudRate);
                }
                SerialPort mySerialPort = new SerialPort(comport);
                if (mySerialPort.IsOpen)
                {
                    Console.WriteLine("Port is open");
                    mySerialPort.Close();
                }
                mySerialPort.BaudRate = BaudRate;
                mySerialPort.Parity = Parity.None;
                mySerialPort.StopBits = StopBits.One;
                mySerialPort.DataBits = 8;
                mySerialPort.Handshake = Handshake.None;
                mySerialPort.RtsEnable = true;
                mySerialPort.Encoding = ASCIIEncoding.ASCII;
                try
                {
                    mySerialPort.Open();
                    Thread.Sleep(100);
                    mySerialPort.Close();
                    Thread.Sleep(100);
                    Report_Window_4 Result_Window = new Report_Window_4();
                    Result_Window.Show();
                }
                catch (Exception error)//(Exception e)
                {
                    MessageBox.Show("Port failed to be opened", application_name);
                    Console.WriteLine(error.Message);
                }
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ComboBox_Port5.Text) || ComboBox_Port5.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the Sensor 5 Port", application_name);
            }

            else if (string.IsNullOrEmpty(ComboBox_BaudRate5.Text) || ComboBox_BaudRate5.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the Sensor 5 Baudrate", application_name);
            }
            else
            {
                string comport = ComboBox_Port5.Text;
                if (Int32.TryParse(ComboBox_BaudRate5.Text, out int BaudRate))
                {
                    Console.WriteLine(BaudRate);
                }
                SerialPort mySerialPort = new SerialPort(comport);
                if (mySerialPort.IsOpen)
                {
                    Console.WriteLine("Port is open");
                    mySerialPort.Close();
                }
                mySerialPort.BaudRate = BaudRate;
                mySerialPort.Parity = Parity.None;
                mySerialPort.StopBits = StopBits.One;
                mySerialPort.DataBits = 8;
                mySerialPort.Handshake = Handshake.None;
                mySerialPort.RtsEnable = true;
                mySerialPort.Encoding = ASCIIEncoding.ASCII;
                try
                {
                    mySerialPort.Open();
                    Thread.Sleep(100);
                    mySerialPort.Close();
                    Thread.Sleep(100);
                    Report_Window_5 Result_Window = new Report_Window_5();
                    Result_Window.Show();
                }
                catch (Exception error)//(Exception e)
                {
                    MessageBox.Show("Port failed to be opened", application_name);
                    Console.WriteLine(error.Message);
                }
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ComboBox_Port6.Text) || ComboBox_Port6.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the Sensor 6 Port", application_name);
            }

            else if (string.IsNullOrEmpty(ComboBox_BaudRate6.Text) || ComboBox_BaudRate6.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the Sensor 6 Baudrate", application_name);
            }
            else
            {
                string comport = ComboBox_Port6.Text;
                if (Int32.TryParse(ComboBox_BaudRate6.Text, out int BaudRate))
                {
                    Console.WriteLine(BaudRate);
                }
                SerialPort mySerialPort = new SerialPort(comport);
                if (mySerialPort.IsOpen)
                {
                    Console.WriteLine("Port is open");
                    mySerialPort.Close();
                }
                mySerialPort.BaudRate = BaudRate;
                mySerialPort.Parity = Parity.None;
                mySerialPort.StopBits = StopBits.One;
                mySerialPort.DataBits = 8;
                mySerialPort.Handshake = Handshake.None;
                mySerialPort.RtsEnable = true;
                mySerialPort.Encoding = ASCIIEncoding.ASCII;
                try
                {
                    mySerialPort.Open();
                    Thread.Sleep(100);
                    mySerialPort.Close();
                    Thread.Sleep(100);
                    Report_Window_6 Result_Window = new Report_Window_6();
                    Result_Window.Show();
                }
                catch (Exception error)//(Exception e)
                {
                    MessageBox.Show("Port failed to be opened", application_name);
                    Console.WriteLine(error.Message);
                }
            }
        }
*/

        private void ButtonFechar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void GridBarraTitulo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Browse_Template_Click(object sender, RoutedEventArgs e)
        {
            //string addr_text = "File not found";
            string filename;
            string filepath;
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = false;
            //open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            bool? result = open.ShowDialog();
            if (result == true)
            {
                filename = System.IO.Path.GetFileName(open.FileName);
                filepath = open.FileName; // Stores Original Path in Textbox
                
                string targetdir = "C:\\Sensor_data\\DataConfig\\"+filename;
                //C:\Sensor_data\DataConfig
                File.Copy(filepath, targetdir);
                //Txt_LogoAddress.Text = filepath;
            }

        }
        private void Btn_Click_1(object sender, RoutedEventArgs e)
        {
            string sourceexcel_2003 = "C:\\Sensor_data\\DataConfig\\excel_template_revised.xls";
            if (!File.Exists(sourceexcel_2003))
            {
                MessageBox.Show("Please Select Excel Template First", application_name);
            }

            else
            {
                try
                {
                    Controller_View Controller_Window = new Controller_View();
                    Controller_Window.Show();
                }
                catch (Exception error)//(Exception e)
                {
                    MessageBox.Show("Connection to database failed. Please check router or comfile pi", application_name);
                    Console.WriteLine(error.Message);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }



    internal class ConsumoViewModel
    {
        public List<Consumo> Consumo { get; private set; }

        public ConsumoViewModel(Consumo consumo)
        {
            Consumo = new List<Consumo>();
            Consumo.Add(consumo);
        }
    }

    internal class Consumo
    {
        public string Titulo { get; private set; }
        public int Porcentagem { get; private set; }

        public Consumo()
        {
            Titulo = "Consumo Atual";
            Porcentagem = CalcularPorcentagem();
        }

        private int CalcularPorcentagem()
        {
            return 47; //Calculo da porcentagem de consumo
        }
    }
}
