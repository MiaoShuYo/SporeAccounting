import type {PageRequest} from "./request.ts";

/**
 * 收支分类
 */
export interface IncomeExpenditureClassification {
    id: string;
    name: string;
    type: number;
    parentId: string;
    parentName: string;
}

/**
 * 收支分类提交
 */
export interface IncomeExpenditureClassificationRequest{
    id: string;
    name: string;
    type: number;
    parentClassificationId: string;
}

/**
 * 分页查询
 */
export interface IncomeExpenditureClassificationPage extends PageRequest {
    classificationName: string;
    type: number;
    parentClassificationId: string;
}