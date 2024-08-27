using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SporeAccounting.BaseModels;
using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class SysUserController : ControllerBase
    {
        private readonly ISysUserServer _sysUserServer;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public SysUserController(ISysUserServer sysUserServer, IMapper mapper, IConfiguration config)
        {
            _sysUserServer = sysUserServer;
            _mapper = mapper;
            _config = config;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="sysUserViewModel"></param>
        /// <returns></returns>
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
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, "", false));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务端异常", false));
            }
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Login/{userName}/{password}")]
        public ActionResult<ResponseData<ResponseTokenViewModel>> Login([FromRoute] string userName, [FromRoute] string password)
        {
            try
            {
                //验证用户
                SysUser sysUser = _sysUserServer.Get(userName);
                if (sysUser == null)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.BadRequest, "用户或密码错误！", false));
                }
                string passwordHash = HashPasswordWithSalt(password, sysUser.Salt);
                //验证密码
                if (sysUser.Password != passwordHash)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.OK, "用户或密码错误！", false));
                }
                //生成Token和刷新Token
                ResponseTokenViewModel sysToken = new ResponseTokenViewModel();
                sysToken.Token= GenerateToken(sysUser.Id);
                sysToken.RefresToken = GenerateRefreshToken();
                return Ok(new ResponseData<ResponseTokenViewModel>(HttpStatusCode.OK,data: sysToken));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务端异常", false));
            }
        }
        
        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RetrievePassword/{userName}/{email}")]
        public ActionResult<ResponseData<string>> RetrievePassword([FromRoute] string userName, [FromRoute] string email)
        {
            try
            {
                //验证用户是否存在
                SysUser sysUser = _sysUserServer.Get(userName);
                if (sysUser == null)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.BadRequest, "用户不存在！", false));
                }
                if (sysUser.Email != email)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.BadRequest, "邮箱不正确！", false));
                }
                //生成12位随机密码
                string newPassword = GenerateRandomPassword(12);
                sysUser.Password= HashPasswordWithSalt(newPassword,sysUser.Salt);
                _sysUserServer.Update(sysUser);

                return Ok(new ResponseData<string>(HttpStatusCode.OK, data: newPassword));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务端异常", false));
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
        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GenerateToken(string userId)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:IssuerSigningKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            int tokenExpirces =int.Parse(_config["JWT:Expirces"]);
            var token = new JwtSecurityToken(
                issuer: _config["JWT:ValidIssuer"],
                audience: _config["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(tokenExpirces), // Token 有效期
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        /// <summary>
        /// 生成刷新Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        /// <summary>
        /// 随机密码生成
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            StringBuilder password = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] buffer = new byte[sizeof(uint)];
                while (password.Length < length)
                {
                    rng.GetBytes(buffer);
                    uint num = BitConverter.ToUInt32(buffer, 0);
                    password.Append(validChars[(int)(num % (uint)validChars.Length)]);
                }
            }
            return password.ToString();
        }


    }
}
