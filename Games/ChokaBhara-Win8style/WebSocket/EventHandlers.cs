using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
namespace WebSocketServer
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(string message, WebSocket.eWebSocketOpcode Op, Socket clientsocket)
        {
            Message = message;
            Opcode = Op;
            clientSocket = clientsocket;
        }

        public string Message { get; private set; }
        public WebSocket.eWebSocketOpcode Opcode;
        public Socket clientSocket;
    }
    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(Exception ex)
        {
            exception = ex;
            Message = exception.Message;
        }

        public Exception exception { get; private set; }
        public string Message { get; private set; }

    }
    public class WebSocketOpened : EventArgs
    {
        public WebSocketOpened()
        {
        }
    }
    public class DebugMessages : EventArgs
    {
        public DebugMessages(string message)
        {
            Message = message;
        }
        public string Message;
        
    }
    public class WebSocketClose : EventArgs
    {
        public WebSocketClose(string reason)
        {
            Reason = reason;
        }
        public string Reason;
    }
    public class NewUser : EventArgs
    {
        public NewUser(Socket newUserSocket)
        {
            this.newUserSocket = newUserSocket;
        }
        public Socket newUserSocket;
    }
}
