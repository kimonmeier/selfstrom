using Grpc.Net.Client.Web;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SelfStrom.Worker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfStrom.Worker.Configuration;
using Microsoft.Extensions.Options;
using Quartz;
using SelfStrom.Worker.Background;
using SelfStrom.Shared.Data;
using SelfStrom.Shared.Mapping;
using SelfStrom.Worker.Data.Repositories;
using Microsoft.Extensions.Logging;
using EFCoreSecondLevelCacheInterceptor;
using Grpc.Core;
using Grpc.Net.Client.Configuration;
using SelfStrom.Worker.Mapping.Converter;

namespace SelfStrom.Worker;

internal static class ConfigureServices
{
    public static void AddBackgroundServices(this IServiceCollection services, IConfiguration configuration)
    {
        // base configuration from appsettings.json
        services.Configure<QuartzOptions>(services => configuration.GetSection("Quartz").Get<QuartzOptions>());

        // if you are using persistent job store, you might want to alter some options
        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true; // default: false
            options.Scheduling.OverWriteExistingData = true; // default: true
        });

        services.AddQuartz(q =>
        {
            // handy when part of cluster or you want to otherwise identify multiple schedulers
            q.SchedulerId = "Selfstrom-Worker";

            q.UseSimpleTypeLoader();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10;
            });


            q.ScheduleJob<PingJob>(jobOptions => jobOptions.WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever()));
            q.ScheduleJob<SearchDeviceJob>(jobOptions => jobOptions.WithSimpleSchedule(x => x.WithIntervalInHours(1).RepeatForever()));
            q.ScheduleJob<UpdateLocalDataJob>(jobOptions => jobOptions.WithSimpleSchedule(x => x.WithIntervalInMinutes(15).RepeatForever()));
            q.ScheduleJob<CaptureDataPoints>(jobOptions => jobOptions.WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever()));
            q.ScheduleJob<SyncDataPointsJob>(jobOptions => jobOptions.WithSimpleSchedule(x => x.WithIntervalInMinutes(30).RepeatForever()));
        });

        services.AddTransient<PingJob>();
        services.AddTransient<SearchDeviceJob>();
        services.AddTransient<UpdateLocalDataJob>();
        services.AddTransient<CaptureDataPoints>();
    }

    public static void AddAutoMapper(this IServiceCollection services)
    {
        services.AddSharedAutoMapper(typeof(Program).Assembly);

        services.AddTransient<TimeStampConverter>();
    }

    public static void AddConfiguration(this IServiceCollection services,  IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddSingleton<WebsiteConfiguration>(configuration.GetSection("Website").Get<WebsiteConfiguration>() ?? throw new Exception("Website Configuration not found!"));
    }

    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });
        services.AddEFSecondLevelCache(options =>
                options.UseMemoryCacheProvider().ConfigureLogging(true).UseCacheKeyPrefix("EF_")
                       // Fallback on db if the caching provider fails.
                       .UseDbCallsIfCachingProviderIsDown(TimeSpan.FromMinutes(1)));

        services.AddScoped<DbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<DbTransactionFactory>();

        services.AddScoped<DataPointRepository>();
        services.AddScoped<ConnectedDevicesRepository>();
    }

    public static void AddGrpcService<TService>(this IServiceCollection services) where TService : Grpc.Core.ClientBase
    {
        var defaultMethodConfig = new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 5,
                InitialBackoff = TimeSpan.FromSeconds(1),
                MaxBackoff = TimeSpan.FromSeconds(10),
                BackoffMultiplier = 2,
                RetryableStatusCodes =
                {
                    StatusCode.Unavailable,
                    StatusCode.DeadlineExceeded,
                    StatusCode.Aborted,
                    StatusCode.ResourceExhausted,
                    StatusCode.Internal
                }
            }
        };
        
        services.AddSingleton(provider =>
        {
            WebsiteConfiguration config = provider.GetRequiredService<WebsiteConfiguration>();

            var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
            {
                BaseAddress = new Uri(config.BaseUrl),
                DefaultRequestHeaders =
                {
                    { "apikey", config.ApiKey }
                },
            };

            var channel = GrpcChannel.ForAddress(config.BaseUrl, new GrpcChannelOptions
            {
                HttpClient = httpClient,
                ServiceConfig = new ServiceConfig
                {
                    MethodConfigs = { defaultMethodConfig }
                }
            });
            return (TService)Activator.CreateInstance(typeof(TService), channel)!;
        });
    }
}
