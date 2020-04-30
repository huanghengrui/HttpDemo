using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace RenLianShiBie
{
    class NetworkManager
    {
        public string ErrorStr;
        IPEndPoint remoteEP;
        
        public NetworkManager()
        {
            ErrorStr = "";
        }

        public bool ConncectCheck(String ipaddr, int port)
        {
            Socket remoteSocket;

            try
            {
                IPAddress ipAddress = IPAddress.Parse(ipaddr);
                remoteEP = new IPEndPoint(ipAddress, (int)port);
                remoteSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                remoteSocket.Connect(remoteEP);
                remoteSocket.Close();
            }
            catch (Exception er)
            {
                ErrorStr = er.Message;
                return false;
            }

            return true;
        }

        public byte[] SendAndReceive(byte[] data)
        {
            MemoryStream readStream = new MemoryStream();
            Socket remoteSocket;

            try
            {
                remoteSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                remoteSocket.Connect(remoteEP);
                // first request command send.
                do
                {

                    try
                    {
                        if (remoteSocket.Poll(50, SelectMode.SelectWrite))
                        {
                            int bytesSent = remoteSocket.Send(data);
                            if (bytesSent > 0) break;
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorStr = e.Message;
                        remoteSocket.Close();
                        return null;
                    }
                    Thread.Sleep(100);
                } while (true);

                // second response data read.
                int TimeOut = 0;
                
                byte[] readByte = new byte[1024];
                do
                {
                    Thread.Sleep(100);
                    try
                    {
                        if (remoteSocket.Poll(50, SelectMode.SelectRead))
                        {
                            int bytesRecv = remoteSocket.Receive(readByte);
                            if (bytesRecv > 0)
                            {
                                readStream.Write(readByte, 0, readByte.Length);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorStr = e.Message;
                        remoteSocket.Close();
                        return null;
                    }
                    if (++TimeOut >= 10)
                    {
                        ErrorStr = "Device not responding";
                        remoteSocket.Close();
                        return null;
                    }
                } while (true);
                remoteSocket.Close();
            }
            catch (Exception er)
            {
                ErrorStr = er.Message;
                
                return null;
            }


            return readStream.GetBuffer();
        }

        public bool Disconnect()
        {
            return true;
        }
    }
}
