<template>
  <div class="voucher-management">
    <!-- Add Voucher Section -->
    <div class="card add-voucher-section">
      <div class="card-header">
        <h2 class="section-title">
          <i class="fas fa-ticket-alt"></i>
          Thêm voucher mới
        </h2>
        <button 
          @click="toggleAddForm" 
          class="btn btn-primary"
          :class="{ 'active': showAddForm }"
        >
          <i class="fas" :class="showAddForm ? 'fa-minus' : 'fa-plus'"></i>
          {{ showAddForm ? 'Ẩn form' : 'Hiển thị form' }}
        </button>
      </div>
      
      <div v-if="showAddForm" class="card-body">
        <form @submit.prevent="saveVoucher" class="voucher-form">
          <!-- Row 1: 3 columns - Mã code, Giá trị giảm, Kiểu giảm giá -->
          <div class="form-grid">
            <div class="form-group">
              <label for="maCode">Mã code <span class="required">*</span></label>
              <input
                id="maCode"
                v-model="voucher.maCode"
                type="text"
                class="form-control"
                placeholder="Nhập mã code"
                required
                @input="removeVietnamese"
              />
            </div>

            <div class="form-group">
              <label for="giaTriGiam">Giá trị giảm <span class="required">*</span></label>
              <input
                id="giaTriGiam"
                v-model.number="voucher.giaTriGiam"
                type="number"
                class="form-control"
                placeholder="Nhập giá trị giảm"
                required
                min="1"
              />
            </div>

            <div class="form-group">
              <label for="kieuGiamGia">Kiểu giảm giá <span class="required">*</span></label>
              <select
                id="kieuGiamGia"
                v-model="voucher.kieuGiamGia"
                class="form-control"
                required
              >
                <option value="">Chọn kiểu giảm giá</option>
                <option value="0">Phần trăm (%)</option>
                <option value="1">Tiền mặt (VNĐ)</option>
              </select>
            </div>
          </div>

          <!-- Row 2: 2 columns - Ngày bắt đầu, Ngày kết thúc -->
          <div class="form-row-2">
            <div class="form-group">
              <label for="ngayBatDau">Ngày bắt đầu <span class="required">*</span></label>
              <input
                id="ngayBatDau"
                v-model="voucher.ngayBatDau"
                type="date"
                class="form-control"
                required
              />
            </div>

            <div class="form-group">
              <label for="ngayKetThuc">Ngày kết thúc <span class="required">*</span></label>
              <input
                id="ngayKetThuc"
                v-model="voucher.ngayKetThuc"
                type="date"
                class="form-control"
                required
              />
            </div>
          </div>

          <!-- Row 3: Số lượng và Phiếu đặc biệt -->
          <div class="form-row-3">
            <div class="form-group">
              <label for="soLuong">Số lượng <span class="required">*</span></label>
              <input
                id="soLuong"
                v-model.number="voucher.soLuong"
                type="number"
                class="form-control"
                placeholder="Nhập số lượng"
                min="1"
                max="100"
                :disabled="voucher.voHan"
                required
              />
            </div>

            <!-- Special voucher checkbox positioned beside quantity -->
            <div class="form-group checkbox-group">
              <label class="checkbox-label">Loại phiếu</label>
              <div class="form-check">
                <input
                  v-model="voucher.voHan"
                  class="form-check-input"
                  type="checkbox"
                  id="voHanCheckbox"
                />
                <label class="form-check-label" for="voHanCheckbox">
                  <i class="fas fa-star"></i>
                  Phiếu Đặc Biệt
                </label>
              </div>
            </div>
          </div>

          <div class="form-actions">
            <button 
              type="submit" 
              class="btn btn-success"
              :disabled="loading"
            >
              <i class="fas fa-save"></i>
              {{ loading ? 'Đang lưu...' : (isEditing ? 'Cập nhật' : 'Thêm voucher') }}
            </button>
            <button 
              type="button" 
              @click="resetForm" 
              class="btn btn-secondary"
            >
              <i class="fas fa-undo"></i>
              Reset
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Vouchers List Section -->
    <div class="card vouchers-list-section">
      <div class="card-header">
        <h2 class="section-title">
          <i class="fas fa-list"></i>
          Danh sách voucher
        </h2>
        <div class="list-controls">
          <div class="search-container">
            <input
              v-model="searchCode"
              type="text"
              class="search-input"
              placeholder="Tìm theo mã voucher"
            />
            <button
              v-if="searchCode"
              @click="clearSearch"
              class="clear-search"
              title="Xóa tìm kiếm"
            >
              <i class="fas fa-times"></i>
            </button>
            <i class="fas fa-search search-icon"></i>
          </div>
          <button @click="resetFilters" class="btn btn-outline-primary">
            <i class="fas fa-sync-alt"></i>
            Làm mới
          </button>
        </div>
      </div>

      <div class="card-body">
        <!-- Filters Section -->
        <div class="filters-section">
          <div class="filter-grid">
            <div class="filter-group">
              <label>Giá trị giảm</label>
              <div class="input-group">
                <input v-model.number="minValue" type="number" class="form-control" placeholder="Từ" />
                <input v-model.number="maxValue" type="number" class="form-control" placeholder="Đến" />
              </div>
            </div>

            <div class="filter-group">
              <label>Kiểu giảm</label>
              <select v-model="selectedType" class="form-control">
                <option value="">Tất cả</option>
                <option value="0">Phần trăm (%)</option>
                <option value="1">Tiền mặt (VNĐ)</option>
              </select>
            </div>

            <div class="filter-group">
              <label>Loại phiếu</label>
              <select v-model="selectedVoucherType" class="form-control">
                <option value="">Tất cả</option>
                <option value="special">Phiếu đặc biệt</option>
                <option value="normal">Phiếu thường</option>
              </select>
            </div>
          </div>
        </div>

        <!-- Loading State -->
        <div v-if="loading" class="loading-state">
          <i class="fas fa-spinner fa-spin"></i>
          Đang tải...
        </div>

        <!-- Empty State -->
        <div v-else-if="!vouchers.length" class="empty-state">
          <i class="fas fa-ticket-alt"></i>
          <p>Không có voucher nào</p>
        </div>

        <!-- Vouchers Table -->
        <div v-else class="table-responsive">
          <table class="vouchers-table">
            <thead>
              <tr>
                <th>Mã code</th>
                <th>Giá trị</th>
                <th>Kiểu giảm</th>
                <th>Ngày bắt đầu</th>
                <th>Ngày kết thúc</th>
                <th>Số lượng</th>
                <th>Trạng thái</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              <tr 
                v-for="v in sortedVouchers" 
                :key="v.voucherID" 
                class="voucher-row"
                :class="{ 'expired': isExpired(v.ngayKetThuc) }"
              >
                <td class="voucher-code">
                  <div class="code-container">
                    <i class="fas fa-ticket-alt"></i>
                    {{ v.maCode }}
                  </div>
                </td>
                <td class="voucher-value">{{ formatNumber(v.giaTriGiam) }}</td>
                <td class="voucher-type">
                  <span class="type-badge" :class="getTypeClass(v.kieuGiamGia)">
                    {{ v.kieuGiamGia === 0 ? '%' : 'VNĐ' }}
                  </span>
                </td>
                <td class="voucher-date">{{ formatDate(v.ngayBatDau) }}</td>
                <td class="voucher-date">{{ formatDate(v.ngayKetThuc) }}</td>
                <td class="voucher-quantity">
                  <span v-if="Number(v.soLuong) === -1" class="special-badge">
                    <i class="fas fa-star"></i>
                    Đặc biệt
                  </span>
                  <span v-else class="quantity-number">{{ v.soLuong }}</span>
                </td>
                <td class="voucher-status">
                  <span 
                    class="status-badge"
                    :class="getStatusClass(v.ngayBatDau, v.ngayKetThuc, v.soLuong)"
                  >
                    {{ getVoucherStatus(v.ngayBatDau, v.ngayKetThuc, v.soLuong) }}
                  </span>
                </td>
                <td class="voucher-actions">
                  <div class="action-buttons">
                    <button 
                      @click="editVoucher(v)" 
                      class="btn btn-sm btn-info"
                      title="Chỉnh sửa"
                    >
                      <i class="fas fa-edit"></i>
                    </button>
                    <button 
                      @click="deleteVoucher(v.voucherID)" 
                      class="btn btn-sm btn-danger"
                      title="Xóa"
                    >
                      <i class="fas fa-trash"></i>
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Toast Notifications -->
    <div class="toast-container">
      <div 
        v-for="toast in toasts" 
        :key="toast.id" 
        class="toast" 
        :class="toast.type"
      >
        <i class="fas" :class="getToastIcon(toast.type)"></i>
        {{ toast.message }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, reactive, watch, computed } from 'vue'
