<template>
  <div class="min-h-screen flex items-center justify-center login-container relative overflow-hidden">
    <!-- Animated background elements -->
    <!-- Main login card -->
    <div class="login-card-wrapper">
      <div class="login-card">
        <!-- Header section -->
        <div class="card-header">

          <h1 class="title">Chào mừng trở lại</h1>
          <p class="subtitle">Đăng nhập tài khoản</p>
        </div>

        <!-- Form section -->
        <form @submit.prevent="handleLogin" class="form-container">
          <!-- Email field -->
          <div class="input-group">
            <div class="input-wrapper" :class="{ 'error': errors.email, 'focused': emailFocused }">
              <div class="input-icon">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
                  <path d="M4 4H20C21.1 4 22 4.9 22 6V18C22 19.1 21.1 20 20 20H4C2.9 20 2 19.1 2 18V6C2 4.9 2.9 4 4 4Z" stroke="currentColor" stroke-width="2"/>
                  <polyline points="22,6 12,13 2,6" stroke="currentColor" stroke-width="2"/>
                </svg>
              </div>
              <input
                id="email"
                v-model="loginForm.email"
                type="email"
                autocomplete="email"
                required
                class="styled-input"
                placeholder="Nhập email của bạn"
                @focus="emailFocused = true"
                @blur="emailFocused = false"
              />
            </div>
            <div class="error-message" v-if="errors.email">{{ errors.email }}</div>
          </div>

          <!-- Password field -->
          <div class="input-group">
            <div class="input-wrapper" :class="{ 'error': errors.password, 'focused': passwordFocused }">
              <div class="input-icon">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
                  <rect x="3" y="11" width="18" height="11" rx="2" ry="2" stroke="currentColor" stroke-width="2"/>
                  <circle cx="12" cy="16" r="1" fill="currentColor"/>
                  <path d="M7 11V7a5 5 0 0 1 10 0v4" stroke="currentColor" stroke-width="2"/>
                </svg>
              </div>
              <input
                id="password"
                v-model="loginForm.password"
                :type="showPassword ? 'text' : 'password'"
                autocomplete="current-password"
                required
                class="styled-input"
                placeholder="Nhập mật khẩu của bạn"
                @focus="passwordFocused = true"
                @blur="passwordFocused = false"
              />
              <button
                type="button"
                class="password-toggle"
                @click="showPassword = !showPassword"
              >
                <svg v-if="!showPassword" width="20" height="20" viewBox="0 0 24 24" fill="none">
                  <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" stroke="currentColor" stroke-width="2"/>
                  <circle cx="12" cy="12" r="3" stroke="currentColor" stroke-width="2"/>
                </svg>
                <svg v-else width="20" height="20" viewBox="0 0 24 24" fill="none">
                  <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24" stroke="currentColor" stroke-width="2"/>
                  <line x1="1" y1="1" x2="23" y2="23" stroke="currentColor" stroke-width="2"/>
                </svg>
              </button>
            </div>
            <div class="error-message" v-if="errors.password">{{ errors.password }}</div>
          </div>

          <!-- Global error message -->
          <div v-if="loginError" class="global-error">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
              <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2"/>
              <line x1="15" y1="9" x2="9" y2="15" stroke="currentColor" stroke-width="2"/>
              <line x1="9" y1="9" x2="15" y2="15" stroke="currentColor" stroke-width="2"/>
            </svg>
            <span>{{ loginError }}</span>
          </div>

          <!-- Submit button -->
          <button
            type="submit"
            :disabled="isLoading"
            class="submit-btn"
            :class="{ 'loading': isLoading }"
          >
            <div class="btn-content">
              <svg v-if="isLoading" class="loading-spinner" width="20" height="20" viewBox="0 0 24 24">
                <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2" fill="none" stroke-linecap="round" stroke-dasharray="60 30" />
              </svg>
              <span>{{ isLoading ? 'Đang xử lý...' : 'Đăng nhập' }}</span>
            </div>
            <div class="btn-bg"></div>
          </button>
        </form>

        <!-- Footer -->
        <div class="card-footer">
          <p> <RouterLink  to="/forget-password" class="signup-link">Quên mật khẩu?</RouterLink></p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import notification from '../notification/notification'
import { authAPI } from '../services/authservice'
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'


const router = useRouter()
const return_url = router.currentRoute.value.query.return_url 

