using APIGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SwaggerController : ControllerBase
    {
        private readonly ISwaggerAggregatorService _aggregator;

        public SwaggerController(ISwaggerAggregatorService aggregator)
        {
            _aggregator = aggregator;
        }

        [HttpGet("all")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAllServicesSwagger()
        {
            var swaggerJson = await _aggregator.GetAggregatedSwaggerJsonAsync();
            return Content(swaggerJson, "application/json");
        }
    }
}
