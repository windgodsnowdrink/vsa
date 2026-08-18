using Microsoft.AspNetCore.Mvc;

namespace PlcAiot.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    [HttpGet("latest")]
    public IActionResult GetLatest() => Ok(new[]
    {
        new { DeviceId = "plc-001", Temperature = 42.5, Pressure = 1.02, Timestamp = DateTimeOffset.UtcNow.AddSeconds(-10) },
        new { DeviceId = "plc-002", Temperature = 38.0, Pressure = 0.98, Timestamp = DateTimeOffset.UtcNow.AddSeconds(-20) },
        new { DeviceId = "plc-003", Temperature = 45.2, Pressure = 1.05, Timestamp = DateTimeOffset.UtcNow.AddSeconds(-5) }
    });
}
