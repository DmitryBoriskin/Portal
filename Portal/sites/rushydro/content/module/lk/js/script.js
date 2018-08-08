$(document).ready(function () {

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
        $("#js_filterBeginDate-input").val(start.format('DD.MM.YYYY'))
        $("#js_filterEndDate-input").val(end.format('DD.MM.YYYY'))
    });

});