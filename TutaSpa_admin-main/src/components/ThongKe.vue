<template>
  <div class="container-fluid p-4">
    <!-- Header với gradient -->
    <div class="page-header mb-4">
      <div class="row align-items-center">
        <div class="col">
          <h2 class="page-title mb-0">
            <i class="fas fa-chart-line me-2"></i>
            ThỐNG KÊ & BÁO CÁO
          </h2>
          <p class="text-muted mb-0">Quản lý thu chi và chi phí</p>
        </div>
      </div>
    </div>
    
    <!-- Thống kê Thu Chi -->
    <div class="card stats-card mb-4">
      <div class="card-header bg-gradient-primary text-white">
        <h5 class="mb-0">
          <i class="fas fa-wallet me-2"></i>
          Thống kê Thu - Chi
        </h5>
      </div>
      <div class="card-body">
        <!-- Filter Controls -->
        <div class="row mb-4">
          <div class="col-lg-3 col-md-6 mb-3">
            <label for="thuChiFilterType" class="form-label fw-semibold">
              <i class="fas fa-filter me-1"></i>Kiểu thống kê
            </label>
            <select
              id="thuChiFilterType"
              v-model="thuChiFilterType"
              @change="layThuChi"
              class="form-select form-select-custom"
            >
              <option value="day">📅 Theo ngày</option>
              <option value="month">📊 Theo tháng</option>
              <option value="year">📈 Theo năm</option>
            </select>
          </div>
          
          <div class="col-lg-3 col-md-6 mb-3" v-if="thuChiFilterType === 'day'">
            <label for="selectedDay" class="form-label fw-semibold">
              <i class="fas fa-calendar-day me-1"></i>Chọn ngày
            </label>
            <input
              type="date"
              id="selectedDay"
              v-model="selectedDay"
              @change="layThuChi"
              class="form-control form-control-custom"
            />
          </div>
          
          <div class="col-lg-3 col-md-6 mb-3" v-if="thuChiFilterType === 'month'">
            <label for="selectedMonth" class="form-label fw-semibold">
              <i class="fas fa-calendar-alt me-1"></i>Chọn tháng
            </label>
            <input
              type="month"
              id="selectedMonth"
              v-model="selectedMonth"
              @change="layThuChi"
              class="form-control form-control-custom"
            />
          </div>
          
          <div class="col-lg-3 col-md-6 mb-3" v-if="thuChiFilterType === 'year'">
            <label for="selectedYear" class="form-label fw-semibold">
              <i class="fas fa-calendar me-1"></i>Chọn năm
            </label>
            <input
              type="number"
              id="selectedYear"
              v-model.number="selectedYear"
              min="2000"
              max="2099"
              @change="layThuChi"
              class="form-control form-control-custom"
            />
          </div>
        </div>

        <!-- Thu Chi Tables -->
        <div class="row">
          <!-- Thu Section -->
          <div class="col-lg-6 mb-4">
            <div class="stats-section revenue-section">
              <div class="section-header bg-success text-white">
                <h6 class="mb-0">
                  <i class="fas fa-arrow-up me-2"></i>
                  Thu nhập
                </h6>
              </div>
              <div class="table-responsive">
                <table class="table table-hover mb-0">
                  <thead class="table-light">
                    <tr>
                      <th>Loại thu</th>
                      <th class="text-end">Số tiền</th>
                      <th class="text-center">%</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="thu in thuChiData.thu" :key="thu.loaiThu" class="animate-row">
                      <td class="fw-medium">{{ thu.loaiThu }}</td>
                      <td class="text-end text-success fw-bold">
                        {{ thu.soTien?.toLocaleString() ?? 0 }}₫
                      </td>
                      <td class="text-center">
                        <span class="badge bg-success-subtle text-success">
                          {{ thu.phanTram ?? 0 }}%
                        </span>
                      </td>
                    </tr>
                    <tr v-if="thuChiData.thu.length === 0">
                      <td colspan="3" class="text-center text-muted py-4">
                        <i class="fas fa-inbox fa-2x mb-2 d-block text-muted"></i>
                        Không có dữ liệu
                      </td>
                    </tr>
                  </tbody>
                  <tfoot class="table-success">
                    <tr>
                      <td class="fw-bold">Tổng thu</td>
                      <td class="text-end fw-bold">
                        {{ thuChiData.tongThu?.toLocaleString() ?? 0 }}₫
                      </td>
                      <td></td>
                    </tr>
                  </tfoot>
                </table>
              </div>
            </div>
          </div>

          <!-- Chi Section -->
          <div class="col-lg-6 mb-4">
            <div class="stats-section expense-section">
              <div class="section-header bg-warning text-dark">
                <h6 class="mb-0">
                  <i class="fas fa-arrow-down me-2"></i>
                  Chi tiêu
                </h6>
              </div>
              <div class="table-responsive">
                <table class="table table-hover mb-0">
                  <thead class="table-light">
                    <tr>
                      <th>Loại chi</th>
                      <th class="text-end">Số tiền</th>
                      <th class="text-center">%</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="chi in thuChiData.chi" :key="chi.loaiChi" class="animate-row">
                      <td class="fw-medium">{{ chi.loaiChi }}</td>
                      <td class="text-end text-danger fw-bold">
                        {{ chi.soTien?.toLocaleString() ?? 0 }}₫
                      </td>
                      <td class="text-center">
                        <span class="badge bg-warning-subtle text-warning">
                          {{ chi.phanTram ?? 0 }}%
                        </span>
                      </td>
                    </tr>
                    <tr v-if="thuChiData.chi.length === 0">
                      <td colspan="3" class="text-center text-muted py-4">
                        <i class="fas fa-inbox fa-2x mb-2 d-block text-muted"></i>
                        Không có dữ liệu
                      </td>
                    </tr>
                  </tbody>
                  <tfoot class="table-warning">
                    <tr>
                      <td class="fw-bold">Tổng chi</td>
                      <td class="text-end fw-bold">
                        {{ thuChiData.tongChi?.toLocaleString() ?? 0 }}₫
                      </td>
                      <td></td>
                    </tr>
                  </tfoot>
                </table>
              </div>
            </div>
          </div>
        </div>

        <!-- Summary Cards -->
        <div class="row mt-4">
          <div class="col-md-4">
            <div class="summary-card bg-success-subtle">
              <div class="summary-icon text-success">
                <i class="fas fa-plus-circle"></i>
              </div>
              <div class="summary-content">
                <h6 class="text-success mb-1">Tổng thu</h6>
                <h4 class="mb-0 text-success">{{ thuChiData.tongThu?.toLocaleString() ?? 0 }}₫</h4>
              </div>
            </div>
          </div>
          <div class="col-md-4">
            <div class="summary-card bg-danger-subtle">
              <div class="summary-icon text-danger">
                <i class="fas fa-minus-circle"></i>
              </div>
              <div class="summary-content">
                <h6 class="text-danger mb-1">Tổng chi</h6>
                <h4 class="mb-0 text-danger">{{ thuChiData.tongChi?.toLocaleString() ?? 0 }}₫</h4>
              </div>
            </div>
          </div>
          <div class="col-md-4">
            <div class="summary-card" :class="netProfit >= 0 ? 'bg-primary-subtle' : 'bg-warning-subtle'">
              <div class="summary-icon" :class="netProfit >= 0 ? 'text-primary' : 'text-warning'">
                <i class="fas fa-chart-line"></i>
              </div>
              <div class="summary-content">
                <h6 class="mb-1" :class="netProfit >= 0 ? 'text-primary' : 'text-warning'">Lợi nhuận ròng</h6>
                <h4 class="mb-0" :class="netProfit >= 0 ? 'text-primary' : 'text-warning'">
                  {{ netProfit.toLocaleString() }}₫
                </h4>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Danh sách chi phí -->
    <div class="card expenses-card">
      <div class="card-header bg-gradient-info text-white d-flex justify-content-between align-items-center">
        <h5 class="mb-0">
          <i class="fas fa-receipt me-2"></i>
          Danh sách chi phí
        </h5>
        <button
          class="btn btn-light btn-add-expense"
          data-bs-toggle="modal"
          data-bs-target="#addExpenseModal"
        >
          <i class="fas fa-plus me-2"></i>
          Thêm chi phí mới
        </button>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th class="text-center" style="width: 60px">#</th>
                <th>Loại chi phí</th>
                <th class="text-end" style="width: 140px">Số tiền</th>
                <th class="text-center" style="width: 120px">Ngày</th>
                <th style="width: 200px">Ghi chú</th>
                <th class="text-center" style="width: 160px">Hành động</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(expense, index) in expenses" :key="expense.expenseId" class="animate-row">
                <td class="text-center text-muted">{{ index + 1 }}</td>
                <td class="fw-medium">
                  <div class="d-flex align-items-center">
                    <div class="expense-icon me-2">
                      <i class="fas fa-tag text-primary"></i>
                    </div>
                    {{ expense.expenseType }}
                  </div>
                </td>
                <td class="text-end">
                  <span class="amount-display text-danger fw-bold">
                    {{ expense.amount.toLocaleString() }}₫
                  </span>
                </td>
                <td class="text-center">
                  <span class="date-badge">
                    {{ formatDate(expense.date) }}
                  </span>
                </td>
                <td>
                  <span class="note-text" :title="expense.note || '—'">
                    {{ expense.note || "—" }}
                  </span>
                </td>
                <td class="text-center">
                  <div class="btn-group" role="group">
                    <button
                      class="btn btn-outline-primary btn-sm"
                      data-bs-toggle="modal"
                      data-bs-target="#editExpenseModal"
                      @click="openEditExpenseModal(expense)"
                      title="Chỉnh sửa"
                    >
                      <i class="fas fa-edit"></i>
                    </button>
                    <button
                      class="btn btn-outline-danger btn-sm"
                      @click="selectExpenseToDelete(expense.expenseId)"
                      data-bs-toggle="modal"
                      data-bs-target="#deleteExpenseModal"
                      title="Xóa"
                    >
                      <i class="fas fa-trash"></i>
                    </button>
                  </div>
                </td>
              </tr>
              <tr v-if="expenses.length === 0">
                <td colspan="6" class="text-center text-muted py-5">
                  <div class="empty-state">
                    <i class="fas fa-receipt fa-3x mb-3 text-muted"></i>
                    <h6 class="text-muted">Chưa có chi phí nào</h6>
                    <p class="text-muted mb-0">Hãy thêm chi phí đầu tiên của bạn</p>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Modal thêm chi phí -->
    <div
      class="modal fade"
      id="addExpenseModal"
      tabindex="-1"
      aria-labelledby="addExpenseModalLabel"
      aria-hidden="true"
    >
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header bg-primary text-white">
            <h5 class="modal-title" id="addExpenseModalLabel">
              <i class="fas fa-plus-circle me-2"></i>
              Thêm chi phí mới
            </h5>
            <button
              type="button"
              class="btn-close btn-close-white"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="createExpense">
              <div class="row">
                <div class="col-md-6 mb-3">
                  <label for="expenseType" class="form-label fw-semibold">
                    <i class="fas fa-tag me-1 text-primary"></i>Loại chi phí
                  </label>
                  <input
                    type="text"
                    class="form-control form-control-custom"
                    id="expenseType"
                    v-model="newExpense.expenseType"
                    placeholder="Ví dụ: Ăn uống, Xăng xe..."
                    required
                  />
                </div>
                <div class="col-md-6 mb-3">
                  <label for="amount" class="form-label fw-semibold">
                    <i class="fas fa-dollar-sign me-1 text-success"></i>Số tiền
                  </label>
                  <input
                    type="number"
                    class="form-control form-control-custom"
                    id="amount"
                    v-model.number="newExpense.amount"
                    placeholder="0"
                    min="0"
                    required
                  />
                </div>
              </div>
              <div class="mb-3">
                <label for="date" class="form-label fw-semibold">
                  <i class="fas fa-calendar me-1 text-info"></i>Ngày
                </label>
                <input
                  type="date"
                  class="form-control form-control-custom"
                  id="date"
                  v-model="newExpense.date"
                  required
                />
              </div>
              <div class="mb-3">
                <label for="note" class="form-label fw-semibold">
                  <i class="fas fa-sticky-note me-1 text-warning"></i>Ghi chú
                </label>
                <textarea
                  class="form-control form-control-custom"
                  id="note"
                  v-model="newExpense.note"
                  rows="3"
                  placeholder="Ghi chú thêm về chi phí này..."
                ></textarea>
              </div>
              <div class="d-grid">
                <button type="submit" class="btn btn-primary btn-lg">
                  <i class="fas fa-save me-2"></i>Thêm chi phí
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal sửa chi phí -->
    <div
      class="modal fade"
      id="editExpenseModal"
      tabindex="-1"
      aria-labelledby="editExpenseModalLabel"
      aria-hidden="true"
    >
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header bg-warning text-dark">
            <h5 class="modal-title" id="editExpenseModalLabel">
              <i class="fas fa-edit me-2"></i>
              Sửa chi phí
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="updateExpense">
              <input type="hidden" v-model="editExpense.expenseId" />
              <div class="row">
                <div class="col-md-6 mb-3">
                  <label for="editExpenseType" class="form-label fw-semibold">
                    <i class="fas fa-tag me-1 text-primary"></i>Loại chi phí
                  </label>
                  <input
                    type="text"
                    class="form-control form-control-custom"
                    id="editExpenseType"
                    v-model="editExpense.expenseType"
                    required
                  />
                </div>
                <div class="col-md-6 mb-3">
                  <label for="editAmount" class="form-label fw-semibold">
                    <i class="fas fa-dollar-sign me-1 text-success"></i>Số tiền
                  </label>
                  <input
                    type="number"
                    class="form-control form-control-custom"
                    id="editAmount"
                    v-model.number="editExpense.amount"
                    min="0"
                    required
                  />
                </div>
              </div>
              <div class="mb-3">
                <label for="editDate" class="form-label fw-semibold">
                  <i class="fas fa-calendar me-1 text-info"></i>Ngày
                </label>
                <input
                  type="date"
                  class="form-control form-control-custom"
                  id="editDate"
                  v-model="editExpense.date"
                  required
                />
              </div>
              <div class="mb-3">
                <label for="editNote" class="form-label fw-semibold">
                  <i class="fas fa-sticky-note me-1 text-warning"></i>Ghi chú
                </label>
                <textarea
                  class="form-control form-control-custom"
                  id="editNote"
                  v-model="editExpense.note"
                  rows="3"
                ></textarea>
              </div>
              <div class="d-grid">
                <button type="submit" class="btn btn-warning btn-lg text-dark">
                  <i class="fas fa-sync-alt me-2"></i>Cập nhật
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal xác nhận xóa chi phí -->
    <div
      class="modal fade"
      id="deleteExpenseModal"
      tabindex="-1"
      aria-labelledby="deleteExpenseModalLabel"
      aria-hidden="true"
    >
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header bg-danger text-white">
            <h5 class="modal-title" id="deleteExpenseModalLabel">
              <i class="fas fa-exclamation-triangle me-2"></i>
              Xác nhận xóa chi phí
            </h5>
            <button
              type="button"
              class="btn-close btn-close-white"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body text-center py-4">
            <div class="mb-3">
              <i class="fas fa-trash-alt fa-3x text-danger mb-3"></i>
              <h6>Bạn có chắc chắn muốn xóa chi phí này không?</h6>
              <p class="text-muted mb-0">Hành động này không thể hoàn tác.</p>
            </div>
          </div>
          <div class="modal-footer justify-content-center">
            <button
              type="button"
              class="btn btn-secondary"
              data-bs-dismiss="modal"
            >
              <i class="fas fa-times me-1"></i>Hủy
            </button>
            <button
              type="button"
              class="btn btn-danger"
              @click="confirmDeleteExpense"
            >
              <i class="fas fa-check me-1"></i>Xóa
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import axios from "axios";
import apiClient from "../utils/axiosClient";
import dayjs from "dayjs";

