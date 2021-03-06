﻿var $modal, $modalTitle, $modalBody, $modalFooter
var change = 0;

$(document).ready(function () {

    //специфический для med.cap.ru
    //показываем селект "новове в медеицине" если выбрана соответствующая категория
    if ($('#itemGroups-select').length > 0) {
        var NewInMedicin = $('#NewInMedicin');
        SpotNewInMedicin();
        $('#itemGroups-select').change(function () {
            SpotNewInMedicin();
        });
        function SpotNewInMedicin() {
            NewInMedicin.hide();
            $('#itemGroups-select').find('option:selected').each(function () {
                if ($(this).attr('value') === '6303b7c5-5404-4ec0-aed2-1c308992c78a') {
                    NewInMedicin.show();
                }
            });
        }
    }



    $('#adminDomain-select').change(function () {
        //ajax get defaultdomain function mast be here

        //window.location.href = this.options[this.selectedIndex].value
    });

    $modal = $('.modal');
    $modalTitle = $('.modal .modal-title');
    $modalBody = $('.modal .modal-body');
    $modalFooter = $('.modal .modal-footer');

    // Если система администрирования загружена в frame открываем её в родительском окне
    if (top !== self) {
        console.log('test');
        top.location.href = location.href;
    }


    //locked removed in v > 4.0 (it was in v3.5.2)
    //<option locked="locked">
    $(".select2").select2({
        language: "ru",
        width: "100%",
        allowClear: false,
        templateSelection: function (tag, container) {
            // here we are finding option element of tag and
            // if it has property 'locked' we will add class 'locked-tag' 
            // to be able to style element in select
            var $option = $('.select2 option[value="' + tag.id + '"]');
            if ($option.attr('locked')) {
                $(container).addClass('locked-tag');
                tag.locked = true;
            }
            return tag.text;
        }
    })
    .on('select2:unselecting', function (e) {
        // before removing tag we check option element of tag and 
        // if it has property 'locked' we will create error to prevent all select2 functionality
        if ($(e.params.args.data.element).attr('locked')) {
            e.preventDefault();
        }
    });

    function iformat(icon) {
        var originalOption = icon.element;
        return $('<span><i class="fa ' + $(originalOption).data('icon') + '"></i> ' + icon.text + '</span>');
    }
    $(".select2ico").select2({
        language: "ru",
        width: "100%",
        allowClear: false,
        templateSelection: iformat,
        templateResult: iformat,
    })
        .on('select2ico:unselecting', function (e) {
            // before removing tag we check option element of tag and 
            // if it has property 'locked' we will create error to prevent all select2 functionality
            if ($(e.params.args.data.element).attr('locked')) {
                e.preventDefault();
            }
        });



    $(".iCheck").iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'icheckbox_square-blue'
        //increaseArea: '%' //optional
    });

    // Полоса прокрутки главного меню
    $(".menu-block").mCustomScrollbar();
    // Полоса прокрутки
    $('.scrollbar').mCustomScrollbar();



    //События Кнопок
    $('input[type=submit], .button').bind({
        mousedown: function () {
            //логика всплывающих окон
            var Action = $(this).attr('data-action');
            if (Action !== undefined) {
                switch (Action) {
                    case "delete":
                        $('form input[required]').removeAttr('required');
                        $('form select[required]').removeAttr('required');
                        Confirm('Уведомление', 'Вы хотите удалить эту запись?', $(this));
                        break;
                    case "cancel":
                        $('form input[required]').removeAttr('required');
                        $('form select[required]').removeAttr('required');
                        if (change !== 0) {
                            Confirm('Уведомление', 'Выйти без изменений?', $(this));
                        }
                        else {
                            $('form input[required]').removeAttr('required');
                            $('form select[required]').removeAttr('required');
                            $(this).trigger('click');
                        }

                        break;
                    case "noPreloader-accept":
                        break;
                        //case
                    default: return false;
                }
            }

        },
        click: function () {
            var btn_class = $(this).attr('value');
            var dataAction = $(this).attr('data-action');
            var req_count = $('form input[required]:invalid').length

            if (req_count > 0 && btn_class === 'save-btn') {
            }
            else if (dataAction === 'noPreloader-accept') {
            }
            else {
                // показываем preloader при клике на кнопку
                var $load_bg = $("<div/>", { "class": "load_page" });
                $load_bg.bind({
                    mousedown: function () { return false; },
                    selectstart: function () { return false; }
                });

                $('body').append($load_bg);
            }
        }
    });

    // Панель авторизации пользователя    
    $('.account-info').click(function () {
        $('.admin-settings').toggle();
    });

    // Показывает в модальном окне фрейм
    $('a.pop-up_frame').mousedown(function () {
        PopUpFrame($(this));
        return false;
    });

    // Показываем сообщение в модальном окне
    $('.modal[data-show="true"]').modal('toggle');
    $('.modal-footer a[data-action=false]').bind({
        click: function () {
            $('.modal').modal('toggle');
            setCursor();
            return false;
        }
    })

    // инициализаация полей даты
    $('input[data-type=date').datepicker({ onSelect: function (dateText, inst) { $(this).attr('value', dateText); } });
    // инициализаация TinyMCE
    $('textarea[type=editor]').each(function () {
        InitTinyMCE_new($(this).attr('id'), 0, $(this).attr('height'), $(this).attr('data-dir'));//"/UserFiles/"
    });
    $('textarea[type=liteeditor]').each(function () {
        InitLiteTinyMCE($(this).attr('id'), 0, 200);
    });
    // Инициализация чекбоксов
    // Инициализация полей выбора файлов
    // Инициализация полей с выпадающим списком


    //Вызов плагина маски ввода
    //alert($('input[data - mask]').data('mask'));
    $('input[data-mask]').each(function () {
        $(this).mask($(this).attr('data-mask'));
    });

    //сортировка фотографий галлери
    $('#sorting_photo').click(function () {
        if ($('.photoalbum').hasClass('Sortable')) {
            location.reload();
        }
        else {
            $('.photoalbum').addClass('Sortable');
            $('.Sortable').each(function () { sortingPhotoInit($(this)); });
        }
    });

    if ($('.list-rss').length > 0) {
        $('.import-rss-new').click(function (e) {
            e.preventDefault();
            var id = $(this).attr('data-id');

            $.ajax({
                type: "POST",
                async: false,
                url: "/Admin/Materials/RealizeRssImport/",
                data: { id: id },
                error: function () { alert("error"); },
                success: function (data) {
                    //$Container.remove();
                    location.reload();
                }
            });

        });


    }
    $('.rss_delete').click(function (e) {
        e.preventDefault();
        var id = $(this).attr('data-id');
        $.ajax({
            type: "POST",
            async: false,
            url: "/Admin/Materials/DeleteRssLenta/",
            data: { id: id },
            error: function () { alert("error"); },
            success: function (data) {
                //$Container.remove();
                location.reload();
            }
        });

    });

    //удаление фотогрфии из галлереи
    if ($('.photoalbum').length > 0) {

        // Центрирование фотографий по вертикали в фотоальбоме
        //var photoImg = $('.photoalbum').find('img');
        //photoImg.each(function () {
        //    var margin = ($(this).parent().height() - $(this).height()) / 2;
        //    $(this).css('margin', margin + 'px 0');
        //});

        // Удаление фотографий из альбома
        $('.delPhoto').click(function () {
            var elem = $(this);
            var id = $(this).attr('data-id');

            $.ajax({
                type: "POST",
                url: '/admin/photoalbums/DeletePhoto/' + id,
                contentType: false,
                processData: false,
                data: false,
                error: function () { elem.parent().remove() },
                success: function (result) {
                    if (result === "true") {
                        elem.parent().remove()
                    }
                    else {
                        if (result !== '') alert(result);
                    }
                }
            });


        });
    }

    // Изменение приоритета
    if ($(".sortable").length > 0) $('.sortable').each(function () {
        Sorting_init($(this));
    });

    // Перехват нажатия клавиш клавиатуры
    $(window).keydown(function (e) {
        //alert(e.keyCode);
        if (e.keyCode === 112) { $('div#HelpIcons a.HelpIcon').trigger('click'); return false; }
        if ((e.ctrlKey) && (e.keyCode === 83)) { $('button[value="create-btn"]').trigger('click'); $('button[value="update-btn"]').trigger('click'); return false; }
        if ((e.ctrlKey) && (e.keyCode === 68)) { $('button[value="delete-btn"]').trigger('click'); return false; }
        if ((e.ctrlKey) && (e.keyCode === 73)) { $('button[value="insert-btn"]').trigger('click'); return false; }
        if (e.keyCode === 27) { $('button[value="cancel-btn"]').trigger('mousedown'); return false; }
        if ((e.ctrlKey) && (e.keyCode === 13)) { FindNext(window.getSelection().toString()); }
    });

    // Помечаем страницу при изменении контента
    $('input, select, textarea').bind({
        change: function () {
            change = 1;
            requiredTest();
        },
        keyup: function () {
            change = 1;
            requiredTest();
        }
    });

    $('.preview_del').on('click', function () {
        change = 1;
        requiredTest();
    });


    //Добавляет связь сайт-модуль
    $("#add-module-btn").on("click", function (e) {
        e.preventDefault();
        var _moduleId = $("#itemModule-select").val();
        var _siteId = $(this).data("siteId");
        var _action = $(this).data("url");
        try {
            $.ajax({
                method: "POST",
                url: _action,
                async: false,
                data: { siteId: _siteId, moduleId: _moduleId },
            })
                .done(function (response) {
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                })
                .always(function (response) {
                    location.reload();
                });
        }
        catch (ex) {
            console.log(ex);
        }
    });

    //Добавляет связь шаблон-модуль
    $(".set-template-default-btn").on("click", function (e) {
        e.preventDefault();
        var _linkId = $(this).data("linkId");
        var _templateId = $(this).data("templateId");
        var _action = $(this).data("url");
        try {
            $.ajax({
                method: "POST",
                url: _action,
                async: false,
                data: { linkId: _linkId, templateId: _templateId },
            })
                .done(function (response) {
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                })
                .always(function (response) {
                    location.reload();
                });
        }
        catch (ex) {
            console.log(ex);
        }
    });

    //удаляем вариант ответа
    $('.delete-answer-btn').on("click", function (e) {
        e.preventDefault();
        var _linkId = $(this).data("linkId");
        var _action = $(this).data("url");
        var _container = $(this).closest("tr");


        try {
            $.ajax({
                method: "POST",
                url: _action,
                async: false,
                data: { linkId: _linkId },
            })
                .done(function (response) {
                    if (response === "Success") {
                        _container.remove();
                    }
                    else {
                        alert("Произошла ошибка!");
                    }
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                })
                .always(function (response) {
                    location.reload();
                });
        }
        catch (ex) {
            console.log(ex);
        }

    });


    //Удаляет связь сайт-модуль
    $(".delete-module-btn").on("click", function (e) {
        e.preventDefault();
        var _linkId = $(this).data("linkId");
        var _action = $(this).data("url");

        var _container = $(this).closest("tr");

        try {
            $.ajax({
                method: "POST",
                url: _action,
                async: false,
                data: { linkId: _linkId },
            })
                .done(function (response) {
                    if (response === "Success") {
                        _container.remove();
                    }
                    else {
                        alert("Произошла ошибка!");
                    }
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                })
                .always(function (response) {
                    location.reload();
                });
        }
        catch (ex) {
            console.log(ex);
        }
    });

    //Изменяет права роли
    $(".roleClaim-chckbx").on('ifToggled', function (e) {
        var _checked = ($(this).is(':checked')) ? true : false;

        var roleClaim = {
            Id : $(this).data("claimId"),
            RoleId : $("#Item_Id").val(),
            Type : $(this).data("claimType"),
            Value: $(this).data("claimValue"),
            Section: $(this).data("claimSection"),
            Checked: _checked
        };


        var _action = "/Admin/Roles/UpdateRoleClaim";
        try {
            $.ajax({
                method: "POST",
                url: _action,
                async: false,
                data: roleClaim
            })
                .done(function (response) {
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                })
                .always(function (response) {
                    //location.reload();
                });
        }
        catch (ex) {
            console.log(ex);
        }
    });

    //Добавляет связь пользователь-роль
    $("#add-userRoleLink-btn").on("click", function (e) {
        e.preventDefault();
        var _role = $("#itemRole-select").val();
        var _userId = $(this).data("userId");
        var _action = $(this).data("url");

        try {
            $.ajax({
                method: "POST",
                url: _action,
                async: false,
                data: { userId: _userId, role: _role },
            })
                .done(function (response) {
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                })
                .always(function (response) {
                    location.reload();
                });
        }
        catch (ex) {
            console.log(ex);
        }
    });

    
    $("#add-userSiteLink-btn").on("click", function (e) {
        e.preventDefault();
        var _siteId = $("#itemSite-select").val();
        var _userId = $(this).data("userId");
        var _action = $(this).data("url");

        try {
            $.ajax({
                method: "POST",
                url: _action,
                async: false,
                data: { userId: _userId, siteId: _siteId },
            })
                .done(function (response) {
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                })
                .always(function (response) {
                    location.reload();
                });
        }
        catch (ex) {
            console.log(ex);
        }
    });

    //Удаляет связь пользователь-роль
    $(".delete-userRoleLink-btn").on("click", function (e) {
        e.preventDefault();
        var _role = $(this).data("role");
        var _userId = $(this).data("userId");
        var _action = $(this).data("url");

        var _container = $(this).closest("tr");

        try {
            $.ajax({
                method: "POST",
                url: _action,
                async: false,
                data: { role: _role, userId: _userId },
            })
                .done(function (response) {
                    if (response === "Success") {
                        _container.remove();
                    }
                    else {
                        alert("Произошла ошибка!");
                    }
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                })
                .always(function (response) {
                    location.reload();
                });
        }
        catch (ex) {
            console.log(ex);
        }
    });

    //Добавляет связь сайт-роль
    $("#add-roleSiteLink-btn").on("click", function (e) {
        e.preventDefault();
        var _siteId = $("#portalRole-siteSelect").val();
        var _roleId = $(this).data("roleId");
        var _action = $(this).data("url");

        try {
            $.ajax({
                method: "POST",
                url: _action,
                async: false,
                data: { roleId: _roleId, siteId: _siteId },
            })
                .done(function (response) {
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                })
                .always(function (response) {
                    location.reload();
                });
        }
        catch (ex) {
            console.log(ex);
        }
    });

    //Удаляет связь сайт-роль
    $(".delete-roleSiteLink-btn").on("click", function (e) {
        e.preventDefault();
        var _siteId = $(this).data("siteId");
        var _roleId = $(this).data("roleId");
        var _action = $(this).data("url");

        try {
            $.ajax({
                method: "POST",
                url: _action,
                async: false,
                data: { roleId: _roleId, siteId: _siteId },
            })
                .done(function (response) {
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                })
                .always(function (response) {
                    location.reload();
                });
        }
        catch (ex) {
            console.log(ex);
        }
    });

    //Домен по умолчанию
    $(".set-domain-default-btn").on("click", function (e) {
        e.preventDefault();
        var _linkId = $(this).data("linkId");
        var _action = $(this).data("url");
        try {

            $.ajax({
                method: "POST",
                url: _action,
                async: false,
                data: { id: _linkId },
            })
                .done(function (response) {
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                })
                .always(function (response) {
                    location.reload();
                });
        }
        catch (ex) {
            console.log(ex);
        }
    });

    //удаление домена
    $('.delete-domain-btn').on("click", function (e) {
        e.preventDefault();

        var _domainId = $(this).attr("data-id");
        var domainName = $(this).data("domainName").trim();
        var _action = $(this).data("url");

        var _container = $(this).closest("tr");

        if (domainName === "localhost") {
            $("#domain_localhost").data("content", "Нельзя удалить значение: localhost! Если необходимо переназначить, добавьте нужному сайту этот домен. Система сделает это автоматически.")
            .popover('show');

            setTimeout(function () {
                $("#domain_localhost").popover('hide');
            }, 3000);
            return false;
        }

        $.ajax({
            type: "POST",
            async: false,
            url: _action,
            data: { id: _domainId }
        })
            .done(function (response) {
                if (response === "Success") {
                    _container.remove();
                }
                else {
                    alert("Нельзя удалить основной домен!");
                }
            })
            .fail(function (jqXHR, status) {
                console.log("Ошибка" + " " + status + " " + jqXHR);
                alert("Произошла непредвиденная ошибка!")
            })
            .always(function (response) {
                location.reload();
            });

    });




    //удаление варианта ответа
    $('.answer_delete').click(function (e) {
        e.preventDefault();
        var idAnswer = $(this).attr("data-id");
        var $Container = $(this).parent().parent();
        $.ajax({
            type: "POST",
            async: false,
            url: "/admin/vote/delanswer",
            data: { id: idAnswer },
            error: function () { alert("error"); },
            success: function (data) {
                $Container.remove();
            }
        });
    });

    

    // валидация обязательных полей для заполнения 
    requiredTest();

    // Скрываем или раскрываем блоки
    if ($('div.group-block').length > 0) GroupBlock_init();

    // устанавливаем курсор
    setCursor();

    // Показываем страницу, убираем preloader
    load_page();

    // Добавляем возможность сортировки
    $('#sorting_element,.sorting_element_on').click(function () {

        var SortList;
        if ($(this).hasClass('sorting_element_on')) {
            SortList = $(this).parent().parent().parent().find('.sort_list');
        }
        else {
            SortList = $(this).parent().parent().parent().parent().find('.sort_list');
        }

        if (SortList.hasClass('sort_list_on')) {
            location.reload();
        }
        SortList.addClass('sort_list_on');

        SortList.find('tbody').addClass('sortable');

        // сортировка для нетабличных записей
        if ($('.sort_parent').length > 0) {
            SortList.find('.sort_parent').addClass('sortable');
        }


        $('.sortable').each(function () {
            Sorting_init($(this));
        });

        //if ($('.sort_list').hasClass('sort_list_on')) {
        //    location.reload();
        //}
        //$('.sort_list').addClass('sort_list_on');

        //$('.sort_list').find('tbody').addClass('sortable');

        //// сортировка для нетабличных записей
        //if ($('.sort_parent').length > 0) {
        //    $('.sort_list').find('.sort_parent').addClass('sortable');
        //}


        //$('.sortable').each(function () {
        //    Sorting_init($(this));
        //});

    });

    //создание сайта - выбор типов создаваемого сайта
    function SpotTypeSite() {
        $Org.hide();
        $People.hide();
        $Event.hide();
        switch ($Type.val()) {
            case 'org': $Org.show();
                if ($Org.find('select').val() === '') {
                    $Org.addClass('invalid');
                }
                else {
                    $Org.removeClass('invalid');
                }

                $People.removeClass('invalid');
                $Event.removeClass('invalid');
                $Org.find('select').attr('required', '');
                $People.find('select').removeAttr('required');
                $Event.find('select').removeAttr('required');
                break;
            case 'spec':
                $People.show();
                $Org.removeClass('invalid');
                $People.addClass('invalid');
                $Event.removeClass('invalid');
                $Org.find('select').removeAttr('required');
                $People.find('select').attr('required', '');
                $Event.find('select').removeAttr('required');
                break;
            case 'event':
                $Event.show();
                $Org.removeClass('invalid');
                $People.removeClass('invalid');
                $Event.addClass('invalid');
                $Org.find('select').removeAttr('required');
                $People.find('select').removeAttr('required');
                $Event.find('select').attr('required', '');
                break;
            default:
                break;
        }
    };

    function SpotContent(obj) {
        document.getElementById('Item_ContentId').value = obj.val();
        if (obj.val() !== '') {
            $Org.removeClass('invalid');
            $People.removeClass('invalid');
            $Event.removeClass('invalid');
        }
    }

    if ($('#site_type').length > 0) {
        var $Type = $('#site_type');
        var $Org = $('#site_org');
        var $People = $('#site_spec');
        var $Event = $('#site_event');
        var $ContentId = $('#Item_ContentId');

        SpotTypeSite();
        $Type.change(function () {
            $ContentId.val('');
            SpotTypeSite();
        });

        $('#contid_wr select').change(function () {
            SpotContent($(this));
        });
    }


    // Инициализация полей для выбора файлов
    //if ($('.uc_fileupload').length > 0) ucFileUpload_Init();

    //$('#my-imageupload').imageupload({
    //    allowedFormats: ['jpg'],
    //    maxFileSizeKb: 512
    //});
    //$('#my-imageupload').imageupload('reset');
    

});

