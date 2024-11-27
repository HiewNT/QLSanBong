

// Bắt đầu quá trình tải vai trò khi trang được tải
document.addEventListener('DOMContentLoaded', loadRoles);

// Mở modal thêm vai trò
function openAddRoleModal() {
    $('#addRoleModal').modal('show');
}

// Xóa thông tin trong các trường khi modal đóng
$('#addRoleModal').on('hidden.bs.modal', function () {
    const form = document.getElementById("addRoleForm");
    const errorMessage = document.getElementById("ermessage");
    form.reset(); // Reset form
    errorMessage.style.display = 'none'; // Ẩn thông báo lỗi
});

// Hàm thêm vai trò
async function addRole() {
    const rolename = document.getElementById("rolename").value;
    const roleinfo = document.getElementById("roleinfo").value;
    const errorMessage = document.getElementById("ermessage");

    // Kiểm tra các trường không được để trống
    if (!rolename || !roleinfo) {
        errorMessage.textContent = "Vui lòng điền đầy đủ thông tin.";
        errorMessage.style.display = 'block';
        return;
    }

    // Gọi API để thêm vai trò
    try {
        const response = await fetch(`https://localhost:7182/api/Role/addrole`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
        'Role': 'Admin',  // Truyền role vào header
                'Authorization': `Bearer ${sessionStorage.getItem("Token")}`
            },
            body: JSON.stringify({
                roleName: rolename,
                thongTin: roleinfo
            })
        });

        if (response.ok) {
            alert("Thêm vai trò thành công!");
            $('#addRoleModal').modal('hide'); // Đóng modal

            // Hủy khởi tạo DataTable nếu đã có
            if ($.fn.DataTable.isDataTable('#roleTable')) {
                $('#roleTable').DataTable().destroy();
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
            errorMessage.style.display = 'block';
        }
    } catch (error) {
        errorMessage.textContent = "Có lỗi xảy ra khi thêm vai trò.";
        errorMessage.style.display = 'block';
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
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });

        if (!response.ok) {
            throw new Error("Không thể tải danh sách vai trò.");
        }

        const roles = await response.json();
        const roleList = document.getElementById('roleList');

        if (!roleList) {
            throw new Error("Không tìm thấy phần tử để hiển thị danh sách vai trò.");
        }

        roleList.innerHTML = ''; // Xóa nội dung trước đó

        roles.forEach(role => {
            roleList.innerHTML += `
                <tr>
                    <td>${role.roleID}</td>
                    <td>${role.roleName}</td>
                    <td>${role.thongTin}</td>
                    <td>
                        <button class="btn btn-info btn-sm" onclick="infoRole('${role.roleID}')"><i class="fas fa-users"></i> Người dùng</button>
                        <button class="btn btn-primary btn-sm" onclick="authRole('${role.roleID}')"><i class="fas fa-atom"></i> Quyền</button>
                        <button class="btn btn-danger btn-sm" onclick="deleteRole('${role.roleID}')"><i class="fas fa-trash"></i> Xóa vai trò</button>
                    </td>
                </tr>
            `;
        });

        // Hủy khởi tạo DataTable nếu đã có
        if ($.fn.DataTable.isDataTable('#roleTable')) {
            $('#roleTable').DataTable().destroy();
        }

        // Khởi tạo DataTable sau khi thêm dữ liệu vào bảng
        $('#roleTable').DataTable({
            "paging": true,
            "ordering": true,
            "info": true,
            "searching": true,
            "language": {
                "paginate": {
                    "next": "Trang sau",
                    "previous": "Trang trước"
                },
                "info": "Hiển thị từ _START_ đến _END_ của _TOTAL_ vai trò",
                "search": "Tìm kiếm:"
            }
        });
    } catch (error) {
        console.error("Lỗi:", error);
        document.getElementById('roleContainer').innerHTML = `<p class="text-danger text-center">${error.message}</p>`;
    }
}