import apiClient from "../utils/axiosClient"

// Reactive data
const vouchers = ref([])
const loading = ref(false)
const showAddForm = ref(false)
const isEditing = ref(false)
const toasts = ref([])

// Search and filter
const searchCode = ref('')
const minValue = ref('')
const maxValue = ref('')
const selectedType = ref('')
const selectedVoucherType = ref('')

// Get today's date
const getTodayDate = () => {
  const now = new Date()
  const yyyy = now.getFullYear()
  const mm = String(now.getMonth() + 1).padStart(2, "0")
  const dd = String(now.getDate()).padStart(2, "0")
  return `${yyyy}-${mm}-${dd}`
}

// Voucher form data
const voucher = reactive({
  voucherID: 0,
  maCode: "",
  giaTriGiam: "",
  kieuGiamGia: "",
  ngayBatDau: getTodayDate(),
  ngayKetThuc: "",
  soLuong: 1,
  voHan: false,
})

// Methods
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

const toggleAddForm = () => {
  showAddForm.value = !showAddForm.value
}

const formatDate = (isoDate) => {
  if (!isoDate) return ""
  return isoDate.split("T")[0]
}

const formatDateForApi = (dateStr) => {
  const d = new Date(dateStr)
  return d.toISOString()
}

const formatNumber = (value) => {
  return new Intl.NumberFormat('vi-VN').format(value)
}