const danhSachHoaDon = ref([]);
const thuChiData = ref({ thu: [], chi: [], tongThu: 0, tongChi: 0 });
const expenses = ref([]);
const newExpense = ref({
  expenseType: "",
  amount: 0,
  date: dayjs().format("YYYY-MM-DD"),
  note: "",
});
const editExpense = ref({
  expenseId: 0,
  expenseType: "",
  amount: 0,
  date: "",
  note: "",
});
const thuChiFilterType = ref("month");
const selectedDay = ref(new Date().toISOString().split("T")[0]);
const selectedMonth = ref(new Date().toISOString().slice(0, 7));
const selectedYear = ref(new Date().getFullYear());
const expenseIdToDelete = ref(null);

// Computed property for net profit
const netProfit = computed(() => {
  return (thuChiData.value.tongThu ?? 0) - (thuChiData.value.tongChi ?? 0);
});

// Lấy dữ liệu thu chi
const layThuChi = async () => {
  try {
    let url = "/ThongKe/ThuChi";
    let params = {};
    if (thuChiFilterType.value === "day") {
      url = "/ThongKe/ThuChiByDay";
      params.day = selectedDay.value;
    } else if (thuChiFilterType.value === "month") {
      url = "/ThongKe/ThuChiByMonth";
      const [year, month] = selectedMonth.value.split("-");
      params.month = parseInt(month);
      params.year = parseInt(year);
    } else if (thuChiFilterType.value === "year") {
      url = "/ThongKe/ThuChiByYear";
      params.year = selectedYear.value;
    }
    const res = await apiClient.get(url, { params });
    thuChiData.value = res;
    console.log("Dữ liệu ThuChi:", res);
  } catch (err) {
    console.error("Lỗi lấy dữ liệu thu chi:", err);
  }
};