// Hàm xóa vai trò
async function deleteRole(roleId) {
    const confirmation = confirm("Bạn có chắc chắn muốn xóa vai trò này?");
    if (confirmation) {
        const response = await fetch(`https://localhost:7182/api/Role/deleterole?roleId=${roleId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${sessionStorage.getItem("Token")}`
            }
        });

        if (response.ok) {
            alert("Xóa vai trò thành công!");

            // Hủy khởi tạo DataTable trước đó nếu có
            if ($.fn.DataTable.isDataTable('#roleTable')) {
                $('#roleTable').DataTable().destroy();
            }

            await loadRoles(); // Tải lại danh sách vai trò
        } else {
            alert("Có lỗi xảy ra khi xóa vai trò.");
        }
    }
}


// Hàm mở modal và lấy dữ liệu từ API
function authRole(roleId) {
    const roleApiUrl = `https://localhost:7182/api/Role/getauthbyrole?roleId=${roleId}`;
    const actionsApiUrl = `https://localhost:7182/api/Auth/getallaction`;
    const servicesApiUrl = `https://localhost:7182/api/Auth/getallservice`;

    // Mở modal và gán roleId vào thuộc tính data-roleid
    const modal = document.getElementById('authModal');
    modal.setAttribute('data-roleid', roleId);
    // Mở modal
    $('#authModal').modal('show');

    // Xóa bảng cũ trước khi hiển thị bảng mới
    const tableContainer = document.getElementById('authTableContainer');
    if (tableContainer) {
        tableContainer.innerHTML = ''; // Xóa nội dung cũ của bảng
    }

    // Gọi API để lấy dữ liệu
    Promise.all([
        fetch(roleApiUrl)
            .then(response => {
                if (!response.ok) {
                    console.warn('Không tìm thấy dữ liệu quyền cho roleId:', roleId);
                    return []; // Nếu không tìm thấy, trả về danh sách rỗng
                }
                return response.json();
            }),
        fetch(actionsApiUrl)
            .then(response => response.json()),
        fetch(servicesApiUrl)
            .then(response => response.json())
    ])
        .then(([roleData, allActions, allServices]) => {
            renderAuthTable(roleData || [], allActions, allServices);
        })
        .catch(error => console.error('Error fetching data:', error));
}

// Hàm render bảng dữ liệu vào trong modal
function renderAuthTable(auths, actions, services) {
    const authTable = document.createElement('table');
    authTable.className = 'table table-bordered';
    authTable.id = 'authTable';

    // Tạo phần tiêu đề
    const thead = document.createElement('thead');
    const headerRow = document.createElement('tr');

    // Cột đầu tiên: Dịch vụ
    const serviceHeader = document.createElement('th');
    serviceHeader.textContent = 'Dịch vụ';
    headerRow.appendChild(serviceHeader);

    // Thêm tiêu đề cho mỗi action
    actions.forEach(action => {
        const actionHeader = document.createElement('th');
        actionHeader.textContent = action.actionName;
        headerRow.appendChild(actionHeader);
    });

    thead.appendChild(headerRow);
    authTable.appendChild(thead);

    // Tạo phần thân bảng
    const tbody = document.createElement('tbody');

    services.forEach(service => {
        const row = document.createElement('tr');

        // Cột dịch vụ
        const serviceCell = document.createElement('td');
        serviceCell.textContent = service.serviceName; // Hiển thị tên dịch vụ
        row.appendChild(serviceCell);

        // Tạo checkbox cho mỗi action
        actions.forEach(action => {
            const cell = document.createElement('td');
            const checkbox = document.createElement('input');
            checkbox.type = 'checkbox';
            checkbox.className = 'action-service-checkbox';

            // Gán dữ liệu serviceId và actionId vào checkbox để sử dụng trong saveAuth
            checkbox.dataset.serviceId = service.serviceId;
            checkbox.dataset.actionId = action.actionId;

            // Kiểm tra xem role có quyền cho action-service này không
            const hasAuth = auths.some(
                auth =>
                    auth.authInfo.serviceId === service.serviceId && // So sánh bằng serviceId
                    auth.authInfo.actionId === action.actionId // So sánh bằng actionId
            );

            checkbox.checked = hasAuth;

            // Lưu trạng thái ban đầu vào data-initial-checked
            checkbox.dataset.initialChecked = checkbox.checked;

            cell.appendChild(checkbox);
            row.appendChild(cell);
        });

        tbody.appendChild(row);
    });

    authTable.appendChild(tbody);

    // Chèn bảng vào container
    const tableContainer = document.getElementById('authTableContainer');
    if (tableContainer) {
        tableContainer.appendChild(authTable);
    }
}

