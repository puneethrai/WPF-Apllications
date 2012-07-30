var io = require('socket.io').listen(55555);

var conf=50;
var roomLimit=3;
var confcount=1;
var roomcount=1;
var room=new Array();

/**
*Event for accepting connections from socket.io clients 
*/
io.sockets.on('connection', function (socket) {
	
	socket.on('getRoom', function (data) {
		console.log("New Connection");
		socket.join(confcount);
		socket.emit('getRoom', {roomID:confcount,userID:roomcount});
		roomcount++;
		if(roomcount%roomLimit==0){
			confcount++;
			console.log("Next User will get new conference Conf No:"+confcount);
		}

	});
	socket.on('test', function (data) {
		console.log("Incoming Data from "+data.roomID+" Message:"+data.msg);
		socket.broadcast.to(data.roomID).emit('test',data.msg);
		//console.log("Broadcasted the messsage");
	});	
});