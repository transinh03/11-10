<template>
  <div class="service-management container d-flex flex-column align-items-center mt-5">
    <!-- Tiêu đề -->
    <h2 class="page-title mb-4">
      <i class="fa fa-cogs me-2"></i> Quản lý Dịch vụ
    </h2>

    <!-- Các lựa chọn -->
    <div class="d-flex flex-wrap justify-content-center gap-4">
      <div class="menu-card card-gradient-blue" @click="DichVu">
        <i class="fa fa-gear icon"></i>
        <h3>Dịch vụ</h3>
        <p>Quản lý thông tin dịch vụ của spa</p>
      </div>

      <div class="menu-card card-gradient-purple" @click="LoaiDichVu">
        <i class="fa fa-list-alt icon"></i>
        <h3>Loại dịch vụ</h3>
        <p>Phân loại và tổ chức dịch vụ</p>
      </div>

      <!-- <div class="menu-card card-gradient-pink" @click="Banggiadichvu">
        <i class="fa fa-money-bill-wave icon"></i>
        <h3>Bảng giá</h3>
        <p>Quản lý giá và khuyến mãi dịch vụ</p>
      </div> -->
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router';
const router = useRouter();

const DichVu = () => router.push("/dichVu");
const LoaiDichVu = () => router.push("/Loaidichvu");
const Banggiadichvu = () => router.push("/Banggiadichvu");
</script>

<style scoped>
/* Tiêu đề */
.page-title {
  font-size: 2.5rem;
  font-weight: bold;
  color: #5a4ff3;
  display: flex;
  align-items: center;
  gap: 10px;
}

/* Card lựa chọn */
.menu-card {
  width: 260px;
  height: 180px;
  border-radius: 20px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  color: white;
  cursor: pointer;
  text-align: center;
  box-shadow: 0 8px 20px rgba(0,0,0,0.15);
  transition: all 0.3s ease;
  padding: 20px;
}

.menu-card .icon {
  font-size: 2.8rem;
  margin-bottom: 10px;
}

.menu-card h3 {
  margin-bottom: 5px;
  font-weight: bold;
}

.menu-card p {
  font-size: 0.95rem;
  opacity: 0.85;
}

/* Hiệu ứng hover */
.menu-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 12px 25px rgba(0,0,0,0.2);
}

/* Gradient cards */
.card-gradient-blue {
  background: linear-gradient(135deg, #4facfe, #00f2fe);
}

.card-gradient-purple {
  background: linear-gradient(135deg, #667eea, #764ba2);
}

.card-gradient-pink {
  background: linear-gradient(135deg, #ff6a88, #ff99ac);
}

/* Responsive */
@media (max-width: 768px) {
  .menu-card {
    width: 100%;
    max-width: 300px;
  }
}
</style>
