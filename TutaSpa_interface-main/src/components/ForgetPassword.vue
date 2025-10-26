<template>
  <div class="spa-forgot-password-wrapper">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-6 col-lg-4">
          <div class="spa-forgot-password-container">
            <!-- Header Section -->
            <div class="forgot-password-header text-center mb-4">
              <div class="welcome-icon mb-3">🔐</div>
              <h1 class="forgot-password-title">
                {{ currentStep === 1 ? "Quên mật khẩu" : "Đặt mật khẩu mới" }}
              </h1>
              <p class="forgot-password-subtitle">
                {{
                  currentStep === 1
                    ? "Nhập số điện thoại để nhận mã OTP"
                    : "Nhập mật khẩu mới cho tài khoản của bạn"
                }}
              </p>
            </div>

            <!-- Step 1: Phone Number & OTP -->
            <div v-if="currentStep === 1">
              <!-- Phone Number Input -->
              <!-- <div v-if="!otpSent" class="mb-3">
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
              </div> -->
              <!-- Email Input -->
              <div v-if="!otpSent" class="mb-3">
                <label for="email" class="form-label">Email</label>
                <div class="input-group">
                  <span class="input-group-text bg-light border-end-0">📧</span>
                  <input
                    type="email"
                    id="email"
                    class="form-control border-start-0"
                    v-model="form.email"
                    placeholder="Nhập email của bạn"
                    :class="{ 'is-invalid': errors.email }"
                  />
                </div>
                <div v-if="errors.email" class="invalid-feedback d-block">
                  <i class="fas fa-exclamation-circle me-1"></i>
                  {{ errors.email }}
                </div>
              </div>

              <!-- Send OTP Button -->
              <button
                v-if="!otpSent"
                type="button"
                class="btn btn-success w-100 mb-3 forgot-password-btn"
                :disabled="isLoading"
                @click="sendOTP"
              >
                <span
                  v-if="isLoading"
                  class="d-flex align-items-center justify-content-center"
                >
                  <span
                    class="spinner-border spinner-border-sm me-2"
                    role="status"
                    aria-hidden="true"
                  ></span>
                  Đang gửi OTP
                </span>
                <span
                  v-else
                  class="d-flex align-items-center justify-content-center"
                >
                  <span class="me-2">📤</span>
                  Gửi mã OTP
                </span>
              </button>

              <!-- OTP Input Section -->
              <div v-if="otpSent">
                <div class="alert alert-success mb-3" role="alert">
                  <i class="fas fa-check-circle me-2"></i>
                  Mã OTP đã được gửi đến email <strong>{{ form.email }}</strong
                  >.
                  <br />
                  <small class="text-muted">
                    ⚠️ Nếu không thấy trong hộp thư đến, vui lòng kiểm tra thêm
                    thư mục <strong>Spam / Thư rác</strong>.
                  </small>
                </div>

                <div class="mb-3">
                  <label for="otp" class="form-label">Mã OTP</label>
                  <div class="input-group">
                    <span class="input-group-text bg-light border-end-0"
                      >🔢</span
                    >
                    <input
                      type="text"
                      id="otp"
                      class="form-control border-start-0"
                      v-model="form.otp"
                      placeholder="Nhập mã OTP (6 số)"
                      maxlength="6"
                      :class="{ 'is-invalid': errors.otp }"
                    />
                  </div>
                  <div v-if="errors.otp" class="invalid-feedback d-block">
                    <i class="fas fa-exclamation-circle me-1"></i>
                    {{ errors.otp }}
                  </div>
                </div>

                <!-- Resend OTP -->
                <div class="text-center mb-3">
                  <span class="text-muted">Không nhận được mã? </span>
                  <button
                    v-if="countdown === 0"
                    type="button"
                    class="btn btn-link p-0 text-success"
                    @click="resendOTP"
                    :disabled="isLoading"
                  >
                    Gửi lại
                  </button>
                  <span v-else class="text-muted">
                    Gửi lại sau {{ countdown }}s
                  </span>
                </div>

                <!-- Verify OTP Button -->
                <button
                  type="button"
                  class="btn btn-success w-100 mb-3 forgot-password-btn"
                  :disabled="isLoading"
                  @click="verifyOTP"
                >
                  <span
                    v-if="isLoading"
                    class="d-flex align-items-center justify-content-center"
                  >
                    <span
                      class="spinner-border spinner-border-sm me-2"
                      role="status"
                      aria-hidden="true"
                    ></span>
                    Đang xác thực...
                  </span>
                  <span
                    v-else
                    class="d-flex align-items-center justify-content-center"
                  >
                    <span class="me-2">✅</span>
                    Xác thực OTP
                  </span>
                </button>
              </div>
            </div>

            <!-- Step 2: New Password -->
            <div v-if="currentStep === 2">
              <form @submit.prevent="setNewPassword" class="new-password-form">
                <!-- New Password -->
                <div class="mb-3">
                  <label for="newPassword" class="form-label"
                    >Mật khẩu mới</label
                  >
                  <div class="input-group">
                    <span class="input-group-text bg-light border-end-0"
                      >🔒</span
                    >
                    <input
                      :type="showNewPassword ? 'text' : 'password'"
                      id="newPassword"
                      class="form-control border-start-0 border-end-0"
                      v-model="form.newPassword"
                      placeholder="Nhập mật khẩu mới"
                      :class="{ 'is-invalid': errors.newPassword }"
                    />
                    <button
                      class="btn btn-outline-secondary border-start-0"
                      type="button"
                      @click="toggleNewPassword"
                    >
                      {{ showNewPassword ? "🙈" : "👁️" }}
                    </button>
                  </div>
                  <div
                    v-if="errors.newPassword"
                    class="invalid-feedback d-block"
                  >
                    <i class="fas fa-exclamation-circle me-1"></i>
                    {{ errors.newPassword }}
                  </div>
                </div>

                <!-- Confirm Password -->
                <div class="mb-3">
                  <label for="confirmPassword" class="form-label"
                    >Xác nhận mật khẩu</label
                  >
                  <div class="input-group">
                    <span class="input-group-text bg-light border-end-0"
                      >🔒</span
                    >
                    <input
                      :type="showConfirmPassword ? 'text' : 'password'"
                      id="confirmPassword"
                      class="form-control border-start-0 border-end-0"
                      v-model="form.confirmPassword"
                      placeholder="Xác nhận mật khẩu mới"
                      :class="{ 'is-invalid': errors.confirmPassword }"
                    />
                    <button
                      class="btn btn-outline-secondary border-start-0"
                      type="button"
                      @click="toggleConfirmPassword"
                    >
                      {{ showConfirmPassword ? "🙈" : "👁️" }}
                    </button>
                  </div>
                  <div
                    v-if="errors.confirmPassword"
                    class="invalid-feedback d-block"
                  >
                    <i class="fas fa-exclamation-circle me-1"></i>
                    {{ errors.confirmPassword }}
                  </div>
                </div>

                <!-- Set New Password Button -->
                <button
                  type="submit"
                  class="btn btn-success w-100 mb-3 forgot-password-btn"
                  :disabled="isLoading"
                >
                  <span
                    v-if="isLoading"
                    class="d-flex align-items-center justify-content-center"
                  >
                    <span
                      class="spinner-border spinner-border-sm me-2"
                      role="status"
                      aria-hidden="true"
                    ></span>
                    Đang cập nhật...
                  </span>
                  <span
                    v-else
                    class="d-flex align-items-center justify-content-center"
                  >
                    <span class="me-2">🔐</span>
                    Đặt mật khẩu mới
                  </span>
                </button>
              </form>
            </div>

            <!-- Back to Login -->
            <div class="text-center">
              <a
                href="#"
                @click.prevent="goToLogin"
                class="text-success text-decoration-none"
              >
                ← Quay lại đăng nhập
              </a>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Toast Notifications -->
    <div class="toast-container position-fixed top-0 end-0 p-3">
      <div
        v-for="toast in toasts"
        :key="toast.id"
        :class="['toast', 'show', `toast-${toast.type}`]"
        role="alert"
      >
        <div class="toast-header">
          <i :class="getToastIcon(toast.type)" class="me-2"></i>
          <strong class="me-auto">{{ toast.title || "Thông báo" }}</strong>
          <button
            type="button"
            class="btn-close"
            @click="removeToast(toast.id)"
          ></button>
        </div>
        <div class="toast-body">
          {{ toast.message }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onUnmounted } from "vue";
