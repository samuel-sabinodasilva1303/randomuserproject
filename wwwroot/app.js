const API_BASE_URL = '/api/users';

let currentPage = 1;
let pageSize = 10;
let searchTerm = '';
let selectedUsers = new Set();

document.addEventListener('DOMContentLoaded', function() {
    loadUsers();
    loadStats();

    document.getElementById('searchInput').addEventListener('input', debounce(function(e) {
        searchTerm = e.target.value;
        currentPage = 1;
        loadUsers();
    }, 500));

    document.getElementById('pageSize').addEventListener('change', function(e) {
        pageSize = parseInt(e.target.value);
        currentPage = 1;
        loadUsers();
    });
});

function showToast(type, title, message, duration = 5000) {
    const toastContainer = document.getElementById('toastContainer');
    const toastId = 'toast-' + Date.now();

    const iconMap = {
        success: 'fa-check-circle',
        error: 'fa-exclamation-circle',
        warning: 'fa-exclamation-triangle',
        info: 'fa-info-circle'
    };

    const toast = document.createElement('div');
    toast.id = toastId;
    toast.className = `toast-custom ${type}`;
    toast.innerHTML = `
        <div class="toast-icon">
            <i class="fas ${iconMap[type]}"></i>
        </div>
        <div class="toast-content">
            <div class="toast-title">${title}</div>
            <div class="toast-message">${message}</div>
        </div>
        <button class="toast-close" onclick="removeToast('${toastId}')">
            <i class="fas fa-times"></i>
        </button>
    `;

    toastContainer.appendChild(toast);

    setTimeout(() => removeToast(toastId), duration);
}

function removeToast(toastId) {
    const toast = document.getElementById(toastId);
    if (toast) {
        toast.style.animation = 'slideOutRight 0.3s ease';
        setTimeout(() => toast.remove(), 300);
    }
}

function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

async function loadStats() {
    try {
        const response = await fetch(`${API_BASE_URL}/stats`);
        if (response.ok) {
            const stats = await response.json();
            document.getElementById('totalUsersDisplay').textContent = stats.totalUsers;
        }
    } catch (error) {
        console.error('Erro ao carregar estatísticas:', error);
    }
}

async function loadUsers() {
    showLoading(true);

    try {
        let url = `${API_BASE_URL}?page=${currentPage}&pageSize=${pageSize}`;
        if (searchTerm) {
            url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
        }

        const response = await fetch(url);

        if (!response.ok) {
            throw new Error('Erro ao carregar usuários');
        }

        const result = await response.json();

        if (Array.isArray(result)) {
            displayUsers(result);
            updateUserCount(result.length);
            document.getElementById('pagination').innerHTML = '';
        } else if (result && result.data && Array.isArray(result.data)) {
            displayUsers(result.data);
            updatePagination(result);
            updateUserCount(result.totalCount);
        } else {
            console.error('Formato de resposta inválido:', result);
            throw new Error('Formato de resposta inválido');
        }

        loadStats();
    } catch (error) {
        console.error('Erro ao carregar usuários:', error);
        showToast('error', 'Erro ao Carregar', error.message);
    } finally {
        showLoading(false);
    }
}

function showAddUsersModal() {
    const modal = new bootstrap.Modal(document.getElementById('addUsersModal'));
    document.getElementById('addUserCount').value = 10;
    modal.show();
}

async function addUsers() {
    const count = parseInt(document.getElementById('addUserCount').value);

    if (!count || count < 1 || count > 100) {
        showToast('warning', 'Valor Inválido', 'Por favor, insira um número entre 1 e 100');
        return;
    }

    const addModal = bootstrap.Modal.getInstance(document.getElementById('addUsersModal'));
    addModal.hide();

    const progressModal = new bootstrap.Modal(document.getElementById('progressModal'));
    progressModal.show();

    // Atualizar título
    document.getElementById('progressTitle').textContent = `Adicionando ${count} usuários...`;
    document.getElementById('progressText').textContent = 'Conectando à API...';
    updateProgressBar(10);

    try {
        await new Promise(resolve => setTimeout(resolve, 300));
        updateProgressBar(20);
        document.getElementById('progressText').textContent = 'Gerando usuários aleatórios...';

        const response = await fetch(`${API_BASE_URL}/add?count=${count}`, {
            method: 'POST'
        });

        if (!response.ok) {
            throw new Error('Erro ao adicionar usuários');
        }

        updateProgressBar(80);
        document.getElementById('progressText').textContent = 'Salvando no banco de dados...';

        const result = await response.json();

        updateProgressBar(100);
        document.getElementById('progressText').textContent = 'Concluído!';
        await new Promise(resolve => setTimeout(resolve, 500));

        progressModal.hide();

        showToast('success', 'Sucesso!', `${result.added} usuários adicionados! Total: ${result.currentTotal}`, 6000);

        currentPage = 1;
        loadUsers();

    } catch (error) {
        console.error('Erro:', error);
        progressModal.hide();
        showToast('error', 'Erro ao Adicionar', error.message);
    }
}

