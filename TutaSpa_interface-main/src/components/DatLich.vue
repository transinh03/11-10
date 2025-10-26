<template>
  <div class="dat-lich-page">
    <!-- Hero Section -->
    <!-- <section class="hero-section">
      <div class="container">
        <div class="hero-content">
          <h1 class="hero-title">Đặt lịch hẹn</h1>
          <p class="hero-subtitle">
            Chọn dịch vụ và thời gian phù hợp để trải nghiệm không gian thư giãn tuyệt vời
          </p>
        </div>
      </div>
    </section> -->

    <div class="container main-content">
      <div class="booking-layout">
        <!-- Left Side - Service Selection -->
        <div class="services-section">
          <div class="section-header">
            <h2>Chọn dịch vụ</h2>
            <p>Tìm kiếm và chọn các dịch vụ bạn muốn sử dụng</p>
          </div>

          <!-- Search and Filter -->
          <div class="search-filter-section">
            <div class="search-box">
              <input
                type="text"
                v-model="searchKeyword"
                placeholder="Tìm kiếm dịch vụ..."
                class="search-input"
                @input="debounceSearch"
              />
              <i class="search-icon">🔍</i>
            </div>

            <div class="filter-section">
              <select
                v-model="selectedCategoryId"
                @change="searchServices"
                class="category-filter"
              >
                <option value="">Tất cả danh mục</option>
                <option
                  v-for="category in categories"
                  :key="category.loaiDichVuID"
                  :value="category.loaiDichVuID"
                >
                  {{ category.tenLoai }}
                </option>
              </select>
            </div>
          </div>

          <!-- Services Grid -->
          <div v-if="servicesLoading" class="loading">
            <div class="loading-spinner"></div>
            <p>Đang tải dịch vụ...</p>
          </div>

          <div v-else-if="servicesError" class="error">
            <p>{{ servicesError }}</p>
          </div>

          <div v-else class="services-grid">
            <div
              v-for="service in services"
              :key="service.id"
              class="service-card"
              :class="{ selected: isServiceSelected(service.id) }"
            >
              <div class="service-image-container">
                <router-link
                  class="detail-service-link"
                  :to="`/DichVuChiTiet/${service.id}`"
                >
                  <img
                    :src="service.image"
                    :alt="service.name"
                    class="service-image"
                  />
                </router-link>
                <div class="service-rating-overlay">
                  <div class="service-rating">
                    <i
                      v-for="n in 5"
                      :key="n"
                      class="rating-star"
                      :class="
                        n <= Math.floor(service.rating)
                          ? 'filled'
                          : n - 0.5 <= service.rating
                          ? 'half-filled'
                          : ''
                      "
                      >★</i
                    >
                    <span class="rating-text"
                      >({{ service.rating?.toFixed(1) || "0.0" }})</span
                    >
                  </div>
                </div>
              </div>
              <div class="service-content">
                <router-link
                  class="detail-service-link"
                  :to="`/DichVuChiTiet/${service.id}`"
                >
                  <h3 class="service-title">{{ service.name }}</h3>
                  <div class="service-meta">
                    <div class="service-duration">
                      <i class="duration-icon">⏱</i>
                      {{ service.duration }} phút
                    </div>
                    <div class="service-price">{{ service.price }} VNĐ</div>
                  </div>
                  <p class="service-description">{{ service.description }}</p>
                </router-link>
                <button
                  class="service-select-btn"
                  :class="{ selected: isServiceSelected(service.id) }"
                  @click="toggleService(service)"
                >
                  <span v-if="isServiceSelected(service.id)">✓ Đã chọn</span>
                  <span v-else>+ Chọn dịch vụ</span>
                </button>
              </div>
            </div>
          </div>

          <!-- Pagination -->
          <div v-if="totalPages > 1" class="pagination">
            <button
              class="page-btn"
              :disabled="currentPage === 1"
              @click="changePage(currentPage - 1)"
            >
              ‹ Trước
            </button>

            <button
              v-for="page in visiblePages"
              :key="page"
              class="page-btn"
              :class="{ active: page === currentPage }"
              @click="changePage(page)"
            >
              {{ page }}
            </button>

            <button
              class="page-btn"
              :disabled="currentPage === totalPages"
              @click="changePage(currentPage + 1)"
            >
              Sau ›
            </button>
          </div>
        </div>

        <!-- Right Side - Booking Form -->
        <div class="booking-section">
          <div class="booking-card">
            <h2>Thông tin đặt lịch</h2>

            <!-- Selected Services - Compact Version -->
            <div class="selected-services" v-if="selectedServices.length > 0">
              <h3>Dịch vụ đã chọn ({{ selectedServices.length }})</h3>
              <div class="selected-list-compact">
                <div
                  v-for="(service, index) in selectedServices"
                  :key="service.id"
                  class="selected-item-compact"
                >
                  <div class="service-info-compact">
                    <h4>{{ service.name }}</h4>
                    <div class="service-details-compact">
                      <span class="duration">{{ service.duration }}p</span>
                      <span class="price">{{ service.price }} VNĐ</span>
                    </div>
                  </div>
                  <!-- <div class="quantity-controls-compact">
                    <input
                      type="number"
                      v-model.number="service.soLuong"
                      min="1"
                      max="10"
                      class="quantity-input-compact"
                    />
                    <button class="remove-btn-compact" @click="removeService(index)">×</button>
                  </div> -->
                </div>
              </div>
              <div class="total-info-compact">
                <div class="total-duration">{{ totalDuration }}p</div>
                <div class="total-price">
                  {{ totalPrice.toLocaleString("vi-VN") }} VNĐ
                </div>
              </div>
            </div>

            <!-- Consultation Option -->
            <div class="consultation-option">
              <label class="consult-checkbox">
                <input
                  type="checkbox"
                  v-model="bookingForm.consultAtStore"
                  @change="handleConsultAtStore"
                />
                <span class="checkmark"></span>
                Tôi muốn đến spa để nhân viên tư vấn trực tiếp
              </label>
            </div>

            <!-- Booking Form -->
            <form @submit.prevent="submitBooking" class="booking-form">
              <div class="form-group">
                <label for="phone">Số điện thoại *</label>
                <input
                  type="tel"
                  id="phone"
                  v-model="bookingForm.phone"
                  required
                  placeholder="0123 456 789"
                  class="form-input"
                />
              </div>

              <div class="form-row">
                <div class="form-group">
                  <label for="date">Ngày hẹn *</label>
                  <input
                    type="date"
                    id="date"
                    v-model="bookingForm.date"
                    :min="minDate"
                    required
                    class="form-input"
                  />
                </div>

                <div class="form-group">
                  <label for="time">Giờ hẹn *</label>
                  <select
                    id="time"
                    v-model="bookingForm.time"
                    required
                    class="form-input"
                  >
                    <option value="">-- Chọn giờ --</option>
                    <option
                      v-for="slot in availableSlots"
                      :key="slot.khungGio"
                      :value="slot.khungGio"
                    >
                      {{ slot.khungGio }}
                    </option>
                  </select>
                </div>
              </div>

              <div class="form-group">
                <label for="notes">Ghi chú</label>
                <textarea
                  id="notes"
                  v-model="bookingForm.notes"
                  rows="3"
                  placeholder="Những yêu cầu đặc biệt hoặc ghi chú khác..."
                  class="form-input"
                ></textarea>
              </div>

              <button
                type="submit"
                class="submit-btn"
                :disabled="!canSubmit || submitting"
              >
                <span v-if="submitting">Đang xử lý...</span>
                <span v-else>
                  <i class="btn-icon">📅</i>
                  Đặt lịch hẹn
                </span>
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch, computed } from "vue";
import { useRoute } from "vue-router";
import apiClient from "../utils/axiosClient";
import dayjs from "dayjs";
const route = useRoute();

