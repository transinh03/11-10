<template>
  <div class="min-h-screen flex items-center justify-center login-container relative overflow-hidden">
    <div class="login-card-wrapper">
      <div class="login-card">
        <div class="card-header">
          <h1 class="title">Đổi mật khẩu</h1>
          <p class="subtitle">Vui lòng nhập mật khẩu hiện tại và mật khẩu mới</p>
        </div>
        <form @submit.prevent="handleChangePassword" class="form-container">
          <!-- Trường mật khẩu hiện tại -->
          <div class="input-group">
            <div class="input-wrapper" :class="{ error: errors.oldPassword, focused: oldFocused }">
              <div class="input-icon">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
                  <rect x="3" y="11" width="18" height="11" rx="2" ry="2" stroke="currentColor" stroke-width="2"/>
                  <circle cx="12" cy="16" r="1" fill="currentColor"/>
                  <path d="M7 11V7a5 5 0 0 1 10 0v4" stroke="currentColor" stroke-width="2"/>
                </svg>
              </div>
              <input
                v-model="form.oldPassword"
                :type="showOldPassword ? 'text' : 'password'"
                autocomplete="current-password"
                required
                class="styled-input"
                placeholder="Mật khẩu hiện tại"
                @focus="oldFocused = true"
                @blur="oldFocused = false"
              />
              <span class="eye-icon" @click="showOldPassword = !showOldPassword">
                <svg v-if="showOldPassword" xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="none" viewBox="0 0 24 24"><path stroke="#888" stroke-width="2" d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7S2 12 2 12Z"/><circle cx="12" cy="12" r="3" stroke="#888" stroke-width="2"/></svg>
                <svg v-else xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="none" viewBox="0 0 24 24"><path stroke="#888" stroke-width="2" d="M17.94 17.94C16.11 19.25 14.13 20 12 20c-6.5 0-10-8-10-8a21.8 21.8 0 0 1 5.06-6.06M9.88 9.88A3 3 0 0 1 12 9c1.66 0 3 1.34 3 3 0 .42-.09.82-.24 1.18"/><path stroke="#888" stroke-width="2" d="m1 1 22 22"/></svg>
              </span>
            </div>
            <div class="error-message" v-if="errors.oldPassword">{{ errors.oldPassword }}</div>
          </div>
          <!-- Trường mật khẩu mới -->
          <div class="input-group">
            <div class="input-wrapper" :class="{ error: errors.newPassword, focused: newFocused }">
              <div class="input-icon">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
                  <rect x="3" y="11" width="18" height="11" rx="2" ry="2" stroke="currentColor" stroke-width="2"/>
                  <circle cx="12" cy="16" r="1" fill="currentColor"/>
                  <path d="M7 11V7a5 5 0 0 1 10 0v4" stroke="currentColor" stroke-width="2"/>
                </svg>
              </div>
              <input
                v-model="form.newPassword"
                :type="showNewPassword ? 'text' : 'password'"
                autocomplete="new-password"
                required
                class="styled-input"
                placeholder="Mật khẩu mới"
                @focus="newFocused = true"
                @blur="newFocused = false"
              />
              <span class="eye-icon" @click="showNewPassword = !showNewPassword">
                <svg v-if="showNewPassword" xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="none" viewBox="0 0 24 24"><path stroke="#888" stroke-width="2" d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7S2 12 2 12Z"/><circle cx="12" cy="12" r="3" stroke="#888" stroke-width="2"/></svg>
                <svg v-else xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="none" viewBox="0 0 24 24"><path stroke="#888" stroke-width="2" d="M17.94 17.94C16.11 19.25 14.13 20 12 20c-6.5 0-10-8-10-8a21.8 21.8 0 0 1 5.06-6.06M9.88 9.88A3 3 0 0 1 12 9c1.66 0 3 1.34 3 3 0 .42-.09.82-.24 1.18"/><path stroke="#888" stroke-width="2" d="m1 1 22 22"/></svg>
              </span>
            </div>
            <div class="error-message" v-if="errors.newPassword">{{ errors.newPassword }}</div>
          </div>
          <!-- Trường xác nhận mật khẩu mới -->
          <div class="input-group">
            <div class="input-wrapper" :class="{ error: errors.confirmPassword, focused: confirmFocused }">
              <div class="input-icon">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
                  <rect x="3" y="11" width="18" height="11" rx="2" ry="2" stroke="currentColor" stroke-width="2"/>
                  <circle cx="12" cy="16" r="1" fill="currentColor"/>
                  <path d="M7 11V7a5 5 0 0 1 10 0v4" stroke="currentColor" stroke-width="2"/>
                </svg>
              </div>
              <input
                v-model="form.confirmPassword"
                :type="showConfirmPassword ? 'text' : 'password'"
                autocomplete="new-password"
                required
                class="styled-input"
                placeholder="Xác nhận mật khẩu mới"
                @focus="confirmFocused = true"
                @blur="confirmFocused = false"
              />
              <span class="eye-icon" @click="showConfirmPassword = !showConfirmPassword">
                <svg v-if="showConfirmPassword" xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="none" viewBox="0 0 24 24"><path stroke="#888" stroke-width="2" d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7S2 12 2 12Z"/><circle cx="12" cy="12" r="3" stroke="#888" stroke-width="2"/></svg>
                <svg v-else xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="none" viewBox="0 0 24 24"><path stroke="#888" stroke-width="2" d="M17.94 17.94C16.11 19.25 14.13 20 12 20c-6.5 0-10-8-10-8a21.8 21.8 0 0 1 5.06-6.06M9.88 9.88A3 3 0 0 1 12 9c1.66 0 3 1.34 3 3 0 .42-.09.82-.24 1.18"/><path stroke="#888" stroke-width="2" d="m1 1 22 22"/></svg>
              </span>
            </div>
            <div class="error-message" v-if="errors.confirmPassword">{{ errors.confirmPassword }}</div>
          </div>
          <div v-if="globalError" class="global-error">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
              <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2"/>
              <line x1="15" y1="9" x2="9" y2="15" stroke="currentColor" stroke-width="2"/>
              <line x1="9" y1="9" x2="15" y2="15" stroke="currentColor" stroke-width="2"/>
            </svg>
            <span>{{ globalError }}</span>
          </div>
          <div v-if="successMsg" class="global-success">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
              <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2"/>
              <polyline points="8 12 11 15 16 10" stroke="currentColor" stroke-width="2" fill="none"/>
            </svg>
            <span>{{ successMsg }}</span>
          </div>
          <button
            type="submit"
            :disabled="isLoading"
            class="submit-btn"
            :class="{ loading: isLoading }"
          >
            <div class="btn-content">
              <svg v-if="isLoading" class="loading-spinner" width="20" height="20" viewBox="0 0 24 24">
                <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2" fill="none" stroke-linecap="round" stroke-dasharray="60 30" />
              </svg>
              <span>{{ isLoading ? 'Đang xử lý...' : 'Đổi mật khẩu' }}</span>
            </div>
            <div class="btn-bg"></div>
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { authAPI } from '../services/authservice'
import notification from '../notification/notification'
import { useRouter } from 'vue-router'

