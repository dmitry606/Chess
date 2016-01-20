'use strict';

(function () {
	angular.module('app').directive('chessPiece', function () {
		var link = function (scope, element, attrs) {
			var targetProp = '' + attrs.ownerboard + '.' + attrs.id;
			scope.$watch(targetProp, function (value) {
				console.log("Watch fired");
				if (value) {
					var imgSrc = sprintf('img/%(color)s/%(pieceChar)s.png', value);
					if(element.children().length == 0) {
						element.append(sprintf('<img src="%1s"></img>', imgSrc));
					}
					else {
						var img = element.children()[0];
						if (img.getAttribute('src') != imgSrc)
							img.setAttribute('src', imgSrc);
					}
				}
				else {
					element.empty();
				}
			});

		};

		var controller = function() {
		}

		return {
			//restrict: 'E', 
			//scope: {
			//	position: '@',
			//	board:'=',
			//	_counter:'='
			//},
			//template: '<img src=""></img>',
			link: link
			//controller: controller,
			//controllerAs: 'ctrl',
			
		}
	})
})();