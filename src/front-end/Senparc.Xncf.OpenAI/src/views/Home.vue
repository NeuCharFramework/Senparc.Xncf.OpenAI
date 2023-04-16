<template>
  <div>
    <h1>{{ title }}</h1>
    <div class="li" @click="goOpenAImian">
      <span>{{ ulli.mianname }}</span>:
      <span>{{ ulli.mianpath }}</span>
    </div>
    <div class="li" @click="goOpenAIdetail">
      <span>{{ ulli.detailname }}</span>:
      <span>{{ ulli.detailpath }}</span>
    </div>
  </div>
</template>
<script>
export default {
  name: "home",
  data() {
    return {
      title: "Senparc.Xncf.OpenAI",
      ulli: {
        mianname: 'Senparc.Xncf.OpenAI首页',
        mianpath: "/Module/SenparcXncf/openindex",
        detailname: 'Senparc.Xncf.OpenAI对话页',
        detailpath: "/Module/SenparcXncf/opendetail",
      },
    };
  },
  methods: {
    // 回到首页
    goOpenAImian() {
      this.$router.push({
        path: '/Module/SenparcXncf/openindex',
      })
    },
    // 去到详情页
    goOpenAIdetail() {
      this.$router.push({
        path: '/Module/SenparcXncf/opendetail',
      })
    },
  },
};
</script>
<style scoped>
.li {
  cursor: pointer;
}
</style>