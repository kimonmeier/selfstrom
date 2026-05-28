using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStrom.Api.Models.Report;

public class DeviceReport
{
    [JsonProperty("power")]
    public double Power { get; set; }

    [JsonProperty("Ws")]
    public double Ws { get; set; }

    [JsonProperty("relay")]
    public bool Relay { get; set; }

    [JsonProperty("temperature")]
    public double Temperature { get; set; }

    [JsonProperty("boot_id")]
    public string BootId { get; set; }

    [JsonProperty("energy_since_boot")]
    public double EnergySinceBoot { get; set; }

    [JsonProperty("time_since_boot")]
    public int TimeSinceBoot { get; set; }
}
