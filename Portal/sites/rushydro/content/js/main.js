$(document).ready(function () {



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