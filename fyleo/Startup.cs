using System;
using System.Linq;
using fyleo.EventLog;
using fyleo.Repository.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace fyleo
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            services.Configure<AuthConfig>(Configuration.GetSection("AuthConfig"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddTransient<LanguageConfig>(x=> new LanguageConfig() { LangCode = "EN" });
            services.AddTransient<ITranslations, Translations>();
            services.AddSingleton<IEventLog, EventLogFile>();
            services.AddSingleton<IAuthRepository, AuthRepository>();
            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddSingleton<ISiteConfigRepository, SiteConfigRepository>();
            services.AddSingleton<IFileRepository, FileRepository>();
            services.AddSingleton<Account>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAuthRepository authRepository)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // Authentication
            app.Use(async (context, next) =>
            {
                if(context.Request.Path.Value.Equals("/login", StringComparison.OrdinalIgnoreCase))
                {
                    await next.Invoke();
                    return;
                }

                if(context.Request.Path.Value.Equals("/privacy", StringComparison.OrdinalIgnoreCase))
                {
                    await next.Invoke();
                    return;
                }

                var authCookie = context.Request.Cookies["authCookie"];
                if(string.IsNullOrEmpty(authCookie))
                {
                    var url = Uri.EscapeUriString($"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
                    if(!context.Request.Path.Value.StartsWith("Login"))
                    {
                        context.Response.Redirect($"Login?RedirectUrl={url}");
                    }
                    else
                    {
                        context.Response.Redirect($"Login");
                    }
                    return;
                }
                else
                {
                    var user = authCookie.Split(',').Single(x=>x.Contains("user:")).Split(':')[1];
                    var token = authCookie.Split(',').Single(x=>x.Contains("token:")).Split(':')[1];

                    var storedToken = authRepository.Get();
                    if(token !=  storedToken.Token)
                    {
                        context.Response.Redirect("Login");
                        return;
                    }

                    context.Items.Add("UserName", user);
                    context.Items.Add("AuthToken", token);
                }

                await next.Invoke();
            });

            // app.UseMvc();
            //app.UseAuthorization();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
