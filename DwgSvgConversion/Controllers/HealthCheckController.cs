using Microsoft.AspNetCore.Mvc;

namespace DwgSvgConversion.Controllers;

[Route(("api/healthcheck"))]
[ApiController]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealCheckStatus()
    {
        return Ok();
    }
}