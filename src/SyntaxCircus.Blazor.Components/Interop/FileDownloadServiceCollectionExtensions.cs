using Microsoft.Extensions.DependencyInjection;

namespace SyntaxCircus.Blazor.Components.Interop;

/// <summary>
/// Registration helper for <see cref="IFileDownloadService"/>.
/// </summary>
public static class FileDownloadServiceCollectionExtensions
{
    /// <summary>
    /// Registers a scoped <see cref="IFileDownloadService"/>/<see cref="FileDownloadService"/>.
    /// </summary>
    public static IServiceCollection AddFileDownload(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IFileDownloadService, FileDownloadService>();

        return services;
    }
}
