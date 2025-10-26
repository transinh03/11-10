<template>
  <div class="cashier-management">
    <!-- Header -->
    <div class="header">
      <h1 class="title">
        <i class="fas fa-cash-register"></i>
        Thu Ngân
      </h1>
    </div>

    <div class="main-content">
      <!-- Danh sách đã chọn -->
      <div class="card selected-items-section">
        <div class="card-header">
          <h2 class="section-title">
            <i class="fas fa-shopping-cart"></i>
            Danh sách đã chọn
          </h2>
          <div class="header-controls">
            <div class="phone-input-container">
              <label class="phone-label">Số điện thoại:</label>
              <input
                type="text"
                class="phone-input"
                v-model="soDienThoai"
                placeholder="Nhập số điện thoại khách hàng"
                @input="filterNumeric"
              />
            </div>
            <span class="item-count-badge">
              {{ danhSachChon.length }} mục
            </span>
          </div>
        </div>

        <div class="card-body">
          <!-- Selected Items List -->
          <div class="selected-items-list">
            <template v-if="danhSachChon.length > 0">
              <div
                v-for="(item, index) in danhSachChon"
                :key="index"
                class="selected-item"
              >
                <div class="item-info">
                  <h3 class="item-name">{{ item.ten }}</h3>
                  <p class="item-price">
                    Đơn giá: {{ item.donGia.toLocaleString() }} ₫
                  </p>
                </div>
                <div class="quantity-controls">
                  <div class="quantity-input-group">
                    <button
                      class="btn btn-outline-secondary btn-sm"
                      @click="
                        item.soLuong > 1 && item.soLuong--;
                        capNhatThanhTien(index);
                      "
                    >
                      <i class="fas fa-minus"></i>
                    </button>
                    <input
                      type="number"
                      min="1"
                      class="quantity-input"
                      v-model.number="item.soLuong"
                      @input="capNhatThanhTien(index)"
                    />
                    <button
                      class="btn btn-outline-primary btn-sm"
                      @click="
                        item.soLuong++;
                        capNhatThanhTien(index);
                      "
                    >
                      <i class="fas fa-plus"></i>
                    </button>
                  </div>
                  <div class="item-total">
                    Thành tiền:
                    <strong>{{ item.thanhTien.toLocaleString() }} ₫</strong>
                  </div>
                </div>
                <div class="item-actions">
                  <button
                    class="btn btn-danger btn-sm"
                    @click="xoaItem(index)"
                    title="Xóa item"
                  >
                    <i class="fas fa-trash-alt"></i>
                  </button>
                </div>
              </div>
            </template>
            <div v-else class="empty-state">
              <i class="fas fa-box-open"></i>
              <p>Chưa có dịch vụ hoặc sản phẩm nào được chọn.</p>
            </div>
          </div>

          <!-- Payment Footer -->
          <div class="payment-footer">
            <!-- Payment Method -->
            <div class="payment-method-section">
              <label class="section-label">Hình thức thanh toán:</label>
              <div class="radio-group">
                <div class="radio-item">
                  <input
                    type="radio"
                    id="tienMat"
                    value="Tiền mặt"
                    v-model="hinhThuc"
                    class="radio-input"
                  />
                  <label for="tienMat" class="radio-label">Tiền mặt</label>
                </div>
                <div class="radio-item">
                  <input
                    type="radio"
                    id="chuyenKhoan"
                    value="Chuyển khoản"
                    v-model="hinhThuc"
                    class="radio-input"
                  />
                  <label for="chuyenKhoan" class="radio-label"
                    >Chuyển khoản</label
                  >
                </div>
              </div>
              <div v-if="isSuaLich" class="form-group-inline">
                <label class="form-label">Chọn khung giờ:</label>
                <select
                  class="form-control"
                  v-model="selectedKhungGio"
                  @change="capNhatThoiGianLich"
                >
                  <option value="" disabled>Chọn khung giờ</option>
                  <option
                    v-for="slot in availableSlots"
                    :key="slot.khungGio"
                    :value="slot.khungGio"
                    :disabled="slot.conLai === 0"
                  >
                    {{ slot.khungGio }} (Còn {{ slot.conLai }} slot)
                  </option>
                </select>
              </div>
            </div>

            <!-- Payment Details -->
            <div class="payment-details">
              <!-- Voucher Section -->
              <div class="form-group">
                <label class="form-label">Mã giảm giá:</label>
                <input
                  type="text"
                  class="form-control"
                  v-model="maGiamGia"
                  placeholder="Nhập mã"
                  @input="apDungMaGiamGia"
                />
                <div v-if="trangThaiGiamGia" class="voucher-status mt-1">
                  <span
                    :class="{
                      'text-success': trangThaiGiamGia.startsWith('✅'),
                      'text-danger': trangThaiGiamGia.startsWith('❌'),
                    }"
                  >
                    {{ trangThaiGiamGia }}
                  </span>
                </div>
              </div>

              <!-- Cash Payment -->
              <div v-if="hinhThuc === 'Tiền mặt'" class="form-group">
                <label class="form-label">Tiền khách đưa:</label>
                <input
                  type="text"
                  class="form-control"
                  v-model="tienKhachDuaHienThi"
                  @keypress="chiNhapSo"
                  @input="xuLyNhapTienKhach"
                  placeholder="Nhập số tiền"
                />
                <div class="change-info">
                  <div
                    v-if="tienKhachDua < tongTien && tienKhachDua >= 0"
                    class="text-danger"
                  >
                    ❌ Thiếu
                    {{ (tongTien - tienKhachDua).toLocaleString("vi-VN") }} ₫
                  </div>
                  <div
                    v-else-if="tienKhachDua >= tongTien"
                    class="text-success"
                  >
                    ✅ Tiền thối lại:
                    <strong
                      >{{
                        (tienKhachDua - tongTien).toLocaleString("vi-VN")
                      }}
                      ₫</strong
                    >
                  </div>
                  <div v-else class="text-warning">
                    ⚠ Vui lòng nhập số tiền hợp lệ
                  </div>
                </div>
              </div>

              <!-- Transfer Payment -->
              <div v-if="hinhThuc === 'Chuyển khoản'" class="form-group">
                <button
                  class="btn btn-warning"
                  @click="taoMaChuyenKhoan"
                  :disabled="danhSachChon.length === 0 || !soDienThoai.trim()"
                >
                  <i class="fas fa-qrcode"></i> Tạo mã
                </button>
              </div>

              <!-- Notes -->
              <div class="form-group">
                <label class="form-label">Ghi chú:</label>
                <textarea
                  class="form-control"
                  v-model="ghiChu"
                  rows="2"
                  placeholder="Ghi chú thêm (nếu có)"
                ></textarea>
              </div>
            </div>

            <!-- Total and Actions -->
            <div class="total-actions">
              <div class="total-amount">
                <h3>Tổng tiền: {{ tongTien.toLocaleString() }} ₫</h3>
              </div>
              <div class="action-buttons">
                <button
                  v-if="isSuaLich"
                  class="btn btn-secondary"
                  @click="huySua"
                >
                  <i class="fas fa-times"></i>
                  Hủy sửa
                </button>
                <button
                  class="btn btn-info"
                  @click="isSuaLich ? capNhatLich() : datLich()"
                  :disabled="danhSachChon.length === 0 || !soDienThoai.trim()"
                >
                  <i class="fas fa-calendar-plus"></i>
                  {{ isSuaLich ? "Lưu thay đổi" : "Đặt lịch" }}
                </button>
                <button
                  class="btn btn-success"
                  @click="taoThanhToan"
                  :disabled="
                    danhSachChon.length === 0 ||
                    (hinhThuc === 'Tiền mặt' && tienKhachDua < tongTien) ||
                    hinhThuc === 'Chuyển khoản' ||
                    !soDienThoai.trim()
                  "
                >
                  <i class="fas fa-money-bill-wave"></i>
                  Tạo thanh toán
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Services/Products Tabs -->
      <div class="card tabs-section">
        <div class="card-header tabs-header">
          <div class="tab-buttons">
            <button
              class="tab-button"
              :class="{ active: tab === 'dichVu' }"
              @click="tab = 'dichVu'"
            >
              <i class="fas fa-concierge-bell"></i>
              Dịch vụ
            </button>
            <button
              class="tab-button"
              :class="{ active: tab === 'sanPham' }"
              @click="tab = 'sanPham'"
            >
              <i class="fas fa-box"></i>
              Sản phẩm
            </button>
          </div>
        </div>
        <div class="card-body">
          <div class="items-grid">
            <div
              v-for="item in tab === 'dichVu' ? dichVus : sanPhams"
              :key="tab === 'dichVu' ? item.dichVuID : item.sanPhamId"
              class="item-card"
            >
              <div class="item-content">
                <h4 class="item-title">
                  {{ tab === "dichVu" ? item.tenDichVu : item.tenSP }}
                </h4>
                <p class="item-price">
                  {{
                    (tab === "dichVu" ? item.gia : item.gia).toLocaleString()
                  }}
                  ₫
                </p>
              </div>
              <button
                class="btn btn-primary btn-sm add-btn"
                @click="tab === 'dichVu' ? chonDichVu(item) : chonSanPham(item)"
              >
                <i class="fas fa-plus"></i>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Booking List -->
    <div class="card booking-list-section">
      <div class="card-header">
        <h2 class="section-title">
          <i class="fas fa-calendar-alt"></i>
          Danh sách đặt lịch
        </h2>
        <button @click="layDanhSach" class="btn btn-outline-primary">
          <i class="fas fa-sync-alt"></i>
          Làm mới
        </button>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="booking-table">
            <thead>
              <tr>
                <th>#</th>
                <th>SĐT</th>
                <th>Thời gian</th>
                <th>Thời lượng</th>
                <th>Trạng thái</th>
                <th>Thanh toán</th>
                <th>Dịch vụ</th>
                <th>Ghi Chú</th>
                <th>Hành động</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="(lich, index) in danhSachDatLich"
                :key="lich.datLichID"
                class="booking-row"
              >
                <td>{{ index + 1 }}</td>
                <td class="phone-cell">{{ lich.soDienThoai }}</td>
                <td class="datetime-cell">
                  {{ formatDateTime(lich.thoiGian) }}
                </td>
                <td class="duration-cell">{{ lich.thoiLuong }} phút</td>
                <td class="status-cell">
                  <span
                    class="status-badge"
                    :class="
                      lich.trangThai === 'Đã đến'
                        ? 'status-arrived'
                        : 'status-waiting'
                    "
                  >
                    {{ lich.trangThai }}
                  </span>
                </td>
                <td class="payment-cell">
                  <span
                    class="payment-badge"
                    :class="
                      lich.daThanhToan ? 'payment-paid' : 'payment-unpaid'
                    "
                  >
                    {{
                      lich.daThanhToan
                        ? "✓ Đã thanh toán"
                        : "❌ Chưa thanh toán"
                    }}
                  </span>
                </td>
                <td class="services-cell">
                  <ul class="services-list">
                    <li
                      v-for="dv in lich.chiTietDatLichs"
                      :key="dv.chiTietDatLichID"
                      class="service-item"
                    >
                      {{ dv.dichVu.tenDichVu }} - {{ dv.dichVu.thoiGian }}p -
                      {{ dv.dichVu.gia.toLocaleString() }}₫
                    </li>
                  </ul>
                </td>
                <td class="notes-cell">{{ lich.ghiChu }}</td>
                <td class="actions-cell">
                  <div class="action-buttons">
                    <button
                      v-if="!lich.daThanhToan"
                      class="btn btn-sm btn-info"
                      @click="doiTrangThai(lich.datLichID)"
                      title="Đổi trạng thái"
                    >
                      <i class="fas fa-exchange-alt"></i>
                    </button>
                    <button
                      v-if="!lich.daThanhToan"
                      class="btn btn-sm btn-warning"
                      @click="batDauSuaLich(lich)"
                      title="Sửa lịch"
                    >
                      <i class="fas fa-edit"></i>
                    </button>
                    <button
                      v-if="!lich.daThanhToan"
                      class="btn btn-sm btn-danger"
                      @click="xoaLich(lich.datLichID)"
                      title="Xóa lịch"
                    >
                      <i class="fas fa-trash"></i>
                    </button>
                  </div>
                </td>
              </tr>
              <tr v-if="danhSachDatLich.length === 0">
                <td colspan="9" class="empty-row">
                  <div class="empty-state">
                    <i class="fas fa-calendar-times"></i>
                    <p>Không có dữ liệu</p>
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
import { ref, computed, onMounted, onBeforeUnmount } from "vue";
import { useRoute } from "vue-router";
import dayjs from "dayjs";
import apiClient from "../utils/axiosClient";
import connection from "../services/bookingService";
import Swal from "sweetalert2";
const route = useRoute();

