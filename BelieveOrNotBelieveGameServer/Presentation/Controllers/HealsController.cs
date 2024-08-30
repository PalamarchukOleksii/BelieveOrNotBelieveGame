using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

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