// Инициализация полей для выбора файлов
//function ucFileUpload_Init() {
//    $('.uc_fileupload').each(function () {
//        var $BlockTitle = $(this).find('.uc_title');
//        var $ControlBlock = $(this).find('.uc_control');
//        var $BlockPreview = $(this).find('.uc_preview');
//        var $ResultInput = $(this).find('input[type=hidden]');

//        //file info
//        if ($(this).find('.uc_preview_img').length > 0) {
//            var id = $(this).find('.uc_input').attr('id');
            
//            var File = document.getElementById(id);
//            var FileName = File.defaultValue.replace(/^.*[\\\/]/, '');
//            var FileType = File.type;
//            var FileSize = File.size;


//            var $PreviewBlock = $("<div/>", { 'class': 'uc_preview_item' });
            



//            // если картинка
//            //var img = $("<img/>");
//            //img.attr('src', File.defaultValue);
//            //$ImgBlock.append(img);

            
        

//            var $InfoBlock = $("<div/>", { 'class': 'uc_preview_info' });
//            $InfoBlock.append('<div class="uc_preview_name">' + FileName + '</div>');
//            $InfoBlock.append('<div class="uc_preview_size">' + FileSize + '</div>');

//            // Кнопка Удалить
//            var $DelPreview = $("<div/>", { 'class': 'uc_preview_btn' }).append('удалить');
//            $DelPreview.click(function () {
//                $ControlBlock.find('input:file').removeClass('UnLook');
//                $ResultInput.val('');
//                $BlockPreview.empty();
//            });

