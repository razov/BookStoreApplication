angular.module("app", ["ngRoute", "authApp", "bookApp", "orderApp"])
.config(['$locationProvider', '$routeProvider', function($locationProvider, $routeProvider) {
    $routeProvider.otherwise({ redirectTo: '/books/' });
    // Редирект
}]);