// Reactive state
const services = ref([]);
const categories = ref([]);
const selectedServices = ref([]);
const servicesLoading = ref(false);
const servicesError = ref(null);
const submitting = ref(false);

// Search and pagination
const searchKeyword = ref("");
const selectedCategoryId = ref("");
const currentPage = ref(1);
const totalPages = ref(1);
const totalItems = ref(0);
const pageSize = 12;

// Booking form
const bookingForm = ref({
  phone: "",
  date: dayjs().format("YYYY-MM-DD"),
  time: "",
  notes: "",
  consultAtStore: false,
});

const availableSlots = ref([]);
const minDate = ref("");

// Base URL for images
const IMAGE_BASE_URL =
  import.meta.env.VITE_BASE_URL.replace("/api", "") + "/images/";

// Debounce timer
let searchTimer = null;

// Computed properties
const totalDuration = computed(() => {
  return selectedServices.value.reduce((total, service) => {
    return total + service.duration * service.soLuong;
  }, 0);
});

const totalPrice = computed(() => {
  return selectedServices.value.reduce((total, service) => {
    const price = parseInt(service.price.replace(/[^\d]/g, ""));
    return total + price * service.soLuong;
  }, 0);
});

const canSubmit = computed(() => {
  return (
    bookingForm.value.phone &&
    bookingForm.value.date &&
    bookingForm.value.time &&
    (selectedServices.value.length > 0 || bookingForm.value.consultAtStore)
  );
});

