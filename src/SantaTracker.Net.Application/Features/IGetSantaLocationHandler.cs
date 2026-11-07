using Microsoft.Extensions.Localization;
using SantaTracker.Net.Contracts.Dtos;
using SantaTracker.Net.Contracts.Enums;
using SantaTracker.Net.Contracts.Responses;

namespace SantaTracker.Net.Application.Features;

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
public sealed class GetSantaLocationHandler(
    ITrackingDataProvider trackingDataProvider,
    IStringLocalizer stringLocalizer) : IGetSantaLocationHandler
{
    public async Task<GetSantaLocationResponse> GeAsync(DateTime? utcNow = null)
    {
        var trackingData = await trackingDataProvider.GetSantaRouteDataAsync();

        utcNow ??= DateTime.UtcNow;
        var status = GetSantaStatus(utcNow.Value, trackingData.Destinations);

        return status switch
        {
            SantaStatus.NorthPole => GetSantaLocationResponse.NorthPoleLocation(stringLocalizer["SantaAtNorthPole"], status.ToString()),
            SantaStatus.Delivered => GetSantaLocationResponse.NorthPoleLocation(stringLocalizer["SantaHaFinishedDeliveringPresents"], status.ToString()),
            SantaStatus.InFlight => GetSantaCityAt(utcNow.Value, trackingData.Destinations),
            _ => GetSantaCityAt(utcNow.Value, trackingData.Destinations)
        };
    }

    private static DateTime FromMs(long unixMilliseconds) =>
        DateTimeOffset
            .FromUnixTimeMilliseconds(unixMilliseconds)
            .UtcDateTime;

    public static SantaStatus GetSantaStatus(
        DateTime utcNow,
        IReadOnlyList<SantaDestinationDto> route)
    {
        if (route.Count == 0)
        {
            return SantaStatus.NorthPole;
        }

        var firstArrival = ShiftToYear(FromMs(route[0].Arrival), utcNow.Year);
        var lastDeparture = ShiftToYear(FromMs(route[^2].Departure), utcNow.Year);

        return utcNow < firstArrival
            ? SantaStatus.NorthPole
            : utcNow > lastDeparture
                ? SantaStatus.Delivered
                : SantaStatus.InFlight;
    }

    private static DateTime ShiftToYear(DateTime utc, int targetYear)
    {
        var delta = targetYear - utc.Year;
        return utc.AddYears(delta);
    }

    private GetSantaLocationResponse GetSantaCityAt(
        DateTime utcNow,
        List<SantaDestinationDto> route)
    {
        // At a city now
        var atACity = route.FirstOrDefault(x => utcNow >= x.ArrivalUtc && utcNow <= x.DepartureUtc);
        if (atACity is not null)
        {
            return new GetSantaLocationResponse
            {
                Country = atACity.Region,
                FromCity = atACity.City,
                Description = stringLocalizer["SantaIsDeliveringPresentsIn", atACity.City, atACity.Region],
                Status = SantaStatus.InFlight.ToString(),
                Latitude = atACity.Location.Lat,
                Longitude = atACity.Location.Lng
            };
        }

        // in flight between cities
        for (var i = 0; i < route.Count - 1; i++)
        {
            if (utcNow > route[i].DepartureUtc &&
                utcNow < route[i + 1].ArrivalUtc)
            {
                var origin = route[i];
                var destination = route[i + 1];
                var journeyDuration = destination.ArrivalUtc - origin.DepartureUtc;
                var journeyProgress = (utcNow - origin.DepartureUtc).TotalMilliseconds /
                                      journeyDuration.TotalMilliseconds;

                return new GetSantaLocationResponse
                {
                    FromCity = origin.City,
                    ToCity = destination.City,
                    Country = origin.Region,
                    Description = stringLocalizer["SantaInFlightBetweenCities", origin.City, origin.Region, destination.City],
                    Status = SantaStatus.InFlight.ToString(),
                    Latitude = Interpolate(origin.Location.Lat, destination.Location.Lat, journeyProgress),
                    Longitude = InterpolateLongitude(origin.Location.Lng, destination.Location.Lng, journeyProgress)
                };
            }
        }

        return GetSantaLocationResponse.NorthPoleLocation(stringLocalizer["SantaAtNorthPole"], SantaStatus.NorthPole.ToString());
    }

    private static double Interpolate(double origin, double destination, double progress) =>
        origin + ((destination - origin) * progress);

    private static double InterpolateLongitude(double origin, double destination, double progress)
    {
        var difference = ((destination - origin + 540) % 360) - 180;
        return ((origin + (difference * progress) + 540) % 360) - 180;
    }
}