// Lấy danh sách chi phí
const layDanhSachChiPhi = async () => {
  try {
    const res = await apiClient.get("/ThongKe/Expense");
    expenses.value = res;
    console.log("Danh sách chi phí:", res);
  } catch (err) {
    console.error("Lỗi lấy danh sách chi phí:", err);
  }
};

// Thêm chi phí mới
const createExpense = async () => {
  try {
    await apiClient.post("/ThongKe/Expense", newExpense.value);
    alert("Thêm chi phí thành công");
    newExpense.value = {
      expenseType: "",
      amount: 0,
      date: dayjs().format("YYYY-MM-DD"),
      note: "",
    };
    await layDanhSachChiPhi();
    await layThuChi();
    document.querySelector("#addExpenseModal .btn-close").click();
  } catch (err) {
    console.error("Lỗi khi thêm chi phí:", err);
    alert("Không thể thêm chi phí.");
  }
};

// Mở modal sửa chi phí
const openEditExpenseModal = (expense) => {
  editExpense.value = {
    expenseId: expense.expenseId || 0,
    expenseType: expense.expenseType || "",
    amount: expense.amount || 0,
    date: expense.date
      ? new Date(expense.date).toISOString().split("T")[0]
      : new Date().toISOString().split("T")[0],
    note: expense.note || "",
  };

  console.log("editExpense:", editExpense.value);

  const modal = new bootstrap.Modal(
    document.getElementById("editExpenseModal")
  );
  modal.show();
};

