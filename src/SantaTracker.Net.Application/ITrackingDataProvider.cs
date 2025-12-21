using SantaTracker.Net.Contracts.Dtos;

namespace SantaTracker.Net.Application
{
    /// <summary>
    /// Provider for santa data.
    /// </summary>
    public interface ITrackingDataProvider
    {
        /// <summary>
        /// Get all the santa route data.
        /// </summary>
        /// <returns></returns>
        Task<SantaRouteDto> GetSantaRouteDataAsync();
    }
}