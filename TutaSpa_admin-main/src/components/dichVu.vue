<template>
  <div class="container">
    <div class="dich-vu-management">
      <!-- Tiêu đề trang -->
      <div class="header">
        <h1 class="title">
          <i class="fa fa-list-alt"></i>
          Quản lý Dịch Vụ
        </h1>
      </div>
      <!-- Bộ lọc -->
      <div class="card filter-section">
        <div class="card-header">
          <h2 class="section-title">
            <i class="fa fa-filter"></i>
            Bộ lọc và tìm kiếm
          </h2>
          <button @click="resetFilters" class="btn btn-outline-light">
            <i class="fas fa-sync-alt"></i>
            Làm mới
          </button>
        </div>
        <div class="card-body">
          <div class="filter-grid">
            <!-- Tìm kiếm theo tên -->
            <div class="filter-group">
              <label class="filter-label">
                <i class="fa-solid fa-magnifying-glass me-1" style="color:#e83e8c;"></i> Tìm theo tên
              </label>
              <div class="search-container">
                <input v-model="searchName" type="text" class="search-input" placeholder="Nhập tên dịch vụ..." />
                <button v-if="searchName" @click="clearSearch" class="clear-search" title="Xóa tìm kiếm">
                  <i class="fas fa-times"></i>
                </button>
                <i class="fa fa-search search-icon"></i>
              </div>
            </div>

            <!-- Lọc theo giá -->
            <div class="filter-group price-filter">
              <label class="filter-label">
                <i class="fa-solid fa-sliders me-1"></i> Lọc giá
              </label>
              <div class="price-inputs">
                <input v-model.number="priceMin" type="number" class="form-control" placeholder="Từ" min="0" />
                <input v-model.number="priceMax" type="number" class="form-control" placeholder="Đến" min="0" />
              </div>
            </div>

            <!-- Lọc theo loại dịch vụ -->
            <div class="filter-group">
              <label class="filter-label">Loại dịch vụ</label>
              <select v-model="selectedLoaiDichVu" class="form-control">
                <option value="">Tất cả loại</option>
                <option v-for="ldv in loaiDichVus" :key="ldv.loaiDichVuID" :value="ldv.loaiDichVuID">
                  {{ ldv.tenLoai }}
                </option>
              </select>
            </div>

            <!-- Lọc theo trạng thái -->
            <div class="filter-group">
              <label class="filter-label">Trạng thái</label>
              <select v-model="selectedTrangThai" class="form-control">
                <option value="">Tất cả</option>
                <option value="1">Hoạt động</option>
                <option value="0">Tạm ngừng</option>
              </select>
            </div>

            <!-- Lọc theo thời gian -->
            <div class="filter-group">
              <label class="filter-label">
                <i class="fa-solid fa-clock me-1"></i> Thời gian (phút)
              </label>
              <div class="time-inputs">
                <input v-model.number="timeMin" type="number" class="form-control" placeholder="Từ" min="0" />
                <input v-model.number="timeMax" type="number" class="form-control" placeholder="Đến" min="0" />
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="content-grid">
        <!-- Form -->
        <div class="form-section">
          <div class="card">
            <div class="card-header">
              <h2 class="section-title">
                <i class="fa fa-plus-circle"></i>
                {{ isEditing ? "Cập nhật Dịch vụ" : "Thêm Dịch vụ mới" }}
              </h2>
            </div>
            <div class="card-body">
              <form @submit.prevent="saveDichVu" class="service-form">
                <div class="form-grid">
                  <div class="form-group">
                    <label class="form-label">Tên dịch vụ <span class="required">*</span></label>
                    <input v-model="dichVu.tenDichVu" class="form-control" required />
                  </div>
                  <div class="form-group">
                    <label class="form-label">Giá (VND) <span class="required">*</span></label>
                    <input v-model.number="dichVu.gia" type="number" class="form-control" min="0" required />
                  </div>
                  <div class="form-group">
                    <label class="form-label">Thời gian (phút) <span class="required">*</span></label>
                    <input v-model.number="dichVu.thoiGian" type="number" class="form-control" min="0" required />
                  </div>
                  <div class="form-group">
                    <label class="form-label">Hình ảnh</label>
                    <input type="file" class="form-control" @change="handleFileChange" accept="image/*" />
                  </div>
                  <div class="form-group full-width">
                    <label class="form-label">Mô tả</label>
                    <textarea v-model="dichVu.moTa" class="form-control" rows="3"></textarea>
                  </div>
                  <div class="form-group">
                    <label class="form-label">Loại dịch vụ <span class="required">*</span></label>
                    <select v-model.number="dichVu.loaiDichVuID" class="form-control" required>
                      <option disabled value="">-- Chọn loại dịch vụ --</option>
                      <option v-for="ldv in loaiDichVus" :key="ldv.loaiDichVuID" :value="ldv.loaiDichVuID">
                        {{ ldv.tenLoai }}
                      </option>
                    </select>
                  </div>
                  <div class="form-group">
                    <label class="form-label">Trạng thái</label>
                    <select v-model.number="dichVu.trangThai" class="form-control">
                      <option :value="1">Hoạt động</option>
                      <option :value="0">Tạm ngừng</option>
                    </select>
                  </div>
                </div>
                <div class="form-actions">
                  <button type="submit" class="btn btn-primary" :disabled="loading">
                    <i class="fa fa-save"></i>
                    {{ loading ? 'Đang lưu...' : (isEditing ? "Cập nhật" : "Thêm") }}
                  </button>
                  <button type="button" class="btn btn-danger" @click="resetForm">
                    <i class="fa fa-undo"></i> Hủy
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>

        <!-- Danh sách -->
        <div class="list-section">
          <div class="card">
            <div class="card-header">
              <h2 class="section-title">
                <i class="fa fa-table"></i>
                Danh sách Dịch vụ ({{ filteredDichVus.length }} kết quả)
              </h2>
            </div>
            <div class="card-body">
              <!-- Loading State -->
              <div v-if="loading" class="loading-state">
                <i class="fas fa-spinner fa-spin"></i>
                Đang tải...
              </div>

              <!-- Empty State -->
              <div v-else-if="!filteredDichVus.length" class="empty-state">
                <i class="fas fa-concierge-bell"></i>
                <p>{{ dichVus.length ? 'Không tìm thấy dịch vụ phù hợp với bộ lọc' : 'Không có dịch vụ nào' }}</p>
              </div>

              <div v-else class="table-responsive">
                <table class="services-table">
                  <thead>
                    <tr>
                      <th>Tên</th>
                      <th>Giá</th>
                      <th>Thời gian</th>
                      <th>Ảnh</th>
                      <th>Ngày tạo</th>
                      <th>Loại</th>
                      <th>Trạng thái</th>
                      <th>Hành động</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="dv in paginatedDichVus" :key="dv.dichVuID" class="service-row">
                      <td class="service-name">{{ dv.tenDichVu }}</td>
                      <td class="service-price">{{ formatCurrency(dv.gia) }}</td>
                      <td class="service-time">{{ dv.thoiGian }} phút</td>
                      <td class="service-image">
                        <img :src="imageUrl + dv.hinhAnh" class="service-img" alt="Service image" />
                      </td>
                      <td class="service-date">{{ formatDate(dv.ngayTao) }}</td>
                      <td class="service-type">
                        <span class="type-badge">{{ dv.tenLoai }}</span>
                      </td>
                      <td class="service-status">
                        <span class="status-badge" :class="dv.trangThai === 1 ? 'active' : 'inactive'">
                          {{ dv.trangThai === 1 ? 'Hoạt động' : 'Tạm ngừng' }}
                        </span>
                      </td>
                      <td class="service-actions">
                        <div class="action-buttons">
                          <button class="btn btn-sm btn-info" @click="editDichVu(dv)" title="Chỉnh sửa">
                            <i class="fa fa-edit"></i>
                          </button>
                          <button class="btn btn-sm" :class="dv.trangThai === 1 ? 'btn-warning' : 'btn-success'"
                            @click="toggleTrangThai(dv)" :title="dv.trangThai === 1 ? 'Tạm ngừng' : 'Kích hoạt'">
                            <i class="fa" :class="dv.trangThai === 1 ? 'fa-pause' : 'fa-play'"></i>
                          </button>
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>

              <!-- Phân trang -->
              <div v-if="filteredDichVus.length > 0" class="pagination-container">
                <button class="btn btn-outline-primary" :disabled="currentPage === 1"
                  @click="goToPage(currentPage - 1)">
                  <i class="fa fa-angle-left"></i>
                  Trước
                </button>
                <span class="page-info">Trang {{ currentPage }} / {{ totalPages }}</span>
                <div class="page-numbers">
                  <button v-for="page in visiblePages" :key="page" class="btn btn-sm page-btn"
                    :class="page === currentPage ? 'btn-primary' : 'btn-outline-secondary'" @click="goToPage(page)">
                    {{ page }}
                  </button>
                </div>
                <button class="btn btn-outline-primary" :disabled="currentPage === totalPages"
                  @click="goToPage(currentPage + 1)">
                  Sau
                  <i class="fa fa-angle-right"></i>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Toast Notifications -->
      <div class="toast-container">
        <div v-for="toast in toasts" :key="toast.id" class="toast" :class="toast.type">
          <i class="fas" :class="getToastIcon(toast.type)"></i>
          {{ toast.message }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue";
import apiClient from "../utils/axiosClient";
import Swal from "sweetalert2";

const imageUrl = import.meta.env.VITE_BASE_URL.replace("/api", "/images/");
const dichVus = ref([]);
const loaiDichVus = ref([]);
const loading = ref(false);
const toasts = ref([]);

// Search and filter states
const searchName = ref("");
const priceMin = ref("");
const priceMax = ref("");
const selectedLoaiDichVu = ref("");
const selectedTrangThai = ref("");
const timeMin = ref("");
const timeMax = ref("");

const dichVu = ref({
  dichVuID: 0,
  tenDichVu: "",
  gia: 0,
  thoiGian: 0,
  moTa: "",
  hinhAnh: "",
  ngayTao: new Date().toISOString(),
  trangThai: 1,
  loaiDichVuID: 1,
});

const selectedImage = ref(null);
const isEditing = ref(false);

// Pagination
const currentPage = ref(1);
const pageSize = ref(5);

// Toast methods
const showToast = (message, type = 'info') => {
  const toast = {
    id: Date.now(),
    message,
    type
  }
  toasts.value.push(toast)
  setTimeout(() => {
    const index = toasts.value.findIndex(t => t.id === toast.id)
    if (index > -1) toasts.value.splice(index, 1)
  }, 3000)
}

const getToastIcon = (type) => {
  switch (type) {
    case 'success': return 'fa-check-circle'
    case 'error': return 'fa-exclamation-circle'
    case 'warning': return 'fa-exclamation-triangle'
    default: return 'fa-info-circle'
  }
}

// Computed properties for filtering
const filteredDichVus = computed(() => {
  let filtered = [...dichVus.value];

  // Filter by name
  if (searchName.value.trim()) {
    filtered = filtered.filter(dv =>
      dv.tenDichVu.toLowerCase().includes(searchName.value.trim().toLowerCase())
    );
  }

  // Filter by price range
  const min = Number(priceMin.value);
  const max = Number(priceMax.value);

  if (!isNaN(min) && priceMin.value !== "" && (isNaN(max) || priceMax.value === "")) {
    filtered = filtered.filter(dv => dv.gia >= min);
  }

  if (!isNaN(max) && priceMax.value !== "" && (isNaN(min) || priceMin.value === "")) {
    filtered = filtered.filter(dv => dv.gia <= max);
  }

  if (!isNaN(min) && priceMin.value !== "" && !isNaN(max) && priceMax.value !== "") {
    filtered = filtered.filter(dv => dv.gia >= min && dv.gia <= max);
  }

  // Filter by service type
  if (selectedLoaiDichVu.value !== "") {
    filtered = filtered.filter(dv => dv.loaiDichVuID === Number(selectedLoaiDichVu.value));
  }

  // Filter by status
  if (selectedTrangThai.value !== "") {
    filtered = filtered.filter(dv => dv.trangThai === Number(selectedTrangThai.value));
  }

  // Filter by time range
  const timeMinVal = Number(timeMin.value);
  const timeMaxVal = Number(timeMax.value);

  if (!isNaN(timeMinVal) && timeMin.value !== "" && (isNaN(timeMaxVal) || timeMax.value === "")) {
    filtered = filtered.filter(dv => dv.thoiGian >= timeMinVal);
  }

  if (!isNaN(timeMaxVal) && timeMax.value !== "" && (isNaN(timeMinVal) || timeMin.value === "")) {
    filtered = filtered.filter(dv => dv.thoiGian <= timeMaxVal);
  }

  if (!isNaN(timeMinVal) && timeMin.value !== "" && !isNaN(timeMaxVal) && timeMax.value !== "") {
    filtered = filtered.filter(dv => dv.thoiGian >= timeMinVal && dv.thoiGian <= timeMaxVal);
  }

  return filtered;
});

const totalPages = computed(() => Math.ceil(filteredDichVus.value.length / pageSize.value));

const paginatedDichVus = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  return filteredDichVus.value.slice(start, start + pageSize.value);
});

