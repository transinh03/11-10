<template>
  <section class="about-section py-5" style="background-color: #f2fbf8">
    <div class="container">
      <!-- Logo + Slogan -->
      <div class="text-center mb-5 fade-in">
        <img
          src="\src\assets\img\logo.png"
          alt="Logo TutaSpa"
          class="mb-3 logo-img"
        />

        <h2 class="fw-bold text-uppercase text-primary-green">
          KHÁM PHÁ DỊCH VỤ NỔI BẬT
        </h2>
        <p class="fst-italic text-subtle">
          "Nơi vẻ đẹp được đánh thức bằng sự tinh tế và yêu thương."
        </p>
      </div>

      <!-- Về Chúng Tôi -->
      <div class="row align-items-center mb-5 fade-in-up">
        <div class="col-md-6">
          <img
            src="\src\assets\img\spa1.jpg"
            alt="Về TutaSpa"
            class="img-fluid rounded-4 shadow-lg"
          />
        </div>
        <div class="col-md-6">
          <h3 class="fw-bold text-primary-green mb-3">
            <i class="fas fa-leaf me-2"></i>Về Chúng Tôi
          </h3>
          <p class="text-subtle lead">
            TutaSpa là điểm đến lý tưởng để thư giãn và làm đẹp, nơi bạn tìm
            thấy sự bình yên, sắc đẹp và sự phục hồi cả thể chất lẫn tinh thần.
            Với đội ngũ chuyên viên tận tâm và không gian sang trọng, chúng tôi
            cam kết mang đến trải nghiệm tuyệt vời cho từng khách hàng.
          </p>
        </div>
      </div>

      <!-- 3 Cột: Tầm nhìn - Đội ngũ - Cam kết -->
      <div class="row text-center mb-5 fade-in-up delay-1">
        <div class="col-md-4 mb-3">
          <div class="p-4 bg-white rounded-4 shadow-sm h-100 icon-box">
            <div class="icon-circle mb-3">
              <i class="fas fa-eye text-primary-green fs-3"></i>
            </div>
            <h5 class="text-primary-green fw-bold mb-2">Tầm Nhìn</h5>
            <p class="text-subtle">
              Trở thành spa được yêu thích và tin tưởng nhất tại Việt Nam.
            </p>
          </div>
        </div>
        <div class="col-md-4 mb-3">
          <div class="p-4 bg-white rounded-4 shadow-sm h-100 icon-box">
            <div class="icon-circle mb-3">
              <i class="fas fa-users text-primary-green fs-3"></i>
            </div>
            <h5 class="text-primary-green fw-bold mb-2">Đội Ngũ</h5>
            <p class="text-subtle">
              Chuyên viên tay nghề cao, đào tạo chuyên nghiệp và luôn phục vụ
              bằng trái tim.
            </p>
          </div>
        </div>
        <div class="col-md-4 mb-3">
          <div class="p-4 bg-white rounded-4 shadow-sm h-100 icon-box">
            <div class="icon-circle mb-3">
              <i class="fas fa-hand-holding-heart text-primary-green fs-3"></i>
            </div>
            <h5 class="text-primary-green fw-bold mb-2">Cam Kết</h5>
            <p class="text-subtle">
              TutaSpa luôn đặt sự hài lòng và trải nghiệm của khách hàng lên
              hàng đầu.
            </p>
          </div>
        </div>
      </div>

      <!-- Ý Nghĩa Logo -->
      <div class="row align-items-center fade-in-up delay-2">
        <div class="col-md-6 order-md-2">
          <img
            src="\src\assets\img\logo.png"
            alt="Ý nghĩa logo"
            class="img-fluid rounded-4 shadow-lg"
          />
        </div>
        <div class="col-md-6 order-md-1">
          <h4 class="fw-bold text-primary-green mb-3">
            <i class="fas fa-gem me-2"></i>Ý Nghĩa Logo
          </h4>
          <p class="text-subtle lead">
            Logo của TutaSpa là sự kết hợp hài hòa giữa thiên nhiên và con
            người, biểu trưng cho sự cân bằng, thư thái và vẻ đẹp thuần khiết.
            Mỗi đường nét được thiết kế tinh tế nhằm tôn vinh vẻ đẹp bên trong
            lẫn bên ngoài.
          </p>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
/* Màu xanh chủ đạo */
.text-primary-green {
  color: #048661;
}

/* Màu chữ phụ (xám xanh dịu) */
.text-subtle {
  color: #4d6f68;
}

/* Logo */
.logo-img {
  width: 120px;
  transition: transform 0.3s ease;
}
.logo-img:hover {
  transform: scale(1.05);
}

/* Icon tròn */
.icon-circle {
  width: 60px;
  height: 60px;
  background-color: #e0f5ef;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto;
}
.icon-box:hover .icon-circle {
  background-color: #007E5A;
}
.icon-box:hover i {
  color: white;
}

/* Animation */
.fade-in {
  animation: fadeIn 1s ease forwards;
}
.fade-in-up {
  opacity: 0;
  transform: translateY(20px);
  animation: fadeInUp 1s ease forwards;
}
.fade-in-up.delay-1 {
  animation-delay: 0.3s;
}
.fade-in-up.delay-2 {
  animation-delay: 0.6s;
}
@keyframes fadeIn {
  to {
    opacity: 1;
  }
}
@keyframes fadeInUp {
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
