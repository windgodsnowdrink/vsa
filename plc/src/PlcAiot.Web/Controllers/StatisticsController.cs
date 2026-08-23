using Microsoft.AspNetCore.Mvc;

namespace PlcAiot.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    [HttpGet("summary")]
    public IActionResult GetSummary() => Ok(new
    {
        TotalDevices = 12,
        OnlineDevices = 9,
        OfflineDevices = 2,
        FaultDevices = 1,
        TotalAlerts = 5,
        ResolvedAlerts = 3
    });

    [HttpGet("trend")]
    public IActionResult GetTrend() => Ok(Enumerable.Range(0, 24).Select(i => new
    {
        Hour = i,
        OnlineRate = 0.7 + Random.Shared.NextDouble() * 0.25,
        AlertCount = Random.Shared.Next(0, 5)
    }));
}
