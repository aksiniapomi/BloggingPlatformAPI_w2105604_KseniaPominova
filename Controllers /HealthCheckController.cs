using Microsoft.AspNetCore.Mvc;

[Route("api/healthcheck")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "API is running!" });
    }
}