function updateProgressBar(percentage) {
    const progressBar = document.getElementById('progressBar');
    progressBar.style.width = percentage + '%';
    progressBar.textContent = percentage + '%';
}

function displayUsers(users) {
    const tbody = document.getElementById('usersTable');

    if (!users || !Array.isArray(users) || users.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="8" class="text-center text-muted py-5">
                    <i class="fas fa-inbox fa-3x mb-3 d-block"></i>
                    <p class="mb-3">Nenhum usuário encontrado</p>
                    <button class="btn btn-gradient-primary" onclick="showAddUsersModal()">
                        <i class="fas fa-user-plus me-2"></i>Adicionar Primeiros Usuários
                    </button>
                </td>
            </tr>
        `;
        clearSelection();
        return;
    }

    tbody.innerHTML = users.map(user => `
        <tr id="user-row-${user.id}" class="${selectedUsers.has(user.id) ? 'table-active' : ''}">
            <td>
                <input type="checkbox" class="form-check-input user-checkbox"
                       data-user-id="${user.id}"
                       ${selectedUsers.has(user.id) ? 'checked' : ''}
                       onchange="toggleUserSelection(${user.id})">
            </td>
            <td>
                <img src="${user.pictureUrl || 'https://via.placeholder.com/50'}"
                     alt="${user.firstName}"
                     class="user-avatar"
                     onerror="this.src='https://via.placeholder.com/50'">
            </td>
            <td><strong>${user.firstName} ${user.lastName}</strong></td>
            <td><small>${user.email}</small></td>
            <td><small>${user.phone}</small></td>
            <td>${user.city}</td>
            <td>${user.country}</td>
            <td class="text-center action-buttons">
                <button class="btn btn-sm btn-outline-primary me-1"
                        onclick="editUser(${user.id})"
                        title="Editar">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger"
                        onclick="deleteUser(${user.id})"
                        title="Excluir">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');

    updateBulkActionsBar();
}

function updatePagination(result) {
    const paginationContainer = document.getElementById('pagination');

    if (result.totalPages <= 1) {
        paginationContainer.innerHTML = '';
        return;
    }

    let paginationHTML = '<nav><ul class="pagination justify-content-center mb-0">';

    paginationHTML += `
        <li class="page-item ${!result.hasPrevious ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="changePage(${currentPage - 1}); return false;">
                <i class="fas fa-chevron-left"></i>
            </a>
        </li>
    `;

    const startPage = Math.max(1, currentPage - 2);
    const endPage = Math.min(result.totalPages, currentPage + 2);

    if (startPage > 1) {
        paginationHTML += `
            <li class="page-item">
                <a class="page-link" href="#" onclick="changePage(1); return false;">1</a>
            </li>
        `;
        if (startPage > 2) {
            paginationHTML += '<li class="page-item disabled"><span class="page-link">...</span></li>';
        }
    }

    for (let i = startPage; i <= endPage; i++) {
        paginationHTML += `
            <li class="page-item ${i === currentPage ? 'active' : ''}">
                <a class="page-link" href="#" onclick="changePage(${i}); return false;">${i}</a>
            </li>
        `;
    }

    if (endPage < result.totalPages) {
        if (endPage < result.totalPages - 1) {
            paginationHTML += '<li class="page-item disabled"><span class="page-link">...</span></li>';
        }
        paginationHTML += `
            <li class="page-item">
                <a class="page-link" href="#" onclick="changePage(${result.totalPages}); return false;">${result.totalPages}</a>
            </li>
        `;
    }

    paginationHTML += `
        <li class="page-item ${!result.hasNext ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="changePage(${currentPage + 1}); return false;">
                <i class="fas fa-chevron-right"></i>
            </a>
        </li>
    `;

    paginationHTML += '</ul></nav>';
    paginationContainer.innerHTML = paginationHTML;
}

function changePage(page) {
    currentPage = page;
    loadUsers();
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

async function editUser(userId) {
    try {
        const response = await fetch(`${API_BASE_URL}/${userId}`);
        if (!response.ok) {
            throw new Error('Erro ao buscar usuário');
        }

        const user = await response.json();

        document.getElementById('editUserId').value = user.id;
        document.getElementById('editFirstName').value = user.firstName;
        document.getElementById('editLastName').value = user.lastName;
        document.getElementById('editEmail').value = user.email;
        document.getElementById('editPhone').value = user.phone;
        document.getElementById('editStreet').value = user.street;
        document.getElementById('editCity').value = user.city;
        document.getElementById('editState').value = user.state;
        document.getElementById('editPostalCode').value = user.postalCode;
        document.getElementById('editCountry').value = user.country;
        document.getElementById('editDateOfBirth').value = user.dateOfBirth.split('T')[0];
        document.getElementById('editGender').value = user.gender;
        document.getElementById('editPictureUrl').value = user.pictureUrl;

        const modal = new bootstrap.Modal(document.getElementById('editUserModal'));
        modal.show();
    } catch (error) {
        console.error('Erro:', error);
        showToast('error', 'Erro ao Editar', error.message);
    }
}

async function saveUser() {
    const userId = document.getElementById('editUserId').value;
    const user = {
        firstName: document.getElementById('editFirstName').value,
        lastName: document.getElementById('editLastName').value,
        email: document.getElementById('editEmail').value,
        phone: document.getElementById('editPhone').value,
        street: document.getElementById('editStreet').value,
        city: document.getElementById('editCity').value,
        state: document.getElementById('editState').value,
        postalCode: document.getElementById('editPostalCode').value,
        country: document.getElementById('editCountry').value,
        dateOfBirth: document.getElementById('editDateOfBirth').value,
        gender: document.getElementById('editGender').value,
        pictureUrl: document.getElementById('editPictureUrl').value
    };

    try {
        const response = await fetch(`${API_BASE_URL}/${userId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(user)
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Erro ao salvar usuário');
        }

        const modal = bootstrap.Modal.getInstance(document.getElementById('editUserModal'));
        modal.hide();

        showToast('success', 'Sucesso!', 'Usuário atualizado com sucesso!');
        loadUsers();
    } catch (error) {
        console.error('Erro:', error);
        showToast('error', 'Erro ao Salvar', error.message);
    }
}

async function deleteUser(userId) {
    const modal = new bootstrap.Modal(document.getElementById('confirmDeleteModal'));
    document.getElementById('confirmDeleteMessage').textContent =
        'Tem certeza que deseja excluir este usuário? Esta ação não pode ser desfeita.';

    modal._element.dataset.deleteType = 'single';
    modal._element.dataset.userId = userId;

    modal.show();
}

async function deleteSelected() {
    if (selectedUsers.size === 0) {
        showToast('warning', 'Atenção', 'Nenhum usuário selecionado');
        return;
    }

    confirmDeleteSelected();
}

function confirmDeleteSelected() {
    const count = selectedUsers.size;
    const modal = new bootstrap.Modal(document.getElementById('confirmDeleteModal'));
    document.getElementById('confirmDeleteMessage').textContent =
        `Tem certeza que deseja excluir ${count} usuário(s) selecionado(s)? Esta ação não pode ser desfeita.`;

    modal._element.dataset.deleteType = 'multiple';

    modal.show();
}

async function executeDelete() {
    const modal = bootstrap.Modal.getInstance(document.getElementById('confirmDeleteModal'));
    const deleteType = modal._element.dataset.deleteType;

    modal.hide();

    if (deleteType === 'single') {
        const userId = modal._element.dataset.userId;
        await executeSingleDelete(userId);
    } else if (deleteType === 'multiple') {
        await executeMultipleDelete();
    }
}

async function executeSingleDelete(userId) {
    try {
        const response = await fetch(`${API_BASE_URL}/${userId}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error('Erro ao excluir usuário');
        }

        showToast('success', 'Sucesso!', 'Usuário excluído com sucesso!');

        const tbody = document.getElementById('usersTable');
        if (tbody.getElementsByTagName('tr').length === 1 && currentPage > 1) {
            currentPage--;
        }

        loadUsers();
    } catch (error) {
        console.error('Erro:', error);
        showToast('error', 'Erro ao Excluir', error.message);
    }
}

async function executeMultipleDelete() {
    const count = selectedUsers.size;
    const progressModal = new bootstrap.Modal(document.getElementById('progressModal'));
    
    progressModal.show();
    document.getElementById('progressTitle').textContent = `Excluindo ${count} usuários...`;
    document.getElementById('progressText').textContent = 'Processando...';
    updateProgressBar(20);

    try {
        const idsToDelete = Array.from(selectedUsers);

        updateProgressBar(50);
        document.getElementById('progressText').textContent = 'Excluindo do banco de dados...';

        const response = await fetch(`${API_BASE_URL}/delete-multiple`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(idsToDelete)
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Erro ao excluir usuários');
        }

        updateProgressBar(90);
        const result = await response.json();

        updateProgressBar(100);
        document.getElementById('progressText').textContent = 'Concluído!';

        await new Promise(resolve => setTimeout(resolve, 500));
        progressModal.hide();

        clearSelection();
        showToast('success', 'Sucesso!', result.message, 6000);
        loadUsers();

    } catch (error) {
        console.error('Erro:', error);
        const progressModal = bootstrap.Modal.getInstance(document.getElementById('progressModal'));
        if (progressModal) progressModal.hide();
        showToast('error', 'Erro ao Excluir', error.message);
    }
}

function clearSearch() {
    document.getElementById('searchInput').value = '';
    searchTerm = '';
    currentPage = 1;
    loadUsers();
}

function updateUserCount(count) {
    // A contagem já é atualizada via loadStats() que atualiza totalUsersDisplay
}

function showLoading(show) {
    const loadingEl = document.getElementById('loadingOverlay');
    if (loadingEl) {
        loadingEl.style.display = show ? 'flex' : 'none';
    }
}

function toggleUserSelection(userId) {
    if (selectedUsers.has(userId)) {
        selectedUsers.delete(userId);
    } else {
        selectedUsers.add(userId);
    }

    const row = document.getElementById(`user-row-${userId}`);
    if (row) {
        row.classList.toggle('table-active', selectedUsers.has(userId));
    }

    updateBulkActionsBar();
    updateSelectAllCheckbox();
}

function toggleSelectAll() {
    const selectAllCheckbox = document.getElementById('selectAll');
    const checkboxes = document.querySelectorAll('.user-checkbox');

    checkboxes.forEach(checkbox => {
        const userId = parseInt(checkbox.dataset.userId);
        if (selectAllCheckbox.checked) {
            selectedUsers.add(userId);
            checkbox.checked = true;
            const row = document.getElementById(`user-row-${userId}`);
            if (row) row.classList.add('table-active');
        } else {
            selectedUsers.delete(userId);
            checkbox.checked = false;
            const row = document.getElementById(`user-row-${userId}`);
            if (row) row.classList.remove('table-active');
        }
    });

    updateBulkActionsBar();
}

function updateSelectAllCheckbox() {
    const selectAllCheckbox = document.getElementById('selectAll');
    const checkboxes = document.querySelectorAll('.user-checkbox');
    const checkedCount = document.querySelectorAll('.user-checkbox:checked').length;

    if (checkboxes.length === 0) {
        selectAllCheckbox.checked = false;
        selectAllCheckbox.indeterminate = false;
    } else if (checkedCount === checkboxes.length) {
        selectAllCheckbox.checked = true;
        selectAllCheckbox.indeterminate = false;
    } else if (checkedCount > 0) {
        selectAllCheckbox.checked = false;
        selectAllCheckbox.indeterminate = true;
    } else {
        selectAllCheckbox.checked = false;
        selectAllCheckbox.indeterminate = false;
    }
}

function updateBulkActionsBar() {
    const bulkActionsBar = document.getElementById('bulkActionsBar');
    const selectedCountEl = document.getElementById('selectedCount');

    if (selectedUsers.size > 0) {
        bulkActionsBar.style.display = 'block';
        selectedCountEl.textContent = selectedUsers.size;
    } else {
        bulkActionsBar.style.display = 'none';
    }
}

function clearSelection() {
    selectedUsers.clear();
    document.querySelectorAll('.user-checkbox').forEach(cb => cb.checked = false);
    document.querySelectorAll('tr[id^="user-row-"]').forEach(row => row.classList.remove('table-active'));
    updateBulkActionsBar();
    updateSelectAllCheckbox();
}
