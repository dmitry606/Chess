(function () {
	var apiBase = '/api/games/';

	angular
		.module('app')
		.factory('BoardFactory', function ($routeParams, GamesService) {
			
			return {
				getForCurrentRoute: function() {
					return new Board($routeParams['id'], GamesService);
				}
			}
		});
})();