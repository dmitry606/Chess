'use strict';

(function () {
	angular.module('app').controller('GameController',
		function ($scope, JoinFactory) {
			$scope.joinCtrl = JoinFactory.getForCurrentRoute();
			$scope.joinCtrl.start();
		});
})();