$(document).ready(function () {

    if ($('.invoice_active').length > 0) {
        $('.invoice_active').focus();        
    }

    $('.collapse_btn').click(function () {
        var el = $(this).closest('.factura');
        if (el.hasClass("factura_open")) {
            el.removeClass('factura_open');
        }
        else {
            el.addClass('factura_open');
        }
    });


    window.onscroll = function () {
        var id = 'index_right';
        if (getBodyScrollTop() >= 75) {
            $('#' + id).addClass('index_right__fix');
        }
        else {            
            $('#' + id).removeClass('index_right__fix');
        }
    }

});


function getBodyScrollTop() {
    return self.pageYOffset || (document.documentElement && document.documentElement.scrollTop) || (document.body && document.body.scrollTop);
}
//определяет ссылку страницы
function UrlPage() {
    var url = location.href;
    var site = url.split('?');
    return site[0];
}