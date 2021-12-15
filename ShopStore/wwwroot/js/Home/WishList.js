$(document).ready(function () {
    Wish.GetProductList()
    Wish.GetWishList()

    $('.btnRemoveWish').on('click', function () {
        var row = $(this).parent().parent()

        Wish.RemoveWishItem(row.attr('data-id'))
        //row.remove()
    })

    $('#btnAddtoCart').on('click', function () {
        var addtoCartList = $(this).parent().parent().parent().find('input:checkbox:checked')

        if (addtoCartList.length > 0) {

            var postdata = []

            addtoCartList.each(function () {
                var item = $(this).parent().parent()
                var itemid = $(this).parent().parent().attr('data-id')
                Wish.RemoveWishItem(item)
                postdata.push(itemid)
            })

            Wish.SendData(postdata)

        } else {
            swal('請先勾選', '', 'error')
        }
    })

    $('#checkAll').click(function () {
        $('input:checkbox').not(this).prop('checked', this.checked)
    })
})

var user = localStorage['user']
var productList
var wishList

var Wish = {
    //取所有產品
    GetProductList: function () {
        productList = JSON.parse(localStorage['item'])
    },
    //取願望清單
    GetWishList: function () {
        try {
            wishList = JSON.parse(localStorage[user])
            Wish.CheckWishList()
            if (wishList.item.length > 0) {
                $.each(wishList.item, function (i, v) {

                    var p = $.grep(productList.item, function (e) {
                        return e.f_id == v
                    })

                    $('#WishTable')
                        .append(`<tr data-id="${p[0].f_id}">
                                    <td class="align-middle" align="center"><input type="checkbox"/></td>
                                    <td align="center"> <img src="/images/${p[0].f_picPath}"></td>
                                    <td class="align-middle"> ${p[0].f_name} </td>
                                    <td class="align-middle" align="center">NT$ ${p[0].f_price} </td>
                                    <td class="align-middle" align="center"> ${p[0].categoryName} </td>
                                    <td class="align-middle" align="center"><input type="button" class="btn btn-sm btn-outline-danger btnRemoveWish" value="刪除"></td>
                            </tr>`)
                })
                $('#WishTable').append(`<tr align="center"><td colspan="6"><input id="btnAddtoCart" type="button" class="btn btn-outline-primary" value="加入購物車"/></td></tr>`)
            }

        } catch {
            Wish.ShowEmptyMsg()
        }
    },

    //移除願望項目
    //傳入item object
    RemoveWishItem: function (item) {

        $(item).remove()
        var itemid = item.attr('data-id')

        wishList.item = $.grep(wishList.item, function (e) {
            return e != itemid
        })

        localStorage[user] = JSON.stringify(wishList)
        Wish.CheckWishList()
    },

    //顯示清單訊息
    ShowEmptyMsg: () =>
        $('#WishTable')
            .append(`<tr>
                         <td align="center" colspan="6">您的清單目前是空的！</td>
                     </tr>`)
    ,

    //檢查WishLsit
    CheckWishList: () => {
        if (wishList.item.length == 0) {
            Wish.ShowEmptyMsg()
            $('#btnAddtoCart').remove()
        }
    },

    //傳送資料
    SendData: function (list) {

        console.log(list)

        var postData = {
            list: list
        }

        $.ajax({
            url: '/Cart/AddListToCart',
            type: 'post',
            data: postData,
            success: function(res) {
                if (res.success) {
                    swal('新增購物車成功', '', 'success')

                    if ($('#cartItemCount').length > 0) {
                        let cartNum = $('#cartItemCount').text();
                        cartNum++;
                        $('#cartItemCount').text(cartNum);
                    } else {
                        $('#myCart').append(`<span id='cartItemCount' class='badge badge-info'>${res.addedItem}</span>`);
                        $('#mymenu').attr('class', 'dropdown-menu show');
                    }

                } else {

                }
            },
            error: function(res) {
                swal(res.message, '', 'error')
            }
        })
    }
}
