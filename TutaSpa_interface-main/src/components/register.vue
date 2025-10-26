<template>
  <div class="spa-register-wrapper">
    <div class="spa-register-container">
      <!-- Header Section -->
      <div class="register-header">
        <div class="welcome-icon">🌱</div>
        <h1 class="register-title">Tham gia cùng chúng tôi</h1>
        <p class="register-subtitle">
          Tạo tài khoản mới để bắt đầu hành trình chăm sóc sức khỏe
        </p>
      </div>

      <!-- Register Form -->
      <form @submit.prevent="handleRegister" class="register-form">
        <!-- Name Input -->
        <div class="form-group">
          <label for="name" class="form-label">Họ và tên</label>
          <div class="input-container">
            <div class="input-icon">👤</div>
            <input
              type="text"
              id="name"
              class="form-input"
              v-model="form.name"
              placeholder="Nhập họ và tên"
              :class="{ error: errors.name }"
            />
          </div>
          <transition name="fade">
            <div v-if="errors.name" class="error-message">
              <i class="fas fa-exclamation-circle"></i>
              {{ errors.name }}
            </div>
          </transition>
        </div>
        <!-- Email Input -->
        <div class="form-group">
          <label for="email" class="form-label">Email</label>
          <div class="input-container">
            <div class="input-icon">📧</div>
            <input
              type="email"
              id="email"
              class="form-input"
              v-model="form.email"
              placeholder="Nhập địa chỉ email"
              :class="{ error: errors.email }"
            />
          </div>
          <transition name="fade">
            <div v-if="errors.email" class="error-message">
              <i class="fas fa-exclamation-circle"></i>
              {{ errors.email }}
            </div>
          </transition>
        </div>

        <!-- Phone Input -->
        <div class="form-group">
          <label for="phone" class="form-label">Số điện thoại</label>
          <div class="input-container">
            <div class="input-icon">📱</div>
            <input
              type="tel"
              id="phone"
              class="form-input"
              v-model="form.phone"
              placeholder="Nhập số điện thoại"
              :class="{ error: errors.phone }"
              :disabled="otpSent"
            />
          </div>
          <transition name="fade">
            <div v-if="errors.phone" class="error-message">
              <i class="fas fa-exclamation-circle"></i>
              {{ errors.phone }}
            </div>
          </transition>
        </div>

        <!-- OTP Section -->
        <!-- <div class="form-group">
          <button
            type="button"
            class="otp-btn"
            @click="sendOTP"
            :disabled="isLoadingOTP || !form.phone || otpSent"
          >
            <div v-if="isLoadingOTP" class="btn-content">
              <span>Đang gửi...</span>
            </div>
            <div v-else-if="otpSent" class="btn-content">
              <span>✅</span>
              <span>Đã gửi OTP</span>
            </div>
            <div v-else class="btn-content">
              <span>📨</span>
              <span>Gửi mã OTP</span>
            </div>
          </button> -->

        <!-- OTP Input -->
        <!--<div v-if="otpSent" class="otp-input-section">
            <label for="otp" class="form-label">Mã OTP</label>
            <div class="input-container otp-container">
              <div class="input-icon">🛡️</div>
              <input
                type="text"
                id="otp"
                class="form-input"
                v-model="form.otp"
                placeholder="Nhập mã OTP (6 số)"
                :class="{ error: errors.otp, verified: otpVerified }"
                maxlength="6"
                :disabled="otpVerified"
              />
              <div v-if="!otpVerified" class="otp-actions">
                <button
                  type="button"
                  class="resend-btn"
                  @click="resendOTP"
                  :disabled="countdown > 0"
                >
                  {{ countdown > 0 ? `${countdown}s` : "Gửi lại" }}
                </button>
                <button
                  type="button"
                  class="verify-btn"
                  @click="verifyOTP"
                  :disabled="isVerifyingOTP || !form.otp"
                >
                  <div
                    v-if="isVerifyingOTP"
                    class="loading-spinner-small"
                  ></div>
                  <span v-else>✓</span>
                </button>
              </div>
              <div v-if="otpVerified" class="verified-badge">
                <span>✅ Đã xác nhận</span>
              </div>
            </div>
            <transition name="fade">
              <div v-if="errors.otp" class="error-message">
                <i class="fas fa-exclamation-circle"></i>
                {{ errors.otp }}
              </div>
            </transition>
          </div>
        </div> -->

        <!-- OTP mail -->
        <div class="form-group">
          <button
            type="button"
            class="otp-btn"
            @click="sendOTPMail"
            :disabled="isLoadingOTPMail || !form.email || otpSentMail"
          >
            <div v-if="isLoadingOTPMail" class="btn-content">
              <span>Đang gửi...</span>
            </div>
            <div v-else-if="otpSentMail" class="btn-content">
              <span>✅</span>
              <span>Đã gửi OTP</span>
            </div>
            <div v-else class="btn-content">
              <span>📨</span>
              <span>Gửi mã OTP mail</span>
            </div>
          </button>

          <!-- OTP Input -->
          <div v-if="otpSentMail" class="otp-input-section">
            <label for="otpMail" class="form-label">Mã OTP Email</label>
            <div class="input-container otp-container">
              <div class="input-icon">🛡️</div>
              <input
                type="text"
                id="otpMail"
                class="form-input"
                v-model="form.otp"
                placeholder="Nhập mã OTP (6 số)"
                :class="{ error: errors.otp, verified: otpVerifiedMail }"
                maxlength="6"
                :disabled="otpVerifiedMail"
              />
              <div v-if="!otpVerifiedMail" class="otp-actions">
                <button
                  type="button"
                  class="resend-btn"
                  @click="resendOTPMail"
                  :disabled="countdownMail > 0"
                >
                  {{ countdownMail > 0 ? `${countdownMail}s` : "Gửi lại" }}
                </button>
                <button
                  type="button"
                  class="verify-btn"
                  @click="verifyOTPMail"
                  :disabled="isVerifyingOTP || !form.otp"
                >
                  <div
                    v-if="isVerifyingOTP"
                    class="loading-spinner-small"
                  ></div>
                  <span v-else>✓</span>
                </button>
              </div>
              <div v-if="otpVerifiedMail" class="verified-badge">
                <span>✅ Đã xác nhận</span>
              </div>
            </div>
            <transition name="fade">
              <div v-if="errors.otp" class="error-message">
                <i class="fas fa-exclamation-circle"></i>
                {{ errors.otp }}
              </div>
            </transition>
          </div>
        </div>

        <!-- Password Input -->
        <div class="form-group">
          <label for="password" class="form-label">Mật khẩu</label>
          <div class="input-container">
            <div class="input-icon">🔒</div>
            <input
              :type="showPassword ? 'text' : 'password'"
              id="password"
              class="form-input"
              v-model="form.password"
              placeholder="Nhập mật khẩu"
              :class="{ error: errors.password }"
            />
            <div class="password-toggle" @click="togglePassword">
              {{ showPassword ? "🙈" : "👁️" }}
            </div>
          </div>
          <transition name="fade">
            <div v-if="errors.password" class="error-message">
              <i class="fas fa-exclamation-circle"></i>
              {{ errors.password }}
            </div>
          </transition>
        </div>

        <!-- Confirm Password Input -->
        <div class="form-group">
          <label for="confirmPassword" class="form-label"
            >Xác nhận mật khẩu</label
          >
          <div class="input-container">
            <div class="input-icon">🔐</div>
            <input
              :type="showConfirmPassword ? 'text' : 'password'"
              id="confirmPassword"
              class="form-input"
              v-model="form.confirmPassword"
              placeholder="Nhập lại mật khẩu"
              :class="{ error: errors.confirmPassword }"
            />
            <div class="password-toggle" @click="toggleConfirmPassword">
              {{ showConfirmPassword ? "🙈" : "👁️" }}
            </div>
          </div>
          <transition name="fade">
            <div v-if="errors.confirmPassword" class="error-message">
              <i class="fas fa-exclamation-circle"></i>
              {{ errors.confirmPassword }}
            </div>
          </transition>
        </div>

        <!-- Register Button -->
        <button
          type="submit"
          class="register-btn"
          :disabled="isLoading || !otpVerifiedMail"
        >
          <!-- !otpVerified -->
          <div v-if="isLoading" class="btn-content">
            <span>Đang đăng ký...</span>
          </div>
          <div v-else class="btn-content">
            <span>✨</span>
            <span>Đăng ký</span>
          </div>
        </button>

        <!-- Divider -->
        <div class="divider">
          <span>Hoặc</span>
        </div>

        <!-- Login Link -->
        <div class="login-section">
          <p class="login-text">Đã có tài khoản?</p>
          <a href="#" @click.prevent="goToLogin" class="login-btn-link">
            <span>🌸</span>
            <span>Đăng nhập ngay</span>
          </a>
        </div>
      </form>

      <!-- Decorative Elements -->
      <div class="decorative-elements">
        <div class="leaf leaf-1">🍃</div>
        <div class="leaf leaf-2">🌱</div>
        <div class="leaf leaf-3">🌿</div>
        <div class="leaf leaf-4">🌾</div>
      </div>
    </div>
  </div>
