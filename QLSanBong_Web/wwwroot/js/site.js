document.addEventListener("DOMContentLoaded", function () {
    // Kiểm tra token ngay lập tức
    const token = sessionStorage.getItem("Token");
    if (!token) {
        sessionStorage.removeItem("currentRole");  // Xóa currentRole khỏi sessionStorage
        console.log("Token không tồn tại, currentRole đã bị xóa.");
    }
    // Hàm lấy roles từ token
    function getRolesFromToken(token) {
        try {
            const payload = JSON.parse(atob(token.split(".")[1])); // Giải mã payload của JWT
            const roles = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
            // Nếu roles là chuỗi, chuyển thành mảng; nếu đã là mảng, trả về trực tiếp
            return Array.isArray(roles) ? roles : [roles];
        } catch (error) {
            console.error("Token không hợp lệ:", error);
            return []; // Trả về mảng rỗng nếu lỗi
        }
    }

    // Hàm trả về URL tương ứng với từng role
    function getRoleUrl(role) {
        // Kiểm tra nếu role là mảng và không rỗng
        if (Array.isArray(role) && role.length === 0) {
            return "/";
        } else if (Array.isArray(role)) {
            // Kiểm tra nếu role là mảng và có giá trị
            return "/";
        } else if (role === "KhachHang") {
            // Kiểm tra nếu role là "KhachHang"
            return "/Customer";
        } else {
            // Trả về "/Admin" cho các trường hợp còn lại
            return "/Admin";
        }
    }

    // Hàm trả về tên hiển thị của vai trò
function getRoleDisplayName(role) {
    // Kiểm tra nếu role là mảng
    if (Array.isArray(role)) {
        return "Trang chủ";  // Nếu role là mảng, trả về "Trang chủ"
    }

    // Nếu không phải mảng, tiếp tục kiểm tra giá trị của role
    switch (role) {
        case "Admin":
            return "Quản trị hệ thống";
        case "NhanVienDS":
            return "Nhân viên đặt sân";
        case "KhachHang":
            return "Khách hàng";
        case null:
            return "Trang chủ";  // Nếu role là null, trả về "Trang chủ"
        default:
            return role ;
    }
}


    // Lấy currentRole từ sessionStorage (nếu có)
    let currentRole = sessionStorage.getItem("currentRole");

    // Nếu không có currentRole trong sessionStorage, lấy từ token hoặc mặc định là Admin
    if (!currentRole && token) {
        const roles = getRolesFromToken(token);
        currentRole = roles.length > 0 ? roles[0] : "Admin"; // Chọn vai trò đầu tiên nếu có
        sessionStorage.setItem("currentRole", currentRole); // Lưu vào sessionStorage
    }

    // Lấy roles từ token và lọc bỏ vai trò hiện tại
    const roles = token ? getRolesFromToken(token).filter(role => role !== currentRole) : [];

    // Xử lý menu thả xuống
    const dropdownMenu = document.querySelector(".dropdown-menu-role");
    const dropdownLi = document.querySelector(".nav-item.chooserole"); // Chọn thẻ <li>

    if (dropdownLi) {
        // Tạo phần tử cho vai trò hiện tại
        if (currentRole) {
            const currentRoleSpan = document.createElement("span");
            currentRoleSpan.classList.add("dropdown-item", "text-muted", "font-weight-bold");
            currentRoleSpan.textContent = getRoleDisplayName(currentRole);
            dropdownMenu.appendChild(currentRoleSpan);
        }

        // Nếu có các vai trò khác, thêm ngăn cách và các liên kết cho các vai trò còn lại
        if (roles.length > 0) {
            const divider = document.createElement("div");
            divider.classList.add("dropdown-divider");
            dropdownMenu.appendChild(divider);

            // Tạo các liên kết cho các vai trò còn lại
            roles.forEach((role) => {
                const link = document.createElement("a");
                link.classList.add("dropdown-item");
                link.href = getRoleUrl(role);
                link.innerHTML = `<i class="fas fa-user fa-sm fa-fw mr-2 text-gray-400"></i> ${getRoleDisplayName(role)}`;
                link.addEventListener("click", function (event) {
                    event.preventDefault(); // Ngăn chặn việc chuyển trang ngay lập tức
                    sessionStorage.setItem("currentRole", role); // Lưu vai trò mới vào sessionStorage
                    console.log("Đã chọn vai trò:", role); // In ra console vai trò đã chọn
                    location.href = getRoleUrl(role); // Chuyển hướng đến URL tương ứng
                });
                dropdownMenu.appendChild(link);
            });
        }
    }
});
