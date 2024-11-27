// Lấy token từ sessionStorage
const token = sessionStorage.getItem("Token"); // Hàm xử lý quyền cho phần tử
async function handlePermissionsForFrontend() {
  if (!token) {
    console.error("Không tìm thấy token.");
    return;
  }

  // Xác định vai trò hiện tại từ URL
  const role = getCurrentRoleFromUrl(window.location.pathname);
  if (!role) {
    console.error("Không xác định được vai trò.");
    return;
  }

  // Lấy quyền từ API
  const permissions = await getPermissionsFromApi(token, role);

  // Xử lý quyền cho các phần tử có data-auth
  document.querySelectorAll("[data-auth]").forEach((el) => {
    const requiredPermissions = el.getAttribute("data-auth").split(","); // Lấy các quyền yêu cầu từ data-auth

    const hasPermission = requiredPermissions.some((permission) =>
      permissions.includes(permission)
    );
    // Ẩn/hiện phần tử dựa trên quyền
    if (hasPermission) {
      el.style.display = "block"; // Hiển thị phần tử nếu có quyền
    } else {
      el.style.display = "none"; // Ẩn phần tử nếu không có quyền
    }
  });
}

// Hàm xác định vai trò hiện tại từ URL
function getCurrentRoleFromUrl(path) {
  if (path.startsWith("/Admin")) return "Admin";
  if (path.startsWith("/Employee")) return "NhanVienDS";
  if (path.startsWith("/Customer")) return "KhachHang";
  return "";
}

// Hàm lấy quyền từ API dựa trên vai trò
async function getPermissionsFromApi(token, role) {
  try {
    const response = await fetch(
      "https://localhost:7182/api/Account/userinfo",
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      }
    );

    const data = await response.json();
    const permissions = data.permissions[role] || [];
    return permissions.length > 0 ? permissions[0].split(",") : [];
  } catch (error) {
    console.error("Lỗi khi lấy quyền từ API:", error);
    return [];
  }
}

// Gọi hàm xử lý quyền khi trang được tải
document.addEventListener("DOMContentLoaded", function () {
handlePermissionsForFrontend();
});
