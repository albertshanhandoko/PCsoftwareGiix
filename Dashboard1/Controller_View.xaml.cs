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
using System.Windows.Shapes;
using System.Windows.Threading;
using Dashboard1.Helper;
using Dashboard1.Constant;
using Dashboard1.Library;
using System.Configuration;
using System.Media;

namespace Dashboard1
{
    /// <summary>
    /// Interaction logic for Controller_View.xaml
    /// </summary>
    public partial class Controller_View : Window
    {
        Sql_Measure_Batch Current_Sensor_Batch = new Sql_Measure_Batch();

        Sql_Measure_Batch Sensor_Batch = new Sql_Measure_Batch();

        //string IP_Address_Input = "192.168.0."+ ((MainWindow)Application.Current.MainWindow).txtblock_sensor1.Text.Last();
        string IP_Address_Input = "192.168.0.4"; //this is only for testing
        int lastbatchid = 0;
        //string IP_Address_Input = "192.168.0.9";

        List<Sql_Measure_Result> List_Measure_Average = new List<Sql_Measure_Result> { };
        List<Sql_Measure_Result> List_Measure_Average_new = new List<Sql_Measure_Result> { };
        List<Data_PDFHistory> List_PDF_Histories = new List<Data_PDFHistory> { };
        List<SQL_Data_Config> List_Data_Configs = new List<SQL_Data_Config> { };


        static string application_name = ConfigurationManager.AppSettings["application_name"] ?? "Not Found";
        float total_average = 0;
        float final_average = 0;
        int counter_print = 0;

        // Thereshold measure
        int thereshold_counter = 0;

        public Controller_View()
        {
            InitializeComponent();
            data_initialization();
            

            //  DispatcherTimer setup
            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            dispatcherTimer.Start();

        }
        private void data_initialization()
        {
            try
            {
                Current_Sensor_Batch = Sensor_input_Helper.MySql_Get_Average(IP_Address_Input);
                lastbatchid = Current_Sensor_Batch.batch_measure_ID_cls;
                txt_date.Text = DateTime.Now.ToString();
                //txt_date.Text = Sensor_Batch.start_date_cls;
                txt_application.Text = Current_Sensor_Batch.product_cls;
                txt_TotInterval.Text = Current_Sensor_Batch.total_interval_cls.ToString();
                txt_TotPCS.Text = (Current_Sensor_Batch.total_interval_cls * Current_Sensor_Batch.number_per_interval_cls).ToString();
                txt_Temperature.Text = Current_Sensor_Batch.temperature_cls;

                txt_date.IsEnabled = false;
                txt_application.IsEnabled = false;
                txt_TotInterval.IsEnabled = false;
                txt_TotPCS.IsEnabled = false;
                txt_Temperature.IsEnabled = false;

            }
            catch (Exception error)//(Exception e)
            {
                MessageBox.Show(error.ToString(), application_name);
                Console.WriteLine(error.Message);
            }

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            // This will tricker every second

            //Sensor_Batch = Sensor_input_Helper.MySql_Get_Measure(IP_Address_Input);
            try
            {
                Sensor_Batch = Sensor_input_Helper.MySql_Get_Average(IP_Address_Input);
                List_PDF_Histories = Sensor_input_Helper.MySql_Get_PrintPDF(IP_Address_Input);
                List_Data_Configs = Sensor_input_Helper.MySql_Get_DataConfig(IP_Address_Input);

              
                
                //txt_date.Text = Sensor_Batch.start_date_cls;
                

                var thereshold_Max_var = List_Data_Configs.Find(item => item.Config_Param == "Thereshold_Max").Config_Value;
                var thereshold_Min_var = List_Data_Configs.Find(item => item.Config_Param == "Thereshold_Min").Config_Value;

                double thereshold_Max = double.Parse(thereshold_Max_var.ToString());
                double thereshold_Min = double.Parse(thereshold_Min_var.ToString());

                //var theresholdmax = 
                //ListFilesToProcess.Count(item => item.IsChecked);
                double number_of_thereshold_max = Sensor_Batch.List_Measure_Result.Count(item => item.measure_result_cls > thereshold_Max);
                double number_of_thereshold_min = Sensor_Batch.List_Measure_Result.Count(item => item.measure_result_cls < thereshold_Min);

                // error
                string Error_Message = string.Empty;
                if (Sensor_Batch.Error_code_cls != "" && Sensor_Batch.Error_code_cls != string.Empty)
                {
                    Error_Sensor_PC enum_ErrorCode = (Error_Sensor_PC)Enum.Parse(typeof(Error_Sensor_PC), "error" + Sensor_Batch.Error_code_cls);
                    Error_Message = Sensor_input_Helper.GetDescription(enum_ErrorCode);
                }

                //MessageBox.Show(this, Error_Message, application_name);
                //
                // 1. check if batch is new
                // 2. if batch is not new , compare count. if new get all data
                // 3. if there is new average data, add ONLY the new one to the old one.
                if (lastbatchid == Sensor_Batch.batch_measure_ID_cls) // 1
                {
                    int current_countaverage = List_Measure_Average.Count;
                    int last_countaverage = Sensor_Batch.List_Average_Result.Count;
                    int selisih = last_countaverage - current_countaverage;
                    for (int i = selisih; i > 0; i--) // 2
                    {
                        List_Measure_Average.Add(Sensor_Batch.List_Average_Result[Sensor_Batch.List_Average_Result.Count - i]); // 3
                    }
                    //List_Measure_Average = Current_Sensor_Batch.List_Average_Result;
                }
                // klo ganti batch ngapain
                else
                {
                    txt_date.Text = DateTime.Now.ToString();
                    txt_application.Text = Sensor_Batch.product_cls;
                    txt_TotInterval.Text = Sensor_Batch.total_interval_cls.ToString();
                    txt_TotPCS.Text = (Sensor_Batch.total_interval_cls * Sensor_Batch.number_per_interval_cls).ToString();
                    txt_Temperature.Text = Sensor_Batch.temperature_cls;

                    List_Measure_Average = Sensor_Batch.List_Average_Result;
                    lastbatchid = Sensor_Batch.batch_measure_ID_cls;

                }

                final_average = total_average / List_Measure_Average.Count();
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    txt_FinalAverage.Text = final_average.ToString();
                    //Average_Grid.ItemsSource = List_Measure_Average;
                    DataContext = List_Measure_Average_new;
                    DataContext = List_Measure_Average;
                    Average_Grid.ItemsSource = List_Measure_Average_new; ;

                    Average_Grid.ItemsSource = List_Measure_Average;
                }));
                total_average = 0;
                foreach (Sql_Measure_Result Measure_Average in List_Measure_Average)
                {
                    total_average = total_average + Measure_Average.measure_result_cls;
                }

