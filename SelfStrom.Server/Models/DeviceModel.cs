namespace SelfStrom.Server.Models;

public class DeviceModel
{
    public Guid? Id { get; set; }

    public required string MacAddress { get; set; }

    public required string IPAddress { get; set; }
}