const visiblePages = computed(() => {
  const pages = [];
  const maxVisible = 5;
  let start = Math.max(1, currentPage.value - Math.floor(maxVisible / 2));
  let end = Math.min(totalPages.value, start + maxVisible - 1);

  if (end - start < maxVisible - 1) {
    start = Math.max(1, end - maxVisible + 1);
  }

  for (let i = start; i <= end; i++) {
    pages.push(i);
  }

  return pages;
});

// Methods
const debounceSearch = () => {
  clearTimeout(searchTimer);
  searchTimer = setTimeout(() => {
    currentPage.value = 1;
    searchServices();
  }, 500);
};

const fetchCategories = async () => {
  try {
    const response = await apiClient.get("/LoaiDichVu");
    categories.value = Array.isArray(response) ? response : response.data || [];
  } catch (err) {
    console.error("Lỗi khi tải danh mục:", err);
  }
};

const searchServices = async () => {
  try {
    servicesLoading.value = true;
    servicesError.value = null;

    const params = {
      keyword: searchKeyword.value,
      cateId: selectedCategoryId.value,
      page: currentPage.value,
    };

    const response = await apiClient.get("/DichVu/filter", { params });
    const data = response;

    // Process services data
    services.value = (data.data || data || [])
      .filter((service) => service.trangThai === 1)
      .map((service) => ({
        id: service.dichVuID,
        name: service.tenDichVu,
        category:
          categories.value.find(
            (cat) => cat.loaiDichVuID === service.loaiDichVuID
          )?.tenLoai || "Khác",
        price: service.gia.toLocaleString("vi-VN"),
        duration: service.thoiGian,
        description: service.moTa,
        rating: service.mucDanhGia || 0,
        image: `${IMAGE_BASE_URL}${service.hinhAnh}`,
      }));

    // Update pagination info
    totalItems.value = data.pagination.totalItems || services.value.length;
    totalPages.value = Math.ceil(totalItems.value / pageSize);
  } catch (err) {
    console.error("Lỗi khi tải dịch vụ:", err);
    servicesError.value = "Không thể tải danh sách dịch vụ";
  } finally {
    servicesLoading.value = false;
  }
};

const changePage = (page) => {
  currentPage.value = page;
  searchServices();
};

const isServiceSelected = (serviceId) => {
  return selectedServices.value.some((s) => s.id === serviceId);
};

const toggleService = (service) => {
  if (bookingForm.value.consultAtStore) return;

  const existingIndex = selectedServices.value.findIndex(
    (s) => s.id === service.id
  );

  if (existingIndex >= 0) {
    selectedServices.value.splice(existingIndex, 1);
  } else {
    selectedServices.value.push({
      ...service,
      soLuong: 1,
    });
  }
};

const removeService = (index) => {
  selectedServices.value.splice(index, 1);
};

const handleConsultAtStore = () => {
  if (bookingForm.value.consultAtStore) {
    selectedServices.value = [];
  }
};

const fetchSlots = async () => {
  if (!bookingForm.value.date) return;
  try {
    const res = await apiClient.get("/DatLich/slots", {
      params: { ngay: bookingForm.value.date },
    });
    availableSlots.value = res.filter((slot) => slot.conLai > 0);
  } catch (err) {
    console.error("Lỗi khi lấy giờ hẹn:", err);
    availableSlots.value = [];
  }
};

