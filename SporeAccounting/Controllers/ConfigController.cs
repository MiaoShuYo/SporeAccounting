using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SporeAccounting.BaseModels;
using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.MQ;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Controllers
{
    /// <summary>
    /// 用户配置控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Consumer,Administrator")]
    public class ConfigController : BaseController
    {
        /// <summary>
        /// 用户配置服务
        /// </summary>
        private readonly IConfigServer _configServer;

        /// <summary>
        /// 映射
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// 发布消息
        /// </summary>
        private readonly RabbitMQPublisher _rabbitMqPublisher;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configServer"></param>
        /// <param name="mapper"></param>
        /// <param name="rabbitMqPublisher"></param>
        public ConfigController(IConfigServer configServer, IMapper mapper, RabbitMQPublisher rabbitMqPublisher)
        {
            _configServer = configServer;
            _mapper = mapper;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        /// <summary>
        /// 查询用户配置
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Query/{configId}")]
        public ActionResult<ResponseData<ConfigViewModel>> Query(string configId)
        {
            try
            {
                Config? config = _configServer.Query(configId);
                ConfigViewModel configViewModel = _mapper.Map<ConfigViewModel>(config);
                return Ok(new ResponseData<ConfigViewModel>(HttpStatusCode.OK, data: configViewModel));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常"));
            }
        }

        /// <summary>
        /// 更新用户配置
        /// </summary>
        /// <param name="configViewModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public ActionResult<ResponseData<bool>> Update([FromBody] ConfigViewModel configViewModel)
        {
            try
            {
                string userId = GetUserId();
                bool isExist = _configServer.IsExist(userId, configViewModel.Id);
                if (!isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, "用户配置不存在"));
                }

                _configServer.Update(userId, configViewModel.Id, configViewModel.Value);

                //如果切换的是主币种，那么就将以前的所有金额全部转换成新的主币种的金额
                _ = _rabbitMqPublisher.Publish<string>("UpdateConversionAmount", "UpdateConversionAmount", 
                    configViewModel.Value);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常"));
            }
        }
    }
}