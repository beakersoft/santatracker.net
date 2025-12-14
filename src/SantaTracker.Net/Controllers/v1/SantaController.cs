using Microsoft.AspNetCore.Mvc;
using SantaTracker.Net.Application.Features;
using Swashbuckle.AspNetCore.Annotations;

namespace SantaTracker.Net.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SantaController : ControllerBase
    {
        /// <summary>
        ///     Gets Santa's current location.
        /// </summary>
        /// <returns></returns>
        [HttpGet("location")]
        [SwaggerOperation(Tags = ["Santa"])]
        public async Task<IActionResult> GetSantaLocation([FromServices] IGetSantaLocationHandler handler)
        {
            var location = await handler.GeAsync();

            return Ok(location);
        }
    }
}