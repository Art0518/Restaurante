#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Script para corregir codificación UTF-8 en archivos Vue de páginas admin
Reemplaza todos los caracteres mal codificados por sus versiones correctas
"""

import os
import sys

# Configurar stdout para UTF-8
if sys.platform == 'win32':
    import codecs
    sys.stdout = codecs.getwriter('utf-8')(sys.stdout.buffer, 'strict')
  sys.stderr = codecs.getwriter('utf-8')(sys.stderr.buffer, 'strict')

# Lista de archivos a procesar
archivos = [
    r'CafeSanJuanVue\pages\admin-promociones\+Page.vue',
    r'CafeSanJuanVue\pages\admin-mesas\+Page.vue',
    r'CafeSanJuanVue\pages\admin-menu\+Page.vue',
    r'CafeSanJuanVue\pages\admin-clientes\+Page.vue',
    r'CafeSanJuanVue\pages\admin-facturacion\+Page.vue',
 r'CafeSanJuanVue\pages\components\Navbar.vue'
]

# Diccionario de reemplazos (mal -> bien)
reemplazos = {
    'Gestión': 'Gestión',
    'Promoción': 'Promoción',
    'promoción': 'promoción',
    'Administración': 'Administración',
    'Edición': 'Edición',
    'Número': 'Número',
    'número': 'número',
    'único': 'único',
    'Descripción': 'Descripción',
    'descripción': 'descripción',
  'Teléfono': 'Teléfono',
    'teléfono': 'teléfono',
    'Dirección': 'Dirección',
    'dirección': 'dirección',
    'Éxito': 'Éxito',
    '¿Está': '¿Está',
    '¿Estás': '¿Estás',
    'eliminación': 'eliminación',
    'Eliminación': 'Eliminación',
    'creación': 'creación',
  'información': 'información',
    'Búsqueda': 'Búsqueda',
    'búsqueda': 'búsqueda',
 'Método': 'Método',
    'método': 'método',
    'Generación': 'Generación',
'generación': 'generación',
    'Inactivación': 'Inactivación',
    'inactivación': 'inactivación',
    'Facturación': 'Facturación',
    'facturación': 'facturación',
    'categoría': 'categoría',
  'válido': 'válido',
    'básica': 'básica',
    'específico': 'específico',
    'página': 'página',
    'están': 'están',
    'más': 'más',
    'está': 'está',
    'será': 'será',
    'también': 'también',
    'después': 'después',
    'según': 'según',
    'código': 'código',
    'autenticación': 'autenticación',
    'selección': 'selección',
    'vacío': 'vacío',
    # Caracteres individuales comunes
    'ó': 'ó',
    'í': 'í',
    'á': 'á',
    'é': 'é',
    'ú': 'ú',
'ñ': 'ñ',
  '¿': '¿',
    '¡': '¡'
}

def corregir_archivo(ruta):
    """Corrige la codificación de un archivo"""
    if not os.path.exists(ruta):
        print(f'??  Archivo no encontrado: {ruta}')
        return False
    
    print(f'\n?? Procesando: {ruta}')
    
    try:
        # Leer el archivo con diferentes codificaciones posibles
    contenido = None
        for encoding in ['utf-8', 'latin-1', 'cp1252', 'iso-8859-1']:
            try:
with open(ruta, 'r', encoding=encoding) as f:
     contenido = f.read()
 break
except UnicodeDecodeError:
           continue
   
      if contenido is None:
       print(f'? No se pudo leer el archivo con ninguna codificación')
        return False
 
        # Aplicar reemplazos
        contenido_original = contenido
        cambios = 0
        
        for mal, bien in reemplazos.items():
   if mal in contenido:
        count = contenido.count(mal)
   contenido = contenido.replace(mal, bien)
cambios += count
    
        # Guardar el archivo en UTF-8
        if contenido != contenido_original:
    with open(ruta, 'w', encoding='utf-8', newline='\n') as f:
 f.write(contenido)
            print(f'? Archivo corregido. {cambios} reemplazos realizados')
            return True
        else:
            print(f'??  No se encontraron caracteres para corregir')
      return False
            
 except Exception as e:
    print(f'? Error procesando archivo: {str(e)}')
        return False

def main():
    """Función principal"""
    print('=' * 60)
    print('Corrección de codificación UTF-8 en páginas admin')
    print('=' * 60)
    
    archivos_corregidos = 0
    archivos_totales = len(archivos)
    
    for archivo in archivos:
        if corregir_archivo(archivo):
      archivos_corregidos += 1
    
    print('\n' + '=' * 60)
    print(f'? Proceso completado')
    print(f'?? Archivos corregidos: {archivos_corregidos}/{archivos_totales}')
    print('=' * 60)
  print('\nPor favor, recarga tu aplicación Vue para ver los cambios.')

if __name__ == '__main__':
    main()
