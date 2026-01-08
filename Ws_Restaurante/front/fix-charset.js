// FIX CHARSET - Utilidades para manejo correcto de codificación UTF-8
// ============================================================

/**
 * Asegura que todos los fetch requests usen codificación UTF-8
 */
function setupCharsetFix() {
    // Override del fetch global para incluir charset UTF-8
    const originalFetch = window.fetch;
    
    window.fetch = function(input, init = {}) {
     // Asegurar headers UTF-8 para requests POST/PUT/PATCH
if (init.method && ['POST', 'PUT', 'PATCH'].includes(init.method.toUpperCase())) {
        init.headers = init.headers || {};
  if (!init.headers['Content-Type']) {
      init.headers['Content-Type'] = 'application/json; charset=utf-8';
} else if (init.headers['Content-Type'].includes('application/json') && 
           !init.headers['Content-Type'].includes('charset')) {
    init.headers['Content-Type'] = init.headers['Content-Type'] + '; charset=utf-8';
        }
        }
        
  // Asegurar Accept header con UTF-8
        init.headers = init.headers || {};
        if (!init.headers['Accept']) {
            init.headers['Accept'] = 'application/json, text/plain, */*; charset=utf-8';
        }
        
        return originalFetch.call(this, input, init);
    };
}

/**
 * Función mejorada para escapar HTML que preserva caracteres especiales
 */
function safeEscapeHtml(text) {
    if (typeof text !== 'string') return text;
    
    // Crear elemento temporal
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

/**
 * Función para decodificar entidades HTML si es necesario
 */
function decodeHtmlEntities(text) {
    if (typeof text !== 'string') return text;
    
    const textarea = document.createElement('textarea');
  textarea.innerHTML = text;
    return textarea.value;
}

/**
 * Función para normalizar texto con tildes y caracteres especiales
 */
function normalizeText(text) {
    if (typeof text !== 'string') return text;
    
    // Si el texto contiene entidades HTML mal codificadas, intentar corregirlas
    let normalized = text;
    
    // Reemplazos comunes para caracteres mal codificados
    const replacements = {
      'Ã¡': 'á',
        'Ã©': 'é',
    'Ã­': 'í',
        'Ã³': 'ó',
        'Ãº': 'ú',
        'Ã±': 'ñ',
        'Ã': 'Á',
        'Ã': 'É',
        'Ã': 'Í',
        'Ã': 'Ó',
      'Ã': 'Ú',
        'Ã': 'Ñ',
        'Â¿': '¿',
   'Â¡': '¡',
        '&aacute;': 'á',
        '&eacute;': 'é',
        '&iacute;': 'í',
        '&oacute;': 'ó',
        '&uacute;': 'ú',
        '&ntilde;': 'ñ',
        '&Aacute;': 'Á',
        '&Eacute;': 'É',
    '&Iacute;': 'Í',
        '&Oacute;': 'Ó',
        '&Uacute;': 'Ú',
        '&Ntilde;': 'Ñ',
  '&iquest;': '¿',
        '&iexcl;': '¡'
    };
    
    Object.keys(replacements).forEach(key => {
        normalized = normalized.replace(new RegExp(key, 'g'), replacements[key]);
    });
 
    return normalized;
}

/**
 * Función para insertar texto de forma segura en elementos DOM
 */
function safeSetTextContent(element, text) {
    if (!element) return;
    
    // Normalizar el texto antes de insertarlo
    const normalizedText = normalizeText(text);
    element.textContent = normalizedText;
}

/**
 * Función para insertar HTML de forma segura
 */
function safeSetInnerHTML(element, html) {
    if (!element) return;
    
    // Normalizar el HTML antes de insertarlo
    const normalizedHtml = normalizeText(html);
    element.innerHTML = normalizedHtml;
}

/**
 * Configuración automática al cargar el script
 */
document.addEventListener('DOMContentLoaded', function() {
 setupCharsetFix();
    console.log('? Charset fix configurado correctamente');
});

// Aplicar fix inmediatamente si el DOM ya está cargado
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupCharsetFix);
} else {
    setupCharsetFix();
}

// Exportar funciones para uso global
window.CharsetFix = {
    safeEscapeHtml,
    decodeHtmlEntities,
    normalizeText,
    safeSetTextContent,
    safeSetInnerHTML
};