const loginForm = reactive({
  email: '',
  password: ''
})

// Reactive state
const isLoading = ref(false)
const loginError = ref('')
const showPassword = ref(false)
const emailFocused = ref(false)
const passwordFocused = ref(false)

const errors = reactive({
  email: '',
  password: ''
})

// Validation function
const validateForm = () => {
  let isValid = true
  
  errors.email = ''
  errors.password = ''
  
  if (!loginForm.email) {
    errors.email = 'Email is required'
    isValid = false
  } else if (!/\S+@\S+\.\S+/.test(loginForm.email)) {
    errors.email = 'Please enter a valid email address'
    isValid = false
  }
  
  if (!loginForm.password) {
    errors.password = 'Password is required'
    isValid = false
  } else if (loginForm.password.length < 6) {
    errors.password = 'Password must be at least 6 characters'
    isValid = false
  }
  
  return isValid
}


// Route to dashboard based on role
const routeToDashboard = (role) => {
  const roleRoutes = {
    Admin: '/',
    Cashier: '/ThuNgan',
    InventoryManager: '/kho'
  }
  const route = roleRoutes[role] || '/'
  return route
}

// Handle login form submission
const handleLogin = async () => {
  if (!validateForm()) {
    return
  }
  
  isLoading.value = true
  loginError.value = ''
  
  try {
    const response = await authAPI.login({
      email: loginForm.email,
      password: loginForm.password
    })

    notification.success("Đăng nhập thành công", {
            title : "",
            timer : 1500
        })
    setTimeout(() => {
      const role = response.userInfo.role
      const url  = return_url || routeToDashboard(role)
      router.push(''+url)
    },2000)
    }
   catch (error) {
    loginError.value = error.message || 'Có lỗi xảy ra khi đăng nhập. Vui lòng thử lại.'
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>

    .login-container {
    display: flex;
    justify-content: center; /* Căn giữa ngang */
    align-items: center;      /* Căn giữa dọc */
      height: 100%;
    background: white;
    position: relative;
    }

    /* Floating background shapes */
    .floating-shapes {
    position: absolute;
    width: 100%;
    height: 100%;
    overflow: hidden;
    z-index: 0;
    }

    .shape {
    position: absolute;
    border-radius: 50%;
    background: linear-gradient(45deg, #6366f1, #8b5cf6);
    opacity: 0.1;
    animation: float 6s ease-in-out infinite;
    }

    .shape-1 {
    width: 80px;
    height: 80px;
    top: 20%;
    left: 10%;
    animation-delay: 0s;
    }

    .shape-2 {
    width: 120px;
    height: 120px;
    top: 60%;
    right: 15%;
    animation-delay: 2s;
    }

    .shape-3 {
    width: 60px;
    height: 60px;
    bottom: 30%;
    left: 20%;
    animation-delay: 4s;
    }

    .shape-4 {
    width: 100px;
    height: 100px;
    top: 10%;
    right: 30%;
    animation-delay: 1s;
    }

    @keyframes float {
    0%, 100% { transform: translateY(0px) rotate(0deg); }
    50% { transform: translateY(-20px) rotate(180deg); }
    }

    /* Login card */
    .login-card-wrapper {
    position: relative;
    z-index: 1;
    max-width: 420px;
    width: 100%;
    margin: 0 1rem;
    }

    .login-card {
    color: black;    
    background: rgba(255, 255, 255, 0.05);
    backdrop-filter: blur(20px);
    border: 1px solid rgba(255, 255, 255, 0.1);
    border-radius: 24px;
    padding: 2.5rem;
    box-shadow: 
        0 20px 40px rgba(0, 0, 0, 0.3),
        inset 0 1px 0 rgba(255, 255, 255, 0.1);
    animation: slideUp 0.8s ease-out;
    }

    @keyframes slideUp {
    from {
        opacity: 0;
        transform: translateY(50px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
    }

    /* Header */
    .card-header {
    text-align: center;
    margin-bottom: 2rem;
    }

    .logo-container {
    margin-bottom: 1.5rem;
    }

    .logo-icon {
    display: inline-block;
    animation: pulse 2s ease-in-out infinite;
    }

    @keyframes pulse {
    0%, 100% { transform: scale(1); }
    50% { transform: scale(1.05); }
    }

    .title {
    font-size: 2rem;
    font-weight: 800;
    background: linear-gradient(135deg, #d580dd 0%, #a855f7 100%);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
    margin-bottom: 0.5rem;
    }

    .subtitle {
    color: black;
    font-size: 0.95rem;
    }

    /* Form */
    .form-container {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
    }

    .input-group {
            color: black;
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    }

    .input-wrapper {
    position: relative;
    display: flex;
    align-items: center;
    background: rgba(255, 255, 255, 0.05);
    border: 2px solid rgba(255, 255, 255, 0.1);
    border-radius: 16px;
    transition: all 0.3s ease;
    overflow: hidden;
    }

    .input-wrapper.focused {
    border-color: #6366f1;
    background: rgba(99, 102, 241, 0.1);
    transform: translateY(-2px);
    box-shadow: 0 10px 30px rgba(99, 102, 241, 0.3);
    }

    .input-wrapper.error {
    border-color: #ef4444;
    background: rgba(239, 68, 68, 0.1);
    animation: shake 0.5s ease-in-out;
    }

    @keyframes shake {
    0%, 100% { transform: translateX(0); }
    25% { transform: translateX(-8px); }
    75% { transform: translateX(8px); }
    }

    .input-icon {
    padding: 0 1rem;
    color: black;
    display: flex;
    align-items: center;
    }

    .styled-input {
    flex: 1;
    background: transparent;
    border: none;
    outline: none;
    padding: 1rem 0;
    color: rgb(0, 0, 0);
    font-size: 1rem;
    }

    .styled-input::placeholder {
    color: rgb(141, 126, 126);
    }

    .password-toggle {
    background: none;
    border: none;
    color: rgba(0, 0, 0, 0.5);
    cursor: pointer;
    padding: 0 1rem;
    transition: color 0.3s ease;
    }

    .password-toggle:hover {
    color: rgba(0, 0, 0, 0.8);
    }

    .error-message {
    color: #ef4444;
    font-size: 0.875rem;
    margin-left: 0.5rem;
    animation: fadeIn 0.3s ease-in;
    }

    @keyframes fadeIn {
    from { opacity: 0; transform: translateY(-10px); }
    to { opacity: 1; transform: translateY(0); }
    }

    /* Global error */
    .global-error {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    background: rgba(239, 68, 68, 0.1);
    border: 1px solid rgba(239, 68, 68, 0.3);
    border-radius: 12px;
    padding: 1rem;
    color: #ef4444;
    font-size: 0.875rem;
    }

    /* Submit button */
    .submit-btn {
    position: relative;
    background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
    border: none;
    border-radius: 16px;
    padding: 1rem 2rem;
    color: white;
    font-weight: 600;
    font-size: 1rem;
    cursor: pointer;
    overflow: hidden;
    transition: all 0.3s ease;
    margin-top: 1rem;
    }

    .submit-btn:hover:not(:disabled) {
    transform: translateY(-2px);
    box-shadow: 0 15px 35px rgba(99, 102, 241, 0.4);
    }

    .submit-btn:disabled {
    opacity: 0.7;
    cursor: not-allowed;
    }

    .btn-content {
    position: relative;
    z-index: 2;
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 0.5rem;
    }

    .btn-bg {
    position: absolute;
    top: 0;
    left: -100%;
    width: 100%;
    height: 100%;
    background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
    transition: left 0.5s ease;
    }

    .submit-btn:hover:not(:disabled) .btn-bg {
    left: 100%;
    }

    .loading-spinner {
    animation: spin 1s linear infinite;
    }

    @keyframes spin {
    from { transform: rotate(0deg); }
    to { transform: rotate(360deg); }
    }

    /* Footer */
    .card-footer {
    text-align: center;
    margin-top: 2rem;
    padding-top: 1.5rem;
    border-top: 1px solid rgba(255, 255, 255, 0.1);
    }

    .card-footer p {
    color: rgba(255, 255, 255, 0.7);
    font-size: 0.875rem;
    }

    .signup-link {
    color: #6366f1;
    text-decoration: none;
    font-weight: 600;
    transition: color 0.3s ease;
    }

    .signup-link:hover {
    color: #8b5cf6;
    text-decoration: underline;
    }

    /* Responsive design */
    @media (max-width: 480px) {
    .login-card {
        padding: 2rem 1.5rem;
        margin: 1rem;
    }
    
    .title {
        font-size: 1.75rem;
    }
    }
</style>