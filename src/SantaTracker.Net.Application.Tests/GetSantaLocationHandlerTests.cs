using SantaTracker.Net.Application.Features;
using SantaTracker.Net.Contracts.Dtos;
using SantaTracker.Net.Contracts.Enums;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace SantaTracker.Net.Application.Tests;

public class GetSantaLocationHandlerTests
{
    [Fact]
    public async Task EmptyRouteReportsSantaAtTheNorthPole()
    {
        var handler = new GetSantaLocationHandler(new EmptyRouteProvider(), new TestStringLocalizer());

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

    private sealed class TestStringLocalizer : IStringLocalizer
    {
        public LocalizedString this[string name] => new(name, (CultureInfo.CurrentUICulture.Name, name) switch
        {
            ("es-ES", "SantaAtNorthPole") => "Papá Noel se está preparando en el Polo Norte",
            ("es-ES", "SantaHaFinishedDeliveringPresents") => "Papá Noel ha terminado de repartir los regalos",
            (_, "SantaAtNorthPole") => "Santa is preparing at the North Pole",
            (_, "SantaHaFinishedDeliveringPresents") => "Santa has finished his rounds",
            _ => name
        });

        public LocalizedString this[string name, params object[] arguments] => this[name];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => [];
    }
}
