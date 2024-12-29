using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SporeAccounting.Controllers;
using SporeAccounting.Models;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Initialization;

/// <summary>
/// 添加角色路径
/// </summary>
public static class AddRolePath
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static void Init(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var sysRoleUrlService = scope.ServiceProvider.GetService<ISysRoleUrlServer>();
            //1. 获取所有的URL
            var sysUrlServer = scope.ServiceProvider.GetService<ISysUrlServer>();
            var urls = sysUrlServer?.Query();
            //2. 获取所有的角色
            var roleService = scope.ServiceProvider.GetService<ISysRoleServer>();
            var roles = roleService.Query();
            //3. 解析每个Controller的Authorize特性中Roles的角色
            var controllerRoles = new Dictionary<string, List<string>>();
            foreach (var controller in Assembly.GetExecutingAssembly().GetTypes()
                         .Where(t => typeof(ControllerBase).IsAssignableFrom(t)))
            {
                var authorizeAttributes = controller.GetCustomAttributes<AuthorizeAttribute>();
                foreach (var attribute in authorizeAttributes)
                {
                    var controllerName = controller.Name.Replace("Controller", "");
                    if (!controllerRoles.ContainsKey(controllerName))
                    {
                        controllerRoles[controllerName] = new List<string>();
                    }

                    controllerRoles[controllerName].AddRange(attribute.Roles.Split(','));
                }
            }

            List<SysRoleUrl> sysRoleUrls = new List<SysRoleUrl>();
            //4. 解析每个Controller的Action的特性如果有AllowAnonymous则跳过，反之拼接Controller和Action生产接口地址
            foreach (var controller in Assembly.GetExecutingAssembly().GetTypes()
                         .Where(type => typeof(BaseController).IsAssignableFrom(type)))
            {
                var actions = controller.GetMethods()
                    .Where(m => m.IsPublic && !m.GetCustomAttributes<NonActionAttribute>().Any());

                foreach (var action in actions)
                {
                    if (action.GetCustomAttributes<AllowAnonymousAttribute>().Any())
                    {
                        continue;
                    }

                    var routeAttribute = controller.GetCustomAttribute<RouteAttribute>();
                    var actionRouteAttribute = action.GetCustomAttribute<RouteAttribute>();
                    if (routeAttribute == null || actionRouteAttribute == null)
                    {
                        continue;
                    }

                    var controllerRoute = routeAttribute.Template;
                    var controllerName = controller.Name.Replace("Controller", "");
                    if (controllerRoute.Contains("[controller]"))
                    {
                        controllerRoute =
                            controllerRoute.Replace("[controller]", controllerName);
                    }

                    var actionName = actionRouteAttribute.Template;
                    if (actionName.Contains("[action]"))
                    {
                        actionName = actionName.Replace("[action]", action.Name);
                    }

                    actionName = actionName.Split("/")[0];
                    var route = $"/{controllerRoute}/{actionName}".Replace("//", "/");
                    controller.Name.Replace("Controller", "");
                    foreach (var kv in controllerRoles)
                    {
                        if (kv.Key == controllerName)
                        {
                            foreach (var role in kv.Value)
                            {
                                var roleId = roles?.FirstOrDefault(x => x.RoleName == role)?.Id;
                                var urlId = urls?.FirstOrDefault(x => x.Url == route)?.Id;
                                bool isExist = sysRoleUrlService.IsExist(roleId, urlId);
                                if(isExist)
                                {
                                    continue;
                                }
                                SysRoleUrl sysRoleUrl = new SysRoleUrl()
                                {
                                    UrlId = urlId,
                                    RoleId = roleId,
                                    CreateUserId = "b47637e2-603f-4df0-abe9-88d70fa870ee"
                                };
                                sysRoleUrls.Add(sysRoleUrl);
                            }
                        }
                    }
                }
            }

            //5. 将角色id和urlid添加到SysRoleUrl表中
            sysRoleUrlService.Add(sysRoleUrls);
        }
    }
}