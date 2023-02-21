<!-- open首页 -->
<template>
    <div class="openai">
        <!-- 首页-功能描述页 -->
        <main>
            <el-container>
                <el-header class="module-header">
                    <span class="start-title">
                        <span class="module-header-v">标题标题标题</span>
                    </span>
                </el-header>
            </el-container>
            <div class="textArea">功能描述-功能介绍</div>
            <!-- 功能模块 -->
            <el-row>
                <el-card class="box-card mw-100">
                    <div class="nav" v-if="yesno == false">
                        <h3>使用该模块需验证登录 </h3>
                        <el-button el-button type=" primary" size="mini" @click="dialogVisible = true">设置Token</el-button>
                    </div>
                    <div class="nav" v-if="yesno == true">
                        <h3>请单击需要使用的模块 </h3>
                        <el-button el-button type=" primary" size="mini" @click="dialogVisible = true">更换Token</el-button>
                    </div>
                    <!-- 搜索 -->
                    <el-input class="box-ipt" v-model="modelValue" placeholder="输入内容按下Enter" clearable
                              @keyup="searchModular"></el-input>
                    <!-- 已登录展示数据  -->
                    <div id="xncf-modules-area" v-if="yesno">
                        <el-col :span="6" :xs="24" :sm="12" :md="12" :lg="8" :xl="6" v-for="item in xncfOpenAIList"
                                :key="item.uid">
                            <el-card class="moduleItem">
                                <div slot="header" class="xncf-item-top svgimg greencolor" @click="goOpenAIdetail">
                                    <span class="moudelName">{{ item.menuName }}</span>
                                    <small class="version">v{{ item.version }}</small>
                                </div>
                                <span class="detail" @click="goOpenAIdetail">描述描述描述</span>
                            </el-card>
                        </el-col>
                    </div>
                    <!-- 未登录展示数据 -->
                    <div id="xncf-modules-area-else" v-else>
                        <el-col :span="6" :xs="24" :sm="12" :md="12" :lg="8" :xl="6" v-for="item in xncfOpenAIList"
                                :key="item.uid">
                            <el-card class="moduleItem">
                                <div slot="header" class="xncf-item-top svgimg greencolor">
                                    <span class="moudelName">{{ item.menuName }}</span>
                                    <small class="version">v{{ item.version }}</small>
                                </div>
                                <div class="detail">描述描述描述描述描述</div>
                            </el-card>
                        </el-col>
                    </div>
                </el-card>
            </el-row>
        </main>
        <el-dialog title="登录" :visible.sync="dialogVisible" width="50%" :before-close="handleClose">
            <!-- 首次操作 -->
            <div class="dialogArea" v-if="passwordState == false">
                <el-input v-model="token" placeholder="请输入内容"></el-input>
                <span>这是一段信息描述,登录请输入您的Token</span>
            </div>
            <!-- 已登录，再次显示 -->
            <div class="dialogArea" v-if="passwordState == true">
                <el-input v-model="token" placeholder="请输入内容" show-password></el-input>
                <span>这是一段信息描述,若修改Token请输入</span>
            </div>
            <span slot="footer" class="dialog-footer">
                <el-button @click="dialogVisible = false">取 消</el-button>
                <el-button type="primary" @click="login">确 定</el-button>
            </span>
        </el-dialog>
    </div>
