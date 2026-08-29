import type {PageRequest} from "./request.ts";

/**
 * 分页查询
 */
export interface RecordPage extends PageRequest {
    startDate: string
    endDate: string
    accountBookId: string
}

/**
 * 账本记录
 */
export interface RecordItem {
    incomeExpenditureRecordId: string
    beforAmount: number
    afterAmount: number
    incomeExpenditureClassificationId: string
    incomeExpenditureClassificationName: string
    accountBookId: string
    accountBookName: string
    recordDate: string
    currencyId: string
    currencyName: string
    remark: string
}

/**
 * 账本记录提交
 */
export interface RecordRequest {
    id: string,
    amount: number
    classificationType: number
    incomeExpenditureClassificationId: string
    accountBookId: string
    recordDate: string
    currencyId: string
    remark: string
}