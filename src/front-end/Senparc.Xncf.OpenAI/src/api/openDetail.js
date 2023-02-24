import request from "../utils/request"
const baseUrl = ""//待定共用

// 发送消息
export function postNews(params) {
    return request({
        url: "/Senparc.Xncf.OpenAI/GPT3AppService/Xncf.OpenAI_GPT3AppService.CreateCompletionStreamAsync",
        params,
        method: 'post'
    })
}