function saveAuth() {
    const modal = document.getElementById('authModal');
    const roleId = modal.getAttribute('data-roleid'); // Lấy roleId từ modal
    const checkboxes = document.querySelectorAll(".action-service-checkbox");
    const promises = []; // Lưu trữ các lời hứa fetch API

    checkboxes.forEach(checkbox => {
        const serviceId = checkbox.dataset.serviceId; // Lấy serviceId từ dataset
        const actionId = checkbox.dataset.actionId; // Lấy actionId từ dataset

        // Kiểm tra sự thay đổi so với trạng thái ban đầu
        if (checkbox.checked !== (checkbox.dataset.initialChecked === 'true')) {
            // Gọi API để lấy authId dựa trên serviceId và actionId
            promises.push(
                fetch(`https://localhost:7182/api/Auth/getauth?serviceId=${serviceId}&actionId=${actionId}`)
                    .then(response => {
                        if (!response.ok) {
                            console.warn('Không tìm thấy authId cho dịch vụ và hành động này:', serviceId, actionId);
                            return null; // Nếu không tìm thấy authId
                        }
                        return response.json();
                    })
                    .then(authData => {
                        if (authData && authData.authId) {
                            const authId = authData.authId; // Lấy authId từ dữ liệu API

                            // Nếu trạng thái thay đổi, thực hiện gọi API thêm hoặc xóa quyền
                            if (checkbox.checked) {
                                // Nếu được tick, gọi API thêm quyền
                                return fetch(`https://localhost:7182/api/Role/addroleauth?roleId=${roleId}&authId=${authId}`, {
                                    method: 'POST'
                                });
                            } else {
                                // Nếu bỏ tick, gọi API xóa quyền
                                return fetch(`https://localhost:7182/api/Role/deleteroleauth?roleId=${roleId}&authId=${authId}`, {
                                    method: 'DELETE'
                                });
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
            alert('Cập nhật quyền thành công!');
            $('#authModal').modal('show'); // Đóng modal sau khi lưu
        })
        .catch(error => {
            console.error('Error updating role permissions:', error);
            alert('Có lỗi xảy ra khi cập nhật quyền!');
        });
}



// Hàm xử lý khi nhấn vào thông tin vai trò
function infoRole(roleId) {
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
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
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

// Hàm điền dữ liệu vào modal chi tiết hóa đơn
function populateDetailModal(detailRole) {
    const modalContent = document.getElementById('modalContent');
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
        detailRole.forEach(detail => {
            content += `
                <tr>
                    <td>${detail.username || 'N/A'}</td>
                    <td>${detail.name || 'N/A'}</td>
                    <td>${detail.sdt }</td>
                    <td>
                        <button class="btn btn-danger btn-sm" onclick="deleteUserFromRole('${detail.userID}')">
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
    $('#roleModal').modal('show');
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
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ userID, roleID: currentID }) // Chuyển thông tin cần thiết vào body
        });

        if (!response.ok) throw new Error('Không thể xóa vai trò này.');

        alert('Xóa vai trò thành công.');

        loadRoleDetail(currentID);
        // Thực hiện lại thao tác như cập nhật UI nếu cần
    } catch (error) {
        console.error('Lỗi khi xóa vai trò:', error);
        alert('Không thể xóa vai trò.');
    }
}
$(".modalclose").on("click", function () {
    $("#roleModal").modal("hide");  // Đóng tất cả các modal
    $("#authModal").modal("hide");  // Đóng tất cả các modal
    $("#addRoleModal").modal("hide");  // Đóng tất cả các modal
});
