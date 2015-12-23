'use strict';

(function () {
	angular
		.module('app')
		.controller('MainController', ['$scope', 'gamesService', function ($scope, gamesService) {
			var main = this;
			main._game = null;

			main.onNewGame = function ($event) {
				main._game = gamesService.newGame(function (data) { main._game = data });

			};


		}]);

})();