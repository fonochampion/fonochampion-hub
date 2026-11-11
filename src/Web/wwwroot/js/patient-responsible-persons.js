(() => {
    const list = document.getElementById("responsible-person-list");
    const addButton = document.getElementById("add-responsible-person");
    const template = document.getElementById("responsible-person-template");

    if (!list || !addButton || !(template instanceof HTMLTemplateElement)) {
        return;
    }

    const updateIndexes = () => {
        const rows = list.querySelectorAll("[data-responsible-person-row]");
        rows.forEach((row, index) => {
            for (const input of row.querySelectorAll("[data-responsible-person-input]")) {
                const field = input.dataset.responsiblePersonInput;
                input.name = `ResponsiblePersons[${index}].${field}`;
                input.id = `ResponsiblePersons_${index}__${field}`;
            }

            for (const label of row.querySelectorAll("[data-responsible-person-label]")) {
                const field = label.dataset.responsiblePersonLabel;
                label.htmlFor = `ResponsiblePersons_${index}__${field}`;
            }

            for (const validation of row.querySelectorAll("[data-responsible-person-validation]")) {
                const field = validation.dataset.responsiblePersonValidation;
                validation.setAttribute(
                    "data-valmsg-for",
                    `ResponsiblePersons[${index}].${field}`);
            }
        });
        list.dataset.nextIndex = String(rows.length);
    };

    updateIndexes();
    addButton.addEventListener("click", () => {
        const row = template.content.cloneNode(true);
        for (const element of row.querySelectorAll("*")) {
            for (const attribute of ["name", "id", "for", "data-valmsg-for"]) {
                if (element.hasAttribute(attribute)) {
                    element.setAttribute(
                        attribute,
                        element.getAttribute(attribute).replaceAll("__index__", String(nextIndex)));
                }
            }
        }

        list.append(row);
        updateIndexes();
    });

    list.addEventListener("click", event => {
        if (!(event.target instanceof Element)) {
            return;
        }

        const removeButton = event.target.closest("[data-remove-responsible-person]");
        removeButton?.closest("[data-responsible-person-row]")?.remove();
        updateIndexes();
    });
})();
