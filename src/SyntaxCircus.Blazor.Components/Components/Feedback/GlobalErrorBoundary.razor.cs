using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace SyntaxCircus.Blazor.Components.Feedback;

/// <summary>
/// Logs rendering failures and renders a recoverable, host-customizable fallback view.
/// </summary>
public partial class GlobalErrorBoundary : IDisposable
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Parameter, EditorRequired]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string BoundaryName { get; set; } = "application UI";

    [Parameter]
    public string Title { get; set; } = "We hit an unexpected snag.";

    [Parameter]
    public string? Description { get; set; } = "Something went wrong rendering this screen. Try again to recover without reloading.";

    [Parameter]
    public string RetryLabel { get; set; } = "Try again";

    [Parameter]
    public string? HomeHref { get; set; } = "/";

    [Parameter]
    public string HomeLabel { get; set; } = "Go home";

    [Parameter]
    public string CssClass { get; set; } = "syntax-circus-global-error";

    [Parameter]
    public RenderFragment? HeaderContent { get; set; }

    [Parameter]
    public RenderFragment? ErrorBodyContent { get; set; }

    [Parameter]
    public RenderFragment? ActionsContent { get; set; }

    [Parameter]
    public RenderFragment<Exception>? ExceptionContent { get; set; }

    private LoggingErrorBoundary? errorBoundary;
    private int renderVersion;

    protected override void OnInitialized()
    {
        Navigation.LocationChanged += HandleLocationChanged;
    }

    private Task RecoverAsync()
    {
        renderVersion++;
        errorBoundary?.Recover();
        return Task.CompletedTask;
    }

    private void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        renderVersion++;
        errorBoundary?.Recover();
    }

    public void Dispose()
    {
        Navigation.LocationChanged -= HandleLocationChanged;
        GC.SuppressFinalize(this);
    }
}
