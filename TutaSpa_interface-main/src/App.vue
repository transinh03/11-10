<template>
  <div id="app">
    <!-- Header -->
    <header class="header">
      <nav class="nav">
        <div class="logo">
          <router-link to="/" class="navbar-brand d-flex align-items-center">
            <img
              src="../src/assets/img/logotutaspa.png"
              alt="TutaSpa Logo"
              class="me-2"
              style="height: 50px; width: auto"
            />
            <span>TutaSpa 🌿</span>
          </router-link>
        </div>

        <!-- Mobile Menu Toggle -->
        <button
          class="mobile-menu-toggle"
          @click="toggleMenu"
          :class="{ active: isMenuVisible }"
        >
          <span></span>
          <span></span>
          <span></span>
        </button>

        <!-- Navigation Links -->
        <ul class="nav-links" :class="{ 'mobile-active': isMenuVisible }">
          <li>
            <router-link to="/" @click="closeMobileMenu">Trang chủ</router-link>
          </li>
          <li>
            <router-link to="/#services" @click="closeMobileMenu"
              >Dịch vụ</router-link
            >
          </li>
          <li>
            <router-link to="/DatLich" @click="closeMobileMenu"
              >Đặt lịch</router-link
            >
          </li>
          <li>
            <router-link to="/#about" @click="closeMobileMenu"
              >Về chúng tôi</router-link
            >
          </li>
          <li>
            <router-link to="/#contact" @click="closeMobileMenu"
              >Liên hệ</router-link
            >
          </li>

          <!-- Mobile User Actions - Inside Menu -->
          <li class="mobile-user-actions">
            <router-link
              style="background-color: orange"
              v-if="!state.isAuthenticated"
              to="/login"
              class="mobile-auth-btn login-btn"
              @click="closeMobileMenu"
            >
              <i class="fa-solid fa-user"></i>
              <span>Đăng nhập</span>
            </router-link>
            <router-link
              v-if="state.isAuthenticated"
              to="/LichSuDatLich"
              class="mobile-auth-btn schedule-btn"
              @click="closeMobileMenu"
            >
              <i class="fa-regular fa-calendar-check"></i>
              <span>Xem lịch</span>
            </router-link>
            <button
              v-if="state.isAuthenticated"
              class="mobile-auth-btn logout-btn"
              @click="handleLogout"
            >
              <i class="fa-solid fa-sign-out-alt"></i>
              <span>Đăng xuất</span>
            </button>
          </li>
        </ul>

        <!-- Desktop User Actions -->
        <div class="user-actions desktop-only">
          <router-link
            v-if="!state.isAuthenticated"
            to="/login"
            class="book-btn"
          >
            <i class="fa-solid fa-user"></i>
            <span class="btn-text">Đăng nhập</span>
          </router-link>
          <router-link
            v-if="state.isAuthenticated"
            to="/LichSuDatLich"
            class="book-btn"
          >
            <i class="fa-regular fa-calendar-check"></i>
            <span class="btn-text">Xem lịch</span>
          </router-link>
          <button
            v-if="state.isAuthenticated"
            class="book-btn"
            @click="state.logout"
          >
            <i class="fa-solid fa-sign-out-alt"></i>
            <span class="btn-text">Đăng xuất</span>
          </button>
        </div>
      </nav>
    </header>

    <!-- Content -->
    <main class="main-content"><router-view></router-view></main>

    <!-- Floating Button -->
    <div class="chat-popup-wrapper">
      <!-- Chat Toggle Button -->
      <button
        v-if="!chatOpen"
        @click="toggleChat"
        class="chat-toggle-btn"
        title="Hỗ trợ trực tuyến"
      >
        <i class="fas fa-comments"></i>
        <span v-if="hasUnreadMessages" class="notification-badge">{{
          unreadCount
        }}</span>
      </button>

      <!-- Chat Window -->
      <div v-if="chatOpen" class="chat-window">
        <div class="chat-header">
          <h3>Hỗ trợ khách hàng</h3>
          <p v-if="assignedAdmin.name">
            Bạn đang trò chuyện với <strong>{{ assignedAdmin.name }}</strong>
          </p>
          <p v-else>Chúng tôi luôn sẵn sàng hỗ trợ bạn</p>
          <div
            :class="['status-indicator', isConnected ? 'connected' : '']"
          ></div>
          <button @click="toggleChat" class="close-btn">
            <i class="fas fa-times"></i>
          </button>
        </div>

        <!-- Welcome Screen -->
        <div v-if="!chatStarted" class="welcome-screen">
          <h4>Chào mừng bạn!</h4>
          <p>Vui lòng nhập tên của bạn để bắt đầu trò chuyện</p>
          <input
            v-model="guestName"
            type="text"
            placeholder="Tên của bạn (tùy chọn)"
            maxlength="50"
            @keypress="handleWelcomeKeyPress"
          />
          <button @click="startChat" class="start-chat-btn">
            Bắt đầu chat
          </button>
        </div>

        <!-- Chat Messages -->
        <div v-if="chatStarted" class="chat-messages" ref="chatMessages">
          <div
            v-for="message in messages"
            :key="message.id"
            :class="[
              'message',
              message.isFromAdmin ? 'support' : '',
              message.isInlineNotice ? 'inline-notice' : '',
              message.isFromAdmin === false &&
              !message.isSystem &&
              !message.isInlineNotice
                ? 'user'
                : '',
            ]"
          >
            <div v-if="message.isInlineNotice" class="inline-system-notice">
              {{ message.message }}
            </div>
            <div v-else class="message-bubble">
              {{ message.message }}
              <div class="message-time">
                {{ formatTime(message.timestamp) }}
              </div>
            </div>
          </div>
        </div>

        <!-- Chat Input -->
        <div v-if="chatStarted && !adminDisconnected" class="chat-input">
          <div class="input-group">
            <input
              v-model="currentMessage"
              type="text"
              class="message-input"
              placeholder="Nhập tin nhắn..."
              maxlength="500"
              @keypress="handleMessageKeyPress"
            />
            <button
              @click="sendMessage"
              class="send-btn"
              :disabled="!currentMessage.trim()"
            >
              <i class="fas fa-paper-plane"></i>
            </button>
          </div>
        </div>

        <!-- New Session Section -->
        <div
          v-if="chatStarted && adminDisconnected"
          class="new-session-section"
        >
          <div class="disconnect-notice">
            <button @click="startNewSession" class="start-new-session-btn">
              Bắt đầu phiên hỗ trợ mới
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Footer -->
    <footer id="contact" class="footer">
      <div class="footer-content">
        <div class="footer-section">
          <h3>Liên hệ</h3>
          <p>📍 31 Nguyễn Mộng Tuân, Phường Liên Chiểu, TP Đà Nẵng</p>
          <p>📞 Hotline: 0123 456 789</p>
          <p>✉️ Email: info@serenityspa.vn</p>
          <p>🌐 Website: tutaspa.vercel.app</p>
        </div>
        <div class="footer-section">
          <h3>Giờ mở cửa</h3>
          <p>Thứ 2 - Thứ 6: 8:00 - 21:00</p>
          <p>Thứ 7 - Chủ nhật: 8:00 - 22:00</p>
          <p>Lễ Tết: 9:00 - 20:00</p>
          <p>🎯 Đặt lịch trước 24h để được phục vụ tốt nhất</p>
        </div>
        <div class="footer-section">
          <h3>Dịch vụ nổi bật</h3>
          <p><router-link to="/#services">Massage trị liệu</router-link></p>
          <p>
            <router-link to="/#services">Chăm sóc da chuyên sâu</router-link>
          </p>
          <p><router-link to="/#services">Detox toàn thân</router-link></p>
          <p><router-link to="/#services">Liệu pháp thư giãn</router-link></p>
        </div>
        <div class="footer-section">
          <h3>Theo dõi chúng tôi</h3>
          <p><a href="#">📘 Facebook: Tuta Spa</a></p>
          <p><a href="#">📷 Instagram: @serenityspa_vn</a></p>
          <p><a href="#">💬 Zalo: 0903209925</a></p>
          <p><a href="#">📺 YouTube: Serenity Spa Vietnam</a></p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { onMounted, onUnmounted, ref, nextTick } from "vue";
