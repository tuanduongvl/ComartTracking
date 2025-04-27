using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net;
using System.Net.Sockets;

namespace ComartTracking
{
    public class CodeReader
    {
        public event EventHandler<string> OnCodeRead;
        TcpClient client;
        System.Timers.Timer timer;
        string code = "";
        string IPaddress = "192.168.1.100";
        int port = 9004;
        bool onOff = true;
        NetworkStream stream;
        public CodeReader()
        {
            //initialize the code reader
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = false;
            //timer.Start();
            client = new TcpClient();
            client.Connect(IPaddress, port);
            stream = client.GetStream();
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        if (!client.Connected)
                        {
                            client.Close();
                            client = new TcpClient();
                            client.Connect(IPaddress, port);
                            stream = client.GetStream();
                        }
                        if (stream.DataAvailable)
                        {
                            byte[] buffer = new byte[100];
                            int bytesRead = stream.Read(buffer, 0, buffer.Length);
                            code = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            OnCodeRead?.Invoke(this, code);
                        }

                    }
                    catch (Exception e)
                    { }
                }
            });
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (client.Connected)
            {
                if (onOff)
                {
                    byte[] buffer = Encoding.ASCII.GetBytes("LOFF\r");
                    stream.Write(buffer, 0, buffer.Length);
                    onOff = false;
                }
                else
                {
                    byte[] buffer = Encoding.ASCII.GetBytes("LON\r");
                    stream.Write(buffer, 0, buffer.Length);
                    onOff = true;
                }
            }

        }
    }
}
