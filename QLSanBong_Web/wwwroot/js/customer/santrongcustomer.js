async function searchButton() {
    const maSb = document.getElementById('idSan')?.value || "";
    const ngayDat = document.getElementById('dayOrder')?.value || new Date().toISOString().split('T')[0];
    const gioBatDau = document.getElementById('timeStart')?.value || "";
    const gioKetThuc = document.getElementById('timeEnd')?.value || "";

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
            body: JSON.stringify({ MaSb: maSb, Ngaysudung: ngayDat, Giobatdau: gioBatDau, Gioketthuc: gioKetThuc }),
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
    if (!modalTableBody) {
        console.error("Không tìm thấy modalTableBody trong DOM.");
        return;
    }

    modalTableBody.innerHTML = ''; // Làm sạch dữ liệu cũ

    if (sanTrongs.length === 0) {
        modalTableBody.innerHTML = '<tr><td colspan="8" class="text-center">Không có sân nào trống trong thời gian này.</td></tr>';
    } else {
        const rows = sanTrongs.map(san => `
            <tr>
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
            </tr>
        `).join('');
        modalTableBody.innerHTML = rows;
    }

    const sanTrongModal = new bootstrap.Modal(document.getElementById('sanTrongModal'));
    sanTrongModal.show();
}

async function loadSanBongs() {
    const sanBongGrid = document.getElementById('sanBongGrid');
    const selectSan = document.getElementById('idSan');
    if (!sanBongGrid || !selectSan) {
        return;
    }

    try {
        const response = await fetch(`https://localhost:7182/api/SanBong/getall`, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${sessionStorage.getItem("Token")}`,
                'Role': 'KhachHang',  // Truyền role vào header
                "Content-Type": "application/json",
            },
        });

        if (!response.ok) {
            const errorData = await response.json();
            alert(errorData?.message || "Không thể tải danh sách sân bóng.");
            return;
        }

        const sanBongs = await response.json();

        selectSan.innerHTML = '<option value="">Chọn sân</option>';
        sanBongGrid.innerHTML = sanBongs.map(item => `
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
                <h4 class="mb-4">Sân bóng ${item.tenSb}</h4>
                <p><i class="fa fa-map-marker-alt text-primary me-3"></i> ${item.diaChi}</p>
                <div class="row pt-2 pb-4">
                    <div class="col-12">
                        <ul class="list-inline m-0">
                            <li class="py-2 border-top border-bottom"><i class="fa fa-check text-primary me-3"></i> ${item.ghichu}</li>
                            <li class="py-2 border-bottom"><i class="fa fa-check text-primary me-3"></i> Diện tích: ${item.dientich} m<sup>2</sup></li>
                        </ul>
                    </div>
                </div>
                <a onclick="Stadiumdetail('${item.maSb}')" class="btn btn-success px-4">Đặt sân</a>
            </div>
        </div>
        <hr class="my-4">
    </div>
</div>

        `).join('');

        selectSan.innerHTML += sanBongs.map(item => `<option value="${item.maSb}">${item.tenSb}</option>`).join('');
    } catch (error) {
        console.error("Lỗi khi tải danh sách sân bóng:", error);
    }
}

document.addEventListener('DOMContentLoaded', loadSanBongs);

function Stadiumdetail(maSb) {
    window.location.href = `/Customer/SanBong?id=${maSb}`;
}


