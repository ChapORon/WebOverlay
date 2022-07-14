function onLoad() {
    var b = document.getElementById('blink');
    setInterval(function() {
        b.style.display = (b.style.display == 'none' ? '' : 'none');
    }, 1000);
	
	let socket = new WebSocket("wss://javascript.info/article/websocket/demo/hello");

	socket.onopen = function(e) {
	  alert("[open] Connection established");
	  alert("Sending to server");
	  socket.send("My name is John");
	};

	socket.onmessage = function(event) {
	  alert(`[message] Data received from server: ${event.data}`);
	};

	socket.onclose = function(event) {
	  if (event.wasClean) {
		alert(`[close] Connection closed cleanly, code=${event.code} reason=${event.reason}`);
	  } else {
		// par exemple : processus serveur arrêté ou réseau en panne
		// event.code est généralement 1006 dans ce cas
		alert('[close] Connection died');
	  }
	};

	socket.onerror = function(error) {
	  alert(`[error] ${error.message}`);
	};
}