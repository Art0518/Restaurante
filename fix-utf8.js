const fs = require('fs');
const path = require('path');

const archivos = [
  'CafeSanJuanVue/pages/admin-promociones/+Page.vue',
  'CafeSanJuanVue/pages/admin-mesas/+Page.vue',
  'CafeSanJuanVue/pages/admin-menu/+Page.vue',
  'CafeSanJuanVue/pages/admin-clientes/+Page.vue',
  'CafeSanJuanVue/pages/admin-facturacion/+Page.vue',
  'CafeSanJuanVue/pages/components/Navbar.vue'
];

const reemplazos = {
  'Gesti?n': 'Gestión',
  'Promoci?n': 'Promoción',
  'promoci?n': 'promoción',
  'Administraci?n': 'Administración',
  'administraci?n': 'administración',
  'Edici?n': 'Edición',
  'edici?n': 'edición',
  'N?mero': 'Número',
  'n?mero': 'número',
  '?nico': 'único',
  'Descripci?n': 'Descripción',
  'descripci?n': 'descripción',
  'Tel?fono': 'Teléfono',
  'tel?fono': 'teléfono',
  'Direcci?n': 'Dirección',
  'direcci?n': 'dirección',
  'categor?a': 'categoría',
  'informaci?n': 'información',
  'p?gina': 'página',
  'b?squeda': 'búsqueda',
  'est?n': 'están',
  'm?s': 'más',
  'ser?': 'será',
  'est?': 'está',
  'despu?s': 'después',
  'seg?n': 'según',
  'v?lido': 'válido',
  'espec?fico': 'específico',
  'b?sica': 'básica',
  'm?todo': 'método',
  'tambi?n': 'también',
  'c?digo': 'código',
  'creaci?n': 'creación',
  'eliminaci?n': 'eliminación',
  'selecci?n': 'selección',
  'facturaci?n': 'facturaci??n',
  'Facturaci?n': 'Facturación',
  'Generaci?n': 'Generación',
  'generaci?n': 'generación',
  'Inactivaci?n': 'Inactivación',
  'inactivaci?n': 'inactivación',
  '?xito': 'Éxito',
  '?Est?': '¿Está',
  '?Est?s': '¿Estás',
  '?Generar': '¿Generar',
  'vac?o': 'vacío',
  'autenticaci?n': 'autenticación'
};

console.log('=== Iniciando corrección UTF-8 ===\n');

archivos.forEach(archivo => {
  try {
  if (!fs.existsSync(archivo)) {
      console.log(`??  Archivo no encontrado: ${archivo}`);
      return;
    }

    console.log(`?? Procesando: ${archivo}`);
    
    // Leer archivo
    let contenido = fs.readFileSync(archivo, 'utf8');
    
    // Aplicar reemplazos
    let cambios = 0;
    Object.keys(reemplazos).forEach(mal => {
      const regex = new RegExp(mal.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), 'g');
   const matches = (contenido.match(regex) || []).length;
      if (matches > 0) {
     contenido = contenido.replace(regex, reemplazos[mal]);
        cambios += matches;
      }
    });
    
    // Guardar archivo
fs.writeFileSync(archivo, contenido, 'utf8');
    
    console.log(`? Archivo corregido. ${cambios} reemplazos realizados\n`);
  } catch (error) {
    console.error(`? Error procesando ${archivo}:`, error.message);
  }
});

console.log('=== Corrección completada ===');
