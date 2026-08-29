/*重置密码*/
export interface ResetPassword {
    /** 旧密码 */
    oldPassword: string;
    /** 新密码 */
    newPassword: string;
    /** 确认新密码 */
    reNewPassword: string;
}
/*安全设置*/
export interface SecuritySetting {
    /** 手机号 */
    phoneNumber: string;
    /** 邮箱 */
    email: string;
}
/*个人信息*/
export interface PersonalInformation {
    /** 用户名 */
    userName: string;
}