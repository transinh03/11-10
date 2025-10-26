import { createRouter, createWebHistory } from "vue-router";

// Import các component
import Home from "../components/home.vue";
import DichVu from "../components/DichVu.vue";
import GioiThieu from "../components/GioiThieu.vue";
import LienHe from "../components/LienHe.vue";
import login from "../components/login.vue";
import register from "../components/register.vue";
import ChiTietDichVu from "../components/Chitiet.vue";
import DatLich from "../components/DatLich.vue";
import ForgetPassword from "../components/ForgetPassword.vue";
import DanhGia from "../components/DanhGia.vue";
import LichSuDatLich from "../components/LichSuDatLich.vue";
import tinhtong from "../components/tinhtong.vue";
import hoanghiem from "../components/hoanghiem.vue";

const routes = [
  { path: "/", name: "Home", component: Home },
  { path: "/DichVu", name: "DichVu", component: DichVu },
  { path: "/GioiThieu", name: "GioiThieu", component: GioiThieu },
  { path: "/LienHe", name: "LienHe", component: LienHe },
  { path: "/tinhtong", name: "tinhtong", component: tinhtong },
  { path: "/DatLich/:serviceId?", name: "DatLich", component: DatLich },
  { path: "/ChiTietDichVu", name: "ChiTietDichVu", component: ChiTietDichVu },
  { path: "/DanhGia/:id?", name: "DanhGia", component: DanhGia },
  { path: "/LichSuDatLich", name: "LichSuDatLich", component: LichSuDatLich },
  { path: "/hoanghiem", name: "hoanghiem", component: hoanghiem },
  {
    path: "/DichVuChiTiet/:id",
    name: "DichVuChiTiet",
    component: ChiTietDichVu,
  },

  // {
  //   path: '/DanhGia/:id?',
  //   name: 'DanhGia',
  //   component: () => import('../views/DanhGia.vue')  // hoặc đường dẫn tương ứng với bạn
  // },

  {
    path: "/login",
    name: "login",
    component: login,
  },
  {
    path: "/register",
    name: "register",
    component: register,
  },
  {
    path: "/ForgetPassword",
    name: "ForgetPassword",
    component: ForgetPassword,
  },

  // Route động cho chi tiết dịch vụ
  {
    path: "/dichvu/:id",
    name: "ChiTietDichVu",
    component: () => import("../components/ChiTietDichVu.vue"),
    props: true, // nếu muốn truyền :id làm prop
  },
];

// const router = createRouter({
//   history: createWebHistory(),
//   routes,
// });
const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior(to, from, savedPosition) {
    if (to.hash) {
      return {
        el: to.hash,
        behavior: `smooth`,
      };
    }
    return { top: 0 };
  },
});

export default router;