import * as signalR from "@microsoft/signalr";
import { useAuthState } from "./services/authstate";
import { authAPI } from "./services/authservice";

const chatClosed = ref(false);
const adminDisconnected = ref(false);
const base_url = import.meta.env.VITE_CHAT_URL;
const chatOpen = ref(false);
const chatStarted = ref(false);
const isConnected = ref(false);
const guestName = ref("");
const currentMessage = ref("");
const messages = ref([]);
const hasUnreadMessages = ref(false);
const unreadCount = ref(0);
const sessionId = ref(null);
const userName = ref(null);
const connection = ref(null);
const chatMessages = ref(null);
const assignedAdmin = ref({ id: null, name: null });

const isNavbarHidden = ref(false);
let lastScroll = 0;
const state = useAuthState();
const isMenuVisible = ref(false);

onMounted(() => {
  window.addEventListener("scroll", () => {
    const currentScroll =
      window.pageYOffset || document.documentElement.scrollTop;
    if (currentScroll > lastScroll && currentScroll > 100) {
      isNavbarHidden.value = true;
    } else {
      isNavbarHidden.value = false;
    }
    lastScroll = currentScroll;
  });

  // Close mobile menu when clicking outside
  document.addEventListener("click", (e) => {
    if (!e.target.closest(".nav") && isMenuVisible.value) {
      isMenuVisible.value = false;
    }
  });

  initializeChat();
});

onUnmounted(async () => {
  try {
    if (
      connection.value &&
      connection.value.state === signalR.HubConnectionState.Connected
    ) {
      await connection.value.stop();
    }
  } catch (error) {
    console.error("Lỗi khi đóng kết nối:", error);
  }
});

