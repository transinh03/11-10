<template>
  <div class="container py-5">
    <div v-if="service">
      <!-- Tiêu đề -->
      <div class="text-center mb-5">
        <h2 class="fw-bold text-success display-5">{{ service.tenDichVu }}</h2>
        <p class="text-muted fs-5">{{ service.moTa }}</p>
      </div>

      <!-- Box thời gian & giá -->
      <div class="row g-4 mb-5 justify-content-center" v-if="giaDichVu.length">
        <div class="col-lg-4 col-md-6" v-for="(g, i) in giaDichVu" :key="i">
          <div
            class="card h-100 shadow-sm border-0 rounded-4 overflow-hidden service-card"
          >
            <img
              :src="getImageUrl(service.hinhAnh)"
              class="card-img-top img-zoom"
              style="height: 230px; object-fit: cover"
            />
            <div class="card-body text-center">
              <h5 class="fw-bold text-primary mb-2">
                {{ service.tenDichVu }} - {{ g.thoiLuong }} phút
              </h5>
              <p class="text-success fs-5 fw-semibold">
                {{ formatCurrency(g.gia) }}
              </p>
              <router-link
                to="/dat-lich"
                class="btn btn-success rounded-pill px-4 shadow-sm btn-hover"
                >Đặt dịch vụ</router-link
              >
            </div>
          </div>
        </div>
      </div>

      <!-- Đánh giá trung bình -->
      <div class="mb-5 text-center">
        <h5 class="fw-bold mb-2">⭐ Đánh giá trung bình:</h5>
        <div v-if="trungBinh !== null">
          <div class="fs-4 text-warning mb-1">
            <i
              class="fa-solid fa-star"
              v-for="n in Math.round(trungBinh)"
              :key="n"
            ></i>
          </div>
          <span class="text-muted fs-5">{{ trungBinh.toFixed(1) }}/5</span>
        </div>
        <div v-else class="text-muted">Chưa có đánh giá</div>
      </div>

      <!-- Danh sách đánh giá -->
      <div v-if="danhGias.length" class="mt-4">
        <h5 class="fw-bold mb-4 text-primary">📢 Phản hồi từ khách hàng:</h5>
        <div
          v-for="dg in danhGias"
          :key="dg.id"
          class="border rounded-4 p-3 mb-3 shadow-sm bg-light review-card"
        >
          <div class="d-flex align-items-center justify-content-between mb-2">
            <div class="text-warning">
              <i class="fa-solid fa-star me-1" v-for="n in dg.sao" :key="n"></i>
            </div>
            <div class="text-muted small">
              {{ new Date(dg.ngayTao).toLocaleDateString() }}
            </div>
          </div>
          <p class="mb-2 fst-italic">"{{ dg.noiDung }}"</p>
          <div class="text-muted small d-flex align-items-center">
            <i class="fa-regular fa-user-circle me-1"></i>
            {{ dg.anDanh ? "Ẩn danh" : "Khách hàng" }}
          </div>
        </div>
      </div>
    </div>

    <!-- Loading -->
    <div v-else class="text-center text-muted py-5">
      <div class="spinner-border text-success mb-3" role="status"></div>
      <p>Đang tải thông tin dịch vụ...</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import axiosClient from "../utils/axiosClient";

var base_url = import.meta.env.VITE_BASE_URL.replace("/api", "/images/");
base_url = base_url.replace("/api", "");
const route = useRoute();
const id = route.params.id;

const service = ref(null);
const giaDichVu = ref([]);
const danhGias = ref([]);
const trungBinh = ref(null);

onMounted(async () => {
  try {
    const [res1, res2, res3, res4] = await Promise.all([
      axiosClient.get(`DichVu/${id}`),
      axiosClient.get(`BangGiaDichVu/GetGiaTheoThoiGian/${id}`),
      axiosClient.get(`DanhGia/dichvu/${id}`),
      axiosClient.get(`DanhGia/trungbinh/${id}`),
    ]);

    service.value = res1;
    giaDichVu.value = res2;
    danhGias.value = res3;
    trungBinh.value = res4;
  } catch (err) {
    console.error("Lỗi khi tải dữ liệu dịch vụ:", err);
  }
});

const getImageUrl = (path) => `${base_url}${path}`;
const formatCurrency = (num) =>
  new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(num);
</script>

<style scoped>
.service-card {
  transition: transform 0.3s, box-shadow 0.3s;
}
.service-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
}
.btn-hover:hover {
  transform: scale(1.05);
  transition: transform 0.2s;
}
.img-zoom {
  transition: transform 0.3s ease-in-out;
}
.img-zoom:hover {
  transform: scale(1.05);
}
.review-card {
  transition: background 0.3s ease;
}
.review-card:hover {
  background: #f8f9fa;
}
</style>
