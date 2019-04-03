using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IsHttpBug;
using Microsoft.AspNetCore.Mvc;

namespace basicmvc.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VariablesController : ControllerBase
    {
        [HttpGet]
        public JsonResult Get()
        {
            return new JsonResult(Environment.GetEnvironmentVariables());
        }

        [HttpGet]
        [Route("/whoami")]
        public JsonResult Who()
        {
            return new JsonResult(new {
                IsHttps = Request.IsHttps,
                InBoundIP = ControllerContext.HttpContext.Connection.RemoteIpAddress.ToString(),
                Version = Startup.VERSION
            });
        }
    }
}