function toggleMenu() {
  isMenuVisible.value = !isMenuVisible.value;
}

function closeMobileMenu() {
  isMenuVisible.value = false;
}

function handleLogout() {
  state.logout();
  closeMobileMenu();
}

// Chat functions
function initializeChat() {
  try {
    if (
      !connection.value ||
      connection.value.state === signalR.HubConnectionState.Disconnected
    ) {
      connection.value = new signalR.HubConnectionBuilder()
        .withUrl(`${base_url}`)
        .withAutomaticReconnect()
        .build();

      setupChatEventHandlers();
    }
  } catch (error) {
    console.error("Lỗi khởi tạo kết nối:", error);
    isConnected.value = false;
    addSystemMessage(
      "Không thể khởi tạo kết nối với server. Vui lòng thử lại.",
      "error"
    );
  }
}

function setupChatEventHandlers() {
  connection.value.onreconnected(() => {
    isConnected.value = true;
    addSystemMessage("Đã kết nối lại với server");
  });

  connection.value.onclose(() => {
    isConnected.value = false;
    addSystemMessage("Mất kết nối với server");
  });

  connection.value.on("AdminAssigned", (data) => {
    console.log("Admin assigned:", data);
    assignedAdmin.value = { id: data.adminId, name: data.adminName };
    adminDisconnected.value = false;
    addInlineSystemNotice(data.message);
  });

  connection.value.on("ChatSessionCreated", (data) => {
    console.log("Chat session created:", data);
    sessionId.value = data.sessionId;
    userName.value = data.userName;
    chatStarted.value = true;
    adminDisconnected.value = false;
    chatClosed.value = false;
    addSystemMessage1(data.message);
  });

  connection.value.on("ReceiveMessage", (message) => {
    addMessage(message);
    if (!chatOpen.value) {
      hasUnreadMessages.value = true;
      unreadCount.value++;
    }
  });

  connection.value.on("ChatClosed", handleSessionEnded);
  connection.value.on("AdminDisconnected", handleSessionEnded);

  function handleSessionEnded(data) {
    chatStarted.value = true;
    adminDisconnected.value = true;
    chatClosed.value = true;
    addInlineSystemNotice(
      (data?.Message || data?.message) ??
        "Phiên chat đã kết thúc. Vui lòng bắt đầu phiên hỗ trợ mới."
    );
  }

  connection.value.on("MessageSent", (message) => {
    console.log("Message sent successfully:", message.messageId);
  });

  connection.value.on("Error", (error) => {
    addSystemMessage(`Lỗi: ${error}`, "error");
  });

  connection.value.on("AdminDisconnected", (data) => {
    adminDisconnected.value = true;
  });
}

async function startConnection() {
  try {
    if (!connection.value) {
      initializeChat();
    }

    if (connection.value.state === signalR.HubConnectionState.Disconnected) {
      await connection.value.start();
      isConnected.value = true;
      console.log("SignalR connection started successfully");
      return true;
    } else if (
      connection.value.state === signalR.HubConnectionState.Connected
    ) {
      isConnected.value = true;
      return true;
    } else {
      console.log("Connection is in state:", connection.value.state);
      await new Promise((resolve) => setTimeout(resolve, 1000));
      return await startConnection();
    }
  } catch (error) {
    console.error("Lỗi kết nối:", error);
    isConnected.value = false;
    addSystemMessage(
      "Không thể kết nối với server. Vui lòng thử lại.",
      "error"
    );
    return false;
  }
}

function toggleChat() {
  chatOpen.value = !chatOpen.value;
  if (chatOpen.value) {
    hasUnreadMessages.value = false;
    unreadCount.value = 0;
  }
}

async function startChat() {
  try {
    if (
      connection.value &&
      connection.value.state === signalR.HubConnectionState.Connected
    ) {
      await connection.value.stop();
      chatStarted.value = false;
      chatClosed.value = false;
      adminDisconnected.value = false;
      sessionId.value = null;
      userName.value = null;
      messages.value = [];
      assignedAdmin.value = { id: null, name: null };
    }
    const connected = await startConnection();
    if (
      connected &&
      connection.value.state === signalR.HubConnectionState.Connected
    ) {
      await connection.value.invoke("JoinAsGuest", guestName.value || "");
    } else {
      addSystemMessage(
        "Không thể kết nối với server. Vui lòng thử lại.",
        "error"
      );
    }
  } catch (error) {
    console.error("Lỗi tham gia chat:", error);
    addSystemMessage("Không thể tham gia chat. Vui lòng thử lại.", "error");
  }
}

async function startNewSession() {
  chatStarted.value = false;
  adminDisconnected.value = false;
  chatClosed.value = false;
  sessionId.value = null;
  userName.value = null;
  messages.value = [];
  assignedAdmin.value = { id: null, name: null };

  if (
    connection.value &&
    connection.value.state === signalR.HubConnectionState.Connected
  ) {
    await connection.value.stop();
  }
}

