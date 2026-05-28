using System.Text.Json.Serialization;

namespace MyStrom.Api.Models.DeviceInfo;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public class DeviceInfoResponse
{
    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("mac")]
    public string MacAddress { get; set; }

    [JsonPropertyName("type")]
    public DeviceType Type { get; set; }

    [JsonPropertyName("ip")]
    public string IPAddress { get; set; }

    [JsonPropertyName("connected")]
    public bool IsConnected { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
