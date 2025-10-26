<template>
  <div class="service-detail-page">
    <!-- Loading State -->
    <div v-if="loading" class="loading-container">
      <div class="loading-spinner"></div>
      <p>Đang tải thông tin dịch vụ...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="error-container">
      <div class="error-icon">⚠️</div>
      <h2>Không thể tải thông tin dịch vụ</h2>
      <p>{{ error }}</p>
      <button @click="$router.go(-1)" class="back-btn">
        Quay lại
      </button>
    </div>

    <!-- Main Content -->
    <div v-else class="service-detail-content">
      <!-- Breadcrumb -->
      <nav class="breadcrumb-container">
        <div class="container">
          <ol class="breadcrumb">
            <li><router-link to="/">Trang chủ</router-link></li>
            <li><router-link to="/#services">Dịch vụ</router-link></li>
            <li class="active">{{ serviceDetail.tenDichVu }}</li>
          </ol>
        </div>
      </nav>

      <!-- Service Hero Section -->
      <section class="service-hero">
        <div class="container">
          <div class="service-hero-content">
            <div class="service-image-container">
              <img
                :src="`${IMAGE_BASE_URL}${serviceDetail.hinhAnh}`"
                :alt="serviceDetail.tenDichVu"
                class="service-hero-image"
              />
              <div class="service-badge">
                {{ serviceDetail.loaiDichVu?.tenLoai || 'Dịch vụ spa' }}
              </div>
            </div>

            <div class="service-info">
              <h1 class="service-title">{{ serviceDetail.tenDichVu }}</h1>
              
              <div class="service-meta">
                <div class="meta-tags">
                  <div class="meta-tag price-tag">
                    <div class="meta-tag-icon">💰</div>
                    <div class="meta-tag-content">
                      <span class="meta-tag-label">Giá dịch vụ</span>
                      <span class="meta-tag-value">{{ formatPrice(serviceDetail.gia) }} VNĐ</span>
                    </div>
                  </div>

                  <div class="meta-tag time-tag">
                    <div class="meta-tag-icon">⏱</div>
                    <div class="meta-tag-content">
                      <span class="meta-tag-label">Thời gian</span>
                      <span class="meta-tag-value">{{ serviceDetail.thoiGian }} phút</span>
                    </div>
                  </div>

                  <div class="meta-tag category-tag">
                    <div class="meta-tag-icon">🎯</div>
                    <div class="meta-tag-content">
                      <span class="meta-tag-label">Danh mục</span>
                      <span class="meta-tag-value">{{ serviceDetail.loaiDichVu?.tenLoai }}</span>
                    </div>
                  </div>
                </div>
              </div>

              <div class="service-description">
                <h3>Mô tả dịch vụ</h3>
                <p>{{ serviceDetail.mota || 'Dịch vụ chất lượng cao tại TutaSpa với đội ngũ chuyên gia giàu kinh nghiệm.' }}</p>
              </div>

              <div class="service-actions">
                <button @click="router.push(`/DatLich/${serviceDetail.id}`)" class="book-now-btn">
                  <span>Đặt lịch ngay</span>
                  <i class="btn-icon">📅</i>
                </button>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Service Reviews -->
      <section class="service-reviews" v-if="reviewsData.reviews.length > 0">
        <div class="container">
          <h2 class="section-title">Đánh giá dịch vụ</h2>
          <div class="overall-rating">
            <span class="rating-value">{{ reviewsData.rating.toFixed(1) }}</span>
            <span class="stars">{{ generateStars(Math.round(reviewsData.rating)) }}</span>
            <span class="review-count">({{ reviewsData.reviews.length }} đánh giá)</span>
          </div>
          <div class="reviews-list">
            <div v-for="review in reviewsData.reviews" :key="review.createdDate" class="review-item">
              <div class="review-header">
                <span class="reviewer-name">{{ review.name }}</span>
                <span class="stars">{{ generateStars(review.rate) }}</span>
                <span class="review-date">{{ formatDate(review.createdDate) }}</span>
              </div>
              <p class="review-content">{{ review.content }}</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Related Services -->
      <section class="related-services" v-if="relatedServices.length > 0">
        <div class="container">
          <h2 class="section-title">Dịch vụ liên quan</h2>
          <div class="related-services-grid">
            <div
              v-for="service in relatedServices"
              :key="service.dichVuID"
              class="related-service-card"
              @click="goToService(service.dichVuID)"
            >
              <div class="related-service-image">
                <img
                  :src="`${IMAGE_BASE_URL}${service.hinhAnh}`"
                  :alt="service.tenDichVu"
                />
              </div>
              <div class="related-service-content">
                <h4>{{ service.tenDichVu }}</h4>
                <div class="related-service-meta">
                  <span class="related-price">{{ formatPrice(service.gia) }} VNĐ</span>
                  <span class="related-duration">{{ service.thoiGian }} phút</span>
                </div>
                <p class="related-description">{{ service.moTa }}</p>
                <button class="related-view-btn">
                  Xem chi tiết →
                </button>
              </div>
            </div>
          </div>
        </div>
      </section>

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed,watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import apiClient from '../utils/axiosClient'

