document.addEventListener('DOMContentLoaded', () => {
    updateCartCount();
    loadCart();
    toggleConfirmButton(); // Kiểm tra nút xác nhận thông tin ngay khi tải trang

    const clearCartButton = document.getElementById('clearCartButton');
    if (clearCartButton) {
        clearCartButton.addEventListener('click', clearCart);
    }
});

function addToCart(maSan, tenSb, diaChi, ngayDatSan, maGio, gioBatDau, gioKetThuc, donGia) {
    const cart = JSON.parse(sessionStorage.getItem('gioHang')) || [];

    const isInCart = cart.some(item =>
        item.MaSb === maSan &&
        item.Ngaysudung === ngayDatSan &&
        item.Magio === maGio
    );

    if (isInCart) {
        Swal.fire({
            toast: true, // Chế độ toast
            position: 'top-end',
            icon: 'error',
            title: 'Giờ thuê đã tồn tại!',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true, // Hiển thị thanh tiến trình
        });
        return;
    }

    const gioHang = {
        MaSb: maSan,
        TenSb: tenSb,
        DiaChi: diaChi,
        Ngaysudung: ngayDatSan,
        Magio: maGio,
        Giobatdau: gioBatDau,
        Gioketthuc: gioKetThuc,
        Dongia: donGia
    };

    cart.push(gioHang);
    sessionStorage.setItem('gioHang', JSON.stringify(cart));
    updateCartCount();
    toggleConfirmButton(); // Kiểm tra hiển thị nút xác nhận
    Swal.fire({
        toast: true, // Chế độ toast
        position: 'top-end',
        icon: 'success',
        title: 'Thêm giờ thuê thành công!',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true, // Hiển thị thanh tiến trình
    });
}

function updateCartCount() {
    const cart = JSON.parse(sessionStorage.getItem('gioHang')) || [];
    const itemCount = cart.length;
    const cartItemCountElement = document.getElementById('cartItemCount');
    if (cartItemCountElement) {
        cartItemCountElement.textContent = itemCount;
    }
}

function clearCart() {
    const cart = JSON.parse(sessionStorage.getItem('gioHang')) || [];
    if (cart.length === 0) {
        alert("Giỏ hàng của bạn hiện tại không có giờ thuê nào!");
        return;
    }

    sessionStorage.removeItem('gioHang');
    updateCartCount();
    alert("Đã xóa tất cả giờ thuê trong danh sách chờ!");
    loadCart();
    toggleConfirmButton(); // Kiểm tra hiển thị nút xác nhận
}

function removeFromCart(maSb, maGio, ngayDat) {
    const cart = JSON.parse(sessionStorage.getItem('gioHang')) || [];
    const updatedCart = cart.filter(item =>
        item.MaSb !== maSb || item.Magio !== maGio || item.Ngaysudung !== ngayDat
    );

    sessionStorage.setItem('gioHang', JSON.stringify(updatedCart));
    updateCartCount();
    loadCart();
    toggleConfirmButton(); // Kiểm tra hiển thị nút xác nhận
    Swal.fire({
        toast: true, // Chế độ toast
        position: 'top-end',
        icon: 'success',
        title: 'Xóa giờ thuê thành công!',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true, // Hiển thị thanh tiến trình
    });

}

function loadCart() {
    const cart = JSON.parse(sessionStorage.getItem('gioHang')) || [];
    const cartItemsElement = document.getElementById('cartItems');
    const totalPriceElement = document.getElementById('totalPrice');
    let totalPrice = 0;

    if (cartItemsElement) {
        if (cart.length > 0) {
            cartItemsElement.innerHTML = cart.map(item => {
                const dongiaFormatted = parseFloat(item.Dongia).toLocaleString('vi-VN');
                totalPrice += parseFloat(item.Dongia);
                return `
                    <tr>
                        <td>${item.MaSb}</td>
                        <td>${item.TenSb}</td>
                        <td>${item.DiaChi}</td>
                        <td>${item.Ngaysudung}</td>
                        <td>${item.Giobatdau}</td>
                        <td>${item.Gioketthuc}</td>
                        <td>${dongiaFormatted} VND</td>
                        <td><button onclick="removeFromCart('${item.MaSb}', '${item.Magio}', '${item.Ngaysudung}')">Xóa</button></td>
                    </tr>
                `;
            }).join('');
        } else {
            cartItemsElement.innerHTML = '<tr><td class="text-center" colspan="8">Hiện tại không có giờ thuê nào trong danh sách chờ.</td></tr>';
            totalPrice = 0;
        }
    }

    if (totalPriceElement) {
        totalPriceElement.innerText = totalPrice.toLocaleString('vi-VN');
    }
}

function toggleConfirmButton() {
    const cart = JSON.parse(sessionStorage.getItem('gioHang')) || [];
    const confirmButton = document.getElementById('xntt');
    const ktraButton = document.getElementById('ktratt');

    if (confirmButton) {
        confirmButton.style.display = cart.length > 0 ? 'inline-block' : 'none';
    }
    if (ktraButton) {
        ktraButton.style.display = cart.length > 0 ? 'inline-block' : 'none';
    }
}
