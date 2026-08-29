<script setup lang="ts">
import { ref, reactive, inject, onMounted, computed, onUnmounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Search, Refresh } from '@element-plus/icons-vue'
import type { Response } from '../Interface/response'
import type { ReportRequest, ReportResponse } from '../Interface/report'
import { ReportTypeEnum } from '../Interface/report'
import * as echarts from 'echarts'

// 注入axios
const axios: any = inject('axios')
const RAW_API_BASE = (import.meta.env.VITE_API_BASE_URL as string | undefined)
const API_BASE = (!RAW_API_BASE || RAW_API_BASE === 'undefined' || RAW_API_BASE === 'null') ? '/api' : RAW_API_BASE

// 响应式数据
const loading = ref(false)
const chartRef = ref<HTMLElement>()
let chartInstance: echarts.ECharts | null = null

// 报表参数
const reportParams = reactive<ReportRequest>({
  reportType: ReportTypeEnum.Monthly,
  year: new Date().getFullYear(),
  month: undefined
})

// 报表数据
const reportData = ref<ReportResponse>({
  data: []
})

// 计算属性：报表类型选项
const reportTypeOptions = computed(() => [
  { label: '月度报表', value: ReportTypeEnum.Monthly },
  { label: '季度报表', value: ReportTypeEnum.Quarterly },
  { label: '年度报表', value: ReportTypeEnum.Yearly }
])

// 计算属性：年份选项（当前年份前后5年）
const yearOptions = computed(() => {
  const currentYear = new Date().getFullYear()
  const years = []
  for (let i = currentYear - 5; i <= currentYear + 5; i++) {
    years.push({ label: `${i}年`, value: i })
  }
  return years
})

// 计算属性：月份选项
const monthOptions = computed(() => {
  const months = []
  for (let i = 1; i <= 12; i++) {
    months.push({ label: `${i}月`, value: i })
  }
  return months
})

// 格式化金额
const formatAmount = (amount: number) => {
  return '¥' + amount.toLocaleString('zh-CN', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  })
}

// 计算属性：格式化后的数据
const formattedData = computed(() => {
  return reportData.value.data.map((item, index) => ({
    ...item,
    timeLabel: getTimeLabel(item.year, item.month, index),
    formattedAmount: formatAmount(item.amount)
  }))
})

// 获取时间标签
const getTimeLabel = (year: number, month: number, index: number) => {
  if (month === 0) {
    return `${year}年第${index + 1}季度`
  }
  return `${year}年${month}月`
}

// 获取报表数据
const getReportData = async () => {
  loading.value = true
  try {
    const response = await axios.post(
      API_BASE + '/Report/GetReport',
      reportParams,
      {
        headers: {
          'Authorization': localStorage.getItem('token')
        }
      }
    )
    
    const result: Response<ReportResponse> = response.data
    if (result.statusCode === 200) {
      reportData.value = result.data
      console.log('报表数据:', reportData.value)
      renderChart()
    } else {
      ElMessage.error(result.errorMessage || '获取报表数据失败')
    }
  } catch (error: any) {
    console.error('获取报表数据失败:', error)
    ElMessage.error('获取报表数据失败')
  } finally {
    loading.value = false
  }
}

// 渲染图表
const renderChart = () => {
  if (!chartRef.value || !reportData.value.data.length) return
  
  // 确保DOM元素已经准备好
  setTimeout(() => {
    if (!chartInstance) {
      chartInstance = echarts.init(chartRef.value)
    }
    
    const chartData = formattedData.value
    const timeLabels = chartData.map(item => item.timeLabel)
    const amountData = chartData.map(item => item.amount)
    
    console.log('图表数据:', { timeLabels, amountData })
    
    const option = {
      title: {
        text: getChartTitle(),
        left: 'center',
        textStyle: {
          fontSize: 18,
          fontWeight: 'bold'
        }
      },
      tooltip: {
        trigger: 'axis',
        axisPointer: {
          type: 'shadow'
        },
        formatter: function(params: any) {
          const param = params[0]
          const value = param.value.toLocaleString('zh-CN', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
          })
          return `${param.name}<br/>金额: ¥${value}`
        }
      },
      grid: {
        left: '3%',
        right: '4%',
        bottom: '3%',
        containLabel: true
      },
      xAxis: {
        type: 'category',
        data: timeLabels,
        axisLabel: {
          rotate: reportParams.reportType === ReportTypeEnum.Yearly ? 0 : 45
        }
      },
      yAxis: {
        type: 'value',
        axisLabel: {
          formatter: function(value: number) {
            return '¥' + value.toLocaleString('zh-CN')
          }
        }
      },
      series: [
        {
          name: '金额',
          type: 'bar',
          data: amountData,
          itemStyle: {
            color: '#339af0'
          }
        }
      ]
    }
    
    chartInstance.setOption(option)
  }, 100)
}

