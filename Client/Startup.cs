﻿using Client.Authorization;
using Client.Clients;
using Client.Middleware;
using Client.OptionModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDefaultIdentity<IdentityUser>()
                .AddDefaultUI(UIFramework.Bootstrap4);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<Services>(Configuration.GetSection(nameof(Services)));
            services.AddTransient<IAuthorizationClient, AuthorizationClient>();
            services.AddScoped<IBankClient, BankClient>();
            services.AddScoped<IStockShareRequesterClient, StockShareRequesterClient>();
            services.AddScoped<IStockShareProviderClient, StockShareProviderClient>();
            services.AddScoped<IPublicShareOwnerControlClient, PublicShareOwnerControlClient>();
            services.AddScoped<IStockTraderBrokerClient, StockTraderBrokerClient>();
            services.AddScoped<IHistoryClient, HistoryClient>();


            var authorizationService = services.BuildServiceProvider().GetService<IOptionsMonitor<Services>>()
                .CurrentValue.AuthorizationService;
            AddAuthenticationAndAuthorization(services, authorizationService);

            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.ExceptionMiddleware();
            }
            
            app.JwtMiddleware();
            SetupReadyAndLiveHealthChecks(app);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void SetupReadyAndLiveHealthChecks(IApplicationBuilder app)
        {
            app.UseHealthChecks("/health/ready", new HealthCheckOptions()
            {
                // Exclude all checks and return a 200-Ok.
                Predicate = (_) => false
            });
            app.UseHealthChecks("/health/live", new HealthCheckOptions()
            {
                // Exclude all checks and return a 200-Ok.
                Predicate = (_) => false
            });
        }

        private void AddAuthenticationAndAuthorization(IServiceCollection services,
            AuthorizationService authorizationService)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authorizationService.BaseAddress;
                    options.Audience = "Client";
                    options.SaveToken = true;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("client.UserActions", policy =>
                    policy.Requirements.Add(new UserHasRequirement("client.UserActions", authorizationService.BaseAddress)));
                options.AddPolicy("client.BusinessActions", policy =>
                    policy.Requirements.Add(new UserHasRequirement("client.BusinessActions", authorizationService.BaseAddress, "StockProvider")));
            });
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

        }
    }
}
