namespace SP.FinanceService.Models.Request
{
    /// <summary>
    /// 账本合并请求模型
    /// </summary>
    public class AccountBookMergeRequest
    {
        /// <summary>
        /// 目标账本ID
        /// </summary>
        public long TargetAccountBookId { get; set; }
        /// <summary>
        /// 源账本ID列表
        /// </summary>
        public List<long> SourceAccountBookIds { get; set; } = new();
    }
}
