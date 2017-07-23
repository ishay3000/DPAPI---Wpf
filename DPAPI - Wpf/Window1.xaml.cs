using client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static DPAPI___Wpf.Form1;
using System.Speech;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Threading;

namespace DPAPI___Wpf
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        SpeechSynthesizer mySpeechSynth = new SpeechSynthesizer();
        Choices lst = new Choices();
        SpeechRecognitionEngine myEngine;
        Grammar myGrammar;
        bool IsListening = false;
        Chrome_History_Window window = null;

        public Window1()
        {
            myEngine = new SpeechRecognitionEngine();
            lst.Add(new string[] { "what is the date", "what is the time", "aig", "is avichay gay", "is avichay gay?", "check my installed voices", "change voice", "hello", "show", "me", "passwords", "history", "hi" , "show me passwords", "show me history", "close history window", "close history", "close Control window", "close RDP window"});
            myGrammar = new Grammar(new GrammarBuilder(lst));
            //mySpeechSynth.SelectVoice("Microsoft David Desktop");
            mySpeechSynth.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Child);
            InitializeComponent();
            mySpeechSynth.SpeakAsync("Welcome back Ishay, What would you like to do?");
            StartListeningForCommands();
        }

        private void StartListeningForCommands()
        {
            try
            {
                myEngine.RequestRecognizerUpdate();
                myEngine.LoadGrammar(myGrammar);
                //myEngine.SpeechRecognized += MyEngine_SpeechRecognizedAsync;
                myEngine.SpeechRecognized += MyEngine_SpeechRecognized;
                myEngine.SetInputToDefaultAudioDevice();
                myEngine.RecognizeAsync(RecognizeMode.Multiple);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void MyEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Task.Run(async () =>
            {
                string result = e.Result.Text;
                IsListening = true;
                if (mySpeechSynth.State != SynthesizerState.Speaking)
                {


                    switch (result)
                    {
                        case "hi":
                        case "hello":
                            SaySomething("Hey there !");
                            break;
                        case "show me passwords":
                            await Passwords();
                            SaySomething(",Okay buddy!Showing passwords.");
                            break;
                        case "check my installed voices":
                            foreach (var item in mySpeechSynth.GetInstalledVoices())
                            {
                                string s = item.VoiceInfo.Name;//item.VoiceInfo.Description + "," + item.VoiceInfo.Gender + "," + item.VoiceInfo.Age;//, item.VoiceInfo.Gender, item.VoiceInfo.Age);
                                SaySomething(s);
                            }
                            break;
                        case "what is the time":
                            SaySomething("The time is " + DateTime.Now.ToLongTimeString());
                            break;
                        case "what is the date":
                            SaySomething("Today's " + DateTime.Now.ToLongDateString());
                            break;
                        case "change voice":
                            SaySomething("Sure, changing voice");
                            mySpeechSynth.SelectVoice("Microsoft David Desktop");
                            break;
                        case "show me history":
                            await GetHistory();
                            SaySomething(",Okay buddy!Showing history!");
                            break;
                        case "close history":
                            SaySomething("Sure, closing history window.");
                            window.Hide();
                            break;
                        default:
                            //SaySomething("Sorry? I didn't quite catch that...");
                            break;
                    }
                }
            });
        }

        protected void SaySomething(string phrase)
        {
            try
            {
                mySpeechSynth.SpeakAsync(phrase);
                IsListening = true;
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private async void MyEngine_SpeechRecognizedAsync(object sender, SpeechRecognizedEventArgs e)
        {
            await Task.Run(async () =>
            {
                string result = e.Result.Text;

                switch (result)
                {
                    case "hi":
                        SaySomething("Hey there !");
                        break;
                    case "show me passwords":
                        SaySomething("Sure thing ! Showing passwords!");
                        await Passwords();
                        break;
                    default:
                        break;
                }
            });
        }

        #region comments
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
        #endregion

        protected async Task GetHistory()
        {
            await Dispatcher.BeginInvoke(new Action(async () =>
            {
                //CircularProgressBar.CircularProgressBar bar = new CircularProgressBar.CircularProgressBar();
                //StackPanel myPanel = new StackPanel();
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
                        window = new Chrome_History_Window(dt);
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

        private async void BtnGetData_Click(object sender, RoutedEventArgs e)
        {
            await GetHistory();
            #region History Request Comments
            //await Dispatcher.BeginInvoke(new Action(async () =>
            //{
            //    //CircularProgressBar.CircularProgressBar bar = new CircularProgressBar.CircularProgressBar();
            //    //StackPanel myPanel = new StackPanel();
            //    //myPanel.Children.Add(bar);
            //    try
            //    {
            //        Dictionary<string, string> myDic = new Dictionary<string, string>()
            //{
            //    {"Command", "History" }
            //};
            //        string sendToSender = await Sender.send_and_get_message(JsonConvert.SerializeObject(myDic));
            //        dynamic result = JsonConvert.DeserializeObject(sendToSender);

            //        if (result.STATUS == "OK")
            //        {
            //            DataTable dt = JsonConvert.DeserializeObject<DataTable>((string)result.HistoryResult);
            //            gv.ItemsSource = dt.AsDataView();
            //            Chrome_History_Window window = new Chrome_History_Window(dt);
            //            window.Show();

            //        }
            //        else
            //        {

            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.WriteLine("[-----DEBUGGER----- " + ex.Message + "-----END DEBUGGER-----]");
            //    }
            //}));
            #endregion
        }
        public async Task UpdatePbar()
        {
            await Task.Run(() =>
            {
                while (Sender.IsComplete == false)
                {
                    myCirclBar.Dispatcher.Invoke(() =>
                    {
                        myCirclBar.Value = Sender.progBarValue;
                    });
                }
            });
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
            //await Application.Current.Dispatcher.BeginInvoke(new Action(async () =>
            //{
            //{
            //    {

            try
            {
                Task t = new Task(Sender.ProgBarUpdate);
                t.Start();
                await Passwords();
                //await UpdatePbar();
                
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
                        //await ProgBarProgress();
                        //System.Windows.Forms.MessageBox.Show(s);
                //    }
                //}
            //}));
            //await Dispatcher.Invoke(async () =>
            //{
            
            //});
            #region Comments

            //await Dispatcher.BeginInvoke(new Action(async () =>
            //{
            //    try
            //    {
            //        Dictionary<string, string> myDic = new Dictionary<string, string>()
            //{
            //    {"Command", "Passwords" }
            //};
            //        string sendToSender = await Sender.send_and_get_message(JsonConvert.SerializeObject(myDic));
            //        dynamic result = JsonConvert.DeserializeObject(sendToSender);

            //        if (result.STATUS == "OK")
            //        {
            //            DataTable dt = JsonConvert.DeserializeObject<DataTable>((string)result.PasswordsResult);
            //            gv.ItemsSource = dt.AsDataView();
            //        }
            //        else
            //        {

            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.ToString());
            //        //Debug.WriteLine("[-----DEBUGGER----- " + ex.Message + "-----END DEBUGGER-----]");
            //    }
            //}));
            #endregion

        }

        private async Task ProgBarProgress()
        {
            await Task.Run(() =>
            {
                while (Sender.progBarValue != 100)
                {
                    //CircularPbar.Dispatcher.Invoke(() => { CircularPbar.Value = Sender.progBarValue; });//((Action).Value = Sender.progBarValue);
                    //CircularPbar.Text = Sender.progBarValue.ToString();
                    pBarTransferBytes.Dispatcher.Invoke(() => { pBarTransferBytes.Value = Sender.progBarValue; });
                }
            });


        }

        private async Task ShutDownServer()
        {
            await Task.Run(async () =>
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
            });
        }

        private async Task Passwords()
        {

                await Task.Run(async () =>
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
                            Dispatcher.Invoke(() =>
                            {
                                DataTable dt = JsonConvert.DeserializeObject<DataTable>((string)result.PasswordsResult);
                            gv.ItemsSource = dt.AsDataView();
                            });

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
                });
        }

        private async void btnShutServer_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.Invoke(async () =>
             {
                 await ShutDownServer();
             });
            #region comments
            //await Dispatcher.BeginInvoke(new Action(async () =>
            //{
            ////    try
            ////    {
            ////        Dictionary<string, string> myDic = new Dictionary<string, string>()
            ////{
            ////    {"Command", "Shut_Down" }
            ////};
            ////        string sendToSender = await Sender.send_and_get_message(JsonConvert.SerializeObject(myDic));
            ////        pBarTransferBytes.Value = 100;
            ////        dynamic result = JsonConvert.DeserializeObject(sendToSender);
            ////        if (result.STATUS == "OK")
            ////        {
            ////            MessageBox.Show("The server has successfully shut down.", "Shut Down Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            ////        }
            ////    }
            ////    catch (Exception ex)
            ////    {
            ////        Debug.WriteLine("[-----DEBUGGER----- " + ex.Message + "-----END DEBUGGER-----]");
            ////    }
            //}));
            #endregion
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