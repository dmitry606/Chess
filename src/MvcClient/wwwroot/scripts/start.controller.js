'use strict';

(function () {
	angular.module('app').controller('StartController', [
		'$scope', '$location', 'GamesService',
		function ($scope, $location, GamesService) {
			var ctrl = this;

			ctrl.onNewGame = function() {
				GamesService.newGame(function (game) {
					$location.path("/game/" + game.Id);
				})
			}

	}]);
})();
		