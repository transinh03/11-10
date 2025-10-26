import axios from 'axios'
import router from '../router/router';
import { useRouter } from 'vue-router'
import { useAuthStore } from '../services/authStore'
import { createSignalRConnection, getSignalRConnection, stopSignalRConnection } from '../services/chatService'

// Cấu hình base URL cho API
const API_BASE_URL = import.meta.env.VITE_BASE_URL;

// Tạo axios instance
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000, // 10 seconds timeout
  withCredentials: true, // Quan trọng: để gửi cookies
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  },
})
// ✅ Biến để theo dõi refresh token đang được thực hiện
let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach(prom => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });
  
  failedQueue = [];
};

// Request interceptor - thêm token vào header
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor - xử lý response và error
apiClient.interceptors.response.use(
  (response) => {
    return response.data
  },
  async (error) => {
    const originalRequest = error.config;

    if (error.response) {
      const { status, data } = error.response
      
      switch (status) {
        case 401:
          // ✅ Kiểm tra nếu đã thử refresh token rồi
          if (originalRequest._retry) {
            // Đã thử refresh nhưng vẫn 401 -> logout
            localStorage.removeItem('accessToken')
            localStorage.removeItem('user-info')
            router.push('/login?return_url=' + router.currentRoute.value.fullPath)
            return Promise.reject(error);
          }

          // ✅ Kiểm tra nếu request này là refresh-token
          if (originalRequest.url.includes('/auth/refresh-token')) {
            // Refresh token cũng bị 401 -> logout
            localStorage.removeItem('accessToken')
            localStorage.removeItem('user-info')
            router.push('/login?return_url=' + router.currentRoute.value.fullPath)
            return Promise.reject(error);
          }

          originalRequest._retry = true;

          if (isRefreshing) {
            // ✅ Nếu đang refresh, đợi kết quả
            return new Promise((resolve, reject) => {
              failedQueue.push({ resolve, reject });
            }).then(token => {
              originalRequest.headers.Authorization = `Bearer ${token}`;
              return apiClient(originalRequest);
            }).catch(err => {
              return Promise.reject(err);
            });
          }

          isRefreshing = true;

          try {
            // ✅ Gọi refresh token API
            const response = await apiClient.post('/auth/refresh-token', {});
            
            if (response.accessToken) {
              localStorage.setItem('accessToken', response.accessToken);
              
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
            localStorage.removeItem('accessToken')
            localStorage.removeItem('user-info')
            router.push('/login?return_url=' + router.currentRoute.value.fullPath)
            return Promise.reject(refreshError);
          } finally {
            isRefreshing = false;
          }
          break;
          
        case 403:
          console.error('Không có quyền truy cập')
          break
        case 404:
          console.error('API không tồn tại')
          break
        case 500:
          console.error('Lỗi server')
          break
        default:
          console.error('Lỗi không xác định:', data?.message || error.message)
      }
      
      return Promise.reject(data || error.response)
    } else if (error.request) {
      console.error('Không nhận được phản hồi từ server:', error.request)
      // Network error
      console.error('Lỗi kết nối mạng')
      return Promise.reject({ message: 'Lỗi kết nối mạng' })
    } else {
      console.error('Lỗi:', error.message)
      return Promise.reject(error)
    }
  }
)

// ====================== AUTH APIs ======================

export const authAPI = {
  // Đăng nhập
  login: async (credentials) => {
    try {
      const response = await apiClient.post('/auth/login', credentials)

      const user_info = response.userInfo
      const token = response.accessToken
      
      // Lưu token vào localStorage
      if (response.accessToken) {
        localStorage.setItem('accessToken', response.accessToken)
        localStorage.setItem("user-info", JSON.stringify(response.userInfo))
      }

      const auth = useAuthStore()
      auth.login(response.accessToken, user_info.role,user_info)  
      
      if (user_info.role === 'Cashier') {
        const conn = createSignalRConnection(token)
      }
      
      return response
    } catch (error) {
      throw error
    }
  },

  // Đăng xuất
  logout: async () => {
    try {
      await apiClient.post('/auth/logout')
      localStorage.removeItem('accessToken')
      localStorage.removeItem('user-info')


      if(getSignalRConnection()) {
        stopSignalRConnection()
      }
      
      // ✅ Clear auth store
      const auth = useAuthStore()
      auth.logout()
      
      return true
    } catch (error) {
      // Vẫn xóa token dù API lỗi
      localStorage.removeItem('accessToken')
      localStorage.removeItem('user-info')
      
      const auth = useAuthStore()
      auth.logout()
      
      throw error
    }
  },

  // ✅ Làm mới token - sử dụng trực tiếp trong interceptor
  refreshToken: async () => {
    try {
      // ✅ Tạo instance riêng để tránh loop
      const refreshClient = axios.create({
        baseURL: API_BASE_URL,
        withCredentials: true,
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json'
        }
      })

      const response = await refreshClient.post('/auth/refresh-token', {})
      
      if (response.data.accessToken) {
        localStorage.setItem('accessToken', response.data.accessToken)
      }
      
      return response.data
    } catch (error) {
      localStorage.removeItem('accessToken')
      localStorage.removeItem('user-info')
      throw error
    }
  },

  // Quên mật khẩu
  forgotPassword: async (email) => {
    try {
      const response = await apiClient.post('/auth/forgot-password', { email })
      return response
    } catch (error) {
      throw error
    }
  },

  // Đặt lại mật khẩu
  resetPassword: async (oldPassword, newPassword, confirmPassword) => {
    try {
      const response = await apiClient.post('/auth/reset-password', {
        oldPassword,
        newPassword,
        confirmPassword
      })
      return response
    } catch (error) {
      throw error
    }
  }
}

// ====================== USER APIs ======================

export const userAPI = {
  // Lấy thông tin user hiện tại
  getCurrentUser: async () => {
    try {
      const response = await apiClient.get('/user/profile')
      return response
    } catch (error) {
      throw error
    }
  },

  // Cập nhật thông tin user
  updateProfile: async (userData) => {
    try {
      const response = await apiClient.put('/user/profile', userData)
      return response
    } catch (error) {
      throw error
    }
  },

  // Thay đổi mật khẩu
  changePassword: async (passwordData) => {
    try {
      const response = await apiClient.put('/user/change-password', passwordData)
      return response
    } catch (error) {
      throw error
    }
  },

  // Upload avatar
  uploadAvatar: async (file) => {
    try {
      const formData = new FormData()
      formData.append('avatar', file)

      const response = await apiClient.post('/user/upload-avatar', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      })
      return response
    } catch (error) {
      throw error
    }
  }
}

// ====================== UTILITY FUNCTIONS ======================

export const apiUtils = {
  // Kiểm tra xem user đã đăng nhập chưa
  isAuthenticated: () => {
    return !!localStorage.getItem('accessToken')
  },

  // Lấy token hiện tại
  getToken: () => {
    return localStorage.getItem('accessToken')
  },

  // Xóa token (logout)
  clearAuth: () => {
    localStorage.removeItem('accessToken')
    localStorage.removeItem('user-info')
  },

  // Format lỗi API để hiển thị
  formatError: (error) => {
    if (error.response?.data?.message) {
      return error.response.data.message
    } else if (error.message) {
      return error.message
    } else {
      return 'Có lỗi xảy ra, vui lòng thử lại'
    }
  },

  // Kiểm tra kết nối internet
  checkConnection: async () => {
    try {
      await apiClient.get('/health-check')
      return true
    } catch (error) {
      return false
    }
  }
}

// Export axios instance nếu cần sử dụng trực tiếp
export default apiClient