// Router setup
const route = useRoute()
const router = useRouter()

// Image base URL
const IMAGE_BASE_URL = import.meta.env.VITE_BASE_URL.replace("/api", "") + "/images/"

// Reactive data
const loading = ref(true)
const error = ref(null)
const serviceDetail = ref({})
const relatedServices = ref([])
const reviewsData = ref({ rating: 0, reviews: [] })

// Quick booking form
const quickBookingForm = ref({
  name: '',
  phone: '',
  date: '',
  time: '',
  notes: ''
})

// Computed properties
const minDate = computed(() => {
  return new Date().toISOString().split('T')[0]
})

const serviceId = computed(() => {
  return route.params.id
})

// Methods
const formatPrice = (price) => {
  return new Intl.NumberFormat('vi-VN').format(price)
}

const fetchServiceDetail = async () => {
  try {
    const response = await apiClient.get(`/DichVu/detail/${serviceId.value}`)
    serviceDetail.value = response
  } catch (err) {
    console.error('Lỗi khi tải chi tiết dịch vụ:', err)
    error.value = 'Không thể tải thông tin dịch vụ. Vui lòng thử lại sau.'
  }
}

const fetchRelatedServices = async () => {
  try {
    const response = await apiClient.get(`/DichVu/${serviceId.value}/related`)
    relatedServices.value = response.filter(service => 
      service.dichVuID !== parseInt(serviceId.value) && service.trangThai === 1
    ).slice(0, 4) // Chỉ lấy 4 dịch vụ liên quan
  } catch (err) {
    console.error('Lỗi khi tải dịch vụ liên quan:', err)
    relatedServices.value = []
  }
}

const fetchReviews = async () => {
  try {
    const response = await apiClient.get(`/DichVu/${serviceId.value}/reviews`)
    reviewsData.value = response
  } catch (err) {
    console.error('Lỗi khi tải đánh giá:', err)
    reviewsData.value = { rating: 0, reviews: [] }
  }
}

const generateStars = (rating) => {
  let stars = '';
  for (let i = 1; i <= 5; i++) {
    stars += i <= rating ? '★' : '☆';
  }
  return stars;
}

const formatDate = (dateStr) => {
  return new Date(dateStr).toLocaleDateString('vi-VN', { year: 'numeric', month: 'long', day: 'numeric' });
}

const bookNow = () => {
  router.push("/")
}


const goToService = (serviceId) => {
  router.push(`/DichVuChiTiet/${serviceId}`)
}

