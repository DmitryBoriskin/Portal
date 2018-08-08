﻿$('#filterDateRange-input').daterangepicker({
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
    "opens": "left"
}, function (start, end, label) {
    $("#filterBeginDate-input").val(start.format('DD.MM.YYYY'))
    $("#filterEndDate-input").val(end.format('DD.MM.YYYY'))
    console.log('range: ' + label);
});

$('#filterDateRange-input').on('apply.daterangepicker', function (ev, picker) {
    $("#filterBeginDate-input").val(start.format('DD.MM.YYYY'))
    $("#filterEndDate-input").val(end.format('DD.MM.YYYY'))
});