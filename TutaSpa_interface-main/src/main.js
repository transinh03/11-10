import { createApp } from "vue";
//import "./style.css";
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap/dist/js/bootstrap.bundle.min.js";
import "@fortawesome/fontawesome-free/css/all.min.css";
import './assets/main.css'
import 'vanilla-tilt'
import App from "./App.vue";
import router from "./router/router";
import { createPinia } from "pinia";
const pinia = createPinia();
createApp(App).use(pinia).use(router).mount("#app");