const visiblePages = computed(() => {
  const total = totalPages.value;
  const current = currentPage.value;
  const delta = 2;

  let pages = [];
  for (let i = Math.max(1, current - delta); i <= Math.min(total, current + delta); i++) {
    pages.push(i);
  }

  if (pages[0] > 1) {
    if (pages[0] > 2) pages.unshift('...');
    pages.unshift(1);
  }

  if (pages[pages.length - 1] < total) {
    if (pages[pages.length - 1] < total - 1) pages.push('...');
    pages.push(total);
  }

  return pages.filter(page => page !== '...' || pages.indexOf(page) === pages.lastIndexOf(page));
});

// Filter methods
const clearSearch = () => {
  searchName.value = '';
}

const resetFilters = () => {
  searchName.value = "";
  priceMin.value = "";
  priceMax.value = "";
  selectedLoaiDichVu.value = "";
  selectedTrangThai.value = "";
  timeMin.value = "";
  timeMax.value = "";
  currentPage.value = 1;
}

// Utility methods
const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(value);
}

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('vi-VN');
}

const handleFileChange = (e) => {
  selectedImage.value = e.target.files[0];
};

const goToPage = (page) => {
  if (page >= 1 && page <= totalPages.value && page !== '...') {
    currentPage.value = page;
  }
};

