using Microsoft.AspNetCore.Mvc;
using System;

namespace stauff_interface_webshop_spryker.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase {
        [HttpGet]
        public dynamic Get() {
            return new {
                message = "pong",
                date = DateTime.Now
            };
        }
    }
}
