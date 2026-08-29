<script setup lang="ts">
import {ref, defineEmits, reactive, inject} from "vue";
import {ElMessage, type FormInstance, type FormRules} from "element-plus";
import type {Register} from "../../Interface/login.ts";
import type {Response} from "../../Interface/response.ts";

const emit = defineEmits(['switch']);
const axios: any = inject('axios')
const RAW_API_BASE = (import.meta.env.VITE_API_BASE_URL as string | undefined)
const API_BASE = (!RAW_API_BASE || RAW_API_BASE === 'undefined' || RAW_API_BASE === 'null') ? '/api' : RAW_API_BASE

const ruleRegisterRef = ref<FormInstance>()
const registerData = reactive<Register>({
  userName: '',
  password: '',
  rePassword: '',
  phoneNumber: '',
  email: ''
})
const rules = reactive<FormRules<Register>>({
  userName: [
    {required: true, message: '用户名不能为空', trigger: 'blur'}
  ],
  password: [
    {required: true, message: '密码不能为空', trigger: 'blur'},
  ],
  rePassword: [
    {required: true, message: '确认密码不能为空', trigger: 'blur'},
    {
      validator: (_rule,value, callback) => {
        if (value !== registerData.password) {
          callback(new Error('两次输入密码不一致'));
        } else {
          callback();
        }
      }, trigger: 'blur'
    }
  ],
  phoneNumber: [
    {required: true, message: '手机号不能为空', trigger: 'blur'},
    {pattern: /^1[3456789]\d{9}$/, message: '手机号格式不正确', trigger: 'blur'}
  ],
  email: [
    {required: true, message: '邮箱不能为空', trigger: 'blur'},
    {type: 'email', message: '邮箱格式不正确', trigger: 'blur'}
  ]
})
const register = (formEl: FormInstance | undefined) => {
  if (!formEl) return
  formEl.validate().then(() => {
    axios.post(API_BASE + '/SysUser/Register', registerData).then((res: any) => {
      const response: Response<boolean> = res.data;
      if (response.statusCode === 200) {
        emit('switch', "login");
      } else {
        ElMessage.error(response.errorMessage)
      }
    }).catch((err: any) => {
      console.log(err);
    });
  }).catch(() => {
    return
  })
}
const toLogin = () => {
  emit('switch', "login");
}
</script>
<template>
  <div style="padding-top:10% ">
    <div style="text-align: center">
      <img src="../../assets/logo.png" style="width:200px;height: 49px; max-height: 100%;" alt="Login Image">
    </div>
    <el-form ref="ruleRegisterRef"
             :model="registerData"
             :rules="rules"
             label-position="top"
             status-icon>
      <el-form-item label="用户名" prop="userName">
        <el-input style="height:40px;line-height: 40px " v-model="registerData.userName"
                  placeholder="请输入..."></el-input>
      </el-form-item>
      <el-form-item label="密码" prop="password">
        <el-input style="height:40px;line-height: 40px " v-model="registerData.password" type="password"
                  placeholder="请输入..."></el-input>
      </el-form-item>
      <el-form-item label="确认密码" prop="rePassword">
        <el-input style="height:40px;line-height: 40px " v-model="registerData.rePassword" type="password"
                  placeholder="请输入..."></el-input>
      </el-form-item>
      <el-form-item label="手机号" prop="phoneNumber">
        <el-input style="height:40px;line-height: 40px " v-model="registerData.phoneNumber" type="text"
                  placeholder="请输入..."></el-input>
      </el-form-item>
      <el-form-item label="邮箱" prop="email">
        <el-input style="height:40px;line-height: 40px " v-model="registerData.email" type="text"
                  placeholder="请输入..."></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" style="width: 100%;height:40px;line-height: 40px " @click="register(ruleRegisterRef)">
          注册
        </el-button>
        <label>已有账号？</label>
        <el-link :underline="false" type="primary" v-on:click="toLogin">登录</el-link>
      </el-form-item>
    </el-form>
  </div>
</template>

<style scoped>

</style>