// API methods
const fetchDichVus = async () => {
  try {
    loading.value = true;
    const data = await apiClient.get("/DichVu/all");
    dichVus.value = data;
    showToast('Tải danh sách dịch vụ thành công', 'success');
  } catch (error) {
    console.error("Lỗi lấy danh sách dịch vụ:", error);
    showToast('Lỗi khi tải danh sách dịch vụ', 'error');
  } finally {
    loading.value = false;
  }
};

const fetchLoaiDichVus = async () => {
  try {
    const data = await apiClient.get("/DichVu/loai");
    loaiDichVus.value = data;
  } catch (error) {
    console.error("Lỗi lấy loại dịch vụ:", error);
    showToast('Lỗi khi tải loại dịch vụ', 'error');
  }
};

const saveDichVu = async () => {
  let hasError = false;

  // --- Validation cơ bản ---
  if (!dichVu.value.tenDichVu || !dichVu.value.tenDichVu.trim()) {
    showToast("Tên dịch vụ không được để trống", "warning");
    return;
  }
  if (/\d/.test(dichVu.value.tenDichVu)) {
    showToast("Tên dịch vụ không được chứa số", "warning");
    return;
  }
  if (dichVu.value.gia == null || dichVu.value.gia <= 0) {
    showToast("Giá dịch vụ phải lớn hơn 0", "warning");
    return;
  }
  if (dichVu.value.thoiGian == null || dichVu.value.thoiGian <= 0) {
    showToast("Thời gian phải lớn hơn 0", "warning");
    return;
  }
  if ((!dichVu.value.hinhAnh || !dichVu.value.hinhAnh.trim()) && !selectedImage.value) {
    showToast("Hình ảnh không được để trống", "warning");
    return;
  }

  try {
    loading.value = true;

    // --- 🔎 Kiểm tra trùng tên theo loại ---
    const checkRes = await apiClient.get("/DichVu"); // hoặc làm API riêng: /DichVu/check?ten=xxx
    const allDV = checkRes || [];
    const tenNormalized = dichVu.value.tenDichVu.trim().toLowerCase();

    const existedDV = allDV.find(
      x =>
        x.tenDichVu.trim().toLowerCase() === tenNormalized &&
        x.dichVuID !== dichVu.value.dichVuID
    );

    if (existedDV) {
      if (existedDV.loaiDichVuID !== dichVu.value.loaiDichVuID) {
        showToast(`Dịch vụ '${dichVu.value.tenDichVu}' đã tồn tại trong loại khác, vui lòng chọn đúng loại dịch vụ.`, "warning");
        return;
      } else {
        showToast("Tên dịch vụ đã tồn tại trong loại này.", "warning");
        return;
      }
    }

    // --- Upload ảnh nếu có ---
    let imageName = dichVu.value.hinhAnh;
    if (selectedImage.value) {
      const formData = new FormData();
      formData.append("file", selectedImage.value);
      formData.append("tenDichVu", dichVu.value.tenDichVu);

      const uploadRes = await apiClient.post("/DichVu/image", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      imageName = uploadRes.fileName || uploadRes;
    }

    // --- Submit ---
    const payload = { ...dichVu.value, hinhAnh: imageName };

    if (isEditing.value) {
      await apiClient.put(`/DichVu/${payload.dichVuID}`, payload);
      showToast("Cập nhật dịch vụ thành công!", "success");
    } else {
      await apiClient.post("/DichVu", payload);
      showToast("Thêm dịch vụ thành công!", "success");
    }

    resetForm();
    selectedImage.value = null;
    await fetchDichVus();
  } catch (error) {
    console.error("Lỗi lưu dịch vụ:", error);
    showToast("Có lỗi khi lưu dịch vụ!", "error");
  } finally {
    loading.value = false;
  }
};
const editDichVu = (dv) => {
  dichVu.value = { ...dv };
  isEditing.value = true;
  // Scroll to form
  document.querySelector('.form-section').scrollIntoView({ behavior: 'smooth' });
};

const resetForm = () => {
  isEditing.value = false;
  dichVu.value = {
    dichVuID: 0,
    tenDichVu: "",
    gia: 0,
    thoiGian: 0,
    moTa: "",
    hinhAnh: "",
    ngayTao: new Date().toISOString(),
    trangThai: 1,
    loaiDichVuID: 1,
  };
  selectedImage.value = null;
};

const toggleTrangThai = async (dv) => {
  const action = dv.trangThai === 1 ? 'tạm ngừng' : 'kích hoạt';

  const result = await Swal.fire({
    title: `Xác nhận ${action}`,
    text: `Bạn có chắc chắn muốn ${action} dịch vụ "${dv.tenDichVu}"?`,
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#3085d6',
    cancelButtonColor: '#d33',
    confirmButtonText: `Có, ${action}!`,
    cancelButtonText: 'Hủy'
  });

  if (result.isConfirmed) {
    try {
      const updatedDv = { ...dv, trangThai: dv.trangThai === 1 ? 0 : 1 };
      await apiClient.put(`/DichVu/${dv.dichVuID}`, updatedDv);
      await fetchDichVus();
      showToast(`${action.charAt(0).toUpperCase() + action.slice(1)} dịch vụ thành công!`, 'success');
    } catch (error) {
      console.error("Lỗi cập nhật trạng thái:", error);
      showToast("Lỗi khi cập nhật trạng thái", 'error');
    }
  }
};

// Watchers for real-time filtering
watch([searchName, priceMin, priceMax, selectedLoaiDichVu, selectedTrangThai, timeMin, timeMax], () => {
  currentPage.value = 1; // Reset to first page when filters change
});

// Lifecycle
onMounted(() => {
  fetchDichVus();
  fetchLoaiDichVus();
});
</script>

<style scoped>
/* ===== BASE STYLES ===== */
.dich-vu-management {
  max-width: 1400px;
  margin: 0 auto;
  padding: 20px;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.header {
  margin-bottom: 30px;
}

.title {
  color: #2c3e50;
  font-size: 2.5rem;
  font-weight: 600;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 15px;
}

.title i {
  color: #3498db;
}

/* ===== CARD COMPONENTS ===== */
.card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.07);
  margin-bottom: 30px;
  overflow: hidden;
  border: 1px solid #e1e8ed;
}

