angular.module("orderApp")
.controller('handlerElementsBasket', function ($scope, $rootScope, $location, order) {
    $scope.orderDetailsCount = null;
    // Получить элементы корзины
    $scope.getOrderDetails = function () {
        order.getOrderDetails().then(function (response) {
            $scope.orderDetailsCount = response.data.length;
        })
    };
    // Увеличить количество книг в корзине
    $scope.$on("addToBasket", function (event, args) {
        order.addOrderDetails({ idBook: args.book.Id }).then(function (response) {
        })
        $scope.orderDetailsCount += 1;
    });

    // Уменьшить количество книг в корзине
    $scope.$on("deleteFromBasket", function () {
        $scope.orderDetailsCount -= 1;
    });

    // Обновить количество книг в корзине
    $scope.$on("deleteBook", function () {
        $scope.getOrderDetails();
    });

    // Обнулить количество книг в корзине при сформированном заказе
    $scope.$on("addOrder", function () {
        $scope.orderDetailsCount = 0;
    });

    $scope.getOrderDetails();
})
.controller('orderDetailsCtrl', function ($scope, $rootScope, $location, order) {
    $scope.orderDetails = null;

    // Получить элементы корзины
    $scope.getOrderDetails = function () {
        order.getOrderDetails().then(function (response) {
            $scope.orderDetails = response.data;
        })
    }

    // Удалить книгу из корзины
    $scope.deleteOrderDetails = function (orderDetail) {
        order.deleteOrderDetails(orderDetail.Id).then(function () {
            $rootScope.$broadcast("deleteFromBasket");
            for (var i = 0; i < $scope.orderDetails.length; i++) {
                if ($scope.orderDetails[i].Id == orderDetail.Id) {
                    $scope.orderDetails.splice(i, 1);
                }
            }
        })
    }

    // Посчитать общую стоимость
    $scope.getSumPrice = function () {
        if ($scope.orderDetails == null) {
            return 0;
        }
        else {
            var sum = 0;
            for (var i = 0; i < $scope.orderDetails.length; i++)
                sum += $scope.orderDetails[i].Book.Price;
            return sum;
        }
    }

    // Сформировать заказ
    $scope.addOrder = function () {
        order.addOrder().then(function () {
            $location.path("/orders/");
            $rootScope.$broadcast("addOrder");
        });
    }

    $scope.getOrderDetails();
})
.controller('orderCtrl', function ($scope, $rootScope, $location, order) {
    $scope.orders = null;

    // Получить множество заказов пользователя
    var getOrders = function () {
        order.getUserOrders().then(function (response) {
            $scope.orders = response.data;
        })
    }

    // Посчитать сумму заказа
    $scope.getSumOrder = function (order) {
        if (order == undefined) {
            return 0;
        }
        sum = 0;
        for (var j = 0; j < order.OrderDetails.length; j++) {
            sum += order.OrderDetails[j].Book.Price;
        }
        return sum;
    }

    // Посчитать стоимость всех заказов
    $scope.getSumPrice = function () {
        if ($scope.orders == null) {
            return 0;
        }
        else {
            var sum = 0;
            for (var i = 0; i < $scope.orders.length; i++) {
                for (var j = 0; j < $scope.orders[i].OrderDetails.length; j++) {
                    sum += $scope.orders[i].OrderDetails[j].Book.Price;
                }
            }
            return sum;
        }
    }
    
    getOrders();
})
.controller('monitoringOrdersCtrl', function ($scope, $rootScope, $location, order) {
    $scope.orders = null;

    // Загрузить все неподтвержденные заказы
    var getOrders = function () {
        order.getUncorfirmedOrders().then(function (response) {
            $scope.orders = response.data;
        })
    }

    // Посчитать стоимость заказа
    $scope.getSumOrder = function (order) {
        if (order == undefined) {
            return 0;
        }
        sum = 0;
        for (var j = 0; j < order.OrderDetails.length; j++) {
            sum += order.OrderDetails[j].Book.Price;
        }
        return sum;
    }

    // Подтвердить\отклонить заказ
    $scope.updateOrder = function (orderId, flag) {

        order.updateOrder(orderId, { Confirmed: flag }).then(function () {
            for (var i = 0; i < $scope.orders.length; i++) {
                if ($scope.orders[i].Id == orderId) {
                    $scope.orders.splice(i, 1);
                }
            }
        })
    }

    getOrders();
})