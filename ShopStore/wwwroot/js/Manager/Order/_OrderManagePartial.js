$(document).ready(function () {
    Order.GetOrderList()
})

$(document).off('click').on('click', '#btnReturn', function () {
    Order.ReturnOfgood($(this).parent().siblings('.bcol:eq(1)').text())
})

$(document).off('click').on('click', '.size', function () {
    $('.size').styleddropdown();
})

$('#btnSave').click(function () {
    Order.SendData()
})


var orderList
var postData = []
var upitemnum = -1

var STATUS = {
    tbd: 1,
    shipped: 2,
    transport: 3,
    returned: 4,
    chanceled: 5,
    abnormal: 6
}

var SipMethod = {
    postm: 1,
    B2B: 2,
    privateCargo: 3
}

var Order = {
    GetOrderList: function () {
        $.ajax({
            url: '/Manager/GetOrderList',
            type: 'get',
            success: function (res) {
                if (res.success) {
                    orderList = res.result
                    Order.InitOrderList()
                } else {

                }
            },
            error: function () {

            }
        })
    },

    InitOrderList: function () {
        $.each(orderList, function (i, v) {
            var btnSwitch = v.isDel == 1 ? 'style="visibility: hidden;"' : ''
            $('#orderList')
                .append(`
                    <div id="ordernum_${v.num}" class="box brow">
                        <div class="box bcol rowckbox">
                            <input type="checkbox" class="visibility"/>
                        </div>
                        <div class="box bcol"><span ordernum="${i}" >${v.num}</span></div>
                        <div class="box bcol"><span>${v.date}</span></div>
                        <div class="box bcol"><span>${v.memberName}</span></div>
                        <div class="box bcol">
                            <div class="size">
                                <span class="bbadge field bg-${v.statusBadge}">${v.status}</span>
                                    <ul menu-type="0" class="list">
                                        <li data-type="1">待確認</li>
                                        <li data-type="2">已出貨</li>
                                        <li data-type="3">運輸中</li>
                                        <li data-type="4">已退貨</li>
                                        <li data-type="5">已取消</li>
                                        <li data-type="6">貨物異常</li>
                                    </ul>
                            </div>
                        </div>
                        <div class="box bcol">
                            <div class="size">
                                <span class="bbadge field ${v.shippingBadge}">${v.shippingMethod}</span>
                                <ul menu-type="1" class="list">
                                    <li data-type="1">郵寄</li>
                                    <li data-type="2">店到店</li>
                                    <li data-type="3">私人集運</li>
                                </ul>
                            </div>
                        </div>
                        <div class="box bcol"><span>NT$ ${v.total}</span></div>
                        <div class="box bcol rowckbox">
                            <input id="btnReturn" type="button" class="btn btn-sm btn-outline-danger" value="退貨" ${btnSwitch}/>
                        </div>
                    </div>
                `)
        })
    },

    ReturnOfgood: function (ordernum) {
        swal({
            title: '確定執行此操作?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            closeOnConfirm: false,
            showLoaderOnConfirm: true,
        },
            function (isConfirm) {
                if (isConfirm) {
                    setTimeout(function () {
                        $.ajax({
                            url: '/Manager/RemoveOrder?id=' + ordernum,
                            type: 'get',
                            success: (res) => {
                                if (res.success) {
                                    swal('執行成功', '訂單' + ordernum + '已退貨', 'success')

                                    var targetItem = $('#ordernum_' + ordernum).find('.bcol:eq(4)>span')
                                    var targetClass = $('#ordernum_' + ordernum).find('.bcol:eq(4)>span').attr('class').split(' ')[1]
                                    targetItem.toggleClass(`${targetClass} bg-danger`)
                                    targetItem.text('已退貨')
                                    $('#ordernum_' + ordernum).find('input[type="button"]').css('visibility', 'hidden')
                                } else {
                                    swal('執行失敗', '資料庫錯誤', 'error')
                                }
                            },
                            error: (res) => {
                                swal('執行失敗', '伺服器錯誤', 'error')
                            }
                        })
                    })
                }
            }
        )
    },

    GetStatus: function (statusCode, type, ordernum, orderid) {

        if (postData[ordernum] == undefined) {
            postData[ordernum] = {
                f_id: '',
                f_status: NaN,
                f_ShippingMethod: NaN
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

            postData[ordernum].f_id = orderid
            postData[ordernum].f_status = statusCode
            console.table(postData)
            return statement[statusCode]
        }

        if (type == 1) {
            const shippingMethod = {
                [SipMethod.postm]: 'bgreen',
                [SipMethod.B2B]: 'bgyellow',
                [SipMethod.privateCargo]: 'bgblue'
            }

            postData[ordernum].f_id = orderid
            postData[ordernum].f_ShippingMethod = statusCode
            console.table(postData)
            return shippingMethod[statusCode]
        }

    },

    SendData: function () {
        alert('send Data ~')

    }
}

$.fn.styleddropdown = function () {
    return this.each(function () {
        var obj = $(this).off('click')
        obj.find('.field').off('click').click(function () { //onclick event, 'list' fadein            
            obj.find('.list').fadeIn(400);

            $(document).off('keyup').keyup(function (event) { //keypress event, fadeout on 'escape'                
                if (event.keyCode == 27) {
                    obj.find('.list').fadeOut(400);
                }
            });

            obj.find('.list').hover(function () { },
                function () {
                    $(this).fadeOut(400);
                });
        });

        obj.find('.list li').off('click').click(function () { //onclick event, change field value with selected 'list' item and fadeout 'list'

            var type = $(this).attr('data-type')
            var menuType = $(this).parent().attr('menu-type')
            var orderid = obj.find('.field').parent().parent().siblings().eq(1).find('span').text()
            var ordernum = obj.find('.field').parent().parent().siblings().eq(1).find('span').attr('ordernum')
            var targetClass = obj.find('.field').attr('class').split(' ')[2]
            obj.find('.field')
                .text($(this).html())
                //.css({
                //    'background': 'black',
                //})
                .toggleClass(`${targetClass} ${Order.GetStatus(type, menuType, ordernum, orderid)}`);
            console.log(orderid)

            obj.find('.list').fadeOut(400);
        });
    });
};