<template>
  <div class="review-management">
    <!-- Toast Notification -->
    <div class="toast-container">
      <div 
        v-if="showToast" 
        class="custom-toast"
        :class="{ 'show': showToast, 'success': toastType === 'success', 'error': toastType === 'error' }"
      >
        <div class="toast-content">
          <div class="toast-icon">
            <i v-if="toastType === 'success'" class="fas fa-check-circle"></i>
            <i v-if="toastType === 'error'" class="fas fa-exclamation-circle"></i>
          </div>
          <span class="toast-message">{{ toastMessage }}</span>
        </div>
      </div>
    </div>

    <!-- Header Section -->
    <div class="page-header">
      <div class="container">
        <div class="row align-items-center">
          <div class="col-md-8">
            <div class="d-flex align-items-center">
              <div class="header-icon me-3">
                <i class="fas fa-star-half-alt"></i>
              </div>
              <div>
                <h1 class="page-title mb-2">Quản lý đánh giá khách hàng</h1>
                <p class="page-subtitle mb-0">Theo dõi và quản lý phản hồi từ khách hàng</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="container">
      <!-- Advanced Filter Section -->
      <div class="filter-section">
        <div class="card shadow-lg border-0">
          <div class="card-header bg-gradient-primary text-white d-flex justify-content-between align-items-center">
            <h5 class="mb-0"><i class="fas fa-filter me-2"></i>Bộ lọc nâng cao</h5>
            <button 
              @click="resetFilters" 
              class="btn btn-light btn-sm d-flex align-items-center"
              title="Làm mới bộ lọc"
            >
              <i class="fas fa-sync-alt me-1"></i>
              Làm mới
            </button>
          </div>
          <div class="card-body p-4">
            <div class="row g-3">
              <!-- Status Filter - Chỉ còn Hiển thị/Ẩn -->
              <div class="col-lg-6">
                <label class="form-label fw-semibold">Trạng thái hiển thị</label>
                <div class="btn-group w-100" role="group">
                  <input type="radio" class="btn-check" name="statusFilter" id="all" v-model="filter" value="all">
                  <label class="btn btn-outline-primary" for="all">
                    <i class="fas fa-list me-2"></i>Tất cả
                  </label>
                  <input type="radio" class="btn-check" name="statusFilter" id="visible" v-model="filter" value="hienthi">
                  <label class="btn btn-outline-success" for="visible">
                    <i class="fas fa-eye me-2"></i>Đang hiển thị
                  </label>
                  <input type="radio" class="btn-check" name="statusFilter" id="hidden" v-model="filter" value="an">
                  <label class="btn btn-outline-danger" for="hidden">
                    <i class="fas fa-eye-slash me-2"></i>Đã ẩn
                  </label>
                </div>
              </div>

              <!-- Search Filter -->
              <div class="col-lg-6">
                <label class="form-label fw-semibold">Tên người dùng</label>
                <div class="input-group">
                  <span class="input-group-text bg-light border-end-0">
                    <i class="fas fa-search text-muted"></i>
                  </span>
                  <input
                    type="text"
                    class="form-control border-start-0 ps-0"
                    placeholder="Nhập tên người dùng..."
                    v-model="searchName"
                  />
                  <button 
                    v-if="searchName" 
                    @click="searchName = ''" 
                    class="btn btn-outline-secondary border-start-0"
                    type="button"
                    title="Xóa tìm kiếm"
                  >
                    <i class="fas fa-times"></i>
                  </button>
                </div>
              </div>

              <!-- Star Rating Filter -->
              <div class="col-lg-6">
                <label class="form-label fw-semibold">Lọc theo số sao</label>
                <div class="star-filter-container">
                  <div class="btn-group w-100" role="group">
                    <input type="radio" class="btn-check" name="starFilter" id="allStars" v-model="starFilter" value="all">
                    <label class="btn btn-outline-secondary" for="allStars">
                      <i class="fas fa-star-half-alt me-2"></i>Tất cả
                    </label>
                    <input type="radio" class="btn-check" name="starFilter" id="star5" v-model="starFilter" value="5">
                    <label class="btn btn-outline-warning" for="star5">
                      <span class="stars-display">
                        <i class="fas fa-star" v-for="n in 5" :key="n"></i>
                      </span>
                      <span class="ms-1">5 sao</span>
                    </label>
                    <input type="radio" class="btn-check" name="starFilter" id="star4" v-model="starFilter" value="4">
                    <label class="btn btn-outline-warning" for="star4">
                      <span class="stars-display">
                        <i class="fas fa-star" v-for="n in 4" :key="n"></i>
                        <i class="far fa-star"></i>
                      </span>
                      <span class="ms-1">4 sao</span>
                    </label>
                  </div>
                  <div class="btn-group w-100 mt-2" role="group">
                    <input type="radio" class="btn-check" name="starFilter" id="star3" v-model="starFilter" value="3">
                    <label class="btn btn-outline-warning" for="star3">
                      <span class="stars-display">
                        <i class="fas fa-star" v-for="n in 3" :key="n"></i>
                        <i class="far fa-star" v-for="n in 2" :key="n"></i>
                      </span>
                      <span class="ms-1">3 sao</span>
                    </label>
                    <input type="radio" class="btn-check" name="starFilter" id="star2" v-model="starFilter" value="2">
                    <label class="btn btn-outline-warning" for="star2">
                      <span class="stars-display">
                        <i class="fas fa-star" v-for="n in 2" :key="n"></i>
                        <i class="far fa-star" v-for="n in 3" :key="n"></i>
                      </span>
                      <span class="ms-1">2 sao</span>
                    </label>
                    <input type="radio" class="btn-check" name="starFilter" id="star1" v-model="starFilter" value="1">
                    <label class="btn btn-outline-danger" for="star1">
                      <span class="stars-display">
                        <i class="fas fa-star"></i>
                        <i class="far fa-star" v-for="n in 4" :key="n"></i>
                      </span>
                      <span class="ms-1">1 sao</span>
                    </label>
                  </div>
                </div>
              </div>

              <!-- Date Range -->
              <div class="col-lg-3">
                <label class="form-label fw-semibold">Từ ngày</label>
                <input 
                  type="date" 
                  class="form-control" 
                  v-model="startDate" 
                  :max="endDate" 
                />
              </div>

              <div class="col-lg-3">
                <label class="form-label fw-semibold">Đến ngày</label>
                <input 
                  type="date" 
                  class="form-control" 
                  v-model="endDate" 
                  :min="startDate" 
                />
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Service Tabs Section - Horizontal Layout -->
      <div class="service-filter-section">
        <div class="card shadow-sm border-0">
          <div class="card-body p-3">
            <div class="d-flex align-items-center justify-content-between mb-3">
              <h6 class="mb-0 fw-semibold text-primary">
                <i class="fas fa-spa me-2"></i>Lọc theo dịch vụ
              </h6>
              <div class="d-flex align-items-center gap-2">
                <small class="text-muted">{{ danhSachLoc.length }} kết quả</small>
                <!-- <button 
                  v-if="starFilter !== 'all'" 
                  @click="starFilter = 'all'" 
                  class="btn btn-outline-secondary btn-sm me-2"
                  title="Xóa lọc sao"
                >
                  <i class="fas fa-star me-1"></i>Xóa lọc sao
                </button> -->
                <!-- <button 
                  v-if="selectedDichVu !== 'all'" 
                  @click="selectedDichVu = 'all'" 
                  class="btn btn-outline-secondary btn-sm"
                  title="Xóa lọc dịch vụ"
                >
                  <i class="fas fa-times me-1"></i>Xóa lọc
              </button> -->
              </div>
            </div>
            
            <div class="service-tabs-horizontal">
              <div class="nav nav-pills flex-nowrap overflow-auto" role="tablist">
                <button
                  class="nav-link flex-shrink-0"
                  :class="{ active: selectedDichVu === 'all' }"
                  @click="selectedDichVu = 'all'"
                >
                  <i class="fas fa-th-large me-1"></i>
                  Tất cả dịch vụ
                </button>
                <button
                  v-for="dv in dichVuTabs"
                  :key="dv"
                  class="nav-link flex-shrink-0"
                  :class="{ active: selectedDichVu === dv }"
                  @click="selectedDichVu = dv"
                >
                  <i class="fas fa-spa me-1"></i>
                  {{ dv }}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Reviews Table -->
      <div class="reviews-section">
        <div v-if="danhSachLoc.length === 0" class="empty-state">
          <div class="card shadow-sm border-0 text-center py-5">
            <div class="card-body">
              <div class="empty-icon mb-3">
                <i class="fas fa-comments text-muted"></i>
              </div>
              <h4 class="text-muted mb-2">Không có đánh giá phù hợp</h4>
              <p class="text-muted mb-3">Thử thay đổi bộ lọc để xem thêm đánh giá</p>
              <button @click="resetFilters" class="btn btn-primary">
                <i class="fas fa-sync-alt me-2"></i>Làm mới bộ lọc
              </button>
            </div>
          </div>
        </div>

        <div v-else class="table-section">
          <div class="card shadow-lg border-0">
            <div class="card-header bg-gradient-info text-white d-flex justify-content-between align-items-center">
              <h5 class="mb-0">
                <i class="fas fa-table me-2"></i>
                Danh sách đánh giá ({{ danhSachLoc.length }} kết quả)
              </h5>
              <button 
                @click="loadDanhSach" 
                class="btn btn-light btn-sm d-flex align-items-center"
                title="Tải lại dữ liệu"
                :disabled="isLoading"
              >
                <i class="fas fa-sync-alt me-1" :class="{ 'fa-spin': isLoading }"></i>
                {{ isLoading ? 'Đang tải...' : 'Tải lại' }}
              </button>
            </div>
            <div class="card-body p-0">
              <div class="table-responsive" style="max-height: 70vh;">
                <table class="table table-hover mb-0 review-table">
                  <thead class="table-dark sticky-top">
                    <tr>
                      <th scope="col" class="text-center" style="width: 60px;">#</th>
                      <th scope="col" style="width: 200px;">Dịch vụ</th>
                      <th scope="col" style="width: 180px;">Người dùng</th>
                      <th scope="col" style="width: 140px;">Đánh giá</th>
                      <th scope="col" style="width: 300px;">Nội dung</th>
                      <th scope="col" style="width: 160px;">Ngày tạo</th>
                      <th scope="col" style="width: 130px;">Trạng thái</th>
                      <th scope="col" class="text-center" style="width: 120px;">Thao tác</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(dg, index) in danhSachLocSorted" :key="dg.id" class="review-row">
                      <td class="text-center align-middle">
                        <span class="badge bg-secondary">{{ index + 1 }}</span>
                      </td>
                      
                      <td class="align-middle">
                        <span class="badge bg-info bg-opacity-10 text-info p-2">
                          <i class="fas fa-spa me-1"></i>
                          {{ dg.dichVu?.tenDichVu || "Không rõ" }}
                        </span>
                      </td>
                      
                      <td class="align-middle">
                        <div class="d-flex align-items-center">
                          <div class="user-avatar me-2">
                            <i class="fas fa-user text-white"></i>
                          </div>
                          <div>
                            <span v-if="dg.anDanh" class="text-muted fst-italic small">
                              <i class="fas fa-user-secret me-1"></i>Ẩn danh
                            </span>
                            <span v-else class="fw-medium">{{ dg.user?.name || "Chưa rõ" }}</span>
                          </div>
                        </div>
                      </td>
                      
                      <td class="align-middle">
                        <div class="rating-display">
                          <div class="stars mb-1">
                            <i
                              v-for="n in 5"
                              :key="n"
                              class="fas fa-star"
                              :class="n <= dg.soSao ? 'text-warning' : 'text-muted opacity-25'"
                            ></i>
                          </div>
                          <small class="text-muted">{{ dg.soSao }}/5</small>
                        </div>
                      </td>
                      
                      <td class="align-middle">
                        <div class="review-content">
                          <p class="mb-0 text-truncate-3">{{ dg.noiDung || "(Không có nội dung)" }}</p>
                        </div>
                      </td>
                      
                      <td class="align-middle">
                        <small class="text-muted d-flex align-items-center">
                          <i class="fas fa-calendar-alt me-1"></i>
                          {{ formatDate(dg.ngayTao) }}
                        </small>
                      </td>
                      
                      <td class="align-middle">
                        <span
                          class="badge"
                          :class="dg.isActive ? 'bg-success' : 'bg-danger'"
                        >
                          <i
                            :class="dg.isActive ? 'fas fa-eye' : 'fas fa-eye-slash'"
                            class="me-1"
                          ></i>
                          {{ dg.isActive ? "Đang hiển thị" : "Đã ẩn" }}
                        </span>
                      </td>
                      
                      <td class="text-center align-middle">
                        <div class="btn-group btn-group-sm" role="group">
                          <button
                            @click="toggleTrangThai(dg.id)"
                            class="btn btn-sm"
                            :class="dg.isActive ? 'btn-danger' : 'btn-success'"
                            :title="dg.isActive ? 'Ẩn đánh giá' : 'Hiển thị lại đánh giá'"
                          >
                            <i :class="dg.isActive ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
                            {{ dg.isActive ? 'Ẩn' : 'Hiện' }}
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

      <!-- Charts Section -->
      <div class="charts-section mt-5">
        <div class="row">
          <div class="col-12 mb-4">
            <h2 class="text-white fw-bold">
              <i class="fas fa-chart-bar me-2"></i>Thống kê đánh giá
            </h2>
          </div>
        </div>
        <div class="row g-4">
          <div class="col-lg-6">
            <div class="card shadow-lg border-0 h-100">
              <div class="card-header bg-gradient-primary text-white">
                <h5 class="mb-0">
                  <i class="fas fa-chart-column me-2"></i>Dịch vụ được đánh giá nhiều
                </h5>
              </div>
              <div class="card-body">
                <BarChart :data="chartDataSoLuong" :options="chartOptions" />
              </div>
            </div>
          </div>
          <div class="col-lg-6">
            <div class="card shadow-lg border-0 h-100">
              <div class="card-header bg-gradient-success text-white">
                <h5 class="mb-0">
                  <i class="fas fa-star me-2"></i>Dịch vụ được đánh giá tốt
                </h5>
              </div>
              <div class="card-body">
                <BarChart :data="chartDataTrungBinh" :options="chartOptions" />
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
import BarChart from "@/components/BarChart.vue";
import axiosClient from "../utils/axiosClient";
import Swal from "sweetalert2";

