using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SantaTracker.Net.Application.Features;
using SantaTracker.Net.Contracts.Responses;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
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
        [HttpGet]
        [ProducesResponseType(typeof(GetSantaLocationResponse), 200)]
        [SwaggerOperation(Tags = ["Santa"])]
        public async Task<IActionResult> GetSantaLocationAsync([FromServices] IGetSantaLocationHandler handler)
        {
            var location = await handler.GeAsync();

            return Ok(location);
        }

        /// <summary>
        ///     Gets Santa's current location.
        /// </summary>
        /// <returns></returns>
        [HttpGet("location-test")]
        [ProducesResponseType(typeof(GetSantaLocationResponse), 200)]
        [SwaggerOperation(Tags = ["Santa"])]
        public async Task<IActionResult> GetSantaLocationTestAsync(
            [FromServices] IGetSantaLocationHandler handler,
            [Required][FromQuery] DateTime testDateTime)
        {
            var location = await handler.GeAsync(testDateTime);

            return Ok(location);
        }
    }
}