</template>

<script setup>
import notification from "../notification";
import { ref, reactive, onUnmounted } from "vue";
import { useRouter } from "vue-router";
import { authAPI } from "../services/authservice";

// Router instance
const router = useRouter();

// Reactive data
const form = reactive({
  name: "",
  phone: "",
  email: "",
  otp: "",
  password: "",
  confirmPassword: "",
});

const errors = reactive({
  name: "",
  phone: "",
  email: "",
  otp: "",
  password: "",
  confirmPassword: "",
});

// Refs
const showPassword = ref(false);
const showConfirmPassword = ref(false);
const isLoading = ref(false);
const isLoadingOTP = ref(false);
const isVerifyingOTP = ref(false);
const otpSent = ref(false);
const otpVerified = ref(false);
const countdown = ref(0);
let countdownInterval = null;
const isLoadingOTPMail = ref(false);
const otpSentMail = ref(false);
const otpVerifiedMail = ref(false);
const countdownMail = ref(0);
let countdownIntervalMail = null;

// Methods
const validateForm = () => {
  let isValid = true;

  // Reset errors
  errors.name = "";
  errors.phone = "";
  errors.password = "";
  errors.confirmPassword = "";

  // Validate name
  if (!form.name.trim()) {
    errors.name = "Vui lòng nhập họ và tên";
    isValid = false;
  } else if (form.name.trim().length < 2) {
    errors.name = "Họ và tên phải có ít nhất 2 ký tự";
    isValid = false;
  }

  // Validate phone
  if (!form.phone.trim()) {
    errors.phone = "Vui lòng nhập số điện thoại";
    isValid = false;
  } else if (!/^[0-9]{10,11}$/.test(form.phone.replace(/\s/g, ""))) {
    errors.phone = "Số điện thoại không hợp lệ";
    isValid = false;
  }

  // Validate password
  if (!form.password.trim()) {
    errors.password = "Vui lòng nhập mật khẩu";
    isValid = false;
  } else if (form.password.length < 6) {
    errors.password = "Mật khẩu phải có ít nhất 6 ký tự";
    isValid = false;
  }

  // Validate confirm password
  if (!form.confirmPassword.trim()) {
    errors.confirmPassword = "Vui lòng xác nhận mật khẩu";
    isValid = false;
  } else if (form.password !== form.confirmPassword) {
    errors.confirmPassword = "Mật khẩu xác nhận không khớp";
    isValid = false;
  }

  return isValid;
};

