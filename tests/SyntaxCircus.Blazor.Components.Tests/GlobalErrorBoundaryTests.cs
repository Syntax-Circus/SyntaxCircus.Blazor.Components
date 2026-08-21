using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Shouldly;
using SyntaxCircus.Blazor.Components.Feedback;
using Xunit;

namespace SyntaxCircus.Blazor.Components.Tests;

public sealed class GlobalErrorBoundaryTests
{
    [Fact]
    public void Render_WhenChildThrows_RendersCustomFallbackRegions()
    {
        using var context = new BunitContext();
        RenderFragment child = builder =>
        {
            builder.OpenComponent<ThrowingComponent>(0);
            builder.CloseComponent();
        };
        RenderFragment body = builder => builder.AddContent(0, "Custom fallback body");

        var markup = context.Render<GlobalErrorBoundary>(parameters => parameters
            .Add(component => component.ChildContent, child)
            .Add(component => component.Title, "Screen unavailable")
            .Add(component => component.ErrorBodyContent, body))
            .Markup;

        markup.ShouldContain("Screen unavailable");
        markup.ShouldContain("Custom fallback body");
        markup.ShouldContain("data-global-error-actions");
    }

    private sealed class ThrowingComponent : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
            => throw new InvalidOperationException("Expected test failure");
    }
}