const submitQuickBooking = async () => {
  try {
    const thoiGian = new Date(
      `${quickBookingForm.value.date}T${quickBookingForm.value.time}`
    ).toISOString()

    const payload = {
      hoTen: quickBookingForm.value.name,
      soDienThoai: quickBookingForm.value.phone,
      thoiGian,
      dichVus: [{
        dichVuID: serviceDetail.value.id,
        soLuong: 1
      }],
      ghiChu: quickBookingForm.value.notes,
      datTruoc: true
    }

    await apiClient.post('/DatLich', payload)
    alert('Đặt lịch thành công! Chúng tôi sẽ liên hệ với bạn sớm nhất.')
    
    // Reset form
    quickBookingForm.value = {
      name: '',
      phone: '',
      date: '',
      time: '',
      notes: ''
    }
  } catch (err) {
    console.error('Lỗi đặt lịch:', err)
    alert('Đặt lịch thất bại! Vui lòng thử lại.')
  }
}
const fetchData = async () => {
  loading.value = true
  try {
    await Promise.all([
      fetchServiceDetail(),
      fetchRelatedServices(),
      fetchReviews()
    ])
  } finally {
    loading.value = false
  }
}
watch(serviceId, () => {
  fetchData()
}, { immediate: true })
// Lifecycle
onMounted(async () => {
  loading.value = true
  try {
    await Promise.all([
      fetchServiceDetail(),
      fetchRelatedServices(),
      fetchReviews()
    ])
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
/* Base Styles */
.service-detail-page {
  min-height: 100vh;
  background: #fafafa;
}

/* Loading & Error States */
.loading-container,
.error-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 60vh;
  text-align: center;
  padding: 2rem;
}

.loading-spinner {
  border: 4px solid #f3f4f6;
  border-top: 4px solid #78ba7e;
  border-radius: 50%;
  width: 50px;
  height: 50px;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

.error-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
}

.error-container h2 {
  color: #ef4444;
  margin-bottom: 1rem;
}

.back-btn {
  background: #78ba7e;
  color: white;
  padding: 0.8rem 2rem;
  border: none;
  border-radius: 25px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s ease;
  margin-top: 1rem;
}

.back-btn:hover {
  background: #5e8c64;
  transform: translateY(-2px);
}

/* Breadcrumb */
.breadcrumb-container {
  background: white;
  padding: 1rem 0;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
}

.breadcrumb {
  list-style: none;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin: 0;
  padding: 0;
  font-size: 0.9rem;
}

.breadcrumb li {
  display: flex;
  align-items: center;
}

.breadcrumb li:not(:last-child)::after {
  content: '›';
  margin-left: 0.5rem;
  color: #9ca3af;
}

.breadcrumb a {
  color: #78ba7e;
  text-decoration: none;
  transition: color 0.3s;
}

.breadcrumb a:hover {
  color: #5e8c64;
}

.breadcrumb .active {
  color: #6b7280;
  font-weight: 500;
}

/* Service Hero */
.service-hero {
  padding: 3rem 0;
  background: linear-gradient(135deg, #f8fdf8 0%, #f0fdf4 100%);
}

.service-hero-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 4rem;
  align-items: start;
}

.service-image-container {
  position: relative;
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 20px 40px rgba(120, 186, 126, 0.2);
}

.service-hero-image {
  width: 100%;
  height: 400px;
  object-fit: cover;
  transition: transform 0.4s ease;
}

.service-image-container:hover .service-hero-image {
  transform: scale(1.05);
}

.service-badge {
  position: absolute;
  top: 1rem;
  left: 1rem;
  background: linear-gradient(45deg, #78ba7e, #5e8c64);
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
}

.service-info {
  padding: 1rem 0;
}

.service-title {
  font-size: 2.5rem;
  color: #2d4a2d;
  margin-bottom: 2rem;
  font-family: 'Lora', serif;
  font-weight: 700;
  line-height: 1.2;
}

.service-meta {
  margin-bottom: 2rem;
}

.meta-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  align-items: center;
}

.meta-tag {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.8rem 1.2rem;
  border-radius: 25px;
  font-size: 0.9rem;
  font-weight: 500;
}

.price-tag {
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: white;
}

.time-tag {
  background: linear-gradient(135deg, #60a5fa 0%, #3b82f6 100%);
  color: white;
}

.category-tag {
  background: linear-gradient(135deg, #78ba7e 0%, #5e8c64 100%);
  color: white;
}

.meta-tag-icon {
  font-size: 1.2rem;
}

.meta-tag-content {
  display: flex;
  flex-direction: column;
  gap: 0.1rem;
}

.meta-tag-label {
  font-size: 0.75rem;
  opacity: 0.9;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.meta-tag-value {
  font-size: 0.95rem;
  font-weight: 700;
}

.service-description {
  background: white;
  padding: 1.5rem;
  border-radius: 15px;
  margin-bottom: 2rem;
  box-shadow: 0 4px 15px rgba(120, 186, 126, 0.1);
}

.service-description h3 {
  font-size: 1.3rem;
  color: #2d4a2d;
  margin-bottom: 1rem;
  font-weight: 600;
}

.service-description p {
  line-height: 1.7;
  color: #4b5563;
  font-size: 1rem;
}

.service-actions {
  display: flex;
  gap: 1rem;
}

.book-now-btn,
.add-to-cart-btn {
  flex: 1;
  padding: 1rem 2rem;
  border: none;
  border-radius: 25px;
  cursor: pointer;
  font-weight: 600;
  font-size: 1rem;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.book-now-btn {
  background: linear-gradient(45deg, #78ba7e, #5e8c64);
  color: white;
}

.book-now-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(120, 186, 126, 0.4);
}

.add-to-cart-btn {
  background: white;
  color: #78ba7e;
  border: 2px solid #78ba7e;
}

.add-to-cart-btn:hover {
  background: #78ba7e;
  color: white;
  transform: translateY(-2px);
}

/* Service Features */
.service-features {
  padding: 6rem 0;
  background: white;
}

.section-title {
  text-align: center;
  font-size: 2.5rem;
  color: #2d4a2d;
  margin-bottom: 3rem;
  font-family: 'Lora', serif;
  position: relative;
}

.section-title::after {
  content: '';
  display: block;
  width: 80px;
  height: 3px;
  background: linear-gradient(45deg, #78ba7e, #5e8c64);
  margin: 1rem auto 0;
  border-radius: 2px;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
}

.feature-item {
  text-align: center;
  padding: 2rem;
  background: linear-gradient(135deg, #f0fdf4 0%, #dcfce7 100%);
  border-radius: 20px;
  transition: all 0.3s ease;
}

.feature-item:hover {
  transform: translateY(-5px);
  box-shadow: 0 15px 30px rgba(120, 186, 126, 0.2);
}

.feature-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.feature-item h4 {
  font-size: 1.2rem;
  color: #2d4a2d;
  margin-bottom: 1rem;
  font-weight: 600;
}

.feature-item p {
  color: #4b5563;
  line-height: 1.6;
}

/* Service Reviews */
.service-reviews {
  padding: 6rem 0;
  background: white;
}

.overall-rating {
  text-align: center;
  margin-bottom: 3rem;
  font-size: 2rem;
  font-weight: bold;
  color: #2d4a2d;
}

.rating-value {
  font-size: 2.5rem;
  color: #f59e0b;
}

.stars {
  font-size: 1.8rem;
  color: #f59e0b;
  margin: 0 0.5rem;
}

.review-count {
  font-size: 1rem;
  color: #6b7280;
  font-weight: normal;
}

.reviews-list {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.review-item {
  background: linear-gradient(135deg, #f0fdf4 0%, #dcfce7 100%);
  padding: 1.5rem;
  border-radius: 15px;
  box-shadow: 0 4px 15px rgba(120, 186, 126, 0.1);
}

.review-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.reviewer-name {
  font-weight: bold;
  color: #2d4a2d;
}

.review-date {
  font-size: 0.85rem;
  color: #9ca3af;
}

.review-content {
  color: #4b5563;
  line-height: 1.6;
}

/* Related Services */
.related-services {
  padding: 6rem 0;
  background: linear-gradient(135deg, #f8fdf8 0%, #f0fdf4 100%);
}

.related-services-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 2rem;
  margin-top: 3rem;
}

.related-service-card {
  background: white;
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 8px 25px rgba(120, 186, 126, 0.12);
  transition: all 0.4s ease;
  cursor: pointer;
}

.related-service-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 20px 40px rgba(120, 186, 126, 0.2);
}

.related-service-image {
  height: 200px;
  overflow: hidden;
}

.related-service-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.4s ease;
}

.related-service-card:hover .related-service-image img {
  transform: scale(1.1);
}

.related-service-content {
  padding: 1.5rem;
}

.related-service-content h4 {
  font-size: 1.2rem;
  color: #2d4a2d;
  margin-bottom: 1rem;
  font-weight: 600;
}

.related-service-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.related-price {
  color: #f59e0b;
  font-weight: bold;
  font-size: 1.1rem;
}

.related-duration {
  color: #6b7280;
  font-size: 0.9rem;
  background: #f3f4f6;
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
}

.related-description {
  color: #6b7280;
  font-size: 0.9rem;
  line-height: 1.5;
  margin-bottom: 1rem;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  line-clamp: 2; /* chuẩn mới */
  overflow: hidden;
}

.related-view-btn {
  background: none;
  border: none;
  color: #78ba7e;
  font-weight: 600;
  cursor: pointer;
  padding: 0;
  transition: color 0.3s ease;
}

.related-view-btn:hover {
  color: #5e8c64;
}

/* Quick Booking */
.quick-booking {
  padding: 6rem 0;
  background: linear-gradient(135deg, #78ba7e 0%, #6ba371 50%, #5e8c64 100%);
  color: white;
}

.quick-booking-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 4rem;
  align-items: start;
}

.booking-info h2 {
  font-size: 2.5rem;
  margin-bottom: 1.5rem;
  font-family: 'Lora', serif;
}

.booking-info p {
  font-size: 1.1rem;
  margin-bottom: 2rem;
  opacity: 0.9;
  line-height: 1.6;
}

.contact-list {
  list-style: none;
  padding: 0;
}

.contact-list li {
  margin-bottom: 1rem;
  font-size: 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.detail-service-link{
  text-decoration: none;

}
.quick-booking-form {
  background: rgba(255, 255, 255, 0.15);
  padding: 2.5rem;
  border-radius: 20px;
  backdrop-filter: blur(10px);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: white;
  font-size: 0.95rem;
}

.form-group input,
.form-group select,
.form-group textarea {
  width: 100%;
  padding: 0.9rem;
  border: none;
  border-radius: 10px;
  font-size: 1rem;
  background: rgba(255, 255, 255, 0.9);
  transition: all 0.3s ease;
  color: #2d4a2d;
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  outline: none;
  background: white;
  transform: scale(1.02);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.quick-submit-btn {
  background: linear-gradient(45deg, #f59e0b, #f97316);
  color: white;
  padding: 1.1rem 2.5rem;
  border: none;
  border-radius: 50px;
  font-size: 1.1rem;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.3s ease;
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.quick-submit-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(245, 158, 11, 0.4);
}

/* Animations */
@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Mobile Responsive */
@media (max-width: 1024px) {
  .service-hero-content {
    grid-template-columns: 1fr;
    gap: 2rem;
  }
  
  .service-actions {
    flex-direction: column;
  }
  
  .quick-booking-content {
    grid-template-columns: 1fr;
    gap: 2rem;
  }
  
  .features-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .related-services-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .reviews-list {
    gap: 1.5rem;
  }
}

@media (max-width: 768px) {
  .container {
    padding: 0 1rem;
  }
  
  .service-hero {
    padding: 2rem 0;
  }
  
  .service-title {
    font-size: 2rem;
    margin-bottom: 1.5rem;
  }
  
  .service-hero-image {
    height: 300px;
  }
  
  .service-features,
  .related-services,
  .quick-booking,
  .service-reviews {
    padding: 4rem 0;
  }
  
  .section-title {
    font-size: 2rem;
  }
  
  .features-grid {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }
  
  .related-services-grid {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }
  
  .form-row {
    grid-template-columns: 1fr;
  }
  
  .quick-booking-form {
    padding: 2rem;
  }
  
  .booking-info h2 {
    font-size: 2rem;
  }
  
  .service-actions {
    gap: 0.8rem;
  }
  
  .book-now-btn,
  .add-to-cart-btn {
    padding: 0.9rem 1.5rem;
    font-size: 0.95rem;
  }
  
  .overall-rating {
    font-size: 1.8rem;
  }
  
  .rating-value {
    font-size: 2rem;
  }
  
  .stars {
    font-size: 1.5rem;
  }
  
  .review-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
  }
  
  .review-item {
    padding: 1.2rem;
  }
}

@media (max-width: 576px) {
  .service-hero {
    padding: 1.5rem 0;
  }
  
  .service-title {
    font-size: 1.6rem;
    text-align: center;
  }
  
  .service-hero-image {
    height: 250px;
  }
  
  .meta-tags {
    flex-direction: column;
    align-items: stretch;
    gap: 0.8rem;
  }
  
  .meta-tag {
    justify-content: flex-start;
    padding: 0.7rem 1rem;
    border-radius: 20px;
  }
  
  .meta-tag-content {
    flex-direction: row;
    align-items: center;
    gap: 0.5rem;
  }
  
  .meta-tag-label {
    font-size: 0.7rem;
  }
  
  .meta-tag-value {
    font-size: 0.9rem;
  }
  
  .service-description {
    padding: 1.2rem;
  }
  
  .service-description h3 {
    font-size: 1.1rem;
  }
  
  .service-description p {
    font-size: 0.9rem;
  }
  
  .service-features,
  .related-services,
  .quick-booking,
  .service-reviews {
    padding: 3rem 0;
  }
  
  .section-title {
    font-size: 1.6rem;
    margin-bottom: 2rem;
  }
  
  .feature-item {
    padding: 1.2rem;
  }
  
  .feature-icon {
    font-size: 2.5rem;
  }
  
  .feature-item h4 {
    font-size: 1rem;
  }
  
  .feature-item p {
    font-size: 0.9rem;
  }
  
  .related-service-card {
    border-radius: 15px;
  }
  
  .related-service-image {
    height: 150px;
  }
  
  .related-service-content {
    padding: 1.2rem;
  }
  
  .related-service-content h4 {
    font-size: 1rem;
  }
  
  .related-price {
    font-size: 1rem;
  }
  
  .related-duration {
    font-size: 0.8rem;
  }
  
  .related-description {
    font-size: 0.85rem;
  }
  
  .quick-booking-form {
    padding: 1.5rem;
  }
  
  .booking-info h2 {
    font-size: 1.6rem;
  }
  
  .booking-info p {
    font-size: 1rem;
  }
  
  .contact-list li {
    font-size: 0.9rem;
  }
  
  .form-group input,
  .form-group select,
  .form-group textarea {
    padding: 0.8rem;
    font-size: 0.95rem;
  }
  
  .quick-submit-btn {
    padding: 1rem 2rem;
    font-size: 1rem;
  }
  
  .breadcrumb {
    font-size: 0.8rem;
  }
  
  .breadcrumb-container {
    padding: 0.8rem 0;
  }
  
  .service-actions {
    gap: 0.6rem;
  }
  
  .book-now-btn,
  .add-to-cart-btn {
    padding: 0.8rem 1.2rem;
    font-size: 0.9rem;
    border-radius: 20px;
  }
  
  .btn-icon {
    font-size: 1rem;
  }
  
  /* Service badge mobile */
  .service-badge {
    font-size: 0.75rem;
    padding: 0.4rem 0.8rem;
    border-radius: 15px;
  }
  
  /* Loading and error states mobile */
  .loading-container,
  .error-container {
    padding: 1.5rem;
    min-height: 50vh;
  }
  
  .error-icon {
    font-size: 3rem;
  }
  
  .error-container h2 {
    font-size: 1.4rem;
  }
  
  .back-btn {
    padding: 0.7rem 1.5rem;
    font-size: 0.9rem;
    border-radius: 20px;
  }
  
  .overall-rating {
    font-size: 1.5rem;
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .rating-value {
    font-size: 1.8rem;
  }
  
  .stars {
    font-size: 1.3rem;
  }
  
  .review-count {
    font-size: 0.9rem;
  }
  
  .review-header {
    flex-direction: column;
    gap: 0.3rem;
  }
  
  .review-item {
    padding: 1rem;
  }
  
  .reviews-list {
    gap: 1rem;
  }
}

/* Extra small screens */
@media (max-width: 480px) {
  .service-title {
    font-size: 1.4rem;
  }
  
  .service-hero-image {
    height: 200px;
  }
  
  .section-title {
    font-size: 1.4rem;
  }
  
  .booking-info h2 {
    font-size: 1.4rem;
  }
  
  .meta-item {
    flex-direction: column;
    text-align: center;
    gap: 0.5rem;
  }
  
  .service-actions {
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .book-now-btn,
  .add-to-cart-btn {
    width: 100%;
    justify-content: center;
  }
  
  .features-grid {
    gap: 1rem;
  }
  
  .feature-item {
    padding: 1rem;
  }
  
  .related-services-grid {
    gap: 1rem;
  }
  
  .quick-booking-form {
    padding: 1.2rem;
  }
  
  .form-group {
    margin-bottom: 1.2rem;
  }
}

/* High DPI screens */
@media (-webkit-min-device-pixel-ratio: 2), (min-resolution: 192dpi) {
  .service-hero-image,
  .related-service-image img {
    image-rendering: -webkit-optimize-contrast;
    image-rendering: crisp-edges;
  }
}

/* Dark mode support */
@media (prefers-color-scheme: dark) {
  .service-detail-page {
    background: #1a1a1a;
    color: #e5e5e5;
  }
  
  .breadcrumb-container {
    background: #2d2d2d;
  }
  
  .service-hero {
    background: linear-gradient(135deg, #2d2d2d 0%, #1a1a1a 100%);
  }
  
  .meta-item,
  .service-description {
    background: #2d2d2d;
    color: #e5e5e5;
  }
  
  .service-features {
    background: #1a1a1a;
  }
  
  .feature-item {
    background: linear-gradient(135deg, #2d2d2d 0%, #3d3d3d 100%);
    color: #e5e5e5;
  }
  
  .related-service-card {
    background: #2d2d2d;
    color: #e5e5e5;
  }
  
  .service-reviews {
    background: #1a1a1a;
  }
  
  .review-item {
    background: linear-gradient(135deg, #2d2d2d 0%, #3d3d3d 100%);
    color: #e5e5e5;
  }
  
  .overall-rating,
  .reviewer-name {
    color: #e5e5e5;
  }
  
  .review-content {
    color: #b3b3b3;
  }
  
  .review-date {
    color: #808080;
  }
}

/* Print styles */
@media print {
  .service-actions,
  .quick-booking,
  .breadcrumb-container {
    display: none;
  }
  
  .service-hero,
  .service-features,
  .related-services {
    background: white !important;
    color: black !important;
  }
  
  .service-hero-content {
    grid-template-columns: 1fr;
  }
  
  .service-hero-image {
    max-height: 300px;
  }
}
</style>