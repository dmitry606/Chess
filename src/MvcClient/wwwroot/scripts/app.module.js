'use strict';

(function () {
	var app = angular.module('app', ['ngRoute']);

	app.config(['$routeProvider', '$locationProvider',
		function ($routeProvider, $locationProvider) {
			$routeProvider
				.when('/', {
					templateUrl: 'partials/start.html',
					controller: 'StartController',
					controllerAs: 'ctrl'
				})
				.when('/game/:id', {
					templateUrl: 'partials/game.html',
					controller: 'GameController',
					controllerAs: 'ctrl'
				}).
				otherwise({
					redirectTo: '/'
				});

//			$locationProvider.html5Mode(true);
		}]);

})();