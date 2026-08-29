// 账本信息接口
export interface AccountBook {
  accountBookId: string;
  name: string;
  remarks?: string;
  balance: number;
}

// 创建账本请求接口
export interface CreateAccountBookRequest {
  name: string;
  remarks?: string;
}

// 更新账本请求接口
export interface UpdateAccountBookRequest {
  accountBookId: string;
  name?: string;
  remarks?: string;
} 