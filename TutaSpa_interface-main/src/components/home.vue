<template>
  <div>
    <section id="home" class="min-vh-100 position-relative">
      <div
        id="spaCarousel"
        class="carousel slide"
        data-bs-ride="carousel"
        data-bs-interval="3000"
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
              <h1
                class="hero-title fw-bold mb-3 mb-md-4 font-lora animate__animated animate__fadeInDown"
              >
                Chào mừng đến TutaSpa
              </h1>
              <h2
                class="hero-subtitle fw-semibold text-white mb-3 mb-md-4 animate__animated animate__fadeInUp animate__delay-1s"
              >
                {{ slides[currentSlide].subtitle }}
              </h2>
              <p
                class="hero-description lead text-white mb-4 mb-md-5 mx-auto animate__animated animate__fadeInUp animate__delay-2s"
              >
                Hành trình thư giãn và tái tạo năng lượng với những liệu pháp tự
                nhiên, mang đến sự cân bằng hoàn hảo cho cơ thể và tâm hồn bạn
              </p>

              <!-- Stats Section with improved responsiveness -->
              <div
                class="stats-container mb-4 mb-md-5 animate__animated animate__fadeInUp animate__delay-3s"
              >
                <div
                  v-for="(stat, index) in stats"
                  :key="index"
                  class="stat-item"
                >
                  <div class="stat-icon">{{ stat.icon }}</div>
                  <div class="stat-details">
                    <div class="stat-number">{{ stat.number }}</div>
                    <div class="stat-label">{{ stat.label }}</div>
                  </div>
                </div>
              </div>

              <!-- Hero Buttons -->
              <div
                class="hero-buttons animate__animated animate__fadeInUp animate__delay-4s"
              >
                <router-link
                  to="/#services"
                  class="btn btn-primary btn-hero rounded-pill fw-bold"
                >
                  Khám phá dịch vụ
                </router-link>
                <router-link
                  to="/#booking"
                  class="btn btn-outline-light btn-hero rounded-pill"
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
    </section>

    <!-- Features Section -->
    <section class="features py-5">
      <div class="container">
        <div class="row g-4">
          <div class="col-lg-3 col-md-6 col-sm-6">
            <div class="feature-card text-center">
              <div class="feature-icon mb-3">🌱</div>
              <h3 class="h5 mb-3">100% Tự nhiên</h3>
              <p class="mb-0">
                Sử dụng các sản phẩm từ thiên nhiên, không chất hóa học có hại,
                an toàn cho mọi loại da
              </p>
            </div>
          </div>
          <div class="col-lg-3 col-md-6 col-sm-6">
            <div class="feature-card text-center">
              <div class="feature-icon mb-3">👥</div>
              <h3 class="h5 mb-3">Chuyên gia giàu kinh nghiệm</h3>
              <p class="mb-0">
                Đội ngũ chuyên viên được đào tạo bài bản, và có kinh nghiệm hơn
                5 năm
              </p>
            </div>
          </div>
          <div class="col-lg-3 col-md-6 col-sm-6">
            <div class="feature-card text-center">
              <div class="feature-icon mb-3">🛏️</div>
              <h3 class="h5 mb-3">Không gian sang trọng</h3>
              <p class="mb-0">
                Thiết kế hiện đại, thoáng mát với âm nhạc thư giãn và hương thơm
                dễ chịu
              </p>
            </div>
          </div>
          <div class="col-lg-3 col-md-6 col-sm-6">
            <div class="feature-card text-center">
              <div class="feature-icon mb-3">🛡️</div>
              <h3 class="h5 mb-3">Đảm bảo vệ sinh</h3>
              <p class="mb-0">
                Tuân thủ nghiêm ngặt các tiêu chuẩn vệ sinh, khử trùng dụng cụ
                sau mỗi lần sử dụng
              </p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Services Section -->
    <section id="services" class="services py-5">
      <div class="container">
        <div class="text-center mb-5">
          <h2 class="section-title mb-3">Dịch vụ của chúng tôi</h2>
          <p class="section-subtitle mx-auto">
            Trải nghiệm những dịch vụ chăm sóc sức khỏe và làm đẹp hàng đầu với
            công nghệ hiện đại và nguyên liệu tự nhiên
          </p>
        </div>

        <!-- Service Categories - Responsive -->
        <div
          class="d-flex flex-wrap justify-content-center gap-2 gap-md-3 mb-4 mb-md-5"
        >
          <button
            class="category-btn"
            :class="{ active: currentCategory === 'all' }"
            @click="filterServices('all')"
          >
            Tất cả
          </button>
          <button
            v-for="category in categories"
            :key="category.loaiDichVuID"
            class="category-btn"
            :class="{ active: currentCategory === category.tenLoai }"
            @click="filterServices(category.tenLoai)"
          >
            {{ category.tenLoai }}
          </button>
        </div>

        <div v-if="loading" class="loading text-center py-5">
          <div class="loading-spinner mx-auto"></div>
          <p class="mt-3">Đang tải dịch vụ...</p>
        </div>
        <div v-else-if="error" class="error text-center py-5">
          <p>{{ error }}</p>
        </div>
        <div v-else class="row g-3 g-md-4">
          <div
            v-for="service in filteredServices"
            :key="service.id"
            class="col-6 col-md-4 col-lg-3"
          >
            <div class="service-card h-100">
              <div class="service-image-container position-relative">
                <router-link
                  class="detail-service-link d-block"
                  :to="`/DichVuChiTiet/${service.id}`"
                >
                  <img
                    :src="service.image"
                    :alt="service.name"
                    class="service-image w-100"
                  />
                </router-link>
                <div class="service-rating-overlay position-absolute">
                  <div class="service-rating d-flex align-items-center">
                    <i
                      v-for="n in 5"
                      :key="n"
                      class="rating-star me-1"
                      :class="
                        n <= Math.floor(service.mucDanhGia)
                          ? 'filled'
                          : n - 0.5 <= service.mucDanhGia
                          ? 'half-filled'
                          : ''
                      "
                      >★</i
                    >
                    <span class="rating-text ms-1"
                      >({{ service.mucDanhGia?.toFixed(1) || "0.0" }})</span
                    >
                  </div>
                </div>
              </div>
              <div class="service-content p-3 d-flex flex-column h-100">
                <router-link
                  class="detail-service-link text-decoration-none"
                  :to="`/DichVuChiTiet/${service.id}`"
                >
                  <h3 class="service-title h6 mb-2">{{ service.name }}</h3>
                  <div
                    class="service-meta d-flex justify-content-between align-items-center mb-2"
                  >
                    <div class="service-duration d-flex align-items-center">
                      <i class="duration-icon me-1">⏱</i>
                      <span>{{ service.duration }}p</span>
                    </div>
                    <div class="service-price fw-bold">
                      {{ service.price }} VNĐ
                    </div>
                  </div>
                </router-link>
                <button
                  class="service-book-btn btn btn-sm"
                  @click="addServiceFromCard(service)"
                >
                  <span>Đặt lịch</span>
                  <i class="btn-arrow ms-1">→</i>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Testimonials Section -->
    <section class="testimonials-section py-5">
      <div class="container">
        <h2 class="section-title text-center mb-5 text-white">
          Khách hàng nói gì về chúng tôi
        </h2>

        <div v-if="loading" class="text-center text-white">
          <div class="loading-spinner mx-auto"></div>
          <p class="mt-3">Đang tải đánh giá...</p>
        </div>

        <div
          v-else-if="testimonials.length === 0"
          class="text-center text-white"
        >
          <div class="no-reviews-icon">💬</div>
          <p class="mt-3">
            Chưa có đánh giá nào. Hãy là người đầu tiên chia sẻ trải nghiệm của
            bạn!
          </p>
        </div>

        <div v-else>
          <div class="row g-4">
            <div
              v-for="(item, index) in visibleTestimonials"
              :key="index"
              class="col-lg-4 col-md-6"
            >
              <div
                class="testimonial-card h-100"
                :class="{ 'testimonial-featured': index === 0 }"
              >
                <div class="testimonial-content">
                  <div
                    class="testimonial-header d-flex align-items-center mb-3"
                  >
                    <div class="testimonial-avatar me-3">
                      <span>{{ getInitials(item.name || "Khách hàng") }}</span>
                    </div>
                    <div class="testimonial-info">
                      <h4 class="testimonial-name h6 mb-1">
                        {{ item.name || "Khách hàng" }}
                      </h4>
                      <div class="testimonial-rating">
                        <i
                          v-for="n in 5"
                          :key="n"
                          class="fa-star fas me-1"
                          :class="
                            n <= item.rate
                              ? 'text-warning'
                              : 'text-secondary opacity-25'
                          "
                        ></i>
                      </div>
                    </div>
                  </div>
                  <p class="testimonial-text mb-0">
                    "{{
                      item.content || "Dịch vụ tuyệt vời, tôi rất hài lòng!"
                    }}"
                  </p>
                </div>
              </div>
            </div>
          </div>

          <div v-if="testimonials.length > 3" class="text-center mt-4">
            <button class="show-more-btn btn" @click="toggleShowAll">
              <span v-if="!showAllTestimonials">
                Xem thêm {{ testimonials.length - 3 }} đánh giá khác
                <i class="fas fa-chevron-down ms-2"></i>
              </span>
              <span v-else>
                Thu gọn
                <i class="fas fa-chevron-up ms-2"></i>
              </span>
            </button>
          </div>

          <div
            v-if="showAllTestimonials && testimonials.length > 3"
            class="additional-testimonials mt-4"
          >
            <div class="row g-4">
              <div
                v-for="(item, index) in additionalTestimonials"
                :key="index + 3"
                class="col-lg-4 col-md-6"
              >
                <div class="testimonial-card h-100 testimonial-fade-in">
                  <div class="testimonial-content">
                    <div
                      class="testimonial-header d-flex align-items-center mb-3"
                    >
                      <div class="testimonial-avatar me-3">
                        <span>{{
                          getInitials(item.user?.name || "Khách hàng")
                        }}</span>
                      </div>
                      <div class="testimonial-info">
                        <h4 class="testimonial-name h6 mb-1">
                          {{ item.name || "Khách hàng" }}
                        </h4>
                        <div class="testimonial-rating">
                          <i
                            v-for="n in 5"
                            :key="n"
                            class="fa-star fas me-1"
                            :class="
                              n <= item.rate
                                ? 'text-warning'
                                : 'text-secondary opacity-25'
                            "
                          ></i>
                        </div>
                      </div>
                    </div>
                    <p class="testimonial-text mb-0">
                      "{{
                        item.content || "Dịch vụ tuyệt vời, tôi rất hài lòng!"
                      }}"
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Booking Section -->
    <section id="booking" class="booking py-5">
      <div class="container">
        <div class="row align-items-center g-4 g-lg-5">
          <!-- Booking Info -->
          <div class="col-lg-6">
            <div class="booking-info">
              <h2 class="mb-4">Đặt lịch hẹn</h2>
              <p class="mb-4">
                Hãy để chúng tôi chăm sóc bạn với những dịch vụ tốt nhất. Đặt
                lịch ngay hôm nay để nhận được ưu đãi đặc biệt và trải nghiệm
                không gian thư giãn tuyệt vời.
              </p>
              <ul class="booking-benefits list-unstyled">
                <li class="mb-2 d-flex align-items-center">
                  <span class="benefit-check me-2">✓</span>
                  Tư vấn miễn phí từ chuyên gia
                </li>
                <li class="mb-2 d-flex align-items-center">
                  <span class="benefit-check me-2">✓</span>
                  Đặt lịch linh hoạt theo thời gian bạn
                </li>
                <li class="mb-2 d-flex align-items-center">
                  <span class="benefit-check me-2">✓</span>
                  Giảm giá 10% cho khách hàng lần đầu
                </li>
                <li class="mb-2 d-flex align-items-center">
                  <span class="benefit-check me-2">✓</span>
                  Cam kết dịch vụ chất lượng cao
                </li>
                <li class="mb-2 d-flex align-items-center">
                  <span class="benefit-check me-2">✓</span>
                  Hỗ trợ 24/7 qua hotline
                </li>
              </ul>
            </div>
          </div>

          <!-- Booking Form -->
          <div class="col-lg-6">
            <form class="booking-form" @submit.prevent="submitBooking">
              <div class="mb-3">
                <label for="phone" class="form-label">Số điện thoại *</label>
                <input
                  type="tel"
                  id="phone"
                  class="form-control"
                  v-model="bookingForm.phone"
                  placeholder="0123 456 789"
                  @input="filterPhone"
                />
              </div>

              <div class="mb-3">
                <label for="service" class="form-label">Chọn dịch vụ *</label>
                <select
                  id="service"
                  class="form-select"
                  v-model="selectedService"
                  :disabled="bookingForm.consultAtStore"
                >
                  <option value="">-- Chọn dịch vụ --</option>
                  <option
                    v-for="service in services"
                    :key="service.id"
                    :value="service"
                  >
                    {{ service.name }} - {{ service.price }} VNĐ
                  </option>
                </select>

                <div
                  class="d-flex flex-column flex-sm-row align-items-start align-items-sm-center gap-3 mt-2"
                >
                  <button
                    type="button"
                    class="btn add-service-btn"
                    :disabled="!selectedService || bookingForm.consultAtStore"
                    @click="addService"
                  >
                    Xác Nhận Dịch Vụ
                  </button>
                  <div class="form-check">
                    <input
                      class="form-check-input"
                      type="checkbox"
                      id="consultAtStore"
                      v-model="bookingForm.consultAtStore"
                      @change="handleConsultAtStore"
                    />
                    <label class="form-check-label" for="consultAtStore">
                      Tới quán nhân viên tư vấn
                    </label>
                  </div>
                </div>
              </div>

              <!-- Selected Services List -->
              <div
                v-if="
                  bookingForm.services.length && !bookingForm.consultAtStore
                "
                class="selected-services mb-3"
              >
                <h4 class="h6 mb-3">Dịch vụ đã chọn:</h4>
                <div class="selected-services-list">
                  <div
                    v-for="(service, index) in bookingForm.services"
                    :key="index"
                    class="selected-service-card d-flex justify-content-between align-items-center p-2 mb-2"
                  >
                    <div class="service-info">
                      <div class="service-name fw-semibold">
                        {{ service.name }}
                      </div>
                      <div class="service-price text-warning">
                        {{ service.price.toLocaleString() }} VNĐ
                      </div>
                    </div>
                    <button
                      type="button"
                      class="btn btn-sm remove-service-btn"
                      @click="removeService(index)"
                      title="Xóa dịch vụ"
                    >
                      &times;
                    </button>
                  </div>
                </div>
              </div>

              <div class="row g-3 mb-3">
                <div class="col-sm-6">
                  <label for="date" class="form-label">Ngày hẹn *</label>
                  <input
                    type="date"
                    id="date"
                    class="form-control"
                    v-model="bookingForm.date"
                    :min="minDate"
                    required
                  />
                </div>
                <div class="col-sm-6">
                  <label for="time" class="form-label">Giờ hẹn *</label>
                  <select
                    id="time"
                    class="form-select"
                    v-model="bookingForm.time"
                  >
                    <option value="">-- Chọn giờ --</option>
                    <option
                      v-for="slot in availableSlots"
                      :key="slot.khungGio"
                      :value="slot.khungGio"
                    >
                      {{ slot.khungGio }}
                    </option>
                  </select>
                </div>
              </div>

              <div class="mb-3">
                <label for="notes" class="form-label">Ghi chú</label>
                <textarea
                  id="notes"
                  class="form-control"
                  rows="3"
                  v-model="bookingForm.notes"
                  placeholder="Những yêu cầu đặc biệt hoặc ghi chú khác..."
                ></textarea>
              </div>

              <button type="submit" class="btn submit-btn w-100">
                Đặt lịch hẹn
              </button>
            </form>
          </div>
        </div>
      </div>
    </section>

    <!-- About Section -->
    <section id="about" class="about-section py-5">
      <div class="container">
        <div class="row align-items-center g-4 g-lg-5">
          <!-- Content -->
          <div class="col-lg-6">
            <div class="about-content-wrapper">
              <h2 class="about-title mb-3">
                <span class="title-accent">Về</span> TutaSpa
              </h2>
              <div class="about-subtitle mb-4">
                Điểm đến hoàn hảo cho sự thư giãn và làm đẹp
              </div>
              <p class="about-description mb-4">
                Với hơn <strong>10 năm kinh nghiệm</strong> trong ngành làm đẹp,
                TutaSpa tự hào mang đến cho khách hàng những trải nghiệm thư
                giãn tuyệt vời nhất. Chúng tôi kết hợp tinh hoa truyền thống với
                công nghệ hiện đại, sử dụng 100% sản phẩm từ thiên nhiên.
              </p>

              <div class="about-features">
                <div class="feature-item d-flex align-items-start mb-3">
                  <div class="feature-icon-wrapper me-3">
                    <span class="feature-emoji">🌿</span>
                  </div>
                  <div class="feature-content">
                    <h4 class="h6 mb-1">Sản phẩm hữu cơ cao cấp</h4>
                    <p class="mb-0 text-muted">
                      Nhập khẩu trực tiếp từ các thương hiệu uy tín
                    </p>
                  </div>
                </div>

                <div class="feature-item d-flex align-items-start mb-3">
                  <div class="feature-icon-wrapper me-3">
                    <span class="feature-emoji">💆‍♀️</span>
                  </div>
                  <div class="feature-content">
                    <h4 class="h6 mb-1">Đội ngũ chuyên gia quốc tế</h4>
                    <p class="mb-0 text-muted">
                      Được đào tạo bài bản tại Hàn Quốc và Nhật Bản
                    </p>
                  </div>
                </div>

                <div class="feature-item d-flex align-items-start mb-3">
                  <div class="feature-icon-wrapper me-3">
                    <span class="feature-emoji">🏆</span>
                  </div>
                  <div class="feature-content">
                    <h4 class="h6 mb-1">Chứng nhận ISO 9001:2015</h4>
                    <p class="mb-0 text-muted">
                      Đảm bảo chất lượng dịch vụ quốc tế
                    </p>
                  </div>
                </div>

                <div class="feature-item d-flex align-items-start">
                  <div class="feature-icon-wrapper me-3">
                    <span class="feature-emoji">💯</span>
                  </div>
                  <div class="feature-content">
                    <h4 class="h6 mb-1">Cam kết hài lòng 100%</h4>
                    <p class="mb-0 text-muted">
                      Hoàn tiền nếu không hài lòng với dịch vụ
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Visual Content -->
          <div class="col-lg-6">
            <div class="about-visual position-relative">
              <div class="about-image-main mb-4">
                <img
                  src="https://images.unsplash.com/photo-1630595271375-5073a6c0638b?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MzZ8fHNwYXxlbnwwfHwwfHx8MA%3D%3D"
                  alt="TutaSpa Interior"
                  class="img-fluid rounded-4 shadow-lg w-100"
                />
                <div
                  class="image-overlay position-absolute bottom-0 start-0 end-0 p-4"
                >
                  <div class="overlay-content text-white">
                    <h3 class="h5 mb-1">Không gian thư giãn</h3>
                    <p class="mb-0">Thiết kế sang trọng, yên tĩnh</p>
                  </div>
                </div>
              </div>

              <!-- Google Map -->
              <div class="map-wrapper rounded-4 overflow-hidden shadow">
                <a
                  href="https://www.google.com/maps/place/31+Nguyễn+Mộng+Tuân,+Hoà+Minh,+Liên+Chiểu,+Đà+Nẵng+550000"
                  target="_blank"
                  class="d-block"
                >
                  <iframe
                    src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3834.168381775547!2d108.1698262!3d16.0633039!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3142191ed695fc89%3A0xb837389a53933ca9!2zMzEgTmfDtG4gTeG7mW5nIFR1w6puLCBIb8OgIE1pbmgsIExpw6puIENow61hLCDEkMOgIE5hbmc!5e0!3m2!1svi!2s!4v1721903400000"
                    width="100%"
                    height="250"
                    style="border: 0"
                    allowfullscreen=""
                    loading="lazy"
                    referrerpolicy="no-referrer-when-downgrade"
                    title="31 Nguyễn Mộng Tuân, Đà Nẵng"
                  ></iframe>
                </a>
              </div>
              <a
                href="https://www.google.com/maps/place/31+Nguyễn+Mộng+Tuân,+Hoà+Minh,+Liên+Chiểu,+Đà+Nẵng+550000"
                target="_blank"
                class="text-decoration-none text-dark mt-2 d-block"
              >
                <i class="fa-solid fa-map-location-dot me-2 text-primary"></i
                >Xem bản đồ
              </a>

              <div class="floating-badge position-absolute">
                <div class="badge-content d-flex align-items-center">
                  <span class="badge-icon me-2">⭐</span>
                  <div class="badge-text">
                    <div class="badge-title">Top Rated</div>
                    <div class="badge-subtitle">Spa in Da Nang</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, onMounted, watch, computed } from "vue";
