using System;
using System.IO;
using System.IO.Compression;
using IotBackend.Api.Infrastructure.Builders;
using IotBackend.Api.Infrastructure.Configuration;
using IotBackend.Api.Infrastructure.Extensions;
using IotBackend.Api.Infrastructure.Handlers;
using IotBackend.Api.Infrastructure.Models;
using IotBackend.Api.Infrastructure.Parsers;
using IotBackend.Api.Infrastructure.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IotBackend.Api
{
    public class Startup
    {
        private IConfiguration _configuration;
        public Startup(IWebHostEnvironment env)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = configurationBuilder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddConfiguration(_configuration)
                .AddParsers()
                .AddBuilders()
                .AddProviders()
                .AddHandlers();

            services.AddControllers();
            services.AddApiVersioning();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}