document.addEventListener("DOMContentLoaded", function () {
    // Lấy các phần tử cần thiết
    const loginForm = document.getElementById("loginForm");
    const addKHButton = document.getElementById("addKH");
    const addKHModal = document.getElementById("addKHModal");
    const errorMessageLogin = document.getElementById("errorMessage");
    const errorMessageSignup = document.getElementById("errorMessagesignup");

    // Lắng nghe sự kiện submit của form login
    loginForm.addEventListener("submit", async function (event) {
        event.preventDefault();

        const formData = new FormData(loginForm);
        const username = formData.get("username");
        const password = formData.get("password");

        // Kiểm tra nếu thiếu tên đăng nhập hoặc mật khẩu
        if (!username || !password) {
            errorMessageLogin.textContent = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
            errorMessageLogin.style.display = "block";
            return;
        }

        try {
            const response = await fetch("https://localhost:7182/api/Account/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ username, password })
            });

            const result = await response.json();

            if (response.ok) {
                const token = result.token;
                sessionStorage.setItem("Token", token);
                const role = getRoleFromToken(token);
                redirectToRoleBasedPage(role);
            } else {
                errorMessageLogin.textContent = result.message || "Tên đăng nhập hoặc mật khẩu không đúng";
                errorMessageLogin.style.display = "block";
            }
        } catch (error) {
            errorMessageLogin.textContent = "Không thể kết nối đến máy chủ. Vui lòng thử lại.";
            errorMessageLogin.style.display = "block";
        }
    });

    // Lấy vai trò từ token
    function getRoleFromToken(token) {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || "KhachHang";
    }

    // Chuyển hướng theo vai trò
    function redirectToRoleBasedPage(role) {
        switch (role) {
            case "Admin":
                window.location.href = "/Admin";
                break;
            case "NhanVienDS":
                window.location.href = "/Employee";
                break;
            case "KhachHang":
                window.location.href = "/Customer";
                break;
            default:
                window.location.href = "/";
        }
    }

    // Xóa thông tin trong modal khi đóng
    addKHModal.addEventListener("hidden.bs.modal", function () {
        document.getElementById("tenkh").value = '';
        document.getElementById("sdtkh").value = '';
        document.getElementById("gtkh").value = 'Nam';
        document.getElementById("dckh").value = '';
        document.getElementById("userkh").value = '';
        document.getElementById("passkh").value = '';
        errorMessageSignup.style.display = 'none';
    });

    // Khi nút "Sign up" được nhấn
    addKHButton.addEventListener("click", async function () {
        const tenKh = document.getElementById("tenkh").value;
        const sdtKh = document.getElementById("sdtkh").value;
        const gtKh = document.getElementById("gtkh").value;
        const dcKh = document.getElementById("dckh").value;
        const userKh = document.getElementById("userkh").value;
        const passKh = document.getElementById("passkh").value;

        // Kiểm tra các trường không được để trống
        if (!tenKh || !sdtKh || !gtKh || !dcKh || !userKh || !passKh) {
            errorMessageSignup.textContent = "Vui lòng điền đầy đủ thông tin.";
            errorMessageSignup.style.display = 'block';
            return;
        }

        const khachHangData = {
            tenKh : tenKh,
            sdt: sdtKh,
            gioitinh: gtKh,
            diachi: dcKh,
            userID:"",
            user: {
                username: userKh,
                password: passKh,
                role: "KhachHang"
            }
        };

        try {
            const response = await fetch("https://localhost:7182/api/Account/signup", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(khachHangData)
            });

            if (response.ok) {
                alert("Đăng ký thành công!");
                $('#addKHModal').modal('hide');

                // Xóa lớp 'modal-backdrop' để tránh màn đen
                $('.modal-backdrop').remove();
            } else {
                let errorMessage = "Có lỗi xảy ra trong quá trình đăng ký.";
                const result = await response.text();

                if (result) {
                    if (result.includes("Tài khoản đã tồn tại.")) {
                        errorMessage = "Tên đăng nhập đã tồn tại.";
                    } else if (result.includes("Số điện thoại đã tồn tại.")) {
                        errorMessage = "Số điện thoại đã tồn tại.";
                    } else {
                        errorMessage = result;
                    }
                }

                errorMessageSignup.textContent = errorMessage;
                errorMessageSignup.style.display = 'block';
            }
        } catch (error) {
            errorMessageSignup.textContent = "Không thể kết nối đến máy chủ. Vui lòng thử lại.";
            errorMessageSignup.style.display = 'block';
        }
    });
});
