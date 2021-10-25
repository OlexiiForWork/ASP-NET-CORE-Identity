using Base.CustomPolicyProvider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Base.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public HomeController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        [Authorize(Policy = "Claim.DoB")]
        public IActionResult SecretPolicy()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SecretRole()
        {
            return View("Secret");
        }


        [SecurityLevel(5)]
        public IActionResult SecretLevel()
        {
            return View("Secret");
        }

        [SecurityLevel(10)]
        public IActionResult SecretHigherLevel()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            var grandmaClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Bob"),
                new Claim(ClaimTypes.Email, "Bob@fmail.com"),
                new Claim(ClaimTypes.DateOfBirth, "11/10/2016"),//SecretPolicy()
                new Claim(ClaimTypes.Role, "Admin"),//SecretRole()
                new Claim(DynamicPolicies.SecurityLevel,"7"),
                new Claim("Grandma.Says", "Bob@fmail.com")
            };

            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, "Bob K Foo"),
                new Claim("DrivingLicense", "A+")
            };
            var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Grandma Identity");

            var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity, licenseIdentity });

            HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DoStuff(/*DependencyInjection*/[FromServices] IAuthorizationService authorizationService)
        {
            var builder = new AuthorizationPolicyBuilder("Schema");
            var customPolicy = builder.RequireClaim("Hello").Build();
            //var authResult = await _authorizationService.AuthorizeAsync(User, customPolicy);
            //При взятии переменой из араметра функции DependencyInjection
            var authResult = await authorizationService.AuthorizeAsync(User, customPolicy);
            if (authResult.Succeeded)
            {
                return View("Index");
            }

            return View("Index");
        }
    }
}