const sendOTP = async () => {
  // Validate phone first
  if (!form.phone.trim()) {
    errors.phone = "Vui lòng nhập số điện thoại";
    return;
  }

  if (!/^[0-9]{10,11}$/.test(form.phone.replace(/\s/g, ""))) {
    errors.phone = "Số điện thoại không hợp lệ";
    return;
  }
  var exists = await authAPI.checkexist(form.phone);
  if (exists) {
    notification.error(
      "Số điện thoại đã được đăng ký. Vui lòng sử dụng số khác."
    );
    return;
  }

  isLoadingOTP.value = true;
  errors.phone = "";

  try {
    const result = await authAPI.sendOTP(form.phone);

    if (result.code == 106 || result.code == 203) {
      otpSent.value = true;
      otpVerified.value = false;
      startCountdown();
      notification.success("Mã OTP đã được gửi đến số điện thoại của bạn", {
        title: "",
        timer: 2000,
      });
    } else {
      notification.error("Gửi OTP thất bại");
    }
  } catch (error) {
    console.error("Send OTP error:", error);
    notification.error(
      error.message ? error.message : "Gửi OTP thất bại. Vui lòng thử lại!"
    );
  } finally {
    isLoadingOTP.value = false;
  }
};

const verifyOTP = async () => {
  if (!form.otp.trim()) {
    errors.otp = "Vui lòng nhập mã OTP";
    return;
  }

  if (!/^[0-9]{6}$/.test(form.otp)) {
    errors.otp = "Mã OTP phải có 6 số";
    return;
  }

  isVerifyingOTP.value = true;
  errors.otp = "";

  try {
    const result = await authAPI.verifyOTP(form.phone, form.otp);

    if (result) {
      otpVerified.value = true;
      notification.success("Xác thực OTP thành công!", {
        title: "",
        timer: 2000,
      });
    } else {
      errors.otp = "Mã OTP không chính xác";
      notification.error("Mã OTP không chính xác");
    }
  } catch (error) {
    console.error("Verify OTP error:", error);
    errors.otp = error.message || "Xác thực OTP thất bại";
    notification.error(
      error.message ? error.message : "Xác thực OTP thất bại. Vui lòng thử lại!"
    );
  } finally {
    isVerifyingOTP.value = false;
  }
};

