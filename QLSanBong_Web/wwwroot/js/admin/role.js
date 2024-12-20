﻿// Bắt đầu quá trình tải vai trò khi trang được tải
document.addEventListener('DOMContentLoaded', loadRoles);

  // Xóa thông tin trong các trường khi modal đóng
  $("#addRoleModal").on("hidden.bs.modal", function () {
    const form = document.getElementById("addRoleForm");
    const errorMessage = document.getElementById("ermessage");
    form.reset(); // Reset form
    errorMessage.style.display = "none"; // Ẩn thông báo lỗi
  });

  // Hàm thêm vai trò
  async function addRole() {
    const rolename = document.getElementById("rolename").value;
    const roleinfo = document.getElementById("roleinfo").value;
    const errorMessage = document.getElementById("ermessage");

    // Kiểm tra các trường không được để trống
    if (!rolename || !roleinfo) {
      errorMessage.textContent = "Vui lòng điền đầy đủ thông tin.";
      errorMessage.style.display = "block";
      return;
    }

    // Gọi API để thêm vai trò
    try {
      const response = await fetch(`https://localhost:7182/api/Role/addrole`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Role: currentRole, // Truyền role vào header
          Authorization: `Bearer ${sessionStorage.getItem("Token")}`,
        },
        body: JSON.stringify({
          roleName: rolename,
          thongTin: roleinfo,
        }),
      });

      if (response.ok) {
        alert("Thêm vai trò thành công!");
        $("#addRoleModal").modal("hide"); // Đóng modal

        // Hủy khởi tạo DataTable nếu đã có
        if ($.fn.DataTable.isDataTable("#roleTable")) {
          $("#roleTable").DataTable().destroy();
        }

        loadRoles(); // Tải lại danh sách vai trò
      } else {
        let errorMessageText = "Có lỗi xảy ra."; // Thông điệp mặc định
        const result = await response.text();

        if (result.includes("Role đã tồn tại.")) {
          errorMessageText = "Role đã tồn tại.";
        } else {
          errorMessageText = result;
        }

        errorMessage.textContent = errorMessageText;
        errorMessage.style.display = "block";
      }
    } catch (error) {
      errorMessage.textContent = "Có lỗi xảy ra khi thêm vai trò.";
      errorMessage.style.display = "block";
    }
  }

  // Hàm tải danh sách vai trò từ API
  async function loadRoles() {
    const url = `https://localhost:7182/api/Role/getallrole`;
    try {
      const token = sessionStorage.getItem("Token");
      const response = await fetch(url, {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
          Role: currentRole,
          "Content-Type": "application/json",
        },
      });

      if (!response.ok) {
        throw new Error("Không thể tải danh sách vai trò.");
      }

      const roles = await response.json();
      const roleList = document.getElementById("roleList");

      if (!roleList) {
        throw new Error(
          "Không tìm thấy phần tử để hiển thị danh sách vai trò."
        );
      }

      roleList.innerHTML = ""; // Xóa nội dung trước đó

      roles.forEach((role) => {
        roleList.innerHTML += `
                <tr>
                    <td>${role.roleID}</td>
                    <td>${role.roleName}</td>
                    <td>${role.thongTin}</td>
                    <td class="d-flex justify-content-start">
                        <button class="btn btn-info btn-sm" onclick="infoRole('${role.roleID}')"><i class="fas fa-users"></i> Người dùng</button>
                        <button  style="display: none;" data-auth="phanquyen_read" class="btn btn-primary btn-sm ml-1" onclick="authRole('${role.roleID}')"><i class="fas fa-atom"></i> Quyền</button>
                        <button  style="display: none;" data-auth="vaitro_delete" class="btn btn-danger btn-sm ml-1" onclick="deleteRole('${role.roleID}')"><i class="fas fa-trash"></i> Xóa vai trò</button>
                    </td>
                </tr>
            `;
      });

      await handlePermissionsForFrontend();
      // Hủy khởi tạo DataTable nếu đã có
      if ($.fn.DataTable.isDataTable("#roleTable")) {
        $("#roleTable").DataTable().clear().destroy();
      }

      // Khởi tạo DataTable sau khi thêm dữ liệu vào bảng
      $("#roleTable").DataTable({
        paging: true,
        ordering: true,
        info: true,
        searching: true,
        language: {
          paginate: {
            next: "Trang sau",
            previous: "Trang trước",
          },
          info: "Hiển thị từ _START_ đến _END_ của _TOTAL_ vai trò",
          search: "Tìm kiếm:",
        },
      });
    } catch (error) {
      console.error("Lỗi:", error);
      document.getElementById(
        "roleContainer"
      ).innerHTML = `<p class="text-danger text-center">${error.message}</p>`;
    }
  }

  // Hàm xóa vai trò
  async function deleteRole(roleId) {
    const confirmation = confirm("Bạn có chắc chắn muốn xóa vai trò này?");
    if (confirmation) {
      const response = await fetch(
        `https://localhost:7182/api/Role/deleterole?roleId=${roleId}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${sessionStorage.getItem("Token")}`,
            Role: currentRole, // currentRole phải được xác định ở đâu đó trong mã của bạn
          },
        }
      );

      if (response.ok) {
        alert("Xóa vai trò thành công!");

        // Hủy khởi tạo DataTable trước đó nếu có
        if ($.fn.DataTable.isDataTable("#roleTable")) {
          $("#roleTable").DataTable().destroy();
        }

        await loadRoles(); // Tải lại danh sách vai trò
      } else {
        alert("Có lỗi xảy ra khi xóa vai trò.");
      }
    }
  }

