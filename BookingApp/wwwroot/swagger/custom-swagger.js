// Funcție pentru așteptarea unui element să fie disponibil în DOM
function waitForElement(selector, callback) {
    const interval = setInterval(() => {
        const element = document.querySelector(selector);
        if (element) {
            clearInterval(interval);
            callback(element);
        }
    }, 500);
}

document.addEventListener("DOMContentLoaded", () => {
    console.log("[custom-swagger.js] DOM-ul este complet încărcat. Inițializarea începe...");

   // setCustomAttributesFromSchema();

    const observer = new MutationObserver(() => {
        const selects = document.querySelectorAll('select');
        if (selects.length > 0) {
            console.log("[DEBUG] Select-uri detectate în DOM:");
            let isAdminFound = false;
            let userIdFound = false;

            selects.forEach((select, index) => {
                const options = Array.from(select.options).map(option => option.value);

                if (options.includes('true') && options.includes('false')) {
                    console.log(`[DEBUG] Dropdown-ul 'isAdmin' detectat la index ${index}:`, select.outerHTML);
                    select.setAttribute('data-is-admin', 'true');
                    select.removeAttribute('disabled');
                    isAdminFound = true;
                }

                if (select.id === 'userDropdown' || options.some(opt => !isNaN(opt))) {
                    console.log(`[DEBUG] Dropdown-ul 'userId' detectat la index ${index}:`, select.outerHTML);
                    select.setAttribute('data-user-id', 'true');
                    select.removeAttribute('disabled');
                    userIdFound = true;
                }
            });

            if (isAdminFound && userIdFound) {
                console.log("[DEBUG] Dropdown-urile `isAdmin` și `userId` au fost detectate. Observatorul va fi oprit.");
                observer.disconnect();
            }
        }
    });

    observer.observe(document.body, { childList: true, subtree: true });

    console.log("[custom-swagger.js] Observatorul DOM a fost configurat.");
    replaceInputWithDropdown();
    observeExecuteButton();
});

// Funcție pentru extragerea valorii numerice din text
function extractNumericValue(value) {
    const numericMatch = value.match(/^\d+/); // Caută prima valoare numerică
    return numericMatch ? parseInt(numericMatch[0], 10) : null;
}

// Funcție pentru popularea dropdown-ului cu utilizatori
function populateDropdown(dropdown) {
    fetch('/api/Hotel/GetUsersForDropdown')
        .then(response => response.json())
        .then(users => {
            console.log("[DEBUG] Utilizatori pentru dropdown:", users);

            dropdown.innerHTML = '<option value="">Selectează un utilizator</option>';

            users.forEach(user => {
                const option = document.createElement('option');
                option.value = user.value; // ID numeric
                option.textContent = `${user.value} - ${user.label}`;
                dropdown.appendChild(option);
            });

            console.log("[DEBUG] Structură dropdown userId după populare:", dropdown.outerHTML);
        })
        .catch(error => console.error("[ERROR] Nu s-a putut popula dropdown-ul:", error));
}

// Funcție pentru monitorizarea butonului "Execute"
function observeExecuteButton() {
    waitForElement('.btn.execute', (button) => {
        console.log("[DEBUG] Buton Execute detectat:", button);

        button.addEventListener("click", (event) => {
            console.log("[DEBUG] Buton Execute apăsat.");

            const userIdSelect = document.querySelector('select[data-user-id="true"]');
            if (!userIdSelect) {
                console.error("[ERROR] Nu s-a găsit dropdown-ul pentru userId.");
                alert("Dropdown-ul pentru userId nu a fost găsit.");
                return;
            }

            const rawValue = userIdSelect.value || '';
            const userId = extractNumericValue(rawValue);

            if (!userId || isNaN(userId)) {
                console.error("[ERROR] Valoare invalidă pentru `userId`:", rawValue);
                alert("Te rugăm să selectezi un utilizator valid.");
                event.preventDefault();
                return;
            }

            console.log("[DEBUG] Valoare userId selectată:", userId);

            const isAdminSelect = document.querySelector('select[data-is-admin="true"]');
            if (!isAdminSelect) {
                console.error("[ERROR] Nu s-a găsit elementul <select> pentru isAdmin.");
                alert("Nu s-a putut detecta câmpul pentru isAdmin.");
                return;
            }

            const isAdmin = isAdminSelect.value;
            if (!isAdmin || isAdmin === "") {
                console.error("[ERROR] Nu a fost selectată nicio valoare pentru `isAdmin`.");
                alert("Te rugăm să selectezi o valoare pentru isAdmin.");
                event.preventDefault();
                return;
            }

            console.log("[DEBUG] Valoare isAdmin selectată:", isAdmin);

            // Construim URL-ul direct cu valorile găsite
            const finalUrl = `https://localhost:7286/api/Hotel/SetAdmin?isAdmin=${isAdmin}&userId=${userId}`;
            console.log("[DEBUG] URL generat:", finalUrl);

            alert(`URL generat: ${finalUrl}`);
        });
    });
}

// Funcție pentru înlocuirea input-urilor cu dropdown-uri
function replaceInputWithDropdown() {
    waitForElement('[placeholder="userId"]', (inputField) => {
        inputField.style.display = "none";

        const selectDropdown = document.createElement('select');
        selectDropdown.id = 'userDropdown';
        selectDropdown.className = inputField.className;

        const defaultOption = document.createElement('option');
        defaultOption.value = '';
        defaultOption.textContent = 'Selectează un utilizator';
        selectDropdown.appendChild(defaultOption);

        inputField.parentNode.insertBefore(selectDropdown, inputField.nextSibling);

        populateDropdown(selectDropdown);
    });
}
