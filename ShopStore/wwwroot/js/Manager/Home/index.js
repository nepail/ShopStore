$(function () {
    Index.InitPage();
})

var Index = {

    InitPage() {
        Index.UC.SetInput();
    },


    UC: {
        SetInput() {
            $(document).on('focus', '.input', this.FocusFunc);
            $(document).on('blur', '.input', this.BlurFunc);
        },

        FocusFunc() {
            $(this).parent().parent().addClass('focus');
        },

        BlurFunc() {
            $(this).parent().parent().removeClass('focus')
        }
    },

    DATA: {

    }
}