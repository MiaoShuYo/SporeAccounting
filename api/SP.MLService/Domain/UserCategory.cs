namespace SP.MLService.Domain;

/// <summary>
/// 用户自定义类目
/// 
/// 数据模型说明：
/// - 代表用户在记账系统中创建的分类类目
/// - 支持个性化的类目管理和命名
/// - 作为机器学习预测的候选项
/// 
/// 设计特点：
/// - 不可变对象，确保数据一致性
/// - 包含唯一标识符和显示名称
/// - 支持友好的字符串表示
/// 
/// 使用场景：
/// - 用户创建和管理个人消费分类
/// - 机器学习模型的预测候选项
/// - UI界面的类目选择和显示
/// 
/// 数据来源：
/// - 用户手动创建的自定义类目
/// - 系统预设的通用类目模板
/// - 从历史数据中学习的推荐类目
/// </summary>
public sealed class UserCategory
{
    /// <summary>
    /// 用户类目构造函数
    /// </summary>
    /// <param name="id">类目唯一标识符，用于数据库存储和引用</param>
    /// <param name="name">类目显示名称，用于用户界面展示和文本匹配</param>
    public UserCategory(string id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>类目唯一标识符，用于数据库存储和系统内部引用</summary>
    public string Id { get; }
        
    /// <summary>类目显示名称，用于用户界面展示和机器学习文本特征</summary>
    public string Name { get; }
        
    /// <summary>
    /// 友好的字符串表示，格式为"名称(ID)"
    /// 便于调试和日志记录
    /// </summary>
    /// <returns>格式化的类目字符串</returns>
    public override string ToString() => $"{Name}({Id})";
}