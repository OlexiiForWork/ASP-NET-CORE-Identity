using Base.AuthorizationRequirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace Base
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("CookieAuth")
                    .AddCookie("CookieAuth", config =>
                   {
                       config.Cookie.Name = "Oleksii.Cookie";
                       config.LoginPath = "/Home/Authenticate";
                   });

            services.AddAuthorization(config =>
            {
                //1) Клейм который мы хотим проверить по дефолту...
                //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                //var defaultAuthPolicy = defaultAuthBuilder
                //        .RequireAuthenticatedUser()
                //        .RequireClaim(ClaimTypes.DateOfBirth)
                //        .Build();
                //config.DefaultPolicy = defaultAuthPolicy;
                
                //2.1) Дополнительные политики проверки 
                //config.AddPolicy("Claim.DoB",policyBuilder => {
                //    policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
                //});
                //2.2) Другой вариант построения через рашыряющий метод
                config.AddPolicy("Claim.DoB", policyBuilder => {
                    policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
                });
                //2.3) Дополнительные политики проверки взамен ролей можно использовать
                //config.AddPolicy("Admin", policyBuilder =>
                //{
                //    policyBuilder.RequireClaim(ClaimTypes.Role, "Admin");
                //});
            });

            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();

            services.AddControllersWithViews();
        }

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
