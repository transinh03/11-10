<template>
  <div class="chi-tieu-wrapper p-3">
    <h2 class="text-center mb-4">Tính tổng chi tiêu</h2>

    <div class="chi-tieu-grid">
      <!-- Bên trái -->
      <div class="chi-tieu-left">
        <h5 class="mb-2">Nhập dữ liệu</h5>
        <textarea
          v-model="inputText"
          rows="12"
          class="form-control mb-3 input-scroll"
          placeholder="Nhập dữ liệu, mỗi dòng một chi tiêu"
        ></textarea>
        <button class="btn btn-primary w-100" @click="tinhTong">
          Tính tổng
        </button>
      </div>

      <!-- Bên phải -->
      <div class="chi-tieu-right">
        <h5 class="mb-2">Kết quả</h5>
        <div v-if="soDaTach.length" class="mb-3">
          <h6>Các số đã tách:</h6>
          <ul class="list-group list-scroll">
            <li
              v-for="(so, index) in soDaTach"
              :key="index"
              class="list-group-item"
            >
              {{ so }}k
            </li>
          </ul>
        </div>

        <div v-if="tong !== null" class="alert alert-success mt-3">
          Tổng tiền: <strong>{{ tong }}k</strong>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";

const inputText = ref();

const tong = ref(null);
const soDaTach = ref([]);

const tinhTong = () => {
  const lines = inputText.value.split("\n");
  let sum = 0;
  let numbers = [];

  for (let line of lines) {
    const match = line.match(/(\d+)k/);
    if (match) {
      const num = parseInt(match[1]);
      numbers.push(num);
      sum += num;
    }
  }

  soDaTach.value = numbers;
  tong.value = sum;
};
</script>

<style scoped>
.chi-tieu-wrapper {
  max-width: 100%;
  margin: auto;
}

.chi-tieu-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 15px;
}

/* Textarea chỉ cuộn ngang trong chính nó */
.input-scroll {
  white-space: nowrap;
  overflow-x: auto;
  overflow-y: auto;
}

/* Danh sách bên phải có thanh cuộn dọc và cao bằng textarea */
.list-scroll {
  max-height: 260px; /* bằng với textarea rows=12 (tùy chỉnh) */
  overflow-y: auto;
}

/* Làm list nhỏ gọn */
.small-list .list-group-item {
  padding: 6px 10px;
  font-size: 14px;
}
</style>
