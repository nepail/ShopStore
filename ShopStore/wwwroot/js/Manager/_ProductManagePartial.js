//var productData = $.get('/Products/ProductLists');

//const product = {
//    listData : [
//        {
//            'id': '',
//            'desc': ''
//        }],

//    aaa() {
//        alert('Hello')
//    },

//    //SetEvent = function () {

//    //},
//};



$(function () {
    productData = fn.getProdata();
})

$(function () {
    $.each(productData, (i, v) => {
        $('tbody').append(
            `<tr data-id="${v.f_id}" onclick="fn.show('${v.f_id}')">                
                <td>${v.f_name}</td>
                <td>${v.f_price}</td>
                <td>${v.f_categoryId}</td>
                <td>${v.f_stock}</td>
                <td>開放中</td>
                <td>${v.createTime}</td>
             </tr>`)
    })
})


fn = {
    //顯示產品content
    show: (v) => {
        var product = $.grep(productData, (e, i) => {
            return e.f_id == v
        })

        var p = product[0]
        $('#showP-content').html(p.f_content)
    },
    getProdata: () => {
        var data;
        $.ajax({
            async: false,
            type: 'get',
            url: '/Products/ProductLists',
            success: (res) => {
                if (res.success) {
                    data = res.item;
                } else {
                    swal(res.message, "", "error")
                }
            },
            error: (res) => {
                swal(res.message, "", "error")
            }
        });

        return data
    }

}


//$(document).ready(function () {
//    $('.editable').inlineEdit({
//        defaultText: true,
//        maxLength: 10,
//        onFocus: function (val) {
//            console.log(val)
//        },
//        onUpdate: function (val) {
//            console.log(val)
//        }
//    });
//});

//$('#defaultText').inlineEdit({
//    defaultText: true,
//    connectWith: '#defaultTextTarget',
//    onFocus: function (val) {
//        console.log("You are inside me LOL!");
//        console.log(val);
//    },
//    onUpdate: function (val) {
//        console.log("You almost done");
//        console.log(val);
//    }
//});


//$('#limitText').inlineEdit({
//    showCounter: true,
//    maxLength: 100,
//    defaultText: true,
//    inputType: 'input'
//});