const resendOTP = async () => {
  var exists = await authAPI.checkexist(form.phone);
  if (exists) {
    notification.error(
      "Số điện thoại đã được đăng ký. Vui lòng sử dụng số khác."
    );
    return;
  }

  otpVerified.value = false;
  form.otp = "";
  await sendOTP();
};

const startCountdown = () => {
  countdown.value = 60;
  countdownInterval = setInterval(() => {
    countdown.value--;
    if (countdown.value <= 0) {
      clearInterval(countdownInterval);
    }
  }, 1000);
};
const sendOTPMail = async () => {
  if (!form.email.trim()) {
    errors.email = "Vui lòng nhập email";
    return;
  }

  // regex check email cơ bản
  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) {
    errors.email = "Email không hợp lệ";
    return;
  }

  isLoadingOTPMail.value = true;
  errors.email = "";

  try {
    const result = await authAPI.sendOTPMail(form.email);

    if (result.code == 106) {
      otpSentMail.value = true;
      otpVerifiedMail.value = false;
      startCountdownMail();
      notification.success("Mã OTP đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư đến hoặc thư rác (Spam).", {
        title: "",
        timer: 4000,
      });
    } else {
      notification.error("Gửi OTP email thất bại");
    }
  } catch (error) {
    console.error("Send OTP mail error:", error);
    notification.error(
      error.message ? error.message : "Gửi OTP mail thất bại. Vui lòng thử lại!"
    );
  } finally {
    isLoadingOTPMail.value = false;
  }
};

// Xác thực OTP mail
const verifyOTPMail = async () => {
  if (!form.otp.trim()) {
    errors.otp = "Vui lòng nhập mã OTP";
    return;
  }

  if (!/^[0-9]{6}$/.test(form.otp)) {
    errors.otp = "Mã OTP phải có 6 số";
    return;
  }

  isVerifyingOTP.value = true;
  errors.otp = "";

  try {
    const result = await authAPI.verifyOTPMail(form.email, form.otp);

    if (result) {
      otpVerifiedMail.value = true;
      notification.success("Xác thực OTP email thành công!", {
        title: "",
        timer: 2000,
      });
    } else {
      errors.otp = "Mã OTP email không chính xác";
      notification.error("Mã OTP email không chính xác");
    }
  } catch (error) {
    console.error("Verify OTP mail error:", error);
    errors.otp = error.message || "Xác thực OTP email thất bại";
    notification.error(
      error.message
        ? error.message
        : "Xác thực OTP email thất bại. Vui lòng thử lại!"
    );
  } finally {
    isVerifyingOTP.value = false;
  }
};

