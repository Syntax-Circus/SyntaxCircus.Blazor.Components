using Bunit;
using Microsoft.AspNetCore.Components;
using Shouldly;
using SyntaxCircus.Blazor.Components.Feedback;
using Xunit;

namespace SyntaxCircus.Blazor.Components.Tests;

public sealed class ReconnectModalTests
{
    [Fact]
    public void Render_WithoutParameters_RendersBlazorReconnectHooksAndDefaults()
    {
        using var context = new BunitContext();
        var markup = context.Render<ReconnectModal>().Markup;

        markup.ShouldContain("id=\"components-reconnect-modal\"");
        markup.ShouldContain("class=\"syntax-circus-reconnect-modal\"");
        markup.ShouldContain("components-seconds-to-next-attempt");
        markup.ShouldContain("data-reconnect-action=\"retry\"");
        markup.ShouldContain("data-reconnect-action=\"resume\"");
        markup.ShouldContain("Rejoining the server...");
    }

    [Fact]
    public void Render_WithContentSlots_ReplacesEveryStateAndActionDefault()
    {
        using var context = new BunitContext();
        RenderFragment content = builder => builder.AddContent(0, "Custom content");
        RenderFragment retryAction = builder => builder.AddMarkupContent(0, "<button data-reconnect-action=\"retry\">Try again</button>");
        RenderFragment resumeAction = builder => builder.AddMarkupContent(0, "<button data-reconnect-action=\"resume\">Continue</button>");

        var markup = context.Render<ReconnectModal>(parameters => parameters
            .Add(component => component.CssClass, "product-reconnect")
            .Add(component => component.LoadingContent, content)
            .Add(component => component.FirstAttemptContent, content)
            .Add(component => component.RetryingContent, content)
            .Add(component => component.FailedContent, content)
            .Add(component => component.PausedContent, content)
            .Add(component => component.ResumeFailedContent, content)
            .Add(component => component.RetryActionContent, retryAction)
            .Add(component => component.ResumeActionContent, resumeAction))
            .Markup;

        markup.ShouldContain("class=\"product-reconnect\"");
        markup.ShouldContain("Custom content");
        markup.ShouldContain("Try again");
        markup.ShouldContain("Continue");
        markup.ShouldNotContain("Rejoining the server...");
        markup.ShouldNotContain("Failed to rejoin. Please retry or reload the page.");
        markup.ShouldNotContain(">Retry</button>");
        markup.ShouldNotContain(">Resume</button>");
    }
}
