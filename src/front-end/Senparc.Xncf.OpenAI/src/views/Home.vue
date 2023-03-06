<template>
  <div>
    <h1>{{ title }}</h1>
    <div class="li" v-for="(item) in ulli" :key="item.name">
      <span>{{ item.name }}</span>:
      <span>{{ item.path }}</span>
    </div>
  </div>
</template>
<script>
export default {
  name: "home",
  data() {
    return {
      title: "Senparc.Xncf.OpenAI",
      ulli: [
        {
          name: 'Senparc.Xncf.OpenAI首页',
          path: "/Module/XncfStoreOpenAI/openindex"
        },
        {
          name: 'Senparc.Xncf.OpenAI对话页',
          path: "/Module/XncfStoreOpenAI/opendetail"
        },
      ]
    };
  },
};
</script>
