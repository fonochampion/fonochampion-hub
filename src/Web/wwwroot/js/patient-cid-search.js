(() => {
    const searchInput = document.getElementById("cid-search");
    const codeInput = document.getElementById("Cid10Code");
    const descriptionInput = document.getElementById("Cid10Description");
    const status = document.getElementById("cid-search-status");
    const results = document.getElementById("cid-search-results");
    const selected = document.getElementById("cid-selected");

    if (!searchInput || !codeInput || !descriptionInput || !status || !results || !selected) {
        return;
    }

    let debounceTimer;
    let activeRequest;

    searchInput.addEventListener("input", () => {
        codeInput.value = "";
        descriptionInput.value = "";
        selected.textContent = "";
        results.replaceChildren();
        window.clearTimeout(debounceTimer);
        activeRequest?.abort();

        const query = searchInput.value.trim();
        if (query.length < 2) {
            status.textContent = "Digite pelo menos 2 caracteres para buscar.";
            return;
        }

        status.textContent = "Buscando CID-10...";
        debounceTimer = window.setTimeout(async () => {
            const request = new AbortController();
            activeRequest = request;
            try {
                const url = new URL(searchInput.dataset.searchUrl, window.location.origin);
                url.searchParams.set("query", query);
                const response = await fetch(url, {
                    headers: { Accept: "application/json" },
                    signal: request.signal
                });
                const payload = await response.json();
                if (request.signal.aborted) {
                    return;
                }

                if (!response.ok) {
                    throw new Error(payload.detail || "Não foi possível buscar o catálogo CID-10.");
                }

                for (const item of payload.items) {
                    const button = document.createElement("button");
                    button.type = "button";
                    button.className = "list-group-item list-group-item-action text-start";
                    const hierarchy = item.hierarchy ? ` (${item.hierarchy})` : "";
                    button.textContent = `${item.code} — ${item.description}${hierarchy}`;
                    button.addEventListener("click", () => {
                        codeInput.value = item.code;
                        descriptionInput.value = item.description;
                        selected.textContent = `CID selecionado: ${item.code} — ${item.description}`;
                        results.replaceChildren();
                        status.textContent = "CID selecionado.";
                    });
                    results.append(button);
                }

                if (payload.items.length === 0) {
                    status.textContent = "Nenhum CID-10 encontrado para essa busca.";
                } else if (payload.hasMore) {
                    status.textContent = "Mostrando os primeiros 25 resultados. Refine a busca para ver outros.";
                } else {
                    status.textContent = `${payload.items.length} resultado(s).`;
                }
            } catch (error) {
                if (request.signal.aborted) {
                    return;
                }

                status.textContent = error.message || "Não foi possível buscar o catálogo CID-10.";
            }
        }, 250);
    });
})();
