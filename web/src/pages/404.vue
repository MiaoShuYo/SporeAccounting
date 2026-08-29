<script setup lang="ts">
import {router} from "../router";
</script>

<template>
  <el-container>
    <el-main>
      <img src="../assets/404.svg" alt="404" @click="router.push('/')"/>
    </el-main>
  </el-container>
</template>
<style scoped>
.el-main {
  display: flex;
  justify-content: center;
  align-items: center;
}

img {
  cursor: pointer;
  max-width: 100%;
  max-height: 90vh;
  height: auto;
}
</style>