// Resend OTP mail
const resendOTPMail = async () => {
  otpVerifiedMail.value = false;
  form.otp = "";
  await sendOTPMail();
};

// Countdown cho OTP mail
const startCountdownMail = () => {
  countdownMail.value = 60;
  countdownIntervalMail = setInterval(() => {
    countdownMail.value--;
    if (countdownMail.value <= 0) {
      clearInterval(countdownIntervalMail);
    }
  }, 1000);
};
const handleRegister = async () => {
  if (!validateForm()) return;

  // if (!otpVerified.value) {
  //   notification.error("Vui lòng xác thực OTP trước khi đăng ký");
  //   return;
  // }
  if (!otpVerifiedMail.value) {
    notification.error("Vui lòng xác thực OTP email trước khi đăng ký");
    return;
  }

  isLoading.value = true;

  try {
    const result = await authAPI.register({
      Name: form.name,
      PhoneNumber: form.phone,
      Email: form.email,
      Password: form.password,
      ConfirmPassword: form.confirmPassword,
    });

    if (result) {
      notification.success("Đăng ký thành công! Vui lòng đăng nhập.", {
        title: "",
        timer: 2000,
      });

      setTimeout(() => {
        router.push("/login");
      }, 2000);
      return;
    }

    notification.error("Đăng ký thất bại");
  } catch (error) {
    console.error("Register error:", error);
    notification.error(
      error.message ? error.message : "Đăng ký thất bại. Vui lòng thử lại!"
    );
  } finally {
    isLoading.value = false;
  }
};

const togglePassword = () => {
  showPassword.value = !showPassword.value;
};

const toggleConfirmPassword = () => {
  showConfirmPassword.value = !showConfirmPassword.value;
};

const goToLogin = () => {
  router.push("/login");
};

// Cleanup countdown on unmount
onUnmounted(() => {
  if (countdownInterval) {
    clearInterval(countdownInterval);
  }
});

// Emit events
const emit = defineEmits(["register-success", "go-to-login"]);
</script>

<style scoped>
.spa-register-wrapper {
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f9f5 0%, #e8f5e8 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem 1rem;
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
  position: relative;
  overflow: hidden;
}

.spa-register-wrapper::before {
  content: "";
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: radial-gradient(
    circle,
    rgba(107, 168, 107, 0.05) 0%,
    transparent 70%
  );
  animation: float 20s ease-in-out infinite;
}

@keyframes float {
  0%,
  100% {
    transform: translate(0, 0) rotate(0deg);
  }
  33% {
    transform: translate(30px, -30px) rotate(120deg);
  }
  66% {
    transform: translate(-20px, 20px) rotate(240deg);
  }
}

.spa-register-container {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  border-radius: 24px;
  box-shadow: 0 20px 40px rgba(45, 90, 45, 0.1),
    0 0 0 1px rgba(255, 255, 255, 0.2);
  padding: 2.5rem;
  width: 100%;
  max-width: 500px;
  position: relative;
  z-index: 2;
  overflow: hidden;
}

.spa-register-container::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, #6ba86b, #4a8c4a, #6ba86b);
  background-size: 200% 100%;
  animation: gradient 3s ease infinite;
}

@keyframes gradient {
  0%,
  100% {
    background-position: 0% 50%;
  }
  50% {
    background-position: 100% 50%;
  }
}

/* Header */
.register-header {
  text-align: center;
  margin-bottom: 2rem;
}

.welcome-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
  animation: bounce 2s ease-in-out infinite;
}

@keyframes bounce {
  0%,
  20%,
  50%,
  80%,
  100% {
    transform: translateY(0);
  }
  40% {
    transform: translateY(-10px);
  }
  60% {
    transform: translateY(-5px);
  }
}

.register-title {
  color: #2d5a2d;
  font-size: 2rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
  letter-spacing: -0.02em;
}

.register-subtitle {
  color: #5a7a5a;
  font-size: 1rem;
  line-height: 1.5;
  margin-bottom: 0;
}

