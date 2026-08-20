using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.HtmlRendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using SyntaxCircus.Blazor.Components.Feedback;
using Xunit;

namespace SyntaxCircus.Blazor.Components.Tests;

public sealed class NotFoundViewTests
{
    [Fact]
    public async Task RenderAsync_WithoutParameters_RendersAccessibleDefaults()
    {
        var markup = await RenderAsync([]);

        markup.ShouldContain("class=\"syntax-circus-not-found\"");
        markup.ShouldContain("<h1");
        markup.ShouldContain(">Page not found</h1>");
        markup.ShouldContain("The page you requested could not be found.");
        markup.ShouldContain("<a href=\"/\">Go home</a>");
    }

    [Fact]
    public async Task RenderAsync_WithContentSlots_RendersSlotsInsteadOfDefaultBodyAndActions()
    {
        RenderFragment media = builder => builder.AddContent(0, "Illustration");
        RenderFragment body = builder => builder.AddContent(0, "Custom explanation");
        RenderFragment actions = builder => builder.AddContent(0, "Custom action");
        var markup = await RenderAsync(new Dictionary<string, object?>
        {
            [nameof(NotFoundView.Title)] = "Missing record",
            [nameof(NotFoundView.MediaContent)] = media,
            [nameof(NotFoundView.ChildContent)] = body,
            [nameof(NotFoundView.ActionsContent)] = actions,
        });

        markup.ShouldContain("data-not-found-media");
        markup.ShouldContain("Illustration");
        markup.ShouldContain(">Missing record</h1>");
        markup.ShouldContain("Custom explanation");
        markup.ShouldContain("data-not-found-actions");
        markup.ShouldContain("Custom action");
        markup.ShouldNotContain("The page you requested could not be found.");
        markup.ShouldNotContain(">Go home</a>");
    }

    private static async Task<string> RenderAsync(IReadOnlyDictionary<string, object?> parameters)
    {
        var services = new ServiceCollection();
        await using var provider = services.BuildServiceProvider();
        await using var renderer = new HtmlRenderer(provider, NullLoggerFactory.Instance);

        return await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await renderer.RenderComponentAsync<NotFoundView>(ParameterView.FromDictionary(parameters));
            return output.ToHtmlString();
        });
    }
}