async function sendMessage() {
  if (!currentMessage.value.trim()) return;

  try {
    const message = {
      fromUserId: sessionId.value,
      fromUserName: userName.value,
      message: currentMessage.value,
      timestamp: new Date().toISOString(),
      isFromAdmin: false,
      id: Date.now(),
    };

    addMessage(message);
    await connection.value.invoke("SendMessageToSupport", currentMessage.value);
    currentMessage.value = "";
  } catch (error) {
    console.error("Lỗi gửi tin nhắn:", error);
    addSystemMessage("Không thể gửi tin nhắn. Vui lòng thử lại.", "error");
  }
}

function addMessage(message) {
  messages.value.push({
    ...message,
    id: message.id || Date.now() + Math.random(),
  });
  scrollToBottom();
}

function addSystemMessage1(message, type = "info") {
  messages.value.push({
    id: Date.now() + Math.random(),
    message: message,
    isFromAdmin: true,
    isSystem: false,
    type: type,
    timestamp: new Date().toISOString(),
  });
  scrollToBottom();
}

function addSystemMessage(message, type = "info") {
  messages.value.push({
    id: Date.now() + Math.random(),
    message: message,
    isFromAdmin: false,
    isSystem: true,
    type: type,
    timestamp: new Date().toISOString(),
  });
  scrollToBottom();
}

function addInlineSystemNotice(message) {
  messages.value.push({
    id: Date.now() + Math.random(),
    message: message,
    isInlineNotice: true,
    timestamp: new Date().toISOString(),
  });
}

function formatTime(timestamp) {
  return new Date(timestamp).toLocaleTimeString("vi-VN", {
    hour: "2-digit",
    minute: "2-digit",
  });
}

function scrollToBottom() {
  nextTick(() => {
    if (chatMessages.value) {
      chatMessages.value.scrollTop = chatMessages.value.scrollHeight;
    }
  });
}

function handleWelcomeKeyPress(e) {
  if (e.key === "Enter") {
    startChat();
  }
}

function handleMessageKeyPress(e) {
  if (e.key === "Enter") {
    sendMessage();
  }
}

function checkConnectionStatus() {
  if (connection.value) {
    console.log("Connection state:", connection.value.state);
    return connection.value.state === signalR.HubConnectionState.Connected;
  }
  return false;
}

setInterval(() => {
  if (connection.value && chatStarted.value) {
    const wasConnected = isConnected.value;
    const nowConnected =
      connection.value.state === signalR.HubConnectionState.Connected;

    if (wasConnected !== nowConnected) {
      isConnected.value = nowConnected;
      if (!nowConnected) {
        addSystemMessage("Mất kết nối với server", "warning");
      }
    }
  }
}, 5000);
</script>

<style scoped>
/* Reset & Base */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: "Georgia", "Times New Roman", serif;
  line-height: 1.7;
  color: #2d4a2d;
  overflow-x: hidden;
  background-color: #f8fcf8;
}

/* Header Styles - Enhanced WOW Factor */
.header {
  background: linear-gradient(
    135deg,
    rgba(120, 186, 126, 0.95) 0%,
    rgba(107, 163, 113, 0.95) 30%,
    rgba(94, 140, 100, 0.95) 70%,
    rgba(78, 120, 85, 0.95) 100%
  );
  color: white;
  padding: 0.5rem 0;
  position: fixed;
  width: 100%;
  top: 0;
  z-index: 1000;
  backdrop-filter: blur(20px) saturate(180%);
  box-shadow: 0 8px 32px rgba(120, 186, 126, 0.4), 0 2px 8px rgba(0, 0, 0, 0.1),
    inset 0 1px 0 rgba(255, 255, 255, 0.2);
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.header::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(
    90deg,
    transparent 0%,
    rgba(255, 255, 255, 0.05) 50%,
    transparent 100%
  );
  animation: shimmer 3s ease-in-out infinite;
}

@keyframes shimmer {
  0%,
  100% {
    opacity: 0;
  }
  50% {
    opacity: 1;
  }
}

.header.hidden {
  transform: translateY(-100%);
  box-shadow: none;
}

.nav {
  display: flex;
  justify-content: space-between;
  align-items: center;
  max-width: 1400px;
  margin: 0 auto;
  padding: 1rem 2rem;
  position: relative;
  height: 70px;
}

.logo {
  font-size: 1.8rem;
  font-weight: 700;
  color: white;
  display: flex;
  align-items: center;
  gap: 0.7rem;
  z-index: 1001;
  transition: all 0.3s ease;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.logo:hover {
  transform: scale(1.05);
  filter: drop-shadow(0 4px 8px rgba(255, 255, 255, 0.3));
}

.logo a {
  color: inherit;
  text-decoration: none;
  display: flex;
  align-items: center;
  gap: 0.7rem;
}

.logo img {
  filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.3));
  transition: all 0.3s ease;
}

