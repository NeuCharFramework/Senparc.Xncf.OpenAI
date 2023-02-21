import request from "@/utils/request"
const baseUrl = ""//共用链接前缀-待定

// 获取appkey和organizationID
export function getAppkeyOrganizationID(data = {}) {
    console.log(data);
    return request({
        url: "/Senparc.Xncf.OpenAI/OpenAiConfigService/Xncf.OpenAI_OpenAiConfigService.GetOpenAiConfigDtoAsync",
        data,
        method: 'get'
    })
}


export function postOpen(data = {}) {
    console.log(data);
    return request({
        url: "",
        data,
        method: 'post'
    })
}

export function putOpen(data = {}) {
    console.log(data);
    return request({
        url: "",
        data,
        method: 'put'
    })
}

export function deleteOpen(data = {}) {
    console.log(data);
    return request({
        url: "",
        data,
        method: 'delete'
    })
}