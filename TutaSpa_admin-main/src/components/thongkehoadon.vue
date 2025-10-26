<template>
  <div class="container-fluid p-4">
    <!-- Header với gradient và shadow -->
    <div class="management-header mb-4">
      <div class="d-flex justify-content-between align-items-center">
        <div class="header-title">
          <h2 class="mb-1 text-white">
            <i class="fas fa-chart-line me-2"></i>Quản lý Hệ thống
          </h2>
          <p class="text-light mb-0 opacity-75">Theo dõi lịch hẹn và hóa đơn</p>
        </div>

        <!-- Tab Navigation với design hiện đại -->
        <div class="nav-pills-custom">
          <button
            class="btn-tab"
            :class="{ active: activeTab === 'lichHen' }"
            @click="
              activeTab = 'lichHen';
              fetchDatLich();
            "
          >
            <i class="fas fa-calendar-check me-2"></i>
            Lịch hẹn
            <span class="badge-count" v-if="datLichs.length">{{
              datLichs.length
            }}</span>
          </button>
          <button
            class="btn-tab"
            :class="{ active: activeTab === 'hoaDon' }"
            @click="
              activeTab = 'hoaDon';
              fetchHoaDon();
            "
          >
            <i class="fas fa-receipt me-2"></i>
            Hóa đơn
            <span class="badge-count" v-if="hoaDons.length">{{
              hoaDons.length
            }}</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Filter Section với card design -->
    <div class="card filter-card mb-4">
      <div class="card-body">
        <div class="row g-3 align-items-end">
          <div class="col-md-3">
            <label class="form-label text-muted small">Từ ngày</label>
            <input
              type="date"
              v-model="fromDate"
              class="form-control form-control-modern"
              @change="activeTab === 'lichHen' ? fetchDatLich() : fetchHoaDon()"
            />
          </div>
          <div class="col-md-3">
            <label class="form-label text-muted small">Đến ngày</label>
            <input
              type="date"
              v-model="toDate"
              class="form-control form-control-modern"
              @change="activeTab === 'lichHen' ? fetchDatLich() : fetchHoaDon()"
            />
          </div>
          <div class="col-md-4">
            <label class="form-label text-muted small">Số điện thoại</label>
            <div class="input-group">
              <span class="input-group-text bg-light border-end-0">
                <i class="fas fa-phone text-muted"></i>
              </span>
              <input
                type="text"
                v-model="phoneFilter"
                class="form-control form-control-modern border-start-0"
                placeholder="Nhập số điện thoại để tìm kiếm..."
                @input="
                  activeTab === 'lichHen' ? fetchDatLich() : fetchHoaDon()
                "
              />
            </div>
          </div>
          <div class="col-md-2">
            <button
              class="btn btn-refresh w-100"
              @click="activeTab === 'lichHen' ? fetchDatLich() : fetchHoaDon()"
            >
              <i class="fas fa-sync-alt me-1"></i>Làm mới
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Alert Messages với animation -->
    <div
      v-if="error.lichHen || error.hoaDon"
      class="alert alert-danger alert-dismissible fade show"
      role="alert"
    >
      <i class="fas fa-exclamation-triangle me-2"></i>
      {{ error.lichHen || error.hoaDon }}
      <button
        type="button"
        class="btn-close"
        @click="
          error.lichHen = null;
          error.hoaDon = null;
        "
      ></button>
    </div>

    <!-- TAB CONTENT -->
    <div class="tab-content">
      <!-- LỊCH HẸN TAB -->
      <div v-show="activeTab === 'lichHen'" class="tab-pane fade show active">
        <div class="card data-card">
          <div class="card-header bg-transparent">
            <h5 class="card-title mb-0">
              <i class="fas fa-calendar-alt text-primary me-2"></i>
              Danh sách Lịch hẹn
            </h5>
          </div>
          <div class="card-body p-0">
            <!-- Loading State -->
            <div v-if="isLoading.lichHen" class="text-center py-5">
              <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Đang tải...</span>
              </div>
              <p class="mt-2 text-muted">Đang tải dữ liệu...</p>
            </div>

            <!-- Empty State -->
            <div v-else-if="datLichs.length === 0" class="text-center py-5">
              <i
                class="fas fa-calendar-times text-muted mb-3"
                style="font-size: 3rem; opacity: 0.3"
              ></i>
              <p class="text-muted">
                Không có lịch hẹn nào trong khoảng thời gian này
              </p>
            </div>

            <!-- Data Table -->
            <div v-else class="table-responsive">
              <table class="table table-hover modern-table mb-0">
                <thead>
                  <tr>
                    <th width="60">#</th>
                    <th width="150">Thời gian</th>
                    <th width="120">SĐT</th>
                    <th width="120">Trạng thái</th>
                    <th width="130">Thanh toán</th>
                    <th>Dịch vụ</th>
                    <th width="150">Ghi chú</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(dl, index) in datLichs" :key="dl.datLichID">
                    <td class="text-muted">{{ index + 1 }}</td>
                    <td>
                      <div class="fw-bold">
                        {{ formatDateTime(dl.thoiGian) }}
                      </div>
                      <small class="text-muted">{{ dl.thoiLuong }}p</small>
                    </td>
                    <td>
                      <span class="badge bg-light text-dark">{{
                        dl.soDienThoai
                      }}</span>
                    </td>
                    <td>
                      <span
                        class="badge"
                        :class="
                          dl.trangThai === 'Đã đến'
                            ? 'bg-success'
                            : 'bg-warning'
                        "
                      >
                        <i
                          :class="
                            dl.trangThai === 'Đã đến'
                              ? 'fas fa-check'
                              : 'fas fa-clock'
                          "
                          class="me-1"
                        ></i>
                        {{ dl.trangThai }}
                      </span>
                    </td>
                    <td>
                      <span
                        class="badge"
                        :class="dl.daThanhToan ? 'bg-success' : 'bg-danger'"
                      >
                        <i
                          :class="
                            dl.daThanhToan
                              ? 'fas fa-check-circle'
                              : 'fas fa-times-circle'
                          "
                          class="me-1"
                        ></i>
                        {{ dl.daThanhToan ? "Đã TT" : "Chưa TT" }}
                      </span>
                    </td>
                    <td>
                      <div class="service-list">
                        <div
                          v-for="ct in dl.chiTietDatLichs || []"
                          :key="ct.chiTietDatLichID"
                          class="service-item"
                        >
                          <span class="quantity">{{ ct.soLuongDV }}x</span>
                          <span class="name">{{ ct.dichVu?.tenDichVu }}</span>
                          <span class="price">{{
                            formatCurrency(ct.dichVu?.gia)
                          }}</span>
                        </div>
                      </div>
                    </td>
                    <td>
                      <span class="text-muted">{{ dl.ghiChu || "—" }}</span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>

      <!-- HÓA ĐƠN TAB -->
      <div v-show="activeTab === 'hoaDon'" class="tab-pane fade show active">
        <div class="card data-card">
          <div class="card-header bg-transparent">
            <h5 class="card-title mb-0">
              <i class="fas fa-file-invoice-dollar text-success me-2"></i>
              Danh sách Hóa đơn
            </h5>
          </div>
          <div class="card-body p-0">
            <!-- Loading State -->
            <div v-if="isLoading.hoaDon" class="text-center py-5">
              <div class="spinner-border text-success" role="status">
                <span class="visually-hidden">Đang tải...</span>
              </div>
              <p class="mt-2 text-muted">Đang tải dữ liệu...</p>
            </div>

            <!-- Empty State -->
            <div v-else-if="hoaDons.length === 0" class="text-center py-5">
              <i
                class="fas fa-file-invoice text-muted mb-3"
                style="font-size: 3rem; opacity: 0.3"
              ></i>
              <p class="text-muted">
                Không có hóa đơn nào trong khoảng thời gian này
              </p>
            </div>

            <!-- Data Table -->
            <div v-else class="table-responsive">
              <table class="table table-hover modern-table mb-0">
                <thead>
                  <tr>
                    <th width="50">#</th>
                    <th width="130">Ngày tạo</th>
                    <th width="100">Tổng tiền</th>
                    <th width="100">Khách đưa</th>
                    <th width="100">Thối lại</th>
                    <th width="100">Hình thức</th>
                    <th width="100">SĐT</th>
                    <th>Dịch vụ</th>
                    <th width="100">Voucher</th>
                    <th width="100">Giảm giá</th>
                    <th width="80">Tải</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(hd, index) in hoaDons" :key="hd.hoaDonID">
                    <td class="text-muted">{{ index + 1 }}</td>
                    <td>{{ formatDateTime(hd.ngayTao) }}</td>
                    <td>
                      <span class="fw-bold text-success">{{
                        formatCurrency(hd.tongTien)
                      }}</span>
                    </td>
                    <td>{{ formatCurrency(hd.tienKhachDua) }}</td>
                    <td>{{ formatCurrency(hd.tienThoiLai) }}</td>
                    <td>
                      <span class="badge bg-info">{{
                        hd.hinhThucThanhToan ?? "—"
                      }}</span>
                    </td>
                    <td>
                      <span class="badge bg-light text-dark">{{
                        hd.userID
                      }}</span>
                    </td>
                    <td>
                      <div class="service-list">
                        <div
                          v-for="ct in hd.chiTietHoaDons || []"
                          :key="ct.chiTietHoaDonID"
                          class="service-item"
                        >
                          <span class="quantity"
                            >{{ ct.soLuongSP ?? "—" }}x</span
                          >
                          <span class="name" v-if="ct.dichVu">
                            {{ ct.dichVu.tenDichVu }} ({{
                              ct.dichVu.thoiGian ?? 0
                            }}p)
                          </span>
                          <span class="name" v-else-if="ct.sanPham">
                            {{ ct.sanPham.productName }}
                          </span>
                          <span class="price">{{
                            formatCurrency(ct.thanhTien)
                          }}</span>
                        </div>
                      </div>
                    </td>
                    <td>
                      <span
                        class="badge"
                        :class="hd.voucher?.maCode ? 'bg-warning' : 'bg-light'"
                      >
                        {{ hd.voucher?.maCode ?? "—" }}
                      </span>
                    </td>
                    <td>
                      <span
                        class="text-danger fw-bold"
                        v-if="hd.giaTriGiam > 0"
                      >
                        -{{ formatCurrency(hd.giaTriGiam) }}
                      </span>
                      <span v-else class="text-muted">—</span>
                    </td>
                    <td>
                      <button
                        class="btn btn-sm btn-download"
                        :disabled="isDownloading[hd.hoaDonID]"
                        @click="taiHoaDon(hd.hoaDonID)"
                      >
                        <i
                          v-if="isDownloading[hd.hoaDonID]"
                          class="fas fa-spinner fa-spin"
                        ></i>
                        <i v-else class="fas fa-download"></i>
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import apiClient from "../utils/axiosClient";
import dayjs from "dayjs";

