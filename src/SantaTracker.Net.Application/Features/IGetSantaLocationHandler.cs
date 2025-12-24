using SantaTracker.Net.Contracts.Dtos;
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
public sealed class GetSantaLocationHandler(ITrackingDataProvider trackingDataProvider) : IGetSantaLocationHandler
{
    public async Task<GetSantaLocationResponse> GeAsync(DateTime? utcNow = null)
    {
        SantaRouteDto trackingData = await trackingDataProvider.GetSantaRouteDataAsync();

        utcNow ??= DateTime.UtcNow;
        SantaStatus status = GetSantaStatus(utcNow.Value, trackingData.Destinations);

        return status switch
        {
            SantaStatus.NorthPole => new GetSantaLocationResponse
            {
                Status = status.ToString(),
                Description = "Santa is preparing at the North Pole 🎅"
            },
            SantaStatus.Delivered => new GetSantaLocationResponse
            {
                Status = status.ToString(),
                Description = "Santa has finished his rounds, and is having a well deserved rest! 🎁"
            },
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

        DateTime firstArrival = ShiftToYear(FromMs(route.First().Arrival), utcNow.Year);
        DateTime lastDeparture = ShiftToYear(FromMs(route[^2].Departure), utcNow.Year);

        return utcNow < firstArrival
            ? SantaStatus.NorthPole
            : utcNow > lastDeparture
                ? SantaStatus.Delivered
                : SantaStatus.InFlight;
    }

    private static DateTime ShiftToYear(DateTime utc, int targetYear)
    {
        int delta = targetYear - utc.Year;
        return utc.AddYears(delta);
    }

    private static GetSantaLocationResponse GetSantaCityAt(
        DateTime utcNow,
        IReadOnlyList<SantaDestinationDto> route)
    {
        // At a city now
        SantaDestinationDto? atACity = route.FirstOrDefault(x => utcNow >= x.ArrivalUtc && utcNow <= x.DepartureUtc);
        if (atACity is not null)
        {
            return new GetSantaLocationResponse
            {
                Country = atACity.Region,
                FromCity = atACity.City,
                Description = $"Delivering presents in {atACity.City}, {atACity.Region}",
                Status = SantaStatus.InFlight.ToString()
            };
        }

        // in flight between cities
        for (int i = 0; i < route.Count - 1; i++)
        {
            if (utcNow > route[i].DepartureUtc &&
                utcNow < route[i + 1].ArrivalUtc)
            {
                return new GetSantaLocationResponse
                {
                    FromCity = route[i].City,
                    ToCity = route[i + 1].City,
                    Country = route[i].Region,
                    Description = $"In flight from {route[i].City} to {route[i + 1].City}",
                    Status = SantaStatus.InFlight.ToString()
                };
            }
        }

        // TODO - make this more 'date aware'
        return new GetSantaLocationResponse
        {
            Description = "At North Pole",
            Status = SantaStatus.NorthPole.ToString()
        };
    }
}
