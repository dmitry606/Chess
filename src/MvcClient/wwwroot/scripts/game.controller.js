'use strict';

(function () {
	angular.module('app').controller('GameController', [
		'$scope', '$routeParams', 'GamesService',
		function ($scope, $routeParams, GamesService) {
			var ctrl = this;
			ctrl.currentGame = null;
			buildBoard();

			function getMoves (pos, onSuccess) {
				GamesService.getMoves($routeParams['id'], pos, onSuccess);
			}

			function getCurrentColor() {
				return ctrl.currentGame.History.length % 2 == 0 ? 'White' : 'Black';
			}

			function buildBoard() {
				$scope.board = {
					getMoves: getMoves,
					getCurrentColor: getCurrentColor
				};

				if (!ctrl.currentGame)
					return;

				var f = function (color) {
					return function (pieceString) {
						$scope.board[pieceString.substr(1, 2)] = { color: color, pieceChar: pieceString[0] };
					}
				}

				ctrl.currentGame.Black.PieceStrings.map(f('Black'));
				ctrl.currentGame.White.PieceStrings.map(f('White'));
			}

			$scope.onMoved = function (from, to) {
				GamesService.makeMove($routeParams['id'], from, to, ctrl.setGame);
			}

			ctrl.setGame = function (game) {
				if (!game) return;
				ctrl.currentGame = game;
				buildBoard();
			}

			GamesService.getGame($routeParams['id'], function (newGame) {
				ctrl.setGame(newGame);
			});
		}]);

	//angular.module('app').directive("testDirective", function () {
	//	return {
	//		scope: {
	//			name: '=player',
	//		},
	//		template: '<h3 ng-click=\'name = "changed in directive"\'>This is a {{name}} turn</h3>',
	//	}
	//})

})();