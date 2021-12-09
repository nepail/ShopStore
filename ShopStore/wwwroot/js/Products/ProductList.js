//window.onload = function () {
//    console.log("getlist");
//    GetProductList(1);
//}

//const productData = $.get('/Products/ProductLists');
var productData;

$(document).ready(function () {
    console.log(productData);
    
    //Product.SetEvent();
});

$(window).on('load',function () {
    //console.log($('#productList').attr('data-ajax-sign'))
    getproduct();
});


function getproduct() {

    var md5 = JSON.parse(localStorage.getItem('item')).ajaxsign;
    console.log(md5)

    $.ajax({
        async: false,
        type: 'GET',
        dataType: 'text',
        url: '/Products/ProductLists?md5string='+md5,
        success: function (result) {
            console.log(result)
            var data = JSON.parse(result);            
            if (data.success) {
                
                productData = data;
                localStorage.setItem('item', JSON.stringify(productData))
                console.log('%c MD5檢查碼不同，成功更新 localStorage', 'color:orange; font-size:20px;');
                console.table(productData.item);

            } else {
                productData = JSON.parse(localStorage.getItem('item'));
                console.error('檢查碼相同，從localStorage 取出')
                console.table(productData.item);
            }            
        },
        error: function () {
            alert("error");
        }
    })
}

function showProductDetail(itemName, itemid) {

    var product = $.grep(productData.item, (e, i) => {
        return e.f_id == itemid
    })

    var p = product[0]


    if (itemName != undefined) {
        $("#navItemName").text(itemName);
    }

    $("#productList").toggle('normal');
    $("#subNav").toggle('normal');
    $("#productDetail").toggle('normal');

    if (itemid != undefined) {
        $("#prodetailCard-content").html(p.f_content)
        $("#prodetailCard-img").attr("src", "/images/" + p.f_picPath)
        $("#prodetailCard-title b").text(p.f_name)
        $("#prodetailCard-inline p.l-grey span:eq(0)").text(p.f_categoryId)
        $("#prodetailCard-inline p.l-grey span:eq(1)").text(p.f_price)
    }

    //$.ajax({
    //    url: "/Products/GetProductDetailById/?id=" + itemName,
    //    method: "GET",

    //    success: res => {
    //        console.log(res);
    //    }
    //});
}

function AddtoCart(item, itemname) {
    let itemid = $(item).attr("data-id");
    console.log(itemid);
    let data = new FormData();
    data.append("id", itemid);

    if ($("#cartItemCount").length > 0) {
        let cartNum = $("#cartItemCount").text();
        cartNum++;
        $("#cartItemCount").text(cartNum);
    } else {
        $("#myCart").append('<span id="cartItemCount" class="badge badge-info">1</span>');
        $("#mymenu").attr("class", "dropdown-menu show");
    }

    $.ajax({
        async: true,
        type: "POST",
        contentType: false,
        processData: false,
        data: data,
        url: "/Cart/AddtoCart",
        success: function (result) {
            toastr.success("已加入購物車", itemname);
        },
        error: function () {
            alert("error");
        }
    });
}

function searchProducts() {
    let input, filter, table, card, h1, i, txtValue;
    input = $("#searchInput");
    filter = input.val().toUpperCase();
    table = $("#productList");
    card = $(".card");

    for (i = 0; i < card.length; i++) {
        h1 = card[i].querySelector("h1");
        if (card[i]) {
            txtValue = h1.innerText;
            if (txtValue.indexOf(filter) > -1) {
                card[i].style.display = "";
            } else {
                card[i].style.display = "none";
            }
        }
    }
}

function sortChange() {
    let selectedType = $("#drpSortList").val();
    let aDiv = $(".card");
    let arr = [];
    for (let i = 0; i < aDiv.length; i++) {
        arr.push(aDiv[i]);
    }
    arr.sort(function (a, b) {
        switch (selectedType) {
            case "1": //名稱
                a = a.querySelector("h1").innerText.substr(0, 1);
                b = b.querySelector("h1").innerText.substr(0, 1);
                break;
            case "2": //價格
                a = parseInt(a.querySelector("td.pl-md").innerText);
                b = parseInt(b.querySelector("td.pl-md").innerText);
                break;
            case "3": //熱門
                break;
        }

        if (a > b) return 1;
        if (a < b) return -1;
        return 0;
    });
    for (let i = 0; i < arr.length; i++) {
        $("#productList").append(arr[i]);
    }
}
