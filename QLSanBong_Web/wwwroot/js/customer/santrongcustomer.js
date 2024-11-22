async function searchButton() {
    const maSb = document.getElementById('idSan').value;
    const ngayDat = document.getElementById('dayOrder').value;
    const gioBatDau = document.getElementById('timeStart').value;
    const gioKetThuc = document.getElementById('timeEnd').value;

    const result = {
        MaSb: maSb || "",
        Ngaysudung: ngayDat || new Date().toISOString().split('T')[0],
        Giobatdau: gioBatDau || "",
        Gioketthuc: gioKetThuc || ""
    };

    if (gioBatDau && gioKetThuc && gioBatDau >= gioKetThuc) {
        alert("Giờ bắt đầu phải nhỏ hơn giờ kết thúc.");
        return;
    }

    try {
        const response = await fetch('https://localhost:7182/api/SanBong/santrong', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(result),
        });

        if (!response.ok) {
            const errorText = await response.text();
            alert("Lỗi: " + errorText);
            return;
        }

        const data = await response.json();
        displaySanTrong(data);
    } catch (error) {
        console.error("Lỗi khi gửi yêu cầu:", error);
    }
}

function displaySanTrong(sanTrongs) {
    const modalTableBody = document.getElementById('modalTableBody');
    modalTableBody.innerHTML = ''; // Xóa dữ liệu cũ

    if (sanTrongs.length === 0) {
        modalTableBody.innerHTML = '<tr><td colspan="7" class="text-center">Không có sân nào trống trong thời gian này.</td></tr>';
    } else {
        sanTrongs.forEach(san => {
            const row = document.createElement('tr');
            row.innerHTML = `
                    <td>${san.maSb}</td>
                    <td>${san.sanBongVM1.tenSb}</td>
                    <td>${san.sanBongVM1.diaChi}</td>
                    <td>${san.ngaysudung}</td>
                    <td>${san.giaGioThueVM1.giobatdau}</td>
                    <td>${san.giaGioThueVM1.gioketthuc}</td>
                    <td>${san.giaGioThueVM1.dongia.toLocaleString()} VND</td>
                    <td>
                        <button class="btn btn-secondary" onclick="addToCart(
                            '${san.maSb}', '${san.sanBongVM1.tenSb}', '${san.sanBongVM1.diaChi}', '${san.ngaysudung}', 
                            '${san.magio}', '${san.giaGioThueVM1.giobatdau}', '${san.giaGioThueVM1.gioketthuc}', '${san.giaGioThueVM1.dongia}')">
                            Thêm vào chờ
                        </button>
                    </td>
                `;
            modalTableBody.appendChild(row);
        });
    }

    const sanTrongModal = new bootstrap.Modal(document.getElementById('sanTrongModal'));
    sanTrongModal.show();
}

/*
    <button class="btn btn-success open-modal" data-bs-toggle="modal" data-bs-target="#modalOrder"
            data-ma-san="${san.maSb}" data-ma-gio="${san.giaGioThueVM1.magio}" 
            data-ngay-dat-san="${san.ngaysudung}" data-gio-bat-dau="${san.giaGioThueVM1.giobatdau}"
            data-gio-ket-thuc="${san.giaGioThueVM1.gioketthuc}" id="order" name="order">Đặt sân</button>
*/

$(document).ready(function () {
    loadSanBongs(); // Gọi hàm tải danh sách sân bóng

    // Hàm tải danh sách sân bóng
    async function loadSanBongs() {
        const url = `https://localhost:7182/api/SanBong/getall`;
        const token = sessionStorage.getItem("Token");

        try {
            // Gửi yêu cầu fetch API
            const response = await fetch(url, {
                method: "GET",
                headers: {
                    "Authorization": `Bearer ${token}`,
                    "Content-Type": "application/json"
                }
            });

            // Kiểm tra xem phản hồi có ok không
            if (!response.ok) {
                throw new Error("Không thể tải danh sách sân bóng.");
            }

            const sanBongs = await response.json();
            const sanBongGrid = $('#sanBongGrid'); // div chứa các ô sân bóng
            const selectSan = $('#idSan').empty(); // Làm sạch phần select trước khi thêm mới

            // Làm sạch lại grid trước khi hiển thị mới
            sanBongGrid.empty();
            
            // Thêm option "Chọn sân" vào select
            selectSan.append('<option value="">Chọn sân</option>');

            // Duyệt qua từng sân bóng và hiển thị vào grid và select
            sanBongs.forEach(item => {
                const sanBongCard = `
                    <div class="col-lg-12 mb-4">
                        <div class="item">
                            <div class="row">
                                <div class="col-lg-6">
                                    <div style="position: relative; width: 100%; padding-top: 56.25%; overflow: hidden;">
                                        <img src="data:image/png;base64,${item.hinhanh}" 
                                             alt="${item.tenSb}" 
                                             style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: cover;" 
                                             class="img-thumbnail">
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <div class="row">
                                        <div class="col-md-8">
                                            <h4 class="mb-4">Sân bóng ${item.tenSb}</h4>
                                        </div>
                                        <div class="col-md-4 text-right">
                                            <a onclick="Stadiumdetail('${item.maSb}')"
                                               class="btn btn-success px-4">
                                                Đặt sân
                                            </a>
                                        </div>
                                    </div>
                                    <p>
                                        <i class="fa fa-map-marker-alt text-primary mr-3"></i> ${item.diaChi}
                                    </p>
                                    <div class="row pt-2 pb-4">
                                        <div class="col-6 col-md-12">
                                            <ul class="list-inline m-0">
                                                <li class="py-2 border-top border-bottom">
                                                    <i class="fa fa-check text-primary mr-3"></i> ${item.ghichu}
                                                </li>
                                                <li class="py-2 border-bottom">
                                                    <i class="fa fa-check text-primary mr-3"></i> Diện tích: ${item.dientich}  m<sup>2</sup>
                                                </li>
                                                <li class="py-2 border-bottom">
                                                    <i class="fa fa-check text-primary mr-3"></i> Giá: 400.000 - 700.000 VND
                                                </li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <hr class="my-4">
                        </div>
                    </div>
                `;

                // Thêm sân bóng vào grid
                sanBongGrid.append(sanBongCard);

                // Thêm option vào select
                const option = `<option value="${item.maSb}">${item.tenSb}</option>`;
                selectSan.append(option);
            });
        } catch (error) {
            console.error("Lỗi:", error);
            // Hiển thị thông báo lỗi
            $('#sanBongGrid').html(`<div class="text-danger text-center">${error.message}</div>`);
        }
    }
});



window.Stadiumdetail = function (maSb) {
    window.location.href = `/Customer/SanBong?id=${maSb}`;
};

$(document).ready(function () {
    $('#userDropdown').on('click', function (event) {
        event.preventDefault(); // Ngăn mặc định để không chuyển hướng
        $(this).next('.dropdown-menu').toggle(); // Bật/tắt dropdown
    });
});

// Đóng dropdown khi nhấp ra ngoài
$(document).click(function (event) {
    if (!$(event.target).closest('#userDropdown').length) {
        $('.dropdown-menu').hide();
    }
});

