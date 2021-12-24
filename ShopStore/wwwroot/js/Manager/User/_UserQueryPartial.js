$(document).ready(function () {
    User.DATA.GetUsers();
    User.InitMember();
})


var postData = {
    f_account: '',
    f_pcode: '',
    f_groupId: 0,
    f_name: ''
}

var User = {

    InitMember: function () {
        $('#btnAddUser').click(function () {
            User.UC.ResetForm();
        });


        $('#submit').click(function (e) {
            e.preventDefault();
            if (!$('#rgForm').valid()) return;


            postData = {
                f_account: $('#rgAccount').val(),
                f_pcode: $('#rgpCode').val(),
                f_groupId: $('#rgSelect :selected').val(),
                f_name: $('#rgName').val()
            }

            console.log(postData);

            User.DATA.AddUser(postData);
        });

        $('#rgForm').validate({
            rules: {
                name: {
                    rangelength: [3, 5],
                    required: true,
                },

                account: {
                    rangelength: [5, 10],
                    required: true,
                },

                password: {
                    required: true,
                    minlength: 5
                },
                password_confirm: {
                    required: true,
                    minlength: 5,
                    equalTo: "#rgpCode"
                },

                messages: {
                    password: {
                        required: '請輸入密碼',
                        minlength: '不得小於5字元'
                    }
                }
            }
        });

        $(document).on('click', '.box-content', function () {
            if ($('#containerR').is(':visible')) {
                $('#containerR').toggle()
            }

            if (!$('#containerP').is(':visible')) {
                $('#containerP').toggle()
            }

            //console.log($(this).children().next().children('p').eq(0).text())
            //console.log($(this).attr('data-id'))

            var id = $(this).attr('data-id');
            var u = $.grep(MainProperties.User.data, function (e) {
                return e.id == id
            })[0]

            $('#smWord').text(u.name[0])
            $('#userName').text(u.name);
            $('#userAccount').text(u.account);
            $('#userId').text(u.id);
            $('#userGName').text(u.groupName);
            $('#userName').text(u.name);
            $('#userCreatTime').text('建立時間：' + u.createTime);
            $('#userUpdateTime').text('修改時間：' + u.updateTime);
        })

        User.UC.SetSearch();
    },

    DATA: {
        GetUsers: function () {
            $.ajax({
                url: '/Manager/GetUsers',
                type: 'get',
                success: (res) => {
                    if (res.success) {
                        MainProperties.User.data = res.item;
                        User.UC.SetUserList();
                        //swal('新增成功', ' ', 'success');                        
                    } else {
                        swal('載入失敗', '資料庫出現錯誤', 'error');
                    }
                },
                error: (res) => {
                    swal('載入失敗', '網路出現錯誤', 'error');
                }
            })
        },

        AddUser: function (data) {
            $.ajax({
                url: '/Manager/AddUser',
                type: 'post',
                data: data,
                success: (res) => {
                    if (res.success) {
                        swal('新增成功', ' ', 'success');
                        User.UC.ResetForm();
                    } else {
                        swal('新增失敗', '資料庫出現錯誤', 'error');
                    }
                },
                error: (res) => {
                    swal('新增失敗', '網路出現錯誤', 'error');
                }
            })
        },

        UpdateUser: function () {

        },
    },

    UC: {
        SetUserList: function () {

            var userContent = '';

            for (var i = 0, len = MainProperties.User.data.length; i < len; i++) {
                var r = MainProperties.User.data[i];
                userContent += `
                    <div class="box box-content" data-id="${r.id}">
                        <div class="box-section">
                            <div class="box-img">${r.name[0]}</div>
                        </div>
                        <div class="box-section tx">
                            ${r.name}
                            <p>${r.account}</p>
                            <p>ID : ${r.id}</p>
                        </div>
                    </div>`
            };

            $('#userList').html(userContent);
        },

        /**
         * 搜尋功能
         */
        SetSearch: function () {
            $('#userSearch').on('keyup', function () {
                var value = $(this).val().toLowerCase();
                $('#userList>.box-content').filter(function () {
                    $(this).toggle($(this).find('.tx').text().toLowerCase().indexOf(value) > -1);
                })
            })
        },

        ResetForm: function () {
            if ($('#containerP').is(':visible')) {
                $('#containerP').toggle()
            }
            $('#containerR').toggle();
            $('form')[0].reset();
        }
    }
}



$.extend($.validator.messages, {
    required: "這是必填項目",
    remote: "請修正此項目",
    email: "請輸入有效的電子郵件地址",
    url: "請輸入有效的網址",
    date: "請輸入有效的日期",
    dateISO: "請輸入有效的日期 (YYYY-MM-DD)",
    number: "請輸入有效的數字",
    digits: "只能輸入數字",
    creditcard: "請輸入有效的信用卡號碼",
    equalTo: "請輸入相同的密碼",
    extension: "請輸入有效的後綴",
    maxlength: $.validator.format("最多可以輸入 {0} 個字元"),
    minlength: $.validator.format("最少要輸入 {0} 個字元"),
    rangelength: $.validator.format("請輸入長度在 {0} 到 {1} 之間的字元"),
    range: $.validator.format("請輸入範圍在 {0} 到 {1} 之間的數值"),
    max: $.validator.format("請輸入不大於 {0} 的數值"),
    min: $.validator.format("請輸入不小於 {0} 的數值")
});