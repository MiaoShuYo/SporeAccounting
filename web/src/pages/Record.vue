<template>
  <el-breadcrumb separator="/">
    <el-breadcrumb-item :to="{path:'/'}">首页</el-breadcrumb-item>
    <el-breadcrumb-item :to="{path:'/accountBook'}">账本</el-breadcrumb-item>
    <el-breadcrumb-item><span style="font-weight: bold">记账</span></el-breadcrumb-item>
  </el-breadcrumb>

  <!-- 搜索功能区域 -->
  <div class="container">
    <el-row :gutter="20">
      <el-col :span="4">
        <el-text>开始日期：</el-text>
        <el-date-picker v-model="recordPage.startDate" type="date" value-format="YYYY-MM-DD"
                        style="width: 65%"></el-date-picker>
      </el-col>
      <el-col :span="4">
        <el-text>结束日期：</el-text>
        <el-date-picker v-model="recordPage.endDate" type="date" value-format="YYYY-MM-DD"
                        style="width: 65%"></el-date-picker>
      </el-col>
      <el-col :span="6">
        <el-button type="primary" @click="queryRecords">搜索</el-button>
        <el-button type="success" @click="openAddDialog">新增账目</el-button>
      </el-col>
    </el-row>
  </div>

  <div>
    <!-- 账目列表 -->
    <el-table :data="recordPageResponse.data" style="width: 100%">
      <el-table-column type="index" width="100" label="编号"></el-table-column>
      <el-table-column prop="recordDate" label="日期" width="120">
        <template #default="scope">
          {{ new Date(scope.row.recordDate).toLocaleDateString() }}
        </template>
      </el-table-column>
      <el-table-column prop="incomeExpenditureClassificationName" label="分类" width="120"/>
      <el-table-column prop="afterAmount" label="金额" width="100"/>
      <el-table-column prop="currencyName" label="货币" width="80"/>
      <el-table-column prop="remark" label="备注"/>
      <el-table-column label="操作" width="180">
        <template #default="scope">
          <el-button link type="warning" size="small" @click="openEditDialog(scope.row)">编辑</el-button>
          <el-button link size="small" type="danger" @click="deleteRecord(scope.row.incomeExpenditureRecordId)">删除
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 分页组件 -->
    <el-pagination
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
        :current-page="currentPage"
        :page-sizes="[10, 20, 30, 40]"
        :page-size="pageSize"
        layout="total, sizes, prev, pager, next, jumper"
        :total="recordPageResponse.rowCount"
        style="margin-top: 20px">
    </el-pagination>
  </div>

  <!-- 新增/编辑弹窗 -->
  <el-dialog :title="dialogTitle" v-model="dialogVisible" width="400">
    <el-form :model="form" :rules="rules" ref="formRef" label-width="80px">
      <el-form-item label="收支分类" prop="classificationType">
        <el-radio-group v-model="form.classificationType" @change="handleClassificationTypeChange">
          <el-radio :value="0">收入</el-radio>
          <el-radio :value="1">支出</el-radio>
          <el-radio :value="-1">其他</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item label="收支类型" prop="incomeExpenditureClassificationId">
        <el-select v-model="form.incomeExpenditureClassificationId" placeholder="请选择收支类型">
          <el-option
              v-for="item in incomeExpenditureClassifications"
              :key="item.id"
              :label="item.name"
              :value="item.id">
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="金额" prop="amount">
        <el-input v-model.number="form.amount" type="number" min="0"/>
      </el-form-item>
      <el-form-item label="货币" prop="currencyId">
        <el-select v-model="form.currencyId" placeholder="请选择货币">
          <el-option
              v-for="item in currencies"
              :key="item.id"
              :label="item.name"
              :value="item.id">
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="备注" prop="remark">
        <el-input v-model="form.remark" maxlength="100" show-word-limit/>
      </el-form-item>
      <el-form-item label="日期" prop="recordDate">
        <el-date-picker v-model="form.recordDate" type="date" value-format="YYYY-MM-DD"/>
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="dialogVisible = false">取 消</el-button>
      <el-button type="primary" @click="submitForm(formRef)">确 定</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import {ref, onMounted, reactive, inject} from 'vue'
import {useRoute} from 'vue-router'
import {ElMessage, ElMessageBox, type FormInstance, type FormRules} from 'element-plus'
import type {PageResponse} from '../Interface/response.ts'
import type {RecordPage, RecordItem,RecordRequest} from '../Interface/record.ts'
import type {IncomeExpenditureClassification} from '../Interface/IncomeExpenditureClassification.ts'
import type {Currency} from '../Interface/currency.ts'