import apiClient from "../utils/axiosClient";
import Swal from "sweetalert2";
import dayjs from "dayjs";
// Reactive state
const isLoading = ref(false);
const services = ref([]);
const categories = ref([]);
const currentCategory = ref("all");
const filteredServices = ref([]);
const loading = ref(true);
const error = ref(null);
const minDate = ref("");
const selectedService = ref(null);
const bookingForm = ref({
  phone: "",
  services: [],
  date: dayjs().format("YYYY-MM-DD"),
  time: "",
  notes: "",
  consultAtStore: false,
});
const modalForm = ref({
  name: "",
  phone: "",
  email: "",
  date: "",
  time: "",
  notes: "",
});

const showAllTestimonials = ref(false);

const availableSlots = ref([]);
// Base URL for images
const IMAGE_BASE_URL =
  import.meta.env.VITE_BASE_URL.replace("/api", "") + "/images/";

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
      "https://plus.unsplash.com/premium_photo-1723867490491-10519f8ed969?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MjF8fHNwYXxlbnwwfHwwfHx8MA%3D%3D",
    title: " Chăm Sóc Chuyên Nghiệp",
    subtitle: "Với đội ngũ chuyên gia giàu kinh nghiệm",
  },
  {
    image:
      "https://media.istockphoto.com/id/1960192709/vi/anh/woman-indulges-in-blissful-foot-massage-at-luxurious-spa-salon-quiescent.jpg?s=612x612&w=0&k=20&c=Ll5MOS6iv3hjUY3jCTkHazaAv9tg7O9WvpJmLUDwTW0=",
    title: "Làm Đẹp Tự Nhiên",
    subtitle: "Sử dụng 100% sản phẩm thiên nhiên",
  },
];

