using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Base.Controllers
{
    public class OperationsController : Controller
    {
        private readonly IAuthorizationService authorizationService;

        public OperationsController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        public async Task<IActionResult> Open()
        {
            // Получить предварительно данные с БД
            var cookieJar = new CookieJar();//get cookie Jar from db

            //var requerement = new OperationAuthorizationRequirement()
            //{
            //    Name = CookieOleksiiOperations.Open
            //};
            //await authorizationService.AuthorizeAsync(User, cookieJar, requerement);
            await authorizationService.AuthorizeAsync(User, cookieJar, CookieJarAuthOperations.Open);
            return View();
        }
    }
    //Что на подобе центрального места хранения логики доступа
    public class CookieJarAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, CookieJar>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            CookieJar resource)
        {
            if (requirement.Name == CookieJarOperations.Look)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    context.Succeed(requirement);
                }
            }
            else if (requirement.Name == CookieJarOperations.ComeNear)
            {
                if (context.User.HasClaim("Friend", "Good"))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
    public static class CookieJarAuthOperations
    {
        public static OperationAuthorizationRequirement Open => new OperationAuthorizationRequirement
        {
            Name = CookieJarOperations.Open
        };
    }
    public static class CookieJarOperations
    {
        public static string Open = "Open";
        public static string TakeCookie = "TakeCookie";
        public static string ComeNear = "ComeNear";
        public static string Look = "Look";
    }

    public  class CookieJar
    {
        public string Name { get; set; }
    }
}
