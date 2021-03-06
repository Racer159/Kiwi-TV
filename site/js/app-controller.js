var app = angular.module('main', ['ui.router', 'ngAnimate', 'ui.bootstrap']);

app.controller('AppController', function($scope, $log, $location, $window) {
	$scope.home = 'active';
	$scope.isCollapsed = true;
	
	$scope.$on('$viewContentLoaded', function(event) {
		$window.ga('send', 'pageview', { page: $location.path() });
	});
	
	$scope.activateNet = function(e) {
		if (!e) e = window.event;
		if (e.shiftKey && e.ctrlKey) {
			window.location = 'http://racer159.github.io/partials/net/code1.html';
		}
	}
	
	$scope.$on('$locationChangeSuccess', function(event){
		$scope.isCollapsed = true;
    	$scope.home = '';
		$scope.faq = '';
		$log.info($location.path());
		if ($location.path() == '/home') {
			$scope.home = 'active';
		} else if ($location.path() == '/faq') {
			$scope.faq = 'active';
		}
    });
});