const stats = [
  { number: "10+", label: "Năm kinh nghiệm", icon: "🏆" },
  { number: "5000+", label: "Khách hàng hài lòng", icon: "👥" },
  { number: "24", label: "Dịch vụ chuyên nghiệp", icon: "🌿" },
];

const testimonials = ref([]);

// Computed testimonials
const visibleTestimonials = computed(() => testimonials.value.slice(0, 3));
const additionalTestimonials = computed(() => testimonials.value.slice(3));

const getInitials = (name) =>
  name
    .split(" ")
    .map((word) => word.charAt(0))
    .join("")
    .toUpperCase()
    .substring(0, 2);

const toggleShowAll = () => {
  showAllTestimonials.value = !showAllTestimonials.value;
};

// Methods
const handleConsultAtStore = () => {
  if (bookingForm.value.consultAtStore) {
    bookingForm.value.services = [];
    selectedService.value = null;
  }
};

const addServiceFromCard = (service) => {
  if (
    !bookingForm.value.services.some((s) => s.id === service.id) &&
    !bookingForm.value.consultAtStore
  ) {
    bookingForm.value.services.push({ ...service, soLuong: 1 });
  }
  const bookingSection = document.getElementById("booking");
  if (bookingSection) {
    bookingSection.scrollIntoView({ behavior: "smooth" });
  }
};

