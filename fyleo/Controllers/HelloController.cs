using Microsoft.AspNetCore.Mvc;
using System;

namespace fyleo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        public ActionResult Ping()
        {
            return Ok($"fyleo is alive, {DateTime.Now}");
        }
    }
}