angular.module("orderApp")
.factory('order', function($http) {
    var orderFactory = {};

    var serviceBase = 'http://localhost:63482/';

    // Получить элементы корзины
    var _getOrderDetails = function() {
        return $http.get(serviceBase + "api/OrderDetails");
    }
    // Добавить книгу в корзину
    var _addOrderDetails = function (addOrderDetailsModel) {
        return $http.post(serviceBase + "api/OrderDetails", addOrderDetailsModel);
    };
    // Удалить книгу из корзины
    var _deleteOrderDetails = function (orderDetailsId) {
        return $http.delete(serviceBase + "api/OrderDetails/" + orderDetailsId);
    }
    // Сформировать заказ
    var _addOrder = function () {
        return $http.post(serviceBase + "api/Order");
    }
    // Получить все заказы пользователя
    var _getUserOrders = function () {
        return $http.get(serviceBase + "api/Order");
    }
    // Получить все неподтвержденные заказы пользователей
    var _getUncorfirmedOrders = function () {
        return $http.get(serviceBase + "api/Order/Uncorfirmed");
    }
    // Подтвердить\отклонить заказ
    var _updateOrder = function (orderId, updateOrderModel) {
        return $http.put(serviceBase + "api/Order/" + orderId, updateOrderModel);
    }

    orderFactory.getOrderDetails = _getOrderDetails;
    orderFactory.addOrderDetails = _addOrderDetails;
    orderFactory.deleteOrderDetails = _deleteOrderDetails;
    orderFactory.addOrder = _addOrder;
    orderFactory.getUserOrders = _getUserOrders;
    orderFactory.getUncorfirmedOrders = _getUncorfirmedOrders;
    orderFactory.updateOrder = _updateOrder;

    return orderFactory;
});