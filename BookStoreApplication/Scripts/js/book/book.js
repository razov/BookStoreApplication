angular.module("bookApp",["ngRoute","angularFileUpload"])
.config(['$routeProvider', function ($routeProvider) {
    // Каталог книг
	$routeProvider.when('/books/', {
		templateUrl: 'Content/Partial/book/listBook.html',
		controller: 'getListBookCtrl'
	});
    // Добавить книгу
	$routeProvider.when('/books/add/', {
	    templateUrl: 'Content/Partial/book/addBook.html',
	    controller: 'addBookCtrl',
	    resolve: {
	        loginrequired: function (auth) {
	            return auth.managerLoginRequired();
	        }
	    }
	});
    // Обновить данные книги с выбранным bookId (прим. стоимость)
	$routeProvider.when('/books/update/:bookId', {
	    templateUrl: 'Content/Partial/book/updateBook.html',
	    controller: 'updateBookCtrl',
	    resolve: {
	        loginrequired: function (auth) {
	            return auth.managerLoginRequired();
	        }
	    }
	});
    // Показать книгу с выбранным bookId
	$routeProvider.when('/books/:bookId', {
		templateUrl: 'Content/Partial/book/itemBook.html',
		controller: 'getBookCtrl'
	});
    // Добавить новую категорию
	$routeProvider.when('/category/add/', {
	    templateUrl: 'Content/Partial/book/addCategory.html',
	    controller: 'addCategoryCtrl',
	    resolve: {
	        loginrequired: function (auth) {
	            return auth.managerLoginRequired();
	        }
	    }
	});
    // Добавить нового автора
	$routeProvider.when('/author/add/', {
	    templateUrl: 'Content/Partial/book/addAuthor.html',
	    controller: 'addAuthorCtrl',
	    resolve: {
	        loginrequired: function (auth) {
	            return auth.managerLoginRequired();
	        }
	    }
	});
    // Добавить новое издательство
	$routeProvider.when('/publisher/add/', {
	    templateUrl: 'Content/Partial/book/addPublisher.html',
	    controller: 'addPublisherCtrl',
	    resolve: {
	        loginrequired: function (auth) {
	            return auth.managerLoginRequired();
	        }
	    }
	});
}])