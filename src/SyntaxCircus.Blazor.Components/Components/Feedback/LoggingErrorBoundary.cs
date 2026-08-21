using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace SyntaxCircus.Blazor.Components.Feedback;

/// <summary>
/// An <see cref="ErrorBoundary"/> that logs unhandled rendering exceptions.
/// </summary>
public sealed class LoggingErrorBoundary : ErrorBoundary
{
    [Inject]
    private ILogger<LoggingErrorBoundary> Logger { get; set; } = default!;

    [Parameter]
    public string BoundaryName { get; set; } = "application UI";

    protected override Task OnErrorAsync(Exception exception)
    {
        Logger.LogError(exception, "Unhandled exception reached {BoundaryName}.", BoundaryName);
        return Task.CompletedTask;
    }
}
