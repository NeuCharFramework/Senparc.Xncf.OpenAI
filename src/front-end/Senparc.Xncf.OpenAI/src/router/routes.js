export default [
  // OpenAI模块加载路由
  {
    path: 'XncfStoreOpenAI/openindex',
    name: 'OpenAI首页',
    component: () =>
      import(/* webpackChunkName: "about" */ '../views/OpenAI/index.vue')
  },
  {
    path: 'XncfStoreOpenAI/opendetail',
    name: 'OpenAI对话页',
    component: () =>
      import(/* webpackChunkName: "about" */ '../views/OpenAI/detail.vue')
  },
]
