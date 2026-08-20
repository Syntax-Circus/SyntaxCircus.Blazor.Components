# SyntaxCircus.Blazor.Components

Reusable, semantic components for Blazor applications. The package is **Blazor/Razor-dependent** but deliberately **UI/CSS-framework-agnostic**: it does not bring Bootstrap, Tailwind, stylesheets, routing, or middleware.

## NotFoundView

`NotFoundView` supplies accessible default 404 content while leaving layout, branding, SEO, routing, and HTTP policy to the host application.

```razor
@using SyntaxCircus.Blazor.Components.Feedback

<NotFoundView />
```

Use the host's local `/not-found` page as the router adapter:

```razor
@page "/not-found"
@attribute [AllowAnonymous]

<NotFoundView Title="That page wandered off.">
    <MediaContent>
        <img src="/images/lost-page.svg" alt="A wandering page" />
    </MediaContent>
    <ChildContent>
        <p>The address may be mistyped, moved, or no longer available.</p>
    </ChildContent>
    <ActionsContent>
        <a href="/">Home</a>
        <a href="/support">Support</a>
    </ActionsContent>
</NotFoundView>
```

`HeaderContent` replaces the default heading, `ChildContent` replaces the default description, and `ActionsContent` replaces the default home link. Set `ShowDefaultActions="false"` when the host renders navigation elsewhere. `MediaContent` is rendered above the heading and accepts an image, SVG, or any host-provided template. Provide appropriate alternative text for meaningful media; decorative media should be hidden from assistive technology by the host markup.

Configure the host router with the adapter page:

```razor
<Router AppAssembly="typeof(Program).Assembly" NotFoundPage="typeof(Pages.NotFound)">
    ...
</Router>
```

For a data-loading component that establishes a requested resource is absent, call .NET 10's `NavigationManager.NotFound()` from the host. This component does not make that decision itself.

For direct HTTP requests, verify the host emits `404 Not Found` and renders the router page. Whether to add status-code middleware for non-Blazor endpoints is host-specific; do not add it merely to render this component.

## Contributing

Add a component only when it has a stable, framework-agnostic contract and at least two concrete consumer styles. Do not centralize application layouts, branding, routing, or CSS systems here.
