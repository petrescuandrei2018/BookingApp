console.log("[custom-swagger.js] Script încărcat! Inițializare începe...");

// Funcție pentru așteptarea unui element în DOM
function waitForElement(selector, callback) {
    console.log(`[waitForElement] Începem să căutăm elementul cu selectorul: ${selector}`);
    const interval = setInterval(() => {
        const element = document.querySelector(selector);
        if (element) {
            console.log(`[waitForElement] Element găsit: ${selector}`, element);
            clearInterval(interval);
            callback(element);
        } else {
            console.log(`[waitForElement] Elementul NU a fost găsit încă: ${selector}`);
        }
    }, 500);
}

// Înlocuiește input-ul cu un dropdown și păstrează input-ul ascuns
function replaceInputWithDropdown() {
    console.log("[replaceInputWithDropdown] Pornim procesul de înlocuire a input-ului pentru userId...");
    waitForElement('[placeholder="userId"]', (inputField) => {
        console.log("[replaceInputWithDropdown] Input găsit pentru userId:", inputField);

        // Ascunde input-ul original
        inputField.style.display = "none";
        console.log("[replaceInputWithDropdown] Input ascuns pentru compatibilitate.");

        // Creare dropdown
        const selectDropdown = document.createElement('select');
        selectDropdown.id = 'userDropdown';
        selectDropdown.className = inputField.className;

        // Adăugare opțiune implicită
        const defaultOption = document.createElement('option');
        defaultOption.value = '';
        defaultOption.textContent = 'Selectează un utilizator';
        selectDropdown.appendChild(defaultOption);

        console.log("[replaceInputWithDropdown] Dropdown creat, opțiunea implicită adăugată:", defaultOption);

        // Adaugă dropdown-ul după input-ul original
        inputField.parentNode.insertBefore(selectDropdown, inputField.nextSibling);

        console.log("[replaceInputWithDropdown] Dropdown inserat în DOM:", selectDropdown);

        observeTryOutButton(selectDropdown, inputField);
        populateDropdown(selectDropdown);
    });
}

// Monitorizează butonul "Try out" pentru a activa dropdown-ul
function observeTryOutButton(dropdown, inputField) {
    console.log("[observeTryOutButton] Începem să monitorizăm butoanele Try out...");
    waitForElement('.btn.try-out__btn', (button) => {
        console.log("[observeTryOutButton] Buton Try out detectat:", button);
        button.addEventListener('click', () => {
            console.log("[observeTryOutButton] Buton Try out apăsat.");
            console.log(`[observeTryOutButton] Dropdown value: ${dropdown.value}`);
            console.log(`[observeTryOutButton] Input field value înainte de setare: ${inputField.value}`);
            inputField.value = dropdown.value;
            console.log(`[observeTryOutButton] Input field value după setare: ${inputField.value}`);
        });
    });

    addDropdownEventListener(dropdown, inputField);
}

// Populează dropdown-ul cu datele din API
function populateDropdown(dropdown) {
    console.log("[populateDropdown] Începem să populăm dropdown-ul...");
    fetch('/api/Hotel/GetUsersForDropdown')
        .then(response => {
            console.log(`[populateDropdown] Răspuns primit de la API: Status ${response.status}`);
            return response.json();
        })
        .then(users => {
            console.log(`[populateDropdown] Date obținute de la server: ${JSON.stringify(users)}`);
            dropdown.innerHTML = '<option value="">Selectează un utilizator</option>'; // Reset dropdown
            users.forEach((user, index) => {
                console.log(`[populateDropdown] Procesăm utilizatorul #${index}: ${JSON.stringify(user)}`);
                const option = document.createElement('option');
                option.value = user.value;
                option.textContent = `${user.value} - ${user.label}`;
                dropdown.appendChild(option);
                console.log(`[populateDropdown] Opțiune adăugată: Value=${option.value}, Label=${option.textContent}`);
            });
        })
        .catch(error => console.error("[populateDropdown] Eroare la popularea dropdown-ului:", error));
}

// Adaugă evenimentul de schimbare pentru dropdown
function addDropdownEventListener(dropdown, inputField) {
    console.log("[addDropdownEventListener] Adăugăm evenimentul de schimbare pentru dropdown...");
    dropdown.addEventListener('change', (event) => {
        const selectedValue = event.target.value;
        console.log("[addDropdownEventListener] Valoare selectată din dropdown:", selectedValue);

        if (inputField) {
            inputField.value = selectedValue;
            console.log("[addDropdownEventListener] Valoarea setată în input-ul userId:", inputField.value);
        } else {
            console.error("[addDropdownEventListener] Input-ul userId nu a fost găsit.");
        }
    });
}

// Asigură-te că DOM-ul Swagger este complet generat înainte de inițializare
document.addEventListener('DOMContentLoaded', () => {
    console.log("[custom-swagger.js] DOMContentLoaded detectat. Pornim inițializarea...");
    replaceInputWithDropdown();
});
