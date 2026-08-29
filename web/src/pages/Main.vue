<script setup lang="ts">
import {ref, reactive, inject, watch} from 'vue'
import {useRoute} from 'vue-router'
import type {ResetPassword, SecuritySetting, PersonalInformation} from '../Interface/personalCenter.ts'
import {type FormInstance, type FormRules, ElMessage} from "element-plus";
import type {Response} from '../Interface/response.ts'
import {router, navigateTo} from "../router";

const route = useRoute()
const RAW_API_BASE = (import.meta.env.VITE_API_BASE_URL as string | undefined)
const API_BASE = (!RAW_API_BASE || RAW_API_BASE === 'undefined' || RAW_API_BASE === 'null') ? '/api' : RAW_API_BASE

// 根据当前路由计算激活的菜单项
const activeMenu = ref('1')

// 监听路由变化，更新菜单高亮
const updateActiveMenu = () => {
  const routeName = route.name as string
  switch (routeName) {
    case 'index':
      activeMenu.value = '1'
      break
    case 'accountBook':
      activeMenu.value = '2'
      break
    case 'report':
      activeMenu.value = '3'
      break
    case 'incomeExpenditureType':
      activeMenu.value = '4-1'
      break
    case 'budget':
      activeMenu.value = '4-2'
      break
    case 'primaryCurrency':
      activeMenu.value = '4-3'
      break
    default:
      activeMenu.value = '1'
  }
}

// 监听路由变化
watch(() => route.name, updateActiveMenu, { immediate: true })

const resetPasswordFormVisible = ref(false)
const resetPasswordRef = ref<FormInstance>()
const resetPasswordRules = reactive<FormRules<ResetPassword>>({
  oldPassword: [
    {required: true, message: '旧密码不能为空', trigger: 'blur'}
  ],
  newPassword: [
    {required: true, message: '新密码不能为空', trigger: 'blur'},
  ],
  reNewPassword: [
    {required: true, message: '确认密码不能为空', trigger: 'blur'},
    {
      validator: (_, value, callback) => {
        if (value !== resetPasswordData.newPassword) {
          callback(new Error('两次输入密码不一致'));
        } else {
          callback();
        }
      }, trigger: 'blur'
    }
  ]
})
const resetPasswordData = reactive<ResetPassword>({
  oldPassword: '',
  newPassword: '',
  reNewPassword: ''
})

const securitySettingFormVisible = ref(false)
const securitySettingRef = ref<FormInstance>()
const securitySettingRules = reactive<FormRules<SecuritySetting>>({
  phoneNumber: [
    {required: true, message: '手机不能为空', trigger: 'blur'},
    {pattern: /^1[3456789]\d{9}$/, message: '手机号格式不正确', trigger: 'blur'}
  ],
  email: [
    {required: true, message: '手机号不能为空', trigger: 'blur'},
    {type: 'email', message: '邮箱格式不正确', trigger: 'blur'}
  ]
})
const securitySettingData = reactive<SecuritySetting>({
  phoneNumber: '',
  email: ''
})

const personalInformationFormVisible = ref(false)
const personalInformationRef = ref<FormInstance>()
const personalInformationRules = reactive<FormRules<PersonalInformation>>({
  userName: [
    {required: true, message: '用户名不能为空', trigger: 'blur'}
  ]
})
const personalInformationData = reactive<PersonalInformation>({
  userName: '',
})

const axios: any = inject('axios')


