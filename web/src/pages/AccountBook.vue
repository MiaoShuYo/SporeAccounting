<script setup lang="ts">
import { ref, onMounted, inject } from 'vue'
import type { AccountBook, CreateAccountBookRequest, UpdateAccountBookRequest } from '../Interface/accountBook.ts'
import type { Response } from '../Interface/response.ts'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Edit, Delete, Setting } from '@element-plus/icons-vue'
import { useRouter } from 'vue-router'

const axios: any = inject('axios')
const RAW_API_BASE = (import.meta.env.VITE_API_BASE_URL as string | undefined)
const API_BASE = (!RAW_API_BASE || RAW_API_BASE === 'undefined' || RAW_API_BASE === 'null') ? '/api' : RAW_API_BASE
const router = useRouter()

// 账本列表
const accountBooks = ref<AccountBook[]>([])
const loading = ref(false)

// 创建账本相关
const createDialogVisible = ref(false)
const createForm = ref<CreateAccountBookRequest>({
  name: '',
  remarks: ''
})

// 编辑账本相关
const editDialogVisible = ref(false)
const editForm = ref<UpdateAccountBookRequest>({
  accountBookId: '',
  name: '',
  remarks: ''
})

// 表单验证规则
const formRules = {
  name: [
    { required: true, message: '请输入账本名称', trigger: 'blur' },
    { max: 20, message: '账本名称不能超过20个字符', trigger: 'blur' }
  ],
  remarks: [
    { max: 100, message: '账本描述不能超过100个字符', trigger: 'blur' }
  ]
}

// 表单引用
const createFormRef = ref()
const editFormRef = ref()

// 获取账本列表
const getAccountBooks = async () => {
  loading.value = true
  try {
    const response = await axios.post(API_BASE + '/AccountBook/Query',
      {
        data: {
          pageIndex: 1,
          pageSize: 100
        }
      },
      {
        headers: {
          'Authorization': localStorage.getItem('token'),
          'Content-Type': 'application/json'
        }
      })
    const result: Response<AccountBook[]> = response.data
    if (result.statusCode === 200) {
      accountBooks.value = result.data.data || []
    } else {
      ElMessage.error(result.errorMessage || '获取账本列表失败')
    }
  } catch (error) {
    ElMessage.error('获取账本列表失败')
  } finally {
    loading.value = false
  }
}

// 创建账本
const createAccountBook = async () => {
  try {
    // 表单验证
    await createFormRef.value.validate()

    const response = await axios.post(API_BASE + '/AccountBook/Add',
      createForm.value, {
      headers: {
        'Authorization': localStorage.getItem('token')
      }
    }
    )
    const result: Response<boolean> = response.data
    if (result.statusCode === 200) {
      ElMessage.success('创建账本成功')
      createDialogVisible.value = false
      // 重置表单
      createForm.value = {
        name: '',
        remarks: ''
      }
      // 重新获取列表
      await getAccountBooks()
    } else {
      ElMessage.error(result.errorMessage || '创建账本失败')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('创建账本失败:', error)
      ElMessage.error('创建账本失败')
    }
  }
}

