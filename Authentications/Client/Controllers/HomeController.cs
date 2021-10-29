using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var apiResponse = await client.GetAsync("https://localhost:44366/secret/index");

            var serverResponse = await client.GetAsync("https://localhost:44382/secret/index");


            //var serverResponse = await AccessTokenRefreshWrapper(
            //    () => SecuredGetRequest("https://localhost:44382/secret/index"));

            //var apiResponse = await AccessTokenRefreshWrapper(
            //    () => SecuredGetRequest("https://localhost:44366/secret/index"));

            return View();
        }

        private async Task<HttpResponseMessage> SecuredGetRequest(string url)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            return await client.GetAsync(url);
        }

        public async Task<HttpResponseMessage> AccessTokenRefreshWrapper(
    Func<Task<HttpResponseMessage>> initialRequest)
        {
            var response = await initialRequest();

            //if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            //{
            //    await RefreshAccessToken();
            //    response = await initialRequest();
            //}

            return response;
        }
    }
}
