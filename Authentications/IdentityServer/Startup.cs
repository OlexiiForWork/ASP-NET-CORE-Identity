using IdentityServer.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionPostgresString = _config.GetConnectionString("DefaultPostgresConnection");
            var assembly = typeof(Startup).Assembly.GetName().Name;

            services.AddDbContext<AppDbContext>(config =>
            {
                config.UseNpgsql(connectionPostgresString,
                    sql => sql.MigrationsAssembly(assembly));
                //config.UseInMemoryDatabase("Memory");
            });

            // AddIdentity registers the services
            services.AddIdentity<IdentityUser, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
                config.SignIn.RequireConfirmedEmail = false;
                config.User.AllowedUserNameCharacters = "йцукенгшщзхъфывапролджэячсмитьбюЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityServer.Cookie";
                config.LoginPath = "/Auth/Login";
                config.LogoutPath = "/Auth/Logout";
            });

            var filePath = Path.Combine(_env.ContentRootPath, "is_sert.pfx");
            var certificate = new X509Certificate2(filePath, "password");

            services.AddIdentityServer()
                .AddAspNetIdentity<IdentityUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(connectionPostgresString,
                        sql => sql.MigrationsAssembly(assembly));
                    options.DefaultSchema = "Configs";//Нужно если переопределена схема
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(connectionPostgresString,
                        sql => sql.MigrationsAssembly(assembly));
                    options.DefaultSchema = "Tokens";//Нужно если переопределена схема
                })
                .AddSigningCredential(certificate);
            //или
            //.AddInMemoryIdentityResources(Configuration.GetIdentityResources())//КОГДА ДОБАВЛЯЮ КАКИЕТО ПРОБЛЕМЫ
            //.AddInMemoryApiResources(Configuration.GetApis())
            //.AddInMemoryClients(Configuration.GetClients())
            //.AddInMemoryApiScopes(Configuration.GetScopes())
            //.AddDeveloperSigningCredential();

            services.AddAuthentication()
                    .AddGoogle(config =>
                    {
                        config.ClientId = "1070737759616-ofn5kqubjsv76ttompddidrc05t8cqpq.apps.googleusercontent.com";
                        config.ClientSecret = "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";
                    });


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

            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
