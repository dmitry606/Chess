function ModelEvent(sender) {
	this._sender = sender;
	this._listeners = [];
}

ModelEvent.prototype = {
	subscribe: function (listener) {
		this._listeners.push(listener);
	},
	raise: function (args) {
		this._listeners.forEach(function (el) { el(this._sender, args); });
	}
};