// Reactive data
const tab = ref("dichVu");
const dichVus = ref([]);
const sanPhams = ref([]);
const danhSachChon = ref([]);
const hinhThuc = ref("Tiền mặt");
const tienKhachDua = ref(0);
const tienKhachDuaHienThi = ref("");
const danhSachDatLich = ref([]);
const soDienThoai = ref("");
const ghiChu = ref("");
const isSuaLich = ref(false);
const lichDangSua = ref(null);
const maGiamGia = ref("");
const giamGia = ref(0);
const trangThaiGiamGia = ref("");
const toasts = ref([]);
let debounceTimer = null;
const availableSlots = ref([]);
const selectedKhungGio = ref("");
const isLoading = ref(false);

// Toast methods
const showToast = (message, type = "info") => {
  toasts.value.push({ id: Date.now(), message, type });
  setTimeout(() => toasts.value.shift(), 3000);
};

const getToastIcon = (type) =>
  ({
    success: "fa-check-circle",
    error: "fa-exclamation-circle",
    warning: "fa-exclamation-triangle",
    info: "fa-info-circle",
  }[type]);

// Phone validation
// const filterNumeric = (event) => {
//   let value = event.target.value.replace(/[^0-9]/g, "");
//   if (value && !value.startsWith("0")) {
//     value = "0" + value;
//   }
//   soDienThoai.value = value;
//   validatePhone();
// };
const filterNumeric = (event) => {
  let value = event.target.value.replace(/\D/g, ""); // chỉ giữ số
  if (value.length > 10) value = value.slice(0, 10); // tối đa 10 số
  if (value && !value.startsWith("0")) value = "0" + value; // bắt buộc số 0 đầu
  soDienThoai.value = value;
};

