export default [
  // 原测试的
  // {
  //   path: 'b/about1',
  //   name: 'b-about1',
  //   component: () =>
  //     import(/* webpackChunkName: "about" */ '../views/About.vue')
  // },
  // {
  //   path: 'b/home1',
  //   name: 'b-home1',
  //   component: () =>
  //     import(/* webpackChunkName: "about" */ '../views/Home.vue')
  // },

  // XncfStore模块加载路由
  // {
  //   path: 'b/shoppingIndex',
  //   name: 'XncfStore商店',
  //   component: () =>
  //     import(/* webpackChunkName: "about" */ '../views/XncfStore/shopping/Index.vue')
  // },
  // {
  //   path: 'b/shoppingdetail',
  //   name: 'XncfStore商店详情',
  //   component: () =>
  //     import(/* webpackChunkName: "about" */ '../views/XncfStore/shopping/detail.vue')
  // },
  // {
  //   path: 'b/shoppingapplication',
  //   name: 'XncfStore个人应用',
  //   component: () =>
  //     import(/* webpackChunkName: "about" */ '../views/XncfStore/shopping/application.vue')
  // },
  // {
  //   path: 'b/application',
  //   name: 'XncfStore应用中枢',
  //   component: () =>
  //     import(/* webpackChunkName: "about" */ '../views/XncfStore/operation/application.vue')
  // },
  // OpenAI模块加载路由
  {
    path: 'b/openindex',
    name: 'Open首页',
    component: () =>
      import(/* webpackChunkName: "about" */ '../views/OpenAI/index.vue')
  },
  {
    path: 'b/opendetail',
    name: 'Open详情',
    component: () =>
      import(/* webpackChunkName: "about" */ '../views/OpenAI/detail.vue')
  },
  //
  // {
  //   path: '/shoppingIndex',
  //   component: () =>
  //     import(/* webpackChunkName: "about" */ '../views/XncfStore/shopping/Index.vue')///views/XncfStore/shopping/Index.vue
  // },

]
