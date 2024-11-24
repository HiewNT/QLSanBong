let dataTable;

function getMakhFromToken(token) {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload["Manguoidung"]; // Giả sử mã khách hàng lưu trong claim "makh"
}


if (!token) {
    console.error("Token không tồn tại.");
    document.getElementById('yeuCauContainer').innerHTML = `<p class="text-danger text-center">Token không tồn tại.</p>`;
} else {
    // Lấy mã khách hàng từ token và gán vào biến toàn cục makh
    makh = getMakhFromToken(token);
}


document.addEventListener("DOMContentLoaded", async function () {
    await loadYeuCaus();
});

async function loadYeuCaus() {
    let url = `https://localhost:7182/api/YeuCauDatSan/getbykh?makh=${makh}`;

    try {
        const response = await fetch(url, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });

        if (!response.ok) {
            // Nếu response không thành công, lấy thông báo lỗi từ API (nếu có)
            const errorData = await response.json(); // Nếu API trả về JSON chứa thông báo
            const errorMessage = errorData?.message || "Không thể tải danh sách yêu cầu";
            document.getElementById('yeuCauContainer').innerHTML = `<p class="text-danger text-center">${errorMessage}</p>`;

            // Hiển thị thông báo warning với Toastr
            alert(errorMessage);

            return;
        }

        const yeuCaus = await response.json();
        populateTable(yeuCaus);

        // Hủy DataTable nếu đã khởi tạo trước đó
        if ($.fn.DataTable.isDataTable('#yeuCauTable')) {
            $('#yeuCauTable').DataTable().destroy();
        }

        // Khởi tạo lại DataTable
        dataTable = $('#yeuCauTable').DataTable({
            "paging": true,
            "ordering": true,
            "info": true,
            "searching": true,
            "language": {
                "paginate": {
                    "next": "Trang sau",
                    "previous": "Trang trước"
                },
                "info": "Hiển thị từ _START_ đến _END_ của _TOTAL_ yêu cầu đặt sân",
                "search": "Tìm kiếm:"
            }
        });
    } catch (error) {
        console.error("Lỗi:", error);
        document.getElementById('yeuCauContainer').innerHTML = `<p class="text-danger text-center">${error.message}</p>`;
    }
}

function getMakhFromToken(token) {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload["Manguoidung"]; // Giả sử mã khách hàng lưu trong claim "makh"
}

function populateTable(yeuCaus) {
    const yeuCauList = document.getElementById('yeuCauList');
    yeuCauList.innerHTML = '';

    yeuCaus.forEach(yc => {
        yc.chiTietYcds.forEach(detail => {
            yeuCauList.innerHTML += `
                <tr>
                    <td>${yc.khachHangDS ? yc.khachHangDS.tenKh : 'N/A'}</td>
                    <td>${yc.khachHangDS ? yc.khachHangDS.sdt : 'N/A'}</td>
                    <td>${yc.thoigiandat}</td>
                    <td>${detail.ngaysudung}</td>
                    <td>${detail.giaGioThueVM1 ? detail.giaGioThueVM1.giobatdau : 'N/A'}</td>
                    <td>${detail.giaGioThueVM1 ? detail.giaGioThueVM1.gioketthuc : 'N/A'}</td>
                    <td>${detail.maSb}</td>
                    <td>${detail.trangThai}</td>
                    <td>
                        <button class="btn btn-info btn-sm"
                                onclick="infoyeuCau('${yc.stt}', '${detail.maSb}', '${detail.magio}', '${detail.ngaysudung}')">
                            Chi tiết
                        </button>
                    </td>
                </tr>
            `;
        });
    });
}

function filterTable(status, btn) {
    if (dataTable) {
        dataTable.columns(7).search(status).draw();
        const buttons = document.querySelectorAll('.btn-group .btn');
        buttons.forEach(button => button.classList.remove('active'));
        btn.classList.add('active');
    } else {
        console.warn('DataTable chưa được khởi tạo.');
    }
}

function infoyeuCau(stt, maSb, magio, ngaysudung) {
    const request = { Id: stt, MaSb: maSb, Magio: magio, Ngaysudung: ngaysudung };
    loadYeuCauDetail(request);
}

async function loadYeuCauDetail(request) {
    let url = `https://localhost:7182/api/YeuCauDatSan/getby`;
    try {
        const token = sessionStorage.getItem("Token");
        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(request)
        });

        if (!response.ok) {
            // Nếu response không thành công, lấy thông báo lỗi từ API (nếu có)
            const errorData = await response.json(); // Nếu API trả về JSON chứa thông báo
            const errorMessage = errorData?.message || "Không thể tải danh sách yêu cầu";

            // Hiển thị thông báo warning với Toastr
            alert(errorMessage);

            return;
        }

        const detailYeuCau = await response.json();
        populateDetailModal(detailYeuCau);
    } catch (error) {
        console.error("Lỗi:", error);
    }
}

