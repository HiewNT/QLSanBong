document.addEventListener("DOMContentLoaded", function () {
    // Kiểm tra token ngay lập tức
    const token = sessionStorage.getItem("Token");

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
        switch (role) {
            case "Admin":
                return "/Admin";
            case "NhanVienDS":
                return "/Employee";
            case "KhachHang":
                return "/Customer";
            default:
                return "/";
        }
    }

    // Hàm trả về tên hiển thị của vai trò
    function getRoleDisplayName(role) {
        switch (role) {
            case "Admin":
                return "Quản trị hệ thống";
            case "NhanVienDS":
                return "Nhân viên đặt sân";
            case "KhachHang":
                return "Khách hàng";
            default:
                return "Trang chủ";
        }
    }

    // Hàm xác định vai trò hiện tại từ URL
    function getCurrentRoleFromUrl(path) {
        if (path.startsWith("/Admin")) return "Admin";
        if (path.startsWith("/Employee")) return "NhanVienDS";
        if (path.startsWith("/Customer")) return "KhachHang";
        return "";
    }

    // Lấy roles từ token và lọc bỏ vai trò hiện tại
    const roles = token ? getRolesFromToken(token).filter(role => role !== getCurrentRoleFromUrl(window.location.pathname)) : [];

    // Xử lý menu thả xuống
    const dropdownMenu = document.querySelector(".dropdown-menu-role");
    const dropdownLi = document.querySelector(".nav-item.chooserole"); // Chọn thẻ <li>

    if (dropdownLi) {
        // Tạo phần tử cho vai trò hiện tại
        const currentRole = getCurrentRoleFromUrl(window.location.pathname);
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
                dropdownMenu.appendChild(link);
            });
        }
    }
});
