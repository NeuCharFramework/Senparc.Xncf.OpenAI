import request from "@/utils/request"
const baseUrl = ""//待定共用


export function getOpen(data = {}) {
    console.log(data);
    return request({
        url: "",
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