import { useRouter } from "vue-router";
import axios from "axios";

const API_URL = import.meta.env.VITE_BASE_URL;

// Router instance
const router = useRouter();

// Reactive data
const currentStep = ref(1);
const otpSent = ref(false);
const countdown = ref(0);
const isLoading = ref(false);
const showNewPassword = ref(false);
const showConfirmPassword = ref(false);
const toasts = ref([]);
let countdownInterval = null;

const form = reactive({
  email: "",
  otp: "",
  newPassword: "",
  confirmPassword: "",
});

const errors = reactive({
  email: "",
  otp: "",
  newPassword: "",
  confirmPassword: "",
});

// Toast methods
const showToast = (type, message, title = "", duration = 5000) => {
  const toast = {
    id: Date.now(),
    type,
    message,
    title,
  };

  toasts.value.push(toast);

  setTimeout(() => {
    removeToast(toast.id);
  }, duration);
};

const removeToast = (id) => {
  const index = toasts.value.findIndex((toast) => toast.id === id);
  if (index > -1) {
    toasts.value.splice(index, 1);
  }
};

const getToastIcon = (type) => {
  switch (type) {
    case "success":
      return "fas fa-check-circle text-success";
    case "error":
      return "fas fa-times-circle text-danger";
    case "warning":
      return "fas fa-exclamation-triangle text-warning";
    default:
      return "fas fa-info-circle text-info";
  }
};

