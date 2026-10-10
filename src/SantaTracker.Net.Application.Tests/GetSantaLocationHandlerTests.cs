using SantaTracker.Net.Application;
using SantaTracker.Net.Application.Features;
using SantaTracker.Net.Contracts.Dtos;
using SantaTracker.Net.Contracts.Enums;

namespace SantaTracker.Net.Application.Tests;

public class GetSantaLocationHandlerTests
{
    [Fact]
    public async Task EmptyRouteReportsSantaAtTheNorthPole()
    {
        var handler = new GetSantaLocationHandler(new EmptyRouteProvider());

        var response = await handler.GeAsync(new DateTime(2026, 12, 24, 21, 0, 0, DateTimeKind.Utc));

        Assert.Equal(SantaStatus.NorthPole.ToString(), response.Status);
        Assert.Contains("North Pole", response.Description);
        Assert.Empty(response.FromCity);
        Assert.Empty(response.ToCity);
    }

    private sealed class EmptyRouteProvider : ITrackingDataProvider
    {
        public Task<SantaRouteDto> GetSantaRouteDataAsync() => Task.FromResult(new SantaRouteDto());
    }
}
