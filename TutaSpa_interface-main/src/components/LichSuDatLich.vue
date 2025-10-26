<template>
  <div class="spa-history-container">
    <!-- Header Section -->
    <div class="header-section">
      <h1 class="page-title">Lịch Sử Đặt Lịch</h1>
      <p class="page-subtitle">
        Theo dõi các lần đặt lịch và trải nghiệm dịch vụ của bạn
      </p>
    </div>

    <!-- Empty State -->
    <div v-if="lichSu.length === 0" class="empty-state">
      <div class="empty-icon">🌿</div>
      <h3>Chưa có lịch sử đặt lịch</h3>
      <p>Hãy trải nghiệm những dịch vụ tuyệt vời của chúng tôi</p>
    </div>

    <!-- Two Column Layout -->
    <div v-else class="history-layout">
      <!-- Left Column - Lịch đã đặt (chưa thanh toán) -->
      <div class="history-column left-column">
        <div class="column-header pending">
          <div class="header-content">
            <div class="header-icon">📅</div>
            <div>
              <h2 class="column-title">Lịch Đã Đặt</h2>
              <p class="column-subtitle">
                {{ lichSuChuaThanhToan.length }} lịch hẹn đang chờ
              </p>
            </div>
          </div>
        </div>

        <div class="history-cards">
          <div
            v-for="datLich in lichSuChuaThanhToan"
            :key="datLich.datLichID"
            class="history-card pending-card"
          >
            <!-- Card Header -->
            <div class="card-header pending-header">
              <div class="date-info">
                <div class="date-icon">📅</div>
                <div>
                  <div class="date-text">
                    {{ formatDate(datLich.thoiGian) }}
                  </div>
                  <div class="payment-status pending">⏳ Chưa thanh toán</div>
                </div>
              </div>
              <div class="status-badge pending-badge">
                {{ datLich.trangThai }}
              </div>
            </div>

            <!-- Services List -->
            <div class="services-list">
              <div
                v-for="ct in datLich.chiTietDatLichs"
                :key="ct.chiTietDatLichID"
                class="service-item"
              >
                <div class="service-main">
                  <div class="service-icon-wrapper pending-icon">
                    <i
                      :class="getServiceIcon(ct.dichVu?.tenDichVu)"
                      class="service-icon"
                    ></i>
                  </div>
                  <div class="service-details">
                    <h4 class="service-name">{{ ct.dichVu.tenDichVu }}</h4>
                    <div class="service-info">
                      <span class="quantity">
                        <i class="fas fa-list-ol"></i>
                        Số lượng: {{ ct.soLuongDV }}
                      </span>
                      <span class="unit-price">
                        <i class="fas fa-tag"></i>
                        Giá: {{ formatCurrency(ct.dichVu.gia) }}
                      </span>
                    </div>
                  </div>
                  <div class="service-total pending-total">
                    {{ formatCurrency(ct.dichVu.gia * ct.soLuongDV) }}
                  </div>
                </div>

                <!-- Action Button -->
                <div class="service-actions">
                  <button
                    class="btn delete-btn"
                    @click="xoaLich(datLich.datLichID)"
                    title="Hủy lịch hẹn"
                  >
                    <i class="fas fa-times-circle"></i>
                    <span>Hủy lịch</span>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Right Column - Lịch đã hoàn thành -->
      <div class="history-column right-column">
        <div class="column-header completed">
          <div class="header-content">
            <div class="header-icon">✨</div>
            <div>
              <h2 class="column-title">Lịch Đã Hoàn Thành</h2>
              <p class="column-subtitle">
                {{ lichSuDaThanhToan.length }} lịch hẹn đã hoàn thành
              </p>
            </div>
          </div>
        </div>

        <div class="history-cards">
          <div
            v-for="datLich in lichSuDaThanhToan"
            :key="datLich.datLichID"
            class="history-card completed-card"
          >
            <!-- Card Header -->
            <div class="card-header completed-header">
              <div class="date-info">
                <div class="date-icon">📅</div>
                <div>
                  <div class="date-text">
                    {{ formatDate(datLich.thoiGian) }}
                  </div>
                  <div class="payment-status completed">✅ Đã thanh toán</div>
                </div>
              </div>
              <div class="status-badge completed-badge">
                {{ datLich.trangThai }}
              </div>
            </div>

            <!-- Services List -->
            <div class="services-list">
              <div
                v-for="ct in datLich.chiTietDatLichs"
                :key="ct.chiTietDatLichID"
                class="service-item"
              >
                <div class="service-main">
                  <div class="service-icon-wrapper completed-icon">
                    <i
                      :class="getServiceIcon(ct.dichVu?.tenDichVu)"
                      class="service-icon"
                    ></i>
                  </div>
                  <div class="service-details">
                    <h4 class="service-name">{{ ct.dichVu.tenDichVu }}</h4>
                    <div class="service-info">
                      <span class="quantity">
                        <i class="fas fa-list-ol"></i>
                        Số lượng: {{ ct.soLuongDV }}
                      </span>
                      <span class="unit-price">
                        <i class="fas fa-tag"></i>
                        Giá: {{ formatCurrency(ct.dichVu.gia) }}
                      </span>
                    </div>
                  </div>
                  <div class="service-total completed-total">
                    {{ formatCurrency(ct.dichVu.gia * ct.soLuongDV) }}
                  </div>
                </div>

                <!-- Action Button -->
                <div class="service-actions">
                  <router-link
                    v-if="ct.dichVu && !checkDaDanhGia(ct)"
                    :to="`/DanhGia/${ct.dichVu.dichVuID}`"
                    class="review-btn primary-btn"
                  >
                    <i class="fas fa-star"></i>
                    <span>Đánh giá dịch vụ</span>
                  </router-link>

                  <router-link
                    v-else-if="ct.dichVu && checkDaDanhGia(ct)"
                    :to="{
                      path: `/DanhGia/${ct.dichVu.dichVuID}`,
                      hash: '#danhgia',
                    }"
                    class="review-btn secondary-btn"
                  >
                    <i class="fas fa-eye"></i>
                    <span>Xem đánh giá của bạn</span>
                  </router-link>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import apiClient from "../utils/axiosClient";
