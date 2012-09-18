using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketClient
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(string message,WebSocket.eWebSocketOpcode Op)
        {
            Message = message;
            Opcode = Op;
        }

        public string Message { get; private set; }
        public WebSocket.eWebSocketOpcode Opcode;
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
}
