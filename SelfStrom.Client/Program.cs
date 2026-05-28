using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SelfStrom.Client;
using SelfStrom.Client.Services;
using SelfStrom.Shared.Services.Website;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddMudServices();
builder.Services.AddMudPopoverService();
builder.Services.AddTransient<ClipboardService>();
builder.Services.AddGrpcService<HomeService.HomeServiceClient>();
builder.Services.AddGrpcService<RoomService.RoomServiceClient>();
builder.Services.AddGrpcService<WorkerService.WorkerServiceClient>();
builder.Services.AddGrpcService<DeviceService.DeviceServiceClient>();

await builder.Build().RunAsync();