// Hàm mở modal và lấy dữ liệu từ API
async function authRole(roleId) {
    const roleApiUrl = `https://localhost:7182/api/Role/getauthbyrole?roleId=${roleId}`;
    const authApiUrl = `https://localhost:7182/api/Auth/getallactionservice`; // Lấy tất cả quyền từ API
    const actionsApiUrl = `https://localhost:7182/api/Auth/getallaction`;
    const servicesApiUrl = `https://localhost:7182/api/Auth/getallservice`;

    const modal = document.getElementById("authModal");
    modal.setAttribute("data-roleid", roleId);
    $("#authModal").modal("show");

    const tableContainer = document.getElementById("authTableContainer");
    if (tableContainer) {
        tableContainer.innerHTML = "";
    }

    try {
        const [roleResponse, authResponse, actionsResponse, servicesResponse] =
            await Promise.all([
                fetch(roleApiUrl),
                fetch(authApiUrl),
                fetch(actionsApiUrl),
                fetch(servicesApiUrl),
            ]);

        if (!roleResponse.ok) {
            console.warn("Không tìm thấy dữ liệu quyền cho roleId:", roleId);
            renderAuthList([], [], [], []); // Trả về mảng rỗng nếu không có dữ liệu
            return;
        }

        const roleData = await roleResponse.json();
        const allAuths = await authResponse.json();
        const allActions = await actionsResponse.json();
        const allServices = await servicesResponse.json();

        renderAuthList(allAuths, roleData, allActions, allServices);
    } catch (error) {
        console.error("Error fetching data:", error);
    }
}

