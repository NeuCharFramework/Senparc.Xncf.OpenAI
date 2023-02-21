import Vue from 'vue'
import VueRouter from 'vue-router'
import Home from '../views/Home.vue'

Vue.use(VueRouter)

let routes = [
  {
    path: '/',
    name: 'home',
    component: Home
  },
  // XncfStore路由
  // {
  //   path: '/appstore',
  //   component: () =>
  //     import('../views/XncfStore/shopping/Index.vue')
  // },
  // {
  //   path: '/shoppingIndex',
  //   component: () =>
  //     import(/* webpackChunkName: "about" */ '../views/XncfStore/shopping/Index.vue')
  // },
  // {
  //   path: '/shoppingdetail',
  //   component: () =>
  //     import(/* webpackChunkName: "about" */ '../views/XncfStore/shopping/detail.vue')
  // },
  // {
  //   path: '/shoppingapplication',
  //   component: () =>
  //     import(/* webpackChunkName: "about" */ '../views/XncfStore/shopping/application.vue')
  // },
  // {
  //   path: '/application',
  //   component: () =>
  //     import(/* webpackChunkName: "about" */ '../views/XncfStore/operation/application.vue')
  // },
  // OpenAI路由
  {
    path: '/openindex',
    component: () =>
      import(/* webpackChunkName: "about" */ '../views/OpenAI/index.vue')
  },
  {
    path: '/opendetail',
    component: () =>
      import(/* webpackChunkName: "about" */ '../views/OpenAI/detail.vue')
  },

]
const router = new VueRouter({
  routes
})

export default router
