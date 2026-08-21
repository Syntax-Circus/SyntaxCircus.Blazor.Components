# Enhancement: extract `ReconnectModal` from `SyntaxCircus.Blazor.Auth`

**Status:** implemented
**Date:** 2026-08-20

## Decision

`ReconnectModal` is a Blazor circuit-lifecycle concern, not an authentication concern. It is moved from `SyntaxCircus.Blazor.Auth.Components.Layout` to `SyntaxCircus.Blazor.Components.Feedback`.

The extraction is supported by existing usage in multiple independent applications, including DivDug and Sinforgiver. Cmsify keeps a local implementation because its branded dark/light dialog needs complete product control.

## Component contract

The new component owns the standard Blazor reconnect, retry, pause, resume, and rejected-circuit behavior. It exposes an outer `CssClass` and replacement fragments for every visible state and retry/resume action. Hosts that replace an action must include an element with `data-reconnect-action="retry"` or `data-reconnect-action="resume"` so the supplied behavior remains wired.

The component intentionally provides no visual CSS. Hosts style the stable class and their supplied markup, retaining framework-agnostic theming. Its small JavaScript module is an explicit exception to the general package boundary because opening a native dialog and invoking Blazor's reconnection APIs cannot be achieved with Razor markup alone.

## Migration

`SyntaxCircus.Blazor.Auth.Components.Layout.ReconnectModal` is removed as a breaking Auth-package change. Consumers upgrade by installing `SyntaxCircus.Blazor.Components`, importing `SyntaxCircus.Blazor.Components.Feedback`, and placing one `<ReconnectModal />` inside the host page body before `blazor.web.js`. Do not render more than one reconnect modal because Blazor targets the single `components-reconnect-modal` element.