import Swal from "sweetalert2";

const lichSu = ref([]);
const isLoading = ref(false);
const userInfo = JSON.parse(localStorage.getItem("user_info") || "{}");
const userId = userInfo.id;

// Map lưu trạng thái đánh giá của từng dịch vụ
const danhGiaStatus = ref({}); // { [dichVuID]: true/false }

onMounted(async () => {
  await loadLichSu();
  await loadDanhGiaStatus();
});
const formatCurrency = (value) =>
  new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(
    value
  );
const formatDate = (str) => {
  const d = new Date(str);
  return d.toLocaleDateString("vi-VN", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  });
};
// Load lịch sử đặt lịch
const loadLichSu = async () => {
  try {
    const res = await apiClient.get("/DatLich/by-user");
    lichSu.value = res;
  } catch (err) {
    console.error("⚠ Lỗi tải lịch sử dịch vụ:", err);
  }
};

// Load trạng thái đánh giá từng dịch vụ
const loadDanhGiaStatus = async () => {
  try {
    // Duyệt tất cả dịch vụ trong lichSu đã thanh toán
    for (const datLich of lichSu.value.filter((l) => l.daThanhToan)) {
      for (const ct of datLich.chiTietDatLichs) {
        if (ct.dichVu) {
          const res = await apiClient.get(
            `/danhgia/dichvu/${ct.dichVu.dichVuID}/${userId}`
          );
          danhGiaStatus.value[ct.dichVu.dichVuID] = res.hasReview;
        }
      }
    }
  } catch (err) {
    console.error("Lỗi tải trạng thái đánh giá:", err);
  }
};

// Kiểm tra đã đánh giá chưa
const checkDaDanhGia = (ct) => {
  return ct.dichVu ? !!danhGiaStatus.value[ct.dichVu.dichVuID] : false;
};

