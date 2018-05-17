$(document).ready(function () {

    if ($('.gallery').length > 0) {
        $('.gallery').each(function () {
            var $ParentBlock = $(this);
            var url = '/core/GetPhotoGallery/' + $ParentBlock.attr('data-id');            
            $.getJSON(url, function (data) {
                for (var i in data) {
                    var $skipwr = $("<div/>", { "class": "swip_wr" });
                    var $skipwr1 = $("<div/>", { "class": "swip_wr1" });
                    var $a = $("<a/>",
                        { "class": "swipebox" ,
                         "data-original": data[i].c_url ,
                         "title": data[i].c_title ,
                         "style": "background-image:url(" + data[i].c_preview+');' ,
                         "href": "data[i].c_url" }
                    );
                    $skipwr1.append($a);
                    $skipwr.append($skipwr1);
                    $ParentBlock.append($skipwr);
                }              
            });

            $(".swipebox").swipebox();
        });
    }
});


