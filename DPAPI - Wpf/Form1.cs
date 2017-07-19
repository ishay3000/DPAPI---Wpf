using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MSTSCLib;
using RDPCOMAPILib;
using client;
using Newtonsoft.Json;
using System.Threading;
using System.Windows.Threading;

namespace DPAPI___Wpf
{
    public partial class Form1 : Form
    {
        private AxRDPCOMAPILib.AxRDPViewer rdp_viewer = new AxRDPCOMAPILib.AxRDPViewer();

        //The connection string for the rdp viewer.
        //private string connection_string;
        Thread myThread = null;

        public Form1()
        {
            //connection_string = conn;
            InitializeComponent();
            //myThread = new Thread(some.ConnectToRDP);
            //myThread.Start();

            //myThread = new Thread(ConnectToRDP);
            //myThread.Start();
            startDispatcherRDP();
        }
        private void startDispatcherRDP()
        {
            Dispatcher.CurrentDispatcher.Invoke(() => {

                myThread = new Thread(ConnectToRDP);
                myThread.Start();
            });
        }
        public static string myConn;
        public async void ConnectToRDP()
        {
            Dictionary<string, string> myDic = new Dictionary<string, string>()
            {
                {"Command", "RDP_SESSION" }
            };
            try
            {
                string sendToSender = await Sender.send_and_get_message(JsonConvert.SerializeObject(myDic));
                dynamic result = JsonConvert.DeserializeObject(sendToSender);

                if (result.STATUS == "OK")
                {
                    myConn = result.CONNSTRING;
                    axRDPViewer1.Connect((string)result.CONNSTRING, "User1", "");
                    axRDPViewer1.SmartSizing = true;

                    //Maximizes the window size. 
                    this.WindowState = FormWindowState.Maximized;
                    //while (axRDPViewer1.IsDisposed)
                    //{
                    //    if (!axRDPViewer1.IsDisposed)
                    //    {
                    //        axRDPViewer1.Connect((string)result.CONNSTRING, "User1", "");

                    //    }
                    //}
                }
                else
                {
                    MessageBox.Show("Couldn't connect to the RDP Server");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void RDP_viewer_Load(object sender, EventArgs e)
        {
            //try
            //{
            //    //Connects to the RDP session.
            //    axRDPViewer1.Connect(myConn, "", "");
            //    axRDPViewer1.SmartSizing = true;

            //    //Maximizes the window size. 
            //    this.WindowState = FormWindowState.Maximized;
            //}
            //catch
            //{
            //    MessageBox.Show("Error");
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //Dictionary<string, string> myDic = new Dictionary<string, string>()
            //{
            //    {"Command", "RDP_SESSION" }
            //};
            //try
            //{
            //    string sendToSender = Sender.send_and_get_message(JsonConvert.SerializeObject(myDic));
            //    dynamic result = JsonConvert.DeserializeObject(sendToSender);

            //    if (result.STATUS == "OK")
            //    {
            //        axRDPViewer1.Connect((string)result.CONNSTRING, "User1", "");
            //        while (axRDPViewer1.IsDisposed)
            //        {
            //            if (!axRDPViewer1.IsDisposed)
            //            {
            //                axRDPViewer1.Connect((string)result.CONNSTRING, "User1", "");

            //            }
            //        }
            //    }
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}

            //string Invitation = textBox1.Text;//Interaction.InputBox("Insert Invitation ConnectionString", "Attention");
            //dynamic myConnString = JsonConvert.DeserializeObject<string>(Sender.send_and_get_message(JsonConvert.SerializeObject("RDP_SESSION")));


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
