using Microsoft.AspNetCore.Mvc;
using SP.Common.Model.Enumeration;
using SP.ConfigService.Models.Enumeration;
using SP.ConfigService.Models.Request;
using SP.ConfigService.Models.Response;
using SP.ConfigService.Service;

namespace SP.ConfigService.Controllers
{
    /// <summary>
    /// 用户配置控制器
    /// </summary>
    [Route("api/configs")]
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
        /// 获取所有配置
        /// </summary>
        /// <returns>用户配置列表</returns>
        [HttpGet]
        public ActionResult<List<ConfigResponse>> GetConfigs()
        {
            List<ConfigResponse> configs = _configServer.GetConfig().Result;
            return Ok(configs);
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <param name="config">配置更新请求</param>
        /// <returns>更新结果</returns>
        [HttpPut]
        public ActionResult<bool> UpdateConfig([FromBody] ConfigEditRequest config)
        {
            _configServer.UpdateConfig(config);
            return Ok(true);
        }

        ///<summary>
        /// 根据配置类型获取配置
        ///</summary>
        [HttpGet("by-type/{type}")]
        public ActionResult<ConfigResponse> QueryByType([FromRoute] ConfigTypeEnum type)
        {
            ConfigResponse config=_configServer.QueryByType(type);
            return Ok(config);
        }
    }
}
