'use strict';

(function () {
	angular.module('app').controller('StartController', [
		'$scope', '$location', 'gamesService',
		function ($scope, $location, gamesService) {
			var ctrl = this;

			ctrl.onNewGame = function() {
				gamesService.newGame(function (game) {
					$location.path("/game/" + game.Id);
				})
			}

	}]);
})();
		