// Money input handling
const xuLyNhapTienKhach = () => {
  const so = parseInt(tienKhachDuaHienThi.value.replace(/\D/g, ""));
  tienKhachDua.value = isNaN(so) ? 0 : so;
  tienKhachDuaHienThi.value = tienKhachDua.value.toLocaleString();
};

const chiNhapSo = (e) => {
  if (!/[0-9]/.test(String.fromCharCode(e.which))) e.preventDefault();
};

// Computed total
const tongTien = computed(
  () =>
    danhSachChon.value.reduce((sum, item) => sum + item.thanhTien, 0) -
    giamGia.value
);

// Item selection methods
const chonDichVu = (dv) => {
  const existing = danhSachChon.value.find((x) => x.dichVuID === dv.dichVuID);
  if (existing) {
    existing.soLuong++;
    existing.thanhTien += dv.gia;
  } else {
    danhSachChon.value.push({
      ten: dv.tenDichVu,
      dichVuID: dv.dichVuID,
      soLuong: 1,
      donGia: dv.gia,
      thanhTien: dv.gia,
    });
  }
  showToast(`Đã thêm ${dv.tenDichVu}`, "success");
};

const chonSanPham = (sp) => {
  const existing = danhSachChon.value.find((x) => x.sanPhamID === sp.sanPhamId);
  if (existing) {
    existing.soLuong++;
    existing.thanhTien += sp.gia;
  } else {
    danhSachChon.value.push({
      ten: sp.tenSP,
      sanPhamID: sp.sanPhamId,
      soLuong: 1,
      donGia: sp.gia,
      thanhTien: sp.gia,
    });
  }
  showToast(`Đã thêm ${sp.tenSP}`, "success");
};

