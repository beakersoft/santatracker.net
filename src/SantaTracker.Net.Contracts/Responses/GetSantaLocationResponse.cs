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
    }
}