// Cập nhật chi phí
const updateExpense = async () => {
  try {
    await apiClient.put(
      `/ThongKe/Expense/${editExpense.value.expenseId}`,
      editExpense.value
    );
    alert("Cập nhật chi phí thành công");
    await layDanhSachChiPhi();
    await layThuChi();
    document.querySelector("#editExpenseModal .btn-close").click();
  } catch (err) {
    console.error("Lỗi khi cập nhật chi phí:", err);
    alert("Không thể cập nhật chi phí.");
  }
};

// Chọn chi phí để xóa
const selectExpenseToDelete = (expenseId) => {
  expenseIdToDelete.value = expenseId;
};

// Xác nhận xóa chi phí
const confirmDeleteExpense = async () => {
  try {
    await apiClient.delete(`/ThongKe/Expense/${expenseIdToDelete.value}`);
    alert("Xóa chi phí thành công");
    await layDanhSachChiPhi();
    await layThuChi();
    document.querySelector("#deleteExpenseModal .btn-close").click();
  } catch (err) {
    console.error("Lỗi khi xóa chi phí:", err);
    alert("Không thể xóa chi phí.");
  }
};

// Xóa chi phí
const deleteExpense = async (expenseId) => {
  selectExpenseToDelete(expenseId);
};

// Tải hóa đơn
const taiHoaDon = async (hoaDonID) => {
  try {
    const response = await apiClient.get(`/ThanhToan/xuat-hoadon/${hoaDonID}`, {
      responseType: "blob",
    });
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement("a");
    link.href = url;
    link.setAttribute("download", `HoaDon_${hoaDonID}.pdf`);
    document.body.appendChild(link);
    link.click();
    link.remove();
  } catch (err) {
    console.error("Lỗi khi tải hóa đơn:", err);
    alert("Không thể tải hóa đơn.");
  }
};

