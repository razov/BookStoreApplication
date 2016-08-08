angular.module("bookApp")
.controller('getListBookCtrl', function ($scope, $location, $routeParams, $rootScope, book, auth) {
    $scope.categories = null;
    $scope.publishers = null;
    $scope.authors = null;

    // Модель поиска подтвержденная
    var searchCurrentModel = {
        name : "",
        category : "",
        publisher : "",
        author : "",
        sort : "name"
    }
    // Модель поиска
    $scope.searchModel = {
        name: "",
        category: "",
        publisher: "",
        author: "",
        sort: "name"
    }

    $scope.authentication = auth.authentication;

    var searchModelReset = function () {
        $scope.searchModel.name = searchCurrentModel.name;
        $scope.searchModel.category = searchCurrentModel.category;
        $scope.searchModel.publisher = searchCurrentModel.publisher;
        $scope.searchModel.author = searchCurrentModel.author;
        $scope.searchModel.sort = searchCurrentModel.sort;
    }

    $scope.search = function () {
        $scope.isBack = false;
        $scope.isNext = true;
        $scope.limit = 8;
        $scope.offsetNext = 0;
        $scope.offset = 0;
        $scope.offsetBack = -$scope.limit;

        searchCurrentModel.name = $scope.searchModel.name;
        searchCurrentModel.category = $scope.searchModel.category;
        searchCurrentModel.publisher = $scope.searchModel.publisher;
        searchCurrentModel.author = $scope.searchModel.author;
        searchCurrentModel.sort = $scope.searchModel.sort;
        
        $scope.moveNext();
    }

    $scope.isBack = false;
    $scope.isNext = true;

    $scope.limit = 8;

    $scope.offsetNext = 0;
    $scope.offset = 0;
    $scope.offsetBack = -$scope.limit;

    // Загрузить метаданные (категории, авторы, издатели)
    $scope.loadMeta = function () {
        book.getCategories().then(function (response) {
            $scope.categories = response.data;
        });

        book.getAuthors().then(function (response) {
            $scope.authors = response.data;
        });

        book.getPublishers().then(function (response) {
            $scope.publishers = response.data;
        });
    }

    // Перейти далее
    $scope.moveNext = function () {
        searchModelReset();
        if ($scope.isNext) {
            $scope.offset = $scope.offsetNext;
            $scope.viewBooks($scope.offset, $scope.limit, searchCurrentModel);
        }
    }

    // Перейти назад
    $scope.moveBack = function () {
        searchModelReset();
        if ($scope.isBack) {
            $scope.offset = $scope.offsetBack;
            $scope.viewBooks($scope.offset, $scope.limit, searchCurrentModel);
        }
    }

    // Загрузить книги
    $scope.viewBooks = function (offset, limit, search) {
        book.getListBook(offset, limit, search).then(function (response) {
            $scope.books = response.data.data;
            $scope.isBack = response.data.paging.isBack;
            $scope.isNext = response.data.paging.isNext;
            
            $scope.offsetNext = $scope.offset + $scope.limit;
            $scope.offsetBack = $scope.offset - $scope.limit;
        }, function (error) {

        });
    }

    $scope.viewBook = function (book) {
        $location.path("/books/" + book.Id);
    }
    $scope.addToBasket = function (book) {
        $rootScope.$broadcast("addToBasket", {
            book: book
        })
    }

    $scope.deleteBook = function (bookDelete) {
        book.deleteBook(bookDelete.Id).then(function () {
            $location.path("/#/")
            $rootScope.$broadcast("deleteBook",bookDelete);
        })
    }

    $scope.updateBook = function (bookUpdate) {
        $location.path("/books/update/" + bookUpdate.Id)
    }

    $scope.loadMeta();
    $scope.moveNext();
})
.controller('getBookCtrl', function ($scope, $routeParams, $rootScope, book, auth) {
    $scope.authentication = auth.authentication;

    $scope.viewBook = function () {
        book.getBook($routeParams.bookId).then(function (response) {
            $scope.book = response.data;
        });
    }
    // Оповестить контроллер  handlerElementsBasket о добавлении новой книги в корзину
    $scope.addToBasket = function (book) {
        $rootScope.$broadcast("addToBasket", {
            book: book
        })
    }

    $scope.viewBook();
})
.controller('addBookCtrl', function ($scope, $routeParams, $rootScope, $location, book, FileUploader) {
    $scope.year = new Date().getFullYear();

    // Флаг завершения добавления книги
    $scope.savedSuccessfully = false;

    // Добавление обложки книги
    $scope.uploader = new FileUploader({
        url: 'http://localhost:63482/api/Book/CoverUpload/',
        queueLimit: 1
    });

    $scope.uploader.filters.push({
        name: 'imageFilter',
        fn: function (item, options) {
            var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
            return '|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1;
        }
    });

    $scope.uploader.onCompleteAll = function () {
        $location.path("/books/" + $scope.bookId);
    };

    // Массив ошибок
    $scope.messages = [];

    // Модель книги
    $scope.book = {
        Name: "",
        CategoryId: "",
        AuthorId: "",
        PublisherId: "",
        Count: 0,
        Year: 0,
        Page: 0,
        Isbn: "",
        Price: 0
    }

    $scope.bookId = null;

    // Загрузить метаданные (категории, издатели, авторы)
    $scope.loadMeta = function () {
        book.getCategories().then(function (response) {
            $scope.categories = response.data;
        });

        book.getAuthors().then(function (response) {
            $scope.authors = response.data;
        });

        book.getPublishers().then(function (response) {
            $scope.publishers = response.data;
        });
    }

    // Добавить книгу
    $scope.addBook = function () {
        book.addBook($scope.book).then(function (response) {
            $scope.savedSuccessfully = true;
            $scope.messages = [];
            $scope.bookId = response.data.Id;
            $scope.uploader.url += response.data.Id;
        }, function (response) {
            $scope.savedSuccessfully = false;
            $scope.messages = [];
            for (var key in response.data.ModelState) {
                for (var i = 0; i < response.data.ModelState[key].length; i++) {
                    $scope.messages.push(response.data.ModelState[key][i]);
                }
            }
        });
    }

    $scope.loadMeta();
})
.controller('updateBookCtrl', function ($scope, $routeParams, $rootScope, $location, book) {
    $scope.book = null;

    // Получить информацию о выбранной книге
    var getBook = function () {
        book.getBook($routeParams.bookId).then(function (response) {
            $scope.book = response.data;
        })
    }

    // Сохранить изменения
    $scope.save = function () {
        book.updateBook($routeParams.bookId, { Count: $scope.book.Count }).then(function () {
            $location.path("/books/")
        })
    }

    getBook();
})
.controller('addCategoryCtrl', function ($scope, $timeout, $routeParams, $rootScope, $location, book) {
    $scope.category = {
        name: ""
    }
    // Массив ошибок
    $messages = [];
    // Флаг сохранения категории
    $scope.savedSuccessfully = false;
    // Добавить категорию
    $scope.addCategory = function () {
        book.addCategory($scope.category).then(function () {
            $scope.savedSuccessfully = true;
            startTimer();
        }, function (response) {
            $scope.savedSuccessfully = false;
            $scope.messages = [];
            for (var key in response.data.ModelState) {
                for (var i = 0; i < response.data.ModelState[key].length; i++) {
                    $scope.messages.push(response.data.ModelState[key][i]);
                }
            }
        })
    }

    var startTimer = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $location.path('/books/');
        }, 2000);
    }
})
.controller('addAuthorCtrl', function ($scope, $timeout, $routeParams, $rootScope, $location, book) {
    $scope.author = {
        name: ""
    }
    // Массив ошибок
    $messages = [];
    // Флаг сохранения автора
    $scope.savedSuccessfully = false;
    // Добавить автора
    $scope.addAuthor = function () {
        book.addAuthor($scope.author).then(function () {
            $scope.savedSuccessfully = true;
            startTimer();
        }, function (response) {
            $scope.savedSuccessfully = false;
            $scope.messages = [];
            for (var key in response.data.ModelState) {
                for (var i = 0; i < response.data.ModelState[key].length; i++) {
                    $scope.messages.push(response.data.ModelState[key][i]);
                }
            }
        })
    }

    var startTimer = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $location.path('/books/');
        }, 2000);
    }
})
.controller('addPublisherCtrl', function ($scope, $timeout, $routeParams, $rootScope, $location, book) {
    $scope.publisher = {
        name: ""
    }
    // Массив ошибок
    $messages = [];
    // Флаг сохранения издателя
    $scope.savedSuccessfully = false;
    // Добавить издателя
    $scope.addPublisher = function () {
        book.addPublisher($scope.publisher).then(function () {
            $scope.savedSuccessfully = true;
            startTimer();
        }, function (response) {
            $scope.savedSuccessfully = false;
            $scope.messages = [];
            for (var key in response.data.ModelState) {
                for (var i = 0; i < response.data.ModelState[key].length; i++) {
                    $scope.messages.push(response.data.ModelState[key][i]);
                }
            }
        })
    }

    var startTimer = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $location.path('/books/');
        }, 2000);
    }
})