$('.single-item').slick({
    dots: true,
    fade: true,
    arrows: true,
    autoplay: true,
    autoplaySpeed: 5000,
});

//card-img-item
$('.card-img-item').slick({
    //dots: true,
    //fade: true,
    //arrows: true,
    //autoplay: true,
    //autoplaySpeed: 5000,
    //centerMode: true,
    //centerPadding: '10px',
    lazyLoad: 'ondemand',
    slidesToShow: 3,
    slidesToScroll: 1
});

//ClassicEditor
//    .create(document.querySelector('#editor'))
//    .catch(error => {
//        console.error(error);
//    });

//ClassicEditor.create(document.querySelector('#editor'))
//    .then(editor => {
//        editor.ui.view.editable.element.style.height = '500px';
//    })
//    .catch(error => {
//        console.error(error);
//    });
