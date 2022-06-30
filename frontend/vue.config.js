const { defineConfig } = require('@vue/cli-service')
const WebpackRequireFrom = require('webpack-require-from')
module.exports = defineConfig({
  transpileDependencies: true,
  configureWebpack: {
    plugins: [
      new WebpackRequireFrom({
        variableName: 'resourceBasePath'
      })
    ]
  }
})
