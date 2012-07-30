using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Forms;
namespace WebSocket
{
    partial class WebSocketApp
    {
        DateTime Time = DateTime.Now;
            // This method will be called when the thread is started.
            public void DoWork()
            {
                Time = DateTime.Now;
                while (!_shouldStop)
                {
                    ServerTime.Text = "Server UpTime:" + (DateTime.Now -Time);
                    Thread.Sleep(1000);
                                       
                }
                Console.WriteLine("worker thread: terminating gracefully.");
            }
            public static void RequestStop()
            {
                _shouldStop = true;
                StopAllClient = true;
            }
            // Volatile is used as hint to the compiler that this data
            // member will be accessed by multiple threads.
            private static volatile bool _shouldStop;
            private static volatile bool StopAllClient = false;
            public static void AppExit(object sender, EventArgs e)
            {
                // Request that the worker thread stop itself:
                RequestStop();
                
                // Use the Join method to block the current thread 
                // until the object's thread terminates.
                workerThread.Join();
            }
            public void clientOperation()
            {
                Socket AcceptSoc = AcceptSocket;
                string DisEndPointAddress ="User Disconnected:" + AcceptSoc.RemoteEndPoint;
                PingKey.Add(AcceptSoc, false);
                while (!StopAllClient)
                {
                    
                    if (PingKey.ContainsKey(AcceptSoc))
                    {
                        PingFrame(AcceptSoc);
                        Thread.Sleep(5000);
                        if (PingKey.ContainsKey(AcceptSoc))
                        {
                            if (PingKey[AcceptSoc])
                            {
                                PingKey[AcceptSoc] = false;
                            }
                            else
                            {
                                SendToAllExceptOne(DisEndPointAddress, AcceptSoc, GetRoomInfo(AcceptSoc), true);
                                break;
                            }
                        }
                    }
                    else
                        break;
                }

            }
            public void CreateThread(Socket AcceptSocket)
            {
                Thread clientThreads;
                if (ShakeHands(AcceptSocket, ServerMessage))
                {
                    Console.WriteLine("Owner");
                    int RandNo = GenerateRoomNo();
                    RoomKey.Add(RoomIndex,RandNo);
                    SocketKey.Add(AcceptSocket,RoomIndex);
                    RoomIndex++;
                    AcceptSocket.Send(Send("RoomID:" + RandNo.ToString()));
                    if (Room_1.InvokeRequired)
                    {
                        Room_1.Invoke(new MethodInvoker(delegate
                        {
                            item.Add("Room " + (RoomIndex));
                            NewRoom = false;

                        }));
                    }
                    RoomFlag = true;
                }
                if (RoomFlag)
                {
                    AcceptSocket.BeginReceive(dataBuffer, 0, dataBuffer.Length, 0, Read, AcceptSocket);
                    // Create the thread object. This does not start the thread.
                    clientThreads = new Thread(clientOperation);

                    // Start the Client thread.
                    clientThreads.Start();
                    CurrentConectedUsers++;
                    Status.Text = "No of connection:" + CurrentConectedUsers;
                    Console.WriteLine("Waiting for other user");
                    RoomFlag = false;
                }
                else
                {

                    AcceptSocket.Send(Send("Invaild Room"));
                    AcceptSocket.Close();
                }

            }
        }
    
}
