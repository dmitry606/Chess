'use strict';

(function () {
	var apiBase = 	'/api/games/';

	angular
		.module('app')
		.factory('GamesService', function ($http, $routeParams) {
			var _currentGame = null;

			function newGame(onSuccess) {
				$http.get(apiBase + 'new').success(function (newId)
				{
					getGame(newId, onSuccess);
				});
			}

			function getCurrent() {
				return angular.copy(_currentGame);
			}

			function getGame(id, onSuccess) {
				if (null == _currentGame || id !== _currentGame.Id) {
					$http.get(apiBase + id).success(function (game) {
						_currentGame = game;
						_currentGame.Id = id;
						onSuccess(getCurrent());
					});
					return;
				}
					
				onSuccess(getCurrent());
			}

			function getMoves(gameId, pos, onSuccess) {
				$http.get(apiBase + gameId + '/pos=' + pos).success(onSuccess);
			}

			function makeMove(gameId, fromPos, toPos, onSuccess) {
				$http.put(apiBase + gameId, { fromPos: fromPos, toPos: toPos }).success(function () {
					_currentGame = null;
					getGame(gameId, onSuccess);
				});
			}

			return {
				newGame: newGame,
				getCurrent: getCurrent,
				getGame: getGame,
				getMoves: getMoves,
				makeMove: makeMove
			}
		})
})();