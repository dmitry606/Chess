'use strict';

(function () {
	var apiBase = '/api/join/';

	function JoinInstance(id, http) {
		this.id = id;
		this.$http = http;
		this.updated = new ModelEvent(this);
		this.currentStatus = [1, 1];
	}

	JoinInstance.prototype = {
		start: function() {
			var promise = this.$http.get(apiBase + this.id);
			wireup(promise, this);
		},

		join: function(color) {
			var promise = this.$http.patch(apiBase + this.id + '?color=' + colorToInt(color));
			wireup(promise, this);
		},

		isJoined: function(color) {
			return this.currentStatus[colorToIndex(color)] == 2;
		},

		isOpen: function(color) {
			return this.currentStatus[colorToIndex(color)] == 1;
		},
	}

	function colorToIndex(color) {
		return color.toLowerCase() == 'white' ? 0 : 1;
	}

	function colorToInt(color) {
		return color.toLowerCase() == 'white' ? 1 : 2;
	}

	function wireup(promise, me) {

		var success = function (res) {
			me.currentStatus = res.data;
			//console.log("Join. data received: " + res.data);
			me.updated.raise();
		}

		var error = function (res) {
		}

		promise.then(success, error);
	}

	var _cached = new Map();

	angular
		.module('app')
		.factory('JoinFactory', function ($routeParams, $http) {
			return {
				getForCurrentRoute: function () {
					var id = $routeParams['id'];
					var instance = _cached[id];
					if (!instance) {
						instance = new JoinInstance(id, $http);
						_cached[id] = instance;
					}
					return instance;
				}
			}
		});
})();