const fetchSlots = async () => {
  if (!bookingForm.value.date) return;
  try {
    const res = await apiClient.get("/DatLich/slots", {
      params: { ngay: bookingForm.value.date },
    });
    availableSlots.value = res.filter((slot) => slot.conLai > 0);
  } catch (err) {
    console.error("Lỗi khi lấy giờ hẹn:", err);
    availableSlots.value = [];
  }
};

// Watch đổi ngày để lấy slot
watch(
  () => bookingForm.value.date,
  () => {
    fetchSlots();
    bookingForm.value.time = "";
  }
);

const filterPhone = (event) => {
  let value = event.target.value.replace(/\D/g, ""); // chỉ giữ số
  if (value.length > 10) value = value.slice(0, 10); // tối đa 10 số
  if (value && !value.startsWith("0")) value = "0" + value; // bắt buộc số 0 đầu
  bookingForm.value.phone = value;
};

const fetchCategories = async () => {
  try {
    const response = await apiClient.get("/LoaiDichVu");
    const data = response;
    categories.value = Array.isArray(data) ? data : data.data || [];
    if (categories.value.length === 0)
      throw new Error("Không có danh mục dịch vụ nào");
  } catch (err) {
    console.error("Lỗi khi tải danh mục:", err);
    error.value = "Không thể tải danh mục dịch vụ. Hiển thị mặc định.";
    categories.value = [
      { loaiDichVuID: 1, tenLoai: "Triệt lông" },
      { loaiDichVuID: 2, tenLoai: "Massage" },
      { loaiDichVuID: 3, tenLoai: "Chăm sóc da" },
    ];
  }
};

