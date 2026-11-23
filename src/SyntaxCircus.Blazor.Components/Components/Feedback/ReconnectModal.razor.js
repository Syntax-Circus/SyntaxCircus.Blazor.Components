const modal = document.getElementById("components-reconnect-modal");

if (modal) {
    // Default English text for DeferMessages mode. Mirrors the server-rendered defaults.
    const defaults = {
        "first": (el) => { el.textContent = "Rejoining the server..."; },
        "retrying": (el) => {
            el.append("Rejoin failed. Trying again in ");
            const seconds = document.createElement("span");
            seconds.id = "components-seconds-to-next-attempt";
            el.append(seconds, " seconds.");
        },
        "failed": (el) => { el.textContent = "Failed to rejoin. Please retry or reload the page."; },
        "paused": (el) => { el.textContent = "The session has been paused by the server."; },
        "resume-failed": (el) => { el.textContent = "Failed to resume the session. Please retry or reload the page."; },
        "retry-label": (el) => { el.textContent = "Retry"; },
        "resume-label": (el) => { el.textContent = "Resume"; },
    };

    // Idempotent: only fills elements that are still empty and not yet filled.
    const injectDefaults = () => {
        modal.querySelectorAll("[data-reconnect-default]").forEach((element) => {
            if (element.dataset.reconnectFilled || element.childNodes.length > 0) {
                return;
            }
            const fill = defaults[element.dataset.reconnectDefault];
            if (fill) {
                fill(element);
                element.dataset.reconnectFilled = "true";
            }
        });
    };

    const setState = (state) => {
        injectDefaults();
        modal.querySelectorAll("[data-reconnect-state]").forEach((element) => {
            element.hidden = !element.dataset.reconnectState.split(" ").includes(state);
        });
    };

    const retryWhenVisible = async () => {
        if (document.visibilityState === "visible") {
            await retry();
        }
    };

    const retry = async () => {
        document.removeEventListener("visibilitychange", retryWhenVisible);

        try {
            const reconnected = await Blazor.reconnect();
            if (!reconnected) {
                const resumed = await Blazor.resumeCircuit();
                if (!resumed) {
                    location.reload();
                } else {
                    modal.close();
                }
            }
        } catch {
            document.addEventListener("visibilitychange", retryWhenVisible);
        }
    };

    const resume = async () => {
        try {
            if (!await Blazor.resumeCircuit()) {
                location.reload();
            }
        } catch {
            setState("resume-failed");
        }
    };

    modal.addEventListener("components-reconnect-state-changed", (event) => {
        if (event.detail.state === "show") {
            setState("first");
            modal.showModal();
        } else if (event.detail.state === "hide") {
            modal.close();
        } else if (event.detail.state === "retrying") {
            setState("retrying");
        } else if (event.detail.state === "failed") {
            setState("failed");
            document.addEventListener("visibilitychange", retryWhenVisible);
        } else if (event.detail.state === "paused") {
            setState("paused");
        } else if (event.detail.state === "rejected") {
            location.reload();
        }
    });

    modal.addEventListener("click", (event) => {
        const action = event.target.closest("[data-reconnect-action]")?.dataset.reconnectAction;
        if (action === "retry") {
            void retry();
        } else if (action === "resume") {
            void resume();
        }
    });
}
