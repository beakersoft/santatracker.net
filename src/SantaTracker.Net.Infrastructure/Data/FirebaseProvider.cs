using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using SantaTracker.Net.Application;
using SantaTracker.Net.Contracts.Dtos;

namespace SantaTracker.Net.Infrastructure.Data;

/// <summary>
///     Firebase provider for santa data.
/// </summary>
public class FirebaseProvider(
    HttpClient httpClient,
    IMemoryCache cache) : ITrackingDataProvider
{
    private const string RouteUrl =
        "https://firebasestorage.googleapis.com/v0/b/santa-tracker-firebase.appspot.com/o/route%2Fsanta_en.json?alt=media";

    private const string CacheKey = "SantaRoute";

    /// <inheritdoc />
    public async Task<SantaRouteDto> GetSantaRouteDataAsync()
    {
        if (cache.TryGetValue(CacheKey, out SantaRouteDto? cached))
        {
            return cached!;
        }

        var route = await httpClient.GetFromJsonAsync<SantaRouteDto>(RouteUrl);

        if (route?.Destinations.Count == 0)
        {
            throw new Exception("Could not get firebase santa data");
        }

        var dateNowUtc = DateTime.UtcNow;

        foreach (var stop in route!.Destinations)
        {
            var arrivalUtc = DateTimeOffset.FromUnixTimeMilliseconds(stop.Arrival).UtcDateTime;
            var departUtc = DateTimeOffset.FromUnixTimeMilliseconds(stop.Departure).UtcDateTime;

            var yearDelta = dateNowUtc.Year - arrivalUtc.Year;
            arrivalUtc = arrivalUtc.AddYears(yearDelta);
            departUtc = departUtc.AddYears(yearDelta);

            stop.Arrival = new DateTimeOffset(arrivalUtc).ToUnixTimeMilliseconds();
            stop.Departure = new DateTimeOffset(departUtc).ToUnixTimeMilliseconds();
            stop.ArrivalUtc = arrivalUtc;
            stop.DepartureUtc = departUtc;
        }

        route.Destinations = route.Destinations.OrderBy(d => d.ArrivalUtc).ToList();

        cache.Set(
            CacheKey,
            route,
            TimeSpan.FromMinutes(5));

        return route;
    }
}
