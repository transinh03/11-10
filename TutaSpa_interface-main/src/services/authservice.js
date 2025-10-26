import axios from "axios";
import { error } from "../notification";
import { useAuthState } from "./authstate";
// Cấu hình base URL cho API
const API_BASE_URL = import.meta.env.VITE_BASE_URL;

// Tạo axios instance
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true, // Cho phép gửi cookie với request
  timeout: 10000, // 10 seconds timeout
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
});

// Request interceptor - thêm token vào header
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("accessToken");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor - xử lý response và error
apiClient.interceptors.response.use(
  (response) => {
    return response.data;
  },
  (error) => {
    if (error.response) {
      // Server trả về error status
      const { status, data } = error.response;

      switch (status) {
        case 401:
          // Token hết hạn hoặc không hợp lệ
          localStorage.removeItem("accessToken");
          localStorage.removeItem("refresh_token");
          // Redirect về trang login
          if (typeof window !== "undefined") {
            window.location.href = "/login";
          }
          break;
        case 403:
          console.error("Không có quyền truy cập");
          break;
        case 404:
          console.error("API không tồn tại");
          break;
        case 500:
          console.error("Lỗi server");
          break;
        default:
          console.error("Lỗi không xác định:", data?.message || error.message);
      }

      return Promise.reject(data || error.response);
    } else if (error.request) {
      // Network error
      console.error("Lỗi kết nối mạng");
      return Promise.reject({ message: "Lỗi kết nối mạng" });
    } else {
      console.error("Lỗi:", error.message);
      return Promise.reject(error);
    }
  }
);

// ====================== AUTH APIs ======================

export const authAPI = {
  // Đăng nhập
  login: async (credentials) => {
    try {
      const response = await apiClient.post("/account/login", credentials);

      // Lưu token vào localStorage
      if (response.accessToken) {
        const state = useAuthState();
        state.login(response.accessToken, response.userInfo);
      }

      return response;
    } catch (error) {
      throw error;
    }
  },

  sendOTP: async (phoneNumber) => {
    try {
      const response = await apiClient.post(
        `/otp/send?phoneNumber=${phoneNumber}`
      );
      return response;
    } catch (error) {
      console.log(error);
      throw error;
    }
  },

  verifyOTP: async (phoneNumber, OTP) => {
    try {
      const response = await apiClient.post(`/otp/verify`, {
        phoneNumber: phoneNumber,
        OTP: OTP,
      });
      return true;
    } catch (error) {
      throw error;
    }
  },
  // Gửi OTP qua email
  sendOTPMail: async (email) => {
    try {
      const response = await apiClient.post(`/otp/sendMail?mail=${email}`);
      return response;
    } catch (error) {
      console.error("Send OTP mail error:", error);
      throw error;
    }
  },

  // Xác thực OTP qua email
  verifyOTPMail: async (email, OTP) => {
    try {
      const response = await apiClient.post(`/otp/verifyMail`, {
        email: email,
        OTP: OTP,
      });
      return true;
    } catch (error) {
      console.error("Verify OTP mail error:", error);
      throw error;
    }
  },

  // Đăng ký
  register: async (userData) => {
    try {
      const response = await apiClient.post("/account/register", userData);
      return true;
    } catch (error) {
      throw error;
    }
  },

  checkexist: async (phoneNumber) => {
    try {
      const response = await apiClient.get(
        `/account/phone/check?phoneNumber=${phoneNumber}`
      );
      return response.exists;
    } catch (error) {
      throw error;
    }
  },

  // Đăng xuất
  logout: async () => {
    try {
      await apiClient.post("/account/logout");
      localStorage.removeItem("accessToken");
      return true;
    } catch (error) {
      // Vẫn xóa token dù API lỗi
      localStorage.removeItem("accessToken");
      throw error;
    }
  },

  // Làm mới token
  refreshToken: async () => {
    try {
      const response = await apiClient.post("/account/refresh-token");
      localStorage.setItem("accessToken", response.accessToken);
      return true;
    } catch (error) {
      localStorage.removeItem("accessToken");
      throw error;
    }
  },

  // Quên mật khẩu
  forgotPassword: async (email) => {
    try {
      const response = await apiClient.post("/auth/forgot-password", { email });
      return response;
    } catch (error) {
      throw error;
    }
  },

  // Đặt lại mật khẩu
  resetPassword: async (token, newPassword) => {
    try {
      const response = await apiClient.post("/auth/reset-password", {
        token,
        new_password: newPassword,
      });
      return response;
    } catch (error) {
      throw error;
    }
  },
};

// ====================== USER APIs ======================

export const userAPI = {
  // Lấy thông tin user hiện tại
  getCurrentUser: async () => {
    try {
      const response = await apiClient.get("/user/profile");
      return response;
    } catch (error) {
      throw error;
    }
  },

  // Cập nhật thông tin user
  updateProfile: async (userData) => {
    try {
      const response = await apiClient.put("/user/profile", userData);
      return response;
    } catch (error) {
      throw error;
    }
  },

  // Thay đổi mật khẩu
  changePassword: async (passwordData) => {
    try {
      const response = await apiClient.put(
        "/user/change-password",
        passwordData
      );
      return response;
    } catch (error) {
      throw error;
    }
  },

  // Upload avatar
  uploadAvatar: async (file) => {
    try {
      const formData = new FormData();
      formData.append("avatar", file);

      const response = await apiClient.post("/user/upload-avatar", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });
      return response;
    } catch (error) {
      throw error;
    }
  },
};

// ====================== GENERIC APIs ======================

export const genericAPI = {
  // GET request
  get: async (endpoint, params = {}) => {
    try {
      const response = await apiClient.get(endpoint, { params });
      return response;
    } catch (error) {
      throw error;
    }
  },

  // POST request
  post: async (endpoint, data = {}) => {
    try {
      const response = await apiClient.post(endpoint, data);
      return response;
    } catch (error) {
      throw error;
    }
  },

  // PUT request
  put: async (endpoint, data = {}) => {
    try {
      const response = await apiClient.put(endpoint, data);
      return response;
    } catch (error) {
      throw error;
    }
  },

  // DELETE request
  delete: async (endpoint) => {
    try {
      const response = await apiClient.delete(endpoint);
      return response;
    } catch (error) {
      throw error;
    }
  },

  // PATCH request
  patch: async (endpoint, data = {}) => {
    try {
      const response = await apiClient.patch(endpoint, data);
      return response;
    } catch (error) {
      throw error;
    }
  },
};

// ====================== UTILITY FUNCTIONS ======================

export const apiUtils = {
  // Kiểm tra xem user đã đăng nhập chưa
  isAuthenticated: () => {
    return !!localStorage.getItem("accessToken");
  },

  // Lấy token hiện tại
  getToken: () => {
    return localStorage.getItem("accessToken");
  },

  // Xóa token (logout)
  clearAuth: () => {
    localStorage.removeItem("accessToken");
  },

  // Format lỗi API để hiển thị
  formatError: (error) => {
    if (error.response?.data?.message) {
      return error.response.data.message;
    } else if (error.message) {
      return error.message;
    } else {
      return "Có lỗi xảy ra, vui lòng thử lại";
    }
  },

  // Kiểm tra kết nối internet
  checkConnection: async () => {
    try {
      await apiClient.get("/health-check");
      return true;
    } catch (error) {
      return false;
    }
  },
};

export default apiClient;
