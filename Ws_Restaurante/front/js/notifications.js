// ============================================================
// SISTEMA DE NOTIFICACIONES MODALES
// Reemplaza los alert() y confirm() nativos con ventanas emergentes elegantes
// ============================================================

/**
 * Muestra una notificación modal personalizada con callback
 * @param {string} message - Mensaje a mostrar
 * @param {string} type - Tipo de notificación: 'success', 'error', 'warning', 'info'
 * @param {string} title - Título opcional (se genera automáticamente si no se proporciona)
 * @param {function} onClose - Callback que se ejecuta al cerrar el modal
 */
function showNotification(message, type = 'info', title = null, onClose = null) {
    const overlay = document.getElementById('notification-modal-overlay');
 const icon = document.getElementById('notification-icon');
    const titleElement = document.getElementById('notification-title');
    const messageElement = document.getElementById('notification-message');
    const btnOk = document.getElementById('notification-btn-ok');
    const modal = overlay.querySelector('.notification-modal');

    // Limpiar clases anteriores
    modal.className = 'notification-modal';
    icon.className = 'notification-icon';

    // Configurar según el tipo
    let iconClass = '';
    let defaultTitle = '';
    let modalClass = '';

    switch(type) {
  case 'success':
            iconClass = 'bi bi-check-circle-fill';
            defaultTitle = '¡Éxito!';
            modalClass = 'notification-success';
            break;
        case 'error':
            iconClass = 'bi bi-x-circle-fill';
         defaultTitle = 'Error';
          modalClass = 'notification-error';
   break;
        case 'warning':
     iconClass = 'bi bi-exclamation-triangle-fill';
            defaultTitle = 'Advertencia';
     modalClass = 'notification-warning';
         break;
        case 'info':
      default:
   iconClass = 'bi bi-info-circle-fill';
            defaultTitle = 'Información';
            modalClass = 'notification-info';
         break;
    }

    // Aplicar estilos
    modal.classList.add(modalClass);
    icon.className = `notification-icon ${iconClass}`;
    titleElement.textContent = title || defaultTitle;
    messageElement.textContent = message;

    // Guardar el callback para cuando se cierre
    btnOk.onclick = function() {
        closeNotificationModal();
        if (onClose && typeof onClose === 'function') {
          // Ejecutar callback después de que se cierre la animación
            setTimeout(onClose, 350);
}
    };

    // Mostrar modal con animación
    overlay.style.display = 'flex';
    setTimeout(() => {
        overlay.classList.add('active');
    }, 10);
}

/**
 * Cierra el modal de notificación
 */
function closeNotificationModal() {
    const overlay = document.getElementById('notification-modal-overlay');
    overlay.classList.remove('active');
    setTimeout(() => {
  overlay.style.display = 'none';
    }, 300);
}

/**
 * Muestra un modal de confirmación con botones Aceptar/Cancelar
 * @param {string} message - Mensaje de confirmación
 * @param {function} onConfirm - Callback si el usuario acepta
 * @param {function} onCancel - Callback si el usuario cancela (opcional)
 */
function showConfirm(message, onConfirm, onCancel = null) {
    const overlay = document.getElementById('confirmation-modal-overlay');
  if (!overlay) {
        // Si no existe el modal de confirmación, usar confirm nativo como fallback
        if (confirm(message)) {
       if (onConfirm) onConfirm();
        } else {
        if (onCancel) onCancel();
  }
        return;
    }

    const messageElement = document.getElementById('confirmation-message');
    const btnConfirm = document.getElementById('confirmation-btn-confirm');
    const btnCancel = document.getElementById('confirmation-btn-cancel');

    messageElement.textContent = message;

    // Configurar botón Aceptar
    btnConfirm.onclick = function() {
      closeConfirmationModal();
        if (onConfirm && typeof onConfirm === 'function') {
    setTimeout(onConfirm, 350);
        }
};

    // Configurar botón Cancelar
    btnCancel.onclick = function() {
        closeConfirmationModal();
  if (onCancel && typeof onCancel === 'function') {
       setTimeout(onCancel, 350);
      }
    };

    // Mostrar modal
    overlay.style.display = 'flex';
    setTimeout(() => {
   overlay.classList.add('active');
    }, 10);
}

/**
 * Cierra el modal de confirmación
 */
function closeConfirmationModal() {
  const overlay = document.getElementById('confirmation-modal-overlay');
  if (overlay) {
        overlay.classList.remove('active');
        setTimeout(() => {
            overlay.style.display = 'none';
        }, 300);
    }
}

/**
 * Reemplazar alert() nativo
 */
window.alert = function(message) {
    showNotification(message, 'info');
};

/**
 * Reemplazar confirm() nativo
 */
window.confirm = function(message) {
    console.warn('confirm() debe usarse con callback. Usa showConfirm() en su lugar.');
    return window.originalConfirm ? window.originalConfirm(message) : true;
};

// Guardar el confirm original por si acaso
window.originalConfirm = window.confirm;

/**
 * Funciones auxiliares
 */
function showSuccess(message, title = null, onClose = null) {
    showNotification(message, 'success', title, onClose);
}

function showError(message, title = null, onClose = null) {
    showNotification(message, 'error', title, onClose);
}

function showWarning(message, title = null, onClose = null) {
    showNotification(message, 'warning', title, onClose);
}

function showInfo(message, title = null, onClose = null) {
    showNotification(message, 'info', title, onClose);
}

// Para compatibilidad con código que use showAlert
window.showAlert = function(message, type = 'info') {
    showNotification(message, type);
};

// NO permitir cerrar con ESC ni click fuera - Usuario DEBE presionar Aceptar
// (Comentado para forzar interacción)
/*
document.addEventListener('keydown', function(e) {
    if (e.key === 'Escape') {
        closeNotificationModal();
    }
});

document.addEventListener('click', function(e) {
    const overlay = document.getElementById('notification-modal-overlay');
    if (e.target === overlay) {
        closeNotificationModal();
    }
});
*/

// Funciones globales
window.showNotification = showNotification;
window.closeNotificationModal = closeNotificationModal;
window.showConfirm = showConfirm;
window.closeConfirmationModal = closeConfirmationModal;
window.showSuccess = showSuccess;
window.showError = showError;
window.showWarning = showWarning;
window.showInfo = showInfo;

console.log("? Sistema de notificaciones modales cargado correctamente");
