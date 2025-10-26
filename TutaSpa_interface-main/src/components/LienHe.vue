<template>
  <section class="contact-section py-5" style="background-color: #f6fff8">
    <div class="container">
      <div class="text-center mb-5 animate__animated animate__fadeInDown">
        <h2 class="display-5 fw-bold text-bright-green">
          Liên hệ với TuTa Spa
        </h2>
        <p class="text-muted">
          Hãy để chúng tôi chăm sóc vẻ đẹp và sự thư giãn của bạn!
        </p>
      </div>

      <!-- Bố cục 3 cột -->
      <div
        class="row g-4 align-items-stretch animate__animated animate__fadeInUp"
      >
        <!-- Thông tin liên hệ -->
        <div class="col-lg-4">
          <div class="bg-white p-4 rounded-4 shadow h-100">
            <h5 class="fw-bold mb-4 text-bright-green">
              <i class="fa-solid fa-spa me-2"></i>Thông tin liên hệ
            </h5>
            <ul class="list-unstyled fs-6">
              <li class="mb-3">
                <i class="fa-solid fa-location-dot me-2 text-primary-green"></i>
                31 Nguyễn Mộng Tuân, Hòa Minh, Liên Chiểu, Đà Nẵng
              </li>
              <li class="mb-3">
                <i class="fa-solid fa-phone me-2 text-success"></i>
                <a href="tel:0909123456" class="text-decoration-none text-dark"
                  >0909 123 456</a
                >
              </li>
              <li class="mb-3">
                <i class="fa-solid fa-envelope me-2 text-danger"></i>
                <a
                  href="mailto:contact@tatusapa.vn"
                  class="text-decoration-none text-dark"
                  >contact@tatusapa.vn</a
                >
              </li>
              <li>
                <i class="fa-solid fa-globe me-2 text-info"></i>
                <a
                  href="https://tatusapa.vn"
                  class="text-decoration-none text-dark"
                  target="_blank"
                  >www.tatusapa.vn</a
                >
              </li>
            </ul>
            <div class="mt-4">
              <h6 class="fw-bold text-bright-green">Giờ làm việc:</h6>
              <p class="mb-1">Thứ 2 - Thứ 6: 8:00 - 18:00</p>
              <p>Thứ 7 - Chủ Nhật: 9:00 - 15:00</p>
            </div>
          </div>
        </div>

        <!-- Form liên hệ -->
        <div class="col-lg-4">
          <form
            class="bg-white p-4 rounded-4 shadow h-100"
            @submit.prevent="submitForm"
          >
            <h5 class="fw-bold text-bright-green mb-4">
              <i class="fa-solid fa-pen-nib me-2"></i>Gửi lời nhắn
            </h5>
            <div class="row g-3">
              <div class="col-md-6">
                <div class="form-floating">
                  <input
                    type="text"
                    class="form-control"
                    id="nameInput"
                    placeholder="Họ tên"
                    v-model="form.name"
                    required
                  />
                  <label for="nameInput">Họ tên</label>
                </div>
              </div>
              <div class="col-md-6">
                <div class="form-floating">
                  <input
                    type="email"
                    class="form-control"
                    id="emailInput"
                    placeholder="Email"
                    v-model="form.email"
                    required
                  />
                  <label for="emailInput">Email</label>
                </div>
              </div>
              <div class="col-md-12">
                <div class="form-floating">
                  <input
                    type="text"
                    class="form-control"
                    id="subjectInput"
                    placeholder="Tiêu đề"
                    v-model="form.subject"
                  />
                  <label for="subjectInput">Tiêu đề</label>
                </div>
              </div>
              <div class="col-md-12">
                <div class="form-floating">
                  <textarea
                    class="form-control"
                    placeholder="Nội dung tin nhắn"
                    id="messageInput"
                    style="height: 120px"
                    v-model="form.message"
                    required
                  ></textarea>
                  <label for="messageInput">Nội dung</label>
                </div>
              </div>
              <div class="col-12 text-end">
                <button
                  class="btn btn-primary-green px-4 py-2 rounded-pill shadow-sm"
                  type="submit"
                >
                  <i class="fa-solid fa-paper-plane me-2"></i>Gửi liên hệ
                </button>
              </div>
            </div>
          </form>
        </div>

        <!-- Google Map -->
        <div class="col-lg-4">
          <div class="map-wrapper rounded-4 overflow-hidden shadow h-100">
            <a
              href="https://www.google.com/maps/place/31+Nguyễn+Mộng+Tuân,+Hoà+Minh,+Liên+Chiểu,+Đà+Nẵng+550000"
              target="_blank"
              class="d-block w-100 h-100"
            >
              <iframe
                src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3834.168381775547!2d108.1698262!3d16.0633039!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3142191ed695fc89%3A0xb837389a53933ca9!2zMzEgTmfDtG4gTeG7mW5nIFR1w6puLCBIb8OgIE1pbmgsIExpw6puIENow61hLCDEkMOgIE5hbmc!5e0!3m2!1svi!2s!4v1721903400000"
                width="100%"
                height="100%"
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
            <i class="fa-solid fa-map-location-dot me-2 text-primary-green"></i
            >Xem bản đồ
          </a>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { reactive } from "vue";

const form = reactive({
  name: "",
  email: "",
  subject: "",
  message: "",
});

const submitForm = () => {
  console.log("Dữ liệu form:", form);
  alert("Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm.");
  form.name = "";
  form.email = "";
  form.subject = "";
  form.message = "";
};
</script>

<style scoped>
.btn-primary-green {
  background-color: #007e5a;
  color: white;
  border: none;
  transition: all 0.3s ease;
}

.btn-primary-green:hover {
  background-color: #0a8d68;
  box-shadow: 0 4px 12px rgba(0, 126, 90, 0.3);
}

.map-wrapper {
  height: 100%;
  min-height: 320px;
}

.form-control:focus {
  border-color: #007e5a;
  box-shadow: 0 0 0 0.25rem rgba(0, 126, 90, 0.25);
}

.form-floating > .form-control,
.form-floating > textarea {
  border-radius: 0.75rem;
}

.text-bright-green {
  color: #28a745 !important;
}

textarea.form-control {
  resize: none;
}
</style>
