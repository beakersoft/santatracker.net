using System.Text.Json.Serialization;

namespace SantaTracker.Net.Contracts.Dtos;

/// <summary>The year-independent route stored in santa-route.json.</summary>
public sealed class LocalSantaRouteDto
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>Base local date in MM-dd format, without a year.</summary>
    [JsonPropertyName("localBaseDate")]
    public string LocalBaseDate { get; set; } = string.Empty;

    [JsonPropertyName("departure")]
    public LocalSantaDepartureDto Departure { get; set; } = new();

    [JsonPropertyName("return")]
    public LocalSantaReturnDto Return { get; set; } = new();

    [JsonPropertyName("timingPolicy")]
    public LocalSantaTimingPolicyDto TimingPolicy { get; set; } = new();

    [JsonPropertyName("stops")]
    public List<LocalSantaStopDto> Stops { get; set; } = [];
}

public sealed class LocalSantaStopDto
{
    /// <summary>Route position; null when absent from the source file.</summary>
    [JsonPropertyName("stopNumber")]
    public int? StopNumber { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("countryCode")]
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>IANA time-zone identifier.</summary>
    [JsonPropertyName("timeZone")]
    public string TimeZone { get; set; } = string.Empty;

    /// <summary>Local arrival time in HH:mm format.</summary>
    [JsonPropertyName("arrivalLocal")]
    public string ArrivalLocal { get; set; } = string.Empty;

    [JsonPropertyName("dayOffset")]
    public int DayOffset { get; set; }

    [JsonPropertyName("durationMinutes")]
    public int DurationMinutes { get; set; }

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    /// <summary>Optional GeoNames reference; not present for every stop.</summary>
    [JsonPropertyName("geonameId")]
    public int? GeonameId { get; set; }
}

public sealed class LocalSantaDepartureDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("timeZone")]
    public string TimeZone { get; set; } = string.Empty;

    [JsonPropertyName("departureLocal")]
    public string DepartureLocal { get; set; } = string.Empty;

    [JsonPropertyName("dayOffset")]
    public int DayOffset { get; set; }
}

public sealed class LocalSantaReturnDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("timeZone")]
    public string TimeZone { get; set; } = string.Empty;

    [JsonPropertyName("arrivalLocal")]
    public string ArrivalLocal { get; set; } = string.Empty;

    [JsonPropertyName("dayOffset")]
    public int DayOffset { get; set; }
}

public sealed class LocalSantaTimingPolicyDto
{
    [JsonPropertyName("targetLocal")]
    public string TargetLocal { get; set; } = string.Empty;

    [JsonPropertyName("earliestLocal")]
    public string EarliestLocal { get; set; } = string.Empty;

    [JsonPropertyName("latestLocal")]
    public string LatestLocal { get; set; } = string.Empty;

    [JsonPropertyName("fixedAnchorIds")]
    public List<string> FixedAnchorIds { get; set; } = [];

    [JsonPropertyName("regionalEarliestLocal")]
    public string RegionalEarliestLocal { get; set; } = string.Empty;

    [JsonPropertyName("regionalLatestLocal")]
    public string RegionalLatestLocal { get; set; } = string.Empty;
}
