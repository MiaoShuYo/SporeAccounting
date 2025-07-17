using Microsoft.AspNetCore.Mvc;
using SP.ConfigService.Models.Response;
using SP.ConfigService.Service;

namespace SP.ConfigService.Controllers
{
    /// <summary>
    /// 用户配置控制器
    /// </summary>
    [Route("api/config")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        /// <summary>
        /// 用户配置服务
        /// </summary>
        private readonly IConfigServer _configServer;

        /// <summary>
        /// 用户配置控制器构造函数
        /// </summary>
        /// <param name="configServer">用户配置服务</param>
        public ConfigController(IConfigServer configServer)
        {
            _configServer = configServer;
        }

        /// <summary>
        /// 查询用户配置
        /// </summary>
        /// <returns>用户配置</returns>
        [HttpGet("query")]
        public ActionResult<List<ConfigResponse>> GetConfig()
        {
            List<ConfigResponse> configs = _configServer.GetConfig();
            return Ok(configs);
        }

        /// <summary>
        /// 更新用户配置
        /// </summary>
        /// <param name="config">配置更新请求</param>
        /// <returns>>更新结果</returns>
        [HttpPut("update")]
        public ActionResult<bool> UpdateConfig([FromBody] ConfigResponse config)
        {
            _configServer.UpdateConfig(config);
            return Ok(true);
        }
    }
}