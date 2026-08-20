# SyntaxCircus.Blazor.Components

Reusable, semantic Razor components for Blazor applications.

The package is **Blazor/Razor-dependent** and targets `net10.0`, but is deliberately **UI/CSS-framework-agnostic**. It does not bring Bootstrap, Tailwind, stylesheets, a layout, a router, middleware, or a design system. A consuming application owns all of those choices.

> **No support guaranteed.** Published as-is and maintained on a best-effort basis. Issues and pull requests are welcome, but there is no SLA.

## Install

```bash
dotnet add package SyntaxCircus.Blazor.Components
```

Import the component namespace in a page or in the application's `_Imports.razor`:

```razor
@using SyntaxCircus.Blazor.Components.Feedback
```

No service registration, JavaScript, CSS import, or middleware registration is required.

## Components

| Component | Namespace | Purpose |
|---|---|---|
| `NotFoundView` | `SyntaxCircus.Blazor.Components.Feedback` | Accessible, customizable markup for a not-found experience. |

## `NotFoundView`

`NotFoundView` renders an accessible not-found section with sensible defaults. It is **presentation only**: it never decides whether a resource is missing, navigates, sets an HTTP response status, changes a document title, or applies authorization.

### Minimal usage

```razor
<NotFoundView />
```

The default output contains:

- a `<section>` with the `syntax-circus-not-found` class;
- an `h1` reading “Page not found”;
- “The page you requested could not be found.”;
- a “Go home” link to `/`.

Style the default class in the host application, or supply `CssClass`:

```razor
<NotFoundView CssClass="my-product-not-found" />
```

### Public parameters

| Parameter | Type | Default | Behavior |
|---|---|---|---|
| `Title` | `string` | `"Page not found"` | Text for the default `h1`. Ignored when `HeaderContent` is supplied. |
| `Description` | `string?` | `"The page you requested could not be found."` | Text for the default paragraph. Ignored when `ChildContent` is supplied; set to `null` or empty to omit it. |
| `HomeHref` | `string?` | `"/"` | Destination for the default action link. Set to `null` or empty to omit the default link. |
| `HomeLabel` | `string` | `"Go home"` | Text for the default action link. |
| `CssClass` | `string` | `"syntax-circus-not-found"` | Class applied to the outer `section`. |
| `MediaContent` | `RenderFragment?` | `null` | Optional media/template rendered before the heading. |
| `HeaderContent` | `RenderFragment?` | `null` | Replaces the default `h1`; the host owns heading semantics when it is used. |
| `ChildContent` | `RenderFragment?` | `null` | Replaces the default description region. |
| `ActionsContent` | `RenderFragment?` | `null` | Replaces the default action-link region. |
| `ShowDefaultActions` | `bool` | `true` | Set to `false` to suppress the default action when `ActionsContent` is not supplied. |

### Content precedence

The component intentionally uses predictable replacement rules:

1. `HeaderContent` replaces the generated `h1`; otherwise `Title` is rendered as the `h1`.
2. `ChildContent` replaces the generated description; otherwise non-empty `Description` is rendered.
3. `ActionsContent` replaces the default action region; otherwise the home link is rendered only when `ShowDefaultActions` is `true` and `HomeHref` is non-empty.
4. `MediaContent`, if present, is always rendered before the header.

Use a slot when the host needs richer markup. Do not duplicate the default region and the replacement slot unless that repetition is intentional.

### Text and action customization

```razor
<NotFoundView Title="That page wandered off."
              Description="Check the address or choose a destination below."
              HomeHref="/catalog"
              HomeLabel="Browse the catalog">
    <ActionsContent>
        <a class="button" href="/catalog">Browse the catalog</a>
        <a class="button button-secondary" href="/support">Contact support</a>
    </ActionsContent>
</NotFoundView>
```

Because `ActionsContent` is present, the generated “Go home” link is not rendered.

### Branded header and body

Use the header and body slots to retain a host design system without making this package depend on it:

```razor
<NotFoundView ShowDefaultActions="false">
    <HeaderContent>
        <div class="hero hero--not-found">
            <p class="eyebrow">Page not found</p>
            <h1>That page wandered off.</h1>
            <p>The address may be mistyped, moved, or no longer available.</p>
        </div>
    </HeaderContent>
    <ChildContent>
        <div class="content-card">
            <h2>Try one of these instead</h2>
            <a href="/">Home</a>
            <a href="/support">Support</a>
        </div>
    </ChildContent>
</NotFoundView>
```

When supplying `HeaderContent`, include an appropriate heading in that fragment. The component does not generate a second `h1` and therefore does not assign `aria-labelledby` to the outer section.

### Images, illustrations, and arbitrary media

`MediaContent` accepts any Razor fragment. The package does not prescribe an image component, image source, dimensions, or alt text policy:

```razor
<NotFoundView>
    <MediaContent>
        <img src="/images/lost-page.svg"
             width="320"
             height="180"
             alt="A map with a missing destination" />
    </MediaContent>
</NotFoundView>
```

Give meaningful media meaningful alternative text. Mark purely decorative media as hidden from assistive technology in the host markup:

```razor
<MediaContent>
    <img src="/images/confetti.svg" alt="" aria-hidden="true" />
</MediaContent>
```

## Wiring a Blazor 404 flow

