using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DPAPI___Wpf
{
    class Client
    {
        const int PORT_NO = 8080;
        const string SERVER_IP = "127.0.0.1";//Ignore this
        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
        public void DoClientStuff(string msg)
        {
            //---data to send to the server---
            string textToSend = msg;

            //---create a TCPClient object at the IP and port no.---
            TcpClient client = new TcpClient("10.0.0.8", PORT_NO);
            NetworkStream nwStream = client.GetStream();
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textToSend);
            //---send the text---
            Console.WriteLine("Sending : " + textToSend);
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);

            //---read back the text---

            //byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            //int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            //Console.WriteLine("Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
            //Console.ReadLine();
            client.Close();
        }
    }
}
