using IdentityExample.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Base
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(config=>{
                config.UseInMemoryDatabase("Memory");
            });

            services.AddIdentity<IdentityUser,IdentityRole>()
                //.AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            //services.AddAuthentication("CookieAuth")
            //        .AddCookie("CookieAuth", config =>
            //        {
            //            config.Cookie.Name = "Oleksii.Cookie";
            //            config.LoginPath = "/Home/Authenticate";
            //        });
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();


            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
