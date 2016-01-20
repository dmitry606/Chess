'use strict';

(function () {
	angular.module('app').controller('GameController', [
		'$scope', '$routeParams', 'gamesService',
		function ($scope, $routeParams, gamesService) {
			var ctrl = this;
			ctrl.currentGame = null;
			$scope.rows = [8, 7, 6, 5, 4, 3, 2, 1];
			$scope.columns = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];
			$scope.board = {};

			/*
			params
			action: function (color, pieceChar, pos);
			color: may be null
			*/
			function foreachPiece(action, color) {
				var f = function (innerColor) {
					ctrl.currentGame.Board[innerColor].PieceStrings.forEach(function (ps) {
						action(innerColor, ps[0], ps.substr(1, 2));
					});
				}

				if (!color) {
					f('Black');
					f('White');
				}
				else {
					f(color);
				}
			}



			function switchHighlight(element) {
				//remove currently highlighted allowed positions
				$$('.cell_allowed_move').forEach(function (elem) {
					elem.classList.remove('cell_allowed_move');
				});

				$$('.cell_allowed_capture').forEach(function (elem) {
					elem.classList.remove('cell_allowed_capture');
				});
				
				//remove currently selected element, if any
				var selected = $('.cell_selected');
				if (selected) {
					selected.classList.remove('cell_selected');
				}

				//without element we just un-highlight all and return
				if(!element)
					return false;

				//the second click on the highlighted cell. Un-highlight and return
				if (selected && selected.id == element.id)
					return false;

				//cell without a piece on it
				if (!$scope.board[element.id]) 
					return false;

				element.classList.add("cell_selected");
				return true;
			}

			ctrl.onCellClick = function ($event) {
				if ($event.currentTarget.classList.contains('cell_allowed_move') ||
					$event.currentTarget.classList.contains('cell_allowed_capture')) {
					var selected = $('.cell_selected');
					gamesService.makeMove(ctrl.currentGame.Id, selected.id, $event.currentTarget.id, ctrl.setGame);
					switchHighlight();
					return;
				}

				if (switchHighlight($event.currentTarget)) {
					gamesService.getMoves(ctrl.currentGame.Id, $event.currentTarget.id, function (moves) {
						moves.forEach(function (m) {
							$('#' + m.Destination).classList.add($scope.board[m.Destination] ?
								'cell_allowed_capture' : 'cell_allowed_move');
						});
					});
				}
			}

			ctrl.deletePiece = function ($event) {
				$scope.board[$event.currentTarget.id] = {};
			}

			ctrl.setGame = function (game) {
				$scope.board = {};
				ctrl.currentGame = game;
				ctrl.refresh();
			}

			ctrl.refresh = function () {
				foreachPiece(function (color, pieceChar, pos) {
					$scope.board[pos] = { color: color, pieceChar: pieceChar };
				});
			}

			ctrl.getPieceSrc = function (pos) {
				if(null == ctrl.currentGame)
					return '';

				var find = function (color) {
					var p = ctrl.currentGame.Board[color].PieceStrings.find(function (s) { return s.substr(1, 2) == pos; });
					return p ? 'img/' + color + '/' + p[0] + '.png' : null;
				}

				return find('Black') || find('White') || '';
			}

			gamesService.getGame($routeParams['id'], function (newGame) {
				ctrl.setGame(newGame);
			});


		}]);

	angular.module('app').directive("testDirective", function () {
		return {
			scope: {
				name: '=player',
			},
			template: '<h3 ng-click=\'name = "changed in directive"\'>This is a {{name}} turn</h3>',
		}
	})

})();