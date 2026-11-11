(() => {
    const list = document.getElementById("responsible-person-list");
    const addButton = document.getElementById("add-responsible-person");
    const personTemplate = document.getElementById("responsible-person-template");
    const contactTemplate = document.getElementById("responsible-person-contact-template");

    if (!list ||
        !addButton ||
        !(personTemplate instanceof HTMLTemplateElement) ||
        !(contactTemplate instanceof HTMLTemplateElement)) {
        return;
    }

    const updateIndexes = () => {
        const personRows = list.querySelectorAll("[data-responsible-person-row]");
        personRows.forEach((personRow, personIndex) => {
            for (const input of personRow.querySelectorAll("[data-responsible-person-input]")) {
                const field = input.dataset.responsiblePersonInput;
                input.name = `ResponsiblePersons[${personIndex}].${field}`;
                input.id = `ResponsiblePersons_${personIndex}__${field}`;
            }

            for (const label of personRow.querySelectorAll("[data-responsible-person-label]")) {
                const field = label.dataset.responsiblePersonLabel;
                label.htmlFor = `ResponsiblePersons_${personIndex}__${field}`;
            }

            for (const validation of personRow.querySelectorAll("[data-responsible-person-validation]")) {
                const field = validation.dataset.responsiblePersonValidation;
                validation.setAttribute(
                    "data-valmsg-for",
                    `ResponsiblePersons[${personIndex}].${field}`);
            }

            const contactRows = personRow.querySelectorAll(
                "[data-responsible-person-contact-row]");
            contactRows.forEach((contactRow, contactIndex) => {
                for (const input of contactRow.querySelectorAll("[data-contact-input]")) {
                    const field = input.dataset.contactInput;
                    input.name =
                        `ResponsiblePersons[${personIndex}].Contacts[${contactIndex}].${field}`;
                    input.id =
                        `ResponsiblePersons_${personIndex}__Contacts_${contactIndex}__${field}`;
                }

                for (const label of contactRow.querySelectorAll("[data-contact-label]")) {
                    const field = label.dataset.contactLabel;
                    label.htmlFor =
                        `ResponsiblePersons_${personIndex}__Contacts_${contactIndex}__${field}`;
                }

                for (const validation of contactRow.querySelectorAll("[data-contact-validation]")) {
                    const field = validation.dataset.contactValidation;
                    validation.setAttribute(
                        "data-valmsg-for",
                        `ResponsiblePersons[${personIndex}].Contacts[${contactIndex}].${field}`);
                }
            });
        });
    };

    updateIndexes();
    addButton.addEventListener("click", () => {
        list.append(personTemplate.content.cloneNode(true));
        updateIndexes();
    });

    list.addEventListener("click", event => {
        if (!(event.target instanceof Element)) {
            return;
        }

        const addContactButton = event.target.closest("[data-add-responsible-person-contact]");
        if (addContactButton) {
            const personRow = addContactButton.closest("[data-responsible-person-row]");
            const contactList = personRow?.querySelector(
                "[data-responsible-person-contact-list]");
            if (contactList) {
                contactList.append(contactTemplate.content.cloneNode(true));
            }
            updateIndexes();
            return;
        }

        const removeContactButton = event.target.closest(
            "[data-remove-responsible-person-contact]");
        if (removeContactButton) {
            removeContactButton.closest("[data-responsible-person-contact-row]")?.remove();
            updateIndexes();
            return;
        }

        const removePersonButton = event.target.closest("[data-remove-responsible-person]");
        removePersonButton?.closest("[data-responsible-person-row]")?.remove();
        updateIndexes();
    });
})();
