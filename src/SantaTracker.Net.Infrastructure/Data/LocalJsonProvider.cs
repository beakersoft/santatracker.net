using System.Globalization;
using System.Text.Json;
using NodaTime;
using NodaTime.Text;
using SantaTracker.Net.Application;
using SantaTracker.Net.Contracts.Dtos;

namespace SantaTracker.Net.Infrastructure.Data;

/// <summary>
///     Local data provider for santa data.
/// </summary>
public class LocalJsonProvider() : ITrackingDataProvider
{
    public async Task<SantaRouteDto> GetSantaRouteDataAsync()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory, "Data", "santa-route.json");

        await using var stream = File.OpenRead(path);

        var route = await JsonSerializer.DeserializeAsync<LocalSantaRouteDto>(
                stream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })
            ?? throw new InvalidOperationException("Route file contained null.");

        var baseDate = LocalDatePattern.Iso.Parse(
            $"{SystemClock.Instance.GetCurrentInstant().InUtc().Year}-{route.LocalBaseDate}").Value;
        var arrivalTimePattern = LocalTimePattern.CreateWithInvariantCulture("HH:mm");

        return new()
        {
            Destinations = route.Stops.ConvertAll(
                x =>
                {
                    var arrivalLocal = baseDate.PlusDays(x.DayOffset).At(
                        arrivalTimePattern.Parse(x.ArrivalLocal).Value);
                    var timeZone = DateTimeZoneProviders.Tzdb[x.TimeZone];

                    // Reject ambiguous or skipped local times instead of silently shifting the schedule.
                    var arrival = arrivalLocal.InZoneStrictly(timeZone).ToInstant();
                    var departure = arrival + Duration.FromMinutes(x.DurationMinutes);

                    return new SantaDestinationDto
                    {
                        City = x.City,
                        Region = new RegionInfo(x.CountryCode).EnglishName,
                        ArrivalUtc = arrival.ToDateTimeUtc(),
                        DepartureUtc = departure.ToDateTimeUtc(),
                        Arrival = arrival.ToUnixTimeMilliseconds(),
                        Departure = departure.ToUnixTimeMilliseconds(),
                        Location = new SantaLocationDto
                        {
                            Lat = x.Latitude,
                            Lng = x.Longitude
                        }
                    };
                })
        };
    }
}
