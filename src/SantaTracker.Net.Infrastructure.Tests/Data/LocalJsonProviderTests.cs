using SantaTracker.Net.Infrastructure.Data;

namespace SantaTracker.Net.Infrastructure.Tests.Data;

public class LocalJsonProviderTests
{
    [Fact]
    public async Task LoadsEntireRouteIncludingKyivWithConsistentUtcTimestamps()
    {
        // Arrange & Act
        var route = await new LocalJsonProvider().GetSantaRouteDataAsync();

        // Assert
        Assert.Equal(128, route.Destinations.Count);
        Assert.Contains(route.Destinations, stop => stop.City == "Kyiv");
        Assert.All(route.Destinations, stop =>
        {
            Assert.Equal(DateTimeKind.Utc, stop.ArrivalUtc.Kind);
            Assert.Equal(DateTimeKind.Utc, stop.DepartureUtc.Kind);
            Assert.Equal(stop.ArrivalUtc, DateTimeOffset.FromUnixTimeMilliseconds(stop.Arrival).UtcDateTime);
            Assert.Equal(stop.DepartureUtc, DateTimeOffset.FromUnixTimeMilliseconds(stop.Departure).UtcDateTime);
            Assert.Equal(TimeSpan.FromMinutes(2), stop.DepartureUtc - stop.ArrivalUtc);
        });
    }

    [Theory]
    [InlineData("London", 24, 21, 0)]
    [InlineData("Wellington", 24, 8, 0)]
    [InlineData("Kathmandu", 24, 15, 15)]
    [InlineData("Mexico City", 25, 3, 0)]
    public async Task ConvertsLocalArrivalsIncludingSummerTimeFractionalOffsetsAndDateRollover(
        string city, int day, int hour, int minute)
    {
        // Arrange & Act
        var route = await new LocalJsonProvider().GetSantaRouteDataAsync();

        // Assert
        var stop = Assert.Single(route.Destinations, stop => stop.City == city);
        Assert.Equal(new DateTime(stop.ArrivalUtc.Year, 12, day, hour, minute, 0, DateTimeKind.Utc), stop.ArrivalUtc);
    }
}
