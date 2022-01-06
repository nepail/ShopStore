let sidebar = document.querySelector(".sidebar");
let sidebarBtn = document.querySelector(".sidebar-button");

sidebarBtn.onclick = () => {
    sidebar.classList.toggle("active");
    $('.sub-menu').css('display', 'none');
}


$(document).ready(function() {
    //Main menu
    $('.sidebar>.nav-links>li>.icon-link>a').click(function(event) {
            event.preventDefault();
            $(this).parent('.icon-link').siblings('.sub-menu').slideToggle();

        })
        //Sub menu
    $('.sidebar>.nav-links>li>.sub-menu>li>a').click(function(event) {
        event.preventDefault();
        // console.log($(this).attr('data-controller'))

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


            //$('#app').load($(this).attr('data-controller'), function (responseTxt, statusTxt, xhr) {

            //    console.log({ responseTxt })
            //    console.log({ statusTxt })
            //    console.log({ xhr })

            //    if (responseTxt.includes('後台管理系統')) {
            //        alert('閒置過久，您已被登出')
            //        window.location.href = '/Manager';
            //    }
            //})
        }

        //加入逾時重定向處理


        // if ($(this).attr('data-controller').indexOf('Member') > 0) {
        //     $('.sidebar>.nav-links>li>.sub-menu').append(`<li><a href="#" onclick="fn.addMenu(this)">➕</a></li>`)

        // }
    })




    // $('.sub-menu li a').each(function(index) {
    //     $(this).on('click', () => {
    //         alert('666')
    //     })
    // })


    // //新增商品
    // $('.sub-menu:eq(0)>li:eq(0) a').click(() => {
    //     $('#app').load('Manager/AddNewProducts')

    // });

    // //商品清單
    // $('.sub-menu:eq(0)>li:eq(1) a').click(() => {
    //     $('#app').load('Manager/ProductList');
    // });

    // //會員查詢
    // $('.sub-menu:eq(1)>li:eq(0) a').click(() => {
    //     $('#app').load('Manager/MemberQuery');
    // });

    // //權限設定
    // $('.sub-menu:eq(1)>li:eq(1) a').click(() => {
    //     $('#app').load('Manager/MemberPermissionSetting');
    // });

    // //等級設定
    // $('.sub-menu:eq(1)>li:eq(2) a').click(() => {
    //     $('#app').load('Manager/MemberLevelSetting');
    // });

    // //訂單查詢
    // $('.sub-menu:eq(2)>li:eq(0) a').click(() => {
    //     alert('訂單查詢')
    // });

    // $('.sub-menu:eq(2)>li:eq(1) a').click(() => {
    //     alert('所有訂單')
    // });

    // $('.sub-menu:eq(3)>li:eq(0) a').click(() => {
    //     alert('菜單設定')
    // });

    // $.ajax({
    //     async: true,
    //     type: 'get',
    //     url: '/Menu/GetMenu',
    //     success: (res) => {
    //         console.log(res)
    //     },
    //     error: (res) => {
    //         console.error('取得菜單錯誤')
    //     }
    // })



})




var baseInstance = axios.create({
    baseURL: window.location.origin
})


new Vue({
    el: '#app',
    data: {
        items: null,
        form: {
            email: '',
            name: ''
        },
        show: true
    },
    mounted() {
        baseInstance.get('/SampleData').then(response => {
            this.items = response.data;
        });
    },

    methods: {
        onSubmit(evt) {
            evt.preventDefault();
            baseInstance.post('/PersonData', this.form)
                .then(response => {
                    console.log(response);
                    this.reset();
                })
                .catch(error => {
                    console.log(error)
                })
        },
        onReset(evt) {
            evt.preventDefault();
            this.reset();
        },
        reset() {
            this.form.email = '';
            this.form.name = '';

            this.show = false;
            this.$nextTick(() => {
                this.show = true;
            })
        }
    }
})