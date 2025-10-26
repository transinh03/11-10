
<template>
  <div class="container d-flex flex-column align-items-center justify-content-start mt-5 min-vh-100" style="background-color: #ffffff;">
    <h2 class="page-title mb-4 text-dark">
      <i class="fa fa-warehouse me-2"></i> Quản lý kho
    </h2>

    <div class="d-flex flex-wrap justify-content-center gap-4">
      <div class="menu-card card-gradient-blue" @click="goToProducts">
        <div class="icon-group">
          <i class="fa fa-cubes icon"></i>
        </div>
        <h3>Sản phẩm</h3>
        <p>Quản lý thông tin sản phẩm trong kho</p>
      </div>
      <div class="menu-card card-gradient-purple" @click="goToCategories">
        <div class="icon-group">
          <i class="fa fa-list-ul icon"></i>
        </div>
        <h3>Loại sản phẩm</h3>
        <p>Phân loại và tổ chức sản phẩm</p>
      </div>
      <div class="menu-card card-gradient-pink" @click="gotoInventory">
        <div class="icon-group">
          <i class="fa fa-truck-loading icon"></i>
        </div>
        <h3>Xuất/nhập hàng</h3>
        <p>Quản lý giao dịch xuất và nhập hàng</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router';

const router = useRouter();

const goToProducts = () => {
  router.push("/Products");
};

const goToCategories = () => {
  router.push("/categories");
};

const gotoInventory = () => {
  router.push("/Kho");
};
</script>

<style scoped>
.page-title {
  font-size: 2.5rem;
  font-weight: bold;
  color: #5a4ff3;
  display: flex;
  align-items: center;
  gap: 10px;
}

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
  border: none;
}

.icon-group {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 10px;
}

.menu-card .icon {
  font-size: 3.5rem; 
}

.menu-card h3 {
  margin-bottom: 5px;
  font-weight: bold;
}

.menu-card p {
  font-size: 0.95rem;
  opacity: 0.85;
}

.menu-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 12px 25px rgba(0,0,0,0.2);
}

.card-gradient-blue {
  background: linear-gradient(135deg, #4facfe, #00f2fe);
}

.card-gradient-purple {
  background: linear-gradient(135deg, #667eea, #764ba2);
}

.card-gradient-pink {
  background: linear-gradient(135deg, #ff6a88, #ff99ac);
}

@media (max-width: 768px) {
  .menu-card {
    width: 100%;
    max-width: 300px;
  }
}
</style>