using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SantaTracker.Net.Application.Features;
using SantaTracker.Net.Contracts.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace SantaTracker.Net.WebApi.Controllers.v1;

[ApiVersion("1.0")]
[ApiController]
[Route("api/[controller]")]
public class SantaController : ControllerBase
{
    /// <summary>
    ///     Gets Santa's current location. Time is based on server UTC time.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetSantaLocationResponse), 200)]
    [SwaggerOperation(Tags = ["Santa"])]
    public async Task<IActionResult> GetSantaLocationAsync([FromServices] IGetSantaLocationHandler handler)
    {
        GetSantaLocationResponse location = await handler.GeAsync();

        return Ok(location);
    }

    /// <summary>
    ///     Gets Santa's current location, time is based on the provided test date time in UTC.
    /// </summary>
    /// <returns></returns>
    [HttpGet("location-test")]
    [ProducesResponseType(typeof(GetSantaLocationResponse), 200)]
    [SwaggerOperation(Tags = ["Santa"])]
    public async Task<IActionResult> GetSantaLocationTestAsync(
        [FromServices] IGetSantaLocationHandler handler,
        [Required][FromQuery] DateTime testDateTime)
    {
        GetSantaLocationResponse location = await handler.GeAsync(testDateTime);

        return Ok(location);
    }
}
