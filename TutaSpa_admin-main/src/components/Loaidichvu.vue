<template>
  <div class="loai-dich-vu-management">
    <!-- Background Animation -->
    <div class="background-animation">
      <div class="floating-shape shape-1"></div>
      <div class="floating-shape shape-2"></div>
      <div class="floating-shape shape-3"></div>
    </div>

    <!-- Header with Glass Effect -->
    <div class="header-container">
      <div class="header-glass">
        <div class="header-content">
          <div class="title-section">
            <div class="icon-wrapper">
              <i class="fa fa-list-alt"></i>
            </div>
            <div>
              <h1 class="title">Quản lý Loại Dịch Vụ</h1>
              <p class="subtitle">Quản lý và tổ chức các loại dịch vụ của bạn</p>
            </div>
          </div>
          <div class="stats-section">
            <div class="stat-card">
              <div class="stat-number">{{ loaiDichVus?.length || 0 }}</div>
              <div class="stat-label">Tổng loại dịch vụ</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Form Section with Modern Card -->
    <div class="form-container">
      <div class="modern-card">
        <div class="card-header-modern">
          <div class="header-left">
            <div class="icon-badge" :class="isEditing ? 'editing' : 'creating'">
              <i class="fa" :class="isEditing ? 'fa-edit' : 'fa-plus-circle'"></i>
            </div>
            <div>
              <h2 class="card-title">{{ isEditing ? 'Cập nhật' : 'Thêm mới' }} Loại Dịch Vụ</h2>
              <p class="card-subtitle">
                {{ isEditing ? 'Chỉnh sửa thông tin loại dịch vụ' : 'Thêm loại dịch vụ mới vào hệ thống' }}

              </p>
            </div>
          </div>
        </div>

        <div class="card-body-modern">
          <form @submit.prevent="isEditing ? updateLoaiDichVu() : createLoaiDichVu()" class="modern-form">
            <div class="form-row">
              <div class="input-group">
                <label class="modern-label">
                  <span class="label-text">Tên loại dịch vụ</span>
                  <span class="required-indicator">*</span>
                </label>
                <div class="input-wrapper">
                  <input type="text" class="modern-input" v-model="form.tenLoai" placeholder="Nhập tên loại dịch vụ..."
                    required />
                  <div class="input-focus-line"></div>
                </div>
              </div>

              <div class="button-group">
                <button type="submit" class="btn-primary-modern">
                  <i class="fa" :class="isEditing ? 'fa-save' : 'fa-plus'"></i>
                  <span>{{ isEditing ? 'Cập nhật' : 'Thêm mới' }}</span>
                </button>

                <button v-if="isEditing" type="button" class="btn-secondary-modern" @click="resetForm">
                  <i class="fa fa-undo"></i>
                  <span>Hủy</span>
                </button>
              </div>
            </div>

            <!-- Import Section -->
            <div class="import-section">
              <div class="import-card">
                <div class="import-header">
                  <i class="fa fa-file-import"></i>
                  <span>Import từ Excel</span>
                </div>
                <div class="import-actions">
                  <label class="file-upload-btn">
                    <i class="fa fa-cloud-upload-alt"></i>
                    <span>Chọn file</span>
                    <input type="file" ref="fileInput" accept=".xlsx,.xls" @change="handleFileChange" />
                  </label>
                  <div class="file-info" v-if="selectedFile">
                    <i class="fa fa-file-excel"></i>
                    <span>{{ selectedFile.name }}</span>
                  </div>
                  <button type="button" class="btn-upload" @click="importLoaiDichVu" :disabled="!selectedFile">
                    <i class="fa fa-upload"></i>
                    <span>Tải lên</span>
                  </button>
                  <button class="btn btn-outline-light btn-sm" @click="downloadTemplate">
                    <i class="fa fa-download me-1"></i>
                    Tải file mẫu
                  </button>
                </div>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- List Section -->
    <div class="list-container">
      <div class="modern-card">
        <div class="card-header-modern">
          <div class="header-left">
            <div class="icon-badge list">
              <i class="fa fa-table"></i>
            </div>
            <div>
              <h2 class="card-title">Danh sách loại dịch vụ</h2>
              <p class="card-subtitle">Quản lý và tìm kiếm các loại dịch vụ</p>
            </div>
          </div>

          <div class="header-actions">
            <div class="search-modern">
              <div class="search-input-wrapper">
                <i class="fa fa-search"></i>
                <input v-model="searchName" @input="searchByName" type="text" class="search-input-modern"
                  placeholder="Tìm kiếm loại dịch vụ..." />
                <button v-if="searchName" @click="resetSearch" class="clear-search-modern">
                  <i class="fa fa-times"></i>
                </button>
              </div>
            </div>

            <button @click="fetchData" class="btn-refresh">
              <i class="fa fa-sync-alt"></i>
              <span>Làm mới</span>
            </button>
          </div>
        </div>

        <div class="card-body-modern">
          <!-- Empty State -->
          <div v-if="!loaiDichVus?.length" class="empty-state-modern">
            <div class="empty-icon">
              <i class="fa fa-folder-open"></i>
            </div>
            <h3>Chưa có loại dịch vụ nào</h3>
            <p>Hãy thêm loại dịch vụ đầu tiên để bắt đầu</p>
            <button @click="() => { }" class="btn-primary-modern">
              <i class="fa fa-plus"></i>
              <span>Thêm loại dịch vụ</span>
            </button>
          </div>

          <!-- Modern Table -->
          <div v-else class="table-container-modern">
            <div class="table-wrapper">
              <table class="modern-table">
                <thead>
                  <tr>
                    <th class="th-name">
                      <span>Tên Loại Dịch Vụ</span>
                    </th>
                    <th class="th-actions">
                      <span>Hành động</span>
                    </th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(item, index) in loaiDichVus" :key="item.loaiDichVuID" class="table-row"
                    :style="{ animationDelay: `${index * 0.05}s` }">
                    <td class="td-name">
                      <div class="name-content">
                        <div class="name-text">{{ item.tenLoai }}</div>
                      </div>
                    </td>
                    <td class="td-actions">
                      <div class="action-buttons-modern">
                        <button class="action-btn edit-btn" @click="editLoaiDichVu(item)" title="Chỉnh sửa">
                          <i class="fa fa-edit"></i>
                        </button>
                        <button class="action-btn delete-btn" @click="deleteLoaiDichVu(item.loaiDichVuID)" title="Xóa">
                          <i class="fa fa-trash"></i>
                        </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modern Toast Notifications -->
    <div class="toast-container-modern">
      <transition-group name="toast" tag="div">
        <div v-for="toast in toasts" :key="toast.id" class="toast-modern" :class="toast.type">
          <div class="toast-icon">
            <i class="fa" :class="getToastIcon(toast.type)"></i>
          </div>
          <div class="toast-content">
            <div class="toast-message">{{ toast.message }}</div>
          </div>
          <button class="toast-close" @click="removeToast(toast.id)">
            <i class="fa fa-times"></i>
          </button>
        </div>
      </transition-group>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import apiClient from "../utils/axiosClient";