// 删除账本
const deleteAccountBook = async (id: string, name: string) => {
  try {
    await ElMessageBox.confirm(`确定要删除账本"${name}"吗？`, '确认删除', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    const response = await axios.delete(API_BASE + `/AccountBook/Delete/${id}`, {
      headers: {
        'Authorization': localStorage.getItem('token')
      }
    })
    const result: Response<boolean> = response.data

    if (result.statusCode === 200) {
      ElMessage.success('删除账本成功')
      await getAccountBooks()
    } else {
      ElMessage.error(result.errorMessage || '删除账本失败')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('删除账本失败:', error)
      ElMessage.error('删除账本失败')
    }
  }
}

// 关闭创建对话框
const closeCreateDialog = () => {
  createDialogVisible.value = false
  // 重置表单验证状态
  createFormRef.value?.resetFields()
}

// 打开编辑对话框
const openEditDialog = (book: AccountBook) => {
  editForm.value = {
    accountBookId: book.accountBookId,
    name: book.name,
    remarks: book.remarks || ''
  }
  editDialogVisible.value = true
}

// 更新账本
const updateAccountBook = async () => {
  try {
    // 表单验证
    await editFormRef.value.validate()

    const response = await axios.put(API_BASE + '/AccountBook/Update',
      editForm.value, {
      headers: {
        'Authorization': localStorage.getItem('token')
      }
    }
    )
    const result: Response<boolean> = response.data
    if (result.statusCode === 200) {
      ElMessage.success('更新账本成功')
      editDialogVisible.value = false
      // 重新获取列表
      await getAccountBooks()
    } else {
      ElMessage.error(result.errorMessage || '更新账本失败')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('更新账本失败:', error)
      ElMessage.error('更新账本失败')
    }
  }
}

// 关闭编辑对话框
const closeEditDialog = () => {
  editDialogVisible.value = false
  // 重置表单验证状态
  editFormRef.value?.resetFields()
}

// 跳转到记账页面
const goToRecord = (accountBookId: string) => {
  router.push({ name: 'record', params: { accountBookId } })
}

onMounted(() => {
  getAccountBooks()
})
</script>

<template>
  <!--面包屑导航-->
  <el-breadcrumb separator="/">
    <el-breadcrumb-item :to="{ path: '/' }">首页</el-breadcrumb-item>
    <el-breadcrumb-item><span style="font-weight: bold">账本</span></el-breadcrumb-item>
  </el-breadcrumb>
  <div class="account-book-page">
    <!-- 页面标题和操作按钮 -->
    <div class="page-header">
      <p></p>
      <el-button type="primary" @click="createDialogVisible = true" :icon="Plus">
        新建账本
      </el-button>
    </div>

    <!-- 账本列表 -->
    <div class="account-book-list" v-loading="loading">
      <el-row :gutter="20">
        <el-col v-for="(book) in accountBooks" :key="book.accountBookId" :xs="24" :sm="12" :md="8" :lg="6" :xl="4"
          class="book-item-col">
          <div class="book-item" @click="goToRecord(book.accountBookId)">
            <div class="book-header">
              <div class="book-icon">
                <el-icon size="24">
                  <Setting />
                </el-icon>
              </div>
            </div>
            <div class="book-info">
              <h3 class="book-name">{{ book.name }}</h3>
              <p class="book-description" v-if="book.remarks">{{ book.remarks }}</p>
              <div class="book-meta">
                <span class="book-balance">{{ book.balance }}</span>
              </div>
            </div>
            <div class="book-actions" @click.stop>
              <el-button type="primary" size="small" circle :icon="Edit" title="编辑" @click="openEditDialog(book)" />
              <el-button type="danger" size="small" circle :icon="Delete" title="删除"
                @click="deleteAccountBook(book.accountBookId, book.name)" />
            </div>
          </div>
        </el-col>
      </el-row>

      <!-- 空状态 -->
      <div v-if="!loading && accountBooks.length === 0" class="empty-state">
        <el-empty description="暂无账本，点击上方按钮创建第一个账本">
          <el-button type="primary" @click="createDialogVisible = true">创建账本</el-button>
        </el-empty>
      </div>
    </div>

    <!-- 创建账本对话框 -->
    <el-dialog v-model="createDialogVisible" title="新建账本" width="500px" center>
      <el-form :model="createForm" label-width="80px" ref="createFormRef" :rules="formRules">
        <el-form-item label="账本名称" prop="name" required>
          <el-input v-model="createForm.name" placeholder="请输入账本名称" />
        </el-form-item>

        <el-form-item label="账本描述" prop="remarks">
          <el-input v-model="createForm.remarks" type="textarea" placeholder="请输入账本描述（可选）" :rows="3" />
        </el-form-item>
      </el-form>

      <template #footer>
        <div class="dialog-footer">
          <el-button @click="closeCreateDialog">取消</el-button>
          <el-button type="primary" @click="createAccountBook">确定</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 编辑账本对话框 -->
    <el-dialog v-model="editDialogVisible" title="编辑账本" width="500px" center>
      <el-form :model="editForm" label-width="80px" ref="editFormRef" :rules="formRules">
        <el-form-item label="账本名称" prop="name" required>
          <el-input v-model="editForm.name" placeholder="请输入账本名称" />
        </el-form-item>

        <el-form-item label="账本描述" prop="remarks">
          <el-input v-model="editForm.remarks" type="textarea" placeholder="请输入账本描述（可选）" :rows="3" />
        </el-form-item>
      </el-form>

      <template #footer>
        <div class="dialog-footer">
          <el-button @click="closeEditDialog">取消</el-button>
          <el-button type="primary" @click="updateAccountBook">确定</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped>
.account-book-page {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.page-header h2 {
  margin: 0;
  color: #303133;
}

.account-book-list {
  min-height: 400px;
}

.book-item-col {
  margin-bottom: 20px;
}

.book-item {
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  transition: all 0.3s ease;
  cursor: pointer;
}

.book-item:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.book-header {
  position: relative;
  height: 120px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 12px 12px 0 0;
}

.book-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 60px;
  height: 60px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
  color: white;
}

.book-icon .el-icon {
  color: white;
  font-size: 24px;
}

.book-info {
  padding: 16px;
}

.book-name {
  margin: 0 0 8px 0;
  font-size: 16px;
  font-weight: 600;
  color: #303133;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.book-description {
  margin: 0 0 12px 0;
  font-size: 14px;
  color: #606266;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.book-meta {
  display: flex;
  justify-content: flex-end;
  align-items: center;
}

.book-balance {
  font-size: 16px;
  font-weight: 600;
  color: #67c23a;
}

.book-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0px;
  transform: translateY(-10px) translateX(-10px);
}

.empty-state {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 400px;
}

.dialog-footer {
  text-align: right;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    gap: 16px;
    align-items: stretch;
  }

  .page-header h2 {
    text-align: center;
  }
}
</style>