using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Server.Helper;
using SelfStrom.Server.Repositories;
using SelfStrom.Shared.Data;
using SelfStrom.Shared.Services.Website;

namespace SelfStrom.Server.Services.Website;

internal class HomeService : Shared.Services.Website.HomeService.HomeServiceBase
{
    private readonly HomeRepository _homeRepository;
    private readonly DbTransactionFactory _transactionFactory;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeService(HomeRepository homeRepository, UserManager<ApplicationUser> userManager, IMapper mapper, DbTransactionFactory transactionFactory)
    {
        _homeRepository = homeRepository;
        _userManager = userManager;
        _mapper = mapper;
        _transactionFactory = transactionFactory;
    }

    public override async Task<ListHomeAnswer> ListByUser(ListByUserRequest request, ServerCallContext context)
    {
        ApplicationUser? user = await context.GetAuthenticatedUser(_userManager);

        if (user == null)
        {
            return new ListHomeAnswer();
        }

        List<Home> homes = await _homeRepository.ListByUserAsync(user.Id);
        ListHomeAnswer answer = new ListHomeAnswer();
        answer.Homes.AddRange(_mapper.Map<List<HomeProto>>(homes));

        return answer;
    }

    public override async Task<HomeChangeResponse> CreateHome(HomeCreateRequest request, ServerCallContext context)
    {
        ApplicationUser? user = await context.GetAuthenticatedUser(_userManager);

        if (user == null)
        {
            return new HomeChangeResponse()
            {
                Success = false,
                ErrorMessage = "The User is not authenticated"
            };
        }

        HomeChangeResponse changeResponse = new HomeChangeResponse()
        {
            Success = true
        };
        using (var dbTransaction = _transactionFactory.CreateTransaction())
        {
            int currentCount = await _homeRepository.CountByUserAsync(user.Id);

            await _homeRepository.AddAsync(new Home()
            {
                Id = Guid.NewGuid(),
                Number = currentCount + 1,
                Name = request.Name,
                Description = request.Description,
                UserId = user.Id,
            });

            await dbTransaction.Commit(context.CancellationToken);
        }


        return changeResponse;
    }

    public override async Task<HomeChangeResponse> DeleteHome(DeleteHomeRequest request, ServerCallContext context)
    {
        ApplicationUser? user = await context.GetAuthenticatedUser(_userManager);

        if (user == null)
        {
            return new HomeChangeResponse()
            {
                Success = false,
                ErrorMessage = "The User is not authenticated"
            };
        }

        HomeChangeResponse changeResponse = new HomeChangeResponse()
        {
            Success = true
        };
        using (var dbTransaction = _transactionFactory.CreateTransaction())
        {
            await _homeRepository.RemoveAsync(Guid.Parse(request.HomeId));

            await dbTransaction.Commit(context.CancellationToken);
        }

        return changeResponse;
    }

    public override async Task<HomeChangeResponse> EditHome(HomeChangeRequest request, ServerCallContext context)
    {
        ApplicationUser? user = await context.GetAuthenticatedUser(_userManager);

        if (user == null)
        {
            return new HomeChangeResponse()
            {
                Success = false,
                ErrorMessage = "The User is not authenticated"
            };
        }

        HomeChangeResponse changeResponse = new HomeChangeResponse()
        {
            Success = true
        };
        using (var dbTransaction = _transactionFactory.CreateTransaction())
        {
            Home? home = await _homeRepository.FindByEntityAsync(Guid.Parse(request.Id));

            if (home == null)
            {
                return new HomeChangeResponse()
                {
                    Success = false,
                    ErrorMessage = $"Home with Id {request.Id} not found"
                };
            }

            _mapper.Map(request, home);

            await _homeRepository.UpdateAsync(home);

            await dbTransaction.Commit(context.CancellationToken);
        }

        return changeResponse;
    }
}