                bool check_need_to_print = Sensor_input_Helper.is_batch_printed(IP_Address_Input, Sensor_Batch.batch_measure_ID_cls);
                if (check_need_to_print == true)
                {
                    string company_name = SensorHelper_2.read_config_name();
                    string company_addres = SensorHelper_2.read_config_addr();

                    SensorHelper_2.Generate_Controller_PDF_revised(company_name, company_addres, txt_supplier.Text, txt_PrintedBy.Text,
                            Sensor_Batch, 1);
                }
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    txt_FinalAverage.Text = final_average.ToString();

                    HistoryGrid1.ItemsSource = List_PDF_Histories;

                    HistoryGrid1.Columns[1].Header = "File Name";
                    TheresholdMax_TextBox.Text = thereshold_Max_var.ToString() + "%";
                    TheresholdMin_TextBox.Text = thereshold_Min_var.ToString() + "%";

                    NumOf_TheresholdMax_TextBox.Text = number_of_thereshold_max.ToString();
                    NumOf_TheresholdMin_TextBox.Text = number_of_thereshold_min.ToString();

                    Error_TextBox.Text = Error_Message;
                }));

                thereshold_counter = Sensor_Batch.List_Measure_Result.Count;
            }
            catch (Exception error)//(Exception e)
            {
                MessageBox.Show(error.ToString(), application_name);
                Console.WriteLine(error.Message);
            }
            
            
        }

        private void Generate_PDF_Click(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrEmpty(txt_supplier.Text))
            {
                MessageBox.Show("Please enter Supplier Name", application_name);
            }

            //Printed By
            else if (String.IsNullOrEmpty(txt_PrintedBy.Text))
            {
                MessageBox.Show("Please enter Printed By", application_name);
            }
            else if(List_Measure_Average.Count() != Sensor_Batch.total_interval_cls )
            {
                MessageBox.Show("Measurement Not Finish", application_name);
            }
            else
            {
                try
                {
                    Sensor_Batch = Sensor_input_Helper.MySql_Get_Average(IP_Address_Input);
                    string company_name = SensorHelper_2.read_config_name();
                    string company_addres = SensorHelper_2.read_config_addr();

                    SensorHelper_2.Generate_Controller_PDF_revised(company_name,company_addres,txt_supplier.Text,txt_PrintedBy.Text,
                        Sensor_Batch,1);

                    MessageBox.Show("PDF has been successfully generated", application_name);

                }
                catch (Exception error)//(Exception e)
                {
                    MessageBox.Show(error.ToString(), application_name);
                    Console.WriteLine(error.Message);
                }
            }

        }

        private void txt_supplier_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txt_supplier_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void Error_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Average_Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void NumOf_TheresholdMax_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void NumOf_TheresholdMin_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txt_FinalAverage_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
