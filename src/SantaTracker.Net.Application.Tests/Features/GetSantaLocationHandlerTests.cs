using Microsoft.Extensions.Localization;
using Moq;
using SantaTracker.Net.Application.Features;
using SantaTracker.Net.Contracts.Dtos;
using SantaTracker.Net.Contracts.Enums;
using Shouldly;

namespace SantaTracker.Net.Application.Tests.Features;

public class GetSantaLocationHandlerTests
{
    private static readonly DateTime RouteStart = new(2026, 12, 24, 12, 0, 0, DateTimeKind.Utc);
    private readonly GetSantaLocationHandler _sut;
    private readonly Mock<ITrackingDataProvider> _trackingDataProviderMock = new();
    private readonly Mock<IStringLocalizer> _stringLocalizerMock = new();

    public GetSantaLocationHandlerTests()
    {
        _sut = new(_trackingDataProviderMock.Object, _stringLocalizerMock.Object);
        ConfigureLocalizer();
    }

    [Fact]
    public async Task GeAsync_WhenRouteIsEmpty_ReturnsNorthPoleDescription()
    {
        // Arrange
        ArrangeRoute();

        // Act
        var response = await _sut.GeAsync();

        // Assert
        response.Status.ShouldBe(SantaStatus.NorthPole.ToString());
        response.Description.ShouldBe("SantaAtNorthPole");
        response.FromCity.ShouldBeEmpty();
        response.ToCity.ShouldBeEmpty();
        response.Country.ShouldBeEmpty();
        _stringLocalizerMock.Verify(localizer => localizer["SantaAtNorthPole"], Times.Once);
    }

    [Fact]
    public async Task GeAsync_WhenSantaHasNotStarted_ReturnsNorthPoleDescription()
    {
        // Arrange
        ArrangeRoute(CreateRoute());

        // Act
        var response = await _sut.GeAsync(RouteStart.AddHours(-1));

        // Assert
        response.Status.ShouldBe(SantaStatus.NorthPole.ToString());
        response.Description.ShouldBe("SantaAtNorthPole");
        response.FromCity.ShouldBeEmpty();
        response.ToCity.ShouldBeEmpty();
    }

    [Fact]
    public async Task GeAsync_WhenSantaHasFinished_ReturnsDeliveredDescription()
    {
        // Arrange
        ArrangeRoute(CreateRoute());

        // Act
        var response = await _sut.GeAsync(RouteStart.AddHours(2));

        // Assert
        response.Status.ShouldBe(SantaStatus.Delivered.ToString());
        response.Description.ShouldBe("SantaHaFinishedDeliveringPresents");
        _stringLocalizerMock.Verify(localizer => localizer["SantaHaFinishedDeliveringPresents"], Times.Once);
    }

    [Fact]
    public async Task GeAsync_WhenSantaArrivesAtACity_ReturnsCityDetailsAndLocalizedDescription()
    {
        // Arrange
        var london = Destination("London", "UK", RouteStart, RouteStart.AddHours(1));
        ArrangeRoute([london, Destination("Paris", "France", RouteStart.AddHours(2), RouteStart.AddHours(3))]);

        // Act
        var response = await _sut.GeAsync(RouteStart);

        // Assert
        response.Status.ShouldBe(SantaStatus.InFlight.ToString());
        response.FromCity.ShouldBe("London");
        response.ToCity.ShouldBeEmpty();
        response.Country.ShouldBe("UK");
        response.Description.ShouldBe("SantaIsDeliveringPresentsIn|London|UK");
        _stringLocalizerMock.Verify(
            localizer => localizer["SantaIsDeliveringPresentsIn", It.IsAny<object[]>()],
            Times.Once);
    }

    [Fact]
    public async Task GeAsync_WhenSantaIsAtCityDepartureBoundary_IncludesCityDetails()
    {
        // Arrange
        var london = Destination("London", "UK", RouteStart, RouteStart.AddHours(1));
        ArrangeRoute([london, Destination("Paris", "France", RouteStart.AddHours(2), RouteStart.AddHours(3))]);

        // Act
        var response = await _sut.GeAsync(RouteStart.AddHours(1));

        // Assert
        response.FromCity.ShouldBe("London");
        response.Country.ShouldBe("UK");
        response.Description.ShouldBe("SantaIsDeliveringPresentsIn|London|UK");
    }

