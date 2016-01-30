'use strict';

(function () {
	angular.module('app').directive('chessBoard', function () {

		var link = function (scope, element, attrs) {
			scope.rows = [8, 7, 6, 5, 4, 3, 2, 1];
			scope.columns = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];
		};

		var controller = function ($scope) {
			var ctrl = this;
			resetSelections();

			function resetSelections() {
				$scope.allowedMoves = [];
				$scope.allowedCaptures = [];
				$scope.active = null;
			}

			ctrl.setMoves = function(moves) {
				moves.forEach(function (move) {
					if ($scope.board[move.Destination])
						$scope.allowedCaptures.push(move.Destination)
					else
						$scope.allowedMoves.push(move.Destination)
				});
			}

			$scope.onCellClick = function ($event) {
				var pos = $event.currentTarget.id;
				var sameCell = $scope.isActive(pos);
				var allowedMove = $scope.isAllowedMove(pos) || $scope.isAllowedCapture(pos);
				var wasActive = $scope.active

				resetSelections();

				if (allowedMove) {
					$scope.onMoved({ from: wasActive, to: pos });
					return;
				}

				if (sameCell || !$scope.board[pos])
					return;

				$scope.active = pos;
				$scope.movesFactory.get(pos, ctrl.setMoves);
			}

			$scope.isActive = function (pos) {
				return pos == $scope.active;
			}

			$scope.isAllowedMove = function (pos) {
				return $scope.allowedMoves.indexOf(pos) >= 0;
			}

			$scope.isAllowedCapture = function (pos) {
				return $scope.allowedCaptures.indexOf(pos) >= 0;
			}
		}

		return {
			replace: true,
			scope: {
				board: '=ownerboard',
				movesFactory: '=moves',
				onMoved: '&onPieceMoved'
			},

			templateUrl: 'partials/chess-board.html',
			link: link,
			controller: controller,
			controllerAs: 'ctrl',
		}
	})
})();