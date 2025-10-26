<template>
  <div class="spa-login-wrapper">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-6 col-lg-4">
          <div class="spa-login-container">
            <!-- Header Section -->
            <div class="login-header text-center mb-4">
              <div class="welcome-icon mb-3">🌿</div>
              <h1 class="login-title">Chào mừng trở lại</h1>
              <p class="login-subtitle">Đăng nhập để tiếp tục hành trình chăm sóc sức khỏe của bạn</p>
            </div>

            <!-- Login Form -->
            <form @submit.prevent="handleLogin" class="login-form">
              <!-- Phone Input -->
              <div class="mb-3">
                <label for="phone" class="form-label">Số điện thoại</label>
                <div class="input-group">
                  <span class="input-group-text bg-light border-end-0">📱</span>
                  <input 
                    type="tel" 
                    id="phone"
                    class="form-control border-start-0"
                    v-model="form.phone"
                    placeholder="Nhập số điện thoại"
                    :class="{ 'is-invalid': errors.phone }"
                  >
                </div>
                <div v-if="errors.phone" class="invalid-feedback d-block">
                  <i class="fas fa-exclamation-circle me-1"></i>
                  {{ errors.phone }}
                </div>
              </div>

              <!-- Password Input -->
              <div class="mb-3">
                <label for="password" class="form-label">Mật khẩu</label>
                <div class="input-group">
                  <span class="input-group-text bg-light border-end-0">🔒</span>
                  <input 
                    :type="showPassword ? 'text' : 'password'"
                    id="password"
                    class="form-control border-start-0 border-end-0"
                    v-model="form.password"
                    placeholder="Nhập mật khẩu"
                    :class="{ 'is-invalid': errors.password }"
                  >
                  <button 
                    class="btn btn-outline-secondary border-start-0"
                    type="button"
                    @click="togglePassword"
                  >
                    {{ showPassword ? '🙈' : '👁️' }}
                  </button>
                </div>
                <div v-if="errors.password" class="invalid-feedback d-block">
                  <i class="fas fa-exclamation-circle me-1"></i>
                  {{ errors.password }}
                </div>
              </div>

              <!-- Login Button -->
              <button 
                type="submit" 
                class="btn btn-success w-100 mb-3 login-btn"
                :disabled="isLoading"
              >
                <span v-if="isLoading" class="d-flex align-items-center justify-content-center">
                  <span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                  Đang đăng nhập...
                </span>
                <span v-else class="d-flex align-items-center justify-content-center">
                  <span class="me-2">🌸</span>
                  Đăng nhập
                </span>
              </button>

              <!-- Forgot Password -->
              <div class="text-center mb-3">
                <a href="#" @click.prevent="forgotPassword" class="text-success text-decoration-none">
                  Quên mật khẩu?
                </a>
              </div>

              <!-- Divider -->
              <div class="text-center mb-3">
                <span class="text-muted">Hoặc</span>
              </div>

              <!-- Register Link -->
              <div class="text-center">
                <p class="text-muted mb-2">Chưa có tài khoản?</p>
                <a href="#" @click.prevent="goToRegister" class="btn btn-outline-success">
                  <span class="me-2">✨</span>
                  Đăng ký ngay
                </a>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import notification from '../notification'
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { authAPI } from '../services/authservice' 

// Router instance
const router = useRouter()

// Reactive data
const form = reactive({
  phone: '',
  password: ''
})

const errors = reactive({
  phone: '',
  password: ''
})

// Refs
const showPassword = ref(false)
const isLoading = ref(false)

// Methods
const validateForm = () => {
  let isValid = true
  
  // Reset errors
  errors.phone = ''
  errors.password = ''

  // Validate phone
  if (!form.phone.trim()) {
    errors.phone = 'Vui lòng nhập số điện thoại'
    isValid = false
  } else if (!/^[0-9]{10,11}$/.test(form.phone.replace(/\s/g, ''))) {
    errors.phone = 'Số điện thoại không hợp lệ'
    isValid = false
  }

  // Validate password
  if (!form.password.trim()) {
    errors.password = 'Vui lòng nhập mật khẩu'
    isValid = false
  } else if (form.password.length < 6) {
    errors.password = 'Mật khẩu phải có ít nhất 6 ký tự'
    isValid = false
  }

  return isValid
}

const handleLogin = async () => {
  if (!validateForm()) return

  isLoading.value = true

  try {
    const result = await authAPI.login({
      PhoneNumber: form.phone,
      Password: form.password
    })

    if(result){
        notification.success("Đăng nhập thành công", {
            title : "",
            timer : 1500
        })

        setTimeout(() => {
            router.push('/')
        }, 1500)
        
        return ;    
    }

    notification.error("Đăng nhập thất bại")
    
  } catch (error) {
    console.error('Login error:', error)
    notification.error( error.message ? error.message : 'Đăng nhập thất bại. Vui lòng thử lại!' )
  } finally {
    isLoading.value = false
  }
}

const togglePassword = () => {
  showPassword.value = !showPassword.value
}

const forgotPassword = () => {
  router.push('/ForgetPassword')
}

const goToRegister = () => {
  router.push('/register')
}

// Emit events
const emit = defineEmits(['login-success', 'forgot-password', 'go-to-register'])
</script>

<style scoped>
/* Import Bootstrap CSS nếu chưa có */
@import url('https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css');

.spa-login-wrapper {
  min-height: 100vh;
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  display: flex;
  align-items: center;
  padding: 2rem 0;
}

.spa-login-container {
  background: white;
  border-radius: 15px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
  padding: 2rem;
  border: 1px solid #e9ecef;
}

.welcome-icon {
  font-size: 3rem;
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0% { transform: scale(1); }
  50% { transform: scale(1.05); }
  100% { transform: scale(1); }
}

.login-title {
  color: #198754;
  font-weight: 700;
  margin-bottom: 0.5rem;
}

.login-subtitle {
  color: #6c757d;
  font-size: 0.95rem;
}

.input-group-text {
  background-color: #f8f9fa !important;
  border-color: #dee2e6;
}

.form-control {
  border-color: #dee2e6;
  padding: 0.75rem;
}

.form-control:focus {
  border-color: #198754;
  box-shadow: 0 0 0 0.2rem rgba(25, 135, 84, 0.25);
}

.login-btn {
  padding: 0.75rem 1.5rem;
  font-weight: 600;
  font-size: 1.1rem;
  border-radius: 10px;
  background: linear-gradient(45deg, #198754, #20c997);
  border: none;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.login-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(25, 135, 84, 0.4);
  background: linear-gradient(45deg, #157347, #0dcaf0);
}

.login-btn:active:not(:disabled) {
  transform: translateY(0);
}

.login-btn:disabled {
  opacity: 0.8;
  transform: none !important;
  box-shadow: none !important;
}

.btn-outline-success {
  border-radius: 10px;
  font-weight: 600;
  transition: all 0.3s ease;
}

.btn-outline-success:hover {
  transform: translateY(-1px);
}

.text-success {
  transition: all 0.3s ease;
}

.text-success:hover {
  color: #157347 !important;
  text-decoration: underline !important;
}

.invalid-feedback {
  font-size: 0.875rem;
  color: #dc3545;
}

/* Responsive */
@media (max-width: 768px) {
  .spa-login-container {
    margin: 1rem;
    padding: 1.5rem;
  }
  
  .login-title {
    font-size: 1.5rem;
  }
  
  .welcome-icon {
    font-size: 2.5rem;
  }
}
</style>