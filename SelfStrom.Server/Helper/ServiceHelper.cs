using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Server.Repositories;
using System.Text;

namespace SelfStrom.Server.Helper;

public static class ServiceHelper
{
    public static async Task<ApplicationUser?> GetAuthenticatedUser(this ServerCallContext context, UserManager<ApplicationUser> userManager)
    {
        ArgumentNullException.ThrowIfNull(context);

        var httpContext = context.GetHttpContext();

        if (httpContext is null)
        {
            throw new Exception("Http Context is null");
        }
        return await userManager.GetUserAsync(httpContext.User);
    }

    public static async Task<Worker?> GetWorkerAsync(this ServerCallContext context, WorkerRepository workerRepository)
    {
        ArgumentNullException.ThrowIfNull(context);

        var apikeyEncoded = context.RequestHeaders.GetValue("apikey");

        if (apikeyEncoded is null)
        {
            return null;
        }
        string apiKey = Encoding.UTF8.GetString(Convert.FromBase64String(apikeyEncoded));
        string? workerId = apiKey.Split(':').ElementAtOrDefault(1);

        if (workerId is null) {
            return null;
        }

        if (!Guid.TryParse(workerId, out Guid workerGuid))
        {
            return null;
        }

        Worker? worker = await workerRepository.FindByEntityAsync(workerGuid);

        if (worker is null)
        {
            return null;
        }

        if (!HashHelper.Verify(worker.ApiKey, apikeyEncoded))
        {
            return null;
        }

        return worker;
    }
}