.card-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 20px 25px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.section-title {
  margin: 0;
  font-size: 1.4rem;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 10px;
}

.card-body {
  padding: 25px;
}

/* ===== FILTER SECTION ===== */
.filter-section {
  margin-bottom: 30px;
  width: 113%;
}

.filter-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
  align-items: end;
}

.filter-group {
  display: flex;
  flex-direction: column;
}

.filter-label {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 8px;
  font-size: 0.95rem;
  display: flex;
  align-items: center;
  gap: 5px;
}

.search-container {
  position: relative;
}

.search-input {
  width: 100%;
  padding: 12px 45px 12px 15px;
  border: 2px solid #e1e8ed;
  border-radius: 25px;
  font-size: 1rem;
  background: rgba(255, 255, 255, 0.9);
  color: #2c3e50;
  transition: all 0.3s ease;
}

.search-input:focus {
  outline: none;
  border-color: #3498db;
  background: white;
  box-shadow: 0 0 0 3px rgba(52, 152, 219, 0.1);
}

.search-icon {
  position: absolute;
  right: 15px;
  top: 50%;
  transform: translateY(-50%);
  color: #7f8c8d;
  font-size: 1.1rem;
}

.clear-search {
  position: absolute;
  right: 40px;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  color: #7f8c8d;
  cursor: pointer;
  padding: 5px;
  border-radius: 50%;
  transition: all 0.3s ease;
}