// Validation methods
const validatePhone = () => {
  errors.phone = "";

  if (!form.phone.trim()) {
    errors.phone = "Vui lòng nhập số điện thoại";
    return false;
  }

  if (!/^[0-9]{10,11}$/.test(form.phone.replace(/\s/g, ""))) {
    errors.phone = "Số điện thoại không hợp lệ";
    return false;
  }

  return true;
};
// Validation email
const validateEmail = () => {
  errors.email = "";

  if (!form.email.trim()) {
    errors.email = "Vui lòng nhập email";
    return false;
  }

  // Regex đơn giản để check email hợp lệ
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(form.email)) {
    errors.email = "Email không hợp lệ";
    return false;
  }

  return true;
};

const validateOTP = () => {
  errors.otp = "";

  if (!form.otp.trim()) {
    errors.otp = "Vui lòng nhập mã OTP";
    return false;
  }

  if (!/^[0-9]{6}$/.test(form.otp)) {
    errors.otp = "Mã OTP phải có 6 số";
    return false;
  }

  return true;
};

const validateNewPassword = () => {
  let isValid = true;

  // Reset errors
  errors.newPassword = "";
  errors.confirmPassword = "";

  // Validate new password
  if (!form.newPassword.trim()) {
    errors.newPassword = "Vui lòng nhập mật khẩu mới";
    isValid = false;
  } else if (form.newPassword.length < 6) {
    errors.newPassword = "Mật khẩu phải có ít nhất 6 ký tự";
    isValid = false;
  }

  // Validate confirm password
  if (!form.confirmPassword.trim()) {
    errors.confirmPassword = "Vui lòng xác nhận mật khẩu";
    isValid = false;
  } else if (form.newPassword !== form.confirmPassword) {
    errors.confirmPassword = "Mật khẩu xác nhận không khớp";
    isValid = false;
  }

  return isValid;
};

// Countdown methods
const startCountdown = () => {
  countdown.value = 60;
  countdownInterval = setInterval(() => {
    countdown.value--;
    if (countdown.value <= 0) {
      clearInterval(countdownInterval);
    }
  }, 1000);
};

// API methods
const sendOTP = async () => {
  if (!validateEmail()) return;

  isLoading.value = true;

  try {
    await axios.post(
      `${API_URL}/account/forgot-password-otp/sendMail/${form.email}`
    );

    otpSent.value = true;
    startCountdown();
    showToast(
      "success",
      `Mã OTP đã được gửi đến email ${form.email}`,
      "Thành công"
    );
  } catch (error) {
    const message =
      error.response?.data?.message || "Gửi OTP thất bại. Vui lòng thử lại!";
    showToast("error", message, "Lỗi");
  } finally {
    isLoading.value = false;
  }
};

const resendOTP = async () => {
  if (countdown.value > 0) return;

  isLoading.value = true;

  try {
    await axios.post(
      `${API_URL}/account/forgot-password-otp/sendMail/${form.email}`
    );

    startCountdown();
    showToast("success", "Mã OTP đã được gửi lại", "Thành công");
  } catch (error) {
    const message =
      error.response?.data?.message ||
      "Gửi lại OTP thất bại. Vui lòng thử lại!";
    showToast("error", message, "Lỗi");
  } finally {
    isLoading.value = false;
  }
};

