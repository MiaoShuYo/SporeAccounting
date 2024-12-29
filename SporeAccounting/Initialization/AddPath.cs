using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SporeAccounting.Controllers;
using SporeAccounting.Models;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Initialization;

/// <summary>
/// 新增web api路径
/// </summary>
public static class AddPath
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static void Init(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var sysUrlServer = scope.ServiceProvider.GetRequiredService<ISysUrlServer>();
            List<SysUrl> sysUrls = new List<SysUrl>();
            //1. 通过反射获取所有的控制器
            var controllers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => typeof(BaseController).IsAssignableFrom(type));
            //2. 获取控制器的Route特性
            foreach (var controller in controllers)
            {
                var routeAttribute = controller.GetCustomAttribute<RouteAttribute>();
                if (routeAttribute != null)
                {
                    //3. 根据特性生成完整的路径，
                    //如果路径中包含[controller]，则替换为控制器的名称，反之直接使用路径
                    var controllerName = routeAttribute.Template;
                    if (controllerName.Contains("[controller]"))
                    {
                        controllerName =
                            controllerName.Replace("[controller]", controller.Name.Replace("Controller", ""));
                    }

                    //4. 获取controller的所有Action
                    var actions = controller.GetMethods()
                        .Where(method => method.IsPublic && !method.GetCustomAttributes<NonActionAttribute>().Any());
                    for (int i = 0; i < actions.Count(); i++)
                    {
                        var action = actions.ElementAt(i);
                        var actionRouteAttribute = action.GetCustomAttribute<RouteAttribute>();
                        if (actionRouteAttribute != null)
                        {
                            var actionName = actionRouteAttribute.Template;
                            if (actionName.Contains("[action]"))
                            {
                                actionName = actionName.Replace("[action]", action.Name);
                            }

                            actionName = actionName.Split("/")[0];
                            var httpMethod = action.GetCustomAttributes<HttpMethodAttribute>().FirstOrDefault()
                                ?.HttpMethods.FirstOrDefault() ?? "GET";
                            var route = $"/{controllerName}/{actionName}".Replace("//", "/");
                            bool isExist = sysUrlServer.IsExist(route, httpMethod);
                            if (isExist)
                            {
                                continue;
                            }
                            var sysUrl = new SysUrl()
                            {
                                Url = route,
                                IsDeleted = false,
                                Description = "",
                                RequestMethod = httpMethod,
                                CreateUserId = "b47637e2-603f-4df0-abe9-88d70fa870ee"
                            };
                            sysUrls.Add(sysUrl);
                        }
                    }
                }
            }

            //5. 将路径添加到数据库
            sysUrlServer.Add(sysUrls);
        }
    }
}