const submitBooking = async () => {
  try {
    submitting.value = true;

    const thoiGian = new Date(
      new Date(
        `${bookingForm.value.date}T${bookingForm.value.time}`
      ).getTime() +
        7 * 60 * 60 * 1000
    ).toISOString();

    const dichVus = bookingForm.value.consultAtStore
      ? []
      : selectedServices.value.map((s) => ({
          dichVuID: s.id,
          soLuong: 1,
        }));

    const payload = {
      soDienThoai: bookingForm.value.phone,
      thoiGian,
      dichVus,
      ghiChu: bookingForm.value.notes,
      datTruoc: true,
    };

    await apiClient.post("/DatLich", payload);

    alert("Đặt lịch thành công! Chúng tôi sẽ liên hệ với bạn sớm nhất.");
    fetchSlots();
    // Reset form
    bookingForm.value = {
      phone: "",
      date: dayjs().format("YYYY-MM-DD"),
      time: "",
      notes: "",
      consultAtStore: false,
    };
    selectedServices.value = [];
  } catch (err) {
    console.error("Lỗi đặt lịch:", err);
    alert("Đặt lịch thất bại! Vui lòng thử lại.");
  } finally {
    submitting.value = false;
  }
};

// Load preselected service from route
const loadPreselectedService = async (serviceId) => {
  try {
    const response = await apiClient.get(`/DichVu/${serviceId}`);
    const serviceData = response;

    if (serviceData && serviceData.trangThai === 1) {
      const service = {
        id: serviceData.dichVuID,
        name: serviceData.tenDichVu,
        category:
          categories.value.find(
            (cat) => cat.loaiDichVuID === serviceData.loaiDichVuID
          )?.tenLoai || "Khác",
        price: serviceData.gia.toLocaleString("vi-VN"),
        duration: serviceData.thoiGian,
        description: serviceData.moTa,
        rating: serviceData.mucDanhGia || 0,
        image: `${IMAGE_BASE_URL}${serviceData.hinhAnh}`,
        soLuong: 1,
      };

      selectedServices.value = [service];
    }
  } catch (err) {
    console.error("Lỗi khi tải dịch vụ được chọn:", err);
  }
};

// Watchers
watch(
  () => bookingForm.value.date,
  () => {
    fetchSlots();
    bookingForm.value.time = "";
  }
);

// Lifecycle
onMounted(async () => {
  minDate.value = dayjs().format("YYYY-MM-DD");

  await fetchCategories();

  // Check if there's a preselected service from route
  if (route.params.serviceId) {
    await loadPreselectedService(route.params.serviceId);
  }

  await searchServices();
  fetchSlots();
});
</script>

<style scoped>
/* Base styles */
.dat-lich-page {
  min-height: 100vh;
  background: linear-gradient(135deg, #f8fdf8 0%, #f0fdf4 50%, #e8f5e8 100%);
}

/* Hero Section */
.hero-section {
  background: linear-gradient(135deg, #78ba7e 0%, #6ba371 50%, #5e8c64 100%);
  color: white;
  padding: 4rem 2rem 2rem;
  text-align: center;
}

.hero-content {
  max-width: 800px;
  margin: 0 auto;
}

.hero-title {
  font-size: 3rem;
  font-family: "Lora", serif;
  margin-bottom: 1rem;
  font-weight: 700;
}

.hero-subtitle {
  font-size: 1.2rem;
  opacity: 0.9;
  line-height: 1.6;
}

/* Main Content */
.container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 1rem;
}

.main-content {
  padding: 4rem 0;
}

.booking-layout {
  display: grid;
  grid-template-columns: 1fr 400px;
  gap: 4rem;
  align-items: start;
}

/* Services Section */
.services-section {
  background: white;
  border-radius: 20px;
  padding: 2.5rem;
  box-shadow: 0 10px 30px rgba(120, 186, 126, 0.1);
}

.section-header h2 {
  font-size: 2rem;
  color: #2d4a2d;
  margin-bottom: 0.5rem;
  font-family: "Lora", serif;
}

.section-header p {
  color: #6b7280;
  margin-bottom: 2rem;
}

/* Search and Filter */
.search-filter-section {
  display: grid;
  grid-template-columns: 1fr auto;
  gap: 1rem;
  margin-bottom: 2rem;
}

.search-box {
  position: relative;
}

.search-input {
  width: 100%;
  padding: 1rem 3rem 1rem 1rem;
  border: 2px solid #e8f5e8;
  border-radius: 25px;
  font-size: 1rem;
  transition: all 0.3s ease;
}

.search-input:focus {
  outline: none;
  border-color: #78ba7e;
  box-shadow: 0 0 0 3px rgba(120, 186, 126, 0.1);
}

.search-icon {
  position: absolute;
  right: 1rem;
  top: 50%;
  transform: translateY(-50%);
  color: #6b7280;
  font-size: 1.2rem;
}

.category-filter {
  padding: 1rem;
  border: 2px solid #e8f5e8;
  border-radius: 25px;
  background: white;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.3s ease;
  min-width: 200px;
}

.category-filter:focus {
  outline: none;
  border-color: #78ba7e;
  box-shadow: 0 0 0 3px rgba(120, 186, 126, 0.1);
}

/* Services Grid */
.services-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 2rem;
  margin-bottom: 2rem;
}

