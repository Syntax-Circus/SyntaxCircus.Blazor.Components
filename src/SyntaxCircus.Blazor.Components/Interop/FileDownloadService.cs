using Microsoft.JSInterop;

namespace SyntaxCircus.Blazor.Components.Interop;

/// <summary>
/// Default <see cref="IFileDownloadService"/> implementation. Lazily imports the
/// <c>fileDownload.js</c> ES module shipped as a static web asset of this package and calls its
/// <c>downloadFile</c> export via a <see cref="DotNetStreamReference"/>.
/// </summary>
public sealed class FileDownloadService(IJSRuntime jsRuntime) : IFileDownloadService, IAsyncDisposable
{
    private const string ModulePath = "./_content/SyntaxCircus.Blazor.Components/fileDownload.js";
    private const string DefaultContentType = "application/octet-stream";

    private Task<IJSObjectReference>? _moduleTask;

    /// <inheritdoc />
    public async Task DownloadAsync(
        string fileName,
        string contentType,
        byte[] content,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);

        using var stream = new MemoryStream(content, writable: false);
        await DownloadCoreAsync(fileName, contentType, stream, leaveOpen: false, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task DownloadAsync(
        string fileName,
        string contentType,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);

        return DownloadCoreAsync(fileName, contentType, content, leaveOpen: true, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_moduleTask is null)
        {
            return;
        }

        try
        {
            var module = await _moduleTask.ConfigureAwait(false);
            await module.DisposeAsync().ConfigureAwait(false);
        }
        catch (JSDisconnectedException)
        {
            // The Blazor Server circuit is already gone; there's no client left to clean up.
        }
    }

    private async Task DownloadCoreAsync(
        string fileName,
        string contentType,
        Stream content,
        bool leaveOpen,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        var effectiveContentType = string.IsNullOrWhiteSpace(contentType) ? DefaultContentType : contentType;
        var module = await GetModuleAsync().ConfigureAwait(false);

        using var streamRef = new DotNetStreamReference(content, leaveOpen);
        await module.InvokeVoidAsync(
                "downloadFile", cancellationToken, fileName, effectiveContentType, streamRef)
            .ConfigureAwait(false);
    }

    private Task<IJSObjectReference> GetModuleAsync() =>
        _moduleTask ??= jsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath).AsTask();
}
