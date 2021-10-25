using Base.AuthorizationRequirements;
using Base.Controllers;
using Base.CustomPolicyProvider;
using Base.Transformer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
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
                config.AddPolicy("Claim.DoB", policyBuilder =>
                {
                    policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
                });
                //2.3) Дополнительные политики проверки взамен ролей можно использовать
                //config.AddPolicy("Admin", policyBuilder =>
                //{
                //    policyBuilder.RequireClaim(ClaimTypes.Role, "Admin");
                //});
            });

            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();

            //Для поверки Claim из шага 2.1 и 2.2 и если нет то запрет доступа...
            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
            //Для поверки Claim из шага 2.1 и 2.2 и если нет то запрет доступа...
            services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandler>();

            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

            services.AddControllersWithViews(config =>
            {
                //1) Добавление глобальной проверки что все имеют Юзера
                var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                var defaultAuthPolicy = defaultAuthBuilder
                        .RequireAuthenticatedUser()
                        .Build();
                // Фильтр глобальный на все контролеры кроме помеченных как AllowAnonymous
                //config.Filters.Add(new AuthorizeFilter(defaultAuthPolicy));
            });
            //Добавление RazorPages
            services.AddRazorPages()
                .AddRazorPagesOptions(config =>
                {
                    config.Conventions.AuthorizePage("/Razor/Secured");
                    config.Conventions.AuthorizePage("/Razor/Policy", DynamicPolicies.Rank);//непонятно почему не работает
                    config.Conventions.AuthorizeFolder("/RazorSecured");
                    config.Conventions.AllowAnonymousToPage("/RazorSecured/Anon");
                });
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
                endpoints.MapRazorPages();
            });
        }
    }
}