</template>
<script>
// 请求
import { getOpen, postOpen, putOpen, deleteOpen } from "@/api/openIndex"
export default {
    data() {
        return {
            modelValue: "",//输入框内容
            // 模块数据
            xncfOpenAIList: [
                {
                    menuName: "模块名称",//名称
                    version: "0.1",//版本
                },
                {
                    menuName: "模块名称",//名称
                    version: "0.1",//版本
                },
                {
                    menuName: "模块名称",//名称
                    version: "0.1",//版本
                },
                {
                    menuName: "模块名称",//名称
                    version: "0.1",//版本
                },
                {
                    menuName: "模块名称",//名称
                    version: "0.1",//版本
                },
                {
                    menuName: "模块名称",//名称
                    version: "0.1",//版本
                },
                {
                    menuName: "模块名称",//名称
                    version: "0.1",//版本
                },
                {
                    menuName: "模块名称",//名称
                    version: "0.1",//版本
                },
                {
                    menuName: "模块名称",//名称
                    version: "0.1",//版本
                },
                {
                    menuName: "模块名称",//名称
                    version: "0.1",//版本
                },
            ],
            // 模块数据搜索筛选
            xncfOpenAIsearch: [],
            yesno: false,//是否可点击
            token: '',//token
            dialogVisible: false,//弹出层状态
            passwordState: false,//输入框是否显示password
        }
    },
    created() {
        this.getToken();
        // this.getList();
    },
    methods: {
        //获取Token
        getToken() {
            // console.log('router', this.$router.currentRoute.query.Token);
            if (this.$router.currentRoute.query.Token) {
                this.token = this.$router.currentRoute.query.Token;//token获取
                this.yesno = true;//可以操作
                this.passwordState = true;//token显示password状态
            }
        },
        // 获取模块数据
        async getList() {
            const xncfOpenAIList = await getOpen();
        },
        //关闭弹出层
        handleClose(done) {
            this.$confirm('确认关闭？')
                .then(_ => {
                    done();
                })
                .catch(_ => { });
        },
        // 登录解锁
        login() {
            if (this.token != '') {
                this.yesno = true;//可操作
                this.dialogVisible = false;//关闭弹出层
                this.passwordState = true;//token显示password状态
            }
        },
        // 去到详情页
        goOpenAIdetail() {
            this.$router.push({
                path: '/OpenAI/detail',
                query: {
                    Token: this.token, //Token
                }
            })
        },
        // 搜索模块内容
        searchModular() {
            // console.log(this.xncfOpenAIsearch);
        }
    }
}
// indexOf()返回字符串中检索指定字符第一次出现的位置
</script>
<style scopen lang="scss">
// 滚动条-共用样式
::-webkit-scrollbar {
    width: 11px;
    height: 11px;
}

// 滚动条-共用样式
::-webkit-scrollbar-thumb {
    border-radius: 10px;
    background-color: #4b5563;
    background-color: #37bc9b;
    border: 2px solid transparent;
    background-clip: padding-box;
}

.openai {
    display: flex;
    flex-direction: column;
    align-items: flex-start;
    box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
    margin: 10px;
    background-color: #fff;

    main {
        width: 100%;
        min-height: 80px;
        padding: 15px;

        .nav {
            display: flex;
            align-items: center;
            justify-content: center;
            width: 100%;
            height: 40px;
            padding: 10px;
            margin-bottom: 10px;

            h3 {
                display: inline-block;
                margin: 10px;
                padding: 10px;
                white-space: nowrap;
            }
        }

        .textArea {
            width: 100%;
            max-height: 100%;
            min-height: 40px;
            padding: 10px;
            margin: 10px 0px;
            white-space: pre-wrap;
            box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
        }

        .box-ipt {
            max-width: 300px;
        }

        .clearfix {
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        ::v-deep .el-card__header {
            padding: 16px;
            font-weight: bold;
        }

        ::v-deep .el-card__body {
            padding: 16px;
        }

        // 已登录
        #xncf-modules-area {
            display: flex;
            flex-wrap: wrap;
            display: flex;

            overflow: hidden;

            .moduleItem {
                margin-top: 10px;
                cursor: pointer;
            }

            .detail {
                white-space: pre-wrap;
                font-size: 14px;
            }

            .xncf-item-top {
                display: flex;
                flex-direction: row;
                font-weight: bold !important;
                flex-wrap: nowrap;
                gap: 10px;
            }

            .moudelName {
                flex: 1;
                word-break: break-all;
            }

            .version {
                white-space: nowrap;
            }
        }

        // 未登录
        #xncf-modules-area-else {
            display: flex;
            flex-wrap: wrap;
            display: flex;
            overflow: hidden;

            .moduleItem {
                cursor: not-allowed;
                // width: 300px;
                margin: 10px 10px 0px 0px;
                background-color: #c0c4cc11;
            }

            .detail {
                white-space: pre-wrap;
                font-size: 14px;
            }

            .xncf-item-top {
                display: flex;
                flex-direction: row;
                font-weight: bold !important;
                flex-wrap: nowrap;
                gap: 10px;
            }

            .moudelName {
                flex: 1;
                word-break: break-all;
            }

            .version {
                white-space: nowrap;
            }
        }
    }

    .dialogArea {
        span {
            line-height: 40px;
        }
    }
}
</style>
<style scoped>
.main>>>.el-card__body {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
}
</style>