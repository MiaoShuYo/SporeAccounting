<script setup lang="ts">
import {ref, defineEmits, inject, reactive} from "vue";
import {type FormRules, type FormInstance, ElMessage} from "element-plus";
import type {Login,LoginResponse} from '../../Interface/login.ts'
import type {Response} from "../../Interface/response.ts";
import {router} from "../../router";

const emit = defineEmits(['switch']);
const axios: any = inject('axios')
const RAW_API_BASE = (import.meta.env.VITE_API_BASE_URL as string | undefined)
const API_BASE = (!RAW_API_BASE || RAW_API_BASE === 'undefined' || RAW_API_BASE === 'null') ? '/api' : RAW_API_BASE


const ruleLoginRef = ref<FormInstance>()
const loginData = reactive<Login>({
  username: '',
  password: ''
})
const rules = reactive<FormRules<Login>>({
  username: [
    {required: true, message: '用户名不能为空', trigger: 'blur'}
  ],
  password: [
    {required: true, message: '密码不能为空', trigger: 'blur'},
  ]
})
const login = (formEl: FormInstance | undefined) => {
  if (!formEl) return
  formEl.validate().then(() => {
    axios.get(API_BASE + '/SysUser/Login/' + loginData.username + '/' + loginData.password).then((res: any) => {
      const response: Response<LoginResponse> = res.data;
      if (response.statusCode === 200) {
        localStorage.setItem('token', response.data.token)
        localStorage.setItem('refreshToken', response.data.refreshToken)
        router.push({name: 'index'});
      } else {
        ElMessage.error(response.errorMessage)
      }
    }).catch((err: any) => {
      console.error(err);
    });
  }).catch(() => {
    return
  })
}
const toRegister = () => {
  emit('switch', "register");
}
const toRetrievePassword = () => {
  emit('switch', "retrievePassword")
}
</script>

<template>
  <div style="padding-top:10% ">
    <div style="text-align: center">
      <img src="../../assets/logo.png" style="width:200px;height: 49px; max-height: 100%;" alt="Login Image">
    </div>
    <el-form ref="ruleLoginRef"
             :model="loginData"
             :rules="rules" l
             label-position="top"
             status-icon>
      <el-form-item label="用户名" prop="username">
        <el-input style="height:40px;line-height: 40px " v-model="loginData.username"
                  placeholder="请输入..."></el-input>
      </el-form-item>
      <el-form-item label="密码" prop="password">
        <el-input style="height:40px;line-height: 40px " v-model="loginData.password" type="password"
                  placeholder="请输入..."></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" style="width: 100%;height:40px;line-height: 40px " @click="login(ruleLoginRef)">登录</el-button>
        <el-link :underline="false" style="text-align: left; width: 50%;display: block;margin-top: 10px;" type="primary"
                 v-on:click="toRetrievePassword">忘记密码
        </el-link>
        <el-link :underline="false" style="text-align: right; width: 50%;display: block;margin-top: 10px" type="primary"
                 v-on:click="toRegister">注册
        </el-link>
      </el-form-item>
    </el-form>
  </div>
</template>

<style scoped>
.el-form-item {
  margin-bottom: 35px;
}

:deep(.el-form-item__label) {
  font-size: 16px;
}
</style>