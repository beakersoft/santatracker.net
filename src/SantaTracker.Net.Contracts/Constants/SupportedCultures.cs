using System.Collections.ObjectModel;
using System.Globalization;

namespace SantaTracker.Net.Contracts.Constants;

/// <summary>
/// Clutures we support. Dont add to here unless we have a localisation file
/// for tranlation.
/// </summary>
public static class SupportedCultures
{
    /// <summary>
    /// Supported cultures
    /// </summary>
    public static readonly ReadOnlyCollection<string> Cultures = new(
    [
        "en-US",
        "es-ES"
    ]);

    /// <summary>
    /// Supported culture hashset
    /// </summary>
    public static HashSet<CultureInfo> CulturesHashSet => [.. Cultures.ToList().Select(c => new CultureInfo(c))];
}
