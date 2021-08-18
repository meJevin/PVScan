import Vue from 'vue';
import VueRouter, { RouteConfig } from 'vue-router';
import MainView from '../views/MainView.vue';

Vue.use(VueRouter);

const routes: Array<RouteConfig> = [
  {
    path: '/',
    name: 'MainView',
    component: MainView,
  },
];

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes,
});

export default router;
