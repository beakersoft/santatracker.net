using System.Net.Http.Json;
using SantaTracker.Net.Contracts.Responses;

namespace SantaTracker.Net.Application.Features
{
    /// <summary>
    ///     Handler for getting Santa's current location.
    /// </summary>
    public interface IGetSantaLocationHandler
    {
        /// <summary>
        ///     Get Santa's current location.
        /// </summary>
        /// <returns></returns>
        Task<GetSantaLocationResponse> GeAsync(DateTime utcNow);
    }

    /// <inheritdoc />
    public sealed class GetSantaLocationHandler(HttpClient httpClient) : IGetSantaLocationHandler
    {
        private const string RouteUrl =
            "https://firebasestorage.googleapis.com/v0/b/santa-tracker-firebase.appspot.com/o/route%2Fsanta_en.json?alt=media";

        public async Task<GetSantaLocationResponse> GeAsync(DateTime utcNow)
        {
            var defaultResponse = new GetSantaLocationResponse
            {
                Description = "Waiting for Christmas!",
                City = "North Pole",
                Country = "Arctic",
            };

            var route = await httpClient.GetFromJsonAsync<SantaRouteDto>(RouteUrl);

            // TODO - normalize the dates in the incoming data to be the current year, so we can use DateTime.UtcNow
            // TODO - this here kinda becomes an e2e test

            var stops = route?.Destinations ?? [];

            var from = route.Destinations[0];
            var to = route.Destinations[1];

            // Interpret as epoch milliseconds
            var dep = DateTimeOffset.FromUnixTimeMilliseconds(from.Departure).UtcDateTime;
            var arr = DateTimeOffset.FromUnixTimeMilliseconds(to.Arrival).UtcDateTime;

            // Pick a time halfway between departure and arrival
            var inFlightNow = dep + TimeSpan.FromTicks((arr - dep).Ticks / 2);
            var now = new DateTimeOffset(inFlightNow).ToUnixTimeMilliseconds();

            for (var i = 0; i < stops.Count - 1; i++)
            {
                var current = stops[i];
                var next = stops[i + 1];

                if (now > current.Departure && now < next.Arrival)
                {
                    return new GetSantaLocationResponse
                    {
                        Status = "InFlight",
                        City = next.City
                    };
                }
            }

            return new GetSantaLocationResponse { Status = "Stopped" };
        }
    }
}