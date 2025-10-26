<!-- BangGiaDichVu.vue -->
<template>
  <div></div>
  <div class="banggia-management">
    <!-- Header -->
    <div class="header">
      <h1 class="title">
        <i class="fas fa-tags"></i>
        Quản lý Bảng Giá Dịch Vụ
      </h1>
    </div>

    <!-- Add/Update Price Section -->
    <div class="card add-banggia-section">
      <div class="card-header">
        <h2 class="section-title">
          <i class="fas fa-plus-circle"></i>
          {{ isEditing ? "Cập nhật Giá" : "Thêm Giá Mới" }}
        </h2>
      </div>
      <div class="card-body">
        <form @submit.prevent="saveBangGia" class="banggia-form">
          <div class="form-grid">
            <div class="form-group">
              <label for="dichVu"
                >Dịch vụ <span class="required">*</span></label
              >
              <select
                id="dichVu"
                v-model="bangGia.dichVuID"
                class="form-control"
                required
              >
                <option disabled value="">-- Chọn dịch vụ --</option>
                <option
                  v-for="dv in dichVus"
                  :key="dv.dichVuID"
                  :value="dv.dichVuID"
                >
                  {{ dv.tenDichVu }}
                </option>
              </select>
            </div>
            <div class="form-group">
              <label for="thoiLuong">Thời lượng (phút)</label>
              <input
                id="thoiLuong"
                v-model="bangGia.thoiLuong"
                type="number"
                class="form-control"
                min="0"
                :disabled="bangGia.kieuTinhGia === 1"
                placeholder="Nhập thời lượng"
              />
            </div>
            <div class="form-group">
              <label for="gia">Giá (VND) <span class="required">*</span></label>
              <input
                id="gia"
                v-model="bangGia.gia"
                type="number"
                class="form-control"
                min="0"
                required
                placeholder="Nhập giá"
              />
            </div>
            <div class="form-group">
              <label for="kieuTinhGia">Kiểu tính giá</label>
              <select
                id="kieuTinhGia"
                v-model="bangGia.kieuTinhGia"
                class="form-control"
              >
                <option :value="0">Theo thời gian</option>
                <option :value="1">Theo quy trình</option>
              </select>
            </div>
          </div>
          <div class="form-actions">
            <button class="btn btn-success" type="submit">
              <i class="fas fa-save"></i>
              {{ isEditing ? "Cập nhật" : "Thêm mới" }}
            </button>
            <button class="btn btn-secondary" type="button" @click="resetForm">
              <i class="fas fa-undo"></i>
              Hủy
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Price List Section -->
    <div class="card banggia-list-section">
      <div class="card-header">
        <h2 class="section-title">
          <i class="fas fa-table"></i>
          Danh sách Bảng Giá
        </h2>
      </div>
      <div class="card-body">
        <div v-if="!bangGias.length" class="empty-state">
          <i class="fas fa-table"></i>
          <p>Chưa có bảng giá nào</p>
        </div>
        <div v-else class="table-responsive">
          <table class="banggia-table">
            <thead>
              <tr>
                <th>Dịch vụ</th>
                <th>Thời lượng</th>
                <th>Giá</th>
                <th>Kiểu tính giá</th>
                <th>Hiển thị</th>
                <th>Hành động</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in bangGias" :key="item.id" class="banggia-row">
                <td class="banggia-name">
                  <div class="name-container">
                    {{ item.tenDichVu || "Không rõ" }}
                  </div>
                </td>
                <td>
                  {{ item.kieuTinhGia === 0 ? item.thoiLuong + " phút" : "-" }}
                </td>
                <td>{{ formatCurrency(item.gia) }}</td>
                <td>
                  <span
                    class="role-badge"
                    :class="
                      item.kieuTinhGia === 0 ? 'time-based' : 'process-based'
                    "
                  >
                    {{
                      item.kieuTinhGia === 0
                        ? "Theo thời gian"
                        : "Theo quy trình"
                    }}
                  </span>
                </td>
                <td>
                  <label class="status-switch">
                    <input
                      type="checkbox"
                      :checked="item.isVisible"
                      @change="toggleHienGia(item.id)"
                    />
                    <span class="slider"></span>
                  </label>
                </td>
                <td class="banggia-actions">
                  <div class="action-buttons">
                    <button
                      class="btn btn-sm btn-info"
                      @click="editBangGia(item)"
                      title="Chỉnh sửa"
                    >
                      <i class="fas fa-edit"></i>
                    </button>
                    <!-- <button class="btn btn-sm btn-danger" @click="deleteBangGia(item.id)" title="Xóa">
                      <i class="fas fa-trash"></i>
                    </button> -->
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import axiosClient from "../utils/axiosClient";
const bangGias = ref([]);
const dichVus = ref([]);
const isEditing = ref(false);

const bangGia = ref({
  id: 0,
  dichVuID: "",
  thoiLuong: null,
  gia: 0,
  kieuTinhGia: 0,
});

const fetchBangGias = async () => {
  try {
    const res = await axiosClient.get("BangGiaDichVu");
    bangGias.value = res;
  } catch (err) {
    console.error("Lỗi khi lấy danh sách bảng giá:", err);
  }
};

const fetchDichVus = async () => {
  try {
    const res = await axiosClient.get("DichVu/all");
    dichVus.value = res;
  } catch (err) {
    console.error("Lỗi khi lấy danh sách dịch vụ:", err);
  }
};

