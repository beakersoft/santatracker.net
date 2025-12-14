namespace SantaTracker.Net.Contracts
{
    /// <summary>
    ///     Response to get Santa's current location.
    /// </summary>
    public record GetSantaLocationResponse
    {
        /// <summary>
        ///     Current City.
        /// </summary>
        public string City { get; init; } = string.Empty;

        /// <summary>
        ///     Current Country.
        /// </summary>
        public string Country { get; init; } = string.Empty;
    }
}