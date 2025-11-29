<template>
  <div class="layout">
    <!-- Top Navbar -->
    <header class="top-nav">
      <div class="nav-left">
        <a href="/" style="text-decoration: none">
          <img src="/src/assets/img/logo.png" alt="Logo" class="logo-img" />
          <span class="brand">TutaSpa </span>
        </a>
      </div>

      <div class="nav-right" v-if="authStore.isAuthenticated">
        <!-- Thông báo cho Cashier -->
        <div style="position: relative">
          <i
            v-if="authStore.isCashier"
            class="fas fa-bell notification-icon"
            @click="toggleNotifications"
            style="cursor: pointer"
          ></i>
          <span v-if="hasUnread" class="notification-dot"></span>
          <div v-if="showNotifications" class="notification-list" @click.stop>
            <div v-if="notifications.length === 0" class="notification-empty">
              Không có thông báo mới
            </div>
            <ul v-else>
              <li v-for="(noti, idx) in notifications" :key="idx">
                <router-link :to="`/chat?sessionId=${noti.sessionId}`">
                  <span v-if="noti.type === 'message'">💬</span>
                  <span v-else>👤</span>
                  {{ noti.content }}
                  <span class="notification-time">{{ noti.time }}</span>
                </router-link>
              </li>
            </ul>
          </div>
        </div>

        <!-- Dropdown -->
        <div class="dropdown">
          <button
            class="btn btn-light dropdown-toggle"
            id="dropdownUser"
            data-bs-toggle="dropdown"
          >
            <img
              src="/src/assets/img/user-1.jpg"
              alt="User"
              class="avatar-img"
            />
          </button>
          <ul
            class="dropdown-menu dropdown-menu-end mt-2 shadow rounded-3 p-3"
            aria-labelledby="dropdownUser"
          >
            <li>
              <a
                class="dropdown-item d-flex align-items-center gap-2 text-secondary"
                href="/ChangePassword"
              >
                <i class="fas fa-envelope fs-6"></i> Đổi mật khẩu
              </a>
            </li>
            <li>
              <a class="btn btn-outline-primary w-100" href="#" @click="Logout">
                Đăng xuất
              </a>
            </li>
          </ul>
        </div>
      </div>
    </header>

    <!-- Sidebar -->
    <aside class="sidebar" v-if="authStore.isAuthenticated">
      <div class="menu">
        <div class="menu-title">HOME</div>

        <!-- Admin -->
        <template v-if="authStore.isAdmin">
          <router-link
            to="/"
            class="menu-item"
            exact-active-class="active"
            title="Dashboard"
          >
            <i class="fas fa-th-large"></i>
          </router-link>
          <router-link
            to="/QlDichVu"
            class="menu-item"
            exact-active-class="active"
            title="Quản lý dịch vụ"
          >
            <i class="fas fa-hand-sparkles"></i>
          </router-link>
          <router-link
            to="/khuyenMai"
            class="menu-item"
            exact-active-class="active"
            title="Quản lý Khuyến mãi"
          >
            <i class="fas fa-tags"></i>
          </router-link>
          <router-link
            to="/QuanLySanPham"
            class="menu-item"
            exact-active-class="active"
            title="Quản lý Sản phẩm"
          >
            <i class="fas fa-pump-soap"></i>
          </router-link>
          <router-link
            to="/employees"
            class="menu-item"
            exact-active-class="active"
            title="Quản lý nhân viên"
          >
            <i class="fas fa-user-friends"></i>
          </router-link>
          <router-link
            to="/DanhGiaKhachHang"
            class="menu-item"
            exact-active-class="active"
            title="Quản lý Đánh Giá"
          >
            <i class="fas fa-star"></i>
          </router-link>
        </template>

        <!-- InventoryManager -->
        <template v-if="authStore.isInventoryManager"> 
        <router-link
          to="/kho"
          class="menu-item"
          exact-active-class="active"
          title="Quản lý kho"
        >
          <i class="fas fa-columns"></i>
        </router-link>
         </template> 

        <!-- Cashier -->
        <template v-if="authStore.isCashier"> 
        <router-link
          to="/ThuNgan"
          class="menu-item"
          exact-active-class="active"
          title="Thu Ngân"
        >
          <i class="fa-solid fa-wallet"></i>
        </router-link>
        <router-link
          to="/chat"
          class="menu-item"
          exact-active-class="active"
          title="Chat"
        >
          <i class="fa-solid fa-comment"></i>
        </router-link>
        </template> 

        <div v-if="authStore.isAdmin" class="menu-title">Thống kê</div>

        <!-- Thống kê -->
        <router-link
          v-if="authStore.isAdmin"
          to="/thongke"
          class="menu-item"
          exact-active-class="active"
          title="Thống kê"
        >
          <i class="fas fa-store"></i>
        </router-link>
        <router-link
          v-if="authStore.isAdmin"
          to="/thongkehoadon"
          class="menu-item"
          exact-active-class="active"
          title="Thống kê hóa đơn"
        >
          <i class="fas fa-chart-simple"></i>
        </router-link>
      </div>
    </aside>

    <!-- Main Content -->
    <div class="main">
      <main class="main-content">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { authAPI } from "./services/authservice";