const isExpired = (dateString) => {
  const today = new Date()
  const endDate = new Date(dateString)
  today.setHours(0, 0, 0, 0)
  endDate.setHours(0, 0, 0, 0)
  return endDate < today
}

const getVoucherStatus = (startDate, endDate, soLuong) => {
  const today = new Date()
  today.setHours(0, 0, 0, 0)

  const start = new Date(startDate)
  const end = new Date(endDate)
  start.setHours(0, 0, 0, 0)
  end.setHours(0, 0, 0, 0)

  if (start > today) return "Chưa bắt đầu"
  if (end < today) return "Hết hạn"
  if (soLuong === 0) return "Hết số lượng"
  return "Còn hạn"
}

const getStatusClass = (startDate, endDate, soLuong) => {
  const status = getVoucherStatus(startDate, endDate, soLuong)
  switch (status) {
    case 'Chưa bắt đầu': return 'pending'
    case 'Còn hạn': return 'active'
    case 'Hết hạn': return 'expired'
    case 'Hết số lượng': return 'outofstock'
    default: return 'pending'
  }
}

const getTypeClass = (type) => {
  return type === 0 ? 'percent' : 'money'
}

const removeVietnamese = (event) => {
  const raw = event.target.value
  const noAccent = raw
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
  event.target.value = noAccent
  voucher.maCode = noAccent.toUpperCase()
}