//            $InfoBlock.append($DelPreview);
//            $PreviewBlock.append($InfoBlock);
//            $BlockPreview.append($PreviewBlock);
//        }

//        // Разворачивает и сворачивает список файлов
//        $BlockTitle.click(function () {
//            var Class = $(this).parent().attr('class');

//            $ControlBlock.slideToggle(
//                function () {
//                    if (Class.indexOf('Open') == -1) $(this).parent().addClass('Open');
//                    else $(this).parent().removeClass('Open');
//                });

//            return false;
//        });

//        // Добавить файл при помощи кнопки
//        $ControlBlock.find('input:file').change(function (e) {
//            $(this).addClass('UnLook');
//            $BlockPreview.empty();

//            var File = $(this)[0].files[0];
//            var FileName = File.name;
//            var FileType = File.type;
//            var FileSize = File.size;

            
//            $BlockPreview.empty();

//            $ResultInput.val(FileName);


//            var $PreviewBlock = $("<div/>", { 'class': 'uc_preview_item' });
//            var $ImgBlock = $("<div/>", { 'class': 'uc_preview_img' });
//            // если картинка
//            if (FileType.indexOf('image') > -1) {
//                var img = $("<img/>");
//                img.attr('src', window.URL.createObjectURL(File));
//                $ImgBlock.append(img);
//            }
//            else {
//                $ImgBlock.append(File.substring(FileName.lastIndexOf(".") + 1));
//            }
//            $PreviewBlock.append($ImgBlock);

