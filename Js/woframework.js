class WOFramework
{
	#socket;

	main()
	{
		this.#socket = new WebSocket("ws" + window.location.href.substring(window.location.href.indexOf("://")));
		this.#socket.onopen = this.#onOpen;
		this.#socket.onmessage = this.#onMessage;
		this.#socket.onclose = this.#onClose;
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
		this.send("ping");
	}

	#onMessage(event)
	{
		var data = JSON.parse(event.data);
		var requests = data["requests"]
		for (var i = 0; i < requests.length; i++)
		{
			var request = requests[i];
			var requestName = request["name"];
			if (requestName == "attribute-added")
				document.getElementById(request["id"]).setAttribute(request["attribute"], request["value"]);
			else if (requestName == "attribute-removed")
				document.getElementById(request["id"]).removeAttribute(request["attribute"]);
		}
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