const danhSach = ref([]);
const filter = ref("all");
const searchName = ref("");
const startDate = ref("");
const endDate = ref("");
const selectedDichVu = ref("all");
const starFilter = ref("all");
const isLoading = ref(false);

// Toast notification states
const showToast = ref(false);
const toastMessage = ref("");
const toastType = ref("success"); // 'success' or 'error'

const chartDataSoLuong = ref({ labels: [], datasets: [] });
const chartDataTrungBinh = ref({ labels: [], datasets: [] });

const chartOptions = {
  responsive: true,
  plugins: { legend: { display: false } },
  scales: { y: { beginAtZero: true } },
};

onMounted(async () => await loadDanhSach());

// Toast notification functions
const showToastMessage = (message, type = "success") => {
  toastMessage.value = message;
  toastType.value = type;
  showToast.value = true;
  
  // Auto hide after 3 seconds
  setTimeout(() => {
    showToast.value = false;
  }, 3000);
};

const loadDanhSach = async () => {
  try {
    isLoading.value = true;
    const res = await axiosClient.get("DanhGia/admin/all");
    danhSach.value = res;
    updateCharts();
  } catch (err) {
    console.error("Lỗi khi tải đánh giá:", err);
    showToastMessage("Lỗi khi tải danh sách đánh giá", "error");
  } finally {
    isLoading.value = false;
  }
};

