using client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DPAPI___Wpf
{
    /// <summary>
    /// Interaction logic for AIWindow.xaml
    /// </summary>
    public partial class AIWindow : Window
    {
        
        SpeechSynthesizer mySpeechSynth = new SpeechSynthesizer();
        Choices lst = new Choices();
        SpeechRecognitionEngine myEngine;
        //Grammar myGrammar;
        bool IsListening = false;
        Chrome_History_Window window = null;
        //https://icanhazdadjoke.com/slack
        public AIWindow()
        {
                InitializeComponent();
                myEngine = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-us"));
                string[] arr = new string[] { "show me passwords", "close passwords window", "hey bamb", "hey cena", "tell mum to go away", "tell me a joke", "thank you", "what is your name", "thank you", "AIG", "avichay is gay", "thank you, close", "thank you, close the app", "sing the aig song", "stop server", "shut down server", "what is the date", "what is the time", "aig", "is avichay gay", "is avichay gay?", "check my installed voices", "change voice", "hello", "show", "me", "passwords", "history", "hi", "show me passwords", "show me history", "close history window", "close history", "close control window", "close rdp window", "show rdp", "start rdp", "start control window", "stop rdp", "close rdp", "cancel rdp" };
                //myGrammar = new Grammar(new GrammarBuilder(lst//, SubsetMatchingMode.SubsequenceContentRequired);//(new GrammarBuilder(lst));
                lst.Add(arr);
                foreach (var item in arr)
                {
                    GrammarBuilder gb = new GrammarBuilder(item, SubsetMatchingMode.SubsequenceContentRequired);
                    Grammar myGrammar = new Grammar(gb);
                    myGrammar.Enabled = true;
                    myEngine.LoadGrammarAsync(myGrammar);
                }
            #region Comments
            //Choices c = new Choices(new string[] { " ", "say", "speak"});
            //SemanticResultKey comType = new SemanticResultKey("comtype", c);
            //GrammarBuilder gbl = new GrammarBuilder();
            //gbl.Append(comType);
            //gbl.AppendDictation();

            //Grammar gr = new Grammar(gbl);
            //myEngine.LoadGrammar(gr);

            //mySpeechSynth.SelectVoice("Microsoft David Desktop");
            #endregion
            //SaySomething(string.Empty, 1);
            //mySpeechSynth.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Child);
            //mySpeechSynth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Teen);
            mySpeechSynth.SelectVoice("Microsoft Hazel Desktop");
                Dispatcher.Invoke(() => { lblResult.Text = "Welcome back, Ishay."; });

                mySpeechSynth.SpeakAsync("Welcome,back,Ishay.");

                //StartListeningForCommands();
            Thread myThread = new Thread(StartListeningForCommands);
            myThread.Start();
        }

        private SpeechRecognitionEngine LoadDictationGrammars()
        {

            // Create a default dictation grammar.
            DictationGrammar defaultDictationGrammar = new DictationGrammar();
            defaultDictationGrammar.Name = "default dictation";
            defaultDictationGrammar.Enabled = true;

            // Create the spelling dictation grammar.
            DictationGrammar spellingDictationGrammar =
              new DictationGrammar("grammar:dictation#spelling");
            spellingDictationGrammar.Name = "spelling dictation";
            spellingDictationGrammar.Enabled = true;

            // Create the question dictation grammar.
            DictationGrammar customDictationGrammar =
              new DictationGrammar("grammar:dictation");
            customDictationGrammar.Name = "question dictation";
            customDictationGrammar.Enabled = true;

            // Create a SpeechRecognitionEngine object and add the grammars to it.
            SpeechRecognitionEngine recoEngine = new SpeechRecognitionEngine();
            recoEngine.LoadGrammar(defaultDictationGrammar);
            recoEngine.LoadGrammar(spellingDictationGrammar);
            recoEngine.LoadGrammar(customDictationGrammar);

            // Add a context to customDictationGrammar.
            customDictationGrammar.SetDictationContext("hey cena", null);

            return recoEngine;
        }

        protected void GetRandomJoke()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                    //wc.DownloadStringAsync(new Uri(@"http://api.icndb.com/jokes/random"));
                    wc.DownloadStringAsync(new Uri(@"https://icanhazdadjoke.com/slack"));
                }
            }
            catch(Exception ex)
            {
                SaySomething(ex.Message);
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }
        private void StartListeningForCommands()
        {
            try
            {
                myEngine.RequestRecognizerUpdate();
                //myEngine.LoadGrammar(myGrammar);
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
                if (result == "hey bamb" || result == "hey cortana" || result == "hey cena")
                {
                    IsListening = true;
                    Dispatcher.Invoke(() => { lblResult.Text = "Yes?"; });
                    mySpeechSynth.SpeakAsync("Yes?");
                }
                if (mySpeechSynth.State != SynthesizerState.Speaking && IsListening)
                {
                    Dispatcher.Invoke(() => { lblSpokenText.Content = (result + Environment.NewLine); });

                    switch (result)
                    {
                        case "say":
                        case "speak":
                            SaySomething(result);
                            break;
                        case "hi":
                        case "hello":
                            SaySomething("Hey there !");
                            break;
                        case "close passwords window":
                            Dispatcher.Invoke(() => { Helper.CloseWindowOfWhichThereIsOnlyOne<ChromePasswordsWindow>(); SaySomething("On it!"); });
                            break;
                        case "show me passwords":
                            await Passwords();
                            SaySomething(",Okay buddy!Showing passwords.");
                            break;
                        case "sing the aig song":
                            SaySomething("Avichay is, oh yeah Avichay is, oh he is very very very very GAY!!! Oh yeah!");
                            break;
                        case "thank you, close the app":
                        case "close the app":
                            //case "thank you, close":
                            //"Shutting down the client!Thanks for using Ishay's Arti-ficial Intelligence Parental Control.");
                            Dispatcher.Invoke(() => { SaySomething("Too-da-loo!"); Thread.Sleep(900); Close(); });
                            break;
                        case "check my installed voices":
                            foreach (var item in mySpeechSynth.GetInstalledVoices())
                            {
                                string s = item.VoiceInfo.Name;//item.VoiceInfo.Description + "," + item.VoiceInfo.Gender + "," + item.VoiceInfo.Age;//, item.VoiceInfo.Gender, item.VoiceInfo.Age);
                                SaySomething(s);
                            }
                            break;
                        case "tell me a joke":
                            GetRandomJoke();
                            //SaySomething("Avichay is gay. Oh... wait, you didn't ask for a fact? sorry!");
                            break;
                        case "thank you":
                            SaySomething("Huh? It's nothing, honestly!");
                            break;
                        case "what is your name":
                            SaySomething("DROP TABLE USERS'); -- oh, I mean, uhm... Can you try that again?");
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
                        case "tell mom to go away":
                            SaySomething("Mum, go away!");
                            break;
                        case "show me history":
                            await GetHistory();
                            SaySomething(",Okay buddy!Showing history!");
                            break;
                        case "start rdp":
                        case "show rdp":
                        case "start control window":
                            await OpenRDP();
                            SaySomething("Started the rdp session.");
                            break;
                        case "stop rdp":
                        case "close rdp":
                        case "close control window":
                            SaySomething("Closed the rdp session!");

                            Dispatcher.Invoke(() => { Helper.CloseWindowOfWhichThereIsOnlyOne<Form1>(); });
                            break;
                        case "shut down server":
                        case "close server":
                        case "stop server":
                            SaySomething("Shutting down the server! Thank you for using Ishay's Artificial Intelligence Parental Control.");
                            await ShutDownServer();
                            break;
                        case "close history":
                            SaySomething("Sure, closing history window.");
                            Dispatcher.Invoke(() => { Helper.CloseWindowOfWhichThereIsOnlyOne<Chrome_History_Window>(); });
                            break;
                        default:
                            //SaySomething("If you've said anything,I didn't hear it.");
                            //SaySomething("Sorry? I didn't quite catch that...");
                            break;
                    }
                }

            });
        }



        protected void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string text = e.Result;
                // … do something with result
                dynamic json = JsonConvert.DeserializeObject(text);
                var r = json["attachments"][0]["text"];
                string joke = Convert.ToString(r);
                SaySomething(joke);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }
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
                        //gv.ItemsSource = dt.AsDataView();
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
        private async Task OpenRDP()
        {
            await Task.Run(() =>
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        Form1 form = new Form1();
                        form.Show();
                    });
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
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
                    //pBarTransferBytes.Value = 100;
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
                            //gv.ItemsSource = dt.AsDataView();
                            ChromePasswordsWindow pWin = new ChromePasswordsWindow(dt);
                            pWin.Show();
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

        protected async Task<string> GetRandomJokeAsync()
        {
            return await Task.Run(() =>
            {
                using (WebClient myClient = new WebClient())
                {
                    Uri r = new Uri("http://api.icndb.com/jokes/random");
                    object obj = null;
                    myClient.DownloadStringAsync(r, obj);
                    dynamic s = JsonConvert.DeserializeObject(obj.ToString());
                    return s.value["joke"];
                }
            });
        }

        protected void SaySomething(string phrase)
        {
            try
            {
                    if (phrase != "hey cena")
                    {
                        Dispatcher.Invoke(() => { lblResult.Text = phrase; });
                        mySpeechSynth.SpeakAsync(phrase);
                        //Thread.Sleep(300);
                        IsListening = false;
                    }
                    else
                    {
                        Dispatcher.Invoke(() => { lblResult.Text = "yes?"; });
                        mySpeechSynth.SpeakAsync("yes?");
                    }
                
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void lblSpokenText_AccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {

        }
        //private void StartListeningForCommands()
        //{

        //}
    }
}
