$(document).ready(function () {

    $(".js_order-product-chkbx").on('ifToggled', function (event) {
        //alert(event.type + ' callback');
        var _self = $(this);

        try {
            var label = _self.parent().next("label");
            var action = "/Cart/Orders/OrderRemoveProduct";
            label.text("Заказать");

            if (_self.is(":checked")) {
                action = "/Cart/Orders/OrderAddProduct";
                label.text("В корзине");
            }
            

            var params = {
                productId: _self.data("productId")
            };

            $.ajax({
                url: action,
                method: "POST",
                async: true,
                cache: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(params)
            })
                .done(function (response) {

                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                    _self.attr("checked", false);
                })
                .always(function (response) {

                });
        }
        catch (ex) {
            console.log(ex);
        };

    });

    $(".js_cart-itemAmount-input").on('change', function () {
        var _self = $(this);

        var action = "/Cart/Orders/OrderUpdateProduct";

        try {
             var params = {
                 productId: _self.data("productId"),
                 amount: _self.val()
            };

            $.ajax({
                url: action,
                method: "POST",
                async: true,
                cache: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(params)
            })
                .done(function (response) {
                    var itemAmount = _self.val();
                    var itemPrice = _self.closest("tr").find(".js_cart-itemPrice-span").val();
                    _self.closest("tr").find(".js_cart-itemAmountSum-span").val(itemPrice * itemAmount);
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                    _self.attr("checked", false);
                })
                .always(function (response) {

                });
        }
        catch (ex) {
            console.log(ex);
        };

    });

    $(".js_cart-itemDelete-link").on('click', function () {
        var _self = $(this);

        var action = "/Cart/Orders/OrderRemoveProduct";

        try {
            var params = {
                productId: _self.data("productId")
            };

            $.ajax({
                url: action,
                method: "POST",
                async: true,
                cache: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(params)
            })
                .done(function (response) {
                    _self.closest("tr").remove();
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                    _self.attr("checked", false);
                })
                .always(function (response) {

                });
        }
        catch (ex) {
            console.log(ex);
        };

    });

});

