angular.module("authApp")
.factory('auth', ['$http', '$q', '$location', 'localStorageService', function ($http, $q, $location, localStorageService) {
    var serviceBase = 'http://localhost:63482/';
    var authFactory = {};

    // Текущие данные аутентификации
    var _authentication = {
        isAuth: false,
        username: "none",
        isUser: false,
        isManager: false,
        isAdmin: false
    };

    // Регистрация пользователя
    var _registration = function (registration) {

        _logOut();

        return $http.post(serviceBase + 'api/account/register', registration).then(function (response) {
            return response;
        });
    };

    // Вход пользователя в систему
    var _login = function (loginData) {

        var data = "grant_type=password&username=" + loginData.username + "&password=" + loginData.password;

        var deferred = $q.defer();

        $http.post(serviceBase + 'Token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {
            localStorageService.set('authorizationData', { token: response.access_token, username: loginData.username });
            _authentication.isAuth = true;
            _authentication.username = loginData.username;
            var roles = JSON.parse(response.roles);
            for (var i = 0; i < roles.length; i++) {
                switch (roles[i]) {
                    case "user":
                        _authentication.isUser = true;
                        break;
                    case "manager":
                        _authentication.isManager = true;
                        break;
                    case "admin":
                        _authentication.isAdmin = true;
                        break;
                }
            }
            localStorageService.set('authorizationData', {
                token: response.access_token, username: loginData.username,
                isUser: _authentication.isUser, isManager: _authentication.isManager, isAdmin: _authentication.isAdmin
            });
            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    // Выход пользователя
    var _logOut = function () {

        localStorageService.remove('authorizationData');

        _authentication.isAuth = false;
        _authentication.username = "";
        _authentication.isAdmin = false;
        _authentication.isManager = false;
        _authentication.isUser = false;

    };

    // Получить текущие данные пользователя из localStorage
    var _getAuthData = function () {

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            _authentication.isAuth = true;
            _authentication.username = authData.username;
            _authentication.isUser = authData.isUser;
            _authentication.isManager = authData.isManager;
            _authentication.isAdmin = authData.isAdmin;
        }
    }

    // Проверка пользователя на роль user
    var _userLoginRequired = function () {
        var deferred = $q.defer();
        if (!_authentication.isUser) {
            deferred.reject()
            $location.path('/login');
        } else {
            deferred.resolve()
        }
        return deferred.promise;
    }

    // Проверка пользователя на роль manager
    var _managerLoginRequired = function () {
        var deferred = $q.defer();
        if (!(_authentication.isAuth && _authentication.isManager)) {
            deferred.reject()
            $location.path('/login');
        } else {
            deferred.resolve();
        }
        return deferred.promise;
    }

    // Проверка пользователя на роль admin
    var _adminLoginRequired = function () {
        var deferred = $q.defer();
        if (!(_authentication.isAuth && _authentication.isAdmin)) {
            deferred.reject()
            $location.path('/login');
        } else {
            deferred.resolve();
        }
        return deferred.promise;
    }

    // Проверка пользователя на анонимного посетителя
    var _unauthorizedRequired = function () {
        var deferred = $q.defer();
        if (_authentication.isAuth) {
            deferred.reject();
            $location.path('/books/');
        } else {
            deferred.resolve();
        }
        return deferred.promise;
    }

    // Добавление пользователя администратором
    var _addUser = function (user) {
        return $http.post(serviceBase + 'api/account/', user);
    }

    authFactory.registration = _registration;
    authFactory.login = _login;
    authFactory.authentication = _authentication;
    authFactory.logOut = _logOut;
    authFactory.getAuthData = _getAuthData;
    authFactory.userLoginRequired = _userLoginRequired;
    authFactory.managerLoginRequired = _managerLoginRequired;
    authFactory.adminLoginRequired = _adminLoginRequired;
    authFactory.unauthorizedRequired = _unauthorizedRequired;
    authFactory.addUser = _addUser;

    return authFactory;
}])
.factory('authInterceptor', ['$q', '$location', 'localStorageService', function ($q, $location, localStorageService) {

    var authInterceptorServiceFactory = {};

    var _request = function (config) {

        config.headers = config.headers || {};

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            config.headers.Authorization = 'Bearer ' + authData.token;
        }

        return config;
    }

    var _responseError = function (rejection) {
        if (rejection.status === 401) {
            auth.logOut();
            $location.path('/login');
        }
        return $q.reject(rejection);
    }

    authInterceptorServiceFactory.request = _request;
    authInterceptorServiceFactory.responseError = _responseError;

    return authInterceptorServiceFactory;
}]);

