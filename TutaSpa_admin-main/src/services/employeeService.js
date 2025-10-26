import axios from "axios";
import { authAPI } from "../services/authservice";

// Cấu hình base URL cho API
const API_BASE_URL = import.meta.env.VITE_BASE_URL;
import router from "../router/router.js";

// Tạo instance của axios với cấu hình mặc định
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 15000, // 10 seconds timeout
});

// Interceptor để thêm token vào request headers
apiClient.interceptors.request.use(
  (config) => {
    // Lấy token từ localStorage hoặc store
    const token =
      localStorage.getItem("accessToken") ||
      sessionStorage.getItem("accessToken");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    } else {
      router.push("/login?return_url=employees");
      return;
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// ✅ Biến để theo dõi refresh token đang được thực hiện
let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });

  failedQueue = [];
};

// Interceptor để xử lý response và error
apiClient.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    const originalRequest = error.config;

    if (error.response) {
      const { status, data } = error.response;

      switch (status) {
        case 401:
          // ✅ Kiểm tra nếu đã thử refresh token rồi
          if (originalRequest._retry) {
            // Đã thử refresh nhưng vẫn 401 -> logout
            localStorage.removeItem("accessToken");
            localStorage.removeItem("user-info");
            router.push(
              "/login?return_url=" + router.currentRoute.value.fullPath
            );
            return Promise.reject(error);
          }

          // ✅ Kiểm tra nếu request này là refresh-token
          if (originalRequest.url.includes("/auth/refresh-token")) {
            // Refresh token cũng bị 401 -> logout
            localStorage.removeItem("accessToken");
            localStorage.removeItem("user-info");
            router.push(
              "/login?return_url=" + router.currentRoute.value.fullPath
            );
            return Promise.reject(error);
          }

          originalRequest._retry = true;

          if (isRefreshing) {
            // ✅ Nếu đang refresh, đợi kết quả
            return new Promise((resolve, reject) => {
              failedQueue.push({ resolve, reject });
            })
              .then((token) => {
                originalRequest.headers.Authorization = `Bearer ${token}`;
                return apiClient(originalRequest);
              })
              .catch((err) => {
                return Promise.reject(err);
              });
          }

          isRefreshing = true;

          try {
            // ✅ Gọi refresh token API
            const response = await authAPI.refreshToken();

            if (response.accessToken) {
              localStorage.setItem("accessToken", response.accessToken);

              // ✅ Cập nhật header cho request gốc
              originalRequest.headers.Authorization = `Bearer ${response.accessToken}`;

              // ✅ Xử lý các request đang chờ
              processQueue(null, response.accessToken);

              // ✅ Retry request gốc
              return apiClient(originalRequest);
            }
          } catch (refreshError) {
            // ✅ Refresh token thất bại -> logout
            processQueue(refreshError, null);
            localStorage.removeItem("accessToken");
            localStorage.removeItem("user-info");
            router.push(
              "/login?return_url=" + router.currentRoute.value.fullPath
            );
            return Promise.reject(refreshError);
          } finally {
            isRefreshing = false;
          }
          break;
        case 403:
          console.error("Unauthorized access - redirecting to login");
          router.push("/login?return_url=employees");
          localStorage.removeItem("accessToken");
          break;
        case 404:
          console.error("Resource not found");
          break;
        case 500:
          console.error("Internal server error");
          break;
        default:
          throw error;
      }
    } else if (error.request) {
      console.error("Network error - no response received");
    } else {
      console.error("Request setup error:", error.message);
    }

    return Promise.reject(error);
  }
);

/**
 * Service class để xử lý các API calls liên quan đến Employee
 */
class EmployeeService {
  /**
   * Lấy danh sách nhân viên với phân trang
   * @param {number} page - Số trang (mặc định là 1)
   * @returns {Promise} Promise chứa dữ liệu nhân viên
   */
  static async getEmployees(page = 1) {
    try {
      const response = await apiClient.get(`/employees?page=${page}`);
      return response.data;
    } catch (error) {
      throw this.handleError(error, "Lỗi khi lấy danh sách nhân viên");
    }
  }

  /**
   * Tạo nhân viên mới
   * @param {Object} employeeData - Dữ liệu nhân viên mới
   * @param {string} employeeData.Email - Email nhân viên
   * @param {string} employeeData.Name - Tên nhân viên
   * @param {string} employeeData.PhoneNumber - Số điện thoại
   * @param {string} employeeData.Address - Địa chỉ
   * @param {number} employeeData.Role - Vai trò (0: Cashier, 1: InventoryManager)
   * @returns {Promise} Promise chứa kết quả tạo nhân viên
   */
  static async createEmployee(employeeData) {
    try {
      // Validate dữ liệu đầu vào
      this.validateEmployeeData(employeeData);

      // Chuyển đổi Role thành số
      const payload = {
        ...employeeData,
        Role: parseInt(employeeData.Role),
      };

      const response = await apiClient.post("/employees", payload);
      return {
        success: true,
        data: response.data.data || response.data,
        message: "Tạo nhân viên thành công",
      };
    } catch (error) {
      throw this.handleError(error, "Lỗi khi tạo nhân viên");
    }
  }

