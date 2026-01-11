# SP.Common - Nacos 封装（OpenAPI）

`SP.Common` 已集成 `nacos-sdk-csharp`（用于 `AddNacosV2Naming`/`AddNacosV2Configuration` 等），同时提供一个更“直接可调用”的 Nacos OpenAPI 封装，用于：

- 服务发现（查询健康实例、解析一个实例地址）
- 服务注册/注销（手动注册实例时使用）
- 配置读取与轮询拉取（Get + Poll/Watch）

## 使用

在各服务的 `Program.cs` 中（保持你现有的 `AddNacosAspNet` / `AddNacosV2Naming` / `AddNacosV2Configuration` 不变），额外注册：

```csharp
using SP.Common.Nacos;

builder.Services.AddSpNacos(builder.Configuration);
```

如果你希望完全不再使用 `nacos-sdk-csharp` 的配置中心注入（即删除 `AddNacosV2Configuration`），可以改为：

```csharp
using SP.Common.Nacos.Configuration;

builder.Configuration.AddSpNacosConfiguration(builder.Configuration.GetSection("nacos"));
builder.Services.AddSpNacos(builder.Configuration);
```

然后通过 DI 使用：

```csharp
public class Demo
{
	private readonly INacosClient _nacos;

	public Demo(INacosClient nacos) => _nacos = nacos;

	public async Task<string?> ReadConfig(CancellationToken ct)
		=> await _nacos.GetConfigAsync("appsettings.json", "DEFAULT_GROUP", ct: ct);
}
```

### 项目说明
本项目是[《.NET 8 实战--孢子记账--从单体到微服务》](https://blog.csdn.net/gangzhucoll/category_12578008.html)系列专栏的教学项目，项目每周都会更新，针对课程的不同阶段会有对应的分支，目前课程处于单体应用阶段，因此如需下载代码请将分支切换到[MonomerApplication](https://github.com/MiaoShuYo/SporeAccounting/tree/MonomerApplication)分支。
### 开源协议
遵循Apache 2.0 License，但还需遵循如下条款：
1. 允许将本项目用于商业用途。但是，未经原作者事先书面许可，本项目不得以其原始形式或修改形式**用于任何有偿培训以及有偿教育服务**。
2. 本项目**可用于免费教育以及免费培训服务**。但是，必须注明原作者，并且必须在培训材料或其他已发表的内容中清楚地说明项目的来源。
### 答疑
由于目前还没有群，如果需要答疑，请在专栏的任意一篇文章中留言，或者在我的CSDN博客中私信我，留下您的微信、QQ我将会及时加上你为您答疑，您也可以扫我的微信二维码加我好友。
![alt text](ff9a3563927a6a03191af63cca6af31.jpg)