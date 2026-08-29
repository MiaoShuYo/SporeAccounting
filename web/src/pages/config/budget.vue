<script setup lang="ts">
import type {Budget, BudgetRequest} from "../../Interface/budget.ts";
import {inject, onMounted, reactive, ref} from "vue";
import {ElMessage, ElMessageBox, type FormInstance, type FormRules} from "element-plus";
import type {IncomeExpenditureClassification} from "../../Interface/IncomeExpenditureClassification.ts";

const axios: any = inject('axios')
const RAW_API_BASE = (import.meta.env.VITE_API_BASE_URL as string | undefined)
const API_BASE = (!RAW_API_BASE || RAW_API_BASE === 'undefined' || RAW_API_BASE === 'null') ? '/api' : RAW_API_BASE
// 弹窗控制
const dialogVisible = ref(false);
// 表格
const budgetResponse = reactive<Budget[]>([])

const queryBudget = () => {
  axios.get(API_BASE + '/Budget/Query', {
    headers: {
      Authorization: localStorage.getItem('token')
    }
  })
      .then((res: any) => {
        const response = res.data;
        if (response.statusCode === 200) {
          budgetResponse.splice(0, budgetResponse.length);
          // 接收返回值
          budgetResponse.push(...response.data);
        } else {
          ElMessage.error(response.errorMessage)
        }
      }).catch((err: any) => {
    ElMessage.error(err.message);
  });
}
const editBudget = (id: any) => {
  // 编辑预算
  queryExpenditure();
  dialogVisible.value = true;
  // 查询预算
  axios.get(API_BASE + '/Budget/QueryById/' + id, {
    headers: {
      Authorization: localStorage.getItem('token')
    }
  })
      .then((res: any) => {
        const response = res.data;
        console.log(response);
        if (response.statusCode === 200) {
          // 接收返回值
          budgetRequest.id = response.data.id;
          budgetRequest.amount = response.data.amount;
          budgetRequest.period = response.data.period;
          budgetRequest.startTime = response.data.startTime;
          budgetRequest.endTime = response.data.endTime;
          budgetRequest.classificationId = response.data.classificationId;
          budgetRequest.classificationName = response.data.classificationName;
          budgetRequest.remark = response.data.remark;
        } else {
          ElMessage.error(response.errorMessage)
        }
      }).catch((err: any) => {
    ElMessage.error(err.message);
  });
}
// 删除预算
const deleteBudget = (id: String) => {
  // 询问是否删除
  ElMessageBox.confirm('是否删除该预算?', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(() => {
    // 删除预算
    axios.delete(API_BASE + '/Budget/Delete/' + id, {
      headers: {
        Authorization: localStorage.getItem('token')
      }
    })
        .then((res: any) => {
          const response = res.data;
          if (response.statusCode === 200) {
            ElMessage.success("删除成功");
            queryBudget();
          } else {
            ElMessage.error(response.errorMessage)
          }
        }).catch((err: any) => {
      ElMessage.error(err.message);
    });
  });

}

// 格式化周期
const formatPeriod = (_row: any, _column: any, cellValue: any, _index: number) => {
  return cellValue === 0 ? '年' : cellValue === 1 ? '月' : '季度';
}

// 新增/修改预算
const budgetRequest = reactive<BudgetRequest>({
  id: '',
  amount: 0,
  period: 0,
  startTime: '',
  endTime: '',
  classificationId: '',
  classificationName: '',
  remark: ''
});

// 预算验证
const rules = reactive<FormRules<BudgetRequest>>({
  amount: [
    {
      validator: (_rule, value, callback) => {
        if (value <= 0) {
          callback(new Error('预算金额必须大于0'));
        } else {
          callback();
        }
      }, trigger: 'blur'
    },
  ],
  period: [
    {required: true, message: '预算周期不能为空', trigger: 'blur'},
  ],
  startTime: [
    {required: true, message: '开始日期不能为空', trigger: 'blur'},
  ],
  endTime: [
    {required: true, message: '结束日期不能为空', trigger: 'blur'},
  ],
  classificationId: [
    {required: true, message: '预算分类不能为空', trigger: 'blur'},
  ],
  remark: [
    {max: 200, message: '备注长度不能超过200字', trigger: 'blur'},
  ]
})
const budgetFormRef = ref<FormInstance>()
const addBudgetBtn = () => {
  dialogVisible.value = true;
  queryExpenditure();
}

// 弹窗
const expenditureClassification = reactive<IncomeExpenditureClassification[]>([]);

// 获取支出分类
const queryExpenditure = () => {
  axios.get(API_BASE + '/IncomeExpenditureClassification/QueryByType/1', {
    headers: {
      Authorization: localStorage.getItem('token')
    }
  })
      .then((res: any) => {
        const response = res.data;
        if (response.statusCode === 200) {
          // 接收返回值
          expenditureClassification.push(...response.data);
          console.log(expenditureClassification);
        } else {
          ElMessage.error(response.errorMessage)
        }
      }).catch((err: any) => {
    ElMessage.error(err.message);
  });
}
// 保存预算
const save = (formEl: FormInstance | undefined) => {
  if (!formEl) return
  formEl.validate().then(() => {
    if (budgetRequest.id === '') {
      axios.post(API_BASE + '/Budget/Add', budgetRequest, {
        headers: {
          Authorization: localStorage.getItem('token')
        }
      })
          .then((res: any) => {
            const response = res.data;
            if (response.statusCode === 200) {
              ElMessage.success("保存成功");
              dialogVisible.value = false;
              queryBudget();
            } else {
              ElMessage.error(response.errorMessage)
            }
          }).catch((err: any) => {
        ElMessage.error(err.message);
      });
    } else {
      axios.put(API_BASE + '/Budget/Update', budgetRequest, {
        headers: {
          Authorization: localStorage.getItem('token')
        }
      })
          .then((res: any) => {
            const response = res.data;
            if (response.statusCode === 200) {
              ElMessage.success("修改成功");
              dialogVisible.value = false;
              queryBudget();
            } else {
              ElMessage.error(response.errorMessage)
            }
          }).catch((err: any) => {
        ElMessage.error(err.message);
      });
    }
  }).catch(() => {
    return
  })
}
// 公用
onMounted(() => {
  queryBudget();
})

</script>

<template>
  <!--面包屑导航-->
  <el-breadcrumb separator="/">
    <el-breadcrumb-item :to="{path:'/'}">首页</el-breadcrumb-item>
    <el-breadcrumb-item :to="{path:'/config'}">系统配置</el-breadcrumb-item>
    <el-breadcrumb-item><span style="font-weight: bold">预算设置</span></el-breadcrumb-item>
  </el-breadcrumb>
  <div class="container">
    <el-button type="primary" @click="addBudgetBtn">新增预算</el-button>
  </div>
  <div>
    <el-table :data="budgetResponse" style="width: 100%">
      <el-table-column type="index" width="100" label="编号"></el-table-column>
      <el-table-column prop="classificationName" label="预算分类"></el-table-column>
      <el-table-column prop="amount" label="预算金额"></el-table-column>
      <el-table-column prop="remaining" label="剩余金额"></el-table-column>
      <el-table-column prop="period" label="预算周期" :formatter="formatPeriod"></el-table-column>
      <el-table-column prop="startTime" label="开始日期"></el-table-column>
      <el-table-column prop="endTime" label="结束日期"></el-table-column>
      <el-table-column label="操作">
        <template #default="{row}">
          <el-button link type="warning" size="small" @click="editBudget(row.id)">
            修改
          </el-button>
          <el-button link type="danger" size="small" @click="deleteBudget(row.id)">
            删除
          </el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
  <!--新增/修改预算-->
  <el-dialog title="预算" v-model="dialogVisible" width="400">
    <el-form :model="budgetRequest" :rules="rules" ref="budgetFormRef"
             label-width="80px">
      <el-form-item label="预算分类" prop="classificationId">
        <el-select v-model="budgetRequest.classificationId" placeholder="请选择预算分类">
          <el-option
              v-for="item in expenditureClassification"
              :key="item.id"
              :label="item.name"
              :value="item.id">
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="预算金额" prop="amount">
        <el-input v-model="budgetRequest.amount" placeholder="请输入预算金额"></el-input>
      </el-form-item>
      <el-form-item label="预算周期" prop="period">
        <el-radio-group v-model="budgetRequest.period">
          <el-radio :value="0">年</el-radio>
          <el-radio :value="1">月</el-radio>
          <el-radio :value="-2">季</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item label="开始日期" prop="startTime">
        <el-date-picker v-model="budgetRequest.startTime" type="date" placeholder="选择日期"></el-date-picker>
      </el-form-item>
      <el-form-item label="结束日期" prop="endTime">
        <el-date-picker v-model="budgetRequest.endTime" type="date" placeholder="选择日期"></el-date-picker>
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="dialogVisible = false">取 消</el-button>
      <el-button type="primary" @click="save(budgetFormRef)">确
        定
      </el-button>
    </template>
  </el-dialog>
</template>

<style scoped>
.container {
  padding: 20px 0 0 0;
  text-align: right;
}
</style>