// Format ngày giờ
const formatDateTime = (dateStr) => {
  const date = new Date(dateStr);
  return `${date.toLocaleTimeString([], {
    hour: "2-digit",
    minute: "2-digit",
  })} - ${date.toLocaleDateString("vi-VN")}`;
};

// Format ngày
const formatDate = (dateStr) => {
  const date = new Date(dateStr);
  return date.toLocaleDateString("vi-VN");
};

// Khởi tạo dữ liệu khi component được mount
onMounted(() => {
  layThuChi();
  layDanhSachChiPhi();
});
</script>

<style scoped>
/* Main Styles */
.page-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 30px;
  box-shadow: 0 8px 32px rgba(102, 126, 234, 0.15);
}

.page-title {
  color: white;
  font-weight: 600;
  font-size: 1.75rem;
}

/* Card Styles */
.card {
  border: none;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
}

.card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.12);
}

.stats-card .card-header {
  border-radius: 12px 12px 0 0;
  border: none;
  padding: 20px;
}

.bg-gradient-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.bg-gradient-info {
  background: linear-gradient(135deg, #48cae4 0%, #0077b6 100%);
}

/* Form Controls */
.form-control-custom,
.form-select-custom {
  border: 2px solid #e9ecef;
  border-radius: 8px;
  padding: 12px 16px;
  transition: all 0.3s ease;
  font-size: 14px;
}

.form-control-custom:focus,
.form-select-custom:focus {
  border-color: #667eea;
  box-shadow: 0 0 0 0.2rem rgba(102, 126, 234, 0.15);
}

.form-label {
  color: #495057;
  font-weight: 500;
  margin-bottom: 8px;
}

/* Stats Section */
.stats-section {
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
}

.stats-section:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.12);
}

