using AutoMapper;
using Microsoft.Extensions.Configuration;
using SelfStrom.Server.Configuration;
using SelfStrom.Server.Mappings.Workers;
using SelfStrom.Server.Repositories;
using SelfStrom.Server.Services;
using SelfStrom.Shared.Mapping;
using System.Reflection;
using System.Text;

namespace SelfStrom.Server;

public static class ConfigureServices
{
    public static void AddCustomAutoMapper(this IServiceCollection services)
    {
        services.AddSharedAutoMapper(typeof(Program).Assembly);

        services.AddTransient<WorkerStatusResolver>();
    }

    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddSingleton<WorkerStatusService>();
        services.AddSingleton<DeviceService>();
    }

    public static void AddRepostories(this IServiceCollection services)
    {
        services.AddTransient<HomeRepository>();
        services.AddTransient<RoomRepository>();
        services.AddTransient<WorkerRepository>();
        services.AddTransient<DeviceRepository>();
        services.AddTransient<DataPointRepository>();
    }

    public static void AddMail(this WebApplicationBuilder app)
    {
        MailConfiguration? mailConfiguration = app.Configuration.GetSection("Mail")?.Get<MailConfiguration>();
        WebsiteConfiguration? websiteConfiguration = app.Configuration.GetSection("Website")?.Get<WebsiteConfiguration>();


        if (mailConfiguration is null)
        {
            throw new Exception("The Mail section is missing in the configuration");
        }
        if (websiteConfiguration is null)
        {
            throw new Exception("The Website section is missing in the configuration");
        }

        app.Services
            .AddFluentEmail(mailConfiguration.Mail!, $"{websiteConfiguration.Name} - No-Reply")
            .AddMailKitSender(new FluentEmail.MailKitSmtp.SmtpClientOptions()
            {
                Server = mailConfiguration.Server,
                Port = mailConfiguration.Port,
                UseSsl = mailConfiguration.SSL,
                User = mailConfiguration.Username,
                Password = mailConfiguration.Password,
                RequiresAuthentication = true,
                PreferredEncoding = Encoding.UTF8.EncodingName,
            });
    }
}
