angular.module("authApp", ["ngRoute", "LocalStorageModule", "angular-loading-bar"])
.config(function ($routeProvider, $httpProvider) {
    // Регистрация пользователя
    $routeProvider.when("/signup/", {
        controller: "signupCtrl",
        templateUrl: "Content/Partial/auth/signup.html",
        resolve: {
            loginrequired: function (auth) {
                return auth.unauthorizedRequired();
            }
        }
    });

    // Аутентификация пользователя
    $routeProvider.when("/login/", {
        controller: "loginCtrl",
        templateUrl: "Content/Partial/auth/login.html",
        resolve: {
            loginrequired: function (auth) {
                return auth.unauthorizedRequired();
            }
        }
    });

    // Добавление нового пользователя
    $routeProvider.when("/account/", {
        controller: "addAccountCtrl",
        templateUrl: "Content/Partial/auth/addAccount.html",
        resolve: {
            loginrequired: function (auth) {
                return auth.adminLoginRequired();
            }
        }
    });

    $httpProvider.interceptors.push('authInterceptor');
})
.run(["auth", function (auth) {
    // Получаем информацию о пользователе при загрузке приложения
    auth.getAuthData();
}]);