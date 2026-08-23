using Microsoft.AspNetCore.Mvc;

namespace PlcAiot.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private static readonly List<DeviceDto> Devices = new()
    {
        new DeviceDto { Id = "plc-001", Name = "生产线PLC-001", Status = "online", Type = "ModbusTCP", LastSeen = DateTimeOffset.UtcNow.AddMinutes(-2) },
        new DeviceDto { Id = "plc-002", Name = "生产线PLC-002", Status = "offline", Type = "S7", LastSeen = DateTimeOffset.UtcNow.AddHours(-1) },
        new DeviceDto { Id = "plc-003", Name = "温度传感器-003", Status = "online", Type = "MQTT", LastSeen = DateTimeOffset.UtcNow.AddSeconds(-30) }
    };

    [HttpGet]
    public IActionResult GetDevices() => Ok(Devices);

    [HttpGet("{id}")]
    public IActionResult GetDevice(string id)
    {
        var device = Devices.FirstOrDefault(d => d.Id == id);
        return device == null ? NotFound() : Ok(device);
    }

    [HttpPost("{id}/command")]
    public IActionResult SendCommand(string id, [FromBody] CommandRequest request)
    {
        var device = Devices.FirstOrDefault(d => d.Id == id);
        if (device == null)
        {
            return NotFound();
        }

        return Ok(new { DeviceId = id, Command = request.Command, Status = "accepted", Timestamp = DateTimeOffset.UtcNow });
    }
}

public class DeviceDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset LastSeen { get; set; }
}

public class CommandRequest
{
    public string Command { get; set; } = string.Empty;
}
