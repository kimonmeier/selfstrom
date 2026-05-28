using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Core;
using SelfStrom.Shared.Services.Worker;
using SelfStrom.Worker;
using SelfStrom.Worker.Data;
using Serilog;

using var log = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
    .WriteTo.Debug(Serilog.Events.LogEventLevel.Verbose)
    .WriteTo.File("log.txt",
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

log.Information("Starting SelfStrom Worker");

CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();


AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
AppDomain.CurrentDomain.ProcessExit += CurrentDomainProcessExit;
Console.CancelKeyPress += ConsoleCancelKeyPress;

void ConsoleCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
{
    log.Information("Stopping requested!");
    cancellationTokenSource.Cancel();

    e.Cancel = true;
}

void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    log.Error(e.ExceptionObject as Exception, "An unhandelded Exception occured");
}

void CurrentDomainProcessExit(object? sender, EventArgs e)
{
    log.Information("Stopping requested!");
    cancellationTokenSource.Cancel();
}

log.Information("Loading Configuration");
ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
configurationBuilder.AddJsonFile("appsettings.json", optional: true);
configurationBuilder.AddJsonFile("appsettings.Development.json", optional: true);
configurationBuilder.AddEnvironmentVariables();

IConfiguration configuration = configurationBuilder.Build();
log.Information("Loaded Configuration");

log.Information("Building Application");

IServiceCollection services = new ServiceCollection();
services.AddConfiguration(configuration);
services.AddLogging(builder => builder.AddSerilog(log));
services.AddSerilog(log);
services.AddBackgroundServices(configuration);
services.AddDatabase(configuration);
services.AddAutoMapper();

services.AddGrpcService<PingService.PingServiceClient>();
services.AddGrpcService<AvailableDevicesService.AvailableDevicesServiceClient>();
services.AddGrpcService<DataSyncService.DataSyncServiceClient>();

log.Information("Application was builded");

IServiceProvider provider = services.BuildServiceProvider();

log.Information("Starting Worker");

log.Debug("Updating Database");

provider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

log.Debug("Starting Background Tasks");
var scheduler = provider.GetRequiredService<ISchedulerFactory>().GetScheduler().ConfigureAwait(false).GetAwaiter().GetResult();
scheduler.Start().ConfigureAwait(false).GetAwaiter().GetResult();
while (!cancellationTokenSource.IsCancellationRequested)
{
    Task.Delay(100).Wait();
}
scheduler.Shutdown().ConfigureAwait(false).GetAwaiter().GetResult();

Log.Information("Shuting down worker!");

return;