async function renderAuthList(allAuths, roleAuths, actions, services) {
    const authListContainer = document.createElement("div");
    authListContainer.className = "auth-list-container";

    // Tạo một map để tra cứu tên hành động và dịch vụ nhanh chóng
    const actionMap = actions.reduce((map, action) => {
        map[action.actionId] = action.actionName; // Ví dụ: { add: "Thêm", edit: "Sửa" }
        return map;
    }, {});

    const serviceMap = services.reduce((map, service) => {
        map[service.serviceId] = service.serviceName; // Ví dụ: { hoadon: "Hóa đơn" }
        return map;
    }, {});

    // Tạo một map để kiểm tra các quyền mà role có
    const roleAuthMap = new Set(roleAuths.map(auth => `${auth.authInfo.serviceId}-${auth.authInfo.actionId}`));

    // Tạo một đối tượng để nhóm các quyền theo serviceId
    const groupedAuths = {};

    // Lặp qua tất cả các quyền và nhóm theo serviceId
    allAuths.forEach((auth) => {
        const serviceName = serviceMap[auth.serviceId];
        const actionName = actionMap[auth.actionId];

        if (serviceName && actionName) {
            const serviceId = auth.serviceId;
            const actionId = auth.actionId;

            // Kiểm tra xem role có quyền này không
            const hasRoleAuth = roleAuthMap.has(`${serviceId}-${actionId}`);

            // Nếu serviceId chưa có trong groupedAuths, tạo một mục mới
            if (!groupedAuths[serviceId]) {
                groupedAuths[serviceId] = {
                    serviceName,
                    actions: []
                };
            }

            // Thêm quyền vào nhóm tương ứng với serviceId
            groupedAuths[serviceId].actions.push({
                actionId,
                actionName,
                hasRoleAuth
            });
        }
    });

    // Lặp qua các dịch vụ trong groupedAuths để hiển thị
    Object.values(groupedAuths).forEach(({ serviceName, actions, serviceId ,actionId}) => {
        const serviceSection = document.createElement("div");
        serviceSection.className = "service-section";

        // Tạo tiêu đề cho dịch vụ với biểu tượng mũi tên
        const serviceTitle = document.createElement("p");
        serviceTitle.classList.add("service-title");

        // Tạo mũi tên để mở/thu
        const arrowIcon = document.createElement("span");
        arrowIcon.classList.add("arrow-icon");
        arrowIcon.textContent = "▼"; // Mũi tên xuống ban đầu
        serviceTitle.appendChild(arrowIcon);

        const serviceLabel = document.createElement("span");
        serviceLabel.textContent = serviceName; // Hiển thị tên dịch vụ
        serviceTitle.appendChild(serviceLabel);

        serviceSection.appendChild(serviceTitle);

        // Tạo danh sách quyền cho mỗi dịch vụ
        const actionsList = document.createElement("ul");
        actionsList.className = "list-group actions-list hidden"; // Danh sách quyền sẽ ẩn ban đầu

        // Tạo item cho từng quyền trong dịch vụ
        actions.forEach(({ actionId, actionName, hasRoleAuth }) => {
            const actionItem = document.createElement("li");
            actionItem.className = "list-group-item d-flex justify-content-between align-items-center";

            // Tạo checkbox cho mỗi action
            const checkbox = document.createElement("input");
            checkbox.type = "checkbox";
            checkbox.className = "action-service-checkbox";
            checkbox.dataset.serviceId = serviceId;
            checkbox.dataset.actionId = actionId;

            checkbox.checked = hasRoleAuth; // Nếu có quyền role thì checked = true

            // Lưu trạng thái ban đầu vào data-initial-checked
            checkbox.dataset.initialChecked = checkbox.checked;

            // Tạo tên quyền (action)
            const actionLabel = document.createElement("span");
            actionLabel.textContent = actionName; // Ví dụ: "Thêm", "Sửa"

            // Thêm checkbox và tên quyền vào phần tử li
            actionItem.appendChild(checkbox);
            actionItem.appendChild(actionLabel);

            // Thêm item vào danh sách
            actionsList.appendChild(actionItem);
        });

        // Thêm danh sách actions vào section của dịch vụ
        serviceSection.appendChild(actionsList);

        // Thêm sự kiện click để toggle (thu/mở) danh sách quyền và thay đổi biểu tượng mũi tên
        serviceTitle.addEventListener("click", () => {
            actionsList.classList.toggle("hidden");
            // Chuyển đổi mũi tên
            if (actionsList.classList.contains("hidden")) {
                arrowIcon.textContent = "▼"; // Mũi tên xuống khi ẩn
            } else {
                arrowIcon.textContent = "▲"; // Mũi tên lên khi hiển thị
            }
        });

        // Thêm section của dịch vụ vào container
        authListContainer.appendChild(serviceSection);
    });

    // Chèn vào container chính trong modal
    const tableContainer = document.getElementById("authTableContainer");
    if (tableContainer) {
        tableContainer.innerHTML = ""; // Xóa nội dung cũ của bảng
        tableContainer.appendChild(authListContainer);
    }
}
    


  async function saveAuth() {
    const modal = document.getElementById("authModal");
    const roleId = modal.getAttribute("data-roleid"); // Lấy roleId từ modal
    const checkboxes = document.querySelectorAll(".action-service-checkbox");
    const promises = []; // Lưu trữ các lời hứa fetch API

    checkboxes.forEach((checkbox) => {
      const serviceId = checkbox.dataset.serviceId; // Lấy serviceId từ dataset
      const actionId = checkbox.dataset.actionId; // Lấy actionId từ dataset

      // Kiểm tra sự thay đổi so với trạng thái ban đầu
      if (checkbox.checked !== (checkbox.dataset.initialChecked === "true")) {
        // Gọi API để lấy authId dựa trên serviceId và actionId
        promises.push(
          fetch(
            `https://localhost:7182/api/Auth/getauth?serviceId=${serviceId}&actionId=${actionId}`
          )
            .then((response) => {
              if (!response.ok) {
                console.warn(
                  "Không tìm thấy authId cho dịch vụ và hành động này:",
                  serviceId,
                  actionId
                );
                return null; // Nếu không tìm thấy authId
              }
              return response.json();
            })
            .then((authData) => {
              if (authData && authData.authId) {
                const authId = authData.authId; // Lấy authId từ dữ liệu API

                // Nếu trạng thái thay đổi, thực hiện gọi API thêm hoặc xóa quyền
                if (checkbox.checked) {
                  // Nếu được tick, gọi API thêm quyền
                  return fetch(
                    `https://localhost:7182/api/Role/addroleauth?roleId=${roleId}&authId=${authId}`,
                    {
                      method: "POST",
                    }
                  );
                } else {
                  // Nếu bỏ tick, gọi API xóa quyền
                  return fetch(
                    `https://localhost:7182/api/Role/deleteroleauth?roleId=${roleId}&authId=${authId}`,
                    {
                      method: "DELETE",
                    }
                  );
                }
              } else {
                return null; // Nếu không có authId, không thực hiện gì cả
              }
            })
        );
      }
    });

    // Thực hiện các lời hứa
    Promise.all(promises)
      .then(() => {
        alert("Cập nhật quyền thành công!");
        $("#authModal").modal("show"); // Đóng modal sau khi lưu
      })
      .catch((error) => {
        console.error("Error updating role permissions:", error);
        alert("Có lỗi xảy ra khi cập nhật quyền!");
      });
  }

  // Hàm xử lý khi nhấn vào thông tin vai trò
  async function infoRole(roleId) {
    loadRoleDetail(roleId);
  }

  let currentID = [];
  // Hàm tải chi tiết vai trò
  async function loadRoleDetail(roleId) {
    const url = `https://localhost:7182/api/Role/getuserbyrole?roleId=${roleId}`;
    try {
      currentID = roleId;
      const token = sessionStorage.getItem("Token");
      const response = await fetch(url, {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      });

      if (!response.ok) {
        throw new Error("Không thể tải thông tin chi tiết vai trò.");
      }

      const detailRole = await response.json();
      populateDetailModal(detailRole);
    } catch (error) {
      console.error("Lỗi:", error);
    }
  }

  async function populateDetailModal(detailRole) {
    const modalContent = document.getElementById("modalContent");
    if (!modalContent) {
      console.error("Không tìm thấy phần tử modalContent");
      return;
    }

    // Tạo nội dung cho modal
    let content = `
        <table class="table table-bordered">
            <thead>
                <tr>
                    <th>Username</th>
                    <th>Tên người dùng</th>
                    <th>SDT</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>`;

    // Kiểm tra và lặp qua danh sách detailRole
    if (Array.isArray(detailRole) && detailRole.length > 0) {
      detailRole.forEach((detail) => {
        content += `
                <tr>
                    <td>${detail.username || "N/A"}</td>
                    <td>${detail.name || "N/A"}</td>
                    <td>${detail.sdt}</td>
                    <td>
                        <button class="btn btn-danger btn-sm" onclick="deleteUserFromRole('${
                          detail.userID
                        }')">
                            <i class="fas fa-trash-alt"></i> Xóa người dùng
                        </button>
                    </td>
                </tr>`;
      });
    } else {
      content += `<tr><td colspan="6" class="text-center">Không có chi tiết nào.</td></tr>`;
    }

    content += `</tbody></table>`;
    modalContent.innerHTML = content;

    // Mở modal
    $("#roleModal").modal("show");
  }

  // Hàm xóa vai trò của người dùng
  async function deleteUserFromRole(userID) {
    // Hiển thị cảnh báo xác nhận trước khi xóa
    const confirmation = confirm("Bạn có chắc chắn muốn xóa người này?");
    if (!confirmation) {
      return; // Nếu người dùng không xác nhận, không thực hiện hành động xóa
    }

    const url = `https://localhost:7182/api/Role/deleteuserrole`;
    try {
      const token = sessionStorage.getItem("Token");
      const response = await fetch(url, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ userID, roleID: currentID }), // Chuyển thông tin cần thiết vào body
      });

      if (!response.ok) throw new Error("Không thể xóa vai trò này.");

      alert("Xóa vai trò thành công.");

      loadRoleDetail(currentID);
      // Thực hiện lại thao tác như cập nhật UI nếu cần
    } catch (error) {
      console.error("Lỗi khi xóa vai trò:", error);
      alert("Không thể xóa vai trò.");
    }
  }

  $(".modalclose").on("click", function () {
    $("#roleModal").modal("hide"); // Đóng tất cả các modal
    $("#authModal").modal("hide"); // Đóng tất cả các modal
    $("#addRoleModal").modal("hide"); // Đóng tất cả các modal
  });

