angular.module("bookApp")
.factory('book', function ($http) {
    var bookFactory = {};

    var serviceBase = 'http://localhost:63482/';
    // Получить каталог книг
    var _getListBook = function (offset, limit, search) {
        var serviceUrl = serviceBase + "api/book/" + "?offset=" + offset + "&limit=" + limit;
        if (search.name != "") { serviceUrl += "&name=" + search.name };
        if (search.category != "") { serviceUrl += "&category=" + search.category.Name };
        if (search.author != "") { serviceUrl += "&author=" + search.author.Name };
        if (search.publisher != "") { serviceUrl += "&publisher=" + search.publisher.Name };
        if (search.sort != "") { serviceUrl += "&sort=" + search.sort };
        return $http.get(serviceUrl);
    };
    // Получить информацию о книге
    var _getBook = function (bookId) {
        return $http.get(serviceBase + "api/book/" + bookId);
    }
    // Получить перечень категорий
    var _getCategories = function () {
        return $http.get(serviceBase + "api/category");
    }
    // получить перечень авторов
    var _getAuthors = function () {
        return $http.get(serviceBase + "api/author");
    }
    // Получить перечень издательств
    var _getPublishers = function () {
        return $http.get(serviceBase + "api/publisher");
    }
    // Добавить новую книгу
    var _addBook = function (book) {
        return $http.post(serviceBase + "api/book/", book);
    }
    // Удалить книгу
    var _deleteBook = function (bookId) {
        return $http.delete(serviceBase + "api/book/" + bookId);
    }
    // Обновить книгу
    var _updateBook = function (bookId, updateBookModel) {
        return $http.put(serviceBase + "api/book/" + bookId, updateBookModel);
    }
    // Добавить категорию
    var _addCategory = function (category) {
        return $http.post(serviceBase + "api/category", category);
    }
    // Добавить автора
    var _addAuthor = function (author) {
        return $http.post(serviceBase + "api/author", author);
    }
    // Добавить издателя
    var _addPublisher = function (publisher) {
        return $http.post(serviceBase + "api/publisher", publisher);
    }

    bookFactory.getListBook = _getListBook;
    bookFactory.getBook = _getBook;
    bookFactory.getCategories = _getCategories;
    bookFactory.getAuthors = _getAuthors;
    bookFactory.getPublishers = _getPublishers;
    bookFactory.addBook = _addBook;
    bookFactory.deleteBook = _deleteBook;
    bookFactory.updateBook = _updateBook;
    bookFactory.addCategory = _addCategory;
    bookFactory.addAuthor = _addAuthor;
    bookFactory.addPublisher = _addPublisher;

    return bookFactory;
})