'use strict';

function $(selector, container) {
	return (container || document).querySelector(selector);
}

function $$(selector, container) {
	var nodeList = (container || document).querySelectorAll(selector);
	return Array.prototype.slice.call(nodeList);
}


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