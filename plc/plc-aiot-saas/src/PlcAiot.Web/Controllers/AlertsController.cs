using Microsoft.AspNetCore.Mvc;

namespace PlcAiot.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private static readonly List<AlertDto> Alerts = new()
    {
        new AlertDto { Id = "alert-001", DeviceId = "plc-001", Message = "温度过高", Severity = "warning", Acknowledged = false, Timestamp = DateTimeOffset.UtcNow.AddMinutes(-15) },
        new AlertDto { Id = "alert-002", DeviceId = "plc-002", Message = "设备离线", Severity = "critical", Acknowledged = false, Timestamp = DateTimeOffset.UtcNow.AddHours(-1) },
        new AlertDto { Id = "alert-003", DeviceId = "plc-003", Message = "压力异常", Severity = "info", Acknowledged = true, Timestamp = DateTimeOffset.UtcNow.AddHours(-2) }
    };

    [HttpGet]
    public IActionResult GetAlerts() => Ok(Alerts);

    [HttpPost("{id}/acknowledge")]
    public IActionResult Acknowledge(string id)
    {
        var alert = Alerts.FirstOrDefault(a => a.Id == id);
        if (alert == null)
        {
            return NotFound();
        }

        alert.Acknowledged = true;
        return Ok(alert);
    }
}

public class AlertDto
{
    public string Id { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public bool Acknowledged { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}