//            var $InfoBlock = $("<div/>", { 'class': 'uc_preview_info' });
//            $InfoBlock.append('<div class="uc_preview_name">' + FileName + '</div>');
//            $InfoBlock.append('<div class="uc_preview_size">' + FileSize + '</div>');

//            // Кнопка Удалить
//            var $DelPreview = $("<div/>", { 'class': 'uc_preview_btn' }).append('удалить');
//            $DelPreview.click(function () {
//                $ControlBlock.find('input:file').removeClass('UnLook');
//                $ResultInput.val('');
//                $BlockPreview.empty();
//            });

//            $InfoBlock.append($DelPreview);
//            $PreviewBlock.append($InfoBlock);
//            $BlockPreview.append($PreviewBlock);
//        });

//        // Кнопка "Удалить"
//        $(this).find('.uc_preview_Del').click(function () {
//            $ControlBlock.find('input:file').removeClass('UnLook');
//            $ResultInput.val('');
//            $BlockPreview.empty();

//            return false;
//        });
//    });
//}


var validSumm = $('.validation-summary-valid');
if (validSumm.length > 0 && validSumm.find('li')[0].innerHTML !== '') validSumm.css('display', 'block');

// Если есть пустые, обязательные для заполнения поля, делаем не активной главную кнопку для сохранеиния записи
function requiredTest() {
    var emptyRequiredLength = $('form input[required]:invalid').length;
    emptyRequiredLength = emptyRequiredLength + $('form select[required]:invalid').length;

    if (emptyRequiredLength > 0 || change === 0)
        $('button[data-primary]').animate({ opacity: "0.3" }, 300).attr('disabled', '');
    else
        $('button[data-primary]').animate({ opacity: "1" }, 300).removeAttr('disabled');
};