// Khai báo các biến phản ứng
const activeTab = ref("hoaDon");
const datLichs = ref([]);
const hoaDons = ref([]);
const fromDate = ref(dayjs().startOf("month").format("YYYY-MM-DD"));
const toDate = ref(dayjs().add(1, "day").format("YYYY-MM-DD"));
const phoneFilter = ref("");
const isLoading = ref({ lichHen: false, hoaDon: false });
const error = ref({ lichHen: null, hoaDon: null });
const isDownloading = ref({});

// Định dạng ngày giờ
const formatDateTime = (dateTime) => {
  if (!dateTime) return "—";
  return dayjs(dateTime).format("HH:mm - DD/MM");
};

// Định dạng tiền tệ
const formatCurrency = (value) => {
  if (value == null) return "—";
  return value.toLocaleString("vi-VN", { style: "currency", currency: "VND" });
};

// Kiểm tra ngày hợp lệ
const isValidDateRange = () => {
  if (!fromDate.value || !toDate.value) return true;
  return dayjs(fromDate.value).isBefore(dayjs(toDate.value));
};

// Lấy danh sách lịch hẹn
const fetchDatLich = async () => {
  if (!isValidDateRange()) {
    error.value.lichHen = "Ngày bắt đầu phải trước ngày kết thúc";
    return;
  }
  try {
    isLoading.value.lichHen = true;
    error.value.lichHen = null;
    const params = {};
    if (fromDate.value) params.startDate = fromDate.value;
    if (toDate.value) params.endDate = toDate.value;
    if (phoneFilter.value) params.sodienthoai = phoneFilter.value.trim();
    const res = await apiClient.get("/thongke/thongKeDatLich", { params });
    datLichs.value = Array.isArray(res) ? res : [];
  } catch (err) {
    error.value.lichHen = "Lỗi khi lấy danh sách lịch hẹn";
    console.error("Lỗi lấy danh sách lịch hẹn:", err.response || err);
    datLichs.value = [];
  } finally {
    isLoading.value.lichHen = false;
  }
};