const fetchVouchers = async () => {
  try {
    loading.value = true
    const res = await apiClient.get("Vouchers")
    vouchers.value = res
  } catch (err) {
    showToast('Lỗi khi tải danh sách voucher', 'error')
    console.error("Lỗi fetch voucher:", err)
  } finally {
    loading.value = false
  }
}

const saveVoucher = async () => {
  const ngayBatDauDate = new Date(voucher.ngayBatDau)
  const ngayKetThucDate = new Date(voucher.ngayKetThuc)
  const today = new Date()
  today.setHours(0, 0, 0, 0)

  if (!voucher.ngayBatDau || !voucher.ngayKetThuc) {
    showToast("Vui lòng nhập đủ ngày bắt đầu và ngày kết thúc.", 'warning')
    return
  }

  if (ngayBatDauDate < today) {
    showToast("Ngày bắt đầu không được nhỏ hơn ngày hiện tại.", 'warning')
    return
  }

  if (ngayBatDauDate > ngayKetThucDate) {
    showToast("Ngày bắt đầu không được lớn hơn ngày kết thúc.", 'warning')
    return
  }

  if (!voucher.voHan && voucher.soLuong < 1) {
    showToast("Số lượng phải lớn hơn hoặc bằng 1.", 'warning')
    return
  }

  // ✅ Kiểm tra nếu là kiểu phần trăm thì không vượt quá 100
  if (voucher.kieuGiamGia === "0" && voucher.giaTriGiam > 100) {
    showToast("Giá trị giảm (%) không được lớn hơn 100.", 'warning')
    return
  }

  try {
    loading.value = true
    const payload = {
      ...voucher,
      ngayBatDau: formatDateForApi(voucher.ngayBatDau),
      ngayKetThuc: formatDateForApi(voucher.ngayKetThuc),
      soLuong: voucher.voHan ? -1 : voucher.soLuong,
    }

    if (isEditing.value) {
      await apiClient.put(`Vouchers/${voucher.voucherID}`, payload)
      showToast("Cập nhật voucher thành công!", 'success')
    } else {
      await apiClient.post("Vouchers", payload)
      showToast("Thêm voucher thành công!", 'success')
    }

    resetForm()
    showAddForm.value = false
    await fetchVouchers()
  } catch (err) {
    showToast(err.response?.data?.message || "Lỗi khi lưu voucher", 'error')
    console.error("Lỗi khi lưu voucher:", err)
  } finally {
    loading.value = false
  }
}


const deleteVoucher = async (id) => {
  if (confirm("Bạn có chắc chắn muốn xóa voucher này?")) {
    try {
      await apiClient.delete(`Vouchers/${id}`)
      showToast("Xóa voucher thành công!", 'success')
      await fetchVouchers()
    } catch (err) {
      showToast("Lỗi khi xóa voucher", 'error')
      console.error("Lỗi xóa voucher:", err)
    }
  }
}

const editVoucher = (v) => {
  Object.assign(voucher, {
    ...v,
    ngayBatDau: v.ngayBatDau.split("T")[0],
    ngayKetThuc: v.ngayKetThuc.split("T")[0],
    voHan: v.soLuong === -1,
    soLuong: v.soLuong === -1 ? 1 : v.soLuong,
  })
  isEditing.value = true
  showAddForm.value = true
}

const resetForm = () => {
  isEditing.value = false
  Object.assign(voucher, {
    voucherID: 0,
    maCode: "",
    giaTriGiam: "",
    kieuGiamGia: "",
    ngayBatDau: getTodayDate(),
    ngayKetThuc: "",
    soLuong: 1,
    voHan: false,
  })
}

