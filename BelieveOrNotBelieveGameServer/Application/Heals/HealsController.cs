using Microsoft.AspNetCore.Mvc;

namespace BelieveOrNotBelieveGameServer.Application.Heals;

[ApiController]
[Route("[controller]")]
public class HealsController : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
}
