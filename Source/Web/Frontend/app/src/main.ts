import Vue from 'vue';
import App from './App.vue';
import router from './router';
import store from './store';

import { library } from '@fortawesome/fontawesome-svg-core'
import { fas } from '@fortawesome/free-solid-svg-icons'

import BarcodesModule from "@/store/modules/BarcodesModule";

library.add(fas)

import "./assets/css/pvscan-global.css";

Vue.config.productionTip = false;

new Vue({
  router,
  store,
  render: (h) => h(App),
  async mounted() {
      await BarcodesModule.Initialize();
  }
}).$mount('#app');