/* Form */
.register-form {
  position: relative;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-label {
  display: block;
  color: #2d5a2d;
  font-weight: 600;
  margin-bottom: 0.5rem;
  font-size: 0.95rem;
}

.input-container {
  position: relative;
  display: flex;
  align-items: center;
}

.input-icon {
  position: absolute;
  left: 1rem;
  font-size: 1.2rem;
  z-index: 1;
  opacity: 0.7;
}

.form-input {
  width: 100%;
  padding: 1rem 1rem 1rem 3rem;
  border: 2px solid #e8f5e8;
  border-radius: 16px;
  font-size: 1rem;
  background: rgba(255, 255, 255, 0.8);
  transition: all 0.3s ease;
  color: #2d5a2d;
}

.form-input::placeholder {
  color: #9db29d;
}

.form-input:focus {
  outline: none;
  border-color: #6ba86b;
  background: rgba(255, 255, 255, 1);
  box-shadow: 0 0 0 4px rgba(107, 168, 107, 0.1);
  transform: translateY(-1px);
}

.form-input.error {
  border-color: #e74c3c;
  box-shadow: 0 0 0 4px rgba(231, 76, 60, 0.1);
}

.form-input.verified {
  border-color: #27ae60;
  box-shadow: 0 0 0 4px rgba(39, 174, 96, 0.1);
}

.password-toggle {
  position: absolute;
  right: 1rem;
  font-size: 1.2rem;
  cursor: pointer;
  z-index: 1;
  padding: 0.25rem;
  border-radius: 50%;
  transition: all 0.3s ease;
}

.password-toggle:hover {
  background: rgba(107, 168, 107, 0.1);
  transform: scale(1.1);
}

/* OTP Buttons */
.otp-btn {
  width: 100%;
  padding: 1rem 1.5rem;
  background: linear-gradient(135deg, #3498db 0%, #2980b9 100%);
  color: white;
  border: none;
  border-radius: 16px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-bottom: 1rem;
  position: relative;
  overflow: hidden;
}

.otp-btn::before {
  content: "";
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    90deg,
    transparent,
    rgba(255, 255, 255, 0.2),
    transparent
  );
  transition: left 0.5s ease;
}

.otp-btn:hover::before {
  left: 100%;
}

.otp-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(52, 152, 219, 0.3);
}

.otp-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
  transform: none;
}

.otp-input-section {
  margin-top: 1rem;
}

.otp-container {
  position: relative;
}

