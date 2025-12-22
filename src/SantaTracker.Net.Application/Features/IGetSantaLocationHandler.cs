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
        Task<GetSantaLocationResponse> GeAsync(DateTime? utcNow = null);
    }

    /// <inheritdoc />
    public sealed class GetSantaLocationHandler(ITrackingDataProvider trackingDataProvider) : IGetSantaLocationHandler
    {
        public async Task<GetSantaLocationResponse> GeAsync(DateTime? utcNow = null)
        {
            var defaultResponse = new GetSantaLocationResponse
            {
                Status = "Stopped",
                Description = "Waiting for Christmas!",
                FromCity = "North Pole",
                Country = "Arctic",
            };

            utcNow ??= DateTime.UtcNow;

            var trackingData = await trackingDataProvider.GetSantaRouteDataAsync();

            var stops = trackingData.Destinations;

            if (utcNow < FromMs(stops[0].Departure))
            {
                return defaultResponse;
            }

            for (var i = 0; i < stops.Count - 1; i++)
            {
                var from = stops[i];
                var to = stops[i + 1];

                var arrivalDateTime = DateTimeOffset.FromUnixTimeMilliseconds(stops[i].Arrival).UtcDateTime;
                var departureDateTime = DateTimeOffset.FromUnixTimeMilliseconds(stops[i].Departure).UtcDateTime;

                if (utcNow >= arrivalDateTime && utcNow <= departureDateTime)
                {
                    return new GetSantaLocationResponse
                    {
                        Status = "InFlight",
                        FromCity = from.City,
                        ToCity = to.City,
                        Country = from.Region
                    };
                }
            }

            return defaultResponse;
        }

        private static DateTime FromMs(long unixMilliseconds)
        {
            return DateTimeOffset
                .FromUnixTimeMilliseconds(unixMilliseconds)
                .UtcDateTime;
        }
    }
}