// Показываем страницу, убираем preloader
function load_page() {
    var $load_bg = $('div.load_page');
    setTimeout(function () {
        $load_bg.fadeOut();
    }, 300);
};

// Очищаем модальное окно
function clear_modal() {
    $modal.find('.modal-dialog').removeClass().addClass('modal-dialog'),
    $modalTitle.empty();
    $modalBody.empty();
    $modalFooter.empty();
};




// Всплывающие окна
function Confirm(Title, Body, Object) {
    clear_modal();

    var $BtnOk = $('<button/>', { 'id': 'modal-btn-ok', 'class': 'btn btn-danger' }).append('Да');
    $BtnOk.click(function () {
        $('form input[required]').removeAttr('required');
        Object.trigger('click');
    });

    var $BtnNo = $('<button/>', { 'id': 'modal-btn-no', 'class': 'btn btn-default' }).append('Нет');
    $BtnNo.click(function () {
        $('.modal').modal('toggle');
    });

    $modalTitle.append(Title);
    $modalBody.append('<p>' + Body + '</p>');
    $modalFooter.append($BtnOk).append($BtnNo);

    $modal.modal('toggle');
};
// Всплывающие окна
//function ModalConfirm(Title, Body, callback) {
//    clear_modal();

//    var $BtnOk = $('<button/>', { 'id': 'modal-btn-ok', 'class': 'btn btn-danger' }).append('Да');
//    $BtnOk.click(function () {
//        $('form input[required]').removeAttr('required');
//        return callback;
//    });

