# AGENTS.md

Guidance for AI coding agents and contributors working in
`SyntaxCircus.Blazor.Components`. Read [README.md](README.md) first: it is
the consumer-facing contract. This file explains how to preserve that contract
while changing the package.

## Purpose and boundary

This is a small `net10.0` Razor component library. It is:

- **Blazor/Razor-dependent**;
- **UI/CSS-framework-agnostic**;
- intentionally limited to reusable component markup and behavior.

It is not an application framework. Do not add application layouts, CSS
frameworks, static images, JavaScript, service registration, router wrappers,
HTTP middleware, SEO policy, authorization policy, or domain-specific copy
without a separate, evidence-based design decision.

`NotFoundView` is presentation only. It must never call
`NavigationManager.NotFound()`, mutate `HttpResponse.StatusCode`, infer
whether a resource exists, or choose authentication behavior.

## Repository map

```text
src/SyntaxCircus.Blazor.Components/
  Components/Feedback/NotFoundView.razor  public not-found component
  _Imports.razor                           library-wide Razor imports
  SyntaxCircus.Blazor.Components.csproj    package metadata and pack settings

tests/SyntaxCircus.Blazor.Components.Tests/
  NotFoundViewTests.cs                     bUnit rendering contract tests
  SyntaxCircus.Blazor.Components.Tests.csproj

README.md                                  consumer install/API/integration guide
AGENTS.md                                  contributor and agent instructions
Directory.Build.props                      analyzers and warning policy
Directory.Packages.props                   central package versions
GitVersion.yml                             package versioning policy
```

## Commands

Run from the repository root:

```bash
dotnet restore SyntaxCircus.Blazor.Components.slnx
dotnet build SyntaxCircus.Blazor.Components.slnx
dotnet test SyntaxCircus.Blazor.Components.slnx
dotnet pack src/SyntaxCircus.Blazor.Components/SyntaxCircus.Blazor.Components.csproj
```

`GitVersion.MsBuild` derives package versions from Git history. Do not
hand-edit package versions. A build requires a repository with at least one
commit. GitVersion is disabled only for NCrunch through
`Directory.Build.props`.

## Public API rules

The namespace `SyntaxCircus.Blazor.Components.Feedback` and every public
`NotFoundView` parameter are consumer API.

- Treat renames, removals, type changes, default changes, or changed slot
  precedence as breaking changes.
- Keep the component free of Bootstrap/Tailwind classes, assumptions about a
  host layout, and product-specific strings.
- Prefer ordinary `string`, `bool`, and `RenderFragment` parameters over
  services, configuration, or hidden host conventions.
- Use replacement slots for genuinely varying host markup. Do not add a
  parameter solely to encode one application’s branding.
- Require evidence from at least two concrete consumer styles before adding a
  new component or widening a component contract.

When public output changes, update all of the following in the same change:

1. `README.md` parameter table and applicable examples;
2. bUnit coverage in `NotFoundViewTests.cs`;
3. accessibility notes if heading, landmark, link, or media semantics change.

## `NotFoundView` rendering contract

Maintain these rules unless intentionally making a documented breaking change:

1. The outer element is a semantic `section`.
2. Without `HeaderContent`, the component renders a default `h1` from
   `Title` and references it with `aria-labelledby`.
3. `HeaderContent` replaces that heading entirely; the host must provide its
   own heading semantics.
4. `ChildContent` replaces `Description`.
5. `ActionsContent` replaces the default home-link region.
6. `ShowDefaultActions == false` suppresses default actions when no action
   slot is present.
7. `MediaContent` appears before the header.
8. The component supplies no styling beyond a stable outer CSS class.

Use semantic, accessible markup. Do not assume an image is decorative, a link
is safe to generate, or a host CSS class exists.

## Correct 404 integration

Keep the three host concerns separate:

| Concern | Owner |
|---|---|
| Render familiar not-found markup | `NotFoundView` |
| Select a local page, layout, title, SEO metadata, and authorization | Host `/not-found` adapter |
| Handle unmatched component routes | Host `Router.NotFoundPage` |
| Signal a matched route whose data is absent | Host calls .NET 10 `NavigationManager.NotFound()` |
| Produce/re-execute an HTTP error response for non-Blazor endpoints | Host pipeline, if appropriate |

Do not add `UseStatusCodePagesWithReExecute`, a router component, or
`NavigationManager` to this library. Status-code re-execution can interact
with fallback authorization policies and API response formats; only a
consuming application can make that pipeline decision safely.

When updating the README’s router or middleware examples, preserve this
distinction. Never claim that rendering `NotFoundView` itself produces an HTTP
404.

## Testing

Tests use xUnit v3, Shouldly, and bUnit.

- Prefer bUnit render tests for parameter defaults, replacement slots, and
  semantic markup.
- Use `ShouldContain`/`ShouldNotContain` in the existing style.
- Add a test for every public rendering behavior added or changed.
- This package cannot prove a consuming application returns HTTP 404. Host
  applications need `WebApplicationFactory` or equivalent integration tests
  that request a nonexistent URL and assert both status and body.

Do not add a web server, database, mock HTTP stack, or JavaScript test
dependency merely to test a pure Razor component.

## Build and dependency conventions

- `Directory.Build.props` enables nullable references, implicit usings,
  latest recommended analyzers, and `TreatWarningsAsErrors`.
- Add package versions only to `Directory.Packages.props`; do not put
  `Version=` attributes on `PackageReference` items.
- Keep `GitVersion.MsBuild` private to the package build.
- xUnit v3 is pinned below 4.0.0 because current NCrunch adapters require the
  older API shape. Do not raise it without checking NCrunch compatibility.
- Keep README and `AGENTS.md` accurate when adding a package dependency or
  changing build/test requirements.

## Completion checklist

Before declaring a change complete:

1. Confirm the public component contract is still reflected in README tables,
   examples, and `AGENTS.md`.
2. Run `dotnet test SyntaxCircus.Blazor.Components.slnx`.
3. Run `dotnet pack` if package metadata, README packing, or public Razor
   content changed.
4. Check that no host-specific CSS, routing, middleware, secrets, or build
   artifacts were added.
