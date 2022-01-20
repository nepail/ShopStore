$(document).ready(function () {
    Layout.Init();
    Layout.SW.Reg();
})

var Layout = {

    Init() {

        Layout.Default.Setting.Slick();
        Layout.Default.Setting.Toastr();

        Layout.SetBtn();
        Layout.On.Unload();
    },

    On: {
        Unload() {
            $(window).on('unload', function (e) {
                e.preventDefalue();
                StopServerHub();
            })
        }
    },

    SetBtn() {
        //User 鈴鐺通知
        $('#btnNotify').click(function () {
            var $notification = $('#notification');
            var $notifyText = $('.notify-text');

            $('#blip').css('opacity', 0);

            $notification.toggleClass('open');
            $notifyText.toggleClass('show');
        })
    },

    Default: {
        Setting: {
            Slick() {
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
            },
            Toastr() {
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

            }
        }
    },

    DATA: {
        //檢查未讀訊息
        CheckUserAlert() {
            $.ajax({
                url: '/Member/CheckUserAlert',
                success: res => {
                    if (res.success) {                        
                        Layout.UC.RenderUserAlert(res.item);                        
                    }
                },
                error: res => {
                    swal('CheckUserAlert Error', 'Network Error', 'error')
                }
            })
        }
    },

    UC: {
        //有未讀訊息，渲染div
        RenderUserAlert(item) {
            var content = '';
            for (var i = 0, len = item.length; i < len; i++) {
                content += `
             <div class="notify-content">
                <div class="notify-text">
                    <h1>${item[i].alertTime}</h1>
                    <p>${item[i].orderId} : ${item[i].stateMsg}</p>
                </div>
            </div>`
            }

            $('#notification').html(content);
            $('#blip').css('opacity', 1);
        }
    },

    SW: {
        Reg() {
            //檢查是否註冊過ServiceWorker
            navigator.serviceWorker.getRegistrations().then(registrations => {

                if (registrations.length > 0) {
                    console.log('service worker already Registration')
                } else {
                    //註冊ServiceWorker
                    RegistrationServiceWorker();
                }

            });

            //註冊ServiceWorker
            function RegistrationServiceWorker() {

                if ('serviceWorker' in navigator) {
                    navigator.serviceWorker
                        .register('/serviceWorker.js')//註冊 Service Worker
                        .then(function (reg) {
                            console.log('Registration succeeded. Scope is ' + reg.scope); //註冊成功
                        })
                        .catch(function (error) {
                            console.log('Registration failed with ' + error); //註冊失敗
                        })
                }
            }
        }
    }
}


//呼叫ServiceWorker
function RemoteServiceWorker(context, method) {

    if (navigator.serviceWorker.controller) {

        //建立 MessageChannel
        var messageChannel = new MessageChannel();

        //Port1 監聽來自SW的訊息
        messageChannel.port1.onmessage = function (event) {

            if (event.data.type == 'UserAlert') {
                toastr.success(event.data.stateMsg, `您的訂單編號(#${event.data.orderId})`);
            }

        }

        //發送訊息至SW，同時挾帶 port2 使SW可以回傳訊息
        navigator.serviceWorker.controller.postMessage({
            'command': method,
            'message': context,
            'url': window.location.origin + '/ServerHub'
        }, [messageChannel.port2]);
    }
}


//斷開長連接
function StopServerHub() {
    if (navigator.serviceWorker.controller) {
        navigator.serviceWorker.controller.postMessage({
            'command': 'StopServerHub'
        })
    }
}


