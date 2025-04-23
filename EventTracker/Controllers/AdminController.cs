using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventTracker.Controllers
{
    [Authorize(Roles = "Client")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpGet("employees")]
        public IEnumerable<string> Get()
        {
            return new List<string> { "Ahmed", "Ali", "Ahsan" };
        }
    }
}
