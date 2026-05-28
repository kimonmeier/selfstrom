using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Server.Repositories;
using SelfStrom.Shared.Services.Website;
using Grpc.Core;
using SelfStrom.Server.Helper;
using SelfStrom.Shared.Data;

namespace SelfStrom.Server.Services.Website;


public class RoomService : Shared.Services.Website.RoomService.RoomServiceBase
{
    private readonly RoomRepository _roomRepository;
    private readonly DbTransactionFactory _transactionFactory;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoomService(UserManager<ApplicationUser> userManager, RoomRepository roomRepository, IMapper mapper, DbTransactionFactory transactionFactory)
    {
        _userManager = userManager;
        _roomRepository = roomRepository;
        _mapper = mapper;
        _transactionFactory = transactionFactory;
    }

    public override async Task<ListRoomAnswer> ListByHome(ListByHomeRequest request, ServerCallContext context)
    {
        ApplicationUser? user = await context.GetAuthenticatedUser(_userManager);

        if (user == null)
        {
            return new ListRoomAnswer();
        }

        List<Room> rooms = await _roomRepository.ListByHome(Guid.Parse(request.HomeId));
        ListRoomAnswer answer = new ListRoomAnswer();
        answer.Rooms.AddRange(_mapper.Map<List<RoomProto>>(rooms));

        return answer;
    }

    public override async Task<RoomChangeResponse> CreateRoom(RoomCreateRequest request, ServerCallContext context)
    {
        ApplicationUser? user = await context.GetAuthenticatedUser(_userManager);

        if (user == null)
        {
            return new RoomChangeResponse()
            {
                Success = false,
                Message = "The User is not authenticated"
            };
        }

        RoomChangeResponse changeResponse = new RoomChangeResponse()
        {
            Success = true
        };

        using (var dbTransaction = _transactionFactory.CreateTransaction())
        {
            int currentCount = await _roomRepository.CountByHomeAsync(Guid.Parse(request.HomeId));

            await _roomRepository.AddAsync(new Room()
            {
                Id = Guid.NewGuid(),
                Number = currentCount + 1,
                Name = request.Name,
                HomeId = Guid.Parse(request.HomeId),
            });

            await dbTransaction.Commit(context.CancellationToken);
        }

        return changeResponse;
    }

    public override async Task<RoomChangeResponse> DeleteRoom(DeleteRoomRequest request, ServerCallContext context)
    {
        ApplicationUser? user = await context.GetAuthenticatedUser(_userManager);

        if (user == null)
        {
            return new RoomChangeResponse()
            {
                Success = false,
                Message = "The User is not authenticated"
            };
        }

        RoomChangeResponse changeResponse = new RoomChangeResponse()
        {
            Success = true
        };
        using (var dbTransaction = _transactionFactory.CreateTransaction())
        {
            await _roomRepository.RemoveAsync(Guid.Parse(request.RoomId));

            await dbTransaction.Commit(context.CancellationToken);
        }

        return changeResponse;
    }

    public override async Task<RoomChangeResponse> EditRoom(RoomChangeRequest request, ServerCallContext context)
    {
        ApplicationUser? user = await context.GetAuthenticatedUser(_userManager);

        if (user == null)
        {
            return new RoomChangeResponse()
            {
                Success = false,
                Message = "The User is not authenticated"
            };
        }

        RoomChangeResponse changeResponse = new RoomChangeResponse()
        {
            Success = true
        };
        using (var dbTransaction = _transactionFactory.CreateTransaction())
        {
            Room? room = await _roomRepository.FindByEntityAsync(Guid.Parse(request.Id));

            if (room == null)
            {
                return new RoomChangeResponse()
                {
                    Success = false,
                    Message = $"Room with Id {request.Id} not found"
                };
            }

            _mapper.Map(request, room);

            await _roomRepository.UpdateAsync(room);

            await dbTransaction.Commit(context.CancellationToken);
        }

        return changeResponse;
    }
}