const applyAllFilters = async () => {
  try {
    const res = await apiClient.get("Vouchers")
    let data = res

    // Filter by code
    if (searchCode.value.trim()) {
      data = data.filter((v) =>
        v.maCode.toLowerCase().includes(searchCode.value.trim().toLowerCase())
      )
    }

    // Filter by value range
    const min = Number(minValue.value)
    const max = Number(maxValue.value)

    if (!isNaN(min) && minValue.value !== "" && (isNaN(max) || maxValue.value === "")) {
      data = data.filter((v) => v.giaTriGiam >= min)
    }

    if (!isNaN(max) && maxValue.value !== "" && (isNaN(min) || minValue.value === "")) {
      data = data.filter((v) => v.giaTriGiam <= max)
    }

    if (!isNaN(min) && minValue.value !== "" && !isNaN(max) && maxValue.value !== "") {
      data = data.filter((v) => v.giaTriGiam >= min && v.giaTriGiam <= max)
    }

    // Filter by type
    if (selectedType.value !== "") {
      data = data.filter((v) => String(v.kieuGiamGia) === selectedType.value)
    }

    // Filter by voucher type
    if (selectedVoucherType.value === "special") {
      data = data.filter((v) => v.soLuong === -1)
    } else if (selectedVoucherType.value === "normal") {
      data = data.filter((v) => v.soLuong !== -1)
    }

    vouchers.value = data
  } catch (err) {
    console.error("Lỗi lọc dữ liệu:", err)
  }
}

const clearSearch = () => {
  searchCode.value = ''
}

const resetFilters = () => {
  searchCode.value = ""
  minValue.value = ""
  maxValue.value = ""
  selectedType.value = ""
  selectedVoucherType.value = ""
  fetchVouchers()
}

// Computed
const sortedVouchers = computed(() => {
  const today = new Date()
  today.setHours(0, 0, 0, 0)

  return [...vouchers.value].sort((a, b) => {
    const aDate = new Date(a.ngayKetThuc)
    const bDate = new Date(b.ngayKetThuc)
    aDate.setHours(0, 0, 0, 0)
    bDate.setHours(0, 0, 0, 0)

    const aExpired = aDate < today
    const bExpired = bDate < today

    if (aExpired && !bExpired) return 1
    if (!aExpired && bExpired) return -1

    return aDate - bDate
  })
})

// Watchers
watch(searchCode, applyAllFilters)
watch(minValue, applyAllFilters)
watch(maxValue, applyAllFilters)
watch(selectedType, applyAllFilters)
watch(selectedVoucherType, applyAllFilters)

// Lifecycle
onMounted(() => {
  fetchVouchers()
})
</script>

<style scoped>
/* ===== BASE STYLES ===== */
.voucher-management {
  max-width: 1400px;
  margin: 0 auto;
  padding: 20px;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
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

/* ===== ADD VOUCHER SECTION ===== */
.add-voucher-section {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border: none;
  box-shadow: 0 10px 30px rgba(102, 126, 234, 0.3);
}

.add-voucher-section .card-header {
  background: transparent;
  border-bottom: 2px solid rgba(255, 255, 255, 0.1);
}

.add-voucher-section .card-body {
  background: white;
  margin: 20px;
  border-radius: 16px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  position: relative;
}

.add-voucher-section .card-body::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, #667eea, #764ba2, #f093fb, #f5576c);
  background-size: 300% 100%;
  animation: gradientShift 3s ease infinite;
}

@keyframes gradientShift {
  0%, 100% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
}

/* ===== FORM STYLES ===== */
.voucher-form {
  padding-top: 10px;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 25px;
  margin-bottom: 25px;
}

.form-row-2 {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 25px;
  margin-bottom: 25px;
}

.form-row-3 {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 25px;
  margin-bottom: 25px;
}

.form-group {
  position: relative;
  margin-bottom: 5px;
}

