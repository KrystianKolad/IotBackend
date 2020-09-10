using System;
using System.IO;
using System.IO.Compression;
using IotBackend.Api.Infrastructure.Builders;
using IotBackend.Api.Infrastructure.Configuration;
using IotBackend.Api.Infrastructure.Handlers;
using IotBackend.Api.Infrastructure.Parsers;
using IotBackend.Api.Infrastructure.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IotBackend.Api.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddParsers(this IServiceCollection services)
        {
            services.AddTransient<IParser, HumidityParser>();
            services.AddTransient<IParser, RainfallParser>();
            services.AddTransient<IParser, TemperatureParser>();
            return services;
        }

        public static IServiceCollection AddProviders(this IServiceCollection services)
        {
            services.AddSingleton<IBlobClientProvider, BlobClientProvider>();
            services.AddSingleton<Func<Stream, StreamReader>>(stream => new StreamReader(stream));
            services.AddSingleton<Func<Stream, ZipArchive>>(stream => new ZipArchive(stream));
            
            return services;
        }

        public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BlobConfiguration>(configuration.GetSection("BlobConfiguration"));
            
            return services;
        }

        public static IServiceCollection AddBuilders(this IServiceCollection services)
        {
            services.AddSingleton<IFilePathBuilder, FilePathBuilder>();
            
            return services;
        }

        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            services.AddScoped<IDevicesHandler, DevicesHandler>();
            
            return services;
        }
    }
}