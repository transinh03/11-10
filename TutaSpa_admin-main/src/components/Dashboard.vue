<template>
  <div class="container-fluid">
    <!-- Stat Cards -->
    <div
      class="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 g-3"
      style="margin-bottom: 15px"
    >
      <!-- Dịch vụ hoàn thành -->
      <div class="col">
        <div
          class="card h-100 border-0 shadow-lg stat-card"
          style="
            background: linear-gradient(135deg, #00b09b 0%, #96c93d 100%);
            border-radius: 15px;
            transition: all 0.3s ease;
            position: relative;
            overflow: hidden;
          "
          @mouseover="handleHover($event, true)"
          @mouseleave="handleHover($event, false)"
        >
          <div class="progress-bar"></div>
          <div class="card-body d-flex align-items-center p-4">
            <div class="stat-icon me-3">
              <i class="fas fa-check-circle fa-1x text-white"></i>
            </div>
            <div class="stat-info text-white">
              <div class="stat-number fs-2 fw-bold mb-1">
                {{ dvHoanThanh.toLocaleString() }}
              </div>
              <div class="stat-label fs-6 fw-medium">Dịch vụ hoàn thành</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Lịch hẹn hôm nay -->
      <div class="col">
        <div
          class="card h-100 border-0 shadow-lg stat-card"
          style="
            background: linear-gradient(135deg, #2193b0 0%, #6dd5ed 100%);
            border-radius: 15px;
            transition: all 0.3s ease;
            position: relative;
            overflow: hidden;
          "
          @mouseover="handleHover($event, true)"
          @mouseleave="handleHover($event, false)"
        >
          <div class="progress-bar"></div>
          <div class="card-body d-flex align-items-center p-4">
            <div class="stat-icon me-3">
              <i class="fas fa-calendar-day fa-1x text-white"></i>
            </div>
            <div class="stat-info text-white">
              <div class="stat-number fs-2 fw-bold mb-1">
                {{ lichHen.toLocaleString() }}
              </div>
              <div class="stat-label fs-6 fw-medium">Lịch hẹn hôm nay</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Đánh giá trung bình -->
      <div class="col">
        <div
          class="card h-100 border-0 shadow-lg stat-card"
          style="
            background: linear-gradient(135deg, #ff9a9e 0%, #fecfef 100%);
            border-radius: 15px;
            transition: all 0.3s ease;
            position: relative;
            overflow: hidden;
          "
          @mouseover="handleHover($event, true)"
          @mouseleave="handleHover($event, false)"
        >
          <div class="progress-bar"></div>
          <div class="card-body d-flex align-items-center p-4">
            <div class="stat-icon me-3">
              <i class="fas fa-star fa-1x text-white"></i>
            </div>
            <div class="stat-info text-white">
              <div class="stat-number fs-2 fw-bold mb-1">{{ danhGia }}/5</div>
              <div class="stat-label fs-6 fw-medium">Đánh giá trung bình</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Doanh thu hôm nay -->
      <div class="col">
        <div
          class="card h-100 border-0 shadow-lg stat-card"
          style="
            background: linear-gradient(135deg, #f7971e 0%, #ffd200 100%);
            border-radius: 15px;
            transition: all 0.3s ease;
            position: relative;
            overflow: hidden;
          "
          @mouseover="handleHover($event, true)"
          @mouseleave="handleHover($event, false)"
        >
          <div class="progress-bar"></div>
          <div class="card-body d-flex align-items-center p-4">
            <div class="stat-icon me-3">
              <i class="fas fa-hand-holding-usd fa-1x text-white"></i>
            </div>
            <div class="stat-info text-white">
              <div class="stat-number fs-2 fw-bold mb-1">
                {{ doanhThu.toLocaleString() }} đ
              </div>
              <div class="stat-label fs-6 fw-medium">Doanh thu hôm nay</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Revenue Chart and Appointment List -->
    <div class="row">
      <div class="col-9">
        <div
          class="card border-0 shadow-lg"
          style="border-radius: 20px; overflow: hidden"
        >
          <div class="card-header bg-white border-0 p-4">
            <div class="d-flex justify-content-between align-items-center flex-wrap">
              <h4 class="mb-0 fw-bold text-primary">📊 Biểu đồ Doanh thu</h4>
              <div class="d-flex align-items-center gap-3 flex-wrap">
                <!-- Dropdown chọn tháng/năm -->
                <div v-if="selectedTimeRange === 'thang'" class="d-flex gap-2">
                  <select 
                    v-model="selectedMonth" 
                    @change="onMonthYearChange"
                    class="form-select form-select-sm"
                    style="width: 100px;"
                  >
                    <option v-for="month in months" :key="month.value" :value="month.value">
                      {{ month.label }}
                    </option>
                  </select>
                  <select 
                    v-model="selectedYear" 
                    @change="onMonthYearChange"
                    class="form-select form-select-sm"
                    style="width: 100px;"
                  >
                    <option v-for="year in years" :key="year" :value="year">
                      {{ year }}
                    </option>
                  </select>
                </div>
                <div v-else class="d-flex gap-2">
                  <select 
                    v-model="selectedYear" 
                    @change="onYearChange"
                    class="form-select form-select-sm"
                    style="width: 100px;"
                  >
                    <option v-for="year in years" :key="year" :value="year">
                      {{ year }}
                    </option>
                  </select>
                </div>
                
                <!-- Toggle buttons -->
                <div class="btn-group" role="group">
                  <button
                    type="button"
                    class="btn btn-outline-primary"
                    :class="{ active: selectedTimeRange === 'thang' }"
                    @click="changeTimeRange('thang')"
                    style="border-radius: 25px 0 0 25px"
                  >
                    Tháng
                  </button>
                  <button
                    type="button"
                    class="btn btn-outline-primary"
                    :class="{ active: selectedTimeRange === 'nam' }"
                    @click="changeTimeRange('nam')"
                    style="border-radius: 0 25px 25px 0"
                  >
                    Năm
                  </button>
                </div>
              </div>
            </div>
          </div>
          <div class="card-body p-4">
            <div v-if="isLoading" class="text-center p-5">
              <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
              </div>
              <p class="mt-2 text-muted">Đang tải dữ liệu...</p>
            </div>
            <div
              v-else-if=" chartData.length > 0"
              ref="chartContainer"
              id="revenueChart"
              style="min-height: 300px; width: 100%"
            ></div>
            <div v-else class="text-center p-5 text-muted">
              <i class="fas fa-chart-bar fa-3x mb-3"></i>
              <p>Không có dữ liệu để hiển thị</p>
            </div>
          </div>
        </div>
      </div>
      <div class="col-lg-3 col-md-6 col-sm-12">
        <div
          class="card border-0 shadow-lg h-100"
          style="border-radius: 20px; overflow: hidden; font-size: 1.1em"
        >
          <!-- Header -->
          <div
            class="card-header bg-light border-0 py-3 px-4 d-flex align-items-center"
          >
            <h5 class="mb-0 fw-bold text-primary">
              <span class="me-2">📅</span> Lịch Hẹn Hôm Nay
            </h5>
          </div>

          <!-- Body -->
          <div
            class="card-body p-4"
            style="overflow-y: auto; overflow-x: hidden; max-height: 430px"
          >
            <!-- Nếu không có lịch -->
            <div
              v-if="lichHenHienThi.length === 0"
              class="text-center text-muted py-5"
            >
              <i class="fas fa-calendar-times fa-3x mb-3"></i>
              <p class="mb-0">Không có lịch hẹn hôm nay</p>
            </div>

            <!-- Danh sách lịch hẹn -->
            <div v-else class="list-group list-group-flush">
              <a
                v-for="lich in lichHenHienThi"
                :key="lich.datLichID"
                href="#"
                class="list-group-item list-group-item-action rounded-3 mb-3 shadow-sm"
                style="border-left: 5px solid transparent; transition: all 0.3s"
                :style="{ borderLeftColor: getColor(lich) }"
              >
                <div class="d-flex justify-content-between align-items-start">
                  <div class="flex-grow-1">
                    <!-- Giờ + SĐT trên cùng 1 hàng -->
                    <div
                      class="d-flex justify-content-between fw-bold text-dark flex-wrap"
                    >
                      <span>
                        <i class="far fa-clock me-1 text-primary"></i>
                        {{ formatGio(lich.thoiGian) }} -
                        {{ tinhGioKetThuc(lich) }}
                      </span>
                      <span>
                        <i class="fas fa-phone-alt me-1 text-success"></i>
                        {{ lich.soDienThoai }}
                      </span>
                    </div>
                    <!-- Tên dịch vụ -->
                    <div class="small text-secondary mt-1">
                      {{ getTenDichVu(lich) }}
                    </div>
                  </div>
                </div>
              </a>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Service Distribution Chart -->
    <div class="row mt-4">
      <div class="col-6">
        <div
          class="card border-0 shadow-lg"
          style="border-radius: 20px; overflow: hidden"
        >
          <div class="card-header bg-white border-0 p-4">
            <h4 class="mb-0 fw-bold text-primary">📊 Phân bố Dịch vụ</h4>
          </div>
          <div class="card-body p-4">
            <div v-if="isLoadingService" class="text-center p-5">
              <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
              </div>
              <p class="mt-2 text-muted">Đang tải dữ liệu...</p>
            </div>
            <div
              v-else-if="serviceData.length > 0"
              style="height: 400px; position: relative"
            >
              <canvas
                ref="pieChart"
                style="max-height: 400px; width: 100%"
              ></canvas>
            </div>
            <div v-else class="text-center p-5 text-muted">
              <i class="fas fa-chart-pie fa-3x mb-3"></i>
              <p>Không có dữ liệu dịch vụ để hiển thị</p>
            </div>
          </div>
        </div>
      </div>
      <div class="col-6">
        <div
          class="card border-0 shadow-lg"
          style="border-radius: 20px; overflow: hidden"
        >
          <div class="card-header bg-white border-0 p-4">
            <h4 class="mb-0 fw-bold text-primary">📊 Phân bố Sản phẩm</h4>
          </div>
          <div class="card-body p-4">
            <div v-if="isLoadingProduct" class="text-center p-5">
              <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
              </div>
              <p class="mt-2 text-muted">Đang tải dữ liệu...</p>
            </div>
            <div
              v-else-if="productData.length > 0"
              style="height: 400px; position: relative"
            >
              <canvas
                ref="productPieChart"
                style="max-height: 400px; width: 100%"
              ></canvas>
            </div>
            <div v-else class="text-center p-5 text-muted">
              <i class="fas fa-chart-pie fa-3x mb-3"></i>
              <p>Không có dữ liệu sản phẩm để hiển thị</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick, watch } from "vue";
import apiClient from "../utils/axiosClient";
import Chart from "chart.js/auto";

// Stats
const lichHen = ref(0);
const danhGia = ref(0);
const dvHoanThanh = ref(0);
const doanhThu = ref(0);

// Chart variables
const chartContainer = ref(null);
const selectedTimeRange = ref("thang");
const selectedMonth = ref(new Date().getMonth() + 1);
const selectedYear = ref(new Date().getFullYear());
const isLoading = ref(false);
const isLoadingService = ref(false);
const chartData = ref([]);
const debugInfo = ref("");

// Pie chart variables
const pieChart = ref(null);
const serviceData = ref([]);
let chartInstance = null;
let pieChartInstance = null;

// Appointment list
const lichHenHienThi = ref([]);

// Dropdown data
const months = ref([
  { value: 1, label: "Tháng 1" },
  { value: 2, label: "Tháng 2" },
  { value: 3, label: "Tháng 3" },
  { value: 4, label: "Tháng 4" },
  { value: 5, label: "Tháng 5" },
  { value: 6, label: "Tháng 6" },
  { value: 7, label: "Tháng 7" },
  { value: 8, label: "Tháng 8" },
  { value: 9, label: "Tháng 9" },
  { value: 10, label: "Tháng 10" },
  { value: 11, label: "Tháng 11" },
  { value: 12, label: "Tháng 12" },
]);

const years = ref([]);

// Initialize years array
const initializeYears = () => {
  const currentYear = new Date().getFullYear();
  const yearList = [];
  for (let i = currentYear - 5; i <= currentYear + 1; i++) {
    yearList.push(i);
  }
  years.value = yearList;
};

// Handle hover effect for stat cards
const handleHover = (event, isHover) => {
  event.currentTarget.style.transform = isHover
    ? "translateY(-5px)"
    : "translateY(0)";
  event.currentTarget.style.boxShadow = isHover
    ? "0 12px 35px rgba(0,0,0,0.25)"
    : "0 8px 25px rgba(0,0,0,0.15)";
};

// Format chart data
const formatDataForChart = (data) => {
  console.log("Raw API data:", data);

  if (!data) {
    debugInfo.value = "No data received from API";
    return [];
  }

  let formattedData = [];

  if (Array.isArray(data)) {
    formattedData = data.map((item) => ({
      x: item.period || item.label || item.name || "Unknown",
      value: Number(item.value || item.amount || item.total || 0),
    }));
  } else if (typeof data === "object") {
    formattedData = Object.keys(data).map((key) => ({
      x: key,
      value: Number(data[key] || 0),
    }));
  }

  debugInfo.value = `Formatted ${formattedData.length} items`;
  console.log("Formatted chart data:", formattedData);
  return formattedData;
};

// Get full chart data with all time periods
const getFullChartData = (rawData, timeRange) => {
  // Xác định các mốc thời gian cần hiển thị
  let labels = [];
  let now = new Date();
  if (timeRange === "thang") {
    // Hiển thị đủ ngày trong tháng hiện tại
    const year = selectedYear.value;
    const month = selectedMonth.value - 1; // JavaScript month is 0-based
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    for (let d = 1; d <= daysInMonth; d++) {
      labels.push(d.toString());
    }
  } else {
    // Hiển thị đủ 12 tháng
    labels = Array.from({ length: 12 }, (_, i) => (i + 1).toString());
  }

  // Chuyển rawData thành map để tra cứu nhanh
  const dataMap = {};
  rawData.forEach((item) => {
    // item.x có thể là ngày/tháng, chuyển về string để so sánh
    dataMap[item.x.toString()] = item.value;
  });

  // Tạo mảng dữ liệu đủ các mốc, nếu không có thì gán 0
  return labels.map((label) => ({
    x: label,
    value: dataMap[label] ?? 0,
  }));
};

// Handle bar click for yearly view
const handleBarClick = (monthIndex) => {
  if (selectedTimeRange.value === 'nam') {
    // Chuyển sang view tháng và set tháng được chọn
    selectedTimeRange.value = 'thang';
    selectedMonth.value = monthIndex + 1; // monthIndex bắt đầu từ 0
    loadChartData('thang', selectedMonth.value, selectedYear.value);
  }
};

// Create custom bar chart
const createChart = (data) => {
  if (!chartContainer.value) return;
  if (!data || data.length === 0) return;

  chartContainer.value.innerHTML = "";

  const maxValue = Math.max(...data.map((d) => d.value));
  const dataLength = data.length;

  // TÍNH barWidth động theo số cột và chiều rộng khung
  const containerWidth = chartContainer.value.offsetWidth || 900;
  const spacing = 8;
  const totalSpacing = spacing * (dataLength - 1);
  const barWidth = Math.max(
    24,
    Math.floor((containerWidth - totalSpacing) / dataLength)
  );

  const chartDiv = document.createElement("div");
  chartDiv.style.cssText = `
    display: flex;
    align-items: end;
    justify-content: center;
    height: 350px;
    padding: 20px;
    background: linear-gradient(135deg, #f8f9ff 0%, #ffffff 100%);
    border-radius: 15px;
    gap: ${spacing}px;
    width: 100%;
  `;

  data.forEach((item, index) => {
    const barContainer = document.createElement("div");
    barContainer.style.cssText = `
      display: flex;
      flex-direction: column;
      align-items: center;
      width: ${barWidth}px;
      min-width: ${barWidth}px;
      cursor: ${selectedTimeRange.value === 'nam' ? 'pointer' : 'default'};
    `;

    const barHeight = Math.max((item.value / maxValue) * 280, 20);
    const valueLabel = document.createElement("div");

    if (item.value >= 1000000) {
      valueLabel.textContent = (item.value / 1000000).toFixed(1) + "M";
    } else if (item.value >= 1000) {
      valueLabel.textContent = (item.value / 1000).toFixed(0) + "K";
    } else {
      valueLabel.textContent = item.value.toLocaleString();
    }

    valueLabel.style.cssText = `
      font-size: 12px;
      font-weight: bold;
      color: #333;
      margin-bottom: 5px;
      text-align: center;
      white-space: nowrap;
    `;

    const bar = document.createElement("div");
    bar.style.cssText = `
      width: 100%;
      height: ${barHeight}px;
      background: linear-gradient(135deg, #4285f4 0%, #34a853 100%);
      border-radius: 8px 8px 4px 4px;
      transition: all 0.3s ease;
      cursor: ${selectedTimeRange.value === 'nam' ? 'pointer' : 'default'};
      box-shadow: 0 2px 8px rgba(66, 133, 244, 0.3);
    `;

    bar.addEventListener("mouseenter", () => {
      bar.style.transform = "scale(1.05) translateY(-5px)";
      bar.style.boxShadow = "0 8px 16px rgba(66, 133, 244, 0.4)";
    });
    
    bar.addEventListener("mouseleave", () => {
      bar.style.transform = "scale(1) translateY(0)";
      bar.style.boxShadow = "0 2px 8px rgba(66, 133, 244, 0.3)";
    });

    // Add click event for yearly view
    if (selectedTimeRange.value === 'nam') {
      bar.addEventListener("click", () => {
        handleBarClick(parseInt(item.x) - 1); // Convert to 0-based index
      });
    }

    const xLabel = document.createElement("div");
    if (selectedTimeRange.value === 'thang') {
      xLabel.textContent = `${item.x}`;
    } else {
      xLabel.textContent = `T${item.x}`;
    }
    
    xLabel.style.cssText = `
      font-size: 12px;
      color: #666;
      margin-top: 10px;
      text-align: center;
      font-weight: 500;
      white-space: nowrap;
    `;

    barContainer.append(valueLabel, bar, xLabel);
    chartDiv.appendChild(barContainer);
  });

  chartContainer.value.appendChild(chartDiv);
};

// Load chart data with optional month/year parameters
const loadChartData = async (timeRange, month = null, year = null) => {
  console.log("Loading chart data for:", timeRange, "Month:", month, "Year:", year);

  try {
    isLoading.value = true;
    debugInfo.value = "Loading data...";

    let endpoint, params;
    
    if (timeRange === "thang") {
      endpoint = "/ThongKe/DoanhThuTheoThang";
      params = {
        month: month || selectedMonth.value,
        year: year || selectedYear.value
      };
    } else {
      endpoint = "/ThongKe/DoanhThuNamHienTai";
      params = {
        year: year || selectedYear.value
      };
    }
    let data;
    try {
      data = await apiClient.get(endpoint, { params });
    } catch (apiError) {
      data = [];
    }

    const formattedData = formatDataForChart(data);
    const fullChartData = getFullChartData(formattedData, timeRange);
    chartData.value = fullChartData;

    if (fullChartData.length > 0 && fullChartData.some(item => item.value > 0)) {
      await nextTick();
      setTimeout(() => {
        createChart(fullChartData);
      }, 100);
    } else {
      chartData.value = [];
      debugInfo.value = "No data after formatting";
    }
  } catch (error) {
    console.error("Error loading chart data:", error);
    debugInfo.value = `Error: ${error.message}`;
    chartData.value = [];
  } finally {
    isLoading.value = false;
  }
};

// Change time range
const changeTimeRange = (timeRange) => {
  selectedTimeRange.value = timeRange;
  loadChartData(timeRange);
};

// Handle month/year change for monthly view
const onMonthYearChange = () => {
  loadChartData('thang', selectedMonth.value, selectedYear.value);
};

// Handle year change for yearly view
const onYearChange = () => {
  loadChartData('nam', null, selectedYear.value);
};

// Load service data for pie chart
const loadServiceData = async () => {
  console.log("Loading service data...");

  try {
    isLoadingService.value = true;
    const response = await apiClient.get("/ThongKe/SoLuongDichVu");
    console.log("Service API Response:", response);

    if (Array.isArray(response) && response.length > 0) {
      serviceData.value = response;
    } else {
      serviceData.value = [];
    }

    if (serviceData.value.length > 0) {
      await nextTick();
      setTimeout(() => {
        createPieChart();
      }, 100);
    }
  } catch (error) {
    console.error("Error loading service data:", error);
    serviceData.value = [];
  } finally {
    isLoadingService.value = false;
  }
};

// Create pie chart
const createPieChart = () => {
  console.log("Creating pie chart with data:", serviceData.value);

  if (!pieChart.value) {
    console.error("Pie chart canvas not found");
    return;
  }

  if (!serviceData.value || serviceData.value.length === 0) {
    console.warn("No service data for pie chart");
    return;
  }

  if (pieChartInstance) {
    pieChartInstance.destroy();
  }

  const ctx = pieChart.value.getContext("2d");

  pieChartInstance = new Chart(ctx, {
    type: "pie",
    data: {
      labels: serviceData.value.map((item) => item.serviceName),
      datasets: [
        {
          data: serviceData.value.map((item) => item.phanTram),
          backgroundColor: [
            "#FF6B6B",
            "#4ECDC4",
            "#45B7D1",
            "#96CEB4",
            "#FFEEAD",
            "#D4A5A5",
            "#95E1D3",
            "#F38BA8",
          ],
          borderWidth: 2,
          borderColor: "#fff",
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: "right",
          labels: {
            font: {
              size: 12,
              family: "Arial",
              weight: "500",
            },
            color: "#333",
            padding: 15,
            usePointStyle: true,
          },
        },
        tooltip: {
          callbacks: {
            label: (context) => {
              const label = context.label || "";
              const value = context.parsed || 0;
              const dataItem = serviceData.value[context.dataIndex];
              return [
                `${label}`,
                `Số lượng: ${dataItem.soLuong.toLocaleString()}`,
                `Tỷ lệ: ${value.toFixed(2)}%`,
              ];
            },
          },
          backgroundColor: "rgba(255, 255, 255, 0.95)",
          titleColor: "#333",
          bodyColor: "#666",
          borderColor: "#ddd",
          borderWidth: 1,
        },
      },
      animation: {
        animateScale: true,
        animateRotate: true,
        duration: 1000,
      },
    },
  });

  console.log("Pie chart created successfully");
};

// Product pie chart variables
const productPieChart = ref(null);
const productData = ref([]);
let productPieChartInstance = null;
const isLoadingProduct = ref(false);

// Load product data for pie chart
const loadProductData = async () => {
  try {
    isLoadingProduct.value = true;
    const response = await apiClient.get("/ThongKe/SoLuongSanPham");
    if (Array.isArray(response) && response.length > 0) {
      productData.value = response;
    } else {
      productData.value = [];
    }
    if (productData.value.length > 0) {
      await nextTick();
      setTimeout(() => {
        createProductPieChart();
      }, 100);
    }
  } catch (error) {
    console.error("Error loading product data:", error);
    productData.value = [];
  } finally {
    isLoadingProduct.value = false;
  }
};

const createProductPieChart = () => {
  if (!productPieChart.value) return;
  if (!productData.value || productData.value.length === 0) return;
  if (productPieChartInstance) productPieChartInstance.destroy();

  const ctx = productPieChart.value.getContext("2d");
  productPieChartInstance = new Chart(ctx, {
    type: "pie",
    data: {
      labels: productData.value.map((item) => item.productName),
      datasets: [
        {
          data: productData.value.map((item) => item.phanTram),
          backgroundColor: [
            "#FF6B6B",
            "#4ECDC4",
            "#45B7D1",
            "#96CEB4",
            "#FFEEAD",
            "#D4A5A5",
            "#95E1D3",
            "#F38BA8",
          ],
          borderWidth: 2,
          borderColor: "#fff",
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: "right",
          labels: {
            font: { size: 12, family: "Arial", weight: "500" },
            color: "#333",
            padding: 15,
            usePointStyle: true,
          },
        },
        tooltip: {
          callbacks: {
            label: (context) => {
              const label = context.label || "";
              const value = context.parsed || 0;
              const dataItem = productData.value[context.dataIndex];
              return [
                `${label}`,
                `Số lượng: ${dataItem.soLuong.toLocaleString()}`,
                `Tỷ lệ: ${value.toFixed(2)}%`,
              ];
            },
          },
          backgroundColor: "rgba(255, 255, 255, 0.95)",
          titleColor: "#333",
          bodyColor: "#666",
          borderColor: "#ddd",
          borderWidth: 1,
        },
      },
      animation: {
        animateScale: true,
        animateRotate: true,
        duration: 1000,
      },
    },
  });
};

// Watch for data changes
watch(chartData, (newData) => {
  if (newData && newData.length > 0 && chartContainer.value) {
    nextTick(() => {
      createChart(newData);
    });
  }
});

watch(serviceData, (newData) => {
  if (newData && newData.length > 0 && pieChart.value) {
    nextTick(() => {
      createPieChart();
    });
  }
});

watch(productData, (newData) => {
  if (newData && newData.length > 0 && productPieChart.value) {
    nextTick(() => {
      createProductPieChart();
    });
  }
});

// Load appointments and sort by time, filter by "Chưa đến"
const getLichHenHomNay = async () => {
  try {
    const res = await apiClient.get("/DatLich");
    console.log("Appointments API Response:", res);
    if (Array.isArray(res)) {
      const today = new Date().toISOString().slice(0, 10); // yyyy-mm-dd
      lichHenHienThi.value = res
        .filter(
          (lich) =>
            lich.thoiGian.slice(0, 10) === today &&
            lich.trangThai === "Chưa đến" &&
            lich.datTruoc == true
        )
        .sort((a, b) => new Date(a.thoiGian) - new Date(b.thoiGian));
    } else {
      console.error("API response is not an array:", res);
      lichHenHienThi.value = [];
    }
  } catch (err) {
    console.error("Lỗi khi lấy lịch hẹn:", err);
    lichHenHienThi.value = [];
  }
};

// Get color based on status
const getColor = (lich) => {
  return lich.trangThai === "Chưa đến" ? "#ff9800" : "#9e9e9e"; // Orange for Chưa đến, gray for others
};

// Get service names
const getTenDichVu = (lich) => {
  return lich.chiTietDatLichs.map((dv) => dv.dichVu.tenDichVu).join(", ");
};

// Format start time
const formatGio = (isoTime) => {
  const date = new Date(isoTime);
  return date.toLocaleTimeString("vi-VN", {
    hour: "2-digit",
    minute: "2-digit",
  });
};

// Calculate end time
const tinhGioKetThuc = (lich) => {
  const start = new Date(lich.thoiGian);
  const totalMinutes = lich.chiTietDatLichs.reduce(
    (sum, dv) => sum + dv.dichVu.thoiGian,
    0
  );
  start.setMinutes(start.getMinutes() + totalMinutes);
  return start.toLocaleTimeString("vi-VN", {
    hour: "2-digit",
    minute: "2-digit",
  });
};

// Initialize component
onMounted(async () => {
  console.log("Component mounted, initializing...");

  // Initialize years dropdown
  initializeYears();

  try {
    // Load basic stats
    try {
      const revenueData = await apiClient.get("/ThongKe/TongTienHomNay");
      doanhThu.value = revenueData.tongTien || 0;

      const appointmentData = await apiClient.get("/ThongKe/lichHenToday");
      lichHen.value = appointmentData.soLichHomNay || 0;

      const serviceCompletedData = await apiClient.get(
        "/ThongKe/dvHoanThanhToday"
      );
      dvHoanThanh.value = serviceCompletedData.soLuotDichVu || 0;
      const danhgia = await apiClient.get("/DanhGia/trungbinh-trongso");
      danhGia.value = danhgia || 0;
      await getLichHenHomNay();
      console.log(lichHenHienThi.value);
    } catch (statsError) {
      console.warn("Error loading stats, using defaults:", statsError);
    }

    // Load charts - mặc định load tháng hiện tại
    await Promise.all([
      loadServiceData(),
      loadProductData(),
      loadChartData(selectedTimeRange.value, selectedMonth.value, selectedYear.value),
    ]);
  } catch (error) {
    console.error("Initialization error:", error);
  }
});

</script>

<style scoped>
.card {
  animation: fadeIn 0.6s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.stat-card {
  cursor: pointer;
}

.stat-icon {
  font-size: 3rem;
  opacity: 0.9;
  filter: drop-shadow(0 2px 8px rgba(0, 0, 0, 0.2));
}

.stat-number {
  text-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
}

.stat-label {
  opacity: 0.9;
  letter-spacing: 0.5px;
}

.progress-bar {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(
    90deg,
    rgba(255, 255, 255, 0.3),
    rgba(255, 255, 255, 0.7),
    rgba(255, 255, 255, 0.3)
  );
}

.btn-group .btn.active {
  background-color: #0d6efd;
  color: white;
  border-color: #0d6efd;
}

#revenueChart::-webkit-scrollbar {
  height: 8px;
}

#revenueChart::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
}

#revenueChart::-webkit-scrollbar-thumb {
  background: linear-gradient(135deg, #4285f4 0%, #34a853 100%);
  border-radius: 10px;
}

#revenueChart::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(135deg, #3367d6 0%, #2e7d32 100%);
}

@media (max-width: 768px) {
  .stat-number {
    font-size: 1.5rem !important;
  }
  .stat-icon {
    font-size: 2.5rem !important;
  }
  .btn-group {
    flex-direction: column;
    width: 100%;
  }
  .btn-group .btn {
    border-radius: 8px !important;
    margin-bottom: 5px;
  }
  .d-flex.gap-3.flex-wrap {
    flex-direction: column;
    align-items: flex-start;
  }
  .d-flex.gap-2 {
    margin-bottom: 10px;
  }
}

.list-group-item {
  transition: background-color 0.3s ease;
}

.list-group-item:hover {
  background-color: #f8f9fa;
}

.badge {
  width: 12px;
  height: 12px;
}

/* Custom scrollbar for appointment list */
.card-body {
  scrollbar-width: thin;
  scrollbar-color: #0d6efd #f1f1f1;
}

.card-body::-webkit-scrollbar {
  width: 6px;
}

.card-body::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
}

.card-body::-webkit-scrollbar-thumb {
  background: #0d6efd;
  border-radius: 10px;
}

.card-body::-webkit-scrollbar-thumb:hover {
  background: #0b5ed7;
}

/* Form select styling */
.form-select-sm {
  font-size: 0.875rem;
  border-radius: 8px;
  border: 1px solid #dee2e6;
  transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
}

.form-select-sm:focus {
  border-color: #86b7fe;
  outline: 0;
  box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.25);
}
</style>