//    var $BtnNo = $('<button/>', { 'id': 'modal-btn-no', 'class': 'btn btn-default' }).append('Нет');
//    $BtnNo.click(function () {
//        $('.modal').modal('toggle');
//    });

//    $modalTitle.append(Title);
//    $modalBody.append('<p>' + Body + '</p>');
//    $modalFooter.append($BtnOk).append($BtnNo);

//    $modal.modal('toggle');
//    return false;
//};

// Всплывающие окна с активным контентом
function PopUpFrame(Object) {
    clear_modal();

    $frale = $('<iframe/>', { 'class': 'modal_frame', 'frameborder': '0', 'height': '20', 'src': Object.attr("href"), 'sandbox': 'allow-same-origin allow-scripts allow-forms allow-top-navigation', 'scrolling': 'no', 'style': 'overflow: hidden; width:100%;' });

    $modal.find('.modal-dialog').addClass(' modal-lg'),
    $modalTitle.append(Object.attr("title"));


    ////////////////////////////////////////////////
    //var modalBody = $(".modal .modal-body");
    //var loader = "<div style='background-color: rgba(46,80,102,.5);' class='text-center'><img src='/Content/img/preloader.svg'></div>";
    //modalBody.html(loader);
    //var res = $.ajax({
    //    url: Object.attr("href"),
    //    cache: false
    //    })
    //  .done(function (response) {
    //      modalBody.html(response);
    //  })
    //  .fail(function (jqXHR, textStatus) {
    //      alert("Request failed: " + textStatus);
    //  });

    $modalBody.append("<div class='modal-preloader-block text-center'><img src='/Content/img/preloader.svg'/></div>");
    ///////////////////////////////////////////////
    $modalBody.append($frale);

    $modal.modal('toggle');

    $('.modal').on('hidden.bs.modal', function (e) {
        e.preventDefault();
        History.pushState(null, document.title, location.href);
        loadPage(location.href);

        function loadPage(url) {
            $('.filtr-list').load(url + ' .filtr-list > *', function () {
                $('.filtr-list a.pop-up_frame').click(function () {
                    PopUpFrame($(this));
                    return false;
                });
            });
        }

        History.Adapter.bind(window, 'statechange', function (e) {
            var State = History.getState();
            loadPage(State.url);
        });
        //location.reload();
    })
};

