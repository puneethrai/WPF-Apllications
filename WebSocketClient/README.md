WebSocket Client for .Net
Usage:
Add WebSocketClient.dll present in WebSocketClient\WebSocketClient\bin\Debug to your project
add this namespace
using WebSocketClient;

in code use
WebSocket ws = new WebSocket(new Uri("ws://localhost:8080/"));
ws.Error += new EventHandler<WebSocketClient.ErrorEventArgs>(ws_Error);
ws.Opened += new EventHandler(ws_Opened);
ws.MessageReceived += new EventHandler<MessageReceivedEventArgs>(ws_MessageReceived);
ws.DebugMessage += new EventHandler<DebugMessages>(ws_DebugMessage);
ws.Closed += new EventHandler<WebSocketClose>(ws_Closed);
ws.BeginConnect();

void ws_Opened(object sender, EventArgs e)
{
	ws.Send("Hello World");
}