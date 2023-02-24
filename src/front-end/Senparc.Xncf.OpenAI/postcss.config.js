const name = defaultSettings.title || 'NCF OpenAI' // page title
module.exports = {
  plugins: {
    autoprefixer: {}
  },
  devServer: {
    https: false,
    port: port,
    open: true,
    disableHostCheck: true, // 是否开启域名检查
    overlay: {
      warnings: false,
      errors: false
    },
    target: 'https://localhost:44311/api', // 跨域开发域名
  },
  configureWebpack: {
    // provide the app's title in webpack's name field, so that
    // it can be accessed in index.html to inject the correct title.
    name: name,
    resolve: {
      alias: {
        '@': resolve('src')
      }
    },
    //2 使用这个插件copy-webpack-plugin
    // 暂时未知做什么的，注释
    // plugins: [
    //   new CopyWebpackPlugin([
    //     { from: '要拷贝的文件', to: '要拷贝到的路径（不写默认是打包的根目录）' }
    //   ])
    // ]
  },
  chainWebpack(config) {
    // it can improve the speed of the first screen, it is recommended to turn on preload
    // it can improve the speed of the first screen, it is recommended to turn on preload
    config.plugin('preload').tap(() => [
      {
        rel: 'preload',
        // to ignore runtime.js
        // https://github.com/vuejs/vue-cli/blob/dev/packages/@vue/cli-service/lib/config/app.js#L171
        fileBlacklist: [/\.map$/, /hot-update\.js$/, /runtime\..*\.js$/],
        include: 'initial'
      }
    ])

    // when there are many pages, it will cause too many meaningless requests
    config.plugins.delete('prefetch')

    // set svg-sprite-loader
    config.module
      .rule('svg')
      .exclude.add(resolve('src/icons'))
      .end()
    config.module
      .rule('icons')
      .test(/\.svg$/)
      .include.add(resolve('src/icons'))
      .end()
      .use('svg-sprite-loader')
      .loader('svg-sprite-loader')
      .options({
        symbolId: 'icon-[name]'
      })
      .end()

    config
      .when(process.env.NODE_ENV !== 'development',
        config => {
          config
            .plugin('ScriptExtHtmlWebpackPlugin')
            .after('html')
            .use('script-ext-html-webpack-plugin', [{
              // `runtime` must same as runtimeChunk name. default is `runtime` | ' runtime '必须与runtimeChunk name相同。默认是“运行时”
              inline: /runtime\..*\.js$/
            }])
            .end()
          config
            .optimization.splitChunks({
              chunks: 'all',
              cacheGroups: {
                libs: {
                  name: 'chunk-libs',
                  test: /[\\/]node_modules[\\/]/,
                  priority: 10,
                  chunks: 'initial' // only package third parties that are initially dependent 只打包最初依赖的第三方
                },
                elementUI: {
                  name: 'chunk-elementUI', // split elementUI into a single package 将elementUI拆分为单个包
                  priority: 20, // the weight needs to be larger than libs and app or it will be packaged into libs or app 权重需要大于libs和app，否则它将被打包到libs或app中
                  test: /[\\/]node_modules[\\/]_?element-ui(.*)/ // in order to adapt to cnpm 为了适应CNPM
                },
                commons: {
                  name: 'chunk-commons',
                  test: resolve('src/components'), // can customize your rules  可以自定义规则
                  minChunks: 3, //  minimum common number
                  priority: 5,
                  reuseExistingChunk: true
                }
              }
            })
          // https:// webpack.js.org/configuration/optimization/#optimizationruntimechunk
          config.optimization.runtimeChunk('single')
        }
      )
  }
};