const capNhatThanhTien = (index) => {
  const item = danhSachChon.value[index];
  item.soLuong = Math.max(1, item.soLuong || 1);
  item.thanhTien = item.soLuong * item.donGia;
};

const xoaItem = (index) => {
  const itemName = danhSachChon.value[index].ten;
  danhSachChon.value.splice(index, 1);
  showToast(`Đã xóa ${itemName}`, "warning");
};

// Payment methods
const taoThanhToan = async () => {
  if (!/^0\d{9}$/.test(soDienThoai.value)) {
    await Swal.fire({
      icon: "error",
      title: "Số điện thoại không hợp lệ",
      text: "Vui lòng nhập đúng 10 số và bắt đầu bằng 0!",
      confirmButtonText: "OK",
    });
    return;
  }
  if (isSuaLich.value && lichDangSua.value?.datLichID) {
    if (lichDangSua.value.trangThai == "Chưa đến")
      return showToast("khách hàng chưa đến", "error");
  }
  isLoading.value = true;
  Swal.fire({
    title: "Đang xử lý...",
    text: "Vui lòng đợi trong giây lát",
    allowOutsideClick: false,
    didOpen: () => {
      Swal.showLoading();
    },
  });
  const data = {
    ngayTao: new Date().toISOString(),
    maGiamGia: maGiamGia.value.toUpperCase(),
    hinhThucThanhToan: hinhThuc.value,
    trangThai: 1,
    tienGiam: giamGia.value,
    tienKhachDua: hinhThuc.value === "Tiền mặt" ? tienKhachDua.value : null,
    tienThoiLai:
      hinhThuc.value === "Tiền mặt"
        ? Math.max(0, tienKhachDua.value - tongTien.value)
        : null,
    nhanVienID: "6c5dad3a-b67c-4ad7-9ac5-ba64b0aabd5f",
    userID: soDienThoai.value,
    chiTietHoaDon: danhSachChon.value.map((item) => ({
      sanPhamID: item.sanPhamID || null,
      dichVuID: item.dichVuID || null,
      soLuongSP: item.soLuong,
      thanhTien: item.thanhTien,
    })),
  };

  try {
    await apiClient.post("/ThanhToan/tao-hoadon", data);
    if (isSuaLich.value && lichDangSua.value?.datLichID) {
      await apiClient.put(
        `/DatLich/capnhat-thanhtoan/${lichDangSua.value.datLichID}`
      );
    }
    showToast("Tạo hóa đơn thành công!", "success");
    resetForm();
    layDanhSach();
  } catch (err) {
    console.error("Lỗi tạo hóa đơn:", err);
    showToast("Tạo hóa đơn thất bại.", "error");
  } finally {
    Swal.close();
    isLoading.value = false;
  }
};

const taoMaChuyenKhoan = async () => {
  if (!danhSachChon.value.length || !soDienThoai.value.trim()) return;
  if (!/^0\d{9}$/.test(soDienThoai.value)) {
    await Swal.fire({
      icon: "error",
      title: "Số điện thoại không hợp lệ",
      text: "Vui lòng nhập đúng 10 số và bắt đầu bằng 0!",
      confirmButtonText: "OK",
    });
    return;
  }
  if (isSuaLich.value && lichDangSua.value?.datLichID) {
    if (lichDangSua.value.trangThai == "Chưa đến")
      return showToast("khách hàng chưa đến", "error");
  }
  const payload = {
    totalAmount: tongTien.value,
    description: "Thanh toán hóa đơn",
    items: danhSachChon.value.map((item) => ({
      name: item.ten,
      quantity: item.soLuong,
      amount: item.donGia,
    })),
    cancelUrl: window.location.href,
    returnUrl: window.location.href,
  };
  localStorage.setItem(
    "checkoutData",
    JSON.stringify({
      danhSachChon: danhSachChon.value,
      hinhThuc: hinhThuc.value,
      soDienThoai: soDienThoai.value,
    })
  );

  try {
    const { checkoutUrl } = await apiClient.post(
      "/ThanhToan/create-link",
      payload
    );
    if (checkoutUrl) window.location.href = checkoutUrl;
    else showToast("Không thể tạo mã chuyển khoản.", "error");
  } catch (error) {
    console.error("Lỗi tạo mã thanh toán:", error);
    showToast("Lỗi tạo mã chuyển khoản.", "error");
  }
};

