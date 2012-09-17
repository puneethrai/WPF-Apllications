using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;

namespace WebSocketClient
{
    public class WebSocketClient
    {
        private Uri mUrl;
        private TcpClient mClient;
        private NetworkStream mStream;
        private bool mHandshakeComplete;
        private Dictionary<string, string> mHeaders;

        public WebSocketClient(Uri url)
        {
            mUrl = url;

            string protocol = mUrl.Scheme;
            if (!protocol.Equals("ws") && !protocol.Equals("wss"))
                throw new ArgumentException("Unsupported protocol: " + protocol);
        }

        public void SetHeaders(Dictionary<string, string> headers)
        {
            mHeaders = headers;
        }

        public void Connect()
        {
            string host = mUrl.DnsSafeHost;
            string path = mUrl.PathAndQuery;
            string origin = "http://" + host;

            mClient = CreateSocket(mUrl);
            mStream = mClient.GetStream();

            int port = ((IPEndPoint)mClient.Client.RemoteEndPoint).Port;
            if (port != 80)
                host = host + ":" + port;

            StringBuilder extraHeaders = new StringBuilder();
            if (mHeaders != null)
            {
                foreach (KeyValuePair<string, string> header in mHeaders)
                    extraHeaders.Append(header.Key + ": " + header.Value + "\r\n");
            }

            string request = "GET " + path + " HTTP/1.1\r\n" +
                             "Upgrade: WebSocket\r\n" +
                             "Connection: Upgrade\r\n" +
                             "Host: " + host + "\r\n" +
                             "Origin: " + origin + "\r\n" +
                             "Sec-WebSocket-Key: " + GenerateWebSocketHandshakeSecurityHash09() + "\r\n" +
                             extraHeaders.ToString() + "\r\n";
            byte[] sendBuffer = Encoding.UTF8.GetBytes(request);

            mStream.Write(sendBuffer, 0, sendBuffer.Length);

            StreamReader reader = new StreamReader(mStream);
            {
                string header = reader.ReadLine();
                if (!header.Equals("HTTP/1.1 101 Switching Protocols"))
                    throw new IOException("Invalid handshake response");

                header = reader.ReadLine();
                if (!header.Equals("Upgrade: websocket"))
                    throw new IOException("Invalid handshake response");

                header = reader.ReadLine();
                if (!header.Equals("Connection: Upgrade"))
                    throw new IOException("Invalid handshake response");
            }

            mHandshakeComplete = true;
        }
        /// <summary>
        /// Converts the given string to byte array of websocket protocol
        /// </summary>
        public byte[] Send(string SendData)
        {
            try
            {
                byte[] binary = Encoding.ASCII.GetBytes(SendData);
                ulong headerLength = 2;
                byte[] data = binary;

                bool mask = false;
                byte[] maskKeys = null;

                if (mask)
                {
                    headerLength += 4;
                    data = (byte[])data.Clone();

                    Random random = new Random(Environment.TickCount);
                    maskKeys = new byte[4];
                    for (int i = 0; i < 4; ++i)
                    {
                        maskKeys[i] = (byte)random.Next(byte.MinValue, byte.MaxValue);
                    }
                    for (int i = 0; i < data.Length; ++i)
                    {
                        data[i] = (byte)(data[i] ^ maskKeys[i % 4]);
                    }
                }

                byte payload;
                if (data.Length >= 65536)
                {
                    headerLength += 8;
                    payload = 127;
                }
                else if (data.Length >= 126)
                {
                    headerLength += 2;
                    payload = 126;
                }
                else
                {
                    payload = (byte)data.Length;
                }

                byte[] header = new byte[headerLength];

                header[0] = 0x80 | 0x1;
                if (mask)
                {
                    header[1] = 0x80;
                }
                header[1] = (byte)(header[1] | payload & 0x40 | payload & 0x20 | payload & 0x10 | payload & 0x8 | payload & 0x4 | payload & 0x2 | payload & 0x1);

                if (payload == 126)
                {
                    byte[] lengthBytes = BitConverter.GetBytes((ushort)data.Length).Reverse().ToArray();
                    header[2] = lengthBytes[0];
                    header[3] = lengthBytes[1];

                    if (mask)
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            header[i + 4] = maskKeys[i];
                        }
                    }
                }
                else if (payload == 127)
                {
                    byte[] lengthBytes = BitConverter.GetBytes((ulong)data.Length).Reverse().ToArray();
                    for (int i = 0; i < 8; ++i)
                    {
                        header[i + 2] = lengthBytes[i];
                    }
                    if (mask)
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            header[i + 10] = maskKeys[i];
                        }
                    }
                }
                mStream.Write(header.Concat(data).ToArray(), 0, header.Concat(data).ToArray().Length);
                mStream.Flush();
                return header.Concat(data).ToArray();

            }
            catch (Exception ex)
            {
                Console.WriteLine("websocket transport protocol Send exception: " + ex.ToString());
            }

            return null;
        }
        /*
        public void Send(string str)
        {
            if (!mHandshakeComplete)
                throw new InvalidOperationException("Handshake not complete");

            byte[] sendBuffer = Encoding.UTF8.GetBytes(str);

            mStream.WriteByte(0x00);
            mStream.Write(sendBuffer, 0, sendBuffer.Length);
            mStream.WriteByte(0xff);
            mStream.Flush();
        }*/
        private string GenerateWebSocketHandshakeSecurityHash09()
        {
            byte[] SecWebSocketKey = new byte[16];
            string sSecWebSocketKey = null;
            Random NextRandByte = new Random();
            NextRandByte.NextBytes(SecWebSocketKey);
            sSecWebSocketKey = Convert.ToBase64String(SecWebSocketKey);
            return sSecWebSocketKey;
        }
        public string Recv()
        {
            if (!mHandshakeComplete)
                throw new InvalidOperationException("Handshake not complete");

            StringBuilder recvBuffer = new StringBuilder();

            BinaryReader reader = new BinaryReader(mStream);
            byte b = reader.ReadByte();
            if ((b & 0x80) == 0x80)
            {
                // Skip data frame
                int len = 0;
                do
                {
                    b = (byte)(reader.ReadByte() & 0x7f);
                    len += b * 128;
                } while ((b & 0x80) != 0x80);

                for (int i = 0; i < len; i++)
                    reader.ReadByte();
            }

            while (true)
            {
                b = reader.ReadByte();
                if (b == 0xff)
                    break;

                recvBuffer.Append(b);           
            }

            return recvBuffer.ToString();
        }

        public void Close()
        {
            mStream.Dispose();
            mClient.Close();
            mStream = null;
            mClient = null;
        }

        private static TcpClient CreateSocket(Uri url)
        {
            string scheme = url.Scheme;
            string host = url.DnsSafeHost;

            int port = url.Port;
            if (port <= 0)
            {
                if (scheme.Equals("wss"))
                    port = 443;
                else if (scheme.Equals("ws"))
                    port = 80;
                else
                    throw new ArgumentException("Unsupported scheme");
            }

            if (scheme.Equals("wss"))
                throw new NotImplementedException("SSL support not implemented yet");
            else
                return new TcpClient(host, port);
        }
    }
}