const saveBangGia = async () => {
  try {
    if (isEditing.value) {
      await axiosClient.put(
        `BangGiaDichVu/UpdateGiaDichVu/${bangGia.value.id}`,
        bangGia.value
      );
    } else {
      await axiosClient.post("BangGiaDichVu/AddGiaDichVu", bangGia.value);
    }
    resetForm();
    fetchBangGias();
  } catch (err) {
    console.error("Lỗi khi lưu bảng giá:", err);
  }
};

const toggleHienGia = async (id) => {
  try {
    await axiosClient.put(`BangGiaDichVu/ToggleGiaDichVu/${id}`);
    fetchBangGias();
  } catch (err) {
    console.error("Lỗi khi bật/tắt hiển thị:", err);
  }
};

const editBangGia = (item) => {
  bangGia.value = { ...item };
  isEditing.value = true;
};

const deleteBangGia = async (id) => {
  if (!confirm("Bạn có chắc muốn xoá?")) return;
  try {
    await axiosClient.delete(`BangGiaDichVu/DeleteGiaDichVu/${id}`);
    fetchBangGias();
  } catch (err) {
    console.error("Lỗi khi xoá bảng giá:", err);
  }
};

const resetForm = () => {
  bangGia.value = {
    id: 0,
    dichVuID: "",
    thoiLuong: null,
    gia: 0,
    kieuTinhGia: 0,
  };
  isEditing.value = false;
};

const formatCurrency = (value) =>
  new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(
    value
  );

onMounted(() => {
  fetchBangGias();
  fetchDichVus();
});
</script>

<style scoped>
.banggia-management {
  max-width: 1400px;
  margin: 0 auto;
  padding: 20px;
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
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

.banggia-form {
  max-width: none;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
  margin-bottom: 25px;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 8px;
  font-size: 0.95rem;
}

.required {
  color: #e74c3c;
}

.form-control {
  padding: 12px 15px;
  border: 2px solid #e1e8ed;
  border-radius: 8px;
  font-size: 1rem;
  transition: all 0.3s ease;
  background: #fafbfc;
}

.form-control:focus {
  outline: none;
  border-color: #3498db;
  background: white;
  box-shadow: 0 0 0 3px rgba(52, 152, 219, 0.1);
}

.form-actions {
  display: flex;
  gap: 15px;
  justify-content: flex-start;
}

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
  transition: all 0.3s ease;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-success {
  background: linear-gradient(135deg, #27ae60, #229954);
  color: white;
}

.btn-success:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(39, 174, 96, 0.4);
}

.btn-secondary {
  background: linear-gradient(135deg, #95a5a6, #7f8c8d);
  color: white;
}

.btn-secondary:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(149, 165, 166, 0.4);
}

.btn-info {
  background: linear-gradient(135deg, #3498db, #2980b9);
  color: white;
}

.btn-info:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(52, 152, 219, 0.4);
}

.empty-state {
  text-align: center;
  padding: 60px 20px;
  color: #7f8c8d;
  font-size: 1.1rem;
}

.empty-state i {
  font-size: 4rem;
  margin-bottom: 20px;
  display: block;
  color: #bdc3c7;
}

.table-responsive {
  overflow-x: auto;
}

.banggia-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 20px;
}

.banggia-table th {
  background: linear-gradient(135deg, #34495e, #2c3e50);
  color: white;
  padding: 15px 12px;
  text-align: left;
  font-weight: 600;
  font-size: 0.95rem;
  border-bottom: 3px solid #3498db;
}

.banggia-table td {
  padding: 15px 12px;
  border-bottom: 1px solid #ecf0f1;
  vertical-align: middle;
}

.banggia-row:hover {
  background: #f8f9fa;
}

.name-container {
  display: flex;
  align-items: center;
  gap: 10px;
}

.role-badge {
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.role-badge.time-based {
  background: linear-gradient(135deg, #f39c12, #e67e22);
  color: white;
}

.role-badge.process-based {
  background: linear-gradient(135deg, #9b59b6, #8e44ad);
  color: white;
}

.status-switch {
  position: relative;
  display: inline-block;
  width: 50px;
  height: 24px;
}

.status-switch input {
  opacity: 0;
  width: 0;
  height: 0;
}

.slider {
  position: absolute;
  cursor: pointer;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: #ccc;
  transition: 0.4s;
  border-radius: 24px;
}

.slider:before {
  position: absolute;
  content: "";
  height: 18px;
  width: 18px;
  left: 3px;
  bottom: 3px;
  background-color: white;
  transition: 0.4s;
  border-radius: 50%;
}

input:checked + .slider {
  background: linear-gradient(135deg, #27ae60, #229954);
}

input:checked + .slider:before {
  transform: translateX(26px);
}

.action-buttons {
  display: flex;
  gap: 8px;
  align-items: center;
}

.btn-sm {
  padding: 8px 12px;
  font-size: 0.85rem;
}

/* Responsive Design */
@media (max-width: 768px) {
  .banggia-management {
    padding: 15px;
  }

  .title {
    font-size: 2rem;
  }

  .form-grid {
    grid-template-columns: 1fr;
  }

  .card-header {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }

  .banggia-table {
    font-size: 0.9rem;
  }

  .banggia-table th,
  .banggia-table td {
    padding: 10px 8px;
  }

  .action-buttons {
    flex-direction: column;
    gap: 5px;
  }
}

@media (max-width: 480px) {
  .table-responsive {
    font-size: 0.8rem;
  }

  .banggia-table th,
  .banggia-table td {
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
}
</style>