const verifyOTP = async () => {
  if (!validateOTP()) return;

  isLoading.value = true;

  try {
    const response = await axios.post(
      `${API_URL}/account/forgot-password-otp/verifiMail`,
      {
        Email: form.email,
        Otp: form.otp,
      }
    );

    // Lưu reset token
    localStorage.setItem("resetToken", response.data.token);

    showToast("success", "Xác thực OTP thành công!", "Thành công");

    setTimeout(() => {
      currentStep.value = 2;
    }, 1500);
  } catch (error) {
    const message =
      error.response?.data?.message ||
      "Xác thực OTP thất bại. Vui lòng thử lại!";
    showToast("error", message, "Lỗi");
  } finally {
    isLoading.value = false;
  }
};

const setNewPassword = async () => {
  if (!validateNewPassword()) return;

  const resetToken = localStorage.getItem("resetToken");
  if (!resetToken) {
    showToast(
      "error",
      "Token không hợp lệ. Vui lòng thực hiện lại từ đầu.",
      "Lỗi"
    );
    currentStep.value = 1;
    otpSent.value = false;
    return;
  }

  isLoading.value = true;

  try {
    await axios.post(
      `${API_URL}/account/set-new-pass`,
      {
        NewPassword: form.newPassword,
        ConfirmPassword: form.confirmPassword,
      },
      {
        headers: {
          Authorization: `Bearer ${resetToken}`,
        },
      }
    );

    // Clear reset token
    localStorage.removeItem("resetToken");

    showToast(
      "success",
      "Đặt mật khẩu mới thành công! Đang chuyển đến trang đăng nhập...",
      "Thành công"
    );

    // Redirect to login after delay
    setTimeout(() => {
      router.push("/login");
    }, 2000);
  } catch (error) {
    const message =
      error.response?.data?.message ||
      "Đặt mật khẩu mới thất bại. Vui lòng thử lại!";
    showToast("error", message, "Lỗi");
  } finally {
    isLoading.value = false;
  }
};

// Toggle password visibility
const toggleNewPassword = () => {
  showNewPassword.value = !showNewPassword.value;
};

const toggleConfirmPassword = () => {
  showConfirmPassword.value = !showConfirmPassword.value;
};

// Navigation
const goToLogin = () => {
  router.push("/login");
};

// Cleanup
onUnmounted(() => {
  if (countdownInterval) {
    clearInterval(countdownInterval);
  }
});
</script>

<style scoped>
/* Import Bootstrap CSS if not already included */
@import url("https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css");

.spa-forgot-password-wrapper {
  min-height: 100vh;
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  display: flex;
  align-items: center;
  padding: 2rem 0;
}

.spa-forgot-password-container {
  background: white;
  border-radius: 15px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
  padding: 2rem;
  border: 1px solid #e9ecef;
  width: 100%;
  margin: 0 auto;
}

.welcome-icon {
  font-size: 3rem;
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.05);
  }
  100% {
    transform: scale(1);
  }
}

.forgot-password-title {
  color: #198754;
  font-weight: 700;
  margin-bottom: 0.5rem;
}