  /**
   * Cập nhật thông tin nhân viên
   * @param {Object} employeeData - Dữ liệu nhân viên cần cập nhật
   * @param {string} employeeData.EmpId - ID nhân viên
   * @param {string} employeeData.Email - Email nhân viên
   * @param {string} employeeData.Name - Tên nhân viên
   * @param {string} employeeData.PhoneNumber - Số điện thoại
   * @param {string} employeeData.Address - Địa chỉ
   * @returns {Promise} Promise chứa kết quả cập nhật
   */
  static async updateEmployee(employeeData) {
    console.log(employeeData);
    try {
      // Validate dữ liệu đầu vào
      this.validateUpdateEmployeeData(employeeData);

      const response = await apiClient.put("/employees", employeeData);
      return {
        success: true,
        data: response.data.data || response.data,
        message: "Cập nhật nhân viên thành công",
      };
    } catch (error) {
      throw this.handleError(error, "Lỗi khi cập nhật nhân viên");
    }
  }

  /**
   * Cập nhật trạng thái nhân viên
   * @param {string} empId - ID nhân viên
   * @param {boolean} status - Trạng thái mới (true: active, false: inactive)
   * @returns {Promise} Promise chứa kết quả cập nhật trạng thái
   */
  static async updateEmployeeStatus(empId, status) {
    try {
      if (!empId) {
        throw new Error("ID nhân viên không được để trống");
      }

      const response = await apiClient.patch(
        `/employees/status/${empId}?status=${status}`
      );
      return {
        success: true,
        data: response.data.data || response.data,
        message: `Trạng thái nhân viên đã được ${
          status ? "kích hoạt" : "vô hiệu hóa"
        }`,
      };
    } catch (error) {
      throw this.handleError(error, "Lỗi khi cập nhật trạng thái nhân viên");
    }
  }

  /**
   * Cập nhật vai trò nhân viên
   * @param {string} empId - ID nhân viên
   * @param {number} role - Vai trò mới (0: Cashier, 1: InventoryManager)
   * @returns {Promise} Promise chứa kết quả cập nhật vai trò
   */
  static async updateEmployeeRole(empId, role) {
    try {
      if (!empId) {
        throw new Error("ID nhân viên không được để trống");
      }

      // Validate role
      if (![0, 1].includes(parseInt(role))) {
        throw new Error("Vai trò không hợp lệ");
      }

      const response = await apiClient.patch(
        `/employees/role/${empId}?role=${role}`
      );
      return {
        success: true,
        data: response.data.data || response.data,
        message: "Vai trò nhân viên đã được cập nhật",
      };
    } catch (error) {
      throw this.handleError(error, "Lỗi khi cập nhật vai trò nhân viên");
    }
  }

  /**
   * Xóa nhân viên
   * @param {string} empId - ID nhân viên cần xóa
   * @returns {Promise} Promise chứa kết quả xóa nhân viên
   */
  static async deleteEmployee(empId) {
    try {
      if (!empId) {
        throw new Error("ID nhân viên không được để trống");
      }

      const response = await apiClient.delete(`/employees/${empId}`);
      return {
        success: true,
        data: response.data.data || response.data,
        message: "Xóa nhân viên thành công",
      };
    } catch (error) {
      throw this.handleError(error, "Lỗi khi xóa nhân viên");
    }
  }

  /**
   * Lấy thông tin chi tiết một nhân viên
   * @param {string} empId - ID nhân viên
   * @returns {Promise} Promise chứa thông tin nhân viên
   */
  static async getEmployeeById(empId) {
    try {
      if (!empId) {
        throw new Error("ID nhân viên không được để trống");
      }

      const response = await apiClient.get(`/employees/${empId}`);
      return {
        success: true,
        data: response.data.data || response.data,
        message: "Lấy thông tin nhân viên thành công",
      };
    } catch (error) {
      throw this.handleError(error, "Lỗi khi lấy thông tin nhân viên");
    }
  }

  /**
   * Tìm kiếm nhân viên theo từ khóa
   * @param {string} keyword - Từ khóa tìm kiếm
   * @param {number} page - Số trang
   * @returns {Promise} Promise chứa kết quả tìm kiếm
   */
  static async searchEmployees(keyword, page = 1) {
    try {
      const response = await apiClient.get(
        `/employees?keyword=${encodeURIComponent(keyword)}&page=${page}`
      );
      return response.data;
    } catch (error) {
      throw this.handleError(error, "Lỗi khi tìm kiếm nhân viên");
    }
  }

