import Vue from 'vue';
import App from './App.vue';
import router from './router';
import store from './store';

import { library } from '@fortawesome/fontawesome-svg-core'
import { fas } from '@fortawesome/free-solid-svg-icons'

import BarcodesModule from "@/store/modules/BarcodesModule";
import UIStateModule from "@/store/modules/UIStateModule";

import Barcode from "@/models/Barcode";

library.add(fas)

import "./assets/css/pvscan-global.css";

Vue.config.productionTip = false;

new Vue({
  router,
  store,
  render: (h) => h(App),
  async mounted() {
      await UIStateModule.Initialize();
      await BarcodesModule.Initialize();

    //   let barcodes: Barcode[] = [];
      
    //   for (let i = 0; i < 1000; ++i) {
    //       barcodes.push({
    //           BarcodeFormat: 10,
    //           Favorite: false,
    //           GUID: "guid_" + i.toString(),
    //           Hash: "hash_" + i.toString(),
    //           LastUpdateTime: new Date(),
    //           ScanLocation: {
    //               Latitude: 30,
    //               Longitude: 50,
    //           },
    //           ScanTime: new Date,
    //           Text: "Something_" + i.toString(),
    //       });
    //   }

    //   await BarcodesModule.AddBarcodes(barcodes);
  }
}).$mount('#app');
