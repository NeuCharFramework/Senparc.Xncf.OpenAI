import request from "@/utils/request"
const baseUrl = ""//待定共用

// 发送消息
export function postNews(data) {
    return request({
        url: "/api/Senparc.Xncf.OpenAI/GPT3AppService/Xncf.OpenAI_GPT3AppService.CreateCompletionStreamAsync",
        data,
        method: 'post'
    })
}
export function testfunction() {
    alert('调用了暴露函数')
}

export function getOpen(data = {}) {
    console.log(data);
    return request({
        url: "",
        data,
        method: 'get'
    })
}