// Reset all filters to default values
const resetFilters = () => {
  filter.value = "all";
  searchName.value = "";
  startDate.value = "";
  endDate.value = "";
  selectedDichVu.value = "all";
  starFilter.value = "all";
  showToastMessage("Đã làm mới bộ lọc");
};

const toggleTrangThai = async (id) => {
  const result = await Swal.fire({
    title: "Xác nhận",
    text: "Bạn có chắc muốn thay đổi trạng thái đánh giá này?",
    icon: "question",
    showCancelButton: true,
    confirmButtonText: "Có",
    cancelButtonText: "Hủy",
  });

  if (result.isConfirmed) {
    try {
      await axiosClient.put(`DanhGia/toggle/${id}`);
      await loadDanhSach();

      await Swal.fire({
        position: "center",
        icon: "success",
        title: "Đã đổi trạng thái thành công!",
        showConfirmButton: false,
        timer: 2000,
      });
    } catch (err) {
      console.error("Lỗi khi thay đổi trạng thái:", err);

      await Swal.fire({
        position: "center",
        icon: "error",
        title: "Thay đổi trạng thái thất bại!",
        showConfirmButton: false,
        timer: 2000,
      });
    }
  }
};


const formatDate = (dateStr) => new Date(dateStr).toLocaleString("vi-VN");

