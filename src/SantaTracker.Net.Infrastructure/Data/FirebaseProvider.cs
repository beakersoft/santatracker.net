using System.Net.Http.Json;
using SantaTracker.Net.Application;
using SantaTracker.Net.Contracts.Dtos;

namespace SantaTracker.Net.Infrastructure.Data
{
    /// <summary>
    /// Firebase provider for santa data.
    /// </summary>
    public class FirebaseProvider(HttpClient httpClient) : ITrackingDataProvider
    {
        private const string RouteUrl =
            "https://firebasestorage.googleapis.com/v0/b/santa-tracker-firebase.appspot.com/o/route%2Fsanta_en.json?alt=media";

        /// <inheritdoc/>
        public async Task<SantaRouteDto> GetSantaRouteDataAsync()
        {
            var route = await httpClient.GetFromJsonAsync<SantaRouteDto>(RouteUrl);

            if (route?.Destinations.Count == 0)
            {
                throw new Exception("Could not get firebase santa data");
            }

            //var yearNow = DateTime.UtcNow.Year;

            //foreach (var stop in route!.Destinations)
            //{
            //    var dt2019Arrival = DateTimeOffset.FromUnixTimeMilliseconds(stop.Arrival).UtcDateTime;
            //    var dt2019Departure = DateTimeOffset.FromUnixTimeMilliseconds(stop.Departure).UtcDateTime;

            //    // Rebuild DateTime for this year
            //    var arrivalThisYear = new DateTime(
            //        yearNow,
            //        dt2019Arrival.Month,
            //        dt2019Arrival.Day,
            //        dt2019Arrival.Hour,
            //        dt2019Arrival.Minute,
            //        dt2019Arrival.Second,
            //        DateTimeKind.Utc);

            //    var departureThisYear = new DateTime(
            //        yearNow,
            //        dt2019Departure.Month,
            //        dt2019Departure.Day,
            //        dt2019Departure.Hour,
            //        dt2019Departure.Minute,
            //        dt2019Departure.Second,
            //        DateTimeKind.Utc);

            //    // Convert to Unix milliseconds via DateTimeOffset
            //    stop.Arrival = new DateTimeOffset(arrivalThisYear).ToUnixTimeMilliseconds();
            //    stop.Departure = new DateTimeOffset(departureThisYear).ToUnixTimeMilliseconds();
            //}

            // normalize the dates in the data to this year
            //var routeStart = DateTimeOffset
            //    .FromUnixTimeMilliseconds(route.Destinations[0].Departure)
            //    .UtcDateTime;

            //var targetBase =
            //    new DateTime(DateTime.UtcNow.Year, 12, 24, 0, 0, 0, DateTimeKind.Utc);

            //var offset = targetBase - routeStart;

            //foreach (var stop in route.Destinations)
            //{
            //    stop.Arrival += (long)offset.TotalMilliseconds;
            //    stop.Departure += (long)offset.TotalMilliseconds;
            //}

            return route;
        }
    }
}