Rendering a not-found view and producing an HTTP 404 are related but separate responsibilities. Choose the path that matches the source of the missing content.

### 1. An unmatched Blazor route

Create a local adapter page in the host application. The local page owns route metadata, layout, SEO metadata, authorization, and product-specific content:

```razor
@page "/not-found"
@layout MainLayout
@attribute [AllowAnonymous]

<PageTitle>Page not found</PageTitle>

<NotFoundView Title="That page wandered off."
              Description="The address may be mistyped, moved, or no longer available.">
    <ActionsContent>
        <a href="/">Home</a>
        <a href="/support">Support</a>
    </ActionsContent>
</NotFoundView>
```

`[AllowAnonymous]` is appropriate only when the host intends the adapter to be public. Applications that use a fallback authorization policy must also ensure their Razor Components endpoint and status-code re-execution path permit the intended request; page metadata alone may not run before endpoint authorization.

Point the root router at that local adapter:

```razor
<Router AppAssembly="typeof(Program).Assembly"
        NotFoundPage="typeof(Pages.NotFound)">
    <Found Context="routeData">
        <RouteView RouteData="routeData"
                   DefaultLayout="typeof(Layout.MainLayout)" />
        <FocusOnNavigate RouteData="routeData" Selector="h1" />
    </Found>
</Router>
```

Use `AuthorizeRouteView` instead of `RouteView` if the host application already requires route authorization:

```razor
<AuthorizeRouteView RouteData="routeData"
                    DefaultLayout="typeof(Layout.MainLayout)">
    <NotAuthorized>
        <RedirectToLogin />
    </NotAuthorized>
</AuthorizeRouteView>
```

In .NET 10, `Router.NotFoundPage` is the appropriate route-miss integration point. Add an integration test that requests a nonexistent public route and asserts both the expected page content and `HttpStatusCode.NotFound`.

### 2. A page or component discovers a missing resource

The router can match a route while a database/API lookup later establishes that the requested resource does not exist. In that case, the host decides to signal not-found:

```razor
@page "/products/{Slug}"
@inject NavigationManager Navigation
@inject IProductClient Products

@code {
    [Parameter]
    public string Slug { get; set; } = string.Empty;

    protected override async Task OnParametersSetAsync()
    {
        var product = await Products.GetBySlugAsync(Slug);

        if (product is null)
        {
            Navigation.NotFound();
            return;
        }

        // Render the product.
    }
}
```

`NavigationManager.NotFound()` is a .NET 10 host API. It tells Blazor that the current route result is not found; it is not called by `NotFoundView`, because a presentation component cannot know whether a domain resource is absent.

If the missing result is an authorization or privacy boundary, do **not** turn it into a public 404 without making that product/security decision explicitly. Preserve the host’s established behavior for `401`, `403`, and non-enumerating resources.

### 3. A non-Blazor endpoint or static-resource response is 404

API endpoints, file middleware, reverse proxies, and static-resource handling can generate a 404 outside the Blazor router. `NotFoundView` does not intercept those responses.

For a host that deliberately wants status-code re-execution to a local adapter, configure it in the HTTP pipeline:

```csharp
app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);
```

This middleware preserves the original status code while re-executing the request path. It is host-specific and has important consequences:

- register it at the deliberate pipeline location for the application, before endpoint mapping;
- ensure the `/not-found` re-execution path is compatible with the application’s authorization policy;
- do not use it merely because a Blazor router already has `NotFoundPage`;
- do not apply an HTML error-page strategy to API endpoints that must return problem details, JSON, or an empty response;
- test the direct request, including status code and body, rather than assuming the rendered component changed the status.

For many component applications, `Router.NotFoundPage` covers unmatched UI routes and status-code middleware is unnecessary. Keep the decision local to the host.

## Accessibility and semantics

- The default heading is an `h1`, and the default outer section references it with `aria-labelledby`.
- A custom header must contain a meaningful heading; the host owns the relationship between custom header content and surrounding landmarks.
- Default links are standard anchors. Host-provided action controls must remain keyboard-operable and have clear text or accessible names.
- The package does not set color, spacing, contrast, focus indicators, or responsive behavior. The host CSS must provide those qualities.

## What this package does not do

This library deliberately does not:

- register services or middleware;
- call `NavigationManager.NotFound()`;
- set `HttpResponse.StatusCode`;
- choose a layout, render mode, router, authorization policy, SEO metadata, canonical URL, or robots directive;
- ship a CSS framework, image assets, icon library, or design system;
- translate or localize copy automatically.

These exclusions keep the public component contract reusable across server-rendered, interactive, authenticated, public, and mobile Blazor hosts.

## Validation

```bash
dotnet test SyntaxCircus.Blazor.Components.slnx
dotnet pack src/SyntaxCircus.Blazor.Components/SyntaxCircus.Blazor.Components.csproj
```

The test suite covers default markup, replacement slots, and action suppression. Consumers should additionally test their own router and HTTP pipeline behavior.

## Contributing

Add a component only when it has a stable, UI/CSS-framework-agnostic contract and at least two concrete consumer styles. Keep application layouts, branding, routing, HTTP middleware, and CSS systems in host applications.

See [AGENTS.md](AGENTS.md) for contributor and AI-agent guidance.
