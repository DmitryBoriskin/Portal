﻿$(document).ready(function () {


    if ($('.treeview-menu li.active').length > 0) {
        var parenttree = $('.treeview-menu li.active').closest('.treeview-menu');
        parenttree.addClass('menu-open');
        parenttree.closest('.treeview').addClass('active');
    }

    //вид меню (схлопнутый/раскрытый)
    $('.sidebar-toggle').click(function () {
        if ($('body').hasClass('sidebar-collapse')) {
            $.cookie('statemenu', 'mini');
        }
        else {
            $.cookie('statemenu', 'big');
        }
    });
    var statemenu = $.cookie('statemenu');    
    if (!(statemenu == undefined || statemenu == '')) {
        if (statemenu == 'mini') {
            $('body').addClass('sidebar-collapse')
        }
    }

    if ($('.gallery').length > 0) {

        $('.gallery').each(function () {
            var $ParentBlock = $(this);
            var data_id = $ParentBlock.attr('data-id');
            var url = '/core/GetPhotoGallery/' + data_id;            
            $.getJSON(url, function (data) {
                for (var i in data) {
                    var $skipwr = $("<div/>", { "class": "swip_wr" });                    
                    var $a = $("<a/>",
                        { "class": "swipebox" ,
                            "data-original": data[i].c_url,
                            "rel": 'r' + data_id,
                          "title": data[i].c_title ,
                          "style": "background-image:url(" + data[i].c_preview+');' ,
                          "href": data[i].c_url }
                    );    
                    
                    $skipwr.append($a);
                    $ParentBlock.append($skipwr);                    
                }              
            });            
        });

        $('body').find('.swipebox').swipebox();
    }


    if ($('.iCheck').length > 0) {
        $('.iCheck').iCheck({
            checkboxClass: 'icheckbox_square-orange',
            radioClass: 'iradio_square-orange',
            increaseArea: '20%' // optional
        });
    }


    //выбор лс
    $('.select_ls').click(function () {
        var idsubscr = $(this).attr('data-id');

        $.ajax({
            url: "Widget/SelectSubscr",
            type: 'POST',
            data: { SubscrId: idsubscr },
            //success: function (data) {
            //    alert('1_' + data);
            //},
            //error: function (data) {
            //    alert('2_' + data);
            //},
        });
        location.reload();
    });

 


    //Маска мобильного телефона
    $(".masked-mobile-phone").mask("+7 (999) 999-99-99");
});


