﻿let hoaDons = [];
let dataTable;
let manv = "";

// Hàm lấy mã khách hàng từ token
function getManvFromToken(token) {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload["Manguoidung"]; 
}

if (!token) {
    console.error("Token không tồn tại.");
    document.getElementById('yeuCauContainer').innerHTML = `<p class="text-danger text-center">Token không tồn tại.</p>`;
} else {
    manv = getManvFromToken(token);
}

document.addEventListener("DOMContentLoaded", async function () {
    await loadHoaDons();
});

async function loadHoaDons() {

    const url = `https://localhost:7182/api/PhieuDatSan/getbynv?maNv=${manv}`;
    try {
        const token = sessionStorage.getItem("Token");
        const response = await fetch(url, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });

        if (!response.ok) {
            throw new Error("Không thể tải danh sách hóa đơn.");
        }

        hoaDons = await response.json();
        populateTable(hoaDons);

        // Khởi tạo DataTable sau khi dữ liệu được tải lên
        dataTable = $('#hoaDonTable').DataTable({
            "paging": true,
            "ordering": true,
            "info": true,
            "searching": true,
            "language": {
                "paginate": {
                    "next": "Trang sau",
                    "previous": "Trang trước"
                },
                "info": "Hiển thị từ _START_ đến _END_ của _TOTAL_ hóa đơn",
                "search": "Tìm kiếm:"
            }
        });
    } catch (error) {
        console.error("Lỗi:", error);
        document.getElementById('hoaDonContainer').innerHTML = `<p class="text-danger text-center">${error.message}</p>`;
    }
}

function populateTable(hoaDons) {
    const hoaDonList = document.getElementById('hoaDonList');
    hoaDonList.innerHTML = '';

    hoaDons.forEach(yc => {
        hoaDonList.innerHTML += `
            <tr>
                <td>${yc.maPds}</td>
                <td>${yc.khachHangDS ? yc.khachHangDS.tenKh : 'N/A'}</td>
                <td>${yc.khachHangDS ? yc.khachHangDS.sdt : 'N/A'}</td>
                <td>${yc.maNv}</td>
                <td>${yc.ngaylap}</td>
                <td>${new Intl.NumberFormat('vi-VN').format(yc.tongTien)} VND</td>
                <td>${yc.ghiChu || 'Không có'}</td>
                <td>
                    <button class="btn btn-info btn-sm"
                            onclick="infoHoaDon('${yc.maPds}')">
                        <i class="fas fa-info"></i> Chi tiết
                    </button>
                </td>
            </tr>
        `;
    });
}

// Hàm xử lý khi nhấn vào thông tin hóa đơn
function infoHoaDon(maPds) {
    loadHoaDonDetail(maPds);
}

// Hàm tải chi tiết hóa đơn
async function loadHoaDonDetail(maPds) {
    const url = `https://localhost:7182/api/PhieuDatSan/getbyid?id=${maPds}`; // Truyền tham số qua URL
    try {
        const token = sessionStorage.getItem("Token");
        const response = await fetch(url, {
            method: "GET", // GET không cần body
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });

        if (!response.ok) {
            throw new Error("Không thể tải thông tin chi tiết hóa đơn.");
        }

        const detailHoaDon = await response.json();
        populateDetailModal(detailHoaDon);
    } catch (error) {
        console.error("Lỗi:", error);
    }
}

// Hàm điền dữ liệu vào modal chi tiết hóa đơn
function populateDetailModal(detailHoaDon) {
    const modalContent = document.getElementById('modalContent');
    if (!modalContent) {
        console.error("Không tìm thấy phần tử modalContent");
        return;
    }

    // Tạo nội dung cho modal
    let content = `
        <p><strong>Mã khách hàng:</strong> ${detailHoaDon.maKh || 'N/A'}</p>
        <p><strong>Tên khách hàng:</strong> ${detailHoaDon.khachHangDS?.tenKh || 'N/A'}</p>
        <p><strong>Số điện thoại:</strong> ${detailHoaDon.khachHangDS?.sdt || 'N/A'}</p>
        <p><strong>Mã nhân viên:</strong> ${detailHoaDon.maNv || 'N/A'}</p>
        <p><strong>Ngày lập hóa đơn:</strong> ${detailHoaDon.ngaylap || 'N/A'}</p>
        <p><strong>Tổng tiền:</strong> ${detailHoaDon.tongTien || 'N/A'} VND </p>
        <p><strong>Ghi Chú:</strong> ${detailHoaDon.ghiChu || 'Không có'}</p>
        <table class="table table-bordered">
            <thead>
                <tr>
                    <th>Mã SB</th>
                    <th>Ngày sử dụng</th>
                    <th>Mã giờ</th>
                    <th>Giờ bắt đầu</th>
                    <th>Giờ kết thúc</th>
                    <th>Đơn giá</th>
                </tr>
            </thead>
            <tbody>`;

    // Kiểm tra và lặp qua chi tiết hóa đơn
    if (Array.isArray(detailHoaDon.chiTietPds) && detailHoaDon.chiTietPds.length > 0) {
        detailHoaDon.chiTietPds.forEach(detail => {
            content += `
                <tr>
                    <td>${detail.maSb || 'N/A'}</td>
                    <td>${detail.ngaysudung || 'N/A'}</td>
                    <td>${detail.maGio || 'N/A'}</td>
                    <td>${detail.giaGioThueVM1?.giobatdau || 'N/A'}</td>
                    <td>${detail.giaGioThueVM1?.gioketthuc || 'N/A'}</td>
                    <td>${new Intl.NumberFormat('vi-VN').format(detail.giaGioThueVM1?.dongia)} VND</td>
                </tr>`;
        });
    } else {
        content += `<tr><td colspan="6" class="text-center">Không có chi tiết nào.</td></tr>`;
    }

    content += `</tbody></table>`;
    modalContent.innerHTML = content;

    // Mở modal
    $('#hoaDonModal').modal('show');
}


// Sự kiện đóng modal
document.querySelector('.modalclose').addEventListener('click', function () {
    $('#hoaDonModal').modal('hide');
});