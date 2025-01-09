console.log("[custom-swagger.js] Script încărcat! Inițializare începe...");

// Funcție pentru așteptarea unui element în DOM
function waitForElement(selector, callback) {
    const interval = setInterval(() => {
        const element = document.querySelector(selector);
        if (element) {
            clearInterval(interval);
            callback(element);
        }
    }, 500);
}

// Înlocuiește input-ul cu un dropdown și păstrează input-ul ascuns
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

        observeExecuteButton(selectDropdown, document.querySelector('[placeholder="isAdmin"]'));
        populateDropdown(selectDropdown);
    });
}

// Monitorizează butonul "Execute" pentru a activa cererea
function observeExecuteButton(dropdown) {
    waitForElement('.btn.execute', (button) => {
        console.log("[DEBUG] Buton Execute detectat:", button);

        button.addEventListener("click", (event) => {
            console.log("[DEBUG] Buton Execute apăsat.");

            // Preluăm valorile selectate
            const userId = dropdown.value;
            console.log("[DEBUG] Valoare userId selectată:", userId);

            const allSelects = document.querySelectorAll('select');
            const isAdminInputField = allSelects[2]; // Index confirmat pentru isAdmin
            if (!isAdminInputField) {
                console.error("[ERROR] Nu s-a găsit input-ul pentru isAdmin.");
                return;
            }

            const isAdmin = isAdminInputField.value;
            console.log("[DEBUG] Valoare isAdmin selectată:", isAdmin);

            // Verificăm dacă valorile sunt valide
            if (!userId || !isAdmin) {
                console.error("[ERROR] Valoarea userId sau isAdmin este invalidă.");
                alert("Te rugăm să selectezi un utilizator și o valoare pentru isAdmin.");
                event.preventDefault();
                return;
            }

            // Interceptăm și rescriem cererea
            interceptAndRewriteRequest(userId, isAdmin);
        });
    });
}

function interceptAndRewriteRequest(userId, isAdmin) {
    const originalOpen = XMLHttpRequest.prototype.open;
    XMLHttpRequest.prototype.open = function (method, url) {
        console.log("[DEBUG] URL inițial al cererii:", url);

        // Rescriem URL-ul cu parametrii corecți
        const newUrl = url.replace(/userId=[^&]*/, `userId=${userId}`)
            .replace(/isAdmin=[^&]*/, `isAdmin=${isAdmin}`);
        console.log("[DEBUG] URL rescris:", newUrl);

        // Continuăm cu cererea rescrisă
        originalOpen.call(this, method, newUrl);
    };
}

// Populează dropdown-ul cu datele din API
function populateDropdown(dropdown) {
    fetch('/api/Hotel/GetUsersForDropdown')
        .then(response => {
            if (!response.ok) {
                return;
            }
            return response.json();
        })
        .then(users => {
            if (!users || users.length === 0) {
                return;
            }
            dropdown.innerHTML = '<option value="">Selectează un utilizator</option>';
            users.forEach((user) => {
                const option = document.createElement('option');
                option.value = user.value;
                option.textContent = `${user.value} - ${user.label}`;
                dropdown.appendChild(option);
            });
        })
        .catch(() => { });
}

// Asigură-te că DOM-ul Swagger este complet generat înainte de inițializare
document.addEventListener('DOMContentLoaded', () => {
    replaceInputWithDropdown();
});
