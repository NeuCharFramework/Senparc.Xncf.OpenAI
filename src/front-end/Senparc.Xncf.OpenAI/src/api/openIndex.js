import request from "../utils/request"
// const baseUrl = ""//共用链接前缀-待定

// 获取appkey和organizationID
export function getAppkeyOrganizationID() {
    return request({
        url: "/Senparc.Xncf.OpenAI/OpenAiConfigService/Xncf.OpenAI_OpenAiConfigService.GetOpenAiConfigDtoAsync",
        method: 'get'
    })
}

// post 传值appkey-organizationid
export function postAppkeyOrganizationID(data = {}) {
    // console.log(data);
    return request({
        url: "/Senparc.Xncf.OpenAI/OpenAiConfigService/Xncf.OpenAI_OpenAiConfigService.Update",
        data,
        method: 'post'
    })
}
