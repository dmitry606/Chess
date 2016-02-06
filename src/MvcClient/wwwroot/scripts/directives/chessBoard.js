'use strict';

(function () {
	angular.module('app').directive('chessBoard', function () {

		//var link = function ($scope, element, attrs) {
		//};

		var controller = function ($scope, BoardFactory, JoinFactory) {
			$scope.joinCtrl = JoinFactory.getForCurrentRoute();
			$scope.rows = [8, 7, 6, 5, 4, 3, 2, 1];
			$scope.columns = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];

			$scope.board = BoardFactory.getForCurrentRoute();
			$scope.board.update();

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
				var wasActive = $scope.active;

				resetSelections();

				if (allowedMove) {
					$scope.board.move(wasActive, pos);
					return;
				}

				if (sameCell || !$scope.canBeSelected(pos))
					return;

				$scope.active = pos;
				$scope.board.getMoves(pos, ctrl.setMoves);
			}

			$scope.canBeSelected = function (pos) {
				return $scope.board[pos] &&
					$scope.board[pos].color == $scope.board.getCurrentColor() &&
					$scope.joinCtrl.isJoined($scope.board[pos].color);
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
			},

			templateUrl: 'partials/chess-board.html',
			controller: controller,
			controllerAs: 'ctrl',
		}
	})
})();