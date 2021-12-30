$(document).ready(function() {
    Member.GetMemberList();
    //Order.SetBtnSave();
    //Order.SetDropDownList();
})

var postData = []

var Member = {
    /**
     * 取得所有Order資料
     */
    GetMemberList: function() {
        $.ajax({
            url: '/Manager/GetMemberList',
            type: 'get',
            success: function(res) {
                if (res.success) {
                    MainProperties.Member.data = res.result;
                    Member.InitMemberList();
                } else {
                    swal('系統錯誤', '資料庫錯誤', 'error');
                }
            },
            error: function() {
                swal('網路錯誤', '無法連上伺服器', 'error');
            }
        })
    },

    Suspend: function(item) {
        //Order.ReturnOfgood($(item).parent().siblings('.bcol:eq(1)').text());
        alert('會員停權設定')
    },
    /**
     * 渲染Order列表
     */
    InitMemberList: function() {

        var listContent = `
            <!--Title部分-->
            <div class="box brow boxtitle">
                <div class="box bcol rowckbox">
                    <input type="checkbox" class="visibility" />
                </div>
                <div class="box bcol">會員編號</div>
                <div class="box bcol">名稱</div>
                <div class="box bcol">帳號</div>
                <div class="box bcol">等級</div>
                <div class="box bcol">購物金</div>
                <div class="box bcol">是否停權</div>
                <div class="box bcol rowckbox">
                    <input type="button" class="btn btn-sm btn-outline-danger" value="停權" style="visibility: hidden" />
                    <i class="bx bx-search"></i>
                    <input id="searchInput" type="search" placeholder="Search..." />
                </div>
            </div>`;

        for (var i = 0, len = MainProperties.Member.data.length; i < len; i++) {

            //var btnSwitch = MainProperties.Member.data[i].isDel == 1 ? 'style="visibility: hidden;"' : '';
            var m = MainProperties.Member.data[i];

            listContent += `
                            <div id="member" class="box brow order">
                                <div class="box bcol rowckbox">
                                    <input type="checkbox" class="visibility" />
                                </div>
                                <div class="box bcol tx"><span>${m.id}</span></div>
                                <div class="box bcol tx"><span>${m.name}</span></div>
                                <div class="box bcol tx"><span>${m.account}</span></div>
                                <div class="box bcol">
                                    <div class="size">
                                        <span class="bbadge field">LV ${m.level}</span>
                                        <ul menu-type="0" class="list">
                                            <li data-type="1">LV 1</li>
                                            <li data-type="2">LV 2</li>
                                            <li data-type="3">LV 3</li>
                                            <li data-type="4">LV 4</li>
                                            <li data-type="5">LV 5</li>
                                            <li data-type="6">LV 6</li>
                                        </ul>
                                    </div>
                                </div>
                                <div class="box bcol">NT$<span>${m.money}</span></div>
                                <div class="box bcol tx"><span>${m.isSuspend == 1?'是':'否'}</span></div>
                                <div class="box bcol rowckbox">
                                    <input id="btnSuspend" type="button" class="btn btn-sm btn-outline-danger" value="停權" onclick="Member.Suspend(this)"/>
                                </div>
                            </div>`;
        }

        $('#memberList').html(listContent);
        Member.SetSearch();
    },

    /**
     * 退貨功能
     * @param {Number} ordernum 
     */
    ReturnOfgood: function(ordernum) {
        swal({
                title: '確定執行此操作?',
                type: 'warning',
                showCancelButton: true,
                confirmButtonText: '確定',
                cancelButtonText: '取消',
                closeOnConfirm: false,
                showLoaderOnConfirm: true,
            },
            function(isConfirm) {
                if (isConfirm) {
                    setTimeout(function() {
                        $.ajax({
                            url: '/Manager/RemoveOrder?id=' + ordernum,
                            type: 'get',
                            success: (res) => {
                                if (res.success) {
                                    swal('執行成功', '訂單' + ordernum + '已退貨', 'success');

                                    var targetItem = $('#ordernum_' + ordernum).find('.bcol:eq(4) span');
                                    var targetClass = $('#ordernum_' + ordernum).find('.bcol:eq(4) span').attr('class').split(' ')[2];
                                    targetItem.toggleClass(`${targetClass} bg-danger`);
                                    targetItem.text('已退貨');
                                    $('#ordernum_' + ordernum).find('input[type="button"]').css('visibility', 'hidden');
                                } else {
                                    swal('執行失敗', '資料庫錯誤', 'error');
                                }
                            },
                            error: (res) => {
                                swal('執行失敗', '伺服器錯誤', 'error');
                            }
                        })
                    })
                }
            }
        )
    },

    /**
     * 狀態變更功能
     * @param {Number} statusCode f_status 的狀態
     * @param {Number} type Menu 的類型
     * @param {Number} ordernum f_id
     * @param {Number} orderid 
     * @returns 狀態的的 CSS Style
     */
    GetStatus: function(statusCode, type, ordernum, orderid) {

        var STATUS = MainProperties.Order.STATUS;
        var SipMethod = MainProperties.Order.SipMethod;

        if (postData[ordernum] == undefined) {
            postData[ordernum] = {
                f_id: '',
                f_status: 0,
                f_ShippingMethod: 0
            }
        }

        if (type == 0) {
            const statement = {
                [STATUS.tbd]: 'bg-info',
                [STATUS.shipped]: 'bg-primary',
                [STATUS.transport]: 'bg-success',
                [STATUS.returned]: 'bg-danger',
                [STATUS.chanceled]: 'bg-secondary',
                [STATUS.abnormal]: 'bg-warning'
            }

            postData[ordernum].f_id = orderid;
            postData[ordernum].f_status = statusCode;
            console.table(postData);
            return statement[statusCode];
        }

        if (type == 1) {
            const shippingMethod = {
                [SipMethod.postm]: 'bgreen',
                [SipMethod.B2B]: 'bgyellow',
                [SipMethod.privateCargo]: 'bgblue'
            }

            postData[ordernum].f_id = orderid;
            postData[ordernum].f_ShippingMethod = statusCode;
            console.table(postData);
            return shippingMethod[statusCode];
        }
    },

    /**
     * 搜尋功能
     */
    SetSearch: function() {
        $('#searchInput').on('keyup', function() {
            var value = $(this).val().toLowerCase();
            $('#orderList>.order').filter(function() {
                //console.log($(this).find('.tx, .field').text())
                $(this).toggle($(this).find('.tx, .field').text().toLowerCase().indexOf(value) > -1);
            })
        })
    },

    /**
     * 保存功能
     */
    SetBtnSave: function() {
        $('#btnSave').click(function() {
            Order.SendData();
        })
    },

    /**
     * 更新訂單資料
     * @returns 成功與否
     */
    SendData: function() {

        if (postData.length == 0) {
            swal('沒有要變更的資料，請先選擇', ' ', 'info');
            return;
        }

        postData = postData.filter(el => el);

        var data = {
            orders: postData
        }

        $.ajax({
            url: '/Manager/UpdateMember',
            type: 'post',
            data: data,
            success: (res) => {
                if (res.success) {
                    swal('保存成功', ' ', 'success');
                }
            },
            error: (res) => {
                console.log('error');
            }
        })
    },

    /**
     * 狀態清單下拉
     */
    SetDropDownList: function() {
        $(document).off('click').on('click', '.size', function() {
            $('.size').styleddropdown();
        })
    }
}

