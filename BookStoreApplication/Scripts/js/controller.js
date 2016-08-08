angular.module("app")
// Контроллер для шапки сайта
.controller('headerCtrl', ['$scope', 'auth', 'order','$location', function($scope, auth, order, $location) {
    $scope.authentication = auth.authentication;

    $scope.logOut = function () {
        auth.logOut();
        $location.path('/books/');
    }
}])
// Контроллер для sidebar
.controller('pageCtrl', ['$scope', 'auth', function($scope, auth) {
    $scope.authentication = auth.authentication;
}])