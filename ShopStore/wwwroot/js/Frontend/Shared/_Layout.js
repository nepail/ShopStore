$('.single-item').slick({
    dots: true,
    fade: true,
    arrows: true,
    autoplay: true,
    autoplaySpeed: 5000,
});

//card-img-item
$('.card-img-item').slick({
    //dots: true,
    //fade: true,
    //arrows: true,
    //autoplay: true,
    //autoplaySpeed: 5000,
    //centerMode: true,
    //centerPadding: '10px',
    lazyLoad: 'ondemand',
    slidesToShow: 3,
    slidesToScroll: 1
});

//ClassicEditor
//    .create(document.querySelector('#editor'))
//    .catch(error => {
//        console.error(error);
//    });

//ClassicEditor.create(document.querySelector('#editor'))
//    .then(editor => {
//        editor.ui.view.editable.element.style.height = '500px';
//    })
//    .catch(error => {
//        console.error(error);
//    });

toastr.options = {
    // 參數設定[註1]
    "positionClass": "toast-top-right",
    "closeButton": false, // 顯示關閉按鈕
    "debug": false, // 除錯
    "newestOnTop": false,  // 最新一筆顯示在最上面
    "progressBar": true, // 顯示隱藏時間進度條
    "preventDuplicates": false, // 隱藏重覆訊息
    "onclick": null, // 當點選提示訊息時，則執行此函式
    "showDuration": "300", // 顯示時間(單位: 毫秒)
    "hideDuration": "1000", // 隱藏時間(單位: 毫秒)
    "timeOut": "5000", // 當超過此設定時間時，則隱藏提示訊息(單位: 毫秒)
    "extendedTimeOut": "1000", // 當使用者觸碰到提示訊息時，離開後超過此設定時間則隱藏提示訊息(單位: 毫秒)
    "showEasing": "swing", // 顯示動畫時間曲線
    "hideEasing": "linear", // 隱藏動畫時間曲線
    "showMethod": "fadeIn", // 顯示動畫效果
    "hideMethod": "fadeOut" // 隱藏動畫效果
}


//User 鈴鐺通知
$('#btnNotify').click(function () {
    var $notification = $('#notification');
    var $notifyText = $('.notify-text');

    $('#blip').css('opacity', 0);

    $notification.toggleClass('open');
    $notifyText.toggleClass('show');   
})

function ClientAliveCheck() {
    alert('HI')
}