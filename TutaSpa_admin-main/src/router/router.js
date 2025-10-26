import { createRouter, createWebHistory } from "vue-router";
import Dashboard from "../components/Dashboard.vue";
import khuyenMai from "../components/khuyenMai.vue";
import dichVu from "../components/dichVu.vue";
import Loaidichvu from "../components/Loaidichvu.vue";
import QLDichVu from "../components/QLDichVu.vue";
import kho from "../components/kho.vue";
import ThuNgan from "../components/ThuNgan.vue";
import login from "../components/login.vue";
import EmployeeManagerment from "../components/EmployeeManagerment.vue";
import QuanLySanPham from "../components/QuanLySanPham.vue";
import LoaiSanPham from "../components/LoaiSanPham.vue";
import SanPham from "../components/sanpham.vue";
import DanhGiaKhachHang from "../components/DanhGiaKhachHang.vue";
import Banggiadichvu from "../components/Banggiadichvu.vue";
import Chat from "../components/chat.vue";
import ThongKe from "../components/ThongKe.vue";
import ChangePassword from "../components/ChangePassword.vue";
import ForgotPassword from "../components/ForgotPassword.vue";
import test from "../components/test.vue";
import thongkehoadon from "../components/thongkehoadon.vue";

const routes = [
  { path: "/", name: "Dashboard", component: Dashboard },
  { path: "/test", name: "test", component: test },
  {
    path: "/thongkehoadon",
    name: "thongkehoadon",
    component: thongkehoadon,
  },
  { path: "/khuyenMai", name: "khuyenMai", component: khuyenMai },
  { path: "/dichVu", name: "dichVu", component: dichVu },
  { path: "/Loaidichvu", name: "Loaidichvu", component: Loaidichvu },
  { path: "/QlDichVu", name: "QlDichVu", component: QLDichVu },
  { path: "/kho", name: "kho", component: kho },
  { path: "/ThuNgan", name: "ThuNgan", component: ThuNgan },
  { path: "/employees", name: "employees", component: EmployeeManagerment },
  { path: "/QuanLySanPham", name: "QuanLySanPham", component: QuanLySanPham },
  { path: "/categories", name: "LoaiSanPham", component: LoaiSanPham },
  { path: "/chat", name: "Chat", component: Chat },
  { path: "/ThongKe", name: "ThongKe", component: ThongKe },
  {
    path: "/forget-password",
    name: "ForgetPassword",
    component: ForgotPassword,
  },
  {
    path: "/ChangePassword",
    name: "ChangePassword",
    component: ChangePassword,
  },
  { path: "/Products", name: "sanpham", component: SanPham },
  {
    path: "/DanhGiaKhachHang",
    name: "DanhGiaKhachHang",
    component: DanhGiaKhachHang,
  },
  { path: "/Banggiadichvu", name: "Banggiadichvu", component: Banggiadichvu },
  {
    path: "/login",
    name: "login",
    component: login,
    meta: {
      layout: false, // Không sử dụng layout chung
      requiresAuth: false,
    },
  },
];
const router = createRouter({
  history: createWebHistory(),
  routes,
});
export default router;
