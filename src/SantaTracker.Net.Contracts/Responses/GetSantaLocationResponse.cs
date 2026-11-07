using SantaTracker.Net.Contracts.Enums;

namespace SantaTracker.Net.Contracts.Responses
{
    /// <summary>
    ///     Response to get Santa's current location.
    /// </summary>
    public record GetSantaLocationResponse
    {
        /// <summary>
        /// Description of what Santa is doing.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        ///     Current City.
        /// </summary>
        public string FromCity { get; init; } = string.Empty;

        /// <summary>
        ///     Next City.
        /// </summary>
        public string ToCity { get; init; } = string.Empty;

        /// <summary>
        ///     Current Country.
        /// </summary>
        public string Country { get; init; } = string.Empty;

        /// <summary>
        /// What is santa doing now.
        /// </summary>
        public string Status { get; init; } = string.Empty;

        /// <summary>
        ///     Santa's current latitude.
        /// </summary>
        public double Latitude { get; init; }

        /// <summary>
        ///     Santa's current longitude.
        /// </summary>
        public double Longitude { get; init; }

        // North Pole location for when Santa is at the North Pole or has finished delivering presents.
        public static GetSantaLocationResponse NorthPoleLocation(string description, string status)
        {
            return new GetSantaLocationResponse
            {
                //Description = "At North Pole",
                Description = description,
                //Status = SantaStatus.NorthPole.ToString(),
                Status = status,
                Latitude = 90,
                Longitude = 0
            };
        }
    }
}
