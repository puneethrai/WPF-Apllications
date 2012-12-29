using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketServer;
namespace WebSocketServerTestDLL
{
    class Program
    {
        static void Main(string[] args)
        {
            //describe:Create new instance with default param
            //test result:passed need to test ping frame
            Console.WriteLine("Starting webserver");
            WebSocket ws = new WebSocket();
            Console.WriteLine("Websocketserver started");
            //describe:Create new instance with specified param
            //test result:passed need to test ping frame
            WebSocket fe = new WebSocket(5555, 10);
            Console.ReadLine();
        }
    }
}
