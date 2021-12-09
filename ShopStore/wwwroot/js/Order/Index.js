function showOrderDetail(itemid) {
    $("#" + itemid).slideToggle("slow");
}

function cancelOrder(ordernum) {
    
    swal({
        title: "確定取消嗎?",
        //text: "可能無法退款",
        type: "warning",
        showCancelButton: true,
        confirmButtonText: "確定",
        cancelButtonText: "取消",
        closeOnConfirm: false,
        showLoaderOnConfirm: true,
    },
        function (isConfirm) {
            if (isConfirm) { 
                setTimeout(function () {
                    $.ajax({
                        url: "/Order/CancelOrder/?ordernum=" + ordernum,
                        type: "get",                        
                        success: (res) => {
                            swal("取消", "編號 #" + ordernum + " 訂單已取消", "success");
                            $("#tb_" + ordernum).remove();
                            emptyOrder();
                        },
                        error: (res) => {
                            alert('fail')
                        }
                    });
                }, 1000);                
            }     
        });
}

function emptyOrder() {
    if ($("#orderList tbody[id]").length == 0) {
        $('#orderList').append('<tbody><tr><td align="center" colspan="7">您的訂單目前是空的！</td></tr></tbody>');
    }

    //table-bordered table-striped
}

function showProductDetail(itemname, itemid) {    
    $("#orderTable").toggle("normal");
    $("#prodetail").toggle("normal");
    if (itemid != undefined) {
        $("#prodetailConent").load("/Products/GetProductDetailById/?id=" + itemid, function () {            
        });
    }
}