.form-group label {
  font-weight: 700;
  color: #2c3e50;
  margin-bottom: 8px;
  font-size: 0.95rem;
  display: flex;
  align-items: center;
  gap: 5px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.form-group label::before {
  content: '';
  width: 3px;
  height: 16px;
  background: linear-gradient(135deg, #667eea, #764ba2);
  border-radius: 2px;
}

.required {
  color: #e74c3c;
  font-weight: 900;
}

.form-control {
  padding: 16px 20px;
  border: 2px solid #e1e8ed;
  border-radius: 12px;
  font-size: 1rem;
  font-weight: 500;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  background: linear-gradient(135deg, #fafbfc 0%, #f8f9fa 100%);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
  width: 100%;
}

.form-control::placeholder {
  color: #adb5bd;
  font-style: italic;
}

.form-control:focus {
  outline: none;
  border-color: #667eea;
  background: white;
  box-shadow: 0 0 0 4px rgba(102, 126, 234, 0.1), 0 4px 12px rgba(0, 0, 0, 0.08);
  transform: translateY(-1px);
}

.form-control:hover:not(:focus) {
  border-color: #c8d0e7;
  background: white;
}

.form-control:disabled {
  background: linear-gradient(135deg, #f1f3f4 0%, #e9ecef 100%);
  color: #6c757d;
  border-color: #dee2e6;
  cursor: not-allowed;
}

/* ===== SPECIAL VOUCHER CHECKBOX ===== */
.checkbox-group {
  display: flex;
  flex-direction: column;
  justify-content: flex-end;
}

.checkbox-label {
  font-weight: 700;
  color: #2c3e50;
  margin-bottom: 8px;
  font-size: 0.95rem;
  display: flex;
  align-items: center;
  gap: 5px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.checkbox-label::before {
  content: '';
  width: 3px;
  height: 16px;
  background: linear-gradient(135deg, #667eea, #764ba2);
  border-radius: 2px;
}

.form-check {
  display: flex;
  align-items: center;
  gap: 8px;
  background: linear-gradient(135deg, #fff3cd 0%, #ffeaa7 100%);
  padding: 16px 16px;
  border-radius: 12px;
  border: 2px solid #f39c12;
  box-shadow: 0 4px 12px rgba(243, 156, 18, 0.2);
  transition: all 0.3s ease;
  cursor: pointer;
  position: relative;
  overflow: hidden;
  white-space: nowrap;
  font-size: 0.95rem;
  height: 58px;
}

.form-check::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.3), transparent);
  transition: left 0.6s;
}

.form-check:hover::before {
  left: 100%;
}

.form-check:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(243, 156, 18, 0.3);
  border-color: #e67e22;
}

.form-check-input {
  width: 18px;
  height: 18px;
  border: 2px solid #f39c12;
  border-radius: 3px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.form-check-input:checked {
  background: linear-gradient(135deg, #f39c12, #e67e22);
  border-color: #e67e22;
}

.form-check-label {
  font-weight: 600;
  color: #2c3e50;
  display: flex;
  align-items: center;
  gap: 6px;
  margin: 0;
  cursor: pointer;
  font-size: 0.9rem;
  text-transform: uppercase;
  letter-spacing: 0.3px;
}

.form-check-label i {
  color: #f39c12;
  font-size: 1rem;
}

/* ===== BUTTONS ===== */
.btn {
  padding: 14px 28px;
  border: none;
  border-radius: 12px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 10px;
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

.btn i {
  font-size: 1.1rem;
}

/* Button Variants */
.btn-primary {
  background: linear-gradient(135deg, #3498db, #2980b9);
  color: white;
  box-shadow: 0 4px 16px rgba(52, 152, 219, 0.3);
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(52, 152, 219, 0.4);
}

.btn-primary.active {
  background: linear-gradient(135deg, #e74c3c, #c0392b);
}

.btn-success {
  background: linear-gradient(135deg, #27ae60, #2ecc71, #58d68d);
  color: white;
  box-shadow: 0 4px 16px rgba(46, 204, 113, 0.3);
}

.btn-success:hover:not(:disabled) {
  transform: translateY(-3px);
  box-shadow: 0 8px 24px rgba(46, 204, 113, 0.4);
  background: linear-gradient(135deg, #229954, #27ae60, #52c41a);
}

.btn-secondary {
  background: linear-gradient(135deg, #6c757d, #adb5bd);
  color: white;
  box-shadow: 0 4px 16px rgba(108, 117, 125, 0.3);
}

.btn-secondary:hover:not(:disabled) {
  transform: translateY(-3px);
  box-shadow: 0 8px 24px rgba(108, 117, 125, 0.4);
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

.btn-info {
  background: linear-gradient(135deg, #17a2b8, #138496);
  color: white;
}

.btn-info:hover:not(:disabled) {
  background: linear-gradient(135deg, #138496, #117a8b);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(23, 162, 184, 0.4);
}

.btn-danger {
  background: linear-gradient(135deg, #dc3545, #c82333);
  color: white;
}

.btn-danger:hover:not(:disabled) {
  background: linear-gradient(135deg, #c82333, #bd2130);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(220, 53, 69, 0.4);
}

.btn-sm {
  padding: 8px 12px;
  font-size: 0.85rem;
}

/* ===== FORM ACTIONS ===== */
.form-actions {
  display: flex;
  gap: 16px;
  justify-content: flex-start;
  padding-top: 20px;
  border-top: 2px solid #f8f9fa;
}

/* ===== LIST SECTION ===== */
.list-controls {
  display: flex;
  gap: 15px;
  align-items: center;
}

.search-container {
  position: relative;
  flex: 1;
  max-width: 400px;
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

.search-input::placeholder {
  color: rgba(67, 63, 63, 0.7);
}

.search-icon {
  position: absolute;
  right: 15px;
  top: 50%;
  transform: translateY(-50%);
  color: rgba(13, 13, 13, 0.8);
  font-size: 1.1rem;
}

.clear-search {
  position: absolute;
  right: 40px;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  color: rgba(255, 255, 255, 0.8);
  cursor: pointer;
  padding: 5px;
  border-radius: 50%;
  transition: all 0.3s ease;
}

.clear-search:hover {
  background: rgba(255, 255, 255, 0.2);
  color: white;
}

/* ===== FILTERS ===== */
.vouchers-list-section .card-body {
  padding: 0;
}

.filters-section {
  margin: 0;
  padding: 25px;
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  border-bottom: 1px solid #dee2e6;
}

.filter-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 20px;
}

.filter-group {
  display: flex;
  flex-direction: column;
}

.filter-group label {
  font-weight: 600;
  color: #495057;
  margin-bottom: 8px;
  font-size: 0.9rem;
}

.input-group {
  display: flex;
  gap: 10px;
}

.input-group .form-control {
  flex: 1;
  padding: 10px 12px;
  font-size: 0.9rem;
}

/* ===== STATES ===== */
.loading-state, .empty-state {
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
}

/* ===== TABLE STYLES ===== */
.table-responsive {
  overflow-x: auto;
  border-radius: 0 0 12px 12px;
}

.vouchers-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  margin: 0;
}

.vouchers-table thead {
  background: linear-gradient(135deg, #495057 0%, #343a40 100%);
  position: sticky;
  top: 0;
  z-index: 10;
}

.vouchers-table thead th {
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

.vouchers-table tbody tr {
  transition: all 0.3s ease;
  border-bottom: 1px solid #f1f3f4;
}

.vouchers-table tbody tr:hover {
  background: linear-gradient(135deg, #f8f9ff 0%, #e3f2fd 100%);
  transform: translateX(3px);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.vouchers-table tbody tr.expired {
  background: linear-gradient(135deg, #fdf2f2 0%, #ffeaea 100%);
  opacity: 0.8;
}

.vouchers-table tbody tr.expired:hover {
  background: linear-gradient(135deg, #fce4ec 0%, #f8bbd9 100%);
}

.vouchers-table td {
  padding: 16px 15px;
  vertical-align: middle;
  border: none;
  font-size: 0.95rem;
}

/* ===== TABLE CELL STYLES ===== */
.voucher-code {
  font-weight: 600;
  min-width: 140px;
}

.code-container {
  display: flex;
  align-items: center;
  gap: 8px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 8px 12px;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 700;
  letter-spacing: 0.5px;
  box-shadow: 0 2px 4px rgba(102, 126, 234, 0.3);
}

.voucher-value {
  font-weight: 600;
  color: #27ae60;
  font-size: 1.05rem;
  min-width: 100px;
}

.type-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 6px 12px;
  border-radius: 15px;
  font-size: 0.8rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  min-width: 50px;
}

.type-badge.percent {
  background: linear-gradient(135deg, #ff9a9e 0%, #fecfef 100%);
  color: #ad1457;
  box-shadow: 0 2px 4px rgba(255, 154, 158, 0.3);
}

.type-badge.money {
  background: linear-gradient(135deg, #a8edea 0%, #fed6e3 100%);
  color: #00695c;
  box-shadow: 0 2px 4px rgba(168, 237, 234, 0.3);
}

.voucher-date {
  color: #495057;
  font-weight: 500;
  min-width: 110px;
}

.special-badge {
  display: inline-flex;
  align-items: center;
  gap: 5px;
  background: linear-gradient(135deg, #ffd89b 0%, #19547b 100%);
  color: white;
  padding: 6px 12px;
  border-radius: 15px;
  font-size: 0.8rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  box-shadow: 0 2px 4px rgba(255, 216, 155, 0.4);
}

.special-badge i {
  font-size: 0.9rem;
  color: #ffd700;
}

.quantity-number {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 6px 12px;
  border-radius: 15px;
  font-size: 0.85rem;
  font-weight: 700;
  box-shadow: 0 2px 4px rgba(102, 126, 234, 0.3);
}

.status-badge {
  display: inline-block;
  padding: 8px 14px;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  text-align: center;
  min-width: 90px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.status-badge.active {
  background: linear-gradient(135deg, #56ab2f 0%, #a8e6cf 100%);
  color: #155724;
}

.status-badge.pending {
  background: linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%);
  color: #856404;
}

.status-badge.expired {
  background: linear-gradient(135deg, #ff8a80 0%, #ffcdd2 100%);
  color: #721c24;
}
.status-badge.outofstock {
  background: linear-gradient(135deg, #e53935 0%, #f5b7b1 100%);
  color: #ffffff;
}

.action-buttons {
  display: flex;
  gap: 8px;
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

/* ===== RESPONSIVE DESIGN ===== */
@media (max-width: 1200px) {
  .form-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .form-row-2 {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .form-row-3 {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .voucher-management {
    padding: 15px;
  }
  
  .card-header {
    flex-direction: column;
    gap: 15px;
    text-align: center;
  }
  
  .list-controls {
    flex-direction: column;
    width: 100%;
  }
  
  .search-container {
    max-width: none;
  }
  
  .filter-grid {
    grid-template-columns: 1fr;
  }
  
  .form-grid {
    grid-template-columns: 1fr;
  }
  
  .form-row-2 {
    grid-template-columns: 1fr;
  }
  
  .form-row-3 {
    grid-template-columns: 1fr;
    gap: 15px;
  }
  
  .vouchers-table {
    font-size: 0.85rem;
  }
  
  .vouchers-table td {
    padding: 12px 8px;
  }
  
  .code-container {
    padding: 6px 10px;
    font-size: 0.8rem;
  }
  
  .action-buttons {
    flex-direction: column;
    gap: 4px;
  }
  
  .toast {
    min-width: 280px;
    margin: 0 10px 10px;
  }
}

@media (max-width: 480px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
  
  .form-row-2 {
    grid-template-columns: 1fr;
  }
  
  .form-row-3 {
    grid-template-columns: 1fr;
  }
  
  .form-actions {
    flex-direction: column;
  }
  
  .vouchers-table thead th,
  .vouchers-table td {
    padding: 8px 4px;
    font-size: 0.8rem;
  }
}
</style>