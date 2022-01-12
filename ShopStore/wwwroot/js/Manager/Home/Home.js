$(document).ready(function () {
    Home.InitPage();    
})

var connection = new signalR.HubConnectionBuilder().withUrl('/chatHub').build();;

var Home = {

    InitPage() {
        Home.UC.SetBtn.Sidebar();
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
                    window.location.href = '/Manager/Home';
                });
            },
        }
    },

    CONNECTION: {
        Init() {
            connection.start().then(function () {
                console.log('--- connection start ---');
            }).catch(function (err) {
                console.error('連線失敗');
                return console.error(err.toString());
            });

            connection.on('ReceiveMessage', function (user, message) {
                //$('#messagesList').html(`${user} ${message}`);

                console.log(message);                

                //return;
                if ($('#Home-container').length > 0) {
                    var item = JSON.parse(message);
                    console.log(item);
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

            connection.onclose(function (e) {
                console.log('連線已中斷');
            });

            connection.stop().then(function () {
                console.log('連線停止');
            }).catch(function (err) {
                console.error('連線停止時發生錯誤');
                console.error(err.toString());
            })
        }
    }
}
