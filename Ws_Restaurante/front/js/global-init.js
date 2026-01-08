/**
 * SCRIPT GLOBAL DE INICIALIZACIÓN
 * Este script se debe cargar en TODAS las páginas del sitio
 * Se encarga de:
 * 1. Verificar que el sistema de notificaciones esté disponible
 * 2. Proporcionar funciones globales de fallback
 * 3. Asegurar que navbar.js funcione correctamente
 */

// Esperar a que el DOM esté listo
document.addEventListener('DOMContentLoaded', function() {
    console.log('?? Inicializando sistema global...');
    
    // Verificar que las funciones de notificación estén disponibles
    const funcionesRequeridas = ['showNotification', 'showConfirm', 'showSuccess', 'showError', 'showWarning', 'showInfo'];
    const funcionesFaltantes = funcionesRequeridas.filter(fn => typeof window[fn] !== 'function');
    
    if (funcionesFaltantes.length > 0) {
        console.warn('?? Funciones de notificación faltantes:', funcionesFaltantes);
   console.log('?? Usando fallbacks nativos para:', funcionesFaltantes);
 
    // Proporcionar fallbacks si no están disponibles
  if (typeof window.showNotification !== 'function') {
      window.showNotification = function(message, type, title, callback) {
   alert(message);
                if (callback) callback();
            };
        }
        
        if (typeof window.showConfirm !== 'function') {
     window.showConfirm = function(message, onConfirm, onCancel) {
          if (confirm(message)) {
        if (onConfirm) onConfirm();
     } else {
   if (onCancel) onCancel();
                }
            };
        }
        
        if (typeof window.showSuccess !== 'function') {
            window.showSuccess = function(message, title, callback) {
    alert(message);
        if (callback) callback();
   };
        }
  
        if (typeof window.showError !== 'function') {
   window.showError = function(message, title, callback) {
                alert(message);
     if (callback) callback();
            };
}
        
     if (typeof window.showWarning !== 'function') {
            window.showWarning = function(message, title, callback) {
    alert(message);
     if (callback) callback();
            };
        }
        
        if (typeof window.showInfo !== 'function') {
  window.showInfo = function(message, title, callback) {
          alert(message);
       if (callback) callback();
            };
 }
    } else {
        console.log('? Todas las funciones de notificación están disponibles');
    }
    
    console.log('? Sistema global inicializado correctamente');
});
