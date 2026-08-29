<script setup lang="ts">
import {inject, onMounted, reactive, ref} from 'vue'
import type {Config as PrimaryCurrency} from "../../Interface/config.ts";
import type {Currency} from "../../Interface/currency.ts";
import type {ExchangeRate} from "../../Interface/exchangeRate.ts";
import type {PageResponse} from '../../Interface/response';
import {ElMessage} from "element-plus";
import {dayjs} from "element-plus";

const axios: any = inject('axios')
const RAW_API_BASE = (import.meta.env.VITE_API_BASE_URL as string | undefined)
const API_BASE = (!RAW_API_BASE || RAW_API_BASE === 'undefined' || RAW_API_BASE === 'null') ? '/api' : RAW_API_BASE
const isEdit = ref(false)
//定义接受返回值的PrimaryCurrency
const primaryCurrency = reactive<PrimaryCurrency>({
  id: '',
  value: '',
  name: ''
})
//接收汇率返回值ExchangeRate
const exchangeRate :PageResponse<ExchangeRate> = reactive({
  data: [],
  rowCount: 0,
  pageCount: 0
})
//定义接收全部币种返回值的Currency
const primaryCurrencyOptions = ref<Currency[]>([]);
const pageSize = ref(10);
const currentPage = ref(1);


onMounted(() => {
  queryPrimaryCurrency();
  queryExchangeRate();
})

// 页面加载出来后，获取主币种
const queryPrimaryCurrency = () => {
  axios.get(API_BASE + '/Config/QueryByConfigType/0', {
    headers: {
      Authorization: localStorage.getItem('token')
    }
  })
      .then((res: any) => {
        const response = res.data;
        if (response.statusCode === 200) {
          // 接收返回值
          primaryCurrency.id = response.data.id;
          primaryCurrency.value = response.data.value;
          primaryCurrency.name = response.data.name;
        } else {
          ElMessage.error(response.errorMessage)
        }
      }).catch((err: any) => {
    ElMessage.error(err.message);
  });
}

// 获取汇率
const queryExchangeRate = () => {
  axios.get(API_BASE + '/ExchangeRate/Query/' + currentPage.value + '/' + pageSize.value, {
    headers: {
      Authorization: localStorage.getItem('token')
    }
  })
      .then((res: any) => {
        const response = res.data;
        if (response.statusCode === 200) {
          // 接收返回值
          exchangeRate.rowCount = response.data.rowCount;
          exchangeRate.data = response.data.data;
          exchangeRate.pageCount=response.data.pageCount;
        } else {
          ElMessage.error(response.errorMessage)
        }
      }).catch((err: any) => {
    ElMessage.error(err.message);
  });
}


const edit = () => {
  isEdit.value = true;
  //获取全部币种
  axios.get(API_BASE + '/Currency/QueryAll', {
    headers: {
      Authorization: localStorage.getItem('token')
    }
  })
      .then((res: any) => {
        const response = res.data;
        if (response.statusCode === 200) {
          // 接收返回值
          primaryCurrencyOptions.value = response.data;
        } else {
          ElMessage.error(response.errorMessage)
        }
      }).catch((err: any) => {
    ElMessage.error(err.message);
  });
}

const save = () => {
  axios.put(API_BASE + '/Config/Update', {
    id: primaryCurrency.id,
    value: primaryCurrency.value,
    configTypeEnum: 0
  }, {
    headers: {
      Authorization: localStorage.getItem('token')
    }
  })
      .then((res: any) => {
        const response = res.data;
        if (response.statusCode === 200) {
          queryPrimaryCurrency();
          isEdit.value = false;
          ElMessage.success("主币种设置完成")
        } else {
          ElMessage.error(response.errorMessage)
        }
      }).catch((err: any) => {
    ElMessage.error(err.message);
  });
}

const handleSizeChange = (value: number) => {
  pageSize.value = value;
  queryExchangeRate();
}
const handleCurrentChange = (value: number) => {
  currentPage.value = value;
  queryExchangeRate();
}

const formatDate = (_row: any, _column: any, cellValue: any, _index: number) => {
  return dayjs(cellValue).format('YYYY-MM-DD');
};
</script>

<template>
  <!--面包屑导航-->
  <el-breadcrumb separator="/">
    <el-breadcrumb-item :to="{path:'/'}">首页</el-breadcrumb-item>
    <el-breadcrumb-item :to="{path:'/config'}">系统配置</el-breadcrumb-item>
    <el-breadcrumb-item><span style="font-weight: bold">主币种设置</span></el-breadcrumb-item>
  </el-breadcrumb>
  <div class="container">
    <div v-if="!isEdit">
      <!-- 一个文本区域 -->
      <el-text>当前主币种：{{ primaryCurrency.name }}</el-text>
      <el-button type="text" style="padding-left: 10px" @click="edit">修改</el-button>
    </div>
    <el-form v-if="isEdit">
      <el-form-item label="主币种：">
        <el-select v-model="primaryCurrency.value" placeholder="请选择主币种" style="width: 10%;min-width: 168px">
          <el-option
              v-for="item in primaryCurrencyOptions"
              :key="item.id"
              :label="item.name"
              :value="item.id">
          </el-option>
        </el-select>
        <el-button id="save" type="primary" @click="save">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
  <div>
    <!--汇率表-->
    <el-table :data="exchangeRate.data" style="width: 100%">
      <el-table-column type="index" width="100" label="编号"></el-table-column>
      <el-table-column prop="currency1" label="币种1"></el-table-column>
      <el-table-column prop="currency2" label="币种2"></el-table-column>
      <el-table-column prop="rate" label="汇率"></el-table-column>
      <el-table-column prop="updateTime" label="日期" :formatter="formatDate"></el-table-column>
    </el-table>
    <el-pagination
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
        :current-page="currentPage"
        :page-sizes="[10, 20, 30, 40]"
        :page-size="pageSize"
        layout="total, sizes, prev, pager, next, jumper"
        :total="exchangeRate.rowCount" style="margin-top: 20px">
    </el-pagination>
  </div>
</template>

<style scoped>
.container {
  padding: 20px 0 0 0;
}

#save {
  margin-left: 10px;
}

</style>