const fetchServices = async () => {
  try {
    const response = await apiClient.get("/DichVu");
    const data = response;
    const serviceData = Array.isArray(data) ? data : data.data || [];
    services.value = serviceData
      .filter((service) => service.trangThai === 1)
      .map((service) => ({
        id: service.dichVuID,
        name: service.tenDichVu,
        category:
          categories.value.find(
            (cat) => cat.loaiDichVuID === service.loaiDichVuID
          )?.tenLoai || "Khác",
        price: service.gia.toLocaleString("vi-VN"),
        duration: service.thoiGian,
        description: service.moTa,
        image: `${IMAGE_BASE_URL}${service.hinhAnh}`,
        mucDanhGia: service.mucDanhGia || 0,
      }));
    if (services.value.length === 0) throw new Error("Không có dịch vụ nào");
  } catch (err) {
    console.error("Lỗi khi tải dịch vụ:", err);
    error.value = "Không thể tải dịch vụ. Hiển thị mặc định.";
  }
};

const filterServices = (category) => {
  currentCategory.value = category;
  filteredServices.value =
    category === "all"
      ? [...services.value]
      : services.value.filter((service) => service.category === category);
};

const addService = () => {
  if (
    selectedService.value &&
    !bookingForm.value.services.some(
      (s) => s.id === selectedService.value.id
    ) &&
    !bookingForm.value.consultAtStore
  ) {
    bookingForm.value.services.push({ ...selectedService.value, soLuong: 1 });
    selectedService.value = null; // Reset dropdown
  }
};

const removeService = (index) => {
  bookingForm.value.services.splice(index, 1);
};

