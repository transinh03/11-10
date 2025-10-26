<template>
  <div class="container py-5">
    <h2 class="text-soft-red fw-bold mb-4 text-center">
      Chi tiết dịch vụ thư giãn
    </h2>

    <div v-if="service" class="row g-4">
      <!-- Hình ảnh dịch vụ -->
      <div class="col-md-6">
        <div class="shadow rounded-4 overflow-hidden">
          <img
            :src="getImageUrl(service.hinhAnh)"
            class="img-fluid w-100"
            style="object-fit: contain; max-height: 450px; background: #fefafa"
            alt="Hình ảnh dịch vụ"
          />
        </div>
      </div>

      <!-- Nội dung chi tiết -->
      <div class="col-md-6">
        <div class="card border-0 h-100 shadow-sm rounded-4 p-4">
          <h3 class="text-soft-red fw-bold mb-3">
            <i class="bi bi-flower1 me-2"></i>{{ service.tenDichVu }}
          </h3>
          <p class="text-muted">{{ service.moTa }}</p>

          <ul class="list-unstyled my-3">
            <li>
              <i class="bi bi-cash-coin text-success"></i>
              <strong>Giá:</strong> {{ service.gia.toLocaleString() }} VND
            </li>
            <li>
              <i class="bi bi-clock-history text-warning"></i>
              <strong>Thời gian:</strong> {{ service.thoiGian }} phút
            </li>
            <li>
              <i class="bi bi-star-fill text-warning"></i>
              <strong>Đánh giá:</strong> 4.8/5 (200+ lượt)
            </li>
            <li>
              <i class="bi bi-heart-pulse text-danger"></i>
              <strong>Lợi ích:</strong>
              Cải thiện tuần hoàn máu, giảm căng thẳng, trẻ hóa làn da, thư giãn
              tinh thần.
            </li>
            <li>
              <i class="bi bi-person-arms-up text-info"></i>
              <strong>Phù hợp với:</strong> Người làm việc văn phòng, mẹ bỉm
              sữa, người cao tuổi.
            </li>
          </ul>

          <!-- Nút đặt ngay -->
          <div class="mt-auto text-center">
            <button
              class="btn btn-lg btn-success px-4 py-2 rounded-pill shadow"
              @click="datNgay"
            >
              <i class="bi bi-cart-check-fill me-2"></i>Đặt ngay
            </button>
            <p class="mt-2 text-muted small">
              🎁 Ưu đãi 20% hôm nay - Miễn phí tư vấn trước khi thực hiện!
            </p>
          </div>
        </div>
      </div>
    </div>

    <div v-else class="text-muted text-center">Đang tải dữ liệu...</div>
  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import { useRoute, useRouter } from "vue-router";

import axiosClient from "../utils/axiosClient";

const api_url = import.meta.env.VITE_BASE_URL;
const route = useRoute();
const router = useRouter();
const service = ref(null);

const getImageUrl = (path) => `${api_url}${path}`;

const datNgay = () => {
  if (service.value) {
    router.push({ name: "ThanhToan", params: { id: service.value.id } });
  }
};

onMounted(async () => {
  try {
    const res = await axiosClient.get(`${route.params.id}`);
    service.value = res;
  } catch (err) {
    console.error("Không tải được dịch vụ:", err);
  }
});
</script>

<style scoped>
.text-soft-red {
  color: #b94c63; /* Màu mận nhạt */
}

.card {
  transition: transform 0.2s ease-in-out;
}
.card:hover {
  transform: translateY(-5px);
}
</style>
