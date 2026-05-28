using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Server.Helper;
using SelfStrom.Server.Repositories;
using SelfStrom.Shared.Data;
using SelfStrom.Shared.Services.Website;
using System.Text;

namespace SelfStrom.Server.Services.Website;

public class WorkerService : Shared.Services.Website.WorkerService.WorkerServiceBase
{
    private readonly DbTransactionFactory _dbTransactionFactory;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly WorkerRepository _workerRepository;

    public WorkerService(DbTransactionFactory dbTransactionFactory, IMapper mapper, UserManager<ApplicationUser> userManager, WorkerRepository workerRepository)
    {
        _dbTransactionFactory = dbTransactionFactory;
        _mapper = mapper;
        _userManager = userManager;
        _workerRepository = workerRepository;
    }

    public override async Task<ListWorkerAnswer> ListByUser(Empty request, ServerCallContext context)
    {
        ApplicationUser? user = await context.GetAuthenticatedUser(_userManager);

        if (user == null)
        {
            return new ListWorkerAnswer();
        }

        var answer = new ListWorkerAnswer();
        var workers = await _workerRepository.ListByUserAsync(user.Id);

        answer.Worker.Add(_mapper.Map<List<WorkerProto>>(workers));

        return answer;
    }

    public override async Task<WorkerDeleteResponse> DeleteWorker(DeleteWorkerRequest request, ServerCallContext context)
    {
        ApplicationUser? user = await context.GetAuthenticatedUser(_userManager);

        if (user == null)
        {
            return new WorkerDeleteResponse()
            {
                Success = false,
                Message = "The User is not authenticated"
            };
        }

        WorkerDeleteResponse deleteResponse = new WorkerDeleteResponse()
        {
            Success = true
        };
        using (var dbTransaction = _dbTransactionFactory.CreateTransaction())
        {
            await _workerRepository.RemoveAsync(Guid.Parse(request.WorkerId));

            await dbTransaction.Commit(context.CancellationToken);
        }

        return deleteResponse;
    }

    public override async Task<WorkerCreateResponse> CreateWorker(WorkerCreateRequest request, ServerCallContext context)
    {
        ApplicationUser? user = await context.GetAuthenticatedUser(_userManager);

        if (user == null)
        {
            return new WorkerCreateResponse()
            {
                Success = false,
            };
        }


        Guid workerGuid = Guid.NewGuid();
        string apiKey = GenerateRandomApiKey(workerGuid);
        WorkerCreateResponse createResponse = new WorkerCreateResponse()
        {
            Success = true,
            ApiKey = apiKey,
        };

        using (DbTransaction transaction = _dbTransactionFactory.CreateTransaction())
        {
            int currentCount = await _workerRepository.CountByUserAsync(user.Id);

            await _workerRepository.AddAsync(new Worker()
            {
                Id = workerGuid,
                Number = currentCount + 1,
                WorkerName = request.Name,
                HomeId = Guid.Parse(request.HomeId),
                ApiKey = HashHelper.Hash(apiKey),
            });

            await transaction.Commit(context.CancellationToken);
        }

        return createResponse;
    }

    private static string GenerateRandomApiKey(Guid workerId, int size = 20)
    {
        string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
        Random random = new Random();

        StringBuilder builder = new StringBuilder();

        builder.Append("api");
        builder.Append(':');
        builder.Append(workerId.ToString());
        builder.Append(':');

        for (int i = 0; i < size; i++)
        {
            builder.Append(validChars[random.Next(0, validChars.Length)]);
        }
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(builder.ToString()));
    }
}
