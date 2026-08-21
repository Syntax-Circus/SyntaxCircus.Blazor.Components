const modal = document.getElementById("components-reconnect-modal");

if (modal) {
    const setState = (state) => {
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