// Lấy danh sách hóa đơn
const fetchHoaDon = async () => {
  if (!isValidDateRange()) {
    error.value.hoaDon = "Ngày bắt đầu phải trước ngày kết thúc";
    return;
  }
  try {
    isLoading.value.hoaDon = true;
    error.value.hoaDon = null;
    const params = {};
    if (fromDate.value) params.startDate = fromDate.value;
    if (toDate.value) params.endDate = toDate.value;
    if (phoneFilter.value) params.sodienthoai = phoneFilter.value.trim();
    const res = await apiClient.get("/thongke/thongKeHoaDon", { params });
    hoaDons.value = Array.isArray(res) ? res : [];
  } catch (err) {
    error.value.hoaDon = "Lỗi khi lấy danh sách hóa đơn";
    console.error("Lỗi lấy danh sách hóa đơn:", err.response || err);
    hoaDons.value = [];
  } finally {
    isLoading.value.hoaDon = false;
  }
};

// Tải hóa đơn
const taiHoaDon = async (hoaDonID) => {
  try {
    isDownloading.value[hoaDonID] = true;
    error.value.hoaDon = null;

    const response = await apiClient.get(`/ThanhToan/xuat-hoadon/${hoaDonID}`, {
      responseType: "blob",
    });

    console.log("Response headers:", response.headers);
    console.log("Response data size:", response.size);

    if (!response || response.size === 0) {
      throw new Error("Dữ liệu PDF trống từ server");
    }

    const url = window.URL.createObjectURL(
      new Blob([response], { type: "application/pdf" })
    );
    const link = document.createElement("a");
    link.href = url;
    link.setAttribute("download", `HoaDon_${hoaDonID}.pdf`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  } catch (err) {
    console.error("Lỗi khi tải hóa đơn:", err);
    if (err.response?.status === 404) {
      error.value.hoaDon = "Không tìm thấy hóa đơn";
    } else if (err.message.includes("PDF trống")) {
      error.value.hoaDon = "Dữ liệu PDF trống từ server";
    } else {
      error.value.hoaDon = "Lỗi khi tải hóa đơn, vui lòng thử lại";
    }
    console.log("Chi tiết lỗi:", err.response?.value || err.message);
  } finally {
    isDownloading.value[hoaDonID] = false;
  }
};

// Tải dữ liệu ban đầu
onMounted(() => {
  if (activeTab.value === "lichHen") {
    fetchDatLich();
  } else {
    fetchHoaDon();
  }
});
</script>

<style scoped>
/* Header Styling */
.management-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
}