.logo:hover img {
  filter: drop-shadow(0 4px 8px rgba(255, 255, 255, 0.2));
  transform: rotate(5deg);
}

/* Mobile Menu Toggle - Enhanced */
.mobile-menu-toggle {
  display: none;
  flex-direction: column;
  background: rgba(255, 255, 255, 0.1);
  border: 2px solid rgba(255, 255, 255, 0.2);
  cursor: pointer;
  padding: 0.6rem;
  z-index: 1001;
  border-radius: 8px;
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
}

.mobile-menu-toggle:hover {
  background: rgba(255, 255, 255, 0.15);
  border-color: rgba(255, 255, 255, 0.3);
  transform: scale(1.05);
}

.mobile-menu-toggle span {
  width: 25px;
  height: 3px;
  background: white;
  margin: 2px 0;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  border-radius: 2px;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
}

.mobile-menu-toggle.active span:nth-child(1) {
  transform: rotate(-45deg) translate(-5px, 6px);
}

.mobile-menu-toggle.active span:nth-child(2) {
  opacity: 0;
}

.mobile-menu-toggle.active span:nth-child(3) {
  transform: rotate(45deg) translate(-5px, -6px);
}

/* Navigation Links - Enhanced */
.nav-links {
  display: flex;
  list-style: none;
  gap: 0.5rem;
  margin: 0;
  padding: 0;
}

.nav-links a {
  color: white;
  text-decoration: none;
  font-weight: 600;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  padding: 0.7rem 1.2rem;
  border-radius: 50px;
  position: relative;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
}