// Hàm lấy user ID
const getUserId = () => userId;

// Hàm lấy icon cho từng loại dịch vụ
const getServiceIcon = (tenDichVu) => {
  if (!tenDichVu) return "fas fa-spa";
  const serviceName = tenDichVu.toLowerCase();
  if (serviceName.includes("massage") || serviceName.includes("mát xa"))
    return "fas fa-hand-sparkles";
  if (serviceName.includes("facial") || serviceName.includes("chăm sóc da mặt"))
    return "fas fa-user-check";
  if (serviceName.includes("nail") || serviceName.includes("móng"))
    return "fas fa-hand-paper";
  if (serviceName.includes("hair") || serviceName.includes("tóc"))
    return "fas fa-cut";
  if (serviceName.includes("body") || serviceName.includes("toàn thân"))
    return "fas fa-user";
  if (serviceName.includes("foot") || serviceName.includes("chân"))
    return "fas fa-shoe-prints";
  if (serviceName.includes("therapy") || serviceName.includes("trị liệu"))
    return "fas fa-heart";
  if (serviceName.includes("relax") || serviceName.includes("thư giãn"))
    return "fas fa-leaf";
  return "fas fa-spa";
};

// Computed phân chia lịch
const lichSuDaThanhToan = computed(() => {
  return lichSu.value
    .filter((l) => l.daThanhToan)
    .sort((a, b) => {
      // Kiểm tra nếu a có ít nhất 1 dịch vụ chưa đánh giá
      const aHasNotReview = a.chiTietDatLichs.some((ct) => !checkDaDanhGia(ct));
      const bHasNotReview = b.chiTietDatLichs.some((ct) => !checkDaDanhGia(ct));

      if (aHasNotReview && !bHasNotReview) return -1; // a lên trước
      if (!aHasNotReview && bHasNotReview) return 1; // b lên trước

      // Nếu bằng nhau, sắp xếp theo thời gian (mới nhất lên trước)
      return new Date(b.thoiGian) - new Date(a.thoiGian);
    });
});

const lichSuChuaThanhToan = computed(() => {
  return lichSu.value
    .filter((l) => !l.daThanhToan)
    .sort((a, b) => new Date(b.thoiGian) - new Date(a.thoiGian));
});

const danhGiaList = ref([]); // Danh sách các đánh giá của user

onMounted(() => {
  loadLichSu();
  loadDanhGiaByUser();
});

// Load danh sách đánh giá của user
const loadDanhGiaByUser = async () => {
  try {
    const res = await apiClient.get(`/DanhGia/by-user${userId}`);
    danhGiaList.value = res;
  } catch (err) {
    console.error("Lỗi tải danh sách đánh giá:", err);
  }
};

const xoaLich = async (id) => {
  // Bước 1: Xác nhận hủy
  const confirmResult = await Swal.fire({
    title: "Xác nhận hủy lịch",
    text: "Bạn có chắc chắn muốn hủy lịch hẹn này không?",
    icon: "question",
    showCancelButton: true,
    confirmButtonColor: "#e74c3c",
    cancelButtonColor: "#27ae60",
    confirmButtonText: "Hủy lịch",
    cancelButtonText: "Đổi giờ với TutaSpa",
    background: "#fff",
    customClass: {
      title: "swal-title",
      content: "swal-content",
    },
  });

  // Nếu KH chọn ĐỔI GIỜ → điều hướng chat box trên trang chủ
  if (confirmResult.dismiss === Swal.DismissReason.cancel) {
    Swal.fire({
      title: "Đổi lịch hẹn",
      html: `
        <p>Bạn có thể chat trực tiếp với thu ngân TutaSpa để chọn giờ mới:</p>
        <button id="chatBtn" class="swal2-confirm swal2-styled" style="background:#27ae60">
          💬 Chat với TutaSpa
        </button>
      `,
      showConfirmButton: false,
      didOpen: () => {
        document.getElementById("chatBtn").addEventListener("click", () => {
          // 👉 Điều hướng đến trang chủ có chat box
          window.location.href = "/"; 
          // hoặc scroll tới chat box nếu có sẵn trên trang
        });
      },
    });
    return; // Không xóa lịch
  }

  // Nếu KH xác nhận HỦY
  if (confirmResult.isConfirmed) {
    try {
      isLoading.value = true;
      Swal.fire({
        title: "Đang xử lý...",
        text: "Vui lòng đợi trong giây lát",
        allowOutsideClick: false,
        didOpen: () => {
          Swal.showLoading();
        },
      });

      await apiClient.delete(`/DatLich/${id}`);
      await loadLichSu();

      await Swal.fire({
        icon: "success",
        title: "Đã hủy thành công",
        text: "Cảm ơn bạn, rất mong được phục vụ bạn vào lần sau!",
        confirmButtonText: "OK",
        confirmButtonColor: "#27ae60",
        background: "#f8fff8",
      });
    } catch (err) {
      console.error("Lỗi khi xóa lịch:", err);
      await Swal.fire({
        icon: "error",
        title: "Thất bại",
        text: "Đã xảy ra lỗi khi hủy lịch hẹn. Vui lòng thử lại!",
        confirmButtonText: "OK",
        confirmButtonColor: "#e74c3c",
      });
    } finally {
      Swal.close();
      isLoading.value = false;
    }
  }
};