// Сортировка
function Sorting_init(Object) {
    Object.sortable({
        axis: "y",
        start: function () { $(this).addClass('active'); },
        stop: function (event, ui) {
            $(this).removeClass('active');

            var _ServiceUrl = $(this).attr('data-service');
            var _SortableItem = ui.item;
            var _Id = _SortableItem.attr('data-id');
            var _Num = $(this).find('.ui-sortable-handle').index(_SortableItem) + 1;

            _ServiceUrl = _ServiceUrl;

            $.ajax({
                type: "POST",
                async: true,
                url: _ServiceUrl,
                data: { id: _Id, position: _Num },
                error: function () { Content = '<div>Error!</div>'; },
                success: function (data) { }
            });
        }
    }).disableSelection();
};

// Инициализация раскрывающегося блока
function GroupBlock_init() {
    $('div.group-block').each(function () {
        $(this).wrapInner('<div class="group-block_info"></div>');
        var $BlockTitle = $("<div/>", { "class": "group-block_title" }).append($(this).attr('title'));
        var $BlockInfo = $(this).find('.group-block_info');

        $BlockTitle.click(function () {
            var Class = $(this).parent().attr('class');

            $BlockInfo.slideToggle(
                function () {
                    if (Class.indexOf('open') === -1) $(this).parent().addClass('open');
                    else $(this).parent().removeClass('open');
                });

            return false;
        });

        $(this).prepend($BlockTitle);
    });
};

// устанавливаем курсор
function setCursor() {
    if ($('.content input.input-validation-error').length > 0) $('.content input.input-validation-error:first').focus();
    else if ($('.content input[required]').val() === '') $('.content input[required]:first').focus();
    else if ($('.content input:not([type=file]):not([data-focus=False])').length > 0) $('.content input:not([type=file]):not([data-focus=False]):first').focus();
};



function sortingPhotoInit(Object) {
    Object.sortable({
        axis: "x, y",
        start: function () { $(this).addClass('Active'); },
        stop: function (event, ui) {
            $(this).removeClass('Active');
            var _Album = $('.photoalbum').data('id');
            var _ServiceUrl = $(this).attr('data-service');
            var _SortableItem = ui.item;
            var _Id = _SortableItem.find('div').attr('data-id');
            var _Num = $(this).find('.ui-sortable-handle').index(_SortableItem) + 1;
            //var _section = _SortableItem.attr('data-section');

            _ServiceUrl = _ServiceUrl;

            $.ajax({
                type: 'POST',
                url: _ServiceUrl,
                data: { album: _Album, id: _Id, permit: _Num },
                error: function () { Content = '<div>Error!</div>'; },
                success: function (data) { }
            });
        }
    }).disableSelection();
};


// TinyMCE
function InitTinyMCE(id, _width, _height, directory) {
    tinymce.init({
        selector: "textarea#" + id,
        //theme: "modern",
        add_unload_trigger: false,
        schema: "html5",
        plugins: [["anchor nonbreaking paste hr searchreplace  textcolor charmap  link autolink image media table visualblocks code fullscreen contextmenu gallery"]],
        toolbar: 'undo redo | styleselect fontsizeselect | bold italic underline superscript subscript | forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist | outdent indent | table | link image media gallery | removeformat code',
        contextmenu: "copy paste | link image",
        extended_valid_elements: "code",
        invalid_elements: "script,!--",
        convert_urls: false,
        relative_urls: false,
        image_advtab: true,
        image_caption: true,
        image_title: true,
        cleanup: false,
        erify_html: false,
        statusbar: false,
        language: 'ru',
        menubar: false,
        verify_html: false,
        width: _width,
        height: _height,
        file_browser_callback: function (field_name, url, type, win) {
            var cmsURL = "http://" + window.location.hostname + "/FileManager/?cmd=edit&path=" + directory;
            $('div#Canvas').after('<div id="Opacity"></div><iframe id="FileManager" src="' + cmsURL + '">Ваш браузер не поддерживает плавающие фреймы!</iframe>');

            $('iframe#FileManager').bind('load', function () { FileManagerLoad(field_name); });
            if (win.getImageData) win.getImageData();
        }
    });
};
// TinyMCE (Урезанная версия)
function InitLiteTinyMCE(id, _width, _height) {
    tinymce.init({
        selector: "textarea#" + id,
        theme: "modern",
        add_unload_trigger: false,
        schema: "html5",
        plugins: [["textcolor"]],
        toolbar: "fontsizeselect | bold italic underline | forecolor backcolor | alignleft aligncenter alignright alignjustify",
        invalid_elements: "script,!--",
        convert_urls: false,
        relative_urls: false,
        image_advtab: true,
        cleanup: false,
        erify_html: false,
        statusbar: false,
        language: 'ru',
        width: _width,
        height: _height,
        menubar: false
    });
};


