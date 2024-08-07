using System.Net;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SporeAccounting.BaseModels;
using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SysUserController : ControllerBase
    {
        private readonly ISysUserServer _sysUserServer;
        private readonly IMapper _mapper;

        public SysUserController(ISysUserServer sysUserServer, IMapper mapper)
        {
            _sysUserServer = sysUserServer;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("Register")]
        public ActionResult<ResponseData<bool>> Register(SysUserViewModel sysUserViewModel)
        {
            try
            {
                SysUser sysUser = _mapper.Map<SysUser>(sysUserViewModel);
                sysUser.Salt = Guid.NewGuid().ToString("N");
                sysUser.Password = HashPasswordWithSalt(sysUser.Password, sysUser.Salt);
                sysUser.CreateUserId = sysUser.Id;
                _sysUserServer.Add(sysUser);
                return new ResponseData<bool>(HttpStatusCode.OK, "", false);
            }
            catch (Exception ex)
            {
                return new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务端异常", false);
            }
        }

        private static string HashPasswordWithSalt(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                string saltedPassword = password + salt;
                byte[] saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
                byte[] hashBytes = sha256.ComputeHash(saltedPasswordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
