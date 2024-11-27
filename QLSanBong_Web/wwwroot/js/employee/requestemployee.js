let yeuCaus = [];
let dataTable;
let currentFilterStatus = ""; // Lưu trữ trạng thái lọc
let manv = "";

// Hàm lấy mã khách hàng từ token
function getManvFromToken(token) {
  const payload = JSON.parse(atob(token.split(".")[1]));
  return payload["Manguoidung"];
}

if (!token) {
  console.error("Token không tồn tại.");
  document.getElementById(
    "yeuCauContainer"
  ).innerHTML = `<p class="text-danger text-center">Token không tồn tại.</p>`;
} else {
  manv = getManvFromToken(token);
}

document.addEventListener("DOMContentLoaded", async function () {
  console.log(manv);
  await loadYeuCaus();
});

async function loadYeuCaus() {
  const url = `https://localhost:7182/api/YeuCauDatSan/getall`;

  try {
    const response = await fetch(url, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
        Role: "NhanVienDS", // Truyền role vào header
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      const errorData = await response.json();
      const errorMessage =
        errorData?.message || "Không thể tải danh sách yêu cầu";

      document.getElementById(
        "yeuCauContainer"
      ).innerHTML = `<p class="text-danger text-center">${errorMessage}</p>`;
      alert(errorMessage);
      return;
    }

    const yeuCaus = await response.json();
    const yeuCauList = document.getElementById("yeuCauList");
    yeuCauList.innerHTML = "";

    yeuCaus.forEach((yc) => {
      yc.chiTietYcds.forEach((detail) => {
        yeuCauList.innerHTML += `
          <tr>
              <td>${yc.khachHangDS ? yc.khachHangDS.tenKh : "N/A"}</td>
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
              <td>
                  <button style="display: none;" data-auth="yeucau_read" 
                          class="btn btn-info btn-sm" 
                          onclick="infoyeuCau('${yc.stt}', '${detail.maSb}', '${detail.magio}', '${detail.ngaysudung}')">
                      Chi tiết
                  </button>
              </td>
          </tr>
        `;
      });
    });

    // Gọi hàm xử lý quyền
    await handlePermissionsForFrontend();
    // Tạo DataTable sau khi dữ liệu được hiển thị
    dataTable = $('#yeuCauTable').DataTable({
      // Cấu hình DataTable
      searching: true,         // Kích hoạt tìm kiếm
      paging: true,            // Kích hoạt phân trang
      info: true,              // Hiển thị thông tin bảng
      autoWidth: false,        // Không tự động tính toán độ rộng cột
      columnDefs: [
          { targets: 7, searchable: true }, // Cho phép tìm kiếm ở cột Trạng thái
          { targets: 8, orderable: false }  // Không sắp xếp ở cột hành động
      ],
      language: {
          lengthMenu: "Hiển thị _MENU_ dòng mỗi trang",
          zeroRecords: "Không tìm thấy dữ liệu",
          info: "Hiển thị _START_ đến _END_ của _TOTAL_ dòng",
          infoEmpty: "Không có dữ liệu",
          infoFiltered: "(lọc từ _MAX_ dòng)",
          search: "Tìm kiếm:",
          paginate: {
              first: "Đầu",
              last: "Cuối",
              next: "Tiếp",
              previous: "Trước"
          }
      }
  });

  } catch (error) {
    console.error("Lỗi:", error);
    document.getElementById(
      "yeuCauContainer"
    ).innerHTML = `<p class="text-danger text-center">${error.message}</p>`;
  }
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
    const response = await fetch(url, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        Role: "NhanVienDS", // Truyền role vào header
        "Content-Type": "application/json",
      },
      body: JSON.stringify(request),
    });

    if (!response.ok) {
      const errorData = await response.json();
      const errorMessage = errorData?.message || "Không thể chỉnh sửa yêu cầu";
      alert(errorMessage);
      return;
    }

    const detailYeuCau = await response.json();


    // Chèn nội dung trực tiếp vào giao diện
    let content = `
      <p><strong>Mã khách hàng:</strong> ${detailYeuCau.maKh}</p>
      <p><strong>Tên khách hàng:</strong> ${
        detailYeuCau.khachHangDS ? detailYeuCau.khachHangDS.tenKh : "N/A"
      }</p>
      <p><strong>Số điện thoại:</strong> ${
        detailYeuCau.khachHangDS ? detailYeuCau.khachHangDS.sdt : "N/A"
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
                      <td>${
                        detail.giaGioThueVM1
                          ? detail.giaGioThueVM1.dongia
                          : "N/A"
                      }</td>
                      <td>${detail.trangThai}</td>
                      <td class="d-flex justify-content-start">${
                        detail.trangThai === "Chờ xác nhận"
                          ? `<button style="display: none;" data-auth="yeucau_edit,hoadon_add" class="btn btn-success btn-sm" 
                                  onclick="confirmAndAddPds('${detail.stt}', '${detail.maSb}', '${detail.magio}', '${detail.ngaysudung}', 'Đã xác nhận', '${detailYeuCau.maKh}')">
                              Xác nhận
                          </button>
                          <button style="display: none;" data-auth="yeucau_edit" class="btn btn-danger btn-sm ml-2"
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
    
    // Gọi hàm xử lý quyền
    await handlePermissionsForFrontend();
  } catch (error) {
    console.error("Lỗi:", error);
  }
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
            ghichu: "",
          },
        ],
      };

      const addPdsResponse = await fetch(addPdsUrl, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
          Role: "NhanVienDS", // Truyền role vào header
          "Content-Type": "application/json",
        },
        body: JSON.stringify(pdsData),
      });

      if (!addPdsResponse.ok) {
        // Nếu response không thành công, lấy thông báo lỗi từ API (nếu có)
        const errorData = await addPdsResponse.json(); // Nếu API trả về JSON chứa thông báo
        const errorMessage =
          errorData?.message || "Không thể tải danh sách yêu cầu";

        // Hiển thị thông báo warning với Toastr
        alert(errorMessage);

        return;
      }
    }

    const updateResponse = await fetch(updateUrl, {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${token}`,
        Role: "NhanVienDS", // Truyền role vào header
        "Content-Type": "application/json",
      },
      body: JSON.stringify(requestData),
    });

    if (!updateResponse.ok) {
      // Nếu response không thành công, lấy thông báo lỗi từ API (nếu có)
      const errorData = await updateResponse.json(); // Nếu API trả về JSON chứa thông báo
      const errorMessage = errorData?.message || "Không thể tạo hóa đơn";

      // Hiển thị thông báo warning với Toastr
      alert(errorMessage);

      return;
    }
    alert("Xử lý yêu cầu thành công!");

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
