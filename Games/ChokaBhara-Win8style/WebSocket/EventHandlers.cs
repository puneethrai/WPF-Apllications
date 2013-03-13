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
        public WebSocket.eWebSocketOpcode Opcode{ get; private set; }
        public Socket clientSocket{ get; private set; }
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
        public string Message{ get; private set; }
        
    }
    public class WebSocketClose : EventArgs
    {
        public WebSocketClose(string reason)
        {
            Reason = reason;
        }
        public string Reason{ get; private set; }
    }
    public class NewUser : EventArgs
    {
        public NewUser(Socket newUserSocket,string ExtraField)
        {
            this.newUserSocket = newUserSocket;
            this.ExtraField = ExtraField;
        }
        public Socket newUserSocket{get;private set;}
        public string ExtraField{get;private set;}
    }
    public class UserLeft : EventArgs
    {
        public UserLeft(Socket UserSocket)
        {
            this.UserSocket = UserSocket;
            
        }
        public Socket UserSocket { get; private set; }
    }
}