/*重置密码*/
const resetPassword = (formEl: FormInstance | undefined) => {
  if (!formEl) return
  formEl.validate().then(() => {
    axios.put(API_BASE + '/SysUser/ResetPassword',
        resetPasswordData, {
          headers: {
            'Authorization': localStorage.getItem('token')
          }
        }).then((res: any) => {
      const response: Response<boolean> = res.data;
      if (response.statusCode === 200) {
        ElMessage.success('修改成功')
        localStorage.removeItem('token')
        localStorage.removeItem('refreshToken')
        resetPasswordFormVisible.value = false
        router.push({name: 'login'});
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
/*安全设置*/
const securitySetting = (formEl: FormInstance | undefined) => {
  if (!formEl) return
  formEl.validate().then(() => {
    axios.put(API_BASE + '/SysUser/EditUserSecurity',
        securitySettingData, {
          headers: {
            'Authorization': localStorage.getItem('token')
          }
        }).then((res: any) => {
      const response: Response<boolean> = res.data;
      if (response.statusCode === 200) {
        ElMessage.success('修改成功')
        securitySettingFormVisible.value = false
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
/*修改用户名*/
const personalInformation = (formEl: FormInstance | undefined) => {
  if (!formEl) return
  formEl.validate().then(() => {
    axios.put(API_BASE + '/SysUser/EditUserName',
        personalInformationData, {
          headers: {
            'Authorization': localStorage.getItem('token')
          }
        }).then((res: any) => {
      const response: Response<boolean> = res.data;
      if (response.statusCode === 200) {
        ElMessage.success('修改成功')
        personalInformationFormVisible.value = false
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
/*注销账户*/
const deleteUser = () => {
  axios.delete(API_BASE + '/SysUser/LogoutUser', {
    headers: {
      'Authorization': localStorage.getItem('token')
    }
  }).then((res: any) => {
    const response: Response<boolean> = res.data;
    if (response.statusCode === 200) {
      ElMessage.success('注销成功')
      localStorage.removeItem('token')
      localStorage.removeItem('refreshToken')
      router.push({name: 'login'});
    } else {
      ElMessage.error(response.errorMessage)
    }
  }).catch((err: any) => {
    console.log(err);
  });
}
/*退出*/
const logout = () => {
  localStorage.removeItem('token')
  localStorage.removeItem('refreshToken')
  router.push({name: 'login'});
}
</script>

<template>
  <div class="main">
    <el-container>
      <el-header>
        <el-menu mode="horizontal" :ellipsis="false">
          <el-menu-item>
            <img
                style="width: 180px"
                src="/src/assets/logo.png"
                alt="Element logo"
            />
          </el-menu-item>
          <el-sub-menu>
            <template #title>
              <el-avatar :size="40"/>
            </template>
            <el-menu-item @click="resetPasswordFormVisible=true">重置密码</el-menu-item>
            <el-menu-item @click="securitySettingFormVisible=true">安全设置</el-menu-item>
            <el-menu-item @click="personalInformationFormVisible=true">修改用户名</el-menu-item>
            <el-menu-item @click="deleteUser">注销账户</el-menu-item>
            <el-menu-item @click="logout">退出</el-menu-item>
          </el-sub-menu>
        </el-menu>
      </el-header>
      <el-container>
        <el-aside width="200px">
          <el-menu
              :default-active="activeMenu"
              id="mainMenu"
          >
            <el-menu-item index="1" @click="navigateTo('index')">
              <el-icon>
                <location/>
              </el-icon>
              <template #title>首页</template>
            </el-menu-item>
            <el-menu-item index="2" @click="navigateTo('accountBook')">
              <el-icon>
                <Collection/>
              </el-icon>
              <template #title>账本</template>
            </el-menu-item>
            <el-menu-item index="3" @click="navigateTo('report')">
              <el-icon>
                <PieChart/>
              </el-icon>
              <template #title>报表</template>
            </el-menu-item>
            <el-sub-menu index="4">
              <template #title>
                <el-icon>
                  <setting/>
                </el-icon>
                <span>配置</span>
              </template>
              <el-menu-item-group>
                <el-menu-item index="4-1" @click="navigateTo('incomeExpenditureType')">收支分类</el-menu-item>
                <el-menu-item index="4-2" @click="navigateTo('budget')">预算设置</el-menu-item>
                <el-menu-item index="4-3" @click="navigateTo('primaryCurrency')">主币种设置</el-menu-item>
              </el-menu-item-group>
            </el-sub-menu>
          </el-menu>
        </el-aside>
        <el-main>
          <router-view></router-view>
        </el-main>
      </el-container>
    </el-container>
  </div>
  <!--重置密码弹窗-->
  <el-dialog center v-model="resetPasswordFormVisible" title="重置密码" width="500">
    <template #header="{titleId, titleClass }">
      <div>
        <span :id="titleId" :class="titleClass">重置密码</span>
      </div>
    </template>
    <el-form
        :model="resetPasswordData"
        label-position="top"
        ref="resetPasswordRef"
        :rules="resetPasswordRules"
        status-icon>
      <el-form-item label="旧密码" prop="oldPassword">
        <el-input type="password" v-model="resetPasswordData.oldPassword"/>
      </el-form-item>
      <el-form-item label="新密码" prop="newPassword">
        <el-input type="password" v-model="resetPasswordData.newPassword"/>
      </el-form-item>
      <el-form-item label="确认密码" prop="reNewPassword">
        <el-input type="password" v-model="resetPasswordData.reNewPassword"/>
      </el-form-item>
    </el-form>
    <template #footer>
      <div class="dialog-footer">
        <el-button style="width: 100%" size="large" type="primary" @click="resetPassword(resetPasswordRef)">
          确定
        </el-button>
      </div>
    </template>
  </el-dialog>
  <!--安全设置弹窗-->
  <el-dialog center v-model="securitySettingFormVisible" title="安全设置" width="500">
    <template #header="{titleId, titleClass }">
      <div>
        <span :id="titleId" :class="titleClass">安全设置</span>
      </div>
    </template>
    <el-form
        :model="securitySettingData"
        label-position="top"
        ref="securitySettingRef"
        :rules="securitySettingRules"
        status-icon>
      <el-form-item label="邮箱" prop="email">
        <el-input v-model="securitySettingData.email"/>
      </el-form-item>
      <el-form-item label="手机号" prop="phoneNumber">
        <el-input v-model="securitySettingData.phoneNumber"/>
      </el-form-item>
    </el-form>
    <template #footer>
      <div class="dialog-footer">
        <el-button style="width: 100%" size="large" type="primary" @click="securitySetting(securitySettingRef)">
          确定
        </el-button>
      </div>
    </template>
  </el-dialog>
  <!--修改用户名弹窗-->
  <el-dialog center v-model="personalInformationFormVisible" title="修改用户名" width="500">
    <template #header="{titleId, titleClass }">
      <div>
        <span :id="titleId" :class="titleClass">修改用户名</span>
      </div>
    </template>
    <el-form
        :model="personalInformationData"
        label-position="top"
        ref="personalInformationRef"
        :rules="personalInformationRules"
        status-icon>
      <el-form-item label="用户名" prop="userName">
        <el-input v-model="personalInformationData.userName"/>
      </el-form-item>
    </el-form>
    <template #footer>
      <div class="dialog-footer">
        <el-button style="width: 100%" size="large" type="primary" @click="personalInformation(personalInformationRef)">
          确定
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<style scoped>
.el-container, .main {
  padding: 0;
  margin: 0;
  height: 100%;
}

.el-menu--horizontal > .el-menu-item:nth-child(1) {
  margin-right: auto;
}

.el-menu-item:not(.is-disabled):hover {
  background-color: white;
}

.el-header {
  padding: 0;
}

.el-main {
  padding-top: 10px;
}

#mainMenu {
  height: 100%;
}
</style>