const axios: any = inject('axios')
const RAW_API_BASE = (import.meta.env.VITE_API_BASE_URL as string | undefined)
const API_BASE = (!RAW_API_BASE || RAW_API_BASE === 'undefined' || RAW_API_BASE === 'null') ? '/api' : RAW_API_BASE
const route = useRoute()
const accountBookId = ref<string>(route.params.accountBookId as string)

// 搜索参数
const recordPage = ref<RecordPage>({
  pageNumber: 1,
  pageSize: 10,
  startDate: '',
  endDate: '',
  accountBookId: accountBookId.value
})

// 分页响应数据
const recordPageResponse: PageResponse<RecordItem> = reactive({
  data: [],
  rowCount: 0,
  pageCount: 0
})

// 分页相关
const pageSize = ref(10)
const currentPage = ref(1)

// 弹窗相关
const dialogVisible = ref(false)
const dialogTitle = ref('新增账目')
const formRef = ref<FormInstance>()
const form = reactive<RecordRequest>({
  id: '',
  amount: 0,
  classificationType: 0,
  incomeExpenditureClassificationId: '',
  accountBookId: accountBookId.value,
  recordDate: '',
  currencyId: '',
  remark: ''
})
let editId: string | null = null

// 收支分类数据
const incomeExpenditureClassifications = ref<IncomeExpenditureClassification[]>([])

// 货币数据
const currencies = ref<Currency[]>([])

// 表单验证规则
const rules = reactive<FormRules>({
  classificationType: [
    {required: true, message: '请选择收支分类', trigger: 'change'}
  ],
  incomeExpenditureClassificationId: [
    {required: true, message: '请选择收支类型', trigger: 'change'}
  ],
  amount: [
    {required: true, message: '请输入金额', trigger: 'blur'},
    {type: 'number', min: 0, message: '金额必须大于0', trigger: 'blur'}
  ],
  accountBookId: [
    {required: true, message: '账本ID不能为空', trigger: 'change'}
  ],
  recordDate: [
    {required: true, message: '请选择日期', trigger: 'change'}
  ],
  currencyId: [
    {required: true, message: '请选择货币', trigger: 'change'}
  ],
  remark: [
    {max: 100, message: '备注不能超过100字', trigger: 'blur'}
  ]
})

// 根据类型查询收支分类
const queryIncomeExpenditureClassifications = (type: number) => {
  axios.get(API_BASE + `/IncomeExpenditureClassification/QueryByType/${type}`, {
    headers: {
      Authorization: localStorage.getItem('token')
    }
  }).then((res: any) => {
    const response = res.data
    if (response.statusCode === 200) {
      incomeExpenditureClassifications.value = response.data
    } else {
      ElMessage.error(response.errorMessage)
    }
  }).catch((err: any) => {
    ElMessage.error(err.message)
  })
}

// 查询货币列表
const queryCurrencies = () => {
  axios.get(API_BASE + '/Currency/QueryAll', {
    headers: {
      Authorization: localStorage.getItem('token')
    }
  }).then((res: any) => {
    const response = res.data
    if (response.statusCode === 200) {
      currencies.value = response.data
    } else {
      ElMessage.error(response.errorMessage)
    }
  }).catch((err: any) => {
    ElMessage.error(err.message)
  })
}

// 收支分类变化处理
const handleClassificationTypeChange = (value: number) => {
  form.incomeExpenditureClassificationId = '' // 重置选择的分类
  queryIncomeExpenditureClassifications(value)
}

// 分页查询账目
const queryRecords = () => {
  recordPage.value.pageNumber = currentPage.value
  recordPage.value.pageSize = pageSize.value
  recordPage.value.accountBookId = accountBookId.value

  // 构建查询参数，只包含有值的日期字段
  const queryParams: any = {
    pageNumber: recordPage.value.pageNumber,
    pageSize: recordPage.value.pageSize,
    accountBookId: recordPage.value.accountBookId
  }

  // 只有当日期不为空时才添加到查询参数中
  if (recordPage.value.startDate) {
    queryParams.startDate = recordPage.value.startDate + 'T00:00:00.000Z'
  }
  if (recordPage.value.endDate) {
    queryParams.endDate = recordPage.value.endDate + 'T23:59:59.999Z'
  }

  axios.post(API_BASE + '/IncomeExpenditureRecord/Query', queryParams, {
    headers: {
      Authorization: localStorage.getItem('token')
    }
  }).then((res: any) => {
    const response = res.data
    if (response.statusCode === 200) {
      recordPageResponse.pageCount = response.data.pageCount
      recordPageResponse.rowCount = response.data.rowCount
      recordPageResponse.data = response.data.data
    } else {
      ElMessage.error(response.errorMessage)
    }
  }).catch((err: any) => {
    ElMessage.error(err.message)
  })
}

