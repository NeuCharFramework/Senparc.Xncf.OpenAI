module.exports = {
  publicPath: './', // 这个值被设置为空字符串 ('') 或是相对路径 ('./')，这样所有的资源都会被链接为相对路径，这样打出来的包可以被部署在任意路径。
  configureWebpack: {
    resolve: {
      symlinks: false
    }
  },
  devServer: {
    proxy: {
      // 所有的请求起始部分全部用 '/api'代替，比如访问"https://192.168.1.4/movie"，那么简写成"/api/movie"即可
      '/api': {
        target: 'https://localhost:44311/api', // 开发域名
      }
    }
  }
}