// TinyMCE новая версия
function InitTinyMCE_new(id, _width, _height, directory) {
    tinymce.init({
        selector: "textarea#" + id,
        //theme: "modern",
        add_unload_trigger: false,
        schema: "html5",
        plugins: [["anchor nonbreaking paste hr searchreplace textcolor charmap link autolink image media table visualblocks code fullscreen contextmenu gallery"]],
        toolbar: 'undo redo | styleselect fontsizeselect | bold italic underline superscript subscript | forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist | outdent indent | table | link image media gallery | removeformat code',
        contextmenu: "copy paste | link image",
        extended_valid_elements: "code",
        invalid_elements: "script,!--",
        convert_urls: false,
        relative_urls: false,
        image_advtab: true,
        cleanup: false,
        statusbar: false,
        language: 'ru',
        menubar: false,
        height: _height,
        verify_html: false,
        width: _width,
        image_caption: true,
        image_title: true,
        content_css: '/Content/css/iframe_tinymce.css',
        automatic_uploads: true,
        images_upload_url: 'http://' + window.location.hostname + (location.port ? ':' + location.port : '') + '/Admin/Services/GetFile/?dir=' + directory,
        file_picker_callback: function (cb, value, meta) {
            var input = document.createElement('input');
            input.setAttribute('type', 'file');
            input.setAttribute('accept', 'image/*');
            input.onchange = function () {
                var file = this.files[0];

                var reader = new FileReader();
                reader.readAsDataURL(file);
                reader.onload = function () {
                    var id = 'blobid' + (new Date()).getTime();
                    var blobCache = tinymce.activeEditor.editorUpload.blobCache;
                    var base64 = reader.result.split(',')[1];
                    var blobInfo = blobCache.create(id, file, base64);
                    blobCache.add(blobInfo);

                    cb(blobInfo.blobUri(), { title: file.name });
                };
            };

            input.click();
        },
        //Svetlana Kuzmina
        setup: function (editor) {
            editor.on('change', function (e) {
                //console.log('change event', e);
                change = 1;
                requiredTest();
            });
        }

        //,file_picker_callback: function (field_name, url, type, win) {
        //    var cmsURL = "http://" + window.location.hostname + "/FileManager/?cmd=edit&path=" + directory;
        //    $('div#Canvas').after('<div id="Opacity"></div><iframe id="FileManager" src="' + cmsURL + '">Ваш браузер не поддерживает плавающие фреймы!</iframe>');

        //    $('iframe#FileManager').bind('load', function () { FileManagerLoad(field_name); });
        //    if (win.getImageData) win.getImageData();
        //}
    });
};

function Coords(x, y, title, desc, zoom) {
    $(document).ready(function () {
        var JQ_Ymaps = $('script[src$="//api-maps.yandex.ru/2.0/?load=package.full&lang=ru-RU"]').length;
        x = x.replace(',', '.');
        y = y.replace(',', '.');
        if (JQ_Ymaps === 0) {

            $.getScript("//api-maps.yandex.ru/2.0/?load=package.full&lang=ru-RU", function () {
                var script = document.createElement('script');
                script.type = 'text/javascript';
                script.src = "//api-maps.yandex.ru/2.0/?load=package.full&lang=ru-RU";
                document.head.appendChild(script);

                ymaps.ready(function () {
                    if (title === '') { title = "Название организации"; }
                    if (desc === '') { desc = "Описание организации"; }
                    var myMap = new ymaps.Map("map", { center: [y, x], zoom: zoom }),
                myPlacemark = new ymaps.Placemark([y, x], {
                    hasBalloon: false,
                }, { draggable: true });
                    //Перемещение метки 
                    myPlacemark.events.add("dragend", function () {
                        var coordinates = this.geometry.getCoordinates();
                        var x = this.geometry.getCoordinates(6)[1];
                        var y = this.geometry.getCoordinates(6)[0];
                        CoordPoint(y, x);
                        return false;

                    }, myPlacemark);
                    //Перемещение метки по клику
                    myMap.events.add('click', function (e) {
                        var coords = e.get('coordPosition');
                        myPlacemark.geometry.setCoordinates(coords);  //Перемещает метку на место клика                        
                        var xMap = coords[0].toPrecision(8)
                        var yMap = coords[1].toPrecision(8)
                        CoordPoint(xMap, yMap);
                        return false;
                    });
                    myMap.controls
                    .add('zoomControl', { left: 5, top: 5 })
                    .add('typeSelector')
                    .add('mapTools', { left: 35, top: 5 });
                    myMap.geoObjects.add(myPlacemark);
                });
            });
        }
    });


    //myMap.events.add('click', function (e) {
    //    var coords = e.get('coordPosition');
    //    var xMap = coords[0].toPrecision(6);
    //    var yMap = coords[1].toPrecision(6);
    //});
};


function CoordPoint(xMap, yMap) {
    $(document).ready(function () {
        var pointX = $('.Item_CoordX');
        pointX.val(String(xMap).replace(/[.]/g, ","))
        var pointY = $('.Item_CoordY');
        pointY.val(String(yMap).replace(/[.]/g, ","))

    });

};
