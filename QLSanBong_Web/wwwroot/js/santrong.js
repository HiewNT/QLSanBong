let debounceTimeout;

function debounce(func, delay) {
    clearTimeout(debounceTimeout);
    debounceTimeout = setTimeout(func, delay);
}

async function searchButton() {
    debounce(async () => {
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
            displaySanTrong(data); // Hiển thị tối đa 5 kết quả
        } catch (error) {
            console.error("Lỗi:", error);
        }
    }, 500); // Chỉ gọi API sau khi dừng thao tác 500ms
}

function displaySanTrong(sanTrongs) {
    const modalTableBody = document.getElementById('modalTableBody');
    modalTableBody.innerHTML = ''; // Xóa dữ liệu cũ

    if (sanTrongs.length === 0) {
        modalTableBody.innerHTML = '<tr><td colspan="8" class="text-center">Không có sân nào trống trong thời gian này.</td></tr>';
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
                        '${san.maSb}', 
                        '${san.sanBongVM1.tenSb}',  
                        '${san.sanBongVM1.diaChi}', 
                        '${san.ngaysudung}', 
                        '${san.magio}',
                        '${san.giaGioThueVM1.giobatdau}', 
                        '${san.giaGioThueVM1.gioketthuc}', 
                        '${san.giaGioThueVM1.dongia}')">
                        Thêm vào chờ
                    </button>
                </td>
            `;
            modalTableBody.appendChild(row);
        });
    }

    // Hiển thị modal sau khi đã điền dữ liệu
    const sanTrongModal = new bootstrap.Modal(document.getElementById('sanTrongModal'));
    sanTrongModal.show();
}


    async function loadSanBongs() {
        try {
            const response = await fetch('https://localhost:7182/api/SanBong/usergetall', {
                method: "GET",
                headers: {
                    "Content-Type": "application/json"
                }
            });

            if (!response.ok) {
                const errorData = await response.json();
                const errorMessage = errorData?.message || "Không thể tải danh sách sân bóng.";
                alert(errorMessage);
                return;
            }

            const sanBongs = await response.json();
            const sanBongGrid = $('#sanBongGrid').empty(); // Làm sạch div trước khi thêm mới
            const selectSan = $('#idSan').empty(); // Làm sạch phần select trước khi thêm mới

            selectSan.append('<option value="">Chọn sân</option>');

            sanBongs.forEach(sb => {
                const sanBongCard = `
                    <div class="col-md-3 mb-6">
                        <div class="card h-100">
                            <img src="data:image/png;base64,${sb.hinhanh}" class="card-img-top" alt="${sb.tenSb}" style="height: 15rem; object-fit: cover;">
                            <div class="card-body">
                                <h5 class="card-title text-center">Sân bóng ${sb.tenSb}</h5>
                                <p class="card-text">Diện tích: ${sb.dientich} m²</p>
                                <p class="card-text">Địa chỉ: ${sb.diaChi}</p>
                                <p class="card-text">${sb.ghichu}</p>
                            </div>
                        </div>
                    </div>
                `;
                sanBongGrid.append(sanBongCard);
                selectSan.append(`<option value="${sb.maSb}">${sb.tenSb}</option>`);
            });
        } catch (error) {
            console.error("Lỗi:", error);
            $('#sanBongGrid').html(`<div class="text-danger text-center">${error.message}</div>`);
        }
}

document.addEventListener('DOMContentLoaded', loadSanBongs);