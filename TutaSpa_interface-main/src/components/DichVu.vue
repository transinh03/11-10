<template>
  <section id="home" class="min-vh-100 position-relative">
    <div
      id="spaCarousel"
      class="carousel slide"
      data-bs-ride="carousel"
      data-bs-interval="5000"
    >
      <!-- Carousel Indicators -->
      <div class="carousel-indicators">
        <button
          v-for="(slide, index) in slides"
          :key="index"
          type="button"
          :data-bs-target="'#spaCarousel'"
          :data-bs-slide-to="index"
          :class="{ active: index === currentSlide }"
          :aria-current="index === currentSlide ? 'true' : 'false'"
          :aria-label="'Slide ' + (index + 1)"
        ></button>
      </div>

      <!-- Carousel Items -->
      <div class="carousel-inner">
        <div
          v-for="(slide, index) in slides"
          :key="slide.title"
          class="carousel-item"
          :class="{ active: index === currentSlide }"
          :style="{
            backgroundImage: `linear-gradient(rgba(0, 0, 0, 0.5), rgba(120, 186, 126, 0.3)), url(${slide.image})`,
          }"
        >
          <div
            class="carousel-caption d-flex flex-column justify-content-center h-100"
          >
            <div class="mb-4 animate__animated animate__fadeInDown">
              <div
                class="d-inline-block p-4 rounded-circle bg-success bg-opacity-75 shadow-lg"
              >
                <svg
                  class="w-50 h-50 text-white"
                  fill="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 17.93c-3.94-.49-7-3.85-7-7.93 0-.62.08-1.21.21-1.79L9 15v1c0 1.1.9 2 2 2v1.93zm6.9-2.54c-.26-.81-1-1.39-1.9-1.39h-1v-3c0-.55-.45-1-1-1H8v-2h2c.55 0 1-.45 1-1V7h2c1.1 0 2-.9 2-2v-.41c2.93 1.19 5 4.06 5 7.41 0 2.08-.8 3.97-2.1 5.39z"
                  />
                </svg>
              </div>
            </div>
            <h1
              class="display-2 fw-bold mb-4 font-lora animate__animated animate__fadeInDown"
            >
              Chào mừng đến TutaSpa
            </h1>
            <h2
              class="fs-3 fw-semibold text-white mb-4 animate__animated animate__fadeInUp animate__delay-1s"
            >
              {{ slides[currentSlide].subtitle }}
            </h2>
            <p
              class="lead text-white mb-5 mx-auto animate__animated animate__fadeInUp animate__delay-2s"
              style="max-width: 700px"
            >
              Hành trình thư giãn và tái tạo năng lượng với những liệu pháp tự
              nhiên, mang đến sự cân bằng hoàn hảo cho cơ thể và tâm hồn bạn
            </p>
            <div
              class="d-flex flex-wrap justify-content-center gap-4 mb-5 animate__animated animate__fadeInUp animate__delay-3s"
            >
              <div
                v-for="(stat, index) in stats"
                :key="index"
                class="text-center px-3"
              >
                <div class="fs-1 mb-2">{{ stat.icon }}</div>
                <div class="fs-3 fw-bold text-warning">{{ stat.number }}</div>
                <div class="text-white">{{ stat.label }}</div>
              </div>
            </div>
            <div
              class="d-flex flex-column flex-sm-row justify-content-center gap-3 animate__animated animate__fadeInUp animate__delay-4s"
            >
              <router-link
                to="/#services"
                class="btn btn-primary btn-lg rounded-pill px-5 py-3 fw-bold"
              >
                Khám phá dịch vụ
              </router-link>
              <router-link
                to="/#booking"
                class="btn btn-outline-light btn-lg rounded-pill px-5 py-3"
              >
                Đặt lịch ngay
              </router-link>
            </div>
          </div>
        </div>
      </div>

      <!-- Carousel Controls -->
      <button
        class="carousel-control-prev"
        type="button"
        data-bs-target="#spaCarousel"
        data-bs-slide="prev"
      >
        <span class="carousel-control-prev-icon" aria-hidden="true"></span>
        <span class="visually-hidden">Previous</span>
      </button>
      <button
        class="carousel-control-next"
        type="button"
        data-bs-target="#spaCarousel"
        data-bs-slide="next"
      >
        <span class="carousel-control-next-icon" aria-hidden="true"></span>
        <span class="visually-hidden">Next</span>
      </button>
    </div>
    <!-- Scroll Indicator -->
    <div
      class="position-absolute bottom-0 start-50 translate-middle-x z-30 animate__animated animate__bounce animate__infinite"
    >
      <div
        class="border border-2 border-white border-opacity-50 rounded-pill p-1"
        style="width: 1.5rem; height: 2.5rem"
      >
        <div
          class="bg-white bg-opacity-75 rounded-pill animate-pulse"
          style="width: 0.5rem; height: 0.75rem"
        ></div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from "vue";

const currentSlide = ref(0);

const slides = [
  {
    image:
      "https://images.unsplash.com/photo-1540555700478-4be289fbecef?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80",
    title: "Thư Giãn & Tái Tạo",
    subtitle: "Trải nghiệm không gian yên bình",
  },
  {
    image:
      "https://images.unsplash.com/photo-1596178065887-1198b6148b2b?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80",
    title: "Chăm Sóc Chuyên Nghiệp",
    subtitle: "Với đội ngũ chuyên gia giàu kinh nghiệm",
  },
  {
    image:
      "https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80",
    title: "Làm Đẹp Tự Nhiên",
    subtitle: "Sử dụng 100% sản phẩm thiên nhiên",
  },
];

const stats = [
  { number: "10+", label: "Năm kinh nghiệm", icon: "🏆" },
  { number: "5000+", label: "Khách hàng hài lòng", icon: "👥" },
  { number: "24", label: "Dịch vụ chuyên nghiệp", icon: "🌿" },
];

const nextSlide = () => {
  currentSlide.value = (currentSlide.value + 1) % slides.length;
};

onMounted(() => {
  const timer = setInterval(nextSlide, 5000);
  onUnmounted(() => clearInterval(timer));
});
</script>

<style scoped>
.carousel-item {
  height: 100vh;
  background-size: cover !important;
  background-position: center !important;
}

.carousel-caption {
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.5);
}

.font-lora {
  font-family: "Lora", serif;
}

.w-50 {
  width: 50px;
}

.h-50 {
  height: 50px;
}
</style>