const dichVuTabs = computed(() => {
  const tenDVs = danhSach.value
    .map((dg) => dg.dichVu?.tenDichVu)
    .filter(Boolean);
  return [...new Set(tenDVs)];
});

const danhSachLoc = computed(() => {
  return danhSach.value.filter((d) => {
    const matchFilter =
      filter.value === "all" ||
      (filter.value === "hienthi" && d.isActive) ||
      (filter.value === "an" && !d.isActive);

    const matchSearch =
      !searchName.value ||
      (!d.anDanh &&
        d.user?.name?.toLowerCase().includes(searchName.value.toLowerCase()));

    const createdAt = new Date(d.ngayTao);
    const start = startDate.value ? new Date(startDate.value) : null;
    const end = endDate.value ? new Date(endDate.value + "T23:59:59") : null;

    const matchDate =
      (!start || createdAt >= start) && (!end || createdAt <= end);

    const matchDichVu =
      selectedDichVu.value === "all" ||
      d.dichVu?.tenDichVu === selectedDichVu.value;

    const matchStar =
      starFilter.value === "all" ||
      d.soSao === parseInt(starFilter.value);

    return matchFilter && matchSearch && matchDate && matchDichVu && matchStar;
  });
});

// Sắp xếp để đưa đánh giá mới nhất lên đầu
const danhSachLocSorted = computed(() => {
  return [...danhSachLoc.value].sort((a, b) => {
    // Sắp xếp theo ngày tạo (mới nhất trước)
    return new Date(b.ngayTao) - new Date(a.ngayTao);
  });
});

