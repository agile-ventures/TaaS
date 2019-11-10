using System;
using System.Net.Http;
using AgileVentures.TezPusher.Web.Configurations;
using AgileVentures.TezPusher.Web.HttpClients;
using AgileVentures.TezPusher.Web.Hubs;
using AgileVentures.TezPusher.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgileVentures.TezPusher.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (string.IsNullOrEmpty(Configuration["Tezos:NodeUrl"]))
            {
                // User-Secrets: https://docs.asp.net/en/latest/security/app-secrets.html
                // See below for registration instructions for each provider.
                throw new InvalidOperationException("Tezos NodeUrl must be configured in ENV variables.");
            }

            services.AddCors(
                options => options.AddPolicy("AllowCors",
                    builder =>
                    {
                        builder
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowed(isOriginAllowed => true);
                    })
            );
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddHttpContextAccessor();
            services.AddOptions();
            services.AddLogging();
            services.AddOptions<TezosConfig>().Bind(Configuration.GetSection("Tezos")).ValidateDataAnnotations();
            services.AddHttpClient<TezosMonitorClient>().ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler { AllowAutoRedirect = false, MaxAutomaticRedirections = 20 }
            );
            services.AddSingleton<IPushService, PushService>();
            services.AddHostedService<TezosMonitorService>();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowCors");
            app.UseSignalR(routes =>
            {
                routes.MapHub<TezosHub>("/tezosHub");
            });
            app.UseMvc();

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }
    }
}