.forgot-password-subtitle {
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

.forgot-password-btn {
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

.forgot-password-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(25, 135, 84, 0.4);
  background: linear-gradient(45deg, #157347, #0dcaf0);
}

.forgot-password-btn:active:not(:disabled) {
  transform: translateY(0);
}

.forgot-password-btn:disabled {
  opacity: 0.8;
  transform: none !important;
  box-shadow: none !important;
}

.btn-link {
  border: none;
  background: none;
  text-decoration: underline;
}

.btn-link:hover {
  text-decoration: underline;
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

.alert {
  border-radius: 10px;
  border: none;
}

.alert-success {
  background: linear-gradient(
    45deg,
    rgba(25, 135, 84, 0.1),
    rgba(32, 201, 151, 0.1)
  );
  color: #198754;
}

/* Toast Styles */
.toast-container {
  z-index: 1050;
}

.toast {
  border-radius: 10px;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
  border: none;
}

.toast-success {
  background: linear-gradient(
    45deg,
    rgba(25, 135, 84, 0.95),
    rgba(32, 201, 151, 0.95)
  );
  color: white;
}

.toast-error {
  background: linear-gradient(
    45deg,
    rgba(220, 53, 69, 0.95),
    rgba(239, 68, 68, 0.95)
  );
  color: white;
}

.toast-warning {
  background: linear-gradient(
    45deg,
    rgba(255, 193, 7, 0.95),
    rgba(251, 191, 36, 0.95)
  );
  color: white;
}

.toast-info {
  background: linear-gradient(
    45deg,
    rgba(13, 110, 253, 0.95),
    rgba(59, 130, 246, 0.95)
  );
  color: white;
}

.toast-header {
  background: rgba(255, 255, 255, 0.1);
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
  color: inherit;
}

.toast .btn-close {
  filter: invert(1);
}

/* Responsive */
@media (max-width: 768px) {
  .spa-forgot-password-wrapper {
    padding: 1rem 0;
  }

  .spa-forgot-password-container {
    margin: 0.5rem;
    padding: 1.5rem 1rem;
    border-radius: 12px;
  }

  .forgot-password-title {
    font-size: 1.4rem;
  }

  .forgot-password-subtitle {
    font-size: 0.9rem;
    line-height: 1.4;
  }

  .welcome-icon {
    font-size: 2.5rem;
  }

  .forgot-password-btn {
    font-size: 1rem;
    padding: 0.875rem 1rem;
  }

  .form-control {
    font-size: 16px; /* Prevents zoom on iOS */
    padding: 0.875rem 0.75rem;
  }

  .input-group-text {
    padding: 0.875rem 0.75rem;
  }

  .toast-container {
    left: 0.5rem;
    right: 0.5rem;
    top: 1rem !important;
  }

  .toast {
    font-size: 0.9rem;
  }

  .alert {
    font-size: 0.9rem;
    padding: 0.75rem;
  }

  .form-label {
    font-size: 0.9rem;
    font-weight: 600;
  }
}

@media (max-width: 480px) {
  .spa-forgot-password-container {
    margin: 0.25rem;
    padding: 1.25rem 0.875rem;
  }

  .forgot-password-title {
    font-size: 1.3rem;
  }

  .welcome-icon {
    font-size: 2.2rem;
  }

  .forgot-password-btn {
    font-size: 0.95rem;
    padding: 0.8rem 0.875rem;
  }

  .form-control {
    font-size: 16px;
    padding: 0.8rem 0.625rem;
  }

  .input-group-text {
    padding: 0.8rem 0.625rem;
    font-size: 0.9rem;
  }

  .toast-container {
    left: 0.25rem;
    right: 0.25rem;
  }
}

/* Step transition animation */
.new-password-form {
  animation: slideIn 0.5s ease-in-out;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateX(20px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

/* Touch-friendly improvements for mobile */
@media (hover: none) and (pointer: coarse) {
  .forgot-password-btn:hover {
    transform: none;
    box-shadow: none;
    background: linear-gradient(45deg, #198754, #20c997);
  }

  .forgot-password-btn:active {
    transform: scale(0.98);
    transition: transform 0.1s ease;
  }

  .btn-link:hover {
    text-decoration: none;
  }

  .text-success:hover {
    color: #198754 !important;
    text-decoration: none !important;
  }
}

/* High DPI displays */
@media (-webkit-min-device-pixel-ratio: 2), (min-resolution: 192dpi) {
  .spa-forgot-password-container {
    box-shadow: 0 8px 25px rgba(0, 0, 0, 0.08);
  }
}

/* Dark mode support */
@media (prefers-color-scheme: dark) {
  .spa-forgot-password-wrapper {
    background: linear-gradient(135deg, #1a1a1a 0%, #2d2d2d 100%);
  }

  .spa-forgot-password-container {
    background: #2d2d2d;
    border-color: #404040;
    color: #ffffff;
  }

  .forgot-password-title {
    color: #22c55e;
  }

  .forgot-password-subtitle {
    color: #9ca3af;
  }

  .input-group-text {
    background-color: #374151 !important;
    border-color: #4b5563;
    color: #ffffff;
  }

  .form-control {
    background-color: #374151;
    border-color: #4b5563;
    color: #ffffff;
  }

  .form-control:focus {
    background-color: #374151;
    border-color: #22c55e;
    color: #ffffff;
  }

  .alert-success {
    background: linear-gradient(
      45deg,
      rgba(34, 197, 94, 0.15),
      rgba(16, 185, 129, 0.15)
    );
    color: #22c55e;
    border-color: rgba(34, 197, 94, 0.3);
  }
}
</style>
