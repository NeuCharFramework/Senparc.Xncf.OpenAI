export default [
  // OpenAI模块加载路由
  {
    path: 'SenparcXncf/openindex',
    name: 'OpenAI首页',
    component: () =>
      import('../views/OpenAI/index.vue')
  },
  {
    path: 'SenparcXncf/opendetail',
    name: 'OpenAI对话页',
    component: () =>
      import('../views/OpenAI/detail.vue')
  },
]
