importScripts('/lib/microsoft/signalr/dist/browser/signalr.js');

var serverHub;


//安裝階段時觸發
self.addEventListener('install', function (event) {
    console.log('[Service Worker] Installing Service Worker ...', event);
});

//激活階段時觸發
self.addEventListener('activate', function (event) {
    console.log('[Service Worker] Activating Service Worker ...', event);
    return self.clients.claim(); //確保被載入&激活
})

//監聽 fetch 
//self.addEventListener('fetch', function (event) {
//    console.log('[Service Worker] Fetch somthing ...', event);
//    //event.respondwith 攔截外部請求
//    event.respondWith(fetch(event.request));
//})

self.addEventListener('message', function (event) {
    var data = event.data;
    if (data.command == 'oneWayCommunication') {
        console.log('Message from the Page:', data.message);
    }

    if (data.command == 'CreateServerHub') {
        CreateServerHub(data.url);
    }

    if (data.command == 'StopServerHub') {
        StopServerHub();
    }
})

//建立長連接
function CreateServerHub(url) {

    console.log({ url });
    console.log({ serverHub });
    

    if (serverHub != undefined && serverHub.state == 'Connected') {
        console.log('已建立過連接');
        console.log(serverHub.state);
        return;
    }
    
    //serverHub = new signalR.HubConnectionBuilder()
    //    .withUrl('http://localhost:6372/ServerHub')
    //    .build();

    serverHub = new signalR.HubConnectionBuilder()
        .withUrl(url)
        .build();

    serverHub.start().then(() => {
        console.log('serverHub Connected');
    }).catch((res) => {        
        console.error('serverHub 連接錯誤');
        console.log(res);
    })

    serverHub.on('SendMessageToFrontedUser', function (user, message) {
        console.log(message)
    })
    
}

function StopServerHub() {
    serverHub.stop();
}