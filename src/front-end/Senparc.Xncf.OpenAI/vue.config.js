// const HtmlWebpackPlugin = require('html-webpack-plugin');
// const CopyWebpackPlugin = require("copy-webpack-plugin");

module.exports = {
  publicPath: './', // 这个值被设置为空字符串 ('') 或是相对路径 ('./')，这样所有的资源都会被链接为相对路径，这样打出来的包可以被部署在任意路径。
  // outputDir: '../Senparc.Xncf.Admin/public/dist',
  outputDir: 'dist',
  assetsDir: 'static',
  indexPath: "SenparcXncfOpenAI.html",

  devServer: {
    https: false,
    open: true,
    disableHostCheck: true, // 是否开启域名检查
    overlay: {
      warnings: false,
      errors: false
    },
    proxy: {
      // 所有的请求起始部分全部用 '/api'代替，比如访问"https://192.168.1.4/movie"，那么简写成"/api/movie"即可
      '/api': {
        target: 'https://localhost:44311/api', // 开发域名
        changeOrigin: true,
        ws: true,
        pathRewrite: {
          '^/api': ''
        }
      }
    },
  },

  // 链式调用修改打包html文件名称：仅限于build:app打包时有效
  // chainWebpack: config => {
  //   config.plugin('html').tap(options => {
  //     options.filename = 'SenparcXncfOpenAI.html';
  //     // console.log("@@@", options.filename);
  //     return options;
  //   })
  // },


  // 修改打包html文件名称：仅限于build:app打包时有效
  // configureWebpack: {
  //   output: {
  //     libraryExport: 'default',
  //     filename: 'SenparcXncfOpenAI.html'
  //   },
  // },

  // configureWebpack: {
  //   output: {
  //     plugins: [
  //       new HtmlWebpackPlugin({
  //         template: 'SenparcXncfOpenAI.html'
  //       }),
  //     ],
  //   }
  // }
  // pages: {
  //   index: {
  //     // page 的入口
  //     entry: 'src/index/main.js',
  //     // 模板来源
  //     template: 'public/index.html',
  //     // 在 dist/index.html 的输出
  //     filename: 'SenparcXncfOpenAI.html',
  //     // 当使用 title 选项时，
  //     // template 中的 title 标签需要是 <title><%= htmlWebpackPlugin.options.title %></title>
  //     title: 'Index SenparcXncfOpenAI',
  //     // 在这个页面中包含的块，默认情况下会包含
  //     // 提取出来的通用 chunk 和 vendor chunk。
  //     chunks: ['chunk-vendors', 'chunk-common', 'index']
  //   },
  //   // 当使用只有入口的字符串格式时，
  //   // 模板会被推导为 `public/subpage.html`
  //   // 并且如果找不到的话，就回退到 `public/index.html`。
  //   // 输出文件名会被推导为 `subpage.html`。
  //   subpage: 'src/module.js'
  // }
  // configureWebpack: {
  //   output: {
  //     // filename: 'SenparcXncfOpenAI.html',//单独输出文件
  //     plugins: [
  //       new HtmlWebpackPlugin({
  //         template: "./index.html",
  //         filename: 'SenparcXncfOpenAI.html',
  //       })
  //     ]
  //   },
  //   // chainWebpack: config => {
  //   //   config.plugin('html').tap(options => {
  //   //     options.filename = 'SenparcXncfOpenAI.html';
  //   //     // console.log("@@@", options.filename);
  //   //     return options;
  //   //   })
  //   // },
  // }





}