.section-header {
  padding: 15px 20px;
  font-weight: 600;
}

.revenue-section {
  border-left: 4px solid #28a745;
}

.expense-section {
  border-left: 4px solid #ffc107;
}

/* Table Styles */
.table {
  margin-bottom: 0;
}

.table th {
  border-top: none;
  font-weight: 600;
  color: #495057;
  font-size: 13px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  padding: 15px 12px;
}

.table td {
  vertical-align: middle;
  padding: 12px;
  border-color: #f1f3f4;
}

.table-hover tbody tr:hover {
  background-color: #f8f9fa;
}

/* Animate rows */
.animate-row {
  animation: fadeInUp 0.5s ease;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Badge Styles */
.badge {
  font-size: 11px;
  font-weight: 500;
  padding: 6px 10px;
  border-radius: 6px;
}

.bg-success-subtle {
  background-color: rgba(40, 167, 69, 0.1);
  color: #28a745;
}

.bg-warning-subtle {
  background-color: rgba(255, 193, 7, 0.1);
  color: #ffc107;
}

.bg-danger-subtle {
  background-color: rgba(220, 53, 69, 0.1);
  color: #dc3545;
}

.bg-primary-subtle {
  background-color: rgba(102, 126, 234, 0.1);
  color: #667eea;
}

/* Summary Cards */
.summary-card {
  background: white;
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
}

.summary-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.12);
}

