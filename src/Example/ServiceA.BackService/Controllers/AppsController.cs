


namespace ServiceA.BackService.Controllers;

[Route("api/servicea/[controller]")]
[ApiController]
public class AppsController : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetNames()
    {
        var m = new NewAppInfoNotification()
        {
            Description = "des",
            Title = "Title",
        };
        return Ok(m);
    }
}
