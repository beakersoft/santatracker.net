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

            var stops = trackingData?.Destinations ?? [];

            if (utcNow < FromMs(stops[0].Departure))
            {
                return defaultResponse;
            }

            long nowMs = new DateTimeOffset(utcNow.Value).ToUnixTimeMilliseconds();

            foreach (var stop in stops)
            {
                if (stop.City == "London")
                {
                    var londonDate = DateTimeOffset.FromUnixTimeMilliseconds(stop.Arrival).UtcDateTime;

                    //london showing at 01:17
                }

                if (nowMs >= stop.Arrival && nowMs <= stop.Departure)
                {
                    return new GetSantaLocationResponse
                    {
                        Status = "InFlight",
                        FromCity = stop.City,
                        Country = stop.Region
                    };
                }
            }

            //for (var i = 0; i < stops.Count - 1; i++)
            //{
            //    var from = stops[i];
            //    var to = stops[i + 1];

            //    var dep = FromMs(from.Departure);
            //    var arr = FromMs(to.Arrival);

            //    if (utcNow >= dep && utcNow <= arr)
            //    {
            //        var t = (utcNow - dep).Value.TotalMilliseconds /
            //                (arr - dep).TotalMilliseconds;

            //        return new GetSantaLocationResponse
            //        {
            //            Status = "InFlight",
            //            FromCity = from.City,
            //            ToCity = to.City,
            //            Country = from.Region
            //        };
            //    }
            //}

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