const submitBooking = async () => {
  try {
    if (!bookingForm.value.phone) {
      await Swal.fire({
        icon: "warning",
        title: "Thiếu thông tin",
        text: "Vui lòng nhập số điện thoại!",
        confirmButtonText: "OK",
      });
      return;
    }
    if (!/^0\d{9}$/.test(bookingForm.value.phone)) {
      await Swal.fire({
        icon: "error",
        title: "Số điện thoại không hợp lệ",
        text: "Vui lòng nhập đúng 10 số và bắt đầu bằng 0!",
        confirmButtonText: "OK",
      });
      return;
    }
    if (
      !bookingForm.value.services.length &&
      !bookingForm.value.consultAtStore
    ) {
      await Swal.fire({
        icon: "warning",
        title: "Thiếu thông tin",
        text: "Vui lòng chọn ít nhất một dịch vụ hoặc chọn tư vấn tại quán!",
        confirmButtonText: "OK",
      });
      return;
    }
    if (!bookingForm.value.time) {
      await Swal.fire({
        icon: "warning",
        title: "Thiếu thông tin",
        text: "Vui lòng chọn thời gian!",
        confirmButtonText: "OK",
      });
      return;
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
    const thoiGian = new Date(
      new Date(
        `${bookingForm.value.date}T${bookingForm.value.time}`
      ).getTime() +
        7 * 60 * 60 * 1000
    ).toISOString();

    const dichVus = bookingForm.value.consultAtStore
      ? []
      : bookingForm.value.services.map((s) => ({
          dichVuID: s.id,
          soLuong: 1,
        }));

    const payload = {
      soDienThoai: bookingForm.value.phone,
      thoiGian,
      dichVus,
      ghiChu: bookingForm.value.notes,
      datTruoc: true,
    };

    const res = await apiClient.post("/DatLich", payload);

    await Swal.fire({
      icon: "success",
      title: "Thành công",
      text: "Đặt lịch thành công!",
      confirmButtonText: "OK",
    });
    resetBookingForm();
  } catch (err) {
    console.error("Lỗi đặt lịch:", err);
    alert("Đặt lịch thất bại!");
  } finally {
    isLoading.value = false;
  }
};

const resetBookingForm = () => {
  bookingForm.value = {
    phone: "",
    services: [],
    date: dayjs().format("YYYY-MM-DD"),
    time: "",
    notes: "",
    consultAtStore: false,
  };
  selectedService.value = null;
};

// ✅ Gom tất cả vào một onMounted duy nhất
onMounted(async () => {
  try {
    minDate.value = dayjs().format("YYYY-MM-DD");
    loading.value = true;

    // Load dữ liệu
    await fetchCategories();
    await fetchServices();
    filterServices("all");
    await fetchSlots();

    // Load testimonials
    const res = await apiClient.get("/DanhGia/admin/highlights");
    testimonials.value = res;
  } catch (err) {
    console.error("Lỗi khi khởi tạo:", err);
  } finally {
    loading.value = false;
  }
});
</script>

<style scoped>
/* Base styles */
.font-lora {
  font-family: "Lora", serif;
}

/* Hero Section - Carousel */
.carousel-item {
  height: 100vh;
  min-height: 600px;
  background-size: cover !important;
  background-position: center !important;
  position: relative;
}

.carousel-caption {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex !important;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  text-align: center;
  padding: 2rem;
}

.hero-title {
  font-size: clamp(2rem, 5vw, 3.5rem);
  font-weight: 700;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.5);
}

.hero-subtitle {
  font-size: clamp(1rem, 3vw, 1.5rem);
  text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.5);
}

.hero-description {
  font-size: clamp(0.9rem, 2.5vw, 1.1rem);
  max-width: 700px;
  line-height: 1.6;
  text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.5);
}

/* Stats Container - Responsive */
.stats-container {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 1.5rem;
  padding: 0 1rem;
}

.stat-item {
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  padding: 1.5rem;
  min-width: 150px;
  border: 1px solid rgba(255, 255, 255, 0.2);
  text-align: center;
  transition: transform 0.3s ease;
}

.stat-item:hover {
  transform: translateY(-5px);
}

.stat-icon {
  font-size: 2.5rem;
  margin-bottom: 0.5rem;
}

.stat-number {
  font-size: 2rem;
  font-weight: bold;
  color: #fbbf24;
  display: block;
}

.stat-label {
  font-size: 0.9rem;
  opacity: 0.9;
  color: white;
}

/* Hero Buttons */
.hero-buttons {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 1rem;
  padding: 0 1rem;
}

.btn-hero {
  padding: 0.9rem 2.5rem;
  font-size: 1rem;
  font-weight: 600;
  transition: all 0.3s ease;
}

.btn-hero:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.2);
}