// 获取图表标题
const getChartTitle = () => {
  const year = reportParams.year
  const month = reportParams.month
  
  switch (reportParams.reportType) {
    case ReportTypeEnum.Monthly:
      return month ? `${year}年${month}月月度报表` : `${year}年月度报表`
    case ReportTypeEnum.Quarterly:
      return `${year}年季度报表`
    case ReportTypeEnum.Yearly:
      return `${year}年年度报表`
    default:
      return '财务报表'
  }
}

// 查询报表
const queryReport = () => {
  getReportData()
}

// 重置参数
const resetParams = () => {
  reportParams.reportType = ReportTypeEnum.Monthly
  reportParams.year = new Date().getFullYear()
  reportParams.month = undefined
}

// 监听窗口大小变化，重新调整图表
const handleResize = () => {
  if (chartInstance) {
    chartInstance.resize()
  }
}

// 组件挂载时初始化
onMounted(() => {
  getReportData()
  window.addEventListener('resize', handleResize)
})

// 组件卸载时清理
onUnmounted(() => {
  window.removeEventListener('resize', handleResize)
  if (chartInstance) {
    chartInstance.dispose()
  }
})
</script>

<template>
  <div class="report-container">
    <!-- 查询条件 -->
    <el-card class="query-card" shadow="never">
      <template #header>
        <div class="card-header">
          <span>查询条件</span>
        </div>
      </template>
      
      <el-form :model="reportParams" inline>
        <el-form-item label="报表类型">
          <el-select v-model="reportParams.reportType" placeholder="请选择报表类型" style="width: 150px">
            <el-option
              v-for="option in reportTypeOptions"
              :key="option.value"
              :label="option.label"
              :value="option.value"
            />
          </el-select>
        </el-form-item>
        
        <el-form-item label="年份">
          <el-select v-model="reportParams.year" placeholder="请选择年份" style="width: 120px">
            <el-option
              v-for="option in yearOptions"
              :key="option.value"
              :label="option.label"
              :value="option.value"
            />
          </el-select>
        </el-form-item>
        
        <el-form-item label="月份" v-if="reportParams.reportType === ReportTypeEnum.Monthly">
          <el-select v-model="reportParams.month" placeholder="请选择月份" style="width: 120px" clearable>
            <el-option
              v-for="option in monthOptions"
              :key="option.value"
              :label="option.label"
              :value="option.value"
            />
          </el-select>
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" @click="queryReport" :loading="loading">
            <el-icon><Search /></el-icon>
            查询
          </el-button>
          <el-button @click="resetParams">
            <el-icon><Refresh /></el-icon>
            重置
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 图表展示 -->
    <el-card class="chart-card" shadow="never">
      <template #header>
        <div class="card-header">
          <span>图表分析</span>
        </div>
      </template>
      
      <div ref="chartRef" class="chart-container"></div>
    </el-card>
  </div>
</template>

<style scoped>
.report-container {
  padding: 20px;
}

.query-card,
.chart-card {
  margin-bottom: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.chart-container {
  height: 400px;
  width: 100%;
}

.amount-text {
  color: #339af0;
  font-weight: 500;
}

:deep(.el-card__header) {
  background-color: #f8f9fa;
  border-bottom: 1px solid #e9ecef;
}

:deep(.el-form-item__label) {
  font-weight: 500;
}
</style> 