.nav-links a::before {
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

.nav-links a::after {
  content: "";
  position: absolute;
  bottom: 0;
  left: 50%;
  width: 0%;
  height: 2px;
  background: linear-gradient(90deg, #ffd700, #ffed4e);
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  transform: translateX(-50%);
  border-radius: 2px;
  box-shadow: 0 0 10px rgba(255, 215, 0, 0.6);
}

.nav-links a:hover {
  color: #ffd700;
  background: rgba(255, 255, 255, 0.15);
  border-color: rgba(255, 215, 0, 0.3);
  transform: translateY(-2px) scale(1.05);
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2), 0 0 20px rgba(255, 215, 0, 0.3);
  text-shadow: 0 0 10px rgba(255, 215, 0, 0.5);
}

.nav-links a:hover::before {
  left: 100%;
}

.nav-links a:hover::after {
  width: 80%;
}

.nav-links a:active {
  transform: translateY(-1px) scale(1.02);
}

/* --- Disable header menu button effects, but keep .book-btn effects --- */
.nav-links a,
.nav-links a:hover,
.nav-links a:active {
  transition: none !important;
  box-shadow: none !important;
  transform: none !important;
  text-shadow: none !important;
  background: rgba(255, 255, 255, 0.05) !important;
  color: white !important;
  border-color: rgba(255, 255, 255, 0.1) !important;
}

.nav-links a::before,
.nav-links a::after {
  display: none !important;
}

/* Mobile User Actions in Menu */
.mobile-user-actions {
  display: none;
}

.mobile-auth-btn {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  padding: 1rem 1.5rem;
  margin: 0.5rem 0;
  border-radius: 50px;
  text-decoration: none;
  font-weight: 600;
  font-size: 1rem;
  transition: all 0.3s ease;
  border: 2px solid transparent;
  cursor: pointer;
  width: 100%;
  justify-content: center;
  min-width: 200px;
}

.mobile-auth-btn i {
  font-size: 1.2em;
}

.login-btn {
  background: linear-gradient(135deg, #f59e0b 0%, #f97316 50%, #ea580c 100%);
  color: white;
  border-color: rgba(255, 255, 255, 0.2);
}

.schedule-btn {
  background: linear-gradient(135deg, #3b82f6 0%, #2563eb 50%, #1d4ed8 100%);
  color: white;
  border-color: rgba(255, 255, 255, 0.2);
}

.logout-btn {
  background: linear-gradient(135deg, #ef4444 0%, #dc2626 50%, #b91c1c 100%);
  color: white;
  border-color: rgba(255, 255, 255, 0.2);
}

.mobile-auth-btn:hover {
  transform: translateY(-2px) scale(1.02);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.3);
}

/* Desktop User Actions */
.user-actions {
  display: flex;
  align-items: center;
  gap: 0.8rem;
}

.desktop-only {
  display: flex;
}

.book-btn {
  background: linear-gradient(135deg, #f59e0b 0%, #f97316 50%, #ea580c 100%);
  padding: 0.8rem 1.5rem;
  border-radius: 50px;
  text-decoration: none;
  color: white;
  font-weight: 700;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 8px 20px rgba(245, 158, 11, 0.4), 0 2px 4px rgba(0, 0, 0, 0.2),
    inset 0 1px 0 rgba(255, 255, 255, 0.3);
  border: 2px solid rgba(255, 255, 255, 0.2);
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.6rem;
  font-size: 0.9rem;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
  position: relative;
  overflow: hidden;
}

.book-btn::before {
  content: "";
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    90deg,
    transparent,
    rgba(255, 255, 255, 0.3),
    transparent
  );
  transition: left 0.6s ease;
}

.book-btn:hover {
  transform: translateY(-3px) scale(1.05);
  box-shadow: 0 15px 35px rgba(245, 158, 11, 0.5), 0 5px 15px rgba(0, 0, 0, 0.3),
    0 0 20px rgba(255, 215, 0, 0.4);
  border-color: rgba(255, 215, 0, 0.4);
  color: white;
  text-decoration: none;
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 50%, #d97706 100%);
}

.book-btn:hover::before {
  left: 100%;
}

.book-btn:active {
  transform: translateY(-1px) scale(1.02);
  box-shadow: 0 8px 20px rgba(245, 158, 11, 0.4), 0 2px 8px rgba(0, 0, 0, 0.2);
}

.book-btn i {
  font-size: 1.1em;
  filter: drop-shadow(0 1px 2px rgba(0, 0, 0, 0.3));
}

/* Main Content */
.main-content {
  margin-top: 80px;
  padding: 0px 0;
}

/* Chat Popup Styles */
.chat-popup-wrapper {
  position: fixed;
  bottom: 20px;
  right: 20px;
  z-index: 1000;
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
}

.chat-toggle-btn {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background: linear-gradient(135deg, #4caf50, #45a049);
  color: white;
  border: none;
  font-size: 24px;
  cursor: pointer;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
  transition: all 0.3s ease;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
}

.chat-toggle-btn:hover {
  transform: scale(1.1);
  box-shadow: 0 6px 25px rgba(0, 0, 0, 0.4);
}

.notification-badge {
  position: absolute;
  top: -5px;
  right: -5px;
  background: #ff4444;
  color: white;
  border-radius: 50%;
  width: 20px;
  height: 20px;
  font-size: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
}

.chat-window {
  width: 350px;
  height: 500px;
  background: white;
  border-radius: 20px;
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.2);
  overflow: hidden;
  display: flex;
  flex-direction: column;
  animation: slideUp 0.3s ease;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.chat-header {
  background: linear-gradient(135deg, #4caf50, #45a049);
  color: white;
  padding: 20px;
  text-align: center;
  position: relative;
}

.chat-header h3 {
  margin: 0 0 5px 0;
  font-size: 1.2em;
}

.chat-header p {
  margin: 0;
  font-size: 0.8em;
  opacity: 0.9;
}

.status-indicator {
  position: absolute;
  top: 15px;
  right: 40px;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  background: #ff4444;
  animation: pulse 2s infinite;
}

.status-indicator.connected {
  background: #4caf50;
}

@keyframes pulse {
  0% {
    transform: scale(1);
    opacity: 1;
  }
  50% {
    transform: scale(1.1);
    opacity: 0.7;
  }
  100% {
    transform: scale(1);
    opacity: 1;
  }
}

.close-btn {
  position: absolute;
  top: 15px;
  right: 15px;
  background: none;
  border: none;
  color: white;
  font-size: 16px;
  cursor: pointer;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.welcome-screen {
  padding: 30px 20px;
  text-align: center;
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.welcome-screen h4 {
  color: #333;
  margin-bottom: 15px;
}

.welcome-screen p {
  color: #666;
  margin-bottom: 20px;
  font-size: 0.9em;
}

.welcome-screen input {
  width: 100%;
  padding: 10px;
  border: 2px solid #ddd;
  border-radius: 20px;
  font-size: 14px;
  margin-bottom: 15px;
  outline: none;
  transition: border-color 0.3s;
}

.welcome-screen input:focus {
  border-color: #4caf50;
}

.start-chat-btn {
  background: linear-gradient(135deg, #4caf50, #45a049);
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 20px;
  font-size: 14px;
  cursor: pointer;
  transition: transform 0.2s;
}

.start-chat-btn:hover {
  transform: translateY(-2px);
}

.chat-messages {
  flex: 1;
  overflow-y: auto;
  padding: 15px;
  background: #f8f9fa;
}

.message {
  margin-bottom: 15px;
  display: flex;
  align-items: flex-end;
}

.message.user {
  justify-content: flex-end;
}

.message.support {
  justify-content: flex-start;
}

.message-bubble {
  max-width: 70%;
  padding: 10px 14px;
  border-radius: 18px;
  font-size: 13px;
  line-height: 1.4;
  position: relative;
  animation: slideIn 0.3s ease;
}

.message.user .message-bubble {
  background: linear-gradient(135deg, #667eea, #764ba2);
  color: white;
  border-bottom-right-radius: 4px;
}

.message.support .message-bubble {
  background: white;
  color: #333;
  border: 1px solid #e0e0e0;
  border-bottom-left-radius: 4px;
}

.message-time {
  font-size: 10px;
  opacity: 0.7;
  margin-top: 5px;
}

.message.user .message-time {
  text-align: right;
  color: rgba(255, 255, 255, 0.7);
}

.message.support .message-time {
  color: #666;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.chat-input {
  padding: 15px;
  background: white;
  border-top: 1px solid #e0e0e0;
}

.input-group {
  display: flex;
  gap: 8px;
  align-items: center;
}

.message-input {
  flex: 1;
  padding: 10px 14px;
  border: 2px solid #ddd;
  border-radius: 20px;
  font-size: 13px;
  outline: none;
  transition: border-color 0.3s;
}

.message-input:focus {
  border-color: #4caf50;
}

.send-btn {
  background: linear-gradient(135deg, #4caf50, #45a049);
  color: white;
  border: none;
  padding: 10px;
  border-radius: 50%;
  cursor: pointer;
  transition: transform 0.2s;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.send-btn:hover {
  transform: scale(1.05);
}

.send-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.inline-system-notice {
  width: 100%;
  text-align: center;
  background: #e5e7eb;
  color: #444;
  border-radius: 8px;
  padding: 7px 12px;
  margin: 10px 0;
  font-size: 0.95em;
  font-style: italic;
  opacity: 0.85;
}

.new-session-section {
  padding: 16px;
}

.start-new-session-btn {
  background: linear-gradient(135deg, #ff6b6b, #ff5252);
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  width: 100%;
}

.start-new-session-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(255, 107, 107, 0.3);
  background: linear-gradient(135deg, #ff5252, #ff4444);
}

.start-new-session-btn:active {
  transform: translateY(0);
}

/* Footer */
.footer {
  background: #1f2937;
  color: white;
  padding: 4rem 2rem 2rem;
}

.footer-content {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 3rem;
  margin-bottom: 3rem;
  max-width: 1200px;
  margin-left: auto;
  margin-right: auto;
}

.footer-section h3 {
  margin-bottom: 1.5rem;
  color: #8bc792;
  font-family: "Georgia", serif;
}

.footer-section p,
.footer-section a {
  color: #d1d5db;
  text-decoration: none;
  line-height: 2;
  transition: color 0.3s ease;
}

.footer-section a:hover {
  color: #8bc792;
}

/* --- Disable header button effects --- */
.nav-links a,
.nav-links a:hover,
.nav-links a:active,
.book-btn,
.book-btn:hover,
.book-btn:active,
.mobile-auth-btn,
.mobile-auth-btn:hover,
.mobile-auth-btn:active {
  transition: none !important;
  box-shadow: none !important;
  transform: none !important;
  text-shadow: none !important;
  background: rgba(255, 255, 255, 0.05) !important;
  color: white !important;
  border-color: rgba(255, 255, 255, 0.1) !important;
}

.nav-links a::before,
.nav-links a::after,
.book-btn::before {
  display: none !important;
}

/* ===== RESPONSIVE STYLES ===== */

/* Tablets (768px and below) */
@media screen and (max-width: 768px) {
  .nav {
    padding: 0 1rem;
  }

  .mobile-menu-toggle {
    display: flex;
  }

  /* Hide desktop user actions */
  .desktop-only {
    display: none;
  }

  /* Show mobile user actions in menu */
  .mobile-user-actions {
    display: block;
    margin-top: 2rem;
    padding-top: 2rem;
    border-top: 1px solid rgba(255, 255, 255, 0.2);
  }

  .nav-links {
    position: fixed;
    top: 0;
    left: -100%;
    width: 100%;
    height: 100vh;
    background: linear-gradient(135deg, #78ba7e 0%, #6ba371 50%, #5e8c64 100%);
    flex-direction: column;
    justify-content: center;
    align-items: center;
    gap: 1.5rem;
    transition: left 0.3s ease;
    z-index: 1000;
    padding: 2rem;
    overflow-y: auto;
  }

  .nav-links.mobile-active {
    left: 0;
  }

  .nav-links li {
    text-align: center;
    width: 100%;
    max-width: 280px;
  }

  .nav-links a {
    font-size: 1.1rem;
    padding: 1rem 2rem;
    display: block;
    width: 100%;
    margin: 0.3rem 0;
  }

  .logo {
    font-size: 1.4rem;
  }

  .logo img {
    height: 40px !important;
  }

  /* Chat window responsive */
  .chat-window {
    width: 90vw;
    max-width: 350px;
    height: 80vh;
    max-height: 500px;
  }

  .footer-content {
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 2rem;
  }
}

/* Mobile phones (480px and below) */
@media screen and (max-width: 480px) {
  .nav {
    padding: 0 0.5rem;
  }

  .logo {
    font-size: 1.2rem;
  }

  .logo span {
    display: none;
  }

  .logo img {
    height: 35px !important;
  }

  .nav-links {
    padding: 1.5rem;
    gap: 1.2rem;
  }

  .nav-links a {
    font-size: 1rem;
    padding: 0.8rem 1.5rem;
  }

  .mobile-auth-btn {
    font-size: 0.9rem;
    padding: 0.8rem 1.2rem;
    min-width: 180px;
  }

  /* Chat positioning for mobile */
  .chat-popup-wrapper {
    bottom: 15px;
    right: 15px;
  }

  .chat-toggle-btn {
    width: 50px;
    height: 50px;
    font-size: 20px;
  }

  .chat-window {
    width: 95vw;
    height: 85vh;
    border-radius: 15px;
    bottom: 10px;
    right: 2.5vw;
    position: fixed;
  }

  .chat-header {
    padding: 15px;
  }

  .chat-header h3 {
    font-size: 1rem;
  }

  .chat-header p {
    font-size: 0.75em;
  }

  .welcome-screen {
    padding: 20px 15px;
  }

  .chat-messages {
    padding: 10px;
  }

  .message-bubble {
    max-width: 85%;
    font-size: 12px;
    padding: 8px 12px;
  }

  .chat-input {
    padding: 10px;
  }

  .message-input {
    font-size: 14px;
    padding: 8px 12px;
  }

  .send-btn {
    width: 36px;
    height: 36px;
  }

  /* Footer responsive */
  .footer {
    padding: 2rem 1rem 1rem;
  }

  .footer-content {
    grid-template-columns: 1fr;
    gap: 1.5rem;
    text-align: center;
  }

  .footer-section h3 {
    font-size: 1.1rem;
    margin-bottom: 1rem;
  }

  .footer-section p {
    font-size: 0.9rem;
    line-height: 1.6;
  }

  .main-content {
    margin-top: 70px;
    padding: 20px 0;
  }
}

/* Extra small devices (360px and below) */
@media screen and (max-width: 360px) {
  .nav {
    padding: 0 0.25rem;
  }

  .logo {
    font-size: 1rem;
  }

  .mobile-menu-toggle span {
    width: 20px;
    height: 2px;
  }

  .nav-links {
    padding: 1rem;
    gap: 1rem;
  }

  .nav-links a {
    font-size: 0.9rem;
    padding: 0.6rem 1rem;
  }

  .mobile-auth-btn {
    font-size: 0.85rem;
    padding: 0.7rem 1rem;
    min-width: 160px;
  }

  .chat-window {
    width: 98vw;
    height: 90vh;
    right: 1vw;
    border-radius: 10px;
  }

  .chat-toggle-btn {
    width: 45px;
    height: 45px;
    font-size: 18px;
  }

  .notification-badge {
    width: 18px;
    height: 18px;
    font-size: 10px;
  }

  .footer-section p {
    font-size: 0.8rem;
  }
}

/* Landscape mobile (max-height: 500px) */
@media screen and (max-height: 500px) and (orientation: landscape) {
  .chat-window {
    height: 90vh;
    width: 350px;
  }

  .nav-links {
    flex-direction: row;
    flex-wrap: wrap;
    justify-content: center;
    padding: 1rem;
    height: auto;
    top: 70px;
    gap: 0.5rem;
  }

  .nav-links li {
    max-width: none;
    width: auto;
  }

  .mobile-user-actions {
    width: 100%;
    display: flex;
    justify-content: center;
    gap: 1rem;
    flex-wrap: wrap;
    margin-top: 1rem;
    padding-top: 1rem;
  }

  .mobile-auth-btn {
    min-width: 120px;
    font-size: 0.8rem;
    padding: 0.6rem 1rem;
  }
}

/* High DPI displays */
@media screen and (-webkit-min-device-pixel-ratio: 2),
  screen and (min-resolution: 192dpi) {
  .logo img {
    image-rendering: -webkit-optimize-contrast;
    image-rendering: crisp-edges;
  }
}

/* Touch devices */
@media (hover: none) and (pointer: coarse) {
  .book-btn:hover,
  .nav-links a:hover,
  .chat-toggle-btn:hover,
  .send-btn:hover,
  .start-chat-btn:hover,
  .start-new-session-btn:hover,
  .mobile-auth-btn:hover {
    transform: none;
    box-shadow: inherit;
  }

  .book-btn:active,
  .chat-toggle-btn:active,
  .send-btn:active,
  .start-chat-btn:active,
  .mobile-auth-btn:active {
    transform: scale(0.95);
  }
}

/* Accessibility improvements */
@media (prefers-reduced-motion: reduce) {
  *,
  *::before,
  *::after {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }
}

/* Dark mode support */
@media (prefers-color-scheme: dark) {
  .chat-window {
    background: #1f2937;
    color: white;
  }

  .chat-messages {
    background: #111827;
  }

  .message.support .message-bubble {
    background: #374151;
    color: white;
    border-color: #4b5563;
  }

  .welcome-screen input {
    background: #374151;
    color: white;
    border-color: #4b5563;
  }

  .message-input {
    background: #374151;
    color: white;
    border-color: #4b5563;
  }
}
</style>
