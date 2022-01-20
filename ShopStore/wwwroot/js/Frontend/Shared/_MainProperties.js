/*總緩存*/

const Mainproperties = {
    //產品列表
    productData: [
        {            
            ajaxsign: '',
            item: [{
                f_id: '',
                f_name: '',
                f_price: 0,
                f_picName: '',
                f_description: '',
                f_content: '',
                f_categoryId: 0,
                catrgoryName: '',
                f_stock: 0,
                f_isdel: 0,
                f_isopen: 0,
                CreateTime: '',
            }]
        }
    ]
}

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


$(window).on('unload', function (e) {
    e.preventDefalue();
    navigator.serviceWorker.controller.postMessage({
        'command':'StopServerHub'
    })    
})


//Ajax 向後端詢問未讀訊息
function CheckUserAlert() {
    $.ajax({
        url: '/Member/CheckUserAlert',
        success: res => {
            if (res.success) {
                console.log(res.item);
                RenderUserAlert(res.item);
            }
        },
        error: res => {
            swal('CheckUserAlert Error','Network Error','error')
        }
    })
}

//有未讀訊息，渲染div
function RenderUserAlert(item) {
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





