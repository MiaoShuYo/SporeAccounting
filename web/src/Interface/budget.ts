// 预算
export interface Budget {
    id: string,
    amount: number,
    period: number,
    remaining:number,
    startTime: string,
    endTime: string,
    classificationName: string,
    remark: string
}

// 预算修改
export interface BudgetRequest{
    id: string,
    amount: number,
    period: number,
    startTime: string,
    endTime: string,
    classificationId: string,
    classificationName: string,
    remark: string
}