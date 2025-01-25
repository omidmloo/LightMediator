namespace FinalService.Controllers;

[Route("api/main/[controller]")]
[ApiController]
public class ServicesController : ControllerBase
{
    [HttpGet("list")]
    public async Task<IActionResult> GetServices()
    { 
        return Ok(new { ServiceName = "1"});
    }
}