    [Fact]
    public async Task GeAsync_WhenSantaIsBetweenCities_ReturnsOriginAndDestination()
    {
        // Arrange
        var london = Destination("London", "UK", RouteStart, RouteStart.AddHours(1));
        var paris = Destination("Paris", "France", RouteStart.AddHours(3), RouteStart.AddHours(4));
        ArrangeRoute([london, paris, Destination("Rome", "Italy", RouteStart.AddHours(6), RouteStart.AddHours(7))]);

        // Act
        var response = await _sut.GeAsync(RouteStart.AddHours(2));

        // Assert
        response.Status.ShouldBe(SantaStatus.InFlight.ToString());
        response.FromCity.ShouldBe("London");
        response.ToCity.ShouldBe("Paris");
        response.Country.ShouldBe("UK");
        response.Description.ShouldBe("SantaInFlightBetweenCities|London|UK|Paris");
        _stringLocalizerMock.Verify(
            localizer => localizer["SantaInFlightBetweenCities", It.IsAny<object[]>()],
            Times.Once);
    }

    [Fact]
    public async Task GeAsync_WhenNoCityOrFlightIntervalMatches_FallsBackToNorthPole()
    {
        // Arrange
        var route = new List<SantaDestinationDto>
        {
            Destination("London", "UK", RouteStart, RouteStart.AddHours(1)),
            Destination("Paris", "France", RouteStart.AddHours(3), RouteStart.AddHours(4)),
            Destination("Rome", "Italy", RouteStart.AddHours(6), RouteStart.AddHours(7))
        };
        foreach (var destination in route)
        {
            destination.ArrivalUtc = RouteStart.AddYears(-1);
            destination.DepartureUtc = RouteStart.AddYears(-1);
        }

        ArrangeRoute(route);

        // Act
        var response = await _sut.GeAsync(RouteStart.AddHours(2));

        // Assert
        response.Status.ShouldBe(SantaStatus.NorthPole.ToString());
        response.Description.ShouldBe("At North Pole");
        response.FromCity.ShouldBeEmpty();
        response.ToCity.ShouldBeEmpty();
        response.Country.ShouldBeEmpty();
    }

    [Fact]
    public void GetSantaStatus_WhenScheduleIsFromPreviousYear_ShiftsScheduleToCurrentYear()
    {
        // Arrange
        var previousYearRoute = new[]
        {
            Destination("London", "UK", RouteStart.AddYears(-1), RouteStart.AddHours(1).AddYears(-1)),
            Destination("Paris", "France", RouteStart.AddHours(2).AddYears(-1), RouteStart.AddHours(3).AddYears(-1))
        };

        // Act & Assert
        GetSantaLocationHandler.GetSantaStatus(RouteStart, previousYearRoute)
            .ShouldBe(SantaStatus.InFlight);
    }

    private void ArrangeRoute(List<SantaDestinationDto>? destinations = null)
    {
        _trackingDataProviderMock
            .Setup(provider => provider.GetSantaRouteDataAsync())
            .ReturnsAsync(new SantaRouteDto { Destinations = destinations ?? [] });
    }

    private static List<SantaDestinationDto> CreateRoute() =>
    [
        Destination("London", "UK", RouteStart, RouteStart.AddHours(1)),
        Destination("Paris", "France", RouteStart.AddHours(2), RouteStart.AddHours(3))
    ];

    private static SantaDestinationDto Destination(
        string city,
        string region,
        DateTime arrivalUtc,
        DateTime departureUtc) => new()
        {
            City = city,
            Region = region,
            Arrival = new DateTimeOffset(arrivalUtc).ToUnixTimeMilliseconds(),
            ArrivalUtc = arrivalUtc,
            Departure = new DateTimeOffset(departureUtc).ToUnixTimeMilliseconds(),
            DepartureUtc = departureUtc
        };

    private void ConfigureLocalizer()
    {
        _stringLocalizerMock
            .Setup(localizer => localizer[It.IsAny<string>()])
            .Returns((string key) => new LocalizedString(key, key));

        _stringLocalizerMock
            .Setup(localizer => localizer[It.IsAny<string>(), It.IsAny<object[]>()])
            .Returns((string key, object[] arguments) =>
                new LocalizedString(key, $"{key}|{string.Join('|', arguments)}"));
    }
}
