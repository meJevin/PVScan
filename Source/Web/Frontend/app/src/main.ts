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

    //   for (let i = 0; i < 1000; ++i) {
    //       await BarcodesModule.AddBarcodes([{
    //         BarcodeFormat: 0,
    //         Text: "Something 1",
    //         ScanLocation: {
    //             Latitude: 29,
    //             Longitude: 30,
    //         },
    //         ScanTime: new Date(2000, 0, 1),
    //         Favorite: false,
    //         Hash: "hash1",
    //         GUID: "guid1",
    //         LastUpdateTime: new Date(2000, 5, 5), 
    //       }])
    //   }
  }
}).$mount('#app');
