using IdentityExample.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly AppDbContext dbContext;

        public HomeController(UserManager<IdentityUser> userManager,SignInManager<IdentityUser>  signInManager, AppDbContext dbContext)
        {

            this.userManager = userManager;
            this.signInManager = signInManager;
            this.dbContext = dbContext;
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
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {

            //login functionality
            var user = await userManager.FindByNameAsync(username);

            if(user != null)
            {
                var signInResult = await signInManager.PasswordSignInAsync(user, password, false, false);
                //sign in
                if(signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Register()
        {


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {

            //Register functionality
            var user = new IdentityUser
            {
                UserName = username,
                Email = ""
            };
            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                //sign user here
                var signInResult = await signInManager.PasswordSignInAsync(user, password, false, false);
                //sign in
                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> LogOut()
        {
            //sign Out user here
            await signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}