.otp-actions {
  position: absolute;
  right: 1rem;
  top: 50%;
  transform: translateY(-50%);
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.resend-btn {
  background: #95a5a6;
  color: white;
  border: none;
  border-radius: 8px;
  padding: 0.4rem 0.8rem;
  font-size: 0.8rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.resend-btn:hover:not(:disabled) {
  background: #7f8c8d;
}

.resend-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.verify-btn {
  background: #27ae60;
  color: white;
  border: none;
  border-radius: 8px;
  padding: 0.4rem 0.6rem;
  font-size: 0.9rem;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: 32px;
  height: 32px;
}

.verify-btn:hover:not(:disabled) {
  background: #229954;
  transform: scale(1.05);
}

.verify-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.verified-badge {
  position: absolute;
  right: 1rem;
  top: 50%;
  transform: translateY(-50%);
  background: #27ae60;
  color: white;
  border-radius: 12px;
  padding: 0.4rem 0.8rem;
  font-size: 0.8rem;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 0.3rem;
}

.loading-spinner-small {
  width: 14px;
  height: 14px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top: 2px solid white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

/* Register Button */
.register-btn {
  width: 100%;
  padding: 1rem 1.5rem;
  background: linear-gradient(135deg, #f39c12 0%, #e67e22 100%);
  color: white;
  border: none;
  border-radius: 16px;
  font-size: 1.1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-bottom: 1.5rem;
  position: relative;
  overflow: hidden;
}

.register-btn::before {
  content: "";
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    90deg,
    transparent,
    rgba(255, 255, 255, 0.2),
    transparent
  );
  transition: left 0.5s ease;
}

.register-btn:hover::before {
  left: 100%;
}

.register-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(243, 156, 18, 0.3);
}

.register-btn:active {
  transform: translateY(0);
}

.register-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
  transform: none;
}

.btn-content {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.loading-spinner {
  width: 20px;
  height: 20px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top: 2px solid white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

/* Divider */
.divider {
  position: relative;
  text-align: center;
  margin: 1.5rem 0;
}

.divider::before {
  content: "";
  position: absolute;
  top: 50%;
  left: 0;
  right: 0;
  height: 1px;
  background: linear-gradient(to right, transparent, #e8f5e8, transparent);
}

.divider span {
  background: rgba(255, 255, 255, 0.95);
  color: #5a7a5a;
  padding: 0 1rem;
  font-size: 0.9rem;
  position: relative;
}

/* Login Section */
.login-section {
  text-align: center;
}

.login-text {
  color: #5a7a5a;
  font-size: 0.95rem;
  margin-bottom: 1rem;
}

.login-btn-link {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  color: #6ba86b;
  text-decoration: none;
  font-weight: 600;
  font-size: 1rem;
  padding: 0.75rem 1.5rem;
  border: 2px solid #6ba86b;
  border-radius: 16px;
  transition: all 0.3s ease;
  background: rgba(107, 168, 107, 0.05);
}

.login-btn-link:hover {
  background: #6ba86b;
  color: white;
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(107, 168, 107, 0.3);
}

/* Error Message */
.error-message {
  color: #e74c3c;
  font-size: 0.85rem;
  margin-top: 0.5rem;
  display: flex;
  align-items: center;
  gap: 0.3rem;
  padding: 0.5rem 0.75rem;
  background: rgba(231, 76, 60, 0.05);
  border-radius: 8px;
  border-left: 3px solid #e74c3c;
}

/* Decorative Elements */
.decorative-elements {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  pointer-events: none;
  z-index: -1;
}

.leaf {
  position: absolute;
  font-size: 1.5rem;
  opacity: 0.3;
  animation: float-leaf 6s ease-in-out infinite;
}

.leaf-1 {
  top: 15%;
  left: 8%;
  animation-delay: 0s;
}

.leaf-2 {
  top: 45%;
  right: 12%;
  animation-delay: 1.5s;
}

.leaf-3 {
  bottom: 35%;
  left: 15%;
  animation-delay: 3s;
}

.leaf-4 {
  top: 70%;
  right: 25%;
  animation-delay: 4.5s;
}

@keyframes float-leaf {
  0%,
  100% {
    transform: translate(0, 0) rotate(0deg);
  }
  25% {
    transform: translate(10px, -10px) rotate(90deg);
  }
  50% {
    transform: translate(-5px, 5px) rotate(180deg);
  }
  75% {
    transform: translate(5px, -5px) rotate(270deg);
  }
}

/* Transitions */
.fade-enter-active,
.fade-leave-active {
  transition: all 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

/* Responsive */
@media (max-width: 768px) {
  .spa-register-wrapper {
    padding: 1rem;
  }

  .spa-register-container {
    padding: 2rem 1.5rem;
    max-width: 100%;
  }

  .register-title {
    font-size: 1.7rem;
  }

  .form-input {
    font-size: 16px; /* Prevent zoom on iOS */
  }

  .otp-actions {
    flex-direction: column;
    gap: 0.3rem;
  }

  .resend-btn,
  .verify-btn {
    font-size: 0.75rem;
    padding: 0.3rem 0.6rem;
  }
}

@media (max-width: 480px) {
  .spa-register-container {
    padding: 1.5rem 1rem;
  }

  .register-title {
    font-size: 1.5rem;
  }

  .welcome-icon {
    font-size: 2.5rem;
  }

  .otp-actions {
    position: static;
    transform: none;
    margin-top: 0.5rem;
    justify-content: center;
  }

  .form-input {
    padding-right: 1rem;
  }

  .verified-badge {
    position: static;
    transform: none;
    margin-top: 0.5rem;
    display: inline-flex;
    justify-content: center;
  }
}
</style>