// Booking management
const datLich = async () => {
  if (!/^0\d{9}$/.test(soDienThoai.value)) {
    await Swal.fire({
      icon: "error",
      title: "Số điện thoại không hợp lệ",
      text: "Vui lòng nhập đúng 10 số và bắt đầu bằng 0!",
      confirmButtonText: "OK",
    });
    return;
  }
  const dichVusSelected = danhSachChon.value
    .filter((item) => item.dichVuID)
    .map((item) => ({ DichVuID: item.dichVuID, SoLuong: item.soLuong }));

  if (!dichVusSelected.length)
    return showToast("Vui lòng chọn ít nhất một dịch vụ.", "warning");
  if (!soDienThoai.value.trim())
    return showToast("Vui lòng nhập số điện thoại.", "warning");

  const payload = {
    SoDienThoai: soDienThoai.value,
    ThoiGian: dayjs().format("YYYY-MM-DDTHH:mm:ss"),
    GhiChu: ghiChu.value,
    DatTruoc: false,
    DichVus: dichVusSelected,
  };

  try {
    await apiClient.post("/DatLich", payload);
    showToast("Đặt lịch thành công!", "success");
    resetForm();
    layDanhSach();
  } catch (err) {
    console.error("Lỗi đặt lịch:", err);
    showToast(err.response?.data || "Đặt lịch thất bại.", "error");
  }
};

const capNhatLich = async () => {
  if (!/^0\d{9}$/.test(soDienThoai.value)) {
    await Swal.fire({
      icon: "error",
      title: "Số điện thoại không hợp lệ",
      text: "Vui lòng nhập đúng 10 số và bắt đầu bằng 0!",
      confirmButtonText: "OK",
    });
    return;
  }
  const dichVusSelected = danhSachChon.value
    .filter((item) => item.dichVuID)
    .map((item) => ({ DichVuID: item.dichVuID, SoLuong: item.soLuong }));

  if (!dichVusSelected.length)
    return showToast("Vui lòng chọn ít nhất một dịch vụ.", "warning");
  if (!selectedKhungGio.value)
    return showToast("Vui lòng chọn khung giờ.", "warning");
  const ngay = dayjs(lichDangSua.value.thoiGian).format("YYYY-MM-DD");
  const thoiGianMoi = `${ngay}T${selectedKhungGio.value}:00`;

  const payload = {
    SoDienThoai: soDienThoai.value,
    ThoiGian: thoiGianMoi,
    GhiChu: ghiChu.value,
    DatTruoc: lichDangSua.value.datTruoc || false,
    DichVus: dichVusSelected,
  };

  try {
    await apiClient.put(`/DatLich/${lichDangSua.value.datLichID}`, payload);
    showToast("Cập nhật lịch thành công!", "success");
    resetForm();
    layDanhSach();
  } catch (err) {
    console.error("Lỗi cập nhật lịch:", err);
    showToast(err.response?.data || "Cập nhật lịch thất bại.", "error");
  }
};

const batDauSuaLich = (lich) => {
  isSuaLich.value = true;
  lichDangSua.value = lich;
  soDienThoai.value = lich.soDienThoai;
  ghiChu.value = lich.ghiChu;
  danhSachChon.value = lich.chiTietDatLichs.map((ct) => ({
    ten: ct.dichVu.tenDichVu,
    dichVuID: ct.dichVuID,
    soLuong: ct.soLuongDV || 1,
    donGia: ct.dichVu.gia,
    thanhTien: ct.dichVu.gia * (ct.soLuongDV || 1),
  }));
  selectedKhungGio.value = dayjs(lich.thoiGian).format("HH:mm");
  layDanhSachSlot(lich.thoiGian);
  showToast("Đã tải thông tin lịch để chỉnh sửa", "info");
};

const huySua = () => {
  resetForm();
  showToast("Đã hủy chỉnh sửa", "info");
};

const doiTrangThai = async (id) => {
  try {
    await apiClient.put(`/DatLich/doitrangthai/${id}`);
    showToast("Đã đổi trạng thái lịch hẹn", "success");
    layDanhSach();
  } catch (err) {
    console.error("Lỗi đổi trạng thái:", err);
    showToast("Lỗi khi đổi trạng thái!", "error");
  }
};
const xoaLich = async (id) => {
  const result = await Swal.fire({
    title: "Bạn có chắc chắn?",
    text: "Lịch này sẽ bị xóa và không thể khôi phục!",
    icon: "warning",
    showCancelButton: true,
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Xóa ngay",
    cancelButtonText: "Hủy",
  });

  if (result.isConfirmed) {
    try {
      isLoading.value = true;
      Swal.fire({
        title: "Đang xử lý...",
        text: "Vui lòng đợi trong giây lát",
        allowOutsideClick: false,
        didOpen: () => {
          Swal.showLoading();
        },
      });
      await apiClient.delete(`/DatLich/${id}`);

      await layDanhSach();
      showToast("Xóa thành công!", "success");
    } catch (err) {
      console.error("Lỗi khi xóa lịch:", err);
      showToast("Xóa thất bại!", "error");
    } finally {
      Swal.close();
      isLoading.value = false;
    }
  }
};

