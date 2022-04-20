using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Console = System.Diagnostics.Debug;

namespace SharpConstraintLayout.Maui.DebugTool
{
    /// <summary>
    /// Data stream
    /// </summary>
    /// <param name="stream"></param>
    internal delegate void AcceptStreamHandler(MemoryStream stream);

    /// <summary>
    /// Accept message from server
    /// </summary>
    internal class MessageClient
    {
        private Socket m_sock;  // Server connection
        private byte[] m_byBuff = new byte[262144];    // Recieved data buffer

        internal event AcceptStreamHandler AcceptedStreamEvent;              // Add Message Event handler for Form

        public string ServerIPAddressText;
        public int ServerPort;

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="port"></param>
        public MessageClient(string serverIp, int port)
        {
            ServerIPAddressText = serverIp;
            ServerPort = port;
            Connect();
        }

        /// <summary>
        /// 连接
        /// </summary>
        public void Connect()
        {
            try
            {
                // Close the socket if it is still open
                if (m_sock != null && m_sock.Connected)
                {
                    m_sock.Shutdown(SocketShutdown.Both);
                    System.Threading.Thread.Sleep(10);
                    m_sock.Close();
                }

                // Create the socket object
                m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Define the Server address and port
                IPEndPoint epServer = new IPEndPoint(IPAddress.Parse(ServerIPAddressText), ServerPort);

                // Connect to the server blocking method and setup callback for recieved data
                // m_sock.Connect( epServer );
                // SetupRecieveCallback( m_sock );

                // Connect to server non-Blocking method
                m_sock.Blocking = true;// false;
                AsyncCallback onconnect = new AsyncCallback(OnConnect);
                m_sock.BeginConnect(epServer, onconnect, m_sock);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ",Server Connect failed!");
            }
        }

        private void OnConnect(IAsyncResult ar)
        {
            // Socket was the passed in object
            Socket sock = (Socket)ar.AsyncState;
            // Check if we were sucessfull
            try
            {
                //sock.EndConnect( ar );
                if (sock.Connected)
                {
                    AsyncCallback recieveData = new AsyncCallback(OnRecievedData);//回调
                    sock.BeginReceive(m_byBuff, 0, m_byBuff.Length, SocketFlags.None, recieveData, sock);//接受数据,植入回调
                }
                else
                    Console.WriteLine("Unable to connect to remote machine," + "Connect Failed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ",Unusual error during Connect!");
            }
        }

        /// <summary>
        /// Send the Message in the Message area. Only do this if we are connected
        /// </summary>
        public void SendMessage(string messageText)
        {
            // Check we are connected
            if (m_sock == null || !m_sock.Connected)
            {
                //Console.WriteLine("Must be connected to Send a message");
                return;
            }

            // Read the message from the text box and send it
            try
            {
                // Convert to byte array and send.
                //Byte[] byteDateLine = Encoding.ASCII.GetBytes(NeedSendMessageText.ToCharArray());//改成支持中文
                byte[] byteDateLine = Encoding.UTF8.GetBytes(messageText.ToCharArray());
                m_sock.Send(byteDateLine, byteDateLine.Length, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " Send Message Failed!");
            }
        }

        MemoryStream stream;
        private void OnRecievedData(IAsyncResult ar)
        {
            // Socket was the passed in object
            Socket sock = (Socket)ar.AsyncState;

            // Check if we got any data
            try
            {
                int nBytesRec = sock.EndReceive(ar);
                if (nBytesRec > 0)
                {
                    // Wrote the data to the List
                    string sRecieved = Encoding.ASCII.GetString(m_byBuff, 0, nBytesRec);
                    int length = int.Parse(sRecieved);//第一次接受的是数据总长度

                    //不每次回调,该次传输就在这里取完
                    Console.WriteLine("*** Start accept dll at {0} ***", DateTime.Now.ToString("G"));

                    sock.Send(Encoding.ASCII.GetBytes("OK"));//告诉服务器准备接受

                    if (stream != null)
                    {
                        stream.Close();
                        stream = null;
                    }
                    stream = new MemoryStream();

                    int acceptedCount = 0;
                    int byteCount = 0;
                    do
                    {
                        byteCount = sock.Receive(m_byBuff, m_byBuff.Length, SocketFlags.None);//接收数据长度
                        stream.Write(m_byBuff, 0, byteCount);//写入流
                        //stream.Flush();//缓冲写入文件

                        acceptedCount = acceptedCount + byteCount;
                    }
                    while (byteCount > 0 && acceptedCount < length);
                    Console.WriteLine("接收长度:" + length);

                    Console.WriteLine("*** Accept dll success at {0} ***", DateTime.Now.ToString("G"));
                    AcceptedStreamEvent.Invoke(stream);

                    //下次回调
                    // If the connection is still usable restablish the callback
                    OnConnect(ar);
                }
                else
                {
                    // If no data was recieved then the connection is probably dead
                    Console.WriteLine("Client {0}, disconnected", sock.RemoteEndPoint);
                    sock.Shutdown(SocketShutdown.Both);
                    sock.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ",Unusual error druing Recieve!");
            }
        }

        /// <summary>
        /// Close the Socket connection before going home
        /// </summary>
        public void Close()
        {
            if (m_sock != null && m_sock.Connected)
            {
                m_sock.Shutdown(SocketShutdown.Both);
                m_sock.Close();
                Console.WriteLine("连接关闭");
            }
        }
    }
}