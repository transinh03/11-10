<template>
  <div class="min-h-screen flex items-center justify-center login-container relative overflow-hidden">
    <div class="login-card-wrapper">
      <div class="login-card">
        <div class="card-header">
          <h1 class="title">Quên mật khẩu</h1>
          <p class="subtitle">Nhập email để nhận mã OTP</p>
        </div>

        <form v-if="step === 1" @submit.prevent="sendOtp" class="form-container">
          <div class="input-group">
            <div class="input-wrapper" :class="{ error: errors.email, focused: emailFocused }">
              <div class="input-icon">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
                  <path d="M4 4H20C21.1 4 22 4.9 22 6V18C22 19.1 21.1 20 20 20H4C2.9 20 2 19.1 2 18V6C2 4.9 2.9 4 4 4Z" stroke="currentColor" stroke-width="2"/>
                  <polyline points="22,6 12,13 2,6" stroke="currentColor" stroke-width="2"/>
                </svg>
              </div>
              <input
                v-model="email"
                type="email"
                required
                class="styled-input"
                placeholder="Nhập email của bạn"
                @focus="emailFocused = true"
                @blur="emailFocused = false"
              />
            </div>
            <div class="error-message" v-if="errors.email">{{ errors.email }}</div>
          </div>
          <button type="submit" class="submit-btn" :disabled="isLoading">
            <div class="btn-content">
              <svg v-if="isLoading" class="loading-spinner" width="20" height="20" viewBox="0 0 24 24">
                <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2" fill="none" stroke-linecap="round" stroke-dasharray="60 30" />
              </svg>
              <span>{{ isLoading ? 'Đang gửi...' : 'Gửi OTP' }}</span>
            </div>
            <div class="btn-bg"></div>
          </button>
        </form>

        <form v-else-if="step === 2" @submit.prevent="verifyOtp" class="form-container">
          <div class="input-group">
            <div class="input-wrapper" :class="{ error: errors.otp, focused: otpFocused }">
              <div class="input-icon">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
                  <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2"/>
                  <path d="M12 8v4l3 3" stroke="currentColor" stroke-width="2"/>
                </svg>
              </div>
              <input
                v-model="otp"
                type="text"
                required
                class="styled-input"
                placeholder="Nhập mã OTP"
                @focus="otpFocused = true"
                @blur="otpFocused = false"
              />
            </div>
            <div class="error-message" v-if="errors.otp">{{ errors.otp }}</div>
          </div>
          <button type="submit" class="submit-btn" :disabled="isLoading">
            <div class="btn-content">
              <svg v-if="isLoading" class="loading-spinner" width="20" height="20" viewBox="0 0 24 24">
                <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2" fill="none" stroke-linecap="round" stroke-dasharray="60 30" />
              </svg>
              <span>{{ isLoading ? 'Đang xác thực...' : 'Xác nhận OTP' }}</span>
            </div>
            <div class="btn-bg"></div>
          </button>
        </form>

        <form v-else-if="step === 3" @submit.prevent="setNewPassword" class="form-container">
          <div class="input-group">
            <div class="input-wrapper" :class="{ error: errors.newPassword, focused: newPasswordFocused }">
              <div class="input-icon">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
                  <rect x="3" y="11" width="18" height="11" rx="2" ry="2" stroke="currentColor" stroke-width="2"/>
                  <circle cx="12" cy="16" r="1" fill="currentColor"/>
                  <path d="M7 11V7a5 5 0 0 1 10 0v4" stroke="currentColor" stroke-width="2"/>
                </svg>
              </div>
              <input
                v-model="newPassword"
                :type="showPassword ? 'text' : 'password'"
                required
                class="styled-input"
                placeholder="Nhập mật khẩu mới"
                @focus="newPasswordFocused = true"
                @blur="newPasswordFocused = false"
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
            <div class="error-message" v-if="errors.newPassword">{{ errors.newPassword }}</div>
          </div>
          <div class="input-group">
            <div class="input-wrapper" :class="{ error: errors.confirmPassword, focused: confirmPasswordFocused }">
              <div class="input-icon">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
                  <rect x="3" y="11" width="18" height="11" rx="2" ry="2" stroke="currentColor" stroke-width="2"/>
                  <circle cx="12" cy="16" r="1" fill="currentColor"/>
                  <path d="M7 11V7a5 5 0 0 1 10 0v4" stroke="currentColor" stroke-width="2"/>
                </svg>
              </div>
              <input
                v-model="confirmPassword"
                :type="showPassword ? 'text' : 'password'"
                required
                class="styled-input"
                placeholder="Xác nhận mật khẩu mới"
                @focus="confirmPasswordFocused = true"
                @blur="confirmPasswordFocused = false"
              />
            </div>
            <div class="error-message" v-if="errors.confirmPassword">{{ errors.confirmPassword }}</div>
          </div>
          <button type="submit" class="submit-btn" :disabled="isLoading">
            <div class="btn-content">
              <svg v-if="isLoading" class="loading-spinner" width="20" height="20" viewBox="0 0 24 24">
                <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2" fill="none" stroke-linecap="round" stroke-dasharray="60 30" />
              </svg>
              <span>{{ isLoading ? 'Đang đổi mật khẩu...' : 'Đổi mật khẩu' }}</span>
            </div>
            <div class="btn-bg"></div>
          </button>
        </form>

        <div v-if="globalError" class="global-error">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
            <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2"/>
            <line x1="15" y1="9" x2="9" y2="15" stroke="currentColor" stroke-width="2"/>
            <line x1="9" y1="9" x2="15" y2="15" stroke="currentColor" stroke-width="2"/>
          </svg>
          <span>{{ globalError }}</span>
        </div>
        <div v-if="successMessage" class="global-error" style="color: #22c55e; border-color: #22c55e;">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
            <circle cx="12" cy="12" r="10" stroke="#22c55e" stroke-width="2"/>
            <polyline points="8,12 11,15 16,9" stroke="#22c55e" stroke-width="2" fill="none"/>
          </svg>
          <span>{{ successMessage }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import axios from 'axios'
import { useRouter } from 'vue-router'

const API_URL = import.meta.env.VITE_BASE_URL + "/auth" 

const router = useRouter()

const step = ref(1)
const isLoading = ref(false)
const globalError = ref('')
const successMessage = ref('')

const email = ref('')
const otp = ref('')
const newPassword = ref('')
const confirmPassword = ref('')
const showPassword = ref(false)

const emailFocused = ref(false)
const otpFocused = ref(false)
const newPasswordFocused = ref(false)
const confirmPasswordFocused = ref(false)

const errors = reactive({
  email: '',
  otp: '',
  newPassword: '',
  confirmPassword: ''
})

const validateEmail = () => {
  errors.email = ''
  if (!email.value) {
    errors.email = 'Vui lòng nhập email'
    return false
  }
  if (!/\S+@\S+\.\S+/.test(email.value)) {
    errors.email = 'Email không hợp lệ'
    return false
  }
  return true
}

const validateOtp = () => {
  errors.otp = ''
  if (!otp.value) {
    errors.otp = 'Vui lòng nhập mã OTP'
    return false
  }
  return true
}

const validatePasswords = () => {
  errors.newPassword = ''
  errors.confirmPassword = ''
  if (!newPassword.value) {
    errors.newPassword = 'Vui lòng nhập mật khẩu mới'
    return false
  }
  if (newPassword.value.length < 6) {
    errors.newPassword = 'Mật khẩu phải từ 6 ký tự'
    return false
  }
  if (!confirmPassword.value) {
    errors.confirmPassword = 'Vui lòng xác nhận mật khẩu'
    return false
  }
  if (newPassword.value !== confirmPassword.value) {
    errors.confirmPassword = 'Mật khẩu xác nhận không khớp'
    return false
  }
  return true
}

const sendOtp = async () => {
  globalError.value = ''
  if (!validateEmail()) return
  isLoading.value = true
  try {
    await axios.post(`${API_URL}/forgot-password-otp/send/${encodeURIComponent(email.value)}`)
    step.value = 2
    successMessage.value = 'Đã gửi OTP đến email của bạn'
    setTimeout(() => successMessage.value = '', 2000)
  } catch (e) {
    globalError.value = e.response?.data?.message || 'Gửi OTP thất bại'
  } finally {
    isLoading.value = false
  }
}

const verifyOtp = async () => {
  globalError.value = ''
  if (!validateOtp()) return
  isLoading.value = true
  try {
    const res = await axios.post(`${API_URL}/forgot-password-otp/verifi`, {
      email: email.value,
      otp: otp.value
    })
    // Lưu token vào localStorage
    localStorage.setItem('forgot_token', res.data.token)
    step.value = 3
    successMessage.value = 'Xác thực OTP thành công'
    setTimeout(() => successMessage.value = '', 2000)
  } catch (e) {
    globalError.value = e.response?.message || 'Xác thực OTP thất bại'
  } finally {
    isLoading.value = false
  }
}

const setNewPassword = async () => {
  globalError.value = ''
  if (!validatePasswords()) return
  isLoading.value = true
  try {
    const token = localStorage.getItem('forgot_token')
    if (!token) {
      globalError.value = 'Thiếu token xác thực OTP'
      return
    }
    await axios.post(`${API_URL}/set-new-pass`, {
      newPassword: newPassword.value
    }, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })
    localStorage.removeItem('forgot_token') 
    successMessage.value = 'Đổi mật khẩu thành công! Đang chuyển hướng...'
    setTimeout(() => {
      router.push('/login')
    }, 2000)
  } catch (e) {
    globalError.value = e.response?.data?.message || 'Đổi mật khẩu thất bại'
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
/* Copy toàn bộ style từ login.vue vào đây */
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  background: white;
  position: relative;
}
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
.card-header {
  text-align: center;
  margin-bottom: 2rem;
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
  /* Thêm padding phải để tránh icon đè lên text */
  padding-right: 2.5rem;
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
.password-toggle {
  position: absolute;
  right: 0.5rem;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  color: #6366f1;
  cursor: pointer;
  padding: 0;
  display: flex;
  align-items: center;
  z-index: 2;
  transition: color 0.3s;
}
.password-toggle:hover {
  color: #a855f7;
}
.styled-input {
  flex: 1;
  background: transparent;
  border: none;
  outline: none;
  padding: 1rem 0;
  color: rgb(0, 0, 0);
  font-size: 1rem;
  /* Thêm padding phải để tránh text bị icon che */
  padding-right: 2.5rem;
}
.styled-input::placeholder {
  color: rgb(141, 126, 126);
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
  margin-top: 1rem;
}
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
