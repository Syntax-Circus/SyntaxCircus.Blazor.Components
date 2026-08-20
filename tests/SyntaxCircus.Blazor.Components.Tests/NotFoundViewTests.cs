using Bunit;
using Microsoft.AspNetCore.Components;
using Shouldly;
using SyntaxCircus.Blazor.Components.Feedback;
using Xunit;

namespace SyntaxCircus.Blazor.Components.Tests;

public sealed class NotFoundViewTests
{
    [Fact]
    public void Render_WithoutParameters_RendersAccessibleDefaults()
    {
        using var context = new BunitContext();
        var markup = context.Render<NotFoundView>().Markup;

        markup.ShouldContain("class=\"syntax-circus-not-found\"");
        markup.ShouldContain("<h1");
        markup.ShouldContain(">Page not found</h1>");
        markup.ShouldContain("The page you requested could not be found.");
        markup.ShouldContain("<a href=\"/\">Go home</a>");
    }

    [Fact]
    public void Render_WithContentSlots_RendersSlotsInsteadOfDefaultBodyAndActions()
    {
        using var context = new BunitContext();
        RenderFragment media = builder => builder.AddContent(0, "Illustration");
        RenderFragment body = builder => builder.AddContent(0, "Custom explanation");
        RenderFragment actions = builder => builder.AddContent(0, "Custom action");
        var markup = context.Render<NotFoundView>(parameters => parameters
            .Add(component => component.Title, "Missing record")
            .Add(component => component.MediaContent, media)
            .Add(component => component.ChildContent, body)
            .Add(component => component.ActionsContent, actions))
            .Markup;

        markup.ShouldContain("data-not-found-media");
        markup.ShouldContain("Illustration");
        markup.ShouldContain(">Missing record</h1>");
        markup.ShouldContain("Custom explanation");
        markup.ShouldContain("data-not-found-actions");
        markup.ShouldContain("Custom action");
        markup.ShouldNotContain("The page you requested could not be found.");
        markup.ShouldNotContain(">Go home</a>");
    }
}
