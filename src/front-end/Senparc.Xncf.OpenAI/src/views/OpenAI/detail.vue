<!-- open详情页 -->
<template>
    <div class="aisearch">
        <div class="showText">
            <!-- 空状态 -->
            <div v-if="this.showState == false">
                <el-empty description="请在下方输入框中输入您想要了解的内容"></el-empty>
            </div>
            <!-- 请求数据区域 -->
            <div v-else>
                <main class="mian" v-for="item in dataState" :key="item.id">
                    <!-- 提问 -->
                    <div class="messageList">
                        <div class="messageListitem">
                            <div class="headPortrait"></div>
                            <div class="QA">
                                {{ item.quesition }}
                            </div>
                        </div>
                    </div>
                    <!-- 正常回复 -->
                    <div class="messageList1" v-if="item.anserState == 1">
                        <div class="messageList1item">
                            <div class="headPortrait"></div>
                            <div class="QA">
                                盛派网络是一家中国的技术公司，主要提供微信公众号，小程序等微信生态系统相关的开发等等......
                            </div>
                        </div>
                    </div>
                    <!-- 回复报错 -->
                    <div class="messageList2" v-if="item.anserState == 2">
                        <div class="messageList2item">
                            <div class="headPortrait"></div>
                            <div class="QA">
                                盛派网络是一家中国的技术公司，主要提供微信公众号，小程序等微信生态系统相关的开发等等......
                            </div>
                        </div>
                    </div>
                </main>
            </div>
        </div>
        <!-- 输入框区域 -->
        <div class="quesitionArea">
            <div class="iptArea">
                <el-input placeholder="请输入内容" v-model="input">
                    <el-button slot="append" icon="el-icon-position" @click="findContent"></el-button>
                </el-input>
                <el-button type="primary" @click="goOpenAImian">返回</el-button>
            </div>
            <div class="iptArea" style="font-size: 14px;">您可以在这里输入想要查找的内容</div>
        </div>
    </div>
</template>
<script>
// 请求
import { getOpen, postOpen, putOpen, deleteOpen } from "@/api/openIndex"
export default {
    data() {
        return {
            input: "",
            token: null,
            idNum: 1,
            firstQuestion: "",//问题
            dataState: [],//数据
            showState: false,//展示状态
        }
    },
    created() {
        this.getToken();
    },
    methods: {
        // 获取路由传值参数
        getToken() {
            if (this.$router.currentRoute.query.Token) {
                this.token = this.$router.currentRoute.query.Token;
            }
        },
        // 回到首页
        goOpenAImian() {
            this.$router.push({
                path: '/OpenAI/index',
                query: {
                    Token: this.token,//Token
                }
            })
        },
        // 查找内容
        findContent() {
            this.showState = true;
            this.idNum = this.idNum + 1;
            // 模拟请求到的数据
            this.dataState.push({
                id: this.idNum,
                quesition: this.input,
                anserData: "没有可回复的内容",
                anserState: "2",
            })
            // 清空
            this.input = null;
        }
    },
}
</script>

<style scoped lang="scss">
// 滚动条-公共样式
::-webkit-scrollbar {
    width: 11px;
    height: 11px;
}

// 滚动条-公共样式
::-webkit-scrollbar-thumb {
    border-radius: 10px;
    background-color: #4b5563;
    background-color: #37bc9b;
    border: 2px solid transparent;
    background-clip: padding-box;
}

.aisearch {
    .showText {
        height: 71vh;
        width: 100%;
        background-color: #fff;
        overflow: hidden;
        overflow-y: auto;
        scrollbar-width: none;
        /* firefox */
        -ms-overflow-style: none;

        .mian {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: flex-start;
            padding: 2px 15px;
            background-color: #fff;
            // height: 80vh;
            overflow: hidden;
            overflow: auto;

            .messageList {
                padding: 20px 5px;
                background-color: #fff;
                border-top: 1px solid rgba(0, 0, 0, 0.31);
                border-bottom: 1px solid rgba(0, 0, 0, 0.20);

                .messageListitem {
                    width: 70%;
                    display: flex;
                    align-items: flex-start;
                    justify-content: flex-start;

                    .QA {
                        padding: 10px;
                        white-space: pre-wrap;
                        font-size: 14px;
                    }
                }

            }

            .messageList1 {

                padding: 20px 5px;
                background-color: rgba(217, 217, 227, .8);
                border-bottom: 1px solid rgba(0, 0, 0, 0.31);

                .messageList1item {
                    width: 70%;
                    display: flex;
                    align-items: flex-start;
                    justify-content: flex-start;

                    .QA {
                        padding: 10px;
                        white-space: pre-wrap;
                        font-size: 14px;
                    }
                }
            }

            .messageList2 {

                padding: 20px 5px;
                background-color: rgba(217, 217, 227, .8);
                border-bottom: 1px solid rgba(0, 0, 0, 0.31);

                .messageList2item {
                    width: 70%;
                    display: flex;
                    align-items: flex-start;
                    justify-content: flex-start;

                    .QA {
                        padding: 10px;
                        white-space: pre-wrap;
                        font-size: 14px;
                        background-color: rgba(239, 68, 68, .1);
                        border-radius: 4px;
                        border: 1px solid rgba(239, 68, 68, 1);
                    }
                }
            }

            .messageList,
            .messageList1,
            .messageList2 {
                display: flex;
                align-items: center;
                justify-content: center;
                border-color: rgba(0, 0, 0, .1);
                width: 100%;

                .headPortrait {
                    width: 30px;
                    height: 30px;
                    border-radius: 4px;
                    background-color: aqua;
                    margin-right: 10px;
                }
            }
        }
    }

    .quesitionArea {
        height: 20vh;
        background-color: #fff;
        box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;


        .iptArea {
            display: flex;
            align-items: center;
            justify-content: center;
            width: 100%;
            padding: 15px;
            margin: 10px;

            ::v-deep .el-input {
                max-width: 800px;
                margin-right: 10px;
            }
        }
    }
}
</style>
<style scoped>
.el-empty {
    height: 70vh;
}
</style>