const apDungMaGiamGia = () => {
  clearTimeout(debounceTimer);
  debounceTimer = setTimeout(async () => {
    const code = maGiamGia.value.trim().toUpperCase();
    giamGia.value = 0;
    trangThaiGiamGia.value = "";
    if (!code) return;

    try {
      const vouchers = await apiClient.get("/Vouchers/status");
      const voucher = vouchers.find((v) => v.maCode.toUpperCase() === code);

      if (!voucher) return (trangThaiGiamGia.value = "❌ Mã không tồn tại");
      if (voucher.trangThai !== "true")
        return (trangThaiGiamGia.value = `❌ Mã không hợp lệ: ${voucher.trangThai}`);

      giamGia.value =
        voucher.tienGiam < 1
          ? Math.round(tongTien.value * voucher.tienGiam)
          : voucher.tienGiam;

      trangThaiGiamGia.value = `✅ Đã áp dụng mã ${voucher.maCode}`;
      showToast(
        `Áp dụng mã giảm giá thành công: ${giamGia.value.toLocaleString()}₫`,
        "success"
      );
    } catch (err) {
      console.error("Lỗi API mã giảm giá:", err);
      trangThaiGiamGia.value = "❌ Lỗi hệ thống";
      showToast("Lỗi khi áp dụng mã giảm giá", "error");
    }
  }, 300);
};

const resetForm = () => {
  isSuaLich.value = false;
  lichDangSua.value = null;
  danhSachChon.value = [];
  soDienThoai.value = "";
  ghiChu.value = "";
  tienKhachDua.value = 0;
  tienKhachDuaHienThi.value = "";
  hinhThuc.value = "Tiền mặt";
  maGiamGia.value = "";
  giamGia.value = 0;
  trangThaiGiamGia.value = "";
  selectedKhungGio.value = "";
  availableSlots.value = [];
  localStorage.removeItem("checkoutData");
};

const layDanhSach = async () => {
  try {
    const today = dayjs().format("YYYY-MM-DD");

    const [resDL, resDV, resSP] = await Promise.all([
      apiClient.get("/DatLich/by-date", { params: { date: today } }),
      apiClient.get("/DichVu"),
      apiClient.get("/Product"),
    ]);
    danhSachDatLich.value = resDL.sort((a, b) => {
      const daThanhToanA = a.daThanhToan ?? false;
      const daThanhToanB = b.daThanhToan ?? false;
      return daThanhToanA !== daThanhToanB
        ? daThanhToanA
          ? 1
          : -1
        : new Date(a.thoiGian) - new Date(b.thoiGian);
    });

    dichVus.value = resDV;
    sanPhams.value = resSP;
  } catch (err) {
    console.error("Lỗi lấy danh sách:", err);
    showToast("Lỗi tải dữ liệu.", "error");
  }
};

const formatDateTime = (dateStr) => {
  return new Date(dateStr.replace("Z", "")).toLocaleTimeString("vi-VN", {
    hour: "2-digit",
    minute: "2-digit",
  });
};

const layDanhSachSlot = async (ngay) => {
  try {
    const response = await apiClient.get("/DatLich/slots", {
      params: { ngay: dayjs(ngay).format("YYYY-MM-DD") },
    });
    availableSlots.value = response;
  } catch (err) {
    console.error("Lỗi lấy danh sách slot:", err);
    showToast("Không thể tải danh sách khung giờ.", "error");
  }
};
// const capNhatThoiGianLich = () => {
//   if (selectedKhungGio.value && lichDangSua.value) {
//     const ngay = dayjs(lichDangSua.value.thoiGian).format("YYYY-MM-DD");
//     lichDangSua.value.thoiGian = `${ngay}T${selectedKhungGio.value}:00`;
//   }
// };

onMounted(async () => {
  try {
    const { status } = route.query;
    const storedData = localStorage.getItem("checkoutData");
    if (storedData) {
      const parsed = JSON.parse(storedData);
      danhSachChon.value = parsed.danhSachChon;
      hinhThuc.value = parsed.hinhThuc;
      soDienThoai.value = parsed.soDienThoai;
      if (status === "PAID") await taoThanhToan();
      localStorage.removeItem("checkoutData");
    }
    await layDanhSach();
    connection.on("ReceiveBookingNotification", (booking) => {
      const today = dayjs().format("YYYY-MM-DD");
      const ngay = booking.thoiGian.slice(0, 10);
      if (ngay === today) {
        booking.daThanhToan = booking.daThanhToan ?? false;
        booking.thoiGian = dayjs(booking.thoiGian).toISOString();
        danhSachDatLich.value = [...danhSachDatLich.value, booking].sort(
          (a, b) => {
            const daThanhToanA = a.daThanhToan ?? false;
            const daThanhToanB = b.daThanhToan ?? false;
            return daThanhToanA !== daThanhToanB
              ? daThanhToanA
                ? 1
                : -1
              : new Date(a.thoiGian) - new Date(b.thoiGian);
          }
        );
        showToast("Có lịch đặt mới!", "success");
      }
    });
    await connection.start();
  } catch (err) {
    console.error("Lỗi tải dữ liệu:", err);
    showToast("Lỗi tải dữ liệu.", "error");
  }
});

onBeforeUnmount(() => connection.stop());
</script>

<style scoped>
.cashier-management {
  max-width: 1400px;
  margin: 0 auto;
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
  background: #f8f9fa;
  min-height: 100vh;
}

