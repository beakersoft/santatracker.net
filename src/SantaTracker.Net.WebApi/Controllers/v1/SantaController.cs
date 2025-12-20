using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SantaTracker.Net.Application.Features;
using SantaTracker.Net.Contracts.Responses;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace SantaTracker.Net.Controllers.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/[controller]")]
    public class SantaController : ControllerBase
    {
        /// <summary>
        ///     Gets Santa's current location.
        /// </summary>
        /// <returns></returns>
        [HttpGet("location")]
        [ProducesResponseType(typeof(GetSantaLocationResponse), 200)]
        [SwaggerOperation(Tags = ["Santa"])]
        public async Task<IActionResult> GetSantaLocation([FromServices] IGetSantaLocationHandler handler)
        {
            var location = await handler.GeAsync(DateTime.UtcNow);

            return Ok(location);
        }
    }
}