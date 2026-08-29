<script setup lang="ts">
import {inject, onMounted, reactive, ref} from 'vue'
import {ElMessage, ElMessageBox, type FormInstance, type FormRules} from "element-plus";
import type {
  IncomeExpenditureClassification,
  IncomeExpenditureClassificationPage, IncomeExpenditureClassificationRequest
} from "../../Interface/IncomeExpenditureClassification";
import type {PageResponse} from "../../Interface/response.ts";

const axios: any = inject('axios')
const RAW_API_BASE = (import.meta.env.VITE_API_BASE_URL as string | undefined)
const API_BASE = (!RAW_API_BASE || RAW_API_BASE === 'undefined' || RAW_API_BASE === 'null') ? '/api' : RAW_API_BASE

// 顶部搜索功能
// 查询收支分类参数IncomeExpenditureClassificationPage
const incomeExpenditureClassificationPage = ref<IncomeExpenditureClassificationPage>({
  pageSize: 10,
  pageNumber: 1,
  type: 2,
  parentClassificationId: '-1',
  classificationName: ''
});
// 接收父级类型的数据IncomeExpenditureClassification
const incomeExpenditureClassificationParent = reactive<IncomeExpenditureClassification[]>([]);
// 父级类型操作的数据
const incomeExpenditureClassificationParentOptions = reactive<IncomeExpenditureClassification[]>([]);

// 查询父级类型
const queryIncomeExpenditureType = () => {
  axios.get(API_BASE + '/IncomeExpenditureClassification/QueryParent', {
    headers: {
      Authorization: localStorage.getItem('token')
    }
  }).then((res: any) => {
    const response = res.data;
    if (response.statusCode === 200) {
      // 接收返回值
      incomeExpenditureClassificationParent.splice(0, incomeExpenditureClassificationParent.length);
      incomeExpenditureClassificationParentOptions.splice(0, incomeExpenditureClassificationParentOptions.length);
      incomeExpenditureClassificationParent.push(...response.data);
      incomeExpenditureClassificationParentOptions.push(...response.data);
    } else {
      ElMessage.error(response.errorMessage)
    }
  }).catch((err: any) => {
    ElMessage.error(err.message);
  });
}
// 收支分类下拉选择框
const incomeExpenditureCategoryChange = (value: any) => {
  // 过滤出来对应分类的类型，不修改元数据，过滤后的数据绑定到父级分类下拉菜单
  incomeExpenditureClassificationParentOptions
      .splice(0, incomeExpenditureClassificationParentOptions.length, ...incomeExpenditureClassificationParent);
  if (value === 2) {
    return;
  }
  const filteredData = incomeExpenditureClassificationParent.filter(item => item.type === Number(value));
  incomeExpenditureClassificationParentOptions.splice(0, incomeExpenditureClassificationParent.length, ...filteredData);
}


// 表格功能
// 接收表格数据
const incomeExpenditureClassificationPageResponse: PageResponse<IncomeExpenditureClassificationPage> = reactive({
  data: [],
  rowCount: 0,
  pageCount: 0
})
// 每页大小
const pageSize = ref(10);
// 当前页
const currentPage = ref(1);
// 弹窗控制
const dialogVisible = ref(false);
const incomeExpenditureClassification = reactive<IncomeExpenditureClassificationRequest>({
  id: '',
  name: '',
  type: 0,
  parentClassificationId: ''
});
const incomeExpenditureClassificationFormRef = ref<FormInstance>()

// 分页查询收支类型
const queryIncomeExpenditure = () => {
  incomeExpenditureClassificationPage.value.pageNumber = currentPage.value;
  incomeExpenditureClassificationPage.value.pageSize = pageSize.value;
  axios.post(API_BASE + '/IncomeExpenditureClassification/Query',
      incomeExpenditureClassificationPage.value, {
        headers: {
          Authorization: localStorage.getItem('token')
        }
      }).then((res: any) => {
    const response = res.data;
    if (response.statusCode === 200) {
      // 接收返回值
      incomeExpenditureClassificationPageResponse.pageCount = response.data.pageCount;
      incomeExpenditureClassificationPageResponse.rowCount = response.data.rowCount;
      incomeExpenditureClassificationPageResponse.data = response.data.data;
    } else {
      ElMessage.error(response.errorMessage)
    }
  }).catch((err: any) => {
    ElMessage.error(err.message);
  });
}