/* Features Section */
.features {
  background: linear-gradient(135deg, #f8fdf8 0%, #f0fdf4 100%);
}

.feature-card {
  background: white;
  border-radius: 20px;
  padding: 2rem;
  height: 100%;
  transition: all 0.3s ease;
  border: 1px solid rgba(120, 186, 126, 0.1);
  box-shadow: 0 5px 15px rgba(120, 186, 126, 0.1);
}

.feature-card:hover {
  transform: translateY(-10px);
  box-shadow: 0 15px 35px rgba(120, 186, 126, 0.2);
}

.feature-icon {
  font-size: 3rem;
  color: #78ba7e;
}

/* Services Section */
.services {
  background: linear-gradient(135deg, #f8fdf8 0%, #f0fdf4 50%, #e8f5e8 100%);
}

.section-title {
  font-size: clamp(2rem, 4vw, 2.8rem);
  color: #335333;
  font-family: "Lora", serif;
  position: relative;
}

.section-title::after {
  content: "";
  display: block;
  width: 80px;
  height: 3px;
  background: linear-gradient(45deg, #78ba7e, #5e8c64);
  margin: 1rem auto 0;
  border-radius: 2px;
}

.section-subtitle {
  font-size: clamp(0.9rem, 2.5vw, 1.1rem);
  color: #4b5563;
  max-width: 600px;
}

.category-btn {
  background: white;
  border: 2px solid #78ba7e;
  color: #78ba7e;
  padding: 0.6rem 1.5rem;
  border-radius: 25px;
  font-weight: 500;
  transition: all 0.3s ease;
  font-size: 0.9rem;
}

.category-btn.active,
.category-btn:hover {
  background: #78ba7e;
  color: white;
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(120, 186, 126, 0.4);
}

/* Service Cards */
.service-card {
  background: white;
  border-radius: 15px;
  overflow: hidden;
  box-shadow: 0 5px 15px rgba(120, 186, 126, 0.1);
  transition: all 0.3s ease;
  border: 1px solid rgba(120, 186, 126, 0.1);
}

.service-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 15px 35px rgba(120, 186, 126, 0.2);
}

.service-image-container {
  position: relative;
  overflow: hidden;
}

.service-image {
  height: 180px;
  object-fit: cover;
  transition: transform 0.4s ease;
}

.service-card:hover .service-image {
  transform: scale(1.05);
}

.service-rating-overlay {
  top: 0.75rem;
  right: 0.75rem;
  background: rgba(255, 255, 255, 0.95);
  padding: 0.4rem 0.6rem;
  border-radius: 15px;
  backdrop-filter: blur(10px);
}

.service-rating {
  font-size: 0.8rem;
}

.rating-star {
  color: #d1d5db;
  font-size: 0.9rem;
  transition: color 0.2s;
}

.rating-star.filled {
  color: #fbbf24;
}

.rating-text {
  font-size: 0.75rem;
  color: #6b7280;
  font-weight: 600;
}

.service-title {
  font-size: 1.1rem;
  color: #2d4a2d;
  font-family: "Lora", serif;
}

.service-meta {
  font-size: 0.85rem;
}

.service-duration {
  background: #f0fdf4;
  color: #4a6741;
  padding: 0.3rem 0.8rem;
  border-radius: 12px;
  font-weight: 500;
}

.service-price {
  color: #f59e0b;
  font-weight: 700;
  font-size: 1rem;
}

.service-book-btn {
  background: linear-gradient(45deg, #78ba7e, #5e8c64);
  color: white;
  border: none;
  border-radius: 20px;
  padding: 0.6rem 1.5rem;
  font-weight: 600;
  transition: all 0.3s ease;
  font-size: 0.85rem;
}

.service-book-btn:hover {
  transform: scale(1.05);
  box-shadow: 0 5px 15px rgba(120, 186, 126, 0.5);
  color: white;
}

.btn-arrow {
  transition: transform 0.3s ease;
}

.service-book-btn:hover .btn-arrow {
  transform: translateX(3px);
}

/* Testimonials Section */
.testimonials-section {
  background: linear-gradient(135deg, #cef8d2 20%, #6ba371 50%, #95dda0 100%);
  position: relative;
  overflow: hidden;
}

.testimonials-section::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><defs><pattern id="grain" width="100" height="100" patternUnits="userSpaceOnUse"><circle cx="25" cy="25" r="1" fill="white" opacity="0.05"/></pattern></defs><rect width="100" height="100" fill="url(%23grain)"/></svg>');
  pointer-events: none;
}

.testimonials-section .section-title {
  color: white;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3);
}

.testimonials-section .section-title::after {
  background: linear-gradient(45deg, #fbbf24, #f59e0b);
}

.testimonial-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  padding: 2rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition: all 0.4s ease;
  position: relative;
  overflow: hidden;
}

.testimonial-card::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, #78ba7e, #fbbf24, #78ba7e);
  transform: scaleX(0);
  transition: transform 0.3s ease;
}

.testimonial-card:hover::before {
  transform: scaleX(1);
}

.testimonial-card:hover {
  transform: translateY(-10px);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.15);
  background: white;
}

.testimonial-avatar {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  background: linear-gradient(135deg, #78ba7e, #5e8c64);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: bold;
  font-size: 1rem;
}

.testimonial-name {
  color: #2d4a2d;
}

.testimonial-text {
  color: #4b5563;
  font-style: italic;
  line-height: 1.6;
  position: relative;
  padding-left: 1rem;
}

.testimonial-text::before {
  content: '"';
  position: absolute;
  left: -0.2rem;
  top: -0.5rem;
  font-size: 2.5rem;
  color: #78ba7e;
  opacity: 0.3;
  line-height: 1;
}

.show-more-btn {
  background: rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(10px);
  color: white;
  border: 2px solid rgba(255, 255, 255, 0.3);
  padding: 1rem 2rem;
  border-radius: 50px;
  font-weight: 600;
  transition: all 0.3s ease;
}

.show-more-btn:hover {
  background: rgba(255, 255, 255, 0.3);
  border-color: rgba(255, 255, 255, 0.5);
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  color: white;
}

.no-reviews-icon {
  font-size: 3rem;
  opacity: 0.5;
}

/* Booking Section */
.booking {
  background: linear-gradient(135deg, #a6ecab 0%, #6ba371 50%, #5e8c64 100%);
  color: white;
}

.booking-info h2 {
  font-size: clamp(2rem, 4vw, 2.5rem);
  font-family: "Lora", serif;
}

.benefit-check {
  color: #fbbf24;
  font-weight: bold;
  font-size: 1.1rem;
}

.booking-form {
  background: rgba(255, 255, 255, 0.15);
  padding: 2rem;
  border-radius: 20px;
  backdrop-filter: blur(10px);
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.1);
}

.form-label {
  font-weight: 600;
  color: white;
  margin-bottom: 0.5rem;
}

.form-control,
.form-select {
  background: rgba(255, 255, 255, 0.9);
  border: none;
  border-radius: 10px;
  padding: 0.8rem;
  transition: all 0.3s ease;
  color: #2d4a2d;
}

.form-control:focus,
.form-select:focus {
  background: white;
  box-shadow: 0 0 0 0.2rem rgba(120, 186, 126, 0.25);
  border-color: #78ba7e;
}

.add-service-btn {
  background: linear-gradient(45deg, #308238, #5e8c64);
  border: none;
  color: white;
  padding: 0.7rem 1.5rem;
  border-radius: 20px;
  font-weight: 600;
  transition: all 0.3s ease;
}

.add-service-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(48, 130, 56, 0.4);
}

.selected-services h4 {
  color: #1f5932;
  font-weight: 700;
}

.selected-service-card {
  background: rgba(255, 255, 255, 0.9);
  border-radius: 12px;
  border: 1px solid rgba(120, 186, 126, 0.2);
  transition: transform 0.2s ease;
}

.selected-service-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
}