const updateCharts = () => {
  const groupByService = {};
  danhSach.value.forEach((dg) => {
    const ten = dg.dichVu?.tenDichVu || "Không rõ";
    if (!groupByService[ten]) groupByService[ten] = { count: 0, total: 0 };
    groupByService[ten].count++;
    groupByService[ten].total += dg.soSao;
  });

  const labels = Object.keys(groupByService);
  const counts = labels.map((ten) => groupByService[ten].count);
  const averages = labels.map((ten) =>
    (groupByService[ten].total / groupByService[ten].count).toFixed(2)
  );

  chartDataSoLuong.value = {
    labels,
    datasets: [
      { label: "Số lượt đánh giá", backgroundColor: "#667eea", data: counts },
    ],
  };

  chartDataTrungBinh.value = {
    labels,
    datasets: [
      { label: "Điểm trung bình", backgroundColor: "#f093fb", data: averages },
    ],
  };
};
</script>

<style scoped>
/* Global Styles */
.review-management {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

/* Toast Notification Styles */
.toast-container {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 9999;
}

.custom-toast {
  min-width: 300px;
  background: #fff;
  border-radius: 10px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  transform: translateX(400px);
  opacity: 0;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  overflow: hidden;
  position: relative;
}

.custom-toast.show {
  transform: translateX(0);
  opacity: 1;
}

.custom-toast.success {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  color: white;
}

.custom-toast.success::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  width: 4px;
  height: 100%;
  background: rgba(255, 255, 255, 0.3);
}

