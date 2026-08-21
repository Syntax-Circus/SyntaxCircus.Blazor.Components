# Enhancement: extract general error boundaries from `SyntaxCircus.Blazor.Auth`

**Status:** implemented
**Date:** 2026-08-20

## Decision

`LoggingErrorBoundary`, `GlobalErrorBoundary`, and `GlobalErrorView` handle Blazor rendering failures and recovery. They have no dependency on OIDC, tokens, cookies, or session-expiry state, so they belong in `SyntaxCircus.Blazor.Components.Feedback`.

The feature is independently consumed by DivDug and Sinforgiver, and Sinforgiver maintains a local duplicate. This establishes the required evidence for a general component contract.

## Contract

The extracted components preserve logging, retry recovery, and navigation recovery while removing Bootstrap markup. Hosts style stable semantic markup and supply replacement fragments for header, body, actions, and opt-in exception details. Default fallback output never renders exception details.

## Migration

The Auth namespace `SyntaxCircus.Blazor.Auth.Components.Errors` is removed as a breaking change. Consumers install `SyntaxCircus.Blazor.Components`, import `SyntaxCircus.Blazor.Components.Feedback`, replace the namespace, and rename boundary body content from `ChildContent` to `ErrorBodyContent` when customizing the fallback body. `AppName` is intentionally removed because it had no rendering or behavior effect.