import { useAuthStore } from "./services/authStore";
import {
  getSignalRConnection,
  registerSignalREvent,
  getConnectionState,
  createSignalRConnection,
} from "./services/chatService";

const router = useRouter();
const authStore = useAuthStore();

const notifications = ref([]);
const showNotifications = ref(false);
const hasUnread = ref(false);

onMounted(() => {
  if (
    !authStore.isAuthenticated &&
    router.currentRoute.value.path !== "/forget-password"
  ) {
    router.push("/login");
    return;
  }

  if (authStore.isCashier) {
    if (getConnectionState() === "Disconnected") {
      createSignalRConnection(authStore.token);
    }

    registerSignalREvent("ReceiveMessage", (message) => {
      notifications.value.unshift({
        sessionId: message.sessionId,
        type: "message",
        content: `Tin nhắn mới từ ${message.fromUserName}: ${message.message}`,
        time: new Date().toLocaleTimeString(),
      });
      hasUnread.value = true;
    });

    registerSignalREvent("UserAssigned", (user) => {
      console.log("UserAssigned event received:", user);
      notifications.value.unshift({
        sessionId: user.sessionId,
        type: "assign",
        content: `Có khách hàng mới cần hỗ trợ: ${user.customerName}`,
        time: new Date().toLocaleTimeString(),
      });
      hasUnread.value = true;
    });
  }
});

function toggleNotifications() {
  showNotifications.value = !showNotifications.value;
  if (showNotifications.value) {
    hasUnread.value = false;
  }
}

async function Logout() {
  try {
    await authAPI.logout();
    authStore.logout();

    localStorage.removeItem("accessToken");
    localStorage.removeItem("user-info");
    router.push("/login");
  } catch (error) {
    console.error("Logout error:", error);
    router.push("/login");
  }
}
</script>

<style scoped>
.layout {
  display: flex;
  height: 100vh;
  padding-top: 80px; /* Đẩy main xuống dưới navbar */
}

/* Navbar */
.top-nav {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 80px;
  background-color: #fff;
  border-bottom: 1px solid #e2e8f0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 20px;
  z-index: 999;
}

.nav-left {
  display: flex;
  align-items: center;
}

.logo-img {
  width: 50px;
  height: auto;
  margin-right: 10px;
}

.brand {
  font-weight: bold;
  font-size: 26px;
  color: #c16fcd;
}

.nav-right {
  display: flex;
  align-items: center;
  gap: 20px;
}

.notification-icon {
  font-size: 20px;
  color: #475569;
}

/* Sidebar */
.sidebar {
  width: 80px;
  background-color: #fff;
  border-right: 1px solid #e2e8f0;
  padding: 20px 0;
  display: flex;
  flex-direction: column;
}

.menu {
  flex: 1;
  display: flex;
  flex-direction: column;
}
.menu-title {
  font-size: 10px;
  font-weight: bold;
  text-transform: uppercase;
  color: #64748b;
  padding: 10px 0;
  text-align: center;
}
.menu-item {
  font-size: 22px;
  padding: 14px;
  display: flex;
  justify-content: center;
  align-items: center;
  color: #1e293b;
  text-decoration: none;
  transition: background 0.2s;
}
.menu-item:hover {
  background-color: #f1f5f9;
}
.menu-item.active {
  background-color: #3b82f6;
  color: #fff;
  border-radius: 8px;
}

/* Dropdown */
.avatar-img {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  object-fit: cover;
}

.dropdown-menu {
  min-width: 220px;
}
.dropdown-item {
  font-size: 14px;
  padding: 6px 12px;
}
.dropdown-item i {
  margin-right: 10px;
}
.dropdown-item:hover {
  background-color: #f1f5f9;
}
.btn-outline-primary {
  border-radius: 8px;
  font-weight: 500;
}

/* Main Content */
.main {
  flex: 1;
  display: flex;
  flex-direction: column;
}
.main-content {
  padding: 20px;
  flex: 1;
  overflow-y: auto;
}

/* Thông báo */
.notification-dot {
  position: absolute;
  top: 2px;
  right: 2px;
  width: 10px;
  height: 10px;
  background: #e11d48;
  border-radius: 50%;
  border: 2px solid #fff;
  z-index: 10;
}
.notification-list {
  position: absolute;
  right: 0;
  top: 30px;
  width: 320px;
  background: #fff;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  padding: 10px 0;
  z-index: 1000;
}
.notification-list ul {
  list-style: none;
  margin: 0;
  padding: 0;
  max-height: 320px;
  overflow-y: auto;
}
.notification-list li {
  padding: 8px 16px;
  font-size: 15px;
  border-bottom: 1px solid #f1f5f9;
  display: flex;
  align-items: center;
  gap: 8px;
}
.notification-list li:last-child {
  border-bottom: none;
}
.notification-time {
  margin-left: auto;
  font-size: 12px;
  color: #64748b;
}
.notification-empty {
  text-align: center;
  color: #64748b;
  padding: 20px 0;
  font-size: 15px;
}
</style>