.header {
  margin-bottom: 10px;
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

.main-content {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 20px;
  margin-bottom: 30px;
}

.card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.07);
  overflow: hidden;
  border: 1px solid #e1e8ed;
}

.card-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 10px 15px;
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
  padding: 15px;
}

/* Selected Items Section */
.selected-items-section {
  min-height: 600px;
  /* max-height: 600px; */
}

.header-controls {
  display: flex;
  align-items: center;
  gap: 20px;
  flex-wrap: wrap;
}

.phone-input-container {
  display: flex;
  align-items: center;
  gap: 10px;
  position: relative;
}

.phone-label {
  font-weight: 500;
  white-space: nowrap;
  color: white;
}

.phone-input {
  padding: 8px 12px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-radius: 6px;
  background: rgba(255, 255, 255, 0.9);
  color: #2c3e50;
  width: 200px;
}

.phone-input:focus {
  outline: none;
  border-color: white;
  background: white;
}

.error-message {
  position: absolute;
  top: 100%;
  left: 0;
  color: #ff6b6b;
  font-size: 0.8rem;
  background: white;
  padding: 2px 6px;
  border-radius: 4px;
  margin-top: 2px;
}

.item-count-badge {
  background: rgba(255, 255, 255, 0.2);
  padding: 6px 12px;
  border-radius: 20px;
  font-weight: 500;
  border: 1px solid rgba(255, 255, 255, 0.3);
}

.selected-items-list {
  max-height: 237px;
  overflow-y: auto;
  margin-bottom: 5px;
}

.selected-item {
  display: grid;
  grid-template-columns: 1fr auto auto;
  gap: 15px;
  align-items: center;
  /* padding: 15px; */
  padding-left: 25px;
  padding-right: 25px;
  border: 1px solid #e1e8ed;
  border-radius: 8px;
  margin-bottom: 10px;
  background: #f8f9fa;
}

.item-info h3 {
  margin: 0 0 5px 0;
  color: #2c3e50;
  font-size: 1.1rem;
}

.item-info p {
  margin: 0;
  color: #7f8c8d;
  font-size: 0.9rem;
}

.quantity-controls {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
}

.quantity-input-group {
  display: flex;
  align-items: center;
  border: 1px solid #ddd;
  border-radius: 6px;
  overflow: hidden;
}

.quantity-input {
  width: 60px;
  padding: 6px;
  border: none;
  text-align: center;
  background: white;
}

.quantity-input:focus {
  outline: none;
  background: #f0f8ff;
}

.item-total {
  font-size: 0.9rem;
  color: #27ae60;
  font-weight: 600;
}

.item-actions {
  display: flex;
  justify-content: center;
}

.empty-state {
  text-align: center;
  padding: 40px 20px;
  color: #7f8c8d;
}

.empty-state i {
  font-size: 3rem;
  margin-bottom: 15px;
  display: block;
  color: #bdc3c7;
}

/* Payment Footer */
.payment-footer {
  border-top: 2px solid #ecf0f1;
  padding-top: 20px;
}

.payment-method-section {
  margin-bottom: 20px;
}

.section-label {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 10px;
  display: block;
}

.radio-group {
  display: flex;
  gap: 20px;
}

.radio-item {
  display: flex;
  align-items: center;
  gap: 8px;
}

.radio-input {
  width: 18px;
  height: 18px;
}

.radio-label {
  font-weight: 500;
  color: #2c3e50;
  cursor: pointer;
}

.payment-details {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
  margin-bottom: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-label {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 8px;
  font-size: 0.95rem;
}

.form-control {
  padding: 10px 12px;
  border: 2px solid #e1e8ed;
  border-radius: 6px;
  font-size: 0.95rem;
  transition: all 0.3s ease;
}

.form-control:focus {
  outline: none;
  border-color: #3498db;
  box-shadow: 0 0 0 3px rgba(52, 152, 219, 0.1);
}

.voucher-status {
  margin-top: 5px;
  font-size: 0.85rem;
}

.change-info {
  margin-top: 8px;
  font-size: 0.9rem;
}

.total-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 15px;
}

.total-amount h3 {
  margin: 0;
  color: #27ae60;
  font-size: 1.5rem;
}

.action-buttons {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}

/* Buttons */
.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 6px;
  font-size: 0.95rem;
  font-weight: 500;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s ease;
  text-decoration: none;
}

.btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-sm {
  padding: 6px 12px;
  font-size: 0.85rem;
}

