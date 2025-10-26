<template>
  <div class="review-page">
    <!-- Hero Section -->
    <div class="hero-section">
      <div class="container">
        <div class="row justify-content-center">
          <div class="col-lg-8 text-center">
            <h1 class="hero-title mb-3">
              <i class="fas fa-star text-warning me-2"></i>
              Đánh Giá Dịch Vụ
            </h1>
            <p id="topdanhgia" class="hero-subtitle">
              Chia sẻ trải nghiệm của bạn với chúng tôi
            </p>
          </div>
        </div>
      </div>
    </div>

    <div class="container py-5">
      <div class="row justify-content-center">
        <div class="col-lg-8">
          <!-- Main Form Card -->
          <div class="card review-card shadow-lg border-0 mb-5">
            <div
              class="card-header bg-gradient-primary text-white text-center py-4"
            >
              <h4 class="mb-0">
                <i class="fas fa-edit me-2"></i>
                {{ editingId ? "Chỉnh sửa đánh giá" : "Viết đánh giá của bạn" }}
              </h4>
            </div>
            <div class="card-body p-4">
              <form @submit.prevent="submitDanhGia">
                <!-- Dịch vụ -->
                <div class="mb-4">
                  <label class="form-label fw-bold">
                    <i class="fas fa-concierge-bell text-primary me-2"></i>
                    Dịch vụ
                  </label>
                  <div v-if="route.params.id" class="service-badge">
                    <div class="d-flex align-items-center">
                      <i class="fas fa-star text-warning me-2"></i>
                      <span class="fw-semibold">
                        {{
                          dichVus.find((d) => d.dichVuID === danhGia.maDichVu)
                            ?.tenDichVu || "Đang tải..."
                        }}
                      </span>
                    </div>
                  </div>
                  <select
                    v-else
                    class="form-select form-select-lg"
                    v-model="danhGia.maDichVu"
                    required
                  >
                    <option value="" disabled>Chọn dịch vụ</option>
                    <option
                      v-for="d in dichVus"
                      :key="d.dichVuID"
                      :value="d.dichVuID"
                    >
                      {{ d.tenDichVu }}
                    </option>
                  </select>
                </div>

                <!-- Đánh giá sao -->
                <div class="mb-4">
                  <label class="form-label fw-bold">
                    <i class="fas fa-star text-warning me-2"></i>
                    Đánh giá của bạn
                  </label>
                  <div class="star-rating-container">
                    <div
                      class="star-rating d-flex justify-content-center gap-2 py-3"
                    >
                      <span
                        v-for="i in 5"
                        :key="i"
                        class="star-btn"
                        :class="{
                          active: i <= (hoveredRating || danhGia.soSao),
                          inactive: i > (hoveredRating || danhGia.soSao),
                        }"
                        @click="setRating(i)"
                        @mouseenter="hoverRating(i)"
                        @mouseleave="resetHover"
                      >
                        <i class="fas fa-star"></i>
                      </span>
                    </div>
                    <div class="rating-text text-center">
                      <span class="badge bg-light text-dark fs-6 px-3 py-2">
                        {{ getRatingText(hoveredRating || danhGia.soSao) }}
                      </span>
                    </div>
                  </div>
                </div>

                <!-- Nội dung -->
                <div class="mb-4">
                  <label class="form-label fw-bold">
                    <i class="fas fa-comment-alt text-info me-2"></i>
                    Nội dung đánh giá
                  </label>
                  <textarea
                    class="form-control"
                    rows="6"
                    v-model="danhGia.noiDung"
                    placeholder="Hãy chia sẻ trải nghiệm của bạn..."
                    maxlength="500"
                  ></textarea>
                  <div class="form-text text-end">
                    {{ danhGia.noiDung.length }}/500 ký tự
                  </div>
                </div>

                <!-- Toggle ẩn danh -->
                <div class="mb-4">
                  <div class="card bg-light border-0">
                    <div class="card-body">
                      <div
                        class="d-flex justify-content-between align-items-center"
                      >
                        <div class="d-flex align-items-center">
                          <i class="fas fa-user text-success me-2"></i>
                          <span class="fw-semibold">Hiện tên</span>
                        </div>

                        <div class="form-check form-switch">
                          <input
                            class="form-check-input"
                            type="checkbox"
                            id="anDanhSwitch"
                            v-model="danhGia.anDanh"
                          />
                        </div>

                        <div class="d-flex align-items-center">
                          <i class="fas fa-user-secret text-secondary me-2"></i>
                          <span class="fw-semibold">Ẩn danh</span>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Submit button -->
                <div class="d-grid">
                  <button
                    type="submit"
                    class="btn btn-primary btn-lg submit-button"
                    :disabled="isSubmitting"
                  >
                    <span
                      v-if="isSubmitting"
                      class="spinner-border spinner-border-sm me-2"
                    ></span>
                    <i v-else class="fas fa-paper-plane me-2"></i>

                    <span v-if="isSubmitting">Đang gửi...</span>
                    <span v-else>{{
                      editingId ? "Cập nhật đánh giá" : "Gửi đánh giá"
                    }}</span>
                  </button>
                </div>
              </form>
            </div>
          </div>

          <!-- Đánh giá gần đây -->
          <div id="danhgia">
            <div v-if="danhSach.length > 0" class="recent-reviews">
              <div class="text-center mb-4">
                <h3 class="text-white fw-bold">
                  <i class="fas fa-comments me-2"></i>
                  Đánh giá gần đây
                </h3>
              </div>

              <div
                v-for="(dg, index) in danhSach"
                :key="dg.id"
                class="card review-item shadow border-0 mb-3"
                :style="{ animationDelay: `${index * 0.1}s` }"
              >
                <div class="card-body">
                  <div
                    class="d-flex justify-content-between align-items-center mb-3"
                  >
                    <div class="review-stars">
                      <span
                        v-for="i in 5"
                        :key="i"
                        class="star-display me-1"
                        :class="{
                          'text-warning': i <= dg.soSao,
                          'text-muted': i > dg.soSao,
                        }"
                      >
                        <i class="fas fa-star"></i>
                      </span>
                    </div>
                    <small class="text-muted">
                      <i class="fas fa-calendar-alt me-1"></i>
                      {{ formatDate(dg.ngayTao) }}
                    </small>
                  </div>

                  <div class="review-content mb-3">
                    <p class="mb-0">{{ dg.noiDung }}</p>
                  </div>

                  <div class="review-author d-flex align-items-center mb-2">
                    <i class="fas fa-user-circle text-primary me-2"></i>
                    <span class="text-muted">Người đánh giá:</span>
                    <span v-if="dg.anDanh" class="badge bg-secondary ms-2">
                      <i class="fas fa-user-secret me-1"></i>
                      Ẩn danh
                    </span>
                    <span v-else class="fw-semibold text-dark ms-2">
                      {{ dg.user?.name || "Không rõ" }}
                    </span>
                  </div>

                  <!-- Nút Sửa -->
                  <div class="review-actions text-end">
                    <button
                      v-if="dg.userId === userId"
                      class="btn btn-sm btn-outline-primary"
                      @click="handleEdit(dg)"
                    >
                      <i class="fas fa-edit me-1"></i> Sửa
                    </button>
                  </div>
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
import { ref, onMounted, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import axiosClient from "../utils/axiosClient";
import Swal from "sweetalert2";

const route = useRoute();
const router = useRouter();

// Reactive data
const danhGia = ref({
  maDichVu: "",
  soSao: 5,
  noiDung: "",
  anDanh: false,
});
const editingId = ref(null);
const dichVus = ref([]);
const danhSach = ref([]);
const isSubmitting = ref(false);
const hoveredRating = ref(0);

const userInfoStr = localStorage.getItem("user_info");
const userInfo = JSON.parse(userInfoStr);
const userId = userInfo.id;

function editDanhGia(dg) {
  editingId.value = dg.id;
  danhGia.value = {
    maDichVu: dg.maDichVu,
    soSao: dg.soSao,
    noiDung: dg.noiDung,
    anDanh: dg.anDanh,
  };
}

// Rating text mapping
const ratingTexts = {
  1: "😞 Rất không hài lòng",
  2: "😕 Không hài lòng",
  3: "🙂 Bình thường",
  4: "😊 Hài lòng",
  5: "🤩 Rất hài lòng",
};
const handleEdit = async (dg) => {
  editDanhGia(dg);

  const el = document.querySelector("#topdanhgia");
  if (el) {
    el.scrollIntoView({ behavior: "smooth", block: "start" });
  }
  router.replace({ hash: "topdanhgia" });
};
const getRatingText = (rating) => ratingTexts[rating] || "";
const setRating = (rating) => (danhGia.value.soSao = rating);
const hoverRating = (rating) => (hoveredRating.value = rating);
const resetHover = () => (hoveredRating.value = 0);

const formatDate = (dateString) => {
  if (!dateString) return "";
  return new Date(dateString).toLocaleDateString("vi-VN");
};

// Load dịch vụ + đánh giá
onMounted(async () => {
  const res = await axiosClient.get("DichVu");
  dichVus.value = res;

  const idFromRoute = route.params.id;
  if (idFromRoute) {
    danhGia.value.maDichVu = parseInt(idFromRoute);
    await loadDanhGia(idFromRoute);
  }
});

watch(
  () => danhGia.value.maDichVu,
  async (newVal) => {
    if (newVal) await loadDanhGia(newVal);
  }
);

async function loadDanhGia(maDichVu) {
  const res = await axiosClient.get(`DanhGia/dichvu/${maDichVu}`);
  danhSach.value = res;
}

async function submitDanhGia() {
  try {
    isSubmitting.value = true;

    if (editingId.value) {
      // Update
      await axiosClient.put(`/DanhGia/update/${editingId.value}`, {
        ...danhGia.value,
        userId,
      });
      //Thông báo thành công
      await Swal.fire({
        icon: "success",
        title: "Cập nhật thành công!",
        showConfirmButton: false,
        timer: 1500,
      });

      editingId.value = null;
    } else {
      // Create new
      await axiosClient.post("/DanhGia", {
        ...danhGia.value,
        userId,
      });
      await Swal.fire({
        icon: "success",
        title: "Thành công",
        text: "Đánh giá thành công!",
        confirmButtonText: "OK",
      });
    }

    const currentDichVuID = danhGia.value.maDichVu;
    danhGia.value = {
      maDichVu: currentDichVuID,
      soSao: 5,
      noiDung: "",
      anDanh: false,
    };
    await loadDanhGia(currentDichVuID);
  } catch (err) {
    console.error(err);
    await Swal.fire({
      position: "center",
      icon: "error",
      title: "Gửi thất bại!",
      showConfirmButton: false,
      timer: 2000,
    });
  } finally {
    isSubmitting.value = false;
  }
}
</script>

<style scoped>
@import url("https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap");

.review-page {
  font-family: "Inter", sans-serif;
  background: linear-gradient(135deg, #8de499 0%, #8de499 50%, #8de499 100%);
  min-height: 100vh;
}

.hero-section {
  background: linear-gradient(
    135deg,
    rgba(168, 255, 214, 0.9),
    rgba(255, 250, 182, 0.9)
  );
  padding: 4rem 0 2rem;
  position: relative;
  overflow: hidden;
}

.hero-section::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1000 100" fill="white" opacity="0.1"><polygon points="0,100 1000,0 1000,100"/></svg>');
  background-size: cover;
}

.hero-title {
  font-size: 3.5rem;
  font-weight: 700;
  color: #5e8c64;
  text-shadow: 0 4px 20px rgba(0, 0, 0, 0.2);
  animation: fadeInUp 1s ease;
}

.hero-subtitle {
  font-size: 1.3rem;
  color: #5e8c64;
  font-weight: 300;
  animation: fadeInUp 1s ease 0.2s both;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.review-card {
  border-radius: 20px;
  overflow: hidden;
  backdrop-filter: blur(10px);
  animation: slideInUp 0.8s ease;
}

.review-card .card-header {
  background: linear-gradient(135deg, #5e8c64, #5e8c64) !important;
  border: none;
  position: relative;
}

.review-card .card-header::after {
  content: "";
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 3px;
  background: linear-gradient(90deg, #ffeaa7, #fdcb6e, #ffeaa7);
  background-size: 200% 100%;
  animation: shimmer 3s ease-in-out infinite;
}

@keyframes shimmer {
  0%,
  100% {
    background-position: 0% 50%;
  }
  50% {
    background-position: 100% 50%;
  }
}

@keyframes slideInUp {
  from {
    opacity: 0;
    transform: translateY(50px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.bg-gradient-primary {
  background: linear-gradient(135deg, #74b9ff, #5e8c64) !important;
}

.form-label {
  color: #2d3748;
  font-size: 1.1rem;
  margin-bottom: 0.75rem;
}

.form-control,
.form-select {
  border: 2px solid #e2e8f0;
  border-radius: 12px;
  padding: 0.875rem 1rem;
  font-size: 1rem;
  transition: all 0.3s ease;
  background: #f8fafc;
}

.form-control:focus,
.form-select:focus {
  border-color: #74b9ff;
  box-shadow: 0 0 0 0.25rem rgba(116, 185, 255, 0.15);
  background: white;
  transform: translateY(-2px);
}

.service-badge {
  background: linear-gradient(135deg, #5e8c64, #5e8c64);
  color: white;
  padding: 1rem 1.5rem;
  border-radius: 12px;
  box-shadow: 0 4px 15px rgba(0, 184, 148, 0.2);
}

.star-rating-container {
  background: linear-gradient(135deg, #f8f9fa, #e9ecef);
  border-radius: 16px;
  padding: 1.5rem;
  border: 2px solid #dee2e6;
  position: relative;
  overflow: hidden;
}

.star-rating-container::before {
  content: "";
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    90deg,
    transparent,
    rgba(116, 185, 255, 0.1),
    transparent
  );
  animation: starGlow 2s ease-in-out infinite;
}

@keyframes starGlow {
  0% {
    left: -100%;
  }
  100% {
    left: 100%;
  }
}

.star-btn {
  font-size: 2.5rem;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  z-index: 2;
  padding: 0.5rem;
  border-radius: 50%;
}

.star-btn:hover {
  transform: scale(1.2) rotate(10deg);
  filter: drop-shadow(0 4px 8px rgba(255, 193, 7, 0.4));
}

.star-btn.active {
  color: #ffc107;
  text-shadow: 0 0 10px rgba(255, 193, 7, 0.5);
}

.star-btn.inactive {
  color: #dee2e6;
}

.rating-text {
  margin-top: 1rem;
}

.submit-button {
  background: linear-gradient(135deg, #5e8c64, #5e8c64);
  border: none;
  padding: 1rem 2rem;
  border-radius: 50px;
  font-weight: 600;
  font-size: 1.1rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.submit-button:hover:not(:disabled) {
  transform: translateY(-3px);
  box-shadow: 0 15px 30px rgba(116, 185, 255, 0.4);
}

.submit-button::before {
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
  transition: left 0.6s ease;
}

.submit-button:hover::before {
  left: 100%;
}

.recent-reviews h3 {
  font-size: 2.5rem;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
}

.review-item {
  border-radius: 16px;
  backdrop-filter: blur(10px);
  background: rgba(255, 255, 255, 0.95);
  animation: slideInUp 0.6s ease forwards;
  opacity: 0;
  transform: translateY(20px);
  transition: all 0.3s ease;
}

.review-item:hover {
  transform: translateY(-5px);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1) !important;
}

@keyframes slideInUp {
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.star-display {
  font-size: 1.2rem;
}

.review-content {
  color: #4a5568;
  line-height: 1.6;
  font-size: 1rem;
}

.review-author {
  color: #718096;
  font-size: 0.95rem;
}

/* Responsive */
@media (max-width: 768px) {
  .hero-title {
    font-size: 2.5rem;
  }

  .hero-subtitle {
    font-size: 1.1rem;
  }

  .star-btn {
    font-size: 2rem;
  }

  .recent-reviews h3 {
    font-size: 2rem;
  }
}

/* Bootstrap Custom Colors */
.btn-primary {
  background: linear-gradient(135deg, #4ebe76, #2ba255);
  border-color: #74b9ff;
}

.btn-primary:hover {
  background: linear-gradient(135deg, #5e8c64, #5e8c64);
  border-color: #0984e3;
}

.text-primary {
  color: #74b9ff !important;
}

.bg-primary {
  background: linear-gradient(135deg, #74b9ff, #0984e3) !important;
}
</style>