const router = useRouter()
const form = reactive({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})
const errors = reactive({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})
const isLoading = ref(false)
const globalError = ref('')
const successMsg = ref('')
const oldFocused = ref(false)
const newFocused = ref(false)
const confirmFocused = ref(false)

// Thêm biến show password
const showOldPassword = ref(false)
const showNewPassword = ref(false)
const showConfirmPassword = ref(false)

const validateForm = () => {
  let valid = true
  errors.oldPassword = ''
  errors.newPassword = ''
  errors.confirmPassword = ''
  globalError.value = ''
  successMsg.value = ''

  if (!form.oldPassword) {
    errors.oldPassword = 'Vui lòng nhập mật khẩu hiện tại'
    valid = false
  }
  if (!form.newPassword) {
    errors.newPassword = 'Vui lòng nhập mật khẩu mới'
    valid = false
  } else if (form.newPassword.length < 6) {
    errors.newPassword = 'Mật khẩu mới phải ít nhất 6 ký tự'
    valid = false
  }
  if (!form.confirmPassword) {
    errors.confirmPassword = 'Vui lòng xác nhận mật khẩu mới'
    valid = false
  } else if (form.confirmPassword !== form.newPassword) {
    errors.confirmPassword = 'Mật khẩu xác nhận không khớp'
    valid = false
  }
  return valid
}

const handleChangePassword = async () => {
  if (!validateForm()) return

  isLoading.value = true
  globalError.value = ''
  successMsg.value = ''

  try {
    await authAPI.resetPassword(form.oldPassword, form.newPassword, form.confirmPassword)
    successMsg.value = 'Đổi mật khẩu thành công!'
    notification.success('Đổi mật khẩu thành công!', { timer: 1500 })
    setTimeout(() => {
      router.push('/profile')
    }, 1800)
  } catch (error) {
    globalError.value = error?.response?.data?.message || error?.message || 'Có lỗi xảy ra, vui lòng thử lại.'
    notification.error(globalError.value, { timer: 2000 })
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
/* Dùng lại style từ login.vue, chỉ thêm màu cho thông báo thành công */
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
}
.global-success {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(34,197,94,0.1);
  border: 1px solid rgba(34,197,94,0.3);
  border-radius: 12px;
  padding: 1rem;
  color: #22c55e;
  font-size: 0.875rem;
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
.eye-icon {
  cursor: pointer;
  padding: 0 0.75rem;
  display: flex;
  align-items: center;
  color: #888;
  user-select: none;
}
.eye-icon svg {
  display: block;
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