.service-card {
  background: white;
  border-radius: 15px;
  overflow: hidden;
  box-shadow: 0 5px 15px rgba(120, 186, 126, 0.1);
  transition: all 0.3s ease;
  border: 2px solid transparent;
  position: relative;
}

.service-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 15px 35px rgba(120, 186, 126, 0.2);
}

.service-card.selected {
  border-color: #78ba7e;
  background: linear-gradient(135deg, #f8fdf8 0%, #f0fdf4 100%);
}

.service-card.selected::before {
  content: "✓";
  position: absolute;
  top: 10px;
  left: 10px;
  background: #78ba7e;
  color: white;
  width: 30px;
  height: 30px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  z-index: 2;
}

.service-image-container {
  position: relative;
  overflow: hidden;
}
.detail-service-link {
  text-decoration: none;
}
.service-image {
  width: 100%;
  height: 180px;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.service-card:hover .service-image {
  transform: scale(1.05);
}

.service-rating-overlay {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: rgba(255, 255, 255, 0.9);
  padding: 0.4rem 0.6rem;
  border-radius: 15px;
  backdrop-filter: blur(10px);
}

.service-rating {
  display: flex;
  align-items: center;
  gap: 0.2rem;
  font-size: 0.8rem;
}

.rating-star {
  color: #d1d5db;
  font-size: 0.9rem;
}

.rating-star.filled {
  color: #fbbf24;
}

.rating-star.half-filled {
  background: linear-gradient(90deg, #fbbf24 50%, #d1d5db 50%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text; /* chuẩn W3C cho các trình duyệt mới */
}

.rating-text {
  font-size: 0.7rem;
  color: #6b7280;
  font-weight: 600;
}

.service-content {
  padding: 1.5rem;
}

.service-title {
  font-size: 1.2rem;
  margin-bottom: 1rem;
  color: #2d4a2d;
  font-family: "Lora", serif;
  font-weight: 600;
}

.service-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.service-duration {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  background: #f0fdf4;
  color: #4a6741;
  padding: 0.3rem 0.8rem;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: 500;
}

.service-price {
  font-size: 1.1rem;
  font-weight: bold;
  color: #f59e0b;
  font-family: "Lora", serif;
}

.service-description {
  color: #6b7280;
  margin-bottom: 1rem;
  line-height: 1.5;
  font-size: 0.9rem;
}

.service-select-btn {
  width: 100%;
  padding: 0.8rem;
  border: 2px solid #78ba7e;
  background: white;
  color: #78ba7e;
  border-radius: 20px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s ease;
  font-size: 0.9rem;
}

.service-select-btn:hover {
  background: #78ba7e;
  color: white;
  transform: translateY(-2px);
}

.service-select-btn.selected {
  background: #78ba7e;
  color: white;
}

/* Pagination */
.pagination {
  display: flex;
  justify-content: center;
  gap: 0.5rem;
  margin-top: 2rem;
}

.page-btn {
  padding: 0.6rem 1rem;
  border: 2px solid #e8f5e8;
  background: white;
  color: #6b7280;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-weight: 500;
}

.page-btn:hover:not(:disabled) {
  border-color: #78ba7e;
  color: #78ba7e;
}

.page-btn.active {
  background: #78ba7e;
  color: white;
  border-color: #78ba7e;
}

.page-btn:disabled {
  background: #f3f4f6;
  color: #d1d5db;
  cursor: not-allowed;
}

.booking-section {
  position: sticky;
  top: 2rem;
  height: fit-content;
}

.booking-card {
  background: linear-gradient(135deg, #78ba7e 0%, #6ba371 50%, #5e8c64 100%);
  color: white;
  padding: 2rem;
  border-radius: 20px;
  box-shadow: 0 15px 40px rgba(120, 186, 126, 0.3);
}

.booking-card h2 {
  font-size: 1.5rem;
  margin-bottom: 1.5rem;
  font-family: "Lora", serif;
  text-align: center;
}

/* Selected Services - Compact Version */
.selected-services {
  background: rgba(255, 255, 255, 0.15);
  padding: 1rem;
  border-radius: 12px;
  margin-bottom: 1.5rem;
  backdrop-filter: blur(10px);
}

.selected-services h3 {
  font-size: 1rem;
  margin-bottom: 0.8rem;
  font-weight: 600;
}

.selected-list-compact {
  max-height: 150px;
  overflow-y: auto;
  margin-bottom: 0.8rem;
  scrollbar-width: thin;
  scrollbar-color: rgba(255, 255, 255, 0.3) transparent;
}

.selected-list-compact::-webkit-scrollbar {
  width: 4px;
}

.selected-list-compact::-webkit-scrollbar-track {
  background: transparent;
}

.selected-list-compact::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.3);
  border-radius: 2px;
}

.selected-item-compact {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: rgba(255, 255, 255, 0.1);
  padding: 0.8rem;
  border-radius: 8px;
  margin-bottom: 0.5rem;
  gap: 0.8rem;
}

.service-info-compact {
  flex: 1;
  min-width: 0;
}

.service-info-compact h4 {
  font-size: 0.9rem;
  margin-bottom: 0.3rem;
  font-weight: 600;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.service-details-compact {
  display: flex;
  gap: 0.8rem;
  font-size: 0.8rem;
  opacity: 0.9;
}

.quantity-controls-compact {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  flex-shrink: 0;
}

.quantity-input-compact {
  width: 40px;
  padding: 0.2rem;
  border: none;
  border-radius: 4px;
  text-align: center;
  font-size: 0.8rem;
  color: #2d4a2d;
}

.remove-btn-compact {
  background: #ef4444;
  color: white;
  border: none;
  border-radius: 50%;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  font-size: 0.9rem;
  transition: all 0.3s ease;
}

.remove-btn-compact:hover {
  background: #dc2626;
  transform: scale(1.1);
}

.total-info-compact {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 0.8rem;
  border-top: 1px solid rgba(255, 255, 255, 0.2);
  font-size: 0.9rem;
}

.total-duration {
  color: rgba(255, 255, 255, 0.9);
  font-weight: 600;
}

.total-price {
  color: #fbbf24;
  font-size: 1rem;
  font-weight: 700;
}

/* Consultation Option */
.consultation-option {
  margin-bottom: 1.5rem;
}

.consult-checkbox {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  cursor: pointer;
  font-size: 0.9rem;
  line-height: 1.3;
}

.consult-checkbox input[type="checkbox"] {
  width: auto;
  margin: 0;
  transform: scale(1.1);
}

/* Booking Form */
.booking-form {
  margin-top: 0.5rem;
}

.form-group {
  margin-bottom: 1.2rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  font-size: 0.9rem;
}

.form-input {
  width: 100%;
  padding: 0.8rem;
  border: none;
  border-radius: 8px;
  font-size: 0.95rem;
  background: rgba(255, 255, 255, 0.9);
  color: #2d4a2d;
  transition: all 0.3s ease;
}

.form-input:focus {
  outline: none;
  background: white;
  transform: scale(1.01);
  box-shadow: 0 3px 10px rgba(0, 0, 0, 0.1);
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.8rem;
}

.submit-btn {
  width: 100%;
  background: linear-gradient(45deg, #f59e0b, #f97316);
  color: white;
  padding: 1rem 1.5rem;
  border: none;
  border-radius: 25px;
  font-size: 1rem;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-top: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.submit-btn:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 8px 20px rgba(245, 158, 11, 0.4);
}

.submit-btn:disabled {
  background: #9ca3af;
  cursor: not-allowed;
  transform: none;
}

.btn-icon {
  font-size: 1.1rem;
}

/* Loading and Error States */
.loading {
  text-align: center;
  padding: 2rem;
  color: #6b7280;
}

.loading-spinner {
  border: 3px solid #f3f4f6;
  border-top: 3px solid #78ba7e;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
  margin: 0 auto 1rem;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

.error {
  text-align: center;
  padding: 1.5rem;
  color: #ef4444;
  background: #fef2f2;
  border-radius: 10px;
  border: 1px solid #fecaca;
}

/* Responsive Design */
@media (max-width: 1200px) {
  .booking-layout {
    grid-template-columns: 1fr 350px;
    gap: 3rem;
  }
}

@media (max-width: 992px) {
  .booking-layout {
    grid-template-columns: 1fr;
    gap: 3rem;
  }

  .booking-section {
    position: static;
  }

  .services-grid {
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  }

  .hero-title {
    font-size: 2.5rem;
  }

  .hero-subtitle {
    font-size: 1.1rem;
  }
}

@media (max-width: 768px) {
  .main-content {
    padding: 2rem 0;
  }

  .hero-section {
    padding: 3rem 1rem 1.5rem;
  }

  .hero-title {
    font-size: 2rem;
  }

  .services-section {
    padding: 2rem;
  }

  .booking-card {
    padding: 2rem;
  }

  .search-filter-section {
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .category-filter {
    min-width: auto;
  }

  .services-grid {
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
    gap: 1.5rem;
  }

  .form-row {
    grid-template-columns: 1fr;
  }

  .selected-item {
    flex-direction: column;
    align-items: stretch;
    gap: 0.8rem;
  }

  .quantity-controls {
    justify-content: space-between;
  }

  .total-info {
    flex-direction: column;
    gap: 0.5rem;
    align-items: stretch;
    text-align: center;
  }
}

@media (max-width: 576px) {
  .hero-title {
    font-size: 1.8rem;
  }

  .hero-subtitle {
    font-size: 1rem;
  }

  .services-section {
    padding: 1.5rem;
  }

  .booking-card {
    padding: 1.5rem;
  }

  .services-grid {
    grid-template-columns: 1fr;
  }

  .service-card {
    margin-bottom: 1rem;
  }

  .service-image {
    height: 150px;
  }

  .service-content {
    padding: 1.2rem;
  }

  .service-title {
    font-size: 1.1rem;
  }

  .service-meta {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
  }

  .booking-card h2 {
    font-size: 1.5rem;
  }

  .selected-services {
    padding: 1.2rem;
  }

  .selected-item {
    padding: 0.8rem;
  }

  .service-info h4 {
    font-size: 0.9rem;
  }

  .service-details {
    flex-direction: column;
    gap: 0.3rem;
  }

  .pagination {
    flex-wrap: wrap;
    gap: 0.3rem;
  }

  .page-btn {
    padding: 0.5rem 0.8rem;
    font-size: 0.9rem;
  }

  .consult-checkbox {
    font-size: 0.9rem;
  }

  .form-input {
    padding: 0.8rem;
  }

  .submit-btn {
    padding: 1rem 1.5rem;
    font-size: 1rem;
  }
}

/* Additional utility classes */
.text-center {
  text-align: center;
}

.mb-0 {
  margin-bottom: 0;
}
.mb-1 {
  margin-bottom: 0.5rem;
}
.mb-2 {
  margin-bottom: 1rem;
}
.mb-3 {
  margin-bottom: 1.5rem;
}

.font-bold {
  font-weight: 700;
}

.font-semibold {
  font-weight: 600;
}

/* Custom scrollbar for selected services */
.selected-list {
  max-height: 300px;
  overflow-y: auto;
  scrollbar-width: thin;
  scrollbar-color: rgba(255, 255, 255, 0.3) transparent;
}

.selected-list::-webkit-scrollbar {
  width: 6px;
}

.selected-list::-webkit-scrollbar-track {
  background: transparent;
}

.selected-list::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.3);
  border-radius: 3px;
}

.selected-list::-webkit-scrollbar-thumb:hover {
  background: rgba(255, 255, 255, 0.5);
}
</style>
