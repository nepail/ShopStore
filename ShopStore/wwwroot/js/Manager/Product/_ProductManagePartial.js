var pdEditor;
var categoryList;
var productData

$(document).ready(function () {    
    fn.GetProdata()
    fn.SetSearch()
    fn.SetbtnSave()
    fn.SetImgUpload()
    fn.SetEditor()
    fn.GetCategoryList()
})

fn = {
    //顯示產品內容
    ShowProduct: (v) => {
        var product = $.grep(productData, (e, i) => {
            return e.f_pId == v
        })
        var p = product[0]

        $('#itemName').val(p.f_name)
        $('#itemName').attr('data-itemid', p.f_pId)
        $('#itemName').attr('itemid', p.f_id)
        $('#itemPrice').val(p.f_price)        
        $('#selectForm').val(p.f_categoryId)
        $('#itemStock').val(p.f_stock)
        $('#itemImg').attr('src', '/images/' + p.f_picName)
        $('#itemDescription').val(p.f_description)

        p.f_isopen == 1 ? $('input#click').prop('checked', true) : $('input#click').prop('checked', false)
        pdEditor.setData(p.f_content)
    },

    SetProductTable: function () {
        //$.each(productData, (i, v) => {
        //    var openStr = ''
        //    v.f_isopen == 1 ? openStr = '開放中' : openStr = '不開放'

        //    $('tbody').append(
        //        `<tr id="pd_${v.f_id}" data-id="${v.f_pId}" onclick="fn.ShowProduct('${v.f_pId}')">
        //        <td>${v.f_name}</td>
        //        <td>${v.f_price}</td>
        //        <td>${v.categoryName}</td>
        //        <td>${v.f_stock}</td>
        //        <td>${openStr}</td>
        //        <td>${v.createTime}</td>
        //     </tr>`)
        //})

        var pdLength = productData.length;
        var pdContent = '';

        for (var i = 0; i < pdLength; i++) {
            var v = productData[i];
            var openStr = ''
            v.f_isopen == 1 ? openStr = '開放中' : openStr = '不開放'
            pdContent +=
                `<tr id="pd_${v.f_id}" data-id="${v.f_pId}" onclick="fn.ShowProduct('${v.f_pId}')">
                    <td>${v.f_name}</td>
                    <td>${v.f_price}</td>
                    <td>${v.categoryName}</td>
                    <td>${v.f_stock}</td>
                    <td>${openStr}</td>
                    <td>${v.createTime}</td>
                 </tr>`;
        }

        $('tbody').html(pdContent);
    },

    SetSearch: function () {
        $('#searchInput').on('keyup', function () {
            var value = $(this).val().toLowerCase()
            $('#productTable tr').filter(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
            })
        })
    },

    SetbtnSave: function () {
        $('#btnSave').click(function () {
            var file_data = $('#customFile').prop('files')[0];

            var f_name = $('#itemName').val();
            var f_price = $('#itemPrice').val();
            var f_categoryId = $('#selectForm').val();
            var f_stock = $('#itemStock').val();

            var formData = new FormData();
                formData.append('ProductPic', file_data);
                formData.append('f_pId', $('#itemName').attr('data-itemid'));
                formData.append('f_name', f_name);
                formData.append('f_price', f_price);
                formData.append('f_categoryId', f_categoryId);
                formData.append('f_stock', f_stock);
                formData.append('f_isopen', $('input#click').prop('checked') == true ? 1 : 0);
                formData.append('f_content', pdEditor.getData());
                formData.append('f_description', $('#itemDescription').val());


            var id = $('#itemName').attr('itemid');
                $(`#pd_${id} td:eq(0)`).text(f_name);
                $(`#pd_${id} td:eq(1)`).text(f_price);
                $(`#pd_${id} td:eq(2)`).text($('#selectForm :selected').text());
                $(`#pd_${id} td:eq(3)`).text(f_stock);
                $(`#pd_${id} td:eq(4)`).text($('input#click').prop('checked') == true ? '開放中' : '不開放');


            fn.UpProdata(formData);
        })
    },

    SetImgUpload: function () {
        //點擊圖片上傳
        $('#customFile').change(function () {
            const file = this.files[0];
            const objectURL = URL.createObjectURL(file);
            $('#itemImg').attr('src', objectURL);
        })
    },

    SetEditor: function () {
        ClassicEditor.create(document.querySelector('#editor'))
            .then(editor => {
                pdEditor = editor
            })
            .catch(error => {
                console.error(error);
            });
    },

    GetProdata: () => {
        $.ajax({
            async: false,
            type: 'get',
            url: '/Products/ProductLists',
            success: (res) => {
                if (res.success) {                    
                    productData = res.item
                    fn.SetProductTable()
                } else {
                    swal(res.message, '', 'error')
                }
            },
            error: (res) => {
                swal(res.message, '', 'error')
            }
        });
    },

    GetCategoryList: function () {
        $.ajax({
            type: 'get',
            url: '/Manager/GetCategoryList',
            success: (res) => {
                if (res.success) {
                    categoryList = res.item
                    fn.SetCategoryList()
                } else {
                    swal('error', 'error', 'error')
                }
            },
            error: (res) => {
                swal('error', 'error', 'error')
            }
        })
    },

    SetCategoryList: function () {        
        $.each(categoryList.selectListItems, function (index, v) {
            $('#selectForm').append(new Option(v.text, v.value))
        })
    },

    UpProdata: function (data) {
                       
        $.ajax({
            type: 'post',
            url: '/Manager/EditProductById',
            data: data,
            cache: false,
            contentType: false,
            processData: false,
            success: (res) => {
                if (res.success) {
                    swal('保存成功', '', 'success')
                } else {
                    swal('保存失敗', '', 'error')
                }
            },
            error: (res) => {
                swal('網路錯誤', '', 'error')
            }
        })
    }
}
