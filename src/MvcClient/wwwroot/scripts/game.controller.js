'use strict';

(function () {
	angular.module('app').controller('GameController', [
		'$scope', '$routeParams', 'GamesService',
		function ($scope, $routeParams, GamesService) {
			var ctrl = this;
			ctrl.currentGame = null;
			$scope.board = {};
			$scope.moves = { 
				id: $routeParams['id'],
				get: function (pos, onSuccess) {
					GamesService.getMoves(ctrl.currentGame.Id, pos, onSuccess);
				}
			};

			$scope.onMoved = function (from, to) {
				GamesService.makeMove(ctrl.currentGame.Id, from, to, ctrl.setGame);
			}

			ctrl.setGame = function (game) {
				if (!game) return;
				ctrl.currentGame = game;
				ctrl.buildBoard();
			}

			ctrl.buildBoard = function () {
				$scope.board = {};
				var f = function (color) {
					return function (pieceString) {
						$scope.board[pieceString.substr(1, 2)] = { color: color, pieceChar: pieceString[0] };
					}
				}

				ctrl.currentGame.Black.PieceStrings.map(f('Black'));
				ctrl.currentGame.White.PieceStrings.map(f('White'));
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