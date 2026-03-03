using Microsoft.AspNetCore.Mvc;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;
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
        private readonly ContextSession _contextSession;

        /// <summary>
        /// 用户配置控制器构造函数
        /// </summary>
        /// <param name="configServer">用户配置服务</param>
        public ConfigController(IConfigServer configServer, ContextSession contextSession)
        {
            _configServer = configServer;
            _contextSession = contextSession;
        }

        /// <summary>
        /// 获取所有配置
        /// </summary>
        /// <returns>用户配置列表</returns>
        [HttpGet]
        public async Task<ActionResult<List<ConfigResponse>>> GetConfigs()
        {
            List<ConfigResponse> configs = await _configServer.GetConfig();
            return Ok(configs);
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <param name="config">配置更新请求</param>
        /// <returns>更新结果</returns>
        [HttpPut]
        public async Task<ActionResult<bool>> UpdateConfig([FromBody] ConfigEditRequest config)
        {
            await _configServer.UpdateConfig(config);
            return Ok(true);
        }

        ///<summary>
        /// 根据配置类型获取配置
        ///</summary>
        [HttpGet("by-type/{type}")]
        public async Task<ActionResult<ConfigResponse>> QueryByType([FromRoute] ConfigTypeEnum type)
        {
            ConfigResponse config = await _configServer.QueryByType(type);
            return Ok(config);
        }

        ///<summary>
        /// 根据配置类型和用户ID获取配置
        ///</summary>
        /// <param name="type">类型</param>
        /// <param name="userId">用户id</param>
        [HttpGet("by-type-and-user/{type}/{userId}")]
        public async Task<ActionResult<ConfigResponse>> QueryByTypeAndUserId([FromRoute] ConfigTypeEnum type,
            [FromRoute] long userId)
        {
            if (_contextSession.UserId <= 0 || _contextSession.UserId != userId)
            {
                throw new ForbiddenException("禁止读取其他用户配置");
            }

            ConfigResponse config = await _configServer.QueryByTypeAndUserId(type, userId);
            return Ok(config);
        }
    }
}