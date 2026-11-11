(() => {
    const certificationList = document.getElementById("professional-certification-list");
    const contactList = document.getElementById("professional-contact-list");
    const addCertificationButton = document.getElementById("add-professional-certification");
    const addContactButton = document.getElementById("add-professional-contact");
    const certificationTemplate = document.getElementById("professional-certification-template");
    const contactTemplate = document.getElementById("professional-contact-template");

    if (!certificationList ||
        !contactList ||
        !addCertificationButton ||
        !addContactButton ||
        !(certificationTemplate instanceof HTMLTemplateElement) ||
        !(contactTemplate instanceof HTMLTemplateElement)) {
        return;
    }

    const updateIndexes = () => {
        const certificationRows = certificationList.querySelectorAll(
            "[data-professional-certification-row]");
        certificationRows.forEach((row, index) => {
            for (const input of row.querySelectorAll("[data-certification-input]")) {
                const field = input.dataset.certificationInput;
                input.name = `Certifications[${index}].${field}`;
                input.id = `Certifications_${index}__${field}`;
            }

            for (const label of row.querySelectorAll("[data-certification-label]")) {
                const field = label.dataset.certificationLabel;
                label.htmlFor = `Certifications_${index}__${field}`;
            }

            for (const validation of row.querySelectorAll(
                "[data-certification-validation]")) {
                const field = validation.dataset.certificationValidation;
                validation.setAttribute(
                    "data-valmsg-for",
                    `Certifications[${index}].${field}`);
            }
        });

        const contactRows = contactList.querySelectorAll("[data-professional-contact-row]");
        contactRows.forEach((row, index) => {
            for (const input of row.querySelectorAll("[data-contact-input]")) {
                const field = input.dataset.contactInput;
                input.name = `Contacts[${index}].${field}`;
                input.id = `Contacts_${index}__${field}`;
            }

            for (const label of row.querySelectorAll("[data-contact-label]")) {
                const field = label.dataset.contactLabel;
                label.htmlFor = `Contacts_${index}__${field}`;
            }

            for (const validation of row.querySelectorAll("[data-contact-validation]")) {
                const field = validation.dataset.contactValidation;
                validation.setAttribute("data-valmsg-for", `Contacts[${index}].${field}`);
            }
        });
    };

    updateIndexes();
    addCertificationButton.addEventListener("click", () => {
        certificationList.append(certificationTemplate.content.cloneNode(true));
        updateIndexes();
    });
    addContactButton.addEventListener("click", () => {
        contactList.append(contactTemplate.content.cloneNode(true));
        updateIndexes();
    });

    certificationList.addEventListener("click", event => {
        if (!(event.target instanceof Element)) {
            return;
        }

        const removeButton = event.target.closest("[data-remove-professional-certification]");
        removeButton?.closest("[data-professional-certification-row]")?.remove();
        updateIndexes();
    });
    contactList.addEventListener("click", event => {
        if (!(event.target instanceof Element)) {
            return;
        }

        const removeButton = event.target.closest("[data-remove-professional-contact]");
        removeButton?.closest("[data-professional-contact-row]")?.remove();
        updateIndexes();
    });
})();