.clear-search:hover {
  background: rgba(108, 117, 125, 0.2);
  color: #495057;
}

.price-inputs,
.time-inputs {
  display: flex;
  gap: 10px;
  align-items: center;
}

/* ===== STATES ===== */
.loading-state,
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  color: #6c757d;
  font-size: 1.1rem;
}

.loading-state i {
  font-size: 2rem;
  margin-bottom: 15px;
  color: #007bff;
}

.empty-state i {
  font-size: 3rem;
  margin-bottom: 20px;
  color: #dee2e6;
}

.empty-state p {
  margin: 0;
  font-weight: 500;
  text-align: center;
}

/* ===== CONTENT GRID ===== */
.content-grid {
  display: grid;
  grid-template-columns: 1fr 2fr;
  gap: 30px;
}

/* ===== FORM STYLES ===== */
.service-form .form-control {
  padding: 12px 14px;
  border: 2px solid #e1e8ed;
  border-radius: 8px;
  font-size: 0.95rem;
  height: 44px;
  /* fix chiều cao */
  box-sizing: border-box;
  transition: border-color 0.3s, box-shadow 0.3s;
}

.service-form .form-control:focus {
  border-color: #3498db;
  box-shadow: 0 0 0 3px rgba(52, 152, 219, 0.15);
  outline: none;
}

