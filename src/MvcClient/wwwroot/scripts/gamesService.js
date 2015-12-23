'use strict';

var a = (function() { return 2 })();

(function () {
	angular
		.module('app')
		.factory('gamesService', ['$http', function ($http) {
			function newGame(onSuccess) {
				return $http.get('/api/games/new').success(onSuccess);
			}

			return {
				newGame: newGame
			}
		}])
})();