  /**
   * Validate dữ liệu nhân viên khi tạo mới
   * @param {Object} employeeData - Dữ liệu nhân viên
   */
  static validateEmployeeData(employeeData) {
    const requiredFields = ["Email", "Name", "PhoneNumber", "Address", "Role"];

    for (const field of requiredFields) {
      if (
        !employeeData[field] ||
        employeeData[field].toString().trim() === ""
      ) {
        throw new Error(
          `Trường ${this.getFieldName(field)} không được để trống`
        );
      }
    }

    // Validate email format
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(employeeData.Email)) {
      throw new Error("Định dạng email không hợp lệ");
    }

    // Validate phone number (basic validation)
    const phoneRegex = /^[\d\s\-\+\(\)]+$/;
    if (!phoneRegex.test(employeeData.PhoneNumber)) {
      throw new Error("Số điện thoại không hợp lệ");
    }

    // Validate role
    if (![0, 1, "0", "1"].includes(employeeData.Role)) {
      throw new Error("Vai trò không hợp lệ");
    }
  }

  /**
   * Validate dữ liệu nhân viên khi cập nhật
   * @param {Object} employeeData - Dữ liệu nhân viên
   */
  static validateUpdateEmployeeData(employeeData) {
    const requiredFields = ["EmpId", "Email", "Name", "PhoneNumber", "Address"];

    for (const field of requiredFields) {
      if (
        !employeeData[field] ||
        employeeData[field].toString().trim() === ""
      ) {
        throw new Error(
          `Trường ${this.getFieldName(field)} không được để trống`
        );
      }
    }

    // Validate email format
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(employeeData.Email)) {
      throw new Error("Định dạng email không hợp lệ");
    }

    // Validate phone number
    const phoneRegex = /^[\d\s\-\+\(\)]+$/;
    if (!phoneRegex.test(employeeData.PhoneNumber)) {
      throw new Error("Số điện thoại không hợp lệ");
    }
  }

  /**
   * Lấy tên trường tiếng Việt
   * @param {string} fieldName - Tên trường tiếng Anh
   * @returns {string} Tên trường tiếng Việt
   */
  static getFieldName(fieldName) {
    const fieldNames = {
      Email: "Email",
      FirtName: "Tên",
      LastName: "Họ",
      PhoneNumber: "Số điện thoại",
      Address: "Địa chỉ",
      Role: "Vai trò",
      EmpId: "ID nhân viên",
    };
    return fieldNames[fieldName] || fieldName;
  }

  /**
   * Xử lý lỗi API
   * @param {Error} error - Lỗi từ API
   * @param {string} defaultMessage - Thông báo lỗi mặc định
   * @returns {Error} Lỗi đã được xử lý
   */
  static handleError(error, defaultMessage) {
    let message = defaultMessage;

    if (error.response && error.response.data) {
      // Lỗi từ server
      message =
        error.response.data.message ||
        error.response.data.error ||
        defaultMessage;
    } else if (error.message) {
      // Lỗi từ client hoặc network
      message = error.message;
    }

    return new Error(message);
  }

  /**
   * Utility method để format role từ số sang text
   * @param {number|string} role - Vai trò (0 hoặc 1)
   * @returns {string} Tên vai trò
   */
  static formatRole(role) {
    const roleMap = {
      0: "Cashier",
      1: "InventoryManager",
      0: "Cashier",
      1: "InventoryManager",
      Cashier: "Cashier",
      InventoryManager: "InventoryManager",
    };
    return roleMap[role] || "Unknown";
  }

  /**
   * Utility method để format role sang tiếng Việt
   * @param {number|string} role - Vai trò
   * @returns {string} Tên vai trò tiếng Việt
   */
  static formatRoleVietnamese(role) {
    const roleMap = {
      0: "Thu ngân",
      1: "Quản lý kho",
      0: "Thu ngân",
      1: "Quản lý kho",
      Cashier: "Thu ngân",
      InventoryManager: "Quản lý kho",
    };
    return roleMap[role] || "Không xác định";
  }

  /**
   * Utility method để validate email
   * @param {string} email - Email cần validate
   * @returns {boolean} True nếu email hợp lệ
   */
  static isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  /**
   * Utility method để validate phone number
   * @param {string} phone - Số điện thoại cần validate
   * @returns {boolean} True nếu số điện thoại hợp lệ
   */
  static isValidPhone(phone) {
    const phoneRegex = /^[\d\s\-\+\(\)]+$/;
    return phoneRegex.test(phone) && phone.replace(/\D/g, "").length >= 10;
  }

  /**
   * Utility method để format phone number
   * @param {string} phone - Số điện thoại
   * @returns {string} Số điện thoại đã format
   */
  static formatPhone(phone) {
    const cleaned = phone.replace(/\D/g, "");
    const match = cleaned.match(/^(\d{3})(\d{3})(\d{4})$/);
    if (match) {
      return `(${match[1]}) ${match[2]}-${match[3]}`;
    }
    return phone;
  }
}

export default EmployeeService;

// Export các utility functions nếu cần sử dụng riêng lẻ
export { EmployeeService, apiClient };