const handleSizeChange = (value: number) => {
  pageSize.value = value;
  queryIncomeExpenditure();
}
const handleCurrentChange = (value: number) => {
  currentPage.value = value;
  queryIncomeExpenditure();
}
//删除收支类型
const deleteIncomeExpenditureClassification = (id: string) => {
  // 弹出确认框
  ElMessageBox.confirm('此操作将永久删除该收支类型, 是否继续?', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(() => {
    // 确认删除
    axios.delete(API_BASE + '/IncomeExpenditureClassification/Delete/' + id, {
      headers: {
        Authorization: localStorage.getItem('token')
      }
    }).then((res: any) => {
      const response = res.data;
      if (response.statusCode === 200) {
        ElMessage.success('删除成功');
        queryIncomeExpenditure();
        queryIncomeExpenditureType();
      } else {
        ElMessage.error(response.errorMessage)
      }
    }).catch((err: any) => {
      ElMessage.error(err.message);
    });
  }).catch(() => {
    ElMessage.info('已取消删除');
  });
}
// 收支分类弹窗事件
const editIncomeExpenditureClassification = (row: any) => {
  dialogVisible.value = true;
  if (row) {
    console.log(row);
    // 修改
    incomeExpenditureClassification.id = row.id;
    incomeExpenditureClassification.name = row.name;
    incomeExpenditureClassification.type = row.type;
    incomeExpenditureClassification.parentClassificationId = row.parentId;
  } else {
    // 新增
    incomeExpenditureClassification.id = '';
    incomeExpenditureClassification.name = '';
    incomeExpenditureClassification.type = 0;
    incomeExpenditureClassification.parentClassificationId = '';
  }
}

/**
 * 保存收支分类
 */
const saveIncomeExpenditureClassification = (formEl: FormInstance | undefined) => {
  if (!formEl) return
  formEl.validate().then(() => {
    if (incomeExpenditureClassification.id) {
      // 修改
      axios.put(API_BASE + '/IncomeExpenditureClassification/Update',
          incomeExpenditureClassification, {
            headers: {
              Authorization: localStorage.getItem('token')
            }
          }).then((res: any) => {
        const response = res.data;
        if (response.statusCode === 200) {
          ElMessage.success('修改成功');
          dialogVisible.value = false;
          queryIncomeExpenditure();
        } else {
          ElMessage.error(response.errorMessage)
        }
      }).catch((err: any) => {
        ElMessage.error(err.message);
      });
    } else {
      // 新增
      axios.post(API_BASE + '/IncomeExpenditureClassification/Add',
          incomeExpenditureClassification, {
            headers: {
              Authorization: localStorage.getItem('token')
            }
          }).then((res: any) => {
        const response = res.data;
        if (response.statusCode === 200) {
          ElMessage.success('新增成功');
          dialogVisible.value = false;
          queryIncomeExpenditure();
        } else {
          ElMessage.error(response.errorMessage)
        }
      }).catch((err: any) => {
        ElMessage.error(err.message);
      });
    }
  }).catch(() => {
    ElMessage.error('请检查输入项');
  });
}

// 收支类型验证
const rules = reactive<FormRules<IncomeExpenditureClassificationRequest>>({
  name: [
    {required: true, message: '分类名称不能为空', trigger: 'blur'}
  ],
  type: [
    {required: true, message: '收支分类不能为空', trigger: 'blur'},
  ]
})

// 格式化收支分类
const formatType = (_row: any, _column: any, cellValue: any, _index: number) => {
  return cellValue === 0 ? '收入' : cellValue === 1 ? '支出' : '其他';
}

// 公用
onMounted(() => {
  queryIncomeExpenditureType();
  queryIncomeExpenditure();
})
</script>

<template>
  <!--面包屑导航-->
  <el-breadcrumb separator="/">
    <el-breadcrumb-item :to="{path:'/'}">首页</el-breadcrumb-item>
    <el-breadcrumb-item :to="{path:'/config'}">系统配置</el-breadcrumb-item>
    <el-breadcrumb-item><span style="font-weight: bold">收支类型</span></el-breadcrumb-item>
  </el-breadcrumb>
  <div class="container">
    <!--搜索功能区域-->
    <el-row :gutter="20">
      <el-col :span="3">
        <el-text>收支分类：</el-text>
        <el-select
            v-model="incomeExpenditureClassificationPage.type"
            :default-first-option="true"
            placeholder="请选择收支分类"
            @change="incomeExpenditureCategoryChange"
            style="width: 65%">
          <el-option label="全部" :value="2"></el-option>
          <el-option label="收入" :value="0"></el-option>
          <el-option label="支出" :value="1"></el-option>
          <el-option label="其他" :value="-1"></el-option>
        </el-select>
      </el-col>
      <el-col :span="3">
        <el-text>父级类型：</el-text>
        <el-select v-model="incomeExpenditureClassificationPage.parentClassificationId" :default-first-option="true"
                   placeholder="请选择父级类型" style="width: 65%">
          <el-option label="全部" value="-1"></el-option>
          <el-option
              v-for="item in incomeExpenditureClassificationParentOptions"
              :key="item.id"
              :label="item.name"
              :value="item.id">
          </el-option>
        </el-select>
      </el-col>
      <el-col :span="4">
        <el-text>类型名：</el-text>
        <el-input placeholder="请输入类型名" v-model="incomeExpenditureClassificationPage.classificationName"
                  style="width: 65%"></el-input>
      </el-col>
      <el-col :span="6">
        <el-button type="primary" @click="queryIncomeExpenditure">搜索</el-button>
        <el-button type="success" @click="editIncomeExpenditureClassification">新增</el-button>
      </el-col>
    </el-row>
  </div>
  <div>
    <el-table :data="incomeExpenditureClassificationPageResponse.data" style="width: 100%">
      <el-table-column type="index" width="100" label="编号"></el-table-column>
      <el-table-column prop="name" label="收支类型"></el-table-column>
      <el-table-column prop="type" label="收支分类" :formatter="formatType"></el-table-column>
      <el-table-column prop="parentName" label="父级类型"></el-table-column>
      <el-table-column label="操作">
        <template #default="{row}">
          <el-button link type="warning" size="small" @click="editIncomeExpenditureClassification(row)">
            修改
          </el-button>
          <el-button link type="danger" size="small" @click="deleteIncomeExpenditureClassification(row.id)">
            删除
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
        :current-page="currentPage"
        :page-sizes="[10, 20, 30, 40]"
        :page-size="pageSize"
        layout="total, sizes, prev, pager, next, jumper"
        :total="incomeExpenditureClassificationPageResponse.rowCount" style="margin-top: 20px">
    </el-pagination>
  </div>
  <!--新增/修改收支类型-->
  <el-dialog title="收支类型" v-model="dialogVisible" width="400">
    <el-form :model="incomeExpenditureClassification" :rules="rules" ref="incomeExpenditureClassificationFormRef"
             label-width="80px">
      <el-form-item label="收支类型" prop="name">
        <el-input v-model="incomeExpenditureClassification.name"></el-input>
      </el-form-item>
      <el-form-item label="收支分类" prop="type">
        <el-radio-group v-model="incomeExpenditureClassification.type">
          <el-radio :value="0">收入</el-radio>
          <el-radio :value="1">支出</el-radio>
          <el-radio :value="-1">其他</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item label="父级类型" prop="parentClassificationId">
        <el-select
            clearable
            v-model="incomeExpenditureClassification.parentClassificationId"
            placeholder="请选择父级类型"
        >
          <el-option
              v-for="item in incomeExpenditureClassificationParentOptions"
              :key="item.id"
              :label="item.name"
              :value="item.id">
          </el-option>
        </el-select>
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="dialogVisible = false">取 消</el-button>
      <el-button type="primary" @click="saveIncomeExpenditureClassification(incomeExpenditureClassificationFormRef)">确
        定
      </el-button>
    </template>
  </el-dialog>
</template>
<style scoped>
.container {
  padding: 20px 0 0 0;
}
</style>