// 分页事件处理
const handleSizeChange = (value: number) => {
  pageSize.value = value
  queryRecords()
}

const handleCurrentChange = (value: number) => {
  currentPage.value = value
  queryRecords()
}

// 删除账目
const deleteRecord = (id: string) => {
  // 弹出确认框
  ElMessageBox.confirm('此操作将永久删除该账目, 是否继续?', '提示', {
    confirmButtonText: '确定',
    type: 'warning'
  }).then(() => {
    // 确认删除
    axios.delete(API_BASE + '/IncomeExpenditureRecord/Delete/' + id, {
      headers: {
        Authorization: localStorage.getItem('token')
      }
    }).then((res: any) => {
      const response = res.data
      if (response.statusCode === 200) {
        ElMessage.success('删除成功')
        queryRecords()
      } else {
        ElMessage.error(response.errorMessage)
      }
    }).catch((err: any) => {
      ElMessage.error(err.message)
    })
  }).catch(() => {
    ElMessage.info('已取消删除')
  })
}

// 打开新增/编辑弹窗
const openAddDialog = () => {
  dialogVisible.value = true
  dialogTitle.value = '新增账目'
  // 新增
  Object.assign(form, {
    amount: 0,
    classificationType: 1,
    incomeExpenditureClassificationId: '',
    accountBookId: accountBookId.value,
    recordDate: '',
    currencyId: '',
    remark: ''
  })
  editId = null
}

// 打开编辑弹窗
const openEditDialog = (row: RecordItem) => {
  dialogVisible.value = true
  dialogTitle.value = '编辑账目'
  if (row) {
    console.log(row)
    // 修改 - 将RecordItem映射到RecordRequest
    Object.assign(form, {
      amount: row.afterAmount,
      classificationType: 1, // 默认设置为支出类型，后续可以根据实际数据调整
      incomeExpenditureClassificationId: row.incomeExpenditureClassificationId,
      accountBookId: row.accountBookId,
      recordDate: row.recordDate,
      currencyId: row.currencyId,
      remark: row.remark
    })
    editId = row.incomeExpenditureRecordId
  }
}

/**
 * 保存账目记录
 */
const submitForm = (formEl: FormInstance | undefined) => {
  if (!formEl) return
  formEl.validate().then(() => {
    // 构建请求参数，确保日期格式正确
    const requestData = {
      ...form,
      recordDate: form.recordDate ? new Date(form.recordDate).toISOString() : new Date().toISOString()
    }

    if (editId) {
      // 修改
      const updateData = {
        incomeExpenditureRecordId: editId,
        amount: requestData.amount,
        incomeExpenditureClassificationId: requestData.incomeExpenditureClassificationId,
        accountBookId: requestData.accountBookId,
        recordDate: requestData.recordDate,
        currencyId: requestData.currencyId,
        remark: requestData.remark
      }
      axios.put(API_BASE + '/IncomeExpenditureRecord/Update', updateData, {
        headers: {
          Authorization: localStorage.getItem('token')
        }
      }).then((res: any) => {
        const response = res.data
        if (response.statusCode === 200) {
          ElMessage.success('修改成功')
          dialogVisible.value = false
          queryRecords()
        } else {
          ElMessage.error(response.errorMessage)
        }
      }).catch((err: any) => {
        ElMessage.error(err.message)
      })
    } else {
      // 新增
      axios.post(API_BASE + '/IncomeExpenditureRecord/Add', requestData, {
        headers: {
          Authorization: localStorage.getItem('token')
        }
      }).then((res: any) => {
        const response = res.data
        if (response.statusCode === 200) {
          ElMessage.success('新增成功')
          dialogVisible.value = false
          queryRecords()
        } else {
          ElMessage.error(response.errorMessage)
        }
      }).catch((err: any) => {
        ElMessage.error(err.message)
      })
    }
  }).catch(() => {
    ElMessage.error('请检查输入项')
  })
}

// 公用
onMounted(() => {
  queryRecords()
  // 初始化时加载默认的收支分类（支出类型）
  queryIncomeExpenditureClassifications(0)
  // 加载货币列表
  queryCurrencies()
})
</script>

<style scoped>
.container {
  padding: 20px 0 0 0;
}
</style> 