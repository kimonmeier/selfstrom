using Quartz;
using SelfStrom.Shared.Data;
using SelfStrom.Shared.Services.Worker;
using SelfStrom.Worker.Data.Entities;
using SelfStrom.Worker.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStrom.Worker.Background;

internal class UpdateLocalDataJob : IJob
{
    private readonly ConnectedDevicesRepository _deviceRepository;
    private readonly DbTransactionFactory _transactionFactory;
    private readonly DataSyncService.DataSyncServiceClient _dataSyncService;

    public UpdateLocalDataJob(ConnectedDevicesRepository deviceRepository, DbTransactionFactory transactionFactory, DataSyncService.DataSyncServiceClient dataSyncService)
    {
        _deviceRepository = deviceRepository;
        _transactionFactory = transactionFactory;
        _dataSyncService = dataSyncService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var response = await _dataSyncService.ListConnectedDevicesAsync(new Google.Protobuf.WellKnownTypes.Empty());

        using (var transaction = _transactionFactory.CreateTransaction())
        {
            List<Guid> handled = new List<Guid>();
            foreach (var device in response.Devices)
            {
                var connectedDevice = await _deviceRepository.FindByEntityAsync(Guid.Parse(device.DeviceId));

                if (connectedDevice is null)
                {
                    await _deviceRepository.AddAsync(new ConnectedDevices()
                    {
                        Id = Guid.Parse(device.DeviceId),
                        MacAdress = device.MacAddress,
                    });
                }


                handled.Add(Guid.Parse(device.DeviceId));
            }


            /*foreach (Guid id in await _deviceRepository.ListAllIds())
            {
                if (handled.Any(x => x.Equals(id)))
                {
                    continue;
                }

                await _deviceRepository.RemoveAsync(id);
            }*/

            await transaction.Commit(context.CancellationToken);
        }
    }
}
