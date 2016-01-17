'use strict';

(function () {
	angular.module('app').controller('GameController', [
		'$scope', '$routeParams', 'gamesService',
		function ($scope, $routeParams, gamesService) {
			var ctrl = this;
			ctrl.currentGame = null;

			ctrl.setGame = function (game){
				ctrl.currentGame = game;
				ctrl.refresh();
			}

			ctrl.refresh = function () {
			}

			ctrl.getPieceSrc = function (pos) {
				if(null == ctrl.currentGame)
					return '';

				var p = ctrl.currentGame.Board.Black.PieceStrings.find(function (s) { return s.substr(1, 2) == pos; });
				if (p != null) {
					return 'img/black/' + p[0] + '.png';
				}
				p = ctrl.currentGame.Board.White.PieceStrings.find(function (s) { return s.substr(1, 2) == pos; });
				if (p != null) {
					return 'img/white/' + p[0] + '.png';
				}

				return '';
			}

			gamesService.getGame($routeParams['id'], function (newGame) {
				ctrl.setGame(newGame);
			});


		}]);

})();