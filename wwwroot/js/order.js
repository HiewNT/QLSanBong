$(document).ready(function () {
    $(".add-to-cart").on('click', function () {
        var masb = $(this).data("ma-san");
        var ngay = $(this).data("ngay-dat-san");
        var magio = $(this).data("ma-gio");
        var giobatdau = $(this).data("gio-batdau");
        var gioketthuc = $(this).data("gio-ketthuc");
        $.ajax({
            url: '/Customer/Cart/AddItem',
            data: {
                MaSb: masb,
                NgayDat: ngay,
                MaGio: magio,
                Giobatdau: giobatdau,
                Gioketthuc: gioketthuc
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.status == true) {
                    $.notify("Thêm vào danh sách chờ thành công!", "success");
                }
            },
        });
    });
});
