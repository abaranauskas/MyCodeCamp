using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCodeCamp.Controllers
{
    [Route("api/[controller]")]
    public class OperationsController : Controller
    {
        private ILogger<OperationsController> _logger;
        private IConfigurationRoot _config;

        public OperationsController(ILogger<OperationsController> logger,
            IConfigurationRoot config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpOptions("realoadConfig")]
        public IActionResult ReloadConfiguration()
        {
            try
            {
                _config.Reload();
                return Ok("Configuration Reloaded!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception was thrown while relading configurations: {ex}");
              
            }
            return BadRequest("Could not reload configurations");
        }
    }
}
