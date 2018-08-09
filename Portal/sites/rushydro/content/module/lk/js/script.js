﻿$(document).ready(function () {

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

    $("#js_filterDocType-select").on("change", function () {
        $("#filter-form").submit();
    });

    $("#js_filterPayed-select").on("change", function () {
        $("#filter-form").submit();
    });

    //НЕ правильно работает
    //$('#js_filterDateRange-input').on("change", function () {
    //    var params = {
    //        datestart: $('#js_filterBeginDate-input').val(),
    //        dateend: $('#js_filterEndDate-input').val()
    //    };
    //    var str = jQuery.param(params);
    //    location.href = UrlPage() + "?" + str;
    //});

});