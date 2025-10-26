<script setup>
import {
  Chart as ChartJS,
  BarElement,
  CategoryScale,
  LinearScale,
  Tooltip,
  Legend,
  Title,
} from "chart.js";
import { Bar } from "vue-chartjs";

// Đăng ký các thành phần cần thiết cho biểu đồ
ChartJS.register(
  BarElement,
  CategoryScale,
  LinearScale,
  Tooltip,
  Legend,
  Title
);

// Nhận props từ cha truyền xuống
defineProps({
  data: Object,
  options: Object,
});
</script>

<template>
  <div style="max-width: 100%; height: auto;">
    <Bar :data="data" :options="options" />
  </div>
</template>