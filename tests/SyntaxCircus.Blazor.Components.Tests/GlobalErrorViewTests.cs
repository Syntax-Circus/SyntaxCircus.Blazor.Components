using Bunit;
using Microsoft.AspNetCore.Components;
using Shouldly;
using SyntaxCircus.Blazor.Components.Feedback;
using Xunit;

namespace SyntaxCircus.Blazor.Components.Tests;

public sealed class GlobalErrorViewTests
{
    [Fact]
    public void Render_WithoutParameters_RendersSemanticDefaultsWithoutExceptionDetails()
    {
        using var context = new BunitContext();
        var markup = context.Render<GlobalErrorView>(parameters => parameters
            .Add(component => component.Exception, new InvalidOperationException("Sensitive detail")))
            .Markup;

        markup.ShouldContain("class=\"syntax-circus-global-error\"");
        markup.ShouldContain("role=\"alert\"");
        markup.ShouldContain("We hit an unexpected snag.");
        markup.ShouldContain("Try again");
        markup.ShouldContain("<a href=\"/\">Go home</a>");
        markup.ShouldNotContain("Sensitive detail");
    }

    [Fact]
    public void Render_WithSlots_ReplacesDefaultRegionsAndRendersOptInExceptionDetails()
    {
        using var context = new BunitContext();
        RenderFragment header = builder => builder.AddMarkupContent(0, "<h1>Custom heading</h1>");
        RenderFragment body = builder => builder.AddContent(0, "Custom explanation");
        RenderFragment actions = builder => builder.AddMarkupContent(0, "<a href=\"/support\">Support</a>");
        RenderFragment<Exception> details = exception => builder => builder.AddContent(0, exception.Message);

        var markup = context.Render<GlobalErrorView>(parameters => parameters
            .Add(component => component.CssClass, "product-error")
            .Add(component => component.Exception, new InvalidOperationException("Debug detail"))
            .Add(component => component.HeaderContent, header)
            .Add(component => component.ChildContent, body)
            .Add(component => component.ActionsContent, actions)
            .Add(component => component.ExceptionContent, details))
            .Markup;

        markup.ShouldContain("class=\"product-error\"");
        markup.ShouldContain("Custom heading");
        markup.ShouldContain("Custom explanation");
        markup.ShouldContain("Debug detail");
        markup.ShouldContain("href=\"/support\"");
        markup.ShouldNotContain("We hit an unexpected snag.");
        markup.ShouldNotContain(">Try again</button>");
    }

    [Fact]
    public void Render_WithEmptyHomeHref_SuppressesOnlyTheDefaultHomeAction()
    {
        using var context = new BunitContext();
        var markup = context.Render<GlobalErrorView>(parameters => parameters
            .Add(component => component.HomeHref, string.Empty))
            .Markup;

        markup.ShouldContain(">Try again</button>");
        markup.ShouldNotContain(">Go home</a>");
    }
}
