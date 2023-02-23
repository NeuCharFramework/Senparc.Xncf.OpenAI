# Senparc.Xncf.OpenAI
OpenAI 和 ChatGPT 等接口对接

## 使用方式

1. 将当前 XNCF 项目引用到 [NCF 项目](https://github.com/NeuCharFramework/NCF)中（引用源码项目或 Nuget 包）。 [NCF 文档](https://www.ncf.pub/docs)
2. 在 NCF 后台安装 `Senparc.Xncf.OpenAI` 模块。
3. 通过 UI 或 Swagger 进行接口测试。

### Swagger 测试

打开 NCF Swagger 地址：`<域名>/swagger/index.html?urls.primaryName=Senparc.Xncf.OpenAI`，如下图：

![image](https://user-images.githubusercontent.com/2281927/220394641-11810f4b-c720-41b2-9ec5-a69fc73e142f.png)

### UI

使用前后端分离版本的 UI项目（front-end），集成到 NCF 项目中，即可看到左侧菜单中的 `OpenAI`。（更新中）

## 前端运行步骤-分布式
## 注：Senparc.Xncf.OpenAI模块所有内容已全部配置，无需动任何代码
1. 步骤一 ：在Senparc.Xncf.OpenAI模块中安装所有依赖 -命令： npm install
2. 步骤二 ：建议使用 Senparc.Xncf.Admin 模块来进行配合分布式，打开并运行 Senparc.Xncf.Admin 模块
3. 步骤三 ：打包 Senparc.Xncf.OpenAI 全部内容 -命令： npm run build:module
4. 步骤四 ：在Senparc.Xncf.Admin模块下展开/Module/home路径，点击加载模块即可使用