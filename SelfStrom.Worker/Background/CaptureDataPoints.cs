using Microsoft.Extensions.Logging;
using MyStrom.Api;
using MyStrom.Api.Models.Report;
using Quartz;
using SelfStrom.Shared.Data;
using SelfStrom.Worker.Data.Entities;
using SelfStrom.Worker.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SelfStrom.Worker.Background;

internal class CaptureDataPoints : IJob
{
    private readonly ConnectedDevicesRepository _devicesRepository;
    private readonly DataPointRepository _dataPointRepository;
    private readonly DbTransactionFactory _dbTransactionFactory;
    private readonly ILogger<CaptureDataPoints> _logger;

    public CaptureDataPoints(ConnectedDevicesRepository devicesRepository, DataPointRepository dataPointRepository, DbTransactionFactory dbTransactionFactory, ILogger<CaptureDataPoints> logger)
    {
        _devicesRepository = devicesRepository;
        _dataPointRepository = dataPointRepository;
        _dbTransactionFactory = dbTransactionFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using (var transaction = _dbTransactionFactory.CreateTransaction())
        {
            foreach (var device in await _devicesRepository.ListAll())
            {
                await CaptureDevice(device);
            }

            await transaction.Commit(context.CancellationToken);
        }
    }

    private async Task CaptureDevice(ConnectedDevices device)
    {
        if (device.IPAddress is null)
        {
            return;
        }

        DeviceApi deviceApi = new DeviceApi($"http://{device.IPAddress}");
        if (!await _dataPointRepository.HasDataPoints(device.Id))
        {
            // Capture first data point to reset
            await deviceApi.GetReport();
            await _dataPointRepository.AddAsync(new DataPoint()
            {
                DeviceId = device.Id,
                IsSynced = true,
                Timestamp = DateTime.UtcNow,
                Value = 0
            });
            return;
        }
        DeviceReport report;
        try
        {
            report = await deviceApi.GetReport();
        } catch(Exception e)
        {
            _logger.LogError(e, "Failed to get report from device {0} at {1}", device.Id, device.IPAddress);
            return;
        }

        await _dataPointRepository.AddAsync(new DataPoint()
        {
            DeviceId = device.Id,
            Timestamp = DateTime.UtcNow,
            Value = report.Ws,
        });
    }
}
