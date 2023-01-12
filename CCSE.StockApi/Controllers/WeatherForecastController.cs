using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCSE.StockApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IEnumerable<string> GetByAdmin()
        {
            return new string[] { "value1", "value2" };
        }

        [Authorize(Roles = "PowerUser")]
        [HttpGet("pu/{id}")]
        public string GetByPowerUser(int id)
        {
            return "value";
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
    }
}