$.fn.styleddropdown = function() {
    return this.each(function() {
        var obj = $(this).off('click');
        obj.find('.field').off('click').click(function() { //onclick event, 'list' fadein            
            obj.find('.list').fadeIn(400);

            $(document).off('keyup').keyup(function(event) { //keypress event, fadeout on 'escape'                
                if (event.keyCode == 27) {
                    obj.find('.list').fadeOut(400);
                }
            });

            obj.find('.list').hover(function() {},
                function() {
                    $(this).fadeOut(400);
                });
        });

        obj.find('.list li').off('click').click(function() { //onclick event, change field value with selected 'list' item and fadeout 'list'

            var type = $(this).attr('data-type');
            var menuType = $(this).parent().attr('menu-type');
            var orderid = obj.find('.field').parent().parent().siblings().eq(1).find('span').text();
            var ordernum = obj.find('.field').parent().parent().siblings().eq(1).find('span').attr('ordernum');
            var targetClass = obj.find('.field').attr('class').split(' ')[2];
            obj.find('.field')
                .text($(this).html())
                //.css({
                //    'background': 'black',
                //})
                .toggleClass(`${targetClass} ${Order.GetStatus(type, menuType, ordernum, orderid)}`);
            console.log(orderid);

            obj.find('.list').fadeOut(400);
        });
    });
};