/* Textarea cao hơn nhưng vẫn cùng style */
.service-form textarea.form-control {
  height: auto;
  min-height: 100px;
  resize: vertical;
}

/* Full width cho các trường dài */
.service-form .form-group.full-width {
  grid-column: 1 / -1;
  /* chiếm trọn hàng */
}

/* Hành động nút */
.service-form .form-actions {
  display: flex;
  gap: 15px;
  margin-top: 20px;
}

/* ===== BUTTON STYLES ===== */
.btn {
  padding: 12px 24px;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  text-decoration: none;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  position: relative;
  overflow: hidden;
}

.btn::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
  transition: left 0.6s;
}

.btn:hover:not(:disabled)::before {
  left: 100%;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none !important;
}

.btn:disabled::before {
  display: none;
}

.btn-primary {
  background: linear-gradient(135deg, #3498db, #2980b9);
  color: white;
  box-shadow: 0 4px 16px rgba(52, 152, 219, 0.3);
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(52, 152, 219, 0.4);
}

.btn-success {
  background: linear-gradient(135deg, #27ae60, #229954);
  color: white;
  box-shadow: 0 4px 16px rgba(39, 174, 96, 0.3);
}

.btn-info {
  background: linear-gradient(135deg, #17a2b8, #138496);
  color: white;
}

.btn-info:hover:not(:disabled) {
  background: linear-gradient(135deg, #138496, #117a8b);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(23, 162, 184, 0.4);
}

.btn-warning {
  background: linear-gradient(135deg, #f39c12, #e67e22);
  color: white;
}

.btn-warning:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(243, 156, 18, 0.4);
}

.btn-danger {
  background: linear-gradient(135deg, #e74c3c, #c0392b);
  color: white;
}

.btn-danger:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(231, 76, 60, 0.4);
}

.btn-outline-primary {
  background: transparent;
  color: #3498db;
  border: 2px solid #3498db;
  box-shadow: none;
}

.btn-outline-primary:hover:not(:disabled) {
  background: #3498db;
  color: white;
  transform: translateY(-2px);
}

.btn-outline-secondary {
  background: transparent;
  color: #95a5a6;
  border: 2px solid #95a5a6;
}

.btn-outline-light {
  background: transparent;
  color: white;
  border: 2px solid rgba(255, 255, 255, 0.3);
}

.btn-outline-light:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.1);
  border-color: white;
}

.btn-sm {
  padding: 8px 12px;
  font-size: 0.85rem;
}

/* ===== TABLE STYLES ===== */
.table-responsive {
  overflow-x: auto;
  border-radius: 0 0 12px 12px;
}

.services-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  margin: 0;
}

.services-table thead {
  background: linear-gradient(135deg, #495057 0%, #343a40 100%);
  position: sticky;
  top: 0;
  z-index: 10;
}

.services-table thead th {
  padding: 18px 15px;
  text-align: left;
  font-weight: 600;
  font-size: 0.9rem;
  color: white;
  border: none;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  white-space: nowrap;
}

.services-table tbody tr {
  transition: all 0.3s ease;
  border-bottom: 1px solid #f1f3f4;
}

.services-table tbody tr:hover {
  background: linear-gradient(135deg, #f8f9ff 0%, #e3f2fd 100%);
  transform: translateX(3px);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.services-table td {
  padding: 16px 15px;
  vertical-align: middle;
  border: none;
  font-size: 0.95rem;
}

/* ===== TABLE CELL STYLES ===== */
.service-name {
  font-weight: 600;
  color: #2c3e50;
  text-align: left;
  min-width: 150px;
}

.service-price {
  color: #27ae60;
  font-weight: 600;
  font-size: 1.05rem;
  min-width: 120px;
}

.service-time {
  color: #7f8c8d;
  font-weight: 500;
  min-width: 80px;
}

.service-img {
  width: 90px;
  height: 70px;
  object-fit: cover;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease;
}

.service-img:hover {
  transform: scale(1.1);
}

.service-date {
  color: #7f8c8d;
  font-size: 0.9rem;
  min-width: 100px;
}

.type-badge {
  display: inline-block;
  min-width: 80px;
  text-align: center;
  background: linear-gradient(135deg, #9b59b6, #8e44ad);
  color: white;
  padding: 6px 12px;
  border-radius: 16px;
  font-size: 0.8rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  white-space: nowrap;
  box-shadow: 0 2px 4px rgba(155, 89, 182, 0.3);
}

.status-badge {
  display: inline-block;
  min-width: 100px;
  text-align: center;
  padding: 8px 14px;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  white-space: nowrap;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.status-badge.active {
  background: linear-gradient(135deg, #27ae60, #229954);
  color: white;
}

.status-badge.inactive {
  background: linear-gradient(135deg, #95a5a6, #7f8c8d);
  color: white;
}

.action-buttons {
  display: flex;
  gap: 8px;
  justify-content: center;
  align-items: center;
}

/* ===== PAGINATION STYLES ===== */
.pagination-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 30px;
  padding-top: 20px;
  border-top: 1px solid #ecf0f1;
}

.page-info {
  font-weight: 600;
  color: #2c3e50;
  font-size: 1rem;
}

.page-numbers {
  display: flex;
  gap: 5px;
}

.page-btn {
  min-width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* ===== TOAST NOTIFICATIONS ===== */
.toast-container {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 9999;
}

.toast {
  background: white;
  border-radius: 12px;
  padding: 16px 20px;
  margin-bottom: 10px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.12);
  border-left: 4px solid #007bff;
  display: flex;
  align-items: center;
  gap: 10px;
  min-width: 300px;
  animation: slideInRight 0.3s ease-out;
}

.toast.success {
  border-left-color: #28a745;
  background: linear-gradient(135deg, #d4edda 0%, #c3e6cb 100%);
  color: #155724;
}

.toast.error {
  border-left-color: #dc3545;
  background: linear-gradient(135deg, #f8d7da 0%, #f5c6cb 100%);
  color: #721c24;
}

.toast.warning {
  border-left-color: #ffc107;
  background: linear-gradient(135deg, #fff3cd 0%, #ffeaa7 100%);
  color: #856404;
}

@keyframes slideInRight {
  from {
    transform: translateX(100%);
    opacity: 0;
  }

  to {
    transform: translateX(0);
    opacity: 1;
  }
}

/* ===== ANIMATIONS ===== */
@keyframes spin {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}

.fa-spin {
  animation: spin 1s linear infinite;
}

/* ===== RESPONSIVE DESIGN ===== */
@media (max-width: 1500px) {
  .content-grid {
    grid-template-columns: 1fr;
  }

  .form-section {
    order: 1;
  }

  .list-section {
    order: 2;
  }

  .filter-grid {
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  }
}

@media (max-width: 768px) {
  .dich-vu-management {
    padding: 15px;
  }

  .title {
    font-size: 2rem;
  }

  .filter-grid {
    grid-template-columns: 1fr;
    gap: 15px;
  }

  .price-inputs,
  .time-inputs {
    flex-direction: column;
    align-items: stretch;
  }

  .form-grid {
    grid-template-columns: 1fr;
  }

  .services-table {
    font-size: 0.9rem;
  }

  .services-table th,
  .services-table td {
    padding: 10px 8px;
  }

  .pagination-container {
    flex-direction: column;
    gap: 15px;
  }

  .card-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 15px;
  }

  .toast {
    min-width: 280px;
    margin: 0 10px 10px;
  }
}

@media (max-width: 480px) {
  .table-responsive {
    font-size: 0.8rem;
  }

  .services-table th,
  .services-table td {
    padding: 8px 6px;
  }

  .btn {
    padding: 10px 16px;
    font-size: 0.9rem;
  }

  .btn-sm {
    padding: 6px 10px;
    font-size: 0.8rem;
  }

  .service-img {
    width: 60px;
    height: 45px;
  }

  .action-buttons {
    flex-direction: column;
    gap: 4px;
  }

  .filter-grid {
    grid-template-columns: 1fr;
  }

  .form-actions {
    flex-direction: column;
  }
}
</style>
