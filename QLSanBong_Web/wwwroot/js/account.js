document.addEventListener("DOMContentLoaded", function () {
    // Lấy các phần tử cần thiết
    const loginForm = document.getElementById("loginForm");
    const signupForm = document.getElementById("signupForm");
    const addKHButton = document.getElementById("addKH");
    const addKHModal = document.getElementById("addKHModal");
    const errorMessageLogin = document.getElementById("errorMessage");
    const errorMessageSignup = document.getElementById("errorMessagesignup");

    // Lắng nghe sự kiện submit của form login
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
                // Lưu token vào sessionStorage
                const token = result.token;
                sessionStorage.setItem("Token", token);

                // Lấy role từ token (cần viết hàm getRoleFromToken)
                const role = getRoleFromToken(token);
                redirectToRoleBasedPage(role); // Chuyển hướng theo role
            } else {
                // Nếu response không thành công, lấy thông báo lỗi từ API
                const errorData = result || {}; // Tránh lỗi nếu result không phải là đối tượng JSON
                const errorMessage = errorData?.message || "Không thể đăng nhập. Vui lòng thử lại.";

                // Hiển thị thông báo lỗi trên màn hình
                errorMessageLogin.textContent = errorMessage;
                errorMessageLogin.style.display = "block";

                // Hiển thị thông báo lỗi bằng Toastr (nếu cần)
                alert(errorMessage);
            }
        } catch (error) {
            // Xử lý lỗi khi không thể kết nối đến API
            errorMessageLogin.textContent = "Không thể kết nối đến máy chủ. Vui lòng thử lại.";
            errorMessageLogin.style.display = "block";
        }
    });


    // Kiểm tra token ngay lập tức
    const token1 = sessionStorage.getItem("Token");

    // Lấy các phần tử DOM
    const formAccount = document.querySelector(".formaccount");
    const formUser = document.querySelector(".formuser");

    if (!token1) {
        document.querySelector(".formaccount").style.display = "flex";
        document.querySelector(".formuser").style.display = "none";
    } else {
        document.querySelector(".formaccount").style.display = "none";
        document.querySelector(".formuser").style.display = "flex";
    }

    // Lấy vai trò từ token
    function getRoleFromToken(token) {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
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
        document.getElementById("dckh").value = '';
        document.getElementById("userkh").value = '';
        document.getElementById("passkh").value = '';
        errorMessageSignup.style.display = 'none';
    });

    // Khi form đăng ký được submit
    signupForm.addEventListener("submit", async function (event) {
        event.preventDefault();

        const tenKh = document.getElementById("tenkh").value;
        const sdtKh = document.getElementById("sdtkh").value;
        const dcKh = document.getElementById("dckh").value;
        const userKh = document.getElementById("userkh").value;
        const passKh = document.getElementById("passkh").value;

        // Kiểm tra các trường không được để trống
        if (!tenKh || !sdtKh || !dcKh || !userKh || !passKh) {
            errorMessageSignup.textContent = "Vui lòng điền đầy đủ thông tin.";
            errorMessageSignup.style.display = 'block';
            return;
        }

        const khachHangData = {
            tenKh: tenKh,
            sdt: sdtKh,
            diachi: dcKh,
            user: {
                username: userKh,
                password: passKh,
                roleVM: [
                    {
                        roleName: "KhachHang", // Đặt role mặc định là KhachHang
                        thongTin: "Khách hàng"
                    }
                ]
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
