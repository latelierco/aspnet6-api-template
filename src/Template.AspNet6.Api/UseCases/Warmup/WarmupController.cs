using Microsoft.AspNetCore.Mvc;

namespace Template.AspNet6.Api.UseCases.Warmup;

[ApiVersion("1.0")]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class WarmupController : ControllerBase
{
    /// <summary>
    /// Warmup endpoint
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Warmup()
    {
        return Ok("warmup!");
    }
}