.summary-icon {
  font-size: 2rem;
  margin-right: 15px;
  width: 60px;
  text-align: center;
}

.summary-content h4 {
  font-weight: 700;
  font-size: 1.5rem;
}

.summary-content h6 {
  font-size: 13px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  font-weight: 600;
}

/* Button Styles */
.btn-add-expense {
  border-radius: 8px;
  padding: 8px 16px;
  font-weight: 500;
  transition: all 0.3s ease;
}

.btn-add-expense:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.btn-sm {
  border-radius: 6px;
  padding: 6px 12px;
  font-size: 12px;
}

.btn-outline-primary:hover,
.btn-outline-danger:hover {
  transform: translateY(-1px);
}

/* Modal Styles */
.modal-content {
  border: none;
  border-radius: 12px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.15);
}

.modal-header {
  border-radius: 12px 12px 0 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
  padding: 20px 30px;
}

.modal-body {
  padding: 30px;
}

.modal-footer {
  border-top: 1px solid #e9ecef;
  padding: 20px 30px;
}

/* Expense List Styles */
.expense-icon {
  width: 30px;
  height: 30px;
  background: rgba(102, 126, 234, 0.1);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.amount-display {
  font-family: 'Courier New', monospace;
  font-size: 14px;
  font-weight: 600;
}

.date-badge {
  background: #f8f9fa;
  padding: 4px 8px;
  border-radius: 6px;
  font-size: 12px;
  color: #6c757d;
  font-weight: 500;
}

.note-text {
  display: block;
  max-width: 180px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  color: #6c757d;
  font-size: 13px;
}

/* Empty State */
.empty-state {
  padding: 40px 20px;
}

.empty-state i {
  opacity: 0.3;
}

/* Responsive */
@media (max-width: 768px) {
  .page-header {
    padding: 15px;
    text-align: center;
  }
  
  .page-title {
    font-size: 1.5rem;
  }
  
  .summary-card {
    flex-direction: column;
    text-align: center;
  }
  
  .summary-icon {
    margin-right: 0;
    margin-bottom: 10px;
  }
  
  .modal-body,
  .modal-header,
  .modal-footer {
    padding: 20px;
  }
  
  .btn-group {
    flex-direction: column;
  }
  
  .btn-group .btn {
    margin-bottom: 5px;
  }
}

/* Custom scrollbar */
.table-responsive::-webkit-scrollbar {
  height: 8px;
}

.table-responsive::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 4px;
}

.table-responsive::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 4px;
}

.table-responsive::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}

/* Loading animation */
@keyframes pulse {
  0% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
  100% {
    opacity: 1;
  }
}

.loading {
  animation: pulse 1.5s ease-in-out infinite;
}

/* Success/Error states */
.text-success {
  color: #28a745 !important;
}

.text-danger {
  color: #dc3545 !important;
}

.text-warning {
  color: #ffc107 !important;
}

.text-info {
  color: #17a2b8 !important;
}

.text-primary {
  color: #667eea !important;
}
</style>
