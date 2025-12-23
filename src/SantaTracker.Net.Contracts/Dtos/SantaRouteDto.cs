namespace SantaTracker.Net.Contracts.Dtos
{
    public sealed class SantaRouteDto
    {
        public List<SantaDestinationDto> Destinations { get; set; } = [];
    }

    public sealed class SantaDestinationDto
    {
        /// <summary>
        ///     City.
        /// </summary>
        public string City { get; set; } = "";

        /// <summary>
        ///     Country.
        /// </summary>
        public string Region { get; set; } = "";

        public long Arrival { get; set; }

        public DateTime ArrivalUtc { get; set; }

        public long Departure { get; set; }

        public DateTime DepartureUtc { get; set; }

        public SantaLocationDto Location { get; set; } = new();
    }

    public sealed class SantaLocationDto
    {
        public double Lat { get; set; }

        public double Lng { get; set; }
    }
}