using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //ServicePointManager.SecurityProtocol
            //ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            services.AddAuthentication(config =>
            {
                //Проверяем куки чтобы что ми зарегестрированы (Authenticate)
                config.DefaultAuthenticateScheme = "ClientCookie";
                //Когда логинимся то работаем с куками
                config.DefaultSignInScheme = "ClientCookie";
                //Используется для работы проверок разрешений
                config.DefaultChallengeScheme = "OurServer";
            })
                .AddCookie("ClientCookie")
                .AddOAuth("OurServer", confg =>
                {

                    confg.ClientId = "client_id";
                    confg.ClientSecret = "client_secret";
                    confg.CallbackPath = new PathString("/oauth/callback");//; // Это сылка на саму себя в миделвер но чтобы эта хрень работала нада HTTPS включеное покрайней мере для IIS
                    confg.AuthorizationEndpoint = "https://localhost:44393/oauth/Authorize";
                    confg.TokenEndpoint ="https://localhost:44393/oauth/token";

                    confg.SaveTokens = true;

                    confg.Events = new OAuthEvents
                    {
                        OnCreatingTicket = context =>
                        {
                            var accessToken = context.AccessToken;
                            var base64payload = accessToken.Split('.')[1];

                            var bytes = Convert.FromBase64String(base64payload);
                            var jsonPayload = Encoding.UTF8.GetString(bytes);
                            var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

                            foreach (var claim in claims)
                            {
                                context.Identity.AddClaim(new Claim(claim.Key, claim.Value));
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
            services.AddHttpClient();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            // Кто ты?
            app.UseAuthentication();
            // А у тебя есть доступ?
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
