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
                return console.error(err.toString());
            });

            connection.on('ReceiveMessage', function (user, message) {
                $('#messagesList').html(`${user} ${message}`);
            });
        }
    }
}