function populateDetailModal(detailYeuCau) {
    let content = `
        <p><strong>Mã khách hàng:</strong> ${detailYeuCau.maKh}</p>
        <p><strong>Tên khách hàng:</strong> ${detailYeuCau.khachHangDS ? detailYeuCau.khachHangDS.tenKh : 'N/A'}</p>
        <p><strong>Số điện thoại:</strong> ${detailYeuCau.khachHangDS ? detailYeuCau.khachHangDS.sdt : 'N/A'}</p>
        <p><strong>Thời Gian Đặt:</strong> ${detailYeuCau.thoigiandat}</p>
        <p><strong>Ghi Chú:</strong> ${detailYeuCau.ghiChu || 'Không có'}</p>
        <table class="table table-bordered">
            <thead>
                <tr>
                    <th>Mã SB</th>
                    <th>Ngày sử dụng</th>
                    <th>Mã giờ</th>
                    <th>Giờ bắt đầu</th>
                    <th>Giờ kết thúc</th>
                    <th>Đơn giá</th>
                    <th>Trạng Thái</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
    `;

    detailYeuCau.chiTietYcds.forEach(detail => {
        content += `
            <tr>
                <td>${detail.maSb}</td>
                <td>${detail.ngaysudung}</td>
                <td>${detail.magio}</td>
                <td>${detail.giaGioThueVM1 ? detail.giaGioThueVM1.giobatdau : 'N/A'}</td>
                <td>${detail.giaGioThueVM1 ? detail.giaGioThueVM1.gioketthuc : 'N/A'}</td>
                <td>${detail.giaGioThueVM1 ? detail.giaGioThueVM1.dongia : 'N/A'}</td>
                <td>${detail.trangThai}</td>
                <td>
                    ${detail.trangThai === 'Chờ xác nhận' ? `
                        <button class="btn btn-danger btn-sm"
                                onclick="confirmRequest('${detail.stt}', '${detail.maSb}', '${detail.magio}', '${detail.ngaysudung}','Đã hủy')">
                            Hủy yêu cầu
                        </button>
                    ` : ''}
                </td>
            </tr>
        `;
    });

    content += `</tbody></table>`;
    document.getElementById('modalContent').innerHTML = content;
    $('#yeuCauModal').modal('show');
}
async function confirmRequest(stt, maSb, magio, ngaysudung, status) {
    // Hiển thị thông báo xác nhận
    const isConfirmed = confirm("Bạn có chắc chắn muốn hủy yêu cầu này không?");

    if (isConfirmed) {
        // Nếu người dùng chọn "OK", tiếp tục với việc cập nhật yêu cầu
        const requestData = { Id: stt, MaSb: maSb, Magio: magio, Ngaysudung: ngaysudung, TrangThai: status };
        await updateYeuCau(requestData);
    } else {
        // Nếu người dùng chọn "Hủy", không làm gì cả
        console.log("Hủy thao tác.");
    }
}

async function updateYeuCau(requestData) {
    const url = 'https://localhost:7182/api/YeuCauDatSan/update';
    try {
        const token = sessionStorage.getItem("Token");
        const response = await fetch(url, {
            method: "PUT",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(requestData)
        });

        if (!response.ok) {
            // Nếu response không thành công, lấy thông báo lỗi từ API (nếu có)
            const errorData = await response.json(); // Nếu API trả về JSON chứa thông báo
            const errorMessage = errorData?.message || "Không thể tải danh sách yêu cầu";

            // Hiển thị thông báo warning với Toastr
            alert(errorMessage);

            return;
        }

        // Kiểm tra nếu phản hồi là một chuỗi thay vì JSON
        const responseText = await response.text();

        // Kiểm tra nếu phản hồi là chuỗi "Success"
        if (responseText === "Success") {
            alert("Hủy yêu cầu thành công.");
            $('#yeuCauModal').modal('hide');
            // Hủy DataTable nếu đã khởi tạo trước đó, sau đó khởi tạo lại
            if ($.fn.DataTable.isDataTable('#yeuCauTable')) {
                $('#yeuCauTable').DataTable().destroy();
            }
            await loadYeuCaus();
        } else {
            throw new Error("Đã xảy ra lỗi khi cập nhật yêu cầu.");
        }
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error.message); // Hiển thị thông báo lỗi cho người dùng
    }
}

$('.modalclose').on('click', function () {
    $('#yeuCauModal').modal('hide');
});