.btn-primary {
  background: linear-gradient(135deg, #3498db, #2980b9);
  color: white;
}

.btn-success {
  background: linear-gradient(135deg, #27ae60, #229954);
  color: white;
}

.btn-secondary {
  background: linear-gradient(135deg, #95a5a6, #7f8c8d);
  color: white;
}

.btn-info {
  background: linear-gradient(135deg, #3498db, #2980b9);
  color: white;
}

.btn-warning {
  background: linear-gradient(135deg, #f39c12, #e67e22);
  color: white;
}

.btn-danger {
  background: linear-gradient(135deg, #e74c3c, #c0392b);
  color: white;
}

.btn-outline-primary {
  background: transparent;
  color: #3498db;
  border: 2px solid #3498db;
}

.btn-outline-primary:hover:not(:disabled) {
  background: #3498db;
  color: white;
}

.btn-outline-secondary {
  background: transparent;
  color: #6c757d;
  border: 1px solid #6c757d;
}

/* Tabs Section */
.tabs-section {
  max-height: 600px;
}

.tabs-header {
  padding: 0;
  background: none;
}

.tab-buttons {
  display: flex;
  width: 100%;
}

.tab-button {
  flex: 1;
  padding: 15px 20px;
  border: none;
  background: #f8f9fa;
  color: #6c757d;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.tab-button.active {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.tab-button:hover:not(.active) {
  background: #e9ecef;
}

.items-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 10px;
  max-height: 500px;
  overflow-y: auto;
}

.item-card {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 15px;
  border: 1px solid #e1e8ed;
  border-radius: 8px;
  background: #f8f9fa;
  transition: all 0.3s ease;
}

.item-card:hover {
  background: #e9ecef;
  border-color: #3498db;
}

.item-content h4 {
  margin: 0 0 5px 0;
  color: #2c3e50;
  font-size: 1rem;
}

.item-content .item-price {
  margin: 0;
  color: #27ae60;
  font-weight: 600;
  font-size: 0.9rem;
}

.add-btn {
  padding: 6px 10px;
  min-width: auto;
}

/* Booking List Section */
.booking-list-section {
  grid-column: 1 / -1;
}

.table-responsive {
  overflow-x: auto;
  margin-top: 10px;
}

.booking-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
}

.booking-table th {
  background: linear-gradient(135deg, #34495e, #2c3e50);
  color: white;
  padding: 12px 10px;
  text-align: left;
  font-weight: 600;
  font-size: 0.9rem;
  border-bottom: 3px solid #3498db;
}

.booking-table td {
  padding: 12px 10px;
  border-bottom: 1px solid #ecf0f1;
  vertical-align: middle;
}

.booking-row:hover {
  background: #f8f9fa;
}

.phone-cell {
  font-family: "Courier New", monospace;
  font-weight: 600;
}

.datetime-cell {
  font-size: 0.9rem;
}

.duration-cell {
  text-align: center;
  font-weight: 600;
}

.status-badge {
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: 600;
}

.status-arrived {
  background: #d4edda;
  color: #155724;
}

.status-waiting {
  background: #fff3cd;
  color: #856404;
}

.payment-badge {
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: 600;
}

.payment-paid {
  background: #d4edda;
  color: #155724;
}

.payment-unpaid {
  background: #f8d7da;
  color: #721c24;
}

.services-list {
  margin: 0;
  padding-left: 15px;
  font-size: 0.85rem;
}

.service-item {
  margin-bottom: 3px;
}

.notes-cell {
  max-width: 150px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-size: 0.9rem;
}

.actions-cell .action-buttons {
  display: flex;
  gap: 5px;
}

.empty-row {
  text-align: center;
  padding: 40px;
}

.empty-row .empty-state {
  padding: 20px;
}

/* Toast Notifications */
.toast-container {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 1100;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.toast {
  padding: 15px 20px;
  border-radius: 8px;
  color: white;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 10px;
  min-width: 250px;
  animation: slideIn 0.3s ease;
}

.toast.success {
  background: linear-gradient(135deg, #27ae60, #229954);
}

.toast.error {
  background: linear-gradient(135deg, #e74c3c, #c0392b);
}

.toast.warning {
  background: linear-gradient(135deg, #f39c12, #e67e22);
}

.toast.info {
  background: linear-gradient(135deg, #3498db, #2980b9);
}

@keyframes slideIn {
  from {
    transform: translateX(100%);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

/* Utility Classes */
.text-success {
  color: #27ae60 !important;
}

.text-danger {
  color: #e74c3c !important;
}

.text-warning {
  color: #f39c12 !important;
}

.text-info {
  color: #3498db !important;
}

/* Responsive Design */
@media (max-width: 1200px) {
  .main-content {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .cashier-management {
    padding: 15px;
  }

  .title {
    font-size: 2rem;
  }

  .header-controls {
    flex-direction: column;
    align-items: stretch;
    gap: 10px;
  }

  .phone-input-container {
    flex-direction: column;
    align-items: stretch;
  }

  .phone-input {
    width: 100%;
  }

  .selected-item {
    grid-template-columns: 1fr;
    gap: 10px;
    text-align: center;
  }

  .payment-details {
    grid-template-columns: 1fr;
  }

  .total-actions {
    flex-direction: column;
    align-items: stretch;
  }

  .action-buttons {
    justify-content: center;
  }

  .booking-table {
    font-size: 0.8rem;
  }

  .booking-table th,
  .booking-table td {
    padding: 8px 6px;
  }

  .toast-container {
    left: 10px;
    right: 10px;
  }

  .toast {
    min-width: auto;
  }
}

@media (max-width: 480px) {
  .items-grid {
    grid-template-columns: 1fr;
  }

  .item-card {
    flex-direction: column;
    gap: 10px;
    text-align: center;
  }

  .btn {
    padding: 8px 16px;
    font-size: 0.9rem;
  }

  .btn-sm {
    padding: 6px 10px;
    font-size: 0.8rem;
  }

  .tab-button {
    padding: 12px 15px;
    font-size: 0.9rem;
  }
}
</style>
