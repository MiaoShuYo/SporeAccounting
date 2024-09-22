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
using SporeAccounting.BaseModels.ViewModel.Response;
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
        public ActionResult<ResponseData<TokenViewModel>> Login([FromRoute] string userName, [FromRoute] string password)
        {
            try
            {
                //验证用户
                SysUser sysUser = _sysUserServer.GetByUserName(userName);
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
                TokenViewModel sysToken = new TokenViewModel();
                sysToken.RefreshToken = GenerateRefreshToken();
                sysToken.Token = GenerateToken(sysUser.Id, sysToken.RefreshToken);
                return Ok(new ResponseData<TokenViewModel>(HttpStatusCode.OK, data: sysToken));
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
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RetrievePassword/{userName}/{email}")]
        public ActionResult<ResponseData<string>> RetrievePassword([FromRoute] string userName, [FromRoute] string email)
        {
            try
            {
                //验证用户是否存在
                SysUser sysUser = _sysUserServer.GetByUserName(userName);
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
                sysUser.Password = HashPasswordWithSalt(newPassword, sysUser.Salt);
                _sysUserServer.Update(sysUser);

                return Ok(new ResponseData<string>(HttpStatusCode.OK, data: newPassword));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务端异常", false));
            }
        }
        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RefreshToken/{refreshToken}")]
        public ActionResult<ResponseData<string>> RefreshToken([FromRoute] string refreshToken)
        {
            try
            {
                //获取token中的user id 和 refreshToken
                string token = HttpContext.Request.Headers["Authorization"].ToString()
                    .Substring("Bearer ".Length).Trim();
                (string userId, _, string reToken) = GetTokenInfo(token);
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(reToken) || refreshToken != reToken)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.BadRequest, "数据违规！", false));
                }
                //根据userid查询用户
                SysUser sysUser = _sysUserServer.GetById(userId);
                if (sysUser == null)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, "用户不存在！", false));
                }
                //使用刷新token刷新token
                string newToken = GenerateToken(userId, refreshToken);
                return Ok(new ResponseData<string>(HttpStatusCode.OK, data: newToken));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务端异常", false));
            }
        }
        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="userPage"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Query")]
        public ActionResult<ResponseData<PageResponseViewModel<SysUserViewModel>>> Query([FromBody] UserPageViewModel userPage)
        {
            try
            {
                (int rowCount, int pageCount, List<SysUser> sysUsers) = _sysUserServer.GetByPage(userPage);
                List<SysUserViewModel> sysUsersView = _mapper.Map<List<SysUserViewModel>>(sysUsers);
                PageResponseViewModel<SysUserViewModel>
                    pageResponseView = new PageResponseViewModel<SysUserViewModel>();
                pageResponseView.Data = sysUsersView;
                pageResponseView.PageCount = pageCount;
                pageResponseView.RowCount = rowCount;
                return Ok(new ResponseData<PageResponseViewModel<SysUserViewModel>>(HttpStatusCode.OK, data: pageResponseView));

            }
            catch (Exception ex)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务端异常", false));
            }
        }
        /// <summary>
        /// 删除用户（逻辑删除）
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Remove/{userId}")]
        public ActionResult<ResponseData<bool>> Remove([FromRoute] string userId)
        {
            try
            {
                bool exist = UserExist(userId);
                if (exist)
                {
                    _sysUserServer.Delete(userId);
                    return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
                }
                else
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, $"用户 {userId} 不存在", false));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常", false));
            }
        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="sysUserEditView"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Edit")]
        public ActionResult<ResponseData<bool>> Edit([FromBody] SysUserEditViewModel sysUserEditView)
        {
            try
            {
                bool exist = UserExist(sysUserEditView.Id);
                if (exist)
                {
                    SysUser sysUser = _sysUserServer.GetById(sysUserEditView.Id);
                    sysUser.Email = sysUserEditView.Email;
                    sysUser.PhoneNumber = sysUserEditView.PhoneNumber;
                    sysUser.UserName = sysUserEditView.UserName;
                    _sysUserServer.Update(sysUser);
                    return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, data: true));
                }
                else
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, $"用户 {sysUserEditView.Id} 不存在", false));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常", false));
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
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        private string GenerateToken(string userId, string refreshToken)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("refreshToken",refreshToken)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:IssuerSigningKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            int tokenExpirces = int.Parse(_config["JWT:Expirces"]);
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
        /// <summary>
        /// 获取token中的userid
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private (string, string, string) GetTokenInfo(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            // 验证令牌格式
            if (!handler.CanReadToken(token))
            {
                throw new ArgumentException("无效的令牌");
            }
            // 读取令牌
            var jwtToken = handler.ReadJwtToken(token);
            // 从声明中提取用户ID（通常是“sub”声明）
            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub");
            var jtiClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "jti");
            var refreshTokenClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "refreshToken");
            return (userIdClaim?.Value, jtiClaim.Value, refreshTokenClaim.Value);
        }
        /// <summary>
        /// //判断用户是否存在
        /// </summary>
        /// <returns></returns>

        private bool UserExist(string userId)
        {
            try
            {
                bool exist = _sysUserServer.GetById(userId) == null;
                return exist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