.service-name {
  color: #333;
  font-size: 0.95rem;
}

.service-price {
  color: #f59e0b;
  font-size: 0.9rem;
  font-weight: 600;
}

.remove-service-btn {
  background: #dc3545;
  border: none;
  color: white;
  width: 30px;
  height: 30px;
  border-radius: 50%;
  font-size: 1.1rem;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.3s ease;
}

.remove-service-btn:hover {
  background: #c82333;
}

.submit-btn {
  background: linear-gradient(45deg, #f59e0b, #f97316);
  border: none;
  color: white;
  padding: 1rem;
  border-radius: 50px;
  font-size: 1.1rem;
  font-weight: bold;
  transition: all 0.3s ease;
}

.submit-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(245, 158, 11, 0.4);
}

/* About Section */
.about-section {
  background: linear-gradient(135deg, #f8fdf8 0%, #f0fdf4 50%, #e8f5e8 100%);
  position: relative;
}

.about-title {
  font-size: clamp(2rem, 4vw, 3rem);
  color: #2d4a2d;
  font-family: "Lora", serif;
}

.title-accent {
  background: linear-gradient(45deg, #78ba7e, #5e8c64);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.about-subtitle {
  font-size: clamp(1rem, 2.5vw, 1.3rem);
  color: #6b7280;
  font-style: italic;
}

.about-description {
  font-size: 1.1rem;
  line-height: 1.8;
  color: #374151;
}

.feature-item {
  padding: 1.2rem;
  background: rgba(255, 255, 255, 0.7);
  border-radius: 15px;
  border: 1px solid rgba(120, 186, 126, 0.1);
  transition: all 0.3s ease;
}

.feature-item:hover {
  transform: translateX(10px);
  box-shadow: 0 8px 25px rgba(120, 186, 126, 0.15);
  background: rgba(255, 255, 255, 0.9);
}

.feature-icon-wrapper {
  width: 50px;
  height: 50px;
  border-radius: 12px;
  background: linear-gradient(135deg, #78ba7e, #5e8c64);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.feature-emoji {
  font-size: 1.5rem;
}

.image-overlay {
  background: linear-gradient(transparent, rgba(0, 0, 0, 0.8));
  color: white;
  border-radius: 0 0 1rem 1rem;
}

.floating-badge {
  top: 1rem;
  right: 1rem;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  padding: 0.8rem;
  border-radius: 12px;
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.badge-icon {
  font-size: 1.2rem;
}

.badge-title {
  font-weight: 600;
  color: #2d4a2d;
  font-size: 0.85rem;
}

.badge-subtitle {
  color: #6b7280;
  font-size: 0.75rem;
}

/* Loading States */
.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid rgba(120, 186, 126, 0.3);
  border-top: 4px solid #78ba7e;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

/* Responsive Design */

/* Tablet - Medium devices */
@media (max-width: 992px) {
  .carousel-item {
    height: 85vh;
    min-height: 500px;
  }

  .stats-container {
    gap: 1rem;
  }

  .stat-item {
    min-width: 120px;
    padding: 1rem;
  }

  .stat-number {
    font-size: 1.5rem;
  }

  .service-image {
    height: 150px;
  }
}

/* Mobile - Small devices */
@media (max-width: 768px) {
  .carousel-item {
    height: 80vh;
    min-height: 450px;
  }

  .carousel-caption {
    padding: 1rem;
  }

  .stats-container {
    flex-direction: column;
    align-items: center;
    gap: 0.8rem;
  }

  .stat-item {
    display: flex;
    align-items: center;
    text-align: left;
    min-width: 250px;
    padding: 0.8rem 1.2rem;
  }

  .stat-details {
    margin-left: 1rem;
  }

  .stat-icon {
    font-size: 2rem;
    margin-bottom: 0;
  }

  .stat-number {
    font-size: 1.3rem;
  }

  .stat-label {
    font-size: 0.8rem;
  }

  .hero-buttons {
    flex-direction: column;
    align-items: stretch;
    max-width: 300px;
    margin: 0 auto;
  }

  .btn-hero {
    padding: 0.8rem 2rem;
    font-size: 0.95rem;
  }

  .feature-card {
    padding: 1.5rem;
  }

  .service-image {
    height: 120px;
  }

  .service-content {
    padding: 1rem;
  }

  .service-title {
    font-size: 1rem;
    line-height: 1.3;
  }

  .category-btn {
    padding: 0.5rem 1rem;
    font-size: 0.8rem;
  }

  .booking-form {
    padding: 1.5rem;
  }

  .testimonial-card {
    padding: 1.5rem;
  }

  .feature-item {
    padding: 1rem;
  }
}

/* Extra Small devices */
@media (max-width: 576px) {
  .carousel-item {
    height: 75vh;
    min-height: 400px;
  }

  .stat-item {
    min-width: 200px;
    padding: 0.6rem 1rem;
  }

  .stat-icon {
    font-size: 1.5rem;
  }

  .stat-number {
    font-size: 1.1rem;
  }

  .stat-label {
    font-size: 0.75rem;
  }

  .service-book-btn {
    padding: 0.5rem 1rem;
    font-size: 0.8rem;
  }

  .booking-form {
    padding: 1rem;
  }

  .feature-card {
    padding: 1rem;
  }

  .testimonial-card {
    padding: 1rem;
  }
}

.detail-service-link {
  text-decoration: none;
  color: inherit;
}

.detail-service-link:hover {
  color: inherit;
  text-decoration: none;
}
</style>
