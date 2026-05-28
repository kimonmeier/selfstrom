using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using SelfStrom.Components;
using SelfStrom.Components.Account;
using SelfStrom.Server;
using SelfStrom.Server.Components;
using SelfStrom.Server.Components.Account;
using SelfStrom.Server.Configuration;
using SelfStrom.Server.Data;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Server.Services;
using SelfStrom.Server.Services.Website;
using SelfStrom.Server.Services.Workers;
using SelfStrom.Shared.Data;

var builder = WebApplication.CreateBuilder(args);
builder.AddMail();

builder.Configuration.AddCommandLine(args);
builder.Configuration.AddJsonFile("appsettings.json", true);
builder.Configuration.AddJsonFile("appsettings.Development.json", true);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingServerAuthenticationStateProvider>();
builder.Services.AddTransient<EmailSender>();
builder.Services.AddMudServices();
builder.Services.AddGrpc(options => {
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 2 * 1024 * 1024; // 2 MB
    options.MaxSendMessageSize = 5 * 1024 * 1024; // 5 MB
});
builder.Services.AddRepostories();
builder.Services.AddCustomAutoMapper();
builder.Services.AddCustomServices();

builder.Services.AddSingleton((services) => services.GetRequiredService<IConfiguration>().GetSection("Mail")?.Get<MailConfiguration>() ?? new MailConfiguration());
builder.Services.AddSingleton((services) => services.GetRequiredService<IConfiguration>().GetSection("Website")?.Get<WebsiteConfiguration>() ?? new WebsiteConfiguration());

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlite(connectionString));
builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
builder.Services.AddScoped<DbTransactionFactory>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddSignInManager()
	.AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, EmailSender>();

var app = builder.Build();

using(IServiceScope scope = app.Services.CreateScope())
{
	scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseGrpcWeb(new GrpcWebOptions
{
	DefaultEnabled = true,
});
// Website Services
app.MapGrpcService<HomeService>().EnableGrpcWeb();
app.MapGrpcService<RoomService>().EnableGrpcWeb();
app.MapGrpcService<WorkerService>().EnableGrpcWeb();
app.MapGrpcService<WebsiteDeviceService>().EnableGrpcWeb();

// Worker Services
app.MapGrpcService<PingService>().EnableGrpcWeb();
app.MapGrpcService<AvailableDeviceService>().EnableGrpcWeb();
app.MapGrpcService<DataSyncService>().EnableGrpcWeb();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(SelfStrom.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