// Safe helpers to handle items that may be service OR product (or missing)
const getTen = (ct) => {
  if (ct && ct.dichVu) return ct.dichVu.tenDichVu;
  if (ct && ct.sanPham)
    return ct.sanPham.tenSanPham || `Sản phẩm #${ct.sanPhamID ?? ""}`;
  const label = ct?.dichVuID
    ? `Dịch vụ #${ct.dichVuID}`
    : ct?.sanPhamID
    ? `Sản phẩm #${ct.sanPhamID}`
    : "Mục không xác định";
  return label;
};

const getGia = (ct) => {
  if (ct && ct.dichVu) return ct.dichVu.gia;
  if (ct && ct.sanPham) {
    return (
      ct.sanPham.gia ??
      ((ct?.soLuongSP
        ? Number(ct.thanhTien) / Number(ct.soLuongSP)
        : Number(ct.thanhTien)) ||
        0)
    );
  }
  return (
    (ct?.soLuongSP
      ? Number(ct?.thanhTien) / Number(ct?.soLuongSP)
      : Number(ct?.thanhTien)) || 0
  );
};
</script>

<style scoped>
.spa-history-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f9f5 0%, #e8f5e8 100%);
  padding: 2rem 1rem;
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
}

/* Header Section */
.header-section {
  text-align: center;
  margin-bottom: 3rem;
  max-width: 1200px;
  margin-left: auto;
  margin-right: auto;
}

.page-title {
  font-size: 2.8rem;
  font-weight: 700;
  color: #2d5a2d;
  margin-bottom: 1rem;
  letter-spacing: -0.02em;
  text-shadow: 0 2px 4px rgba(45, 90, 45, 0.1);
}

.page-subtitle {
  font-size: 1.2rem;
  color: #5a7a5a;
  margin-bottom: 0;
  line-height: 1.6;
}

/* Empty State */
.empty-state {
  text-align: center;
  padding: 4rem 2rem;
  background: white;
  border-radius: 20px;
  box-shadow: 0 10px 30px rgba(45, 90, 45, 0.1);
  max-width: 500px;
  margin: 0 auto;
}

.empty-icon {
  font-size: 4rem;
  margin-bottom: 1.5rem;
}

.empty-state h3 {
  color: #2d5a2d;
  font-size: 1.5rem;
  margin-bottom: 0.5rem;
}

.empty-state p {
  color: #5a7a5a;
  font-size: 1rem;
}

/* Two Column Layout */
.history-layout {
  max-width: 1400px;
  margin: 0 auto;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  align-items: start;
}

