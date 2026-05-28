using AutoMapper;
using Microsoft.Extensions.Logging;
using Quartz;
using SelfStrom.Shared.Data;
using SelfStrom.Shared.Services.Worker;
using SelfStrom.Worker.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStrom.Worker.Background;

internal class SyncDataPointsJob : IJob
{
    private readonly DataPointRepository _dataPointRepository;
    private readonly DataSyncService.DataSyncServiceClient _dataSyncService;
    private readonly DbTransactionFactory _dbTransactionFactory;
    private readonly ILogger<SyncDataPointsJob> _logger;
    private readonly IMapper _mapper;

    public SyncDataPointsJob(DataSyncService.DataSyncServiceClient dataSyncService, DataPointRepository dataPointRepository, DbTransactionFactory dbTransactionFactory, IMapper mapper, ILogger<SyncDataPointsJob> logger)
    {
        _dataSyncService = dataSyncService;
        _dataPointRepository = dataPointRepository;
        _dbTransactionFactory = dbTransactionFactory;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var pointsToSync = await _dataPointRepository.ListUnsynchedPoints();

        try
        {
            ReportDataPointsRequest request = new ReportDataPointsRequest();
            request.DataPoints.Add(_mapper.Map<List<DataPointProto>>(pointsToSync));

            await _dataSyncService.ReportDataPointsAsync(request);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "The SyncDataPointsJob failed to sync data points. Because there was an error on the server.");
            throw;
        }

        using (var transaction = _dbTransactionFactory.CreateTransaction())
        {
            foreach(var dataPoint in pointsToSync)
            {
                dataPoint.IsSynced = true;
                await _dataPointRepository.UpdateAsync(dataPoint);
            }

            await transaction.Commit(context.CancellationToken);
        }
    }
}
