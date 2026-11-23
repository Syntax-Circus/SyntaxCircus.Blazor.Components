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

    private static readonly string[] DefaultTexts =
    [
        "Rejoining the server",
        "Rejoin failed",
        "Trying again in",
        "Failed to rejoin",
        "The session has been paused",
        "Failed to resume",
        ">Retry<",
        ">Resume<",
    ];

    [Fact]
    public void Render_DefaultMode_ContainsNoDeferHooks()
    {
        using var context = new BunitContext();
        var markup = context.Render<ReconnectModal>().Markup;

        markup.ShouldNotContain("data-reconnect-default");
        foreach (var text in DefaultTexts)
        {
            markup.ShouldContain(text);
        }
    }

    [Fact]
    public void Render_DeferMessages_OmitsDefaultTextButKeepsStructure()
    {
        using var context = new BunitContext();
        var markup = context.Render<ReconnectModal>(parameters => parameters
            .Add(component => component.DeferMessages, true)).Markup;

        foreach (var text in DefaultTexts)
        {
            markup.ShouldNotContain(text);
        }

        foreach (var state in new[] { "first retrying", "first", "retrying", "failed", "paused", "resume-failed" })
        {
            markup.ShouldContain($"data-reconnect-state=\"{state}\"");
        }

        markup.ShouldContain("data-reconnect-action=\"retry\"");
        markup.ShouldContain("data-reconnect-action=\"resume\"");
        markup.ShouldContain("aria-labelledby=\"reconnect-");
        markup.ShouldContain("id=\"reconnect-");
        foreach (var key in new[] { "first", "retrying", "failed", "paused", "resume-failed", "retry-label", "resume-label" })
        {
            markup.ShouldContain($"data-reconnect-default=\"{key}\"");
        }
    }

    [Fact]
    public void Render_DeferMessages_StillRendersCustomFragments()
    {
        using var context = new BunitContext();
        RenderFragment content = builder => builder.AddContent(0, "Custom content");
        RenderFragment retryAction = builder => builder.AddMarkupContent(0, "<button data-reconnect-action=\"retry\">Try again</button>");

        var markup = context.Render<ReconnectModal>(parameters => parameters
            .Add(component => component.DeferMessages, true)
            .Add(component => component.FirstAttemptContent, content)
            .Add(component => component.RetryActionContent, retryAction))
            .Markup;

        markup.ShouldContain("Custom content");
        markup.ShouldContain("Try again");
        markup.ShouldNotContain("data-reconnect-default=\"first\"");
        markup.ShouldNotContain("data-reconnect-default=\"retry-label\"");
        markup.ShouldContain("data-reconnect-default=\"failed\"");
    }
}