.history-column {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

/* Column Headers */
.column-header {
  background: white;
  border-radius: 15px;
  padding: 1.5rem;
  box-shadow: 0 8px 25px rgba(45, 90, 45, 0.1);
  border-top: 4px solid;
  position: sticky;
  top: 1rem;
  z-index: 10;
}

.column-header.completed {
  border-top-color: #27ae60;
  background: linear-gradient(
    135deg,
    rgba(39, 174, 96, 0.08) 0%,
    rgba(39, 174, 96, 0.03) 100%
  );
}

.column-header.pending {
  border-top-color: #f39c12;
  background: linear-gradient(
    135deg,
    rgba(243, 156, 18, 0.08) 0%,
    rgba(243, 156, 18, 0.03) 100%
  );
}

.header-content {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.header-icon {
  font-size: 2.5rem;
}

.column-title {
  color: #2d5a2d;
  font-size: 1.4rem;
  font-weight: 700;
  margin: 0 0 0.25rem 0;
}

.column-subtitle {
  color: #5a7a5a;
  font-size: 0.95rem;
  margin: 0;
  opacity: 0.8;
}

/* History Cards */
.history-cards {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.history-card {
  background: white;
  border-radius: 18px;
  box-shadow: 0 8px 25px rgba(45, 90, 45, 0.08);
  overflow: hidden;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
}

.history-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 15px 35px rgba(45, 90, 45, 0.12);
}

/* Completed Cards */
.completed-card {
  border-left: 4px solid #27ae60;
}

.completed-card::before {
  content: "✓";
  position: absolute;
  top: 15px;
  right: 15px;
  width: 30px;
  height: 30px;
  background: linear-gradient(135deg, #27ae60, #2ecc71);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 16px;
  font-weight: bold;
  z-index: 1;
}

/* Pending Cards */
.pending-card {
  border-left: 4px solid #f39c12;
  animation: subtle-pulse 3s infinite;
}

.pending-card::before {
  content: "⏳";
  position: absolute;
  top: 15px;
  right: 15px;
  width: 30px;
  height: 30px;
  background: linear-gradient(135deg, #f39c12, #e67e22);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 14px;
  z-index: 1;
}

@keyframes subtle-pulse {
  0%,
  100% {
    box-shadow: 0 8px 25px rgba(45, 90, 45, 0.08);
  }
  50% {
    box-shadow: 0 8px 25px rgba(243, 156, 18, 0.15);
  }
}

/* Card Headers */
.card-header {
  color: white;
  padding: 1.5rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: relative;
  z-index: 2;
}

.completed-header {
  background: linear-gradient(135deg, #27ae60 0%, #2ecc71 100%);
}

.pending-header {
  background: linear-gradient(135deg, #f39c12 0%, #e67e22 100%);
}

.date-info {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.date-icon {
  font-size: 1.8rem;
  opacity: 0.9;
}

.date-text {
  font-size: 1.1rem;
  font-weight: 600;
  margin-bottom: 0.25rem;
}

.payment-status {
  font-size: 0.85rem;
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-weight: 500;
  background: rgba(255, 255, 255, 0.2);
  color: #fff;
}

.status-badge {
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 500;
  background: rgba(255, 255, 255, 0.25);
  backdrop-filter: blur(10px);
}

/* Services List */
.services-list {
  padding: 0;
}

.service-item {
  padding: 1.5rem;
  border-bottom: 1px solid #f5f5f5;
  position: relative;
}

.service-item:last-child {
  border-bottom: none;
}

.service-main {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  margin-bottom: 1rem;
}

/* Service Icons */
.service-icon-wrapper {
  width: 50px;
  height: 50px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.service-icon-wrapper.completed-icon {
  background: linear-gradient(
    135deg,
    rgba(39, 174, 96, 0.1) 0%,
    rgba(46, 204, 113, 0.1) 100%
  );
  border: 2px solid rgba(39, 174, 96, 0.2);
}

.service-icon-wrapper.pending-icon {
  background: linear-gradient(
    135deg,
    rgba(243, 156, 18, 0.1) 0%,
    rgba(230, 126, 34, 0.1) 100%
  );
  border: 2px solid rgba(243, 156, 18, 0.2);
}

.service-icon {
  font-size: 1.5rem;
}

.completed-icon .service-icon {
  color: #27ae60;
}

.pending-icon .service-icon {
  color: #f39c12;
}

.service-details {
  flex: 1;
}

.service-name {
  color: #2d5a2d;
  font-size: 1.2rem;
  font-weight: 600;
  margin: 0 0 0.5rem 0;
  line-height: 1.4;
}

.service-info {
  display: flex;
  gap: 1.5rem;
  flex-wrap: wrap;
}

.quantity,
.unit-price {
  color: #5a7a5a;
  font-size: 0.95rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.quantity i,
.unit-price i {
  font-size: 0.8rem;
  opacity: 0.7;
}

.service-total {
  font-size: 1.3rem;
  font-weight: 700;
  text-align: right;
  flex-shrink: 0;
}

.completed-total {
  color: #27ae60;
}

.pending-total {
  color: #f39c12;
}

/* Service Actions */
.service-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
}

.review-btn,
.delete-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.25rem;
  border-radius: 25px;
  text-decoration: none;
  font-weight: 500;
  font-size: 0.9rem;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  border: none;
  cursor: pointer;
}

.primary-btn {
  background: linear-gradient(135deg, #27ae60 0%, #2ecc71 100%);
  color: white;
  box-shadow: 0 4px 15px rgba(39, 174, 96, 0.3);
}

.primary-btn:hover {
  background: linear-gradient(135deg, #219a52 0%, #27ae60 100%);
  transform: translateY(-2px) scale(1.03);
  box-shadow: 0 8px 25px rgba(39, 174, 96, 0.4);
  color: white;
}

.secondary-btn {
  background: linear-gradient(135deg, #3498db 0%, #5dade2 100%);
  color: white;
  box-shadow: 0 4px 15px rgba(52, 152, 219, 0.3);
}

.secondary-btn:hover {
  background: linear-gradient(135deg, #2980b9 0%, #3498db 100%);
  transform: translateY(-2px) scale(1.03);
  box-shadow: 0 8px 25px rgba(52, 152, 219, 0.4);
  color: white;
}

.delete-btn {
  background: linear-gradient(135deg, #e74c3c 0%, #ec7063 100%);
  color: white;
  box-shadow: 0 4px 15px rgba(231, 76, 60, 0.3);
}

.delete-btn:hover {
  background: linear-gradient(135deg, #c0392b 0%, #e74c3c 100%);
  transform: translateY(-2px) scale(1.03);
  box-shadow: 0 8px 25px rgba(231, 76, 60, 0.4);
}

/* SweetAlert2 Custom Styles */
.swal-title {
  color: #2d5a2d !important;
  font-weight: 600 !important;
}

.swal-content {
  color: #5a7a5a !important;
}

/* Responsive Design */
@media (max-width: 1024px) {
  .history-layout {
    grid-template-columns: 1fr;
    gap: 2rem;
  }

  .column-header {
    position: static;
  }
}

@media (max-width: 768px) {
  .spa-history-container {
    padding: 1rem 0.5rem;
  }

  .page-title {
    font-size: 2.2rem;
  }

  .card-header {
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }

  .service-main {
    flex-direction: column;
    gap: 1rem;
  }

  .service-total {
    text-align: left;
    font-size: 1.2rem;
  }

  .service-info {
    flex-direction: column;
    gap: 0.5rem;
  }

  .service-actions {
    justify-content: stretch;
    flex-direction: column;
  }

  .review-btn,
  .delete-btn {
    width: 100%;
    justify-content: center;
  }

  .column-title {
    font-size: 1.2rem;
  }
}

@media (max-width: 480px) {
  .page-title {
    font-size: 1.8rem;
  }

  .service-icon-wrapper {
    width: 40px;
    height: 40px;
  }

  .service-icon {
    font-size: 1.2rem;
  }

  .service-name {
    font-size: 1.1rem;
  }

  .header-content {
    flex-direction: column;
    text-align: center;
    gap: 0.5rem;
  }
}
</style>
