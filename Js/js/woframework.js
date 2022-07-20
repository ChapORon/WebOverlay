class WOFramework
{
	#socket;

	main()
	{
		this.#socket = new WebSocket("ws" + window.location.href.substring(window.location.href.indexOf("://")));
		this.#socket.onopen = this.#onOpen;
		this.#socket.onmessage = this.#onMessage;
		this.#socket.onclose = this.#onError;
		this.#socket.onerror = this.#onError;
	}

	stop(code = 1000, reason = "")
	{
		this.#socket.close(code, reason);
    }

	send(content)
	{
		this.#socket.send(content);
    }

	#onOpen(event)
	{
		alert("[open] Connection established");
		alert("Sending to server");
		this.send("My name is John");
	}

	#onMessage(event)
	{
		alert(`[message] Data received from server: ${event.data}`);
	}

	#onClose(event)
	{
		if (event.wasClean)
			alert(`[close] Connection closed cleanly, code=${event.code} reason=${event.reason}`);
		else
			alert('[close] Connection died');
	}

	#onError(error)
	{
		alert(`[error] ${error.message}`);
	}
};

let wof = new WOFramework();