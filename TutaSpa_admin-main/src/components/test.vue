<template>
  <div>
    <button class="btn btn-warning" @click="taoMaChuyenKhoan">
      Tạo mã chuyển khoản
    </button>

    <!-- Modal -->
    <div v-if="showModal" class="modal-backdrop">
      <div class="modal-container">
        <h5>Quét QR để thanh toán</h5>

        <!-- Render QR từ chuỗi -->
        <qrcode-vue :value="qrCode" :size="250" level="M" />

        <p style="margin-top: 10px">
          <a :href="checkoutUrl" target="_blank">
            Hoặc bấm vào đây để thanh toán
          </a>
        </p>
        <button class="btn btn-secondary" @click="showModal = false">
          Đóng
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import apiClient from "../utils/axiosClient";
import { ref } from "vue";
import QrcodeVue from "qrcode.vue";

const showModal = ref(false);
const qrCode = ref("");
const checkoutUrl = ref("");

const taoMaChuyenKhoan = async () => {
  try {
    const res = await apiClient.post("/ThanhToan/create-link", {
      totalAmount: 3000,
      description: "Thanh toán dịch vụ Spa",
      items: [
        { name: "Gội đầu", quantity: 1, amount: 50000 },
        { name: "Massage", quantity: 1, amount: 100000 },
      ],
      cancelUrl: window.location.origin + "/cancel",
      returnUrl: window.location.origin + "/success",
    });

    // Lấy dữ liệu từ API trả về
    qrCode.value = res.qrCode; // ✅ chuỗi QR string
    checkoutUrl.value = res.checkoutUrl;
    showModal.value = true;
  } catch (err) {
    console.error("Lỗi tạo link:", err);
  }
};
</script>

<style>
.modal-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
}
.modal-container {
  background: #fff;
  padding: 20px;
  border-radius: 12px;
  text-align: center;
  max-width: 400px;
  width: 100%;
}
</style>
