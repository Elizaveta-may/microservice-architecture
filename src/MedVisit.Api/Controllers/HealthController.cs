using Microsoft.AspNetCore.Mvc;

namespace MedVisit.Api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHealth()
        {
            return Ok(new { status = "OK" });
        }
    }
}
