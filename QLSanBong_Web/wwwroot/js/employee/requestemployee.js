let yeuCaus = [];
let dataTable;
let currentFilterStatus = ""; // Lưu trữ trạng thái lọc
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
    console.log(manv);
  await loadYeuCaus();
});

async function loadYeuCaus() {
    let url = `https://localhost:7182/api/YeuCauDatSan/getall`;
  try {
    const token = sessionStorage.getItem("Token");
    const response = await fetch(url, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      throw new Error("Không thể tải danh sách yêu cầu đặt sân.");
    }

    yeuCaus = await response.json();
    populateTable(yeuCaus);

    dataTable = $("#yeuCauTable").DataTable({
      paging: true,
      ordering: true,
      info: true,
      searching: true,
      language: {
        paginate: {
          next: "Trang sau",
          previous: "Trang trước",
        },
        info: "Hiển thị từ _START_ đến _END_ của _TOTAL_ yêu cầu đặt sân",
        search: "Tìm kiếm:",
      },
    });
  } catch (error) {
    console.error("Lỗi:", error);
    document.getElementById(
      "yeuCauContainer"
    ).innerHTML = `<p class="text-danger text-center">${error.message}</p>`;
  }
}

function populateTable(yeuCaus) {
  const yeuCauList = document.getElementById("yeuCauList");
  yeuCauList.innerHTML = "";

  yeuCaus.forEach((yc) => {
    yc.chiTietYcds.forEach((detail) => {
      yeuCauList.innerHTML += `
                    <tr>
                        <td>${
                          yc.khachHangDS ? yc.khachHangDS.tenKh : "N/A"
                        }</td>
                        <td>${yc.khachHangDS ? yc.khachHangDS.sdt : "N/A"}</td>
                        <td>${yc.thoigiandat}</td>
                        <td>${detail.ngaysudung}</td>
                        <td>${
                          detail.giaGioThueVM1
                            ? detail.giaGioThueVM1.giobatdau
                            : "N/A"
                        }</td>
                        <td>${
                          detail.giaGioThueVM1
                            ? detail.giaGioThueVM1.gioketthuc
                            : "N/A"
                        }</td>
                        <td>${detail.maSb}</td>
                        <td>${detail.trangThai}</td>
                        <td><button class="btn btn-info btn-sm" onclick="infoyeuCau('${
                          yc.stt
                        }', '${detail.maSb}', '${detail.magio}', '${
        detail.ngaysudung
      }')">Chi tiết</button>
                        </td>
                    </tr>
                `;
    });
  });
}

function filterTable(status, btn) {
  currentFilterStatus = status; // Lưu trạng thái lọc hiện tại
  dataTable.columns(7).search(status).draw();
  const buttons = document.querySelectorAll(".btn-group .btn");
  buttons.forEach((button) => button.classList.remove("active"));
  btn.classList.add("active");
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
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(request),
    });

    if (!response.ok) {
      throw new Error("Không thể tải thông tin chi tiết yêu cầu đặt sân.");
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
    <p><strong>Tên khách hàng:</strong> ${detailYeuCau.khachHangDS ? detailYeuCau.khachHangDS.tenKh : "N/A"
        }</p>
    <p><strong>Số điện thoại:</strong> ${detailYeuCau.khachHangDS ? detailYeuCau.khachHangDS.sdt : "N/A"
        }</p>
    <p><strong>Thời Gian Đặt:</strong> ${detailYeuCau.thoigiandat}</p>
    <p><strong>Ghi Chú:</strong> ${detailYeuCau.ghiChu || "Không có"}</p>
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

    detailYeuCau.chiTietYcds.forEach((detail) => {
        content += `
                <tr>
                    <td>${detail.maSb}</td>
                    <td>${detail.ngaysudung}</td>
                    <td>${detail.magio}</td>
                    <td>${detail.giaGioThueVM1
                ? detail.giaGioThueVM1.giobatdau
                : "N/A"
            }</td>
                    <td>${detail.giaGioThueVM1
                ? detail.giaGioThueVM1.gioketthuc
                : "N/A"
            }</td>
                    <td>${detail.giaGioThueVM1 ? detail.giaGioThueVM1.dongia : "N/A"
            }</td>
                    <td>${detail.trangThai}</td>
                    <td>${detail.trangThai === "Chờ xác nhận"
                ? `<button class="btn btn-success btn-sm" onclick="confirmAndAddPds('${detail.stt}', '${detail.maSb}', '${detail.magio}', '${detail.ngaysudung}', 'Đã xác nhận', '${detailYeuCau.maKh}')">
                    Xác nhận
                </button>
                <button class="btn btn-danger btn-sm"
                        onclick="confirmAndAddPds('${detail.stt}', '${detail.maSb}', '${detail.magio}', '${detail.ngaysudung}', 'Đã từ chối', '${detailYeuCau.maKh}')">
                    Từ chối
                </button>`
                : ""
            }
                    </td>
                </tr>
            `;
    });

    content += `</tbody></table>`;
    document.getElementById("modalContent").innerHTML = content;
    $("#yeuCauModal").modal("show");
}


async function confirmAndAddPds(stt, maSb, magio, ngaysudung, status, maKh) {
  const updateUrl = "https://localhost:7182/api/YeuCauDatSan/update";
  const addPdsUrl = "https://localhost:7182/api/PhieuDatSan/add";

  const requestData = {
    Id: stt,
    MaSb: maSb,
    Magio: magio,
    Ngaysudung: ngaysudung,
    TrangThai: status,
  };

  try {
    const token = sessionStorage.getItem("Token");

    if (status === "Đã xác nhận") {
      const pdsData = {
        maNv: manv,
        maKh: maKh,
        ngaylap: new Date().toISOString(),
        ghiChu: "Đã thanh toán",
        sttds: stt,
        chiTietPdsVM: [
          {
            maGio: magio,
            maSb: maSb,
            ngaysudung: ngaysudung,
            ghichu: ""
          }
        ]
      };

      const addPdsResponse = await fetch(addPdsUrl, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify(pdsData),
      });

      if (!addPdsResponse.ok) {
        throw new Error("Không thể thêm phiếu đặt sân.");
      }
    }

    const updateResponse = await fetch(updateUrl, {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(requestData),
    });

    if (!updateResponse.ok) {
      throw new Error("Không thể cập nhật yêu cầu đặt sân.");
    }
    alert("Tạo hoá đơn thành công!");

    if ($.fn.DataTable.isDataTable("#yeuCauTable")) {
      $("#yeuCauTable").DataTable().destroy();
    }

    await loadYeuCaus();

    // Áp dụng bộ lọc trạng thái hiện tại
    if (currentFilterStatus) {
      dataTable.columns(7).search(currentFilterStatus).draw();
    }

    $("#yeuCauModal").modal("hide");
  } catch (error) {
    console.error("Lỗi:", error);
    alert(error.message);
  }
}




$(".modalclose").on("click", function () {
  $("#yeuCauModal").modal("hide");
});