.custom-toast.error {
  background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
  color: white;
}

.custom-toast.error::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  width: 4px;
  height: 100%;
  background: rgba(255, 255, 255, 0.3);
}

.toast-content {
  padding: 16px 20px;
  display: flex;
  align-items: center;
  gap: 12px;
}

.toast-icon {
  font-size: 1.2rem;
  flex-shrink: 0;
}

.toast-message {
  font-weight: 500;
  font-size: 0.95rem;
  line-height: 1.4;
}

/* Header Section */
.page-header {
  padding: 3rem 0;
  color: white;
}

.header-icon {
  width: 60px;
  height: 60px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 15px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.8rem;
  backdrop-filter: blur(10px);
}

.page-title {
  font-size: 2.2rem;
  font-weight: 700;
  margin: 0;
}

.page-subtitle {
  font-size: 1.1rem;
  opacity: 0.9;
  margin: 0;
}

/* Filter Section */
.filter-section {
  margin-top: -2rem;
  margin-bottom: 2rem;
  position: relative;
  z-index: 10;
}

.bg-gradient-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%) !important;
}

.bg-gradient-info {
  background: linear-gradient(135deg, #17a2b8 0%, #138496 100%) !important;
}

.bg-gradient-success {
  background: linear-gradient(135deg, #28a745 0%, #20c997 100%) !important;
}

/* Service Filter Section */
.service-filter-section {
  margin-bottom: 2rem;
}

.service-tabs-horizontal {
  position: relative;
}

.service-tabs-horizontal .nav {
  padding-bottom: 0.5rem;
  scrollbar-width: thin;
  scrollbar-color: #667eea transparent;
}

.service-tabs-horizontal .nav::-webkit-scrollbar {
  height: 4px;
}

.service-tabs-horizontal .nav::-webkit-scrollbar-track {
  background: #f8f9fa;
  border-radius: 2px;
}

.service-tabs-horizontal .nav::-webkit-scrollbar-thumb {
  background: #667eea;
  border-radius: 2px;
}

.service-tabs-horizontal .nav-link {
  border-radius: 20px;
  margin-right: 0.5rem;
  padding: 0.5rem 1rem;
  font-size: 0.9rem;
  white-space: nowrap;
  background: #f8f9fa;
  border: 1px solid #dee2e6;
  color: #6c757d;
  transition: all 0.3s ease;
}

.service-tabs-horizontal .nav-link:hover,
.service-tabs-horizontal .nav-link.active {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-color: #667eea;
  color: white;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
}

/* Table Improvements */
.review-table {
  font-size: 0.9rem;
}

.review-row {
  transition: all 0.2s ease;
  cursor: default;
}

.review-row:hover {
  background: linear-gradient(90deg, rgba(102, 126, 234, 0.05) 0%, rgba(255, 255, 255, 0.8) 100%) !important;
  box-shadow: inset 3px 0 0 #667eea;
}

.user-avatar {
  width: 35px;
  height: 35px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.9rem;
}

.text-truncate-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  line-clamp: 2;
  overflow: hidden;
  line-height: 1.4;
  max-height: 4.2em;
}

/* Empty State */
.empty-icon {
  font-size: 4rem;
  color: #6c757d;
  opacity: 0.5;
}

/* Charts Section */
.charts-section {
  padding-bottom: 3rem;
}

/* Refresh Button Styles */
.btn-light {
  transition: all 0.3s ease;
}

.btn-light:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.fa-spin {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

/* Star Filter Styles */
.star-filter-container .btn-group {
  border-radius: 8px;
  overflow: hidden;
}

.star-filter-container .btn {
  border: 1px solid #dee2e6;
  background: #f8f9fa;
  color: #6c757d;
  transition: all 0.3s ease;
  padding: 0.5rem 0.75rem;
  font-size: 0.85rem;
  white-space: nowrap;
  display: flex;
  align-items: center;
  justify-content: center;
}

.star-filter-container .stars-display {
  display: inline-flex;
  gap: 1px;
  font-size: 0.75rem;
}

.star-filter-container .stars-display .fas.fa-star {
  color: #ffc107;
}

.star-filter-container .stars-display .far.fa-star {
  color: #dee2e6;
}

.btn-check:checked + .btn-outline-warning {
  background: linear-gradient(135deg, #f8ce14 0%, #f8ce14 100%);
  border-color: #f8ce14;
  color: white;
}

.btn-check:checked + .btn-outline-danger {
  background: linear-gradient(135deg, #dc3545 0%, #c82333 100%);
  border-color: #dc3545;
  color: white;
}

.star-filter-container .btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

/* Toast Animation Enhancements */
@keyframes slideInRight {
  from {
    transform: translateX(400px);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

@keyframes slideOutRight {
  from {
    transform: translateX(0);
    opacity: 1;
  }
  to {
    transform: translateX(400px);
    opacity: 0;
  }
}

.custom-toast.show {
  animation: slideInRight 0.3s cubic-bezier(0.4, 0, 0.2, 1) forwards;
}

.custom-toast:not(.show) {
  animation: slideOutRight 0.3s cubic-bezier(0.4, 0, 0.2, 1) forwards;
}

/* Toast Success Pulse Effect */
.custom-toast.success .toast-icon {
  animation: successPulse 0.6s ease-in-out;
}

@keyframes successPulse {
  0% { transform: scale(1); }
  50% { transform: scale(1.2); }
  100% { transform: scale(1); }
}

/* Toast Error Shake Effect */
.custom-toast.error .toast-icon {
  animation: errorShake 0.5s ease-in-out;
}

@keyframes errorShake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-3px); }
  75% { transform: translateX(3px); }
}

/* Progress Bar for Toast Auto-hide */
.custom-toast::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  height: 3px;
  background: rgba(255, 255, 255, 0.3);
  animation: progressBar 3s linear forwards;
  width: 0;
}

@keyframes progressBar {
  from { width: 0; }
  to { width: 100%; }
}

/* Responsive Design */
@media (max-width: 768px) {
  .toast-container {
    right: 10px;
    left: 10px;
    top: 10px;
  }
  
  .custom-toast {
    min-width: auto;
    width: 100%;
  }
  
  .custom-toast {
    transform: translateY(-100px);
  }
  
  .custom-toast.show {
    transform: translateY(0);
  }
  
  @keyframes slideInRight {
    from {
      transform: translateY(-100px);
      opacity: 0;
    }
    to {
      transform: translateY(0);
      opacity: 1;
    }
  }
  
  .page-title {
    font-size: 1.8rem;
  }
  
  .review-table {
    font-size: 0.8rem;
  }
  
  .user-avatar {
    width: 30px;
    height: 30px;
    font-size: 0.8rem;
  }
  
  .text-truncate-3 {
    -webkit-line-clamp: 2;
    line-clamp: 2;
    max-height: 2.8em;
  }
  
  .service-tabs-horizontal .nav-link {
    font-size: 0.8rem;
    padding: 0.4rem 0.8rem;
  }
  
  .card-header {
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .card-header .btn {
    align-self: flex-end;
  }
}

/* Custom Bootstrap Overrides */
.btn-check:checked + .btn-outline-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-color: #667eea;
}

.btn-check:checked + .btn-outline-warning {
  background: linear-gradient(135deg, #28a745 0%, #28a745 100%);
  border-color: #ffc107;
}

.btn-check:checked + .btn-outline-success {
  background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
  border-color: #28a745;
}

/* Dark mode support for toast */
@media (prefers-color-scheme: dark) {
  .custom-toast:not(.success):not(.error) {
    background: #2d3748;
    color: #e2e8f0;
    border-color: rgba(255, 255, 255, 0.1);
  }
}
</style>