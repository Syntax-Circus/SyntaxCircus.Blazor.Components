namespace SyntaxCircus.Blazor.Components.Interop;

/// <summary>
/// Saves generated file content to the browser's downloads by turning it into a client-side
/// blob and triggering a temporary anchor download. Requires an interactive Blazor render mode
/// (Server or WebAssembly) — there is no JavaScript runtime to invoke during static server-side
/// rendering, so calls made from a statically rendered component have no effect.
/// </summary>
public interface IFileDownloadService
{
    /// <summary>
    /// Downloads <paramref name="content"/> as a file named <paramref name="fileName"/>.
    /// </summary>
    /// <param name="fileName">The suggested file name. Must not be empty or whitespace.</param>
    /// <param name="contentType">
    /// The MIME type used for the generated blob. Empty or whitespace falls back to
    /// <c>application/octet-stream</c>.
    /// </param>
    /// <param name="content">The file bytes to download.</param>
    /// <param name="cancellationToken">A token to cancel the JavaScript interop call.</param>
    Task DownloadAsync(
        string fileName,
        string contentType,
        byte[] content,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the remaining contents of <paramref name="content"/> as a file named
    /// <paramref name="fileName"/>. The stream is read to its end but not disposed by this method.
    /// </summary>
    /// <param name="fileName">The suggested file name. Must not be empty or whitespace.</param>
    /// <param name="contentType">
    /// The MIME type used for the generated blob. Empty or whitespace falls back to
    /// <c>application/octet-stream</c>.
    /// </param>
    /// <param name="content">The file content stream to download.</param>
    /// <param name="cancellationToken">A token to cancel the JavaScript interop call.</param>
    Task DownloadAsync(
        string fileName,
        string contentType,
        Stream content,
        CancellationToken cancellationToken = default);
}
