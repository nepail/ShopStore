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


//var serverHub = new signalR.HubConnectionBuilder().withUrl('/ServerHub').build();

//serverHub.start().then(function () {
//    console.log('--- Server Connection Start ---');
//}).catch(function (err) {
//    console.error('--- fail ---')
//})

//設定範圍 '/serviceWorker.js', { scope: '/test/' }


navigator.serviceWorker.getRegistrations().then(registrations => {    

    if (registrations.length > 0) {
        console.log('service worker already Registration')
    } else {
        RegistrationServiceWorker();
    }

});


function RegistrationServiceWorker() {
    if ('serviceWorker' in navigator) {
        console.log('--service worker yes')
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

function RemoteServiceWorker(context, method) {
    if (navigator.serviceWorker.controller) {
        navigator.serviceWorker.controller.postMessage({
            'command': method,
            'message': context,
            'url': window.location.origin + '/ServerHub'
        });
    }
}

//if (window.Worker) {
//    var myWorker = new Worker('/js/Frontend/Shared/worker.js');
//}