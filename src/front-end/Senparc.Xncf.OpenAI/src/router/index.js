import Vue from 'vue'
import VueRouter from 'vue-router'

Vue.use(VueRouter)

let routes = [

  {
    path: '/',
    component: () =>
      import('../views/Home.vue')
  },
  // OpenAI路由
  {
    path: '/Module/XncfStoreOpenAI/openindex',
    component: () =>
      import('../views/OpenAI/index.vue')
  },
  {
    path: '/Module/XncfStoreOpenAI/opendetail',
    component: () =>
      import('../views/OpenAI/detail.vue')
  },
]
const router = new VueRouter({
  routes
})

export default router
