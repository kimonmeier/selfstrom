using Flurl;
using Flurl.Http;
using MyStrom.Api.Models.DeviceInfo;
using MyStrom.Api.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStrom.Api;

public class DeviceApi
{
    private string BaseUrl { get; init; }

    public DeviceApi(string baseUrl)
    {
        BaseUrl = baseUrl;
    }

    public Task<DeviceInfoResponse> GetDeviceInfoAsync(CancellationToken cancellationToken = default)
    {
        return BaseUrl.AppendPathSegments("api", "v1", "info").GetJsonAsync<DeviceInfoResponse>(cancellationToken: cancellationToken);
    }

    public Task<DeviceReport> GetReport(CancellationToken cancellationToken = default)
    {
        return BaseUrl.AppendPathSegments("report").GetJsonAsync<DeviceReport>(cancellationToken: cancellationToken);
    }
}