// State
const loaiDichVus = ref([]);
const allLoaiDichVus = ref([]);
const form = ref({
  tenLoai: ''
});
const isEditing = ref(false);
const searchName = ref('');
const toasts = ref([]);
const fileInput = ref(null);
const selectedFile = ref(null);

// File handling
const handleFileChange = (event) => {
  const file = event.target.files[0];
  selectedFile.value = file || null;
};

const importLoaiDichVu = async () => {
  if (!selectedFile.value) {
    showToast("Vui lòng chọn file Excel để import", "warning");
    return;
  }

  const formData = new FormData();
  formData.append("file", selectedFile.value);

  try {
    const res = await apiClient.post("/LoaiDichVu/importt", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
    showToast(`Import thành công loại dịch vụ!`, "success");

    selectedFile.value = null;
    if (fileInput.value) fileInput.value.value = "";
    fetchData();
  } catch (error) {
    console.error(error);
    showToast("Lỗi khi import loại dịch vụ", "error");
  }
};

// Toast methods
const showToast = (message, type = 'info') => {
  const toast = {
    id: Date.now(),
    message,
    type
  }
  toasts.value.push(toast)
  setTimeout(() => {
    removeToast(toast.id)
  }, 5000)
}

const removeToast = (id) => {
  const index = toasts.value.findIndex(t => t.id === id)
  if (index > -1) toasts.value.splice(index, 1)
}

const getToastIcon = (type) => {
  switch (type) {
    case 'success': return 'fa-check-circle'
    case 'error': return 'fa-exclamation-circle'
    case 'warning': return 'fa-exclamation-triangle'
    default: return 'fa-info-circle'
  }
}

// API methods
const fetchData = async () => {
  try {
    const res = await apiClient.get('/LoaiDichVu');
    allLoaiDichVus.value = res;
    loaiDichVus.value = res;
  } catch (error) {
    showToast("Lỗi khi tải danh sách loại dịch vụ", 'error');
    console.error(error);
  }
};

const createLoaiDichVu = async () => {
  if (!form.value.tenLoai.trim()) {
    showToast("Vui lòng nhập tên loại", 'warning');
    return;
  }
  try {
    await apiClient.post('/LoaiDichVu', form.value);
    showToast("Thêm loại dịch vụ thành công!", 'success');
    resetForm();
    fetchData();
  } catch (error) {
    showToast("Lỗi khi thêm loại dịch vụ", 'error');
    console.error(error);
  }
};

const updateLoaiDichVu = async () => {
  try {
    await apiClient.put(`/LoaiDichVu/${form.value.loaiDichVuID}`, form.value);
    showToast("Cập nhật loại dịch vụ thành công!", 'success');
    resetForm();
    fetchData();
  } catch (error) {
    showToast("Lỗi khi cập nhật loại dịch vụ", 'error');
    console.error(error);
  }
};

const deleteLoaiDichVu = async (id) => {
  if (confirm("Bạn có chắc muốn xóa loại dịch vụ này?")) {
    try {
      await apiClient.delete(`/LoaiDichVu/${id}`);
      showToast("Xóa loại dịch vụ thành công!", 'success');
      fetchData();
    } catch (error) {
      showToast("Lỗi khi xóa loại dịch vụ", 'error');
      console.error(error);
    }
  }
};

const editLoaiDichVu = (item) => {
  form.value = { ...item };
  isEditing.value = true;
};
const downloadTemplate = () => {
  const link = document.createElement("a");
  link.href = "https://docs.google.com/spreadsheets/d/1XKhk2qVil9iVqwYN_tVd19N9orq-7QEc/export?format=xlsx";
  link.download = "LDV_DV_LSP_SP.xlsx";
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

const resetForm = () => {
  form.value = {
    loaiDichVuID: 0,
    tenLoai: ''
  };
  isEditing.value = false;
};

const searchByName = () => {
  const keyword = searchName.value.trim().toLowerCase();
  if (!keyword) {
    loaiDichVus.value = [...allLoaiDichVus.value];
    return;
  }
  loaiDichVus.value = allLoaiDichVus.value.filter(item =>
    item.tenLoai.toLowerCase().includes(keyword)
  );
};

const resetSearch = () => {
  searchName.value = '';
  loaiDichVus.value = [...allLoaiDichVus.value];
};

onMounted(() => {
  fetchData();
});
</script>

<style scoped>
* {
  box-sizing: border-box;
}

.loai-dich-vu-management {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  position: relative;
  overflow-x: hidden;
}

/* Background Animation */
.background-animation {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  z-index: 1;
}

.floating-shape {
  position: absolute;
  opacity: 0.1;
  animation: float 20s infinite ease-in-out;
}

.shape-1 {
  width: 300px;
  height: 300px;
  background: radial-gradient(circle, #ff6b6b, #feca57);
  border-radius: 50%;
  top: 10%;
  left: 10%;
  animation-delay: 0s;
}

.shape-2 {
  width: 200px;
  height: 200px;
  background: radial-gradient(circle, #48dbfb, #0abde3);
  border-radius: 50%;
  top: 60%;
  right: 10%;
  animation-delay: -7s;
}

.shape-3 {
  width: 250px;
  height: 250px;
  background: radial-gradient(circle, #ff9ff3, #f368e0);
  border-radius: 50%;
  bottom: 20%;
  left: 20%;
  animation-delay: -14s;
}

@keyframes float {

  0%,
  100% {
    transform: translateY(0px) rotate(0deg);
  }

  25% {
    transform: translateY(-20px) rotate(90deg);
  }

  50% {
    transform: translateY(0px) rotate(180deg);
  }

  75% {
    transform: translateY(-10px) rotate(270deg);
  }
}

/* Header Section */
.header-container {
  position: relative;
  z-index: 10;
  margin-bottom: 30px;
}

.header-glass {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(20px);
  border-radius: 24px;
  border: 1px solid rgba(255, 255, 255, 0.2);
  padding: 30px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 30px;
}

.title-section {
  display: flex;
  align-items: center;
  gap: 20px;
}

.icon-wrapper {
  width: 80px;
  height: 80px;
  background: linear-gradient(135deg, #ff6b6b, #feca57);
  border-radius: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2rem;
  color: white;
  box-shadow: 0 10px 30px rgba(255, 107, 107, 0.3);
}

.title {
  font-size: 2.5rem;
  font-weight: 700;
  color: white;
  margin: 0 0 5px 0;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.subtitle {
  font-size: 1.1rem;
  color: rgba(255, 255, 255, 0.8);
  margin: 0;
}

.stats-section {
  text-align: center;
}

.stat-card {
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(10px);
  border-radius: 16px;
  padding: 20px 30px;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.stat-number {
  font-size: 2.5rem;
  font-weight: 700;
  color: white;
  line-height: 1;
}

.stat-label {
  font-size: 0.9rem;
  color: rgba(255, 255, 255, 0.8);
  margin-top: 5px;
}

/* Modern Card */
.form-container,
.list-container {
  position: relative;
  z-index: 10;
  margin-bottom: 30px;
}

.modern-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  border-radius: 24px;
  border: 1px solid rgba(255, 255, 255, 0.3);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  transition: all 0.3s ease;
}

.modern-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 30px 60px rgba(0, 0, 0, 0.15);
}

.card-header-modern {
  padding: 25px 30px;
  background: linear-gradient(135deg, #667eea, #764ba2);
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 20px;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 20px;
}

.icon-badge {
  width: 60px;
  height: 60px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  color: white;
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.2);
}

.icon-badge.creating {
  background: linear-gradient(135deg, #00d2ff, #3a7bd5);
}

.icon-badge.editing {
  background: linear-gradient(135deg, #f093fb, #f5576c);
}

.icon-badge.list {
  background: linear-gradient(135deg, #4facfe, #00f2fe);
}

.card-title {
  font-size: 1.5rem;
  font-weight: 600;
  margin: 0 0 5px 0;
}

.card-subtitle {
  font-size: 0.95rem;
  opacity: 0.9;
  margin: 0;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 15px;
}

.card-body-modern {
  padding: 30px;
}

/* Modern Form */
.modern-form {
  max-width: 100%;
}

.form-row {
  display: flex;
  flex-direction: column;
  gap: 25px;
}

.input-group {
  flex: 1;
}

.modern-label {
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  color: #2d3748;
  font-size: 0.95rem;
}

.label-text {
  margin-right: 5px;
}

.required-indicator {
  color: #e53e3e;
  font-weight: 700;
}

.input-wrapper {
  position: relative;
}

.modern-input {
  width: 100%;
  padding: 16px 20px;
  border: 2px solid #e2e8f0;
  border-radius: 12px;
  font-size: 1rem;
  background: #f7fafc;
  color: #2d3748;
  transition: all 0.3s ease;
  outline: none;
}

.modern-input:focus {
  border-color: #667eea;
  background: white;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.modern-input:focus+.input-focus-line {
  transform: scaleX(1);
}

.input-focus-line {
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 2px;
  background: linear-gradient(135deg, #667eea, #764ba2);
  transform: scaleX(0);
  transition: transform 0.3s ease;
  border-radius: 1px;
}

.button-group {
  display: flex;
  gap: 15px;
  align-items: center;
}

/* Modern Buttons */
.btn-primary-modern {
  background: linear-gradient(135deg, #667eea, #764ba2);
  color: white;
  border: none;
  padding: 14px 28px;
  border-radius: 12px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
}

.btn-primary-modern:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(102, 126, 234, 0.6);
}

.btn-secondary-modern {
  background: linear-gradient(135deg, #a0aec0, #718096);
  color: white;
  border: none;
  padding: 14px 28px;
  border-radius: 12px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s ease;
}

.btn-secondary-modern:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(160, 174, 192, 0.4);
}

.btn-refresh {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 1px solid rgba(255, 255, 255, 0.3);
  padding: 12px 20px;
  border-radius: 10px;
  font-size: 0.9rem;
  font-weight: 500;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
}

.btn-refresh:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
}

/* Import Section */
.import-section {
  margin-top: 30px;
  padding-top: 30px;
  border-top: 1px solid #e2e8f0;
}

.import-card {
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
  border-radius: 16px;
  padding: 20px;
  color: white;
}

.import-header {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 1.1rem;
  font-weight: 600;
  margin-bottom: 15px;
}

.import-actions {
  display: flex;
  align-items: center;
  gap: 15px;
  flex-wrap: wrap;
}

.file-upload-btn {
  background: rgba(255, 255, 255, 0.2);
  border: 2px dashed rgba(255, 255, 255, 0.5);
  border-radius: 10px;
  padding: 12px 20px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s ease;
  font-weight: 500;
}

.file-upload-btn:hover {
  background: rgba(255, 255, 255, 0.3);
  border-color: rgba(255, 255, 255, 0.8);
}

.file-upload-btn input {
  display: none;
}

.file-info {
  background: rgba(255, 255, 255, 0.2);
  padding: 8px 15px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 0.9rem;
}

.btn-upload {
  background: rgba(255, 255, 255, 0.9);
  color: #f5576c;
  border: none;
  padding: 12px 20px;
  border-radius: 10px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s ease;
}

.btn-upload:hover:not(:disabled) {
  background: white;
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
}

.btn-upload:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Search Modern */
.search-modern {
  position: relative;
}

.search-input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.search-input-wrapper i {
  position: absolute;
  left: 15px;
  color: rgba(255, 255, 255, 0.7);
  z-index: 2;
}

.search-input-modern {
  background: rgba(255, 255, 255, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: 10px;
  padding: 12px 45px 12px 45px;
  color: white;
  font-size: 0.95rem;
  width: 300px;
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
  outline: none;
}

.search-input-modern::placeholder {
  color: rgba(255, 255, 255, 0.7);
}

.search-input-modern:focus {
  background: rgba(255, 255, 255, 0.3);
  border-color: rgba(255, 255, 255, 0.6);
  box-shadow: 0 0 0 3px rgba(255, 255, 255, 0.1);
}

.clear-search-modern {
  position: absolute;
  right: 15px;
  background: none;
  border: none;
  color: rgba(255, 255, 255, 0.7);
  cursor: pointer;
  padding: 5px;
  border-radius: 50%;
  transition: all 0.3s ease;
}

.clear-search-modern:hover {
  background: rgba(255, 255, 255, 0.2);
  color: white;
}

/* Empty State Modern */
.empty-state-modern {
  text-align: center;
  padding: 80px 20px;
  color: #718096;
}

.empty-icon {
  width: 120px;
  height: 120px;
  background: linear-gradient(135deg, #e2e8f0, #cbd5e0);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 30px;
  font-size: 3rem;
  color: #a0aec0;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
}

.empty-state-modern h3 {
  font-size: 1.5rem;
  font-weight: 600;
  margin: 0 0 10px;
  color: #4a5568;
}

.empty-state-modern p {
  font-size: 1rem;
  margin: 0 0 30px;
  color: #718096;
}

/* Modern Table */
.modern-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  table-layout: auto;
}

.modern-table th,
.modern-table td {
  padding: 14px 20px;
  border: none;
  vertical-align: middle;
  font-size: 0.95rem;
}

/* Header */
.modern-table thead {
  background: linear-gradient(135deg, #2d3748, #4a5568);
}

.modern-table th {
  font-weight: 600;
  font-size: 0.95rem;
  color: white;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Cột tên loại dịch vụ */
.th-name,
.td-name {
  text-align: left;
  width: 70%;
}

/* Cột hành động */
.th-actions,
.td-actions {
  text-align: center;
  width: 30%;
}

/* Nút hành động căn giữa */
.action-buttons-modern {
  display: flex;
  justify-content: center;
  align-items: center;
  /* căn giữa theo chiều dọc */
  gap: 12px;
}

.td-name .name-content {
  display: flex;
  align-items: center;
  gap: 15px;
}

.name-text {
  font-weight: 600;
  font-size: 1rem;
  color: #2d3748;
}

.td-actions {
  text-align: center;
}

.action-buttons-modern {
  display: flex;
  gap: 10px;
  justify-content: center;
}

.action-btn {
  width: 40px;
  height: 40px;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.9rem;
  transition: all 0.3s ease;
  color: white;
  font-weight: 500;
}

.edit-btn {
  background: linear-gradient(135deg, #48bb78, #38a169);
  box-shadow: 0 2px 10px rgba(72, 187, 120, 0.3);
}

.edit-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(72, 187, 120, 0.5);
}

.delete-btn {
  background: linear-gradient(135deg, #f56565, #e53e3e);
  box-shadow: 0 2px 10px rgba(245, 101, 101, 0.3);
}

.delete-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(245, 101, 101, 0.5);
}

/* Toast Notifications Modern */
.toast-container-modern {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 1000;
  display: flex;
  flex-direction: column;
  gap: 10px;
  max-width: 400px;
}

.toast-modern {
  background: white;
  border-radius: 16px;
  padding: 20px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.15);
  border: 1px solid #e2e8f0;
  display: flex;
  align-items: center;
  gap: 15px;
  backdrop-filter: blur(20px);
  position: relative;
  overflow: hidden;
}

.toast-modern::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  width: 4px;
  height: 100%;
  background: var(--toast-color);
}

.toast-modern.success {
  --toast-color: #48bb78;
}

.toast-modern.error {
  --toast-color: #f56565;
}

.toast-modern.warning {
  --toast-color: #ed8936;
}

.toast-modern.info {
  --toast-color: #4299e1;
}

.toast-icon {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 1.2rem;
  flex-shrink: 0;
}

.toast-modern.success .toast-icon {
  background: linear-gradient(135deg, #48bb78, #38a169);
}

.toast-modern.error .toast-icon {
  background: linear-gradient(135deg, #f56565, #e53e3e);
}

.toast-modern.warning .toast-icon {
  background: linear-gradient(135deg, #ed8936, #dd6b20);
}

.toast-modern.info .toast-icon {
  background: linear-gradient(135deg, #4299e1, #3182ce);
}

.toast-content {
  flex: 1;
}

.toast-message {
  font-weight: 500;
  color: #2d3748;
  font-size: 0.95rem;
  line-height: 1.4;
}

.toast-close {
  background: none;
  border: none;
  color: #a0aec0;
  cursor: pointer;
  padding: 5px;
  border-radius: 50%;
  transition: all 0.3s ease;
  flex-shrink: 0;
}

.toast-close:hover {
  background: #f7fafc;
  color: #4a5568;
}

/* Toast Animations */
.toast-enter-active,
.toast-leave-active {
  transition: all 0.4s ease;
}

.toast-enter-from {
  opacity: 0;
  transform: translateX(100%) scale(0.8);
}

.toast-enter-to {
  opacity: 1;
  transform: translateX(0) scale(1);
}

.toast-leave-from {
  opacity: 1;
  transform: translateX(0) scale(1);
}

.toast-leave-to {
  opacity: 0;
  transform: translateX(100%) scale(0.8);
}

/* Table Animation */
@keyframes fadeInUp {
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Responsive Design */
@media (max-width: 768px) {
  .loai-dich-vu-management {
    padding: 15px;
  }

  .header-content {
    flex-direction: column;
    text-align: center;
    gap: 25px;
  }

  .title {
    font-size: 2rem;
  }

  .title-section {
    flex-direction: column;
    text-align: center;
  }

  .card-header-modern {
    flex-direction: column;
    gap: 20px;
    text-align: center;
  }

  .header-actions {
    width: 100%;
    justify-content: center;
    flex-wrap: wrap;
  }

  .search-input-modern {
    width: 100%;
    max-width: 300px;
  }

  .form-row {
    gap: 20px;
  }

  .button-group {
    flex-direction: column;
    width: 100%;
  }

  .btn-primary-modern,
  .btn-secondary-modern {
    width: 100%;
    justify-content: center;
  }

  .import-actions {
    flex-direction: column;
    align-items: stretch;
  }

  .file-upload-btn,
  .btn-upload {
    width: 100%;
    justify-content: center;
  }

  .modern-table {
    font-size: 0.9rem;
  }

  .modern-table th,
  .modern-table td {
    padding: 15px 10px;
  }

  .action-buttons-modern {
    flex-direction: column;
    gap: 8px;
  }

  .toast-container-modern {
    left: 15px;
    right: 15px;
    max-width: none;
  }
}

@media (max-width: 480px) {
  .icon-wrapper {
    width: 60px;
    height: 60px;
    font-size: 1.5rem;
  }

  .title {
    font-size: 1.8rem;
  }

  .subtitle {
    font-size: 1rem;
  }

  .icon-badge {
    width: 50px;
    height: 50px;
    font-size: 1.2rem;
  }

  .card-title {
    font-size: 1.3rem;
  }

  .modern-table th,
  .modern-table td {
    padding: 12px 8px;
  }

  .action-btn {
    width: 35px;
    height: 35px;
    font-size: 0.8rem;
  }

  .id-badge {
    padding: 4px 8px;
    font-size: 0.8rem;
    min-width: 40px;
  }
}
</style>