/* Custom Tab Navigation */
.nav-pills-custom {
  display: flex;
  gap: 0.5rem;
}

.btn-tab {
  background: rgba(255, 255, 255, 0.1);
  color: white;
  border: 2px solid rgba(255, 255, 255, 0.2);
  border-radius: 25px;
  padding: 0.75rem 1.5rem;
  font-weight: 500;
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
  position: relative;
}

.btn-tab:hover {
  background: rgba(255, 255, 255, 0.2);
  transform: translateY(-2px);
}

.btn-tab.active {
  background: white;
  color: #667eea;
  box-shadow: 0 4px 15px rgba(255, 255, 255, 0.2);
}

.badge-count {
  background: #ff4757;
  color: white;
  font-size: 0.75rem;
  padding: 0.2rem 0.5rem;
  border-radius: 12px;
  margin-left: 0.5rem;
}

/* Filter Card */
.filter-card {
  border: none;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  border-radius: 12px;
}

.form-control-modern {
  border: 2px solid #e9ecef;
  border-radius: 8px;
  padding: 0.75rem 1rem;
  transition: all 0.3s ease;
}

.form-control-modern:focus {
  border-color: #667eea;
  box-shadow: 0 0 0 0.2rem rgba(102, 126, 234, 0.25);
}

.btn-refresh {
  background: linear-gradient(45deg, #667eea, #764ba2);
  color: white;
  border: none;
  border-radius: 8px;
  padding: 0.75rem 1rem;
  font-weight: 500;
  transition: all 0.3s ease;
}

.btn-refresh:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
  color: white;
}

