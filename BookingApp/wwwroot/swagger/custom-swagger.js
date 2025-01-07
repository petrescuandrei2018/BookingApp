function waitForElement(selector, callback) {
    console.log(`Se așteaptă elementul cu selectorul: ${selector}`);
    const element = document.querySelector(selector);
    if (element) {
        console.log(`Element găsit: ${selector}`);
        callback(element);
    } else {
        console.log(`Elementul nu a fost găsit încă: ${selector}`);
        setTimeout(() => waitForElement(selector, callback), 500);
    }
}

function replaceInputWithDropdown() {
    console.log("Se încearcă înlocuirea input-ului cu un dropdown");

    // Selector pentru <input> cu placeholder "userId"
    waitForElement('[placeholder="userId"]', (inputField) => {
        console.log("Câmpul input a fost găsit:", inputField);

        // Creare dropdown
        const selectDropdown = document.createElement('select');
        selectDropdown.id = 'userDropdown';
        selectDropdown.className = inputField.className;
        selectDropdown.disabled = true; // Dropdown dezactivat inițial

        // Adăugare opțiune implicită
        const defaultOption = document.createElement('option');
        defaultOption.value = '';
        defaultOption.textContent = 'Selectează un utilizator';
        selectDropdown.appendChild(defaultOption);

        // Înlocuire <input> cu <select>
        inputField.replaceWith(selectDropdown);
        console.log("Câmpul input a fost înlocuit cu dropdown:", selectDropdown);

        // Monitorizăm apăsarea pe butonul "Try out"
        observeTryOutButton(selectDropdown);
    });
}

function observeTryOutButton(dropdown) {
    console.log("Se monitorizează butoanele Try out...");
    const tryOutButtons = document.querySelectorAll('.try-out__btn');
    if (tryOutButtons.length === 0) {
        console.log("Nu s-au găsit butoane Try out.");
    }
    tryOutButtons.forEach((button) => {
        console.log("Buton Try out găsit:", button);
        button.addEventListener('click', () => {
            console.log("S-a apăsat pe butonul Try out, se activează dropdown-ul...");
            dropdown.disabled = false; // Activăm dropdown-ul
            populateDropdown(dropdown); // Populăm dropdown-ul
        });
    });
}

function populateDropdown(dropdown) {
    console.log("Se obțin utilizatorii pentru dropdown...");
    fetch("https://localhost:7286/api/Hotel/GetUsersForDropdown")
        .then((response) => {
            if (!response.ok) {
                console.error(`Eroare HTTP! Status: ${response.status}`);
                throw new Error(`Eroare HTTP! Status: ${response.status}`);
            }
            return response.json();
        })
        .then((data) => {
            console.log("Utilizatori obținuți:", data);

            // Curățăm dropdown-ul înainte de populare
            dropdown.innerHTML = '';
            const defaultOption = document.createElement('option');
            defaultOption.value = '';
            defaultOption.textContent = 'Selectează un utilizator';
            dropdown.appendChild(defaultOption);

            data.forEach((user, index) => {
                console.log(`Utilizator #${index}:`, user);

                // Normalizează proprietățile pentru a trata litera mare sau mică
                const value = user.Value || user.value;
                const label = user.Label || user.label;

                if (!value || !label) {
                    console.error(`Obiectul utilizatorului nu are proprietățile necesare:`, user);
                    return;
                }

                const option = document.createElement('option');
                option.value = value;
                option.textContent = label;
                dropdown.appendChild(option);
            });

            console.log("Dropdown populat cu succes");
        })
        .catch((error) => console.error("Eroare la obținerea utilizatorilor:", error));
}

// Asigură-te că DOM-ul Swagger este complet generat înainte de apelare
document.addEventListener('DOMContentLoaded', () => {
    console.log("custom-swagger.js: Evenimentul DOMContentLoaded a fost declanșat!");
    replaceInputWithDropdown();
});
