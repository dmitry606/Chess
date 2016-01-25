'use strict';

(function () {
	var apiBase = 	'/api/games/';

	angular
		.module('app')
		.factory('gamesService', ['$http', function ($http) {
			//var _initialBoard = '{ "Id": "567bbe5057403f1e80d497a1", "Caption": "New game", "CreatedAt": "2015-12-24T09:43:44.7842838Z", "LastModifiedAt": "2015-12-24T09:43:44.7842838Z", "WhiteName": "White player", "BlackName": "Black player", "Board": { "History": [], "White": { "PieceStrings": ["pa2", "pb2", "pc2", "pd2", "pe2", "pf2", "pg2", "ph2", "Ra1", "Nb1", "Bc1", "Qd1", "Ke1", "Bf1", "Ng1", "Rh1"] }, "Black": { "PieceStrings": ["pa7", "pb7", "pc7", "pd7", "pe7", "pf7", "pg7", "ph7", "Ra8", "Nb8", "Bc8", "Qd8", "Ke8", "Bf8", "Ng8", "Rh8"] } } }';
			//var srv = this;
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
		}])
})();