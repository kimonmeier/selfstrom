using Grpc.Net.Client.Web;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Grpc.Core;

namespace SelfStrom.Client;

public static class ConfigureServices
{

    public static void AddGrpcService<TService>(this IServiceCollection services) where TService : Grpc.Core.ClientBase
    {
        services.AddSingleton(services =>
        {
            var baseUri = services.GetRequiredService<NavigationManager>().BaseUri;
            var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions
            {
                HttpHandler = new GrpcWebHandler(new HttpClientHandler())
            });
            return (TService)Activator.CreateInstance(typeof(TService), channel)!;
        });
    }
}
