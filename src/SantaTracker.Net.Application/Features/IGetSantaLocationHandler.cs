using SantaTracker.Net.Contracts;

namespace SantaTracker.Net.Application.Features
{
    /// <summary>
    ///     Handler for getting Santa's current location.
    /// </summary>
    public interface IGetSantaLocationHandler
    {
        /// <summary>
        ///     Get Santa's current location.
        /// </summary>
        /// <returns></returns>
        Task<GetSantaLocationResponse> GeAsync();
    }

    /// <inheritdoc />
    public sealed class GetSantaLocationHandler : IGetSantaLocationHandler
    {
        public Task<GetSantaLocationResponse> GeAsync()
        {
            throw new NotImplementedException();
        }
    }
}