/* Data Cards */
.data-card {
  border: none;
  box-shadow: 0 6px 30px rgba(0, 0, 0, 0.1);
  border-radius: 12px;
}

.data-card .card-header {
  border-bottom: 2px solid #f8f9fa;
  padding: 1.5rem;
}

/* Modern Table */
.modern-table {
  font-size: 0.9rem;
}

.modern-table thead th {
  background: #f8f9fa;
  border: none;
  font-weight: 600;
  color: #495057;
  padding: 1rem 0.75rem;
  font-size: 0.85rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.modern-table tbody td {
  border: none;
  padding: 1rem 0.75rem;
  vertical-align: middle;
  border-bottom: 1px solid #f1f3f4;
}

.modern-table tbody tr:hover {
  background-color: #f8f9fa;
}

/* Service List Styling */
.service-list {
  max-width: 300px;
}

.service-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.25rem;
  font-size: 0.85rem;
}

.service-item:last-child {
  margin-bottom: 0;
}

.service-item .quantity {
  background: #e3f2fd;
  color: #1976d2;
  padding: 0.1rem 0.4rem;
  border-radius: 4px;
  font-weight: 600;
  min-width: 30px;
  text-align: center;
  font-size: 0.75rem;
}

.service-item .name {
  flex: 1;
  color: #495057;
}

.service-item .price {
  font-weight: 600;
  color: #28a745;
  font-size: 0.8rem;
}

/* Download Button */
.btn-download {
  background: linear-gradient(45deg, #28a745, #20c997);
  color: white;
  border: none;
  border-radius: 6px;
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
}

.btn-download:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(40, 167, 69, 0.3);
  color: white;
}

.btn-download:disabled {
  background: #6c757d;
  cursor: not-allowed;
}

/* Badge Improvements */
.badge {
  font-size: 0.75rem;
  padding: 0.5rem 0.75rem;
  border-radius: 6px;
  font-weight: 500;
}

/* Animation Classes */
.fade {
  transition: opacity 0.3s ease;
}

.show {
  opacity: 1;
}

/* Responsive Design */
@media (max-width: 768px) {
  .management-header {
    padding: 1.5rem;
  }

  .management-header .d-flex {
    flex-direction: column;
    gap: 1rem;
  }

  .nav-pills-custom {
    justify-content: center;
  }

  .btn-tab {
    padding: 0.5rem 1rem;
    font-size: 0.9rem;
  }

  .service-list {
    max-width: 250px;
  }

  .modern-table {
    font-size: 0.8rem;
  }

  .modern-table thead th,
  .modern-table tbody td {
    padding: 0.5rem 0.25rem;
  }
}

/* Loading Animation */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.tab-content > div {
  animation: fadeIn 0.4s ease-out;
}
</style>
