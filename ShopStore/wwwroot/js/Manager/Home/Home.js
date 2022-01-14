$(document).ready(function () {
    Home.InitPage();
    
})

var connection = new signalR.HubConnectionBuilder().withUrl('/chatHub').build();;


const chatButton = $('.chatbox__button');
const chatContent = $('.chatbox__support');
const icons = {
    isClicked: '<img src="~img/svg/chatbox-icon.svg" />',
    isNotClicked: '<img src="~img/svg/chatbox-icon.svg" />'
}

const chatbox = new InteractiveChatbox(chatButton, chatContent, icons);
chatbox.display();
//chatbox.toggleIcon(false, chatButton);

//$('#chatbox').css('top', $(document).scrollTop()+20)
/*$("#chatbox").animate({ scrollTop: $('#chatbox').prop("scrollHeight") }, 1000);*/


var Home = {

    InitPage() {
        Home.UC.SetBtn.Sidebar();
        Home.UC.SetChatRoom.SetBtnUserListClick();
        Home.CONNECTION.Init();
    },

    DATA: {
        GetPercent(num, total) {
            num = parseFloat(num);
            total = parseFloat(total);
            if (isNaN(num) || isNaN(total)) {
                return "-";
            }
            return total <= 0 ? '0' : (Math.round(num / total * 10000) / 100.00);
        }
    },

    UC: {
        SetBtn: {
            Sidebar() {
                $('.sidebar-button').click(function () {
                    $('.sidebar').toggleClass('active');
                    $('.sub-menu').css('display', 'none');
                });

                $('.icon-link').find('a').click(function (e) {
                    e.preventDefault();
                    $(this).parent('.icon-link').siblings('.sub-menu').slideToggle();
                });

                $('.sub-menu').find('a').click(function (e) {
                    e.preventDefault;

                    if ($(this).attr('data-controller').includes('Logout')) {
                        window.location.href = '/Manager';
                    }

                    if ($(this).attr('data-controller') != '') {

                        $.ajax({
                            url: $(this).attr('data-controller'),
                            success: res => {
                                if (res.includes('後台管理系統')) {
                                    window.location.href = '/Manager';
                                }
                                $('#app').html(res)
                            },
                            error: res => {
                                window.location.href = '/Manager';
                            }
                        })
                    }
                });

                $('#btnHome').click(function () {
                    //window.location.href = '/Manager/Home';
                    $('#app').html(`
                        <div id="Home-container" class="Home-container">
                            <div id="loading-section" class="loading">                                
                                <span class="loading__anim"></span>
                            </div>
                        </div>`)
                });
            },
        },

        SetChatRoom: {
            SetBtnUserListClick() {

                //$(document).on('click', '.users-list>a, #prevToChatList', function (e) {
                //    e.preventDefault();
                //    console.log(this)
                //    console.log($(this))
                //    //$('.wrapper').html(`
                //    //      `)
                //    $('#main-chat').toggle();
                //    $('#sub-chat').toggle();

                //    var $div = $('.chat-box');
                //    $div.scrollTop($div[0].scrollHeight);
                //});

                $('#prevToChatList').click(function (e) {
                    e.preventDefault();
                    //console.log(this);
                    //console.log($(this));

                    $('#main-chat').toggle();
                    $('#sub-chat').toggle();

                    var $div = $('#chat-box');
                    $div.scrollTop($div[0].scrollHeight);
                });

                //$(document).on('click', '#users-list>a', function (e) {
                //    e.preventDefault();
                //    console.log(this);
                //    console.log($(this));

                //    $('#main-chat').toggle();
                //    $('#sub-chat').toggle();

                //    var $div = $('#chat-box');
                //    $div.scrollTop($div[0].scrollHeight);
                //})

                $('#btnSendToChat').click(function (e) {
                    e.preventDefault();
                    var $chatBox = $('#chat-box');
                    var $btnSend = $(this);
                    var textContent = $btnSend.siblings('input').val();

                    if (textContent == '') return;

                    $btnSend.siblings('input').val('');

                    $chatBox.append(
                        `      <div class="chat outgoing">
                                    <div class="details">
                                        <p>${textContent}</p>
                                    </div>
                                </div>`)

                    $chatBox.animate({ scrollTop: $chatBox.prop("scrollHeight") }, 500);
                    $btnSend.siblings('input').focus();

                    Home.CONNECTION.SendMsg($(this).attr('data-id'), textContent);
                })
            },

            //點擊user進入對話框
            SwitchChat(Id) {
                //e.preventDefault();
                //console.log(this);
                //console.log($(this));

                //console.log(Id)

                //console.log($('#main-chat'))
                $('#main-chat').toggle();
                $('#sub-chat').toggle();
                $('#btnSendToChat').attr('data-id', Id);

                var $div = $('#chat-box');
                $div.scrollTop($div[0].scrollHeight);
            }
        }
    },

    CONNECTION: {
        Init() {

            

            connection.start().then(function () {
                console.log('--- connection start ---');
            }).catch(function (err) {
                console.error('連線失敗');
                $('#loading-section').html('<span>讀取資料出現異常，請聯繫網站管理員</span>');
                return console.error(err.toString());
            });

            //test
            connection.on('ReceiveMessageFromUser', function (user, message) {
                console.log(user + message);                
            })

            //庫存量警告
            connection.on('ReceiveMessage', function (user, message) {

                if ($('#Home-container').length > 0) {
                    var item = JSON.parse(message);

                    var itemHtml = '';

                    for (var keys in item) {
                        var percent = Home.DATA.GetPercent(item[keys].Stock, 20);
                        itemHtml += `
                                <div class="Home-item">
                                    <div class="item-textAlert">
                                        <span>庫存量不足警告</span>
                                    </div>
                                    <div class="item-content">
                                        <div class="item-section item-detail">
                                            <span>產品名稱</span>
                                            <span>${item[keys].Name}</span>
                                            <span>庫存量</span>
                                            <span>${item[keys].Stock}</span>
                                            <span>最低庫存量:20</span>
                                        </div>

                                        <div class="item-section">
                                            <div class="flex-wrapper">
                                                <div class="single-chart">
                                                    <svg viewBox="0 0 36 36" class="circular-chat orange">
                                                        <path class="circle-bg" d="M18 2.0845
                                                                      a 15.9155 15.9155 0 0 1 0 31.831
                                                                      a 15.9155 15.9155 0 0 1 0 -31.831"/>

                                                        <path class="circle" stroke-dasharray="${percent}, 100" d="M18 2.0845
                                                                      a 15.9155 15.9155 0 0 1 0 31.831
                                                                      a 15.9155 15.9155 0 0 1 0 -31.831"/>

                                                        <text x="18" y="20.35" class="percentage">${percent}%</text>
                                                    </svg>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>`;
                    }

                    $('#Home-container').html(itemHtml);
                }
            });

            //接收User回傳的訊息
            connection.on('ReceivePrivateFromUser', function (userId, msg) {
                //console.log(msg);
                console.log({ userId })
                if (!$('#chatbox-support').hasClass('chatbox--active')) {
                    //視窗關閉狀態
                    $('#chatbox-support').addClass('chatbox--active');
                    $('#main-chat').toggle();
                    $('#sub-chat').toggle();
                }
                else
                {
                    //視窗開啟狀態
                    
                    var $userTarget = $('#users-list>a').filter(function () {
                        return $(this).data('id') == userId
                    });

                    //console.log({userId})
                    //console.log({ $userTarget });
                    //console.log($userTarget.find('.user-incoming-msg').text(msg));
                    $userTarget.find('.user-incoming-msg').text(msg);
                }


                
                $('#btnSendToChat').attr('data-id', userId);

                //$('#sub-chat').show();
                $('#chat-box').append(`
                                <div class="chat incoming">
                                    <div class="user-img"></div>
                                    <div class="details">
                                        <p>${msg}</p>
                                    </div>
                                </div>`)

                var $div = $('#chat-box');
                $div.scrollTop($div[0].scrollHeight);
            })


            connection.on('GetConList', function (ConList) {

                console.table(ConList)
                var userName = localStorage.getItem('user');
                $('#chat-username').text(userName);

                var index = ConList.findIndex(x => x.userName == userName);              
                ConList.splice(index, 1);

                var userListHtml = '';

                for (var i = 0, len = ConList.length; i < len; i++) {
                    userListHtml +=
                        `      <a href="#" data-id="${ConList[i].connectionID}" onclick="Home.UC.SetChatRoom.SwitchChat('${ConList[i].connectionID}')">
                                    <div class="content">
                                        <div class="user-img">
                                            <span>${ConList[i].userName[0]}</span>
                                        </div>
                                        <div class="details">
                                            <span>${ConList[i].userName}</span>
                                            <p class="user-incoming-msg"></p>
                                        </div>
                                    </div>
                                    <div class="status-dot"><i class="fas fa-circle"></i></div>
                                </a>`
                }

                $('#users-list').html(userListHtml);
            })



            connection.onclose(function (e) {
                console.log('連線已中斷');
            });

            //connection.stop().then(function () {
            //    console.log('連線停止');
            //}).catch(function (err) {
            //    console.error('連線停止時發生錯誤');
            //    console.error(err.toString());
            //})
        },

        SendMsg(userId, msg) {

            if (msg == '') return;

            connection.invoke('SendPrivateMessage', userId, msg).catch(function (err) {
                console.error('send msg error');
            })
        }
    }
}
