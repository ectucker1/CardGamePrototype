extends Node


var _client = WebSocketClient.new()


signal closed(was_clean)
signal established(proto)
signal data_received(data)


func _ready():
	_client.connect("connection_closed", self, "_closed")
	_client.connect("connection_error", self, "_closed")
	_client.connect("connection_established", self, "_connected")
	_client.connect("data_received", self, "_on_data")

func connect_url(url: String) -> bool:
	var err = _client.connect_to_url(url)
	return err == OK

func disconnect_host():
	_client.disconnect_from_host()

func _closed(was_clean = false):
	emit_signal("closed", was_clean)

func _connected(proto = ""):
	emit_signal("established", proto)

func _on_data():
	emit_signal("data_received", _client.get_peer(1).get_packet())

func _process(delta):
	if _client.get_connection_status() == WebSocketClient.CONNECTION_CONNECTING or _client.get_connection_status() == WebSocketClient.CONNECTION_CONNECTED:
		_client.poll()
