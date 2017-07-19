﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json;
using DPAPI___Wpf;

namespace client
{
    /// <summary>
    /// A class to handle the messages with the server.
    /// </summary>
    static class Sender
    {
        /// <summary>
        /// The server's default ip and port.
        /// </summary>
        private static string server_ip = "10.0.0.6";
        private static int server_port = 8080;
        

        public static string Server_ip
        {
            get
            {
                return server_ip;
            }
            set
            {
                server_ip = value;
            }
        }

        public static int Server_port
        {
            get
            {
                return server_port;
            }
            set
            {
                server_port = value;
            }
        }

        /// <summary>
        ///  Sends a text message to the server and gets a response.
        /// </summary>
        /// <param name="message">A string message to the server.</param>
        /// <returns>A string from the server.</returns>
        public async static Task<string> send_and_get_message(string message, int time_out = 1000000000)
        {
            return await Task.Run(() => { 
            int tmpProggressBarPercent = 0;
            try
            {
                TcpClient client = new TcpClient(server_ip, server_port);
               
                //---open the connection send the message---
                NetworkStream nwStream = client.GetStream();
                client.ReceiveTimeout = time_out ;
                
                byte[] bytesToSend = Encoding.UTF8.GetBytes(message);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);

                // ---get the message from the server----
               
                //A list and an array for the message's bytes.
                List <byte> list_byte_to_read = new List<byte>();
                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
             
                //Gets the data in chuncks.
                while (nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize) > 0)
                {
                    tmpProggressBarPercent = (bytesToRead.Length);
                    int tmp = 0;
                    foreach (byte b in bytesToRead)
                    {
                        if (b != 0)
                        {
                            tmp++;
                        }
                    }
                    //Appends only the not-null bytes.
                    foreach (byte b in bytesToRead)
                        if (b != 0)
                        {
                            list_byte_to_read.Add(b);
                            //Application.Current.Dispatcher.Invoke(() => { ((Window1)Application.Current.MainWindow).pBarTransferBytes.Value = (double)((list_byte_to_read.Count / tmpProggressBarPercent) * 100.0); });
                            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => ((Window1)Application.Current.MainWindow).pBarTransferBytes.Value = (double)((list_byte_to_read.Count / tmp)) * 100));//() => { ((Window1)Application.Current.MainWindow).pBarTransferBytes.Value = (double)((list_byte_to_read.Count / tmp)) * 100.0; });
                            //Application.Current.Dispatcher.Invoke(() => { ((Window1)Application.Current.MainWindow).pBarTransferBytes.UpdateLayout(); });
                        }
                                        
                    bytesToRead = new byte[client.ReceiveBufferSize];
                }

                //Converts the list of bytes to a string.
                string respond = Convert.ToString(JsonConvert.DeserializeObject(Encoding.UTF8.GetString(list_byte_to_read.ToArray(), 0, list_byte_to_read.Count())));

                client.Close();
                return respond;

            }
            catch (Exception crap)
            {
                return crap.ToString();
            }
            });
        }

        /// <summary>
        /// Checks if the server is up and llistening on the specific port.
        /// </summary>
        /// <param name="time_out">The timeout for the server's respond.</param>
        /// <returns>true if the server is up and running or false otherwise.</returns>
        public static bool Is_up(int time_out)
        {
            using (var tcp = new TcpClient())
            {
                try
                {
                    tcp.ReceiveTimeout = time_out;
                    tcp.Connect(server_ip, server_port);
                    tcp.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Checks if the given ip is valid.
        /// </summary>
        /// <returns></returns>
        public static bool Is_ip()
        {
            if (String.IsNullOrWhiteSpace(server_ip))
            {
                return false;
            }
            
            //Checks if there are 4 fields.
            string[] splitValues = server_ip.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;
           
            //Checks if all the fields are bytes(0-255)
            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

       
    }
}
