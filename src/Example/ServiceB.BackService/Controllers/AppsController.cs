namespace ServiceB.BackService.Controllers;

[Route("api/serviceb/[controller]")]
[ApiController]
public class UserAppsController : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetUserApps()
    {
        var m = new NewAppInfoNotification()
        {
            Description = "des",
            Title = "Title",
        };
        return Ok(new List<NewAppInfoNotification>() { m , m});
    }
}
