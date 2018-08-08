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

    $('#js_filterDateRange-input').daterangepicker({
        "locale": {
            "format": "DD.MM.YYYY",
            "separator": " - ",
            "applyLabel": "Применить",
            "cancelLabel": "Отмена",
            "fromLabel": "С",
            "toLabel": "По",
            "customRangeLabel": "Custom",
            "weekLabel": "Нед",
            "daysOfWeek": [
                "Вс",
                "ПН",
                "Вт",
                "Ср",
                "Чт",
                "Пн",
                "Сб"
            ],
            "monthNames": [
                "Январь",
                "Февраль",
                "Март",
                "Апрель",
                "Май",
                "Июнь",
                "Июль",
                "Август",
                "Сентябрь",
                "Октябрь",
                "Ноябрь",
                "Декабрь"
            ],
            "firstDay": 1
        },
        "opens": "right"
    }, function (start, end, label) {
        $("#js_filterBeginDate-input").val(start.format('DD.MM.YYYY'))
        $("#js_filterEndDate-input").val(end.format('DD.MM.YYYY'))
        console.log('range: ' + label);
    });

    $('#js_filterDateRange-input').on('apply.daterangepicker', function (ev, picker) {
        $("#js_filterBeginDate-input").val(picker.startDate.format('DD.MM.YYYY'));
        $("#js_filterEndDate-input").val(picker.endDate.format('DD.MM.YYYY'));

        $("#filter-form").submit();
    });

});

