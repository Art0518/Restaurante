# ?? Guía Rápida - Pruebas de APIs con test-apis.http

## ?? Ubicación del archivo
El archivo `test-apis.http` está en la raíz del proyecto:
```
C:\Users\luisa\Documents\CafeSanJuan\test-apis.http
```

## ?? Cómo usar el archivo en Visual Studio

### **Método 1: Abrir directamente**
1. En Visual Studio, presiona **`Ctrl + O`**
2. Navega a la carpeta del proyecto
3. Selecciona `test-apis.http`
4. ¡Listo! Verás flechitas verdes ?? al lado de cada petición

### **Método 2: Agregar al proyecto**
1. Clic derecho en el Solution Explorer
2. **Add ? Existing Item...**
3. Selecciona `test-apis.http`
4. Haz clic en **Add**

### **Método 3: Arrastrar y soltar**
1. Abre el explorador de archivos de Windows
2. Navega a `C:\Users\luisa\Documents\CafeSanJuan\`
3. Arrastra `test-apis.http` al Solution Explorer de Visual Studio

---

## ?? Iniciar los Servicios

### **Opción A: Script Automático (Recomendado)**

Ejecuta el script PowerShell para iniciar todos los servicios:

```powershell
cd C:\Users\luisa\Documents\CafeSanJuan
.\start-all-services.ps1
```

Esto abrirá 4 ventanas de PowerShell, una para cada servicio:
- ? **SeguridadService** en puerto **5001**
- ? **MenuService** en puerto **5002**
- ? **ReservasService** en puerto **5003**
- ? **FacturacionService** en puerto **5004**

### **Opción B: Manual (4 terminales)**

Terminal 1 - SeguridadService:
```powershell
cd SeguridadService
dotnet run
```

Terminal 2 - MenuService:
```powershell
cd MenuService
dotnet run
```

Terminal 3 - ReservasService:
```powershell
cd ReservasService
dotnet run
```

Terminal 4 - FacturacionService:
```powershell
cd FacturacionService
dotnet run
```

---

## ?? Ejecutar las Pruebas

### En Visual Studio:
1. Abre `test-apis.http`
2. Verás una **flecha verde ??** al lado izquierdo de cada petición HTTP
3. Haz clic en la flecha para ejecutar esa petición
4. Los resultados aparecerán en un panel a la derecha

### Ejemplo visual:
```
### [SEGURIDAD-01] Verificar servicio de Seguridad  ??? Haz clic aquí
GET {{baseUrlSeguridad}}/               en la flecha
Content-Type: application/json         verde ??
```

---

## ?? Orden Recomendado de Pruebas

### ?? **NIVEL 1: Verificar que todos los servicios funcionan**

1. **[SEGURIDAD-01]** - Verificar servicio de Seguridad
2. **[MENU-01]** - Verificar servicio de Menú
3. **[RESERVAS-01]** - Verificar servicio de Reservas
4. **[FACTURACION-01]** - Verificar servicio de Facturación

? **Resultado esperado**: Todos deben responder con un mensaje del servicio

---

### ?? **NIVEL 2: Crear datos básicos**

#### A) Crear usuarios:
5. **[SEGURIDAD-02]** - Registrar nuevo usuario
6. **[SEGURIDAD-03]** - Registrar usuario Admin
7. **[SEGURIDAD-04]** - Login (guarda el token)

#### B) Crear platos:
8. **[MENU-04]** - Crear plato Espresso
9. **[MENU-05]** - Crear plato Cappuccino
10. **[MENU-06]** - Crear plato Gallo Pinto

#### C) Crear promociones:
11. **[PROMOCIONES-03]** - Crear promoción Navideña

---

### ?? **NIVEL 3: Flujo completo de negocio**

12. **[RESERVAS-02]** - Ver restaurantes disponibles
13. **[RESERVAS-03]** - Ver mesas disponibles
14. **[RESERVAS-05]** - Crear una reserva
15. **[FACTURACION-03]** - Generar factura para la reserva
16. **[FACTURACION-12]** - Marcar factura como pagada

---

## ?? Código de Colores en las Respuestas

| Color | Significado |
|-------|-------------|
| ?? **200** | ? Éxito - Todo funcionó correctamente |
| ?? **400** | ?? Bad Request - Error en los datos enviados |
| ?? **404** | ? Not Found - Recurso no encontrado |
| ?? **500** | ? Server Error - Error en el servidor |

---

## ?? Variables de Entorno

El archivo usa estas variables (definidas al inicio):

```http
@baseUrlSeguridad = http://localhost:5001
@baseUrlMenu = http://localhost:5002
@baseUrlReservas = http://localhost:5003
@baseUrlFacturacion = http://localhost:5004
```

Puedes cambiarlas si tus servicios corren en otros puertos.

---

## ?? Estructura del Archivo

El archivo está organizado en secciones:

```
test-apis.http
?
??? ?? SEGURIDAD SERVICE (13 pruebas)
?   ??? Registro de usuarios
?   ??? Login y tokens
?   ??? Gestión de usuarios
?
??? ??? MENU SERVICE (16 pruebas)
?   ??? Gestión de platos
?   ??? Gestión de promociones
?
??? ?? RESERVAS SERVICE (16 pruebas)
?   ??? Consulta de restaurantes y mesas
? ??? Gestión de reservas
?
??? ?? FACTURACION SERVICE (15 pruebas)
?   ??? Generación y gestión de facturas
?
??? ?? FLUJOS COMPLETOS (3 flujos)
?   ??? Flujo de usuario completo
?   ??? Flujo de menú
?   ??? Flujo de reserva + facturación
?
??? ? PRUEBAS DE ERROR (12 pruebas)
    ??? Casos negativos y validaciones
```

---

## ?? Ejemplo de Uso Paso a Paso

### **Paso 1**: Iniciar servicios
```powershell
.\start-all-services.ps1
```

### **Paso 2**: Abrir test-apis.http en Visual Studio
- Presiona `Ctrl + O`
- Selecciona `test-apis.http`

### **Paso 3**: Verificar servicios
Ejecuta en orden:
- [SEGURIDAD-01]
- [MENU-01]
- [RESERVAS-01]
- [FACTURACION-01]

### **Paso 4**: Crear un usuario y hacer login
```http
### [SEGURIDAD-02] Registrar nuevo usuario
POST {{baseUrlSeguridad}}/api/usuarios/registrar
Content-Type: application/json

{
  "nombre": "Juan",
  "apellido": "Pérez",
  "email": "juan.perez@cafe.com",
  "contrasena": "Pass123!",
  "telefono": "8765-4321",
  "rol": "Usuario"
}
```

Luego:
```http
### [SEGURIDAD-04] Login
POST {{baseUrlSeguridad}}/api/usuarios/login
Content-Type: application/json

{
  "email": "juan.perez@cafe.com",
  "contrasena": "Pass123!"
}
```

**Respuesta esperada**:
```json
{
  "success": true,
  "mensaje": "Login exitoso",
  "data": {
    "usuario": { ... },
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

### **Paso 5**: Crear platos
```http
### [MENU-04] Crear plato - Espresso Doble
POST {{baseUrlMenu}}/api/platos/crear
```

### **Paso 6**: Crear una reserva
```http
### [RESERVAS-05] Crear reserva
POST {{baseUrlReservas}}/api/reservas/crear
```

### **Paso 7**: Generar factura
```http
### [FACTURACION-03] Generar factura
POST {{baseUrlFacturacion}}/api/facturacion/generar
```

---

## ?? Detener los Servicios

### **Opción A: Script Automático**
```powershell
.\stop-all-services.ps1
```

### **Opción B: Manual**
- Presiona `Ctrl + C` en cada terminal
- O cierra las ventanas de PowerShell

---

## ?? Troubleshooting

### Problema: "No se puede conectar al servicio"
**Solución**:
1. Verifica que el servicio esté corriendo
2. Revisa que el puerto esté correcto
3. Asegúrate de que no haya firewall bloqueando

### Problema: "400 Bad Request"
**Solución**:
1. Verifica el formato JSON
2. Asegúrate de que todos los campos requeridos estén presentes
3. Revisa que los IDs existan en la base de datos

### Problema: "500 Internal Server Error"
**Solución**:
1. Revisa los logs del servicio en la terminal
2. Verifica la conexión a la base de datos
3. Asegúrate de que las tablas existan

### Problema: "No veo las flechas verdes ??"
**Solución**:
- Asegúrate de que tienes Visual Studio 2022 actualizado
- El soporte para archivos `.http` viene nativo en VS 2022

---

## ?? Recursos Adicionales

### Scripts útiles:
- `start-all-services.ps1` - Inicia todos los servicios
- `stop-all-services.ps1` - Detiene todos los servicios
- `test-apis.http` - Archivo de pruebas

### Puertos de los servicios:
- **5001** - SeguridadService
- **5002** - MenuService
- **5003** - ReservasService
- **5004** - FacturacionService

### Base de datos:
- **Servidor**: localhost
- **Base de datos**: CafeSanJuan
- **Tipo**: SQL Server

---

## ?? Tips Profesionales

1. **Organiza las pruebas**: Ejecuta primero las pruebas de verificación
2. **Guarda los IDs**: Cuando crees recursos, anota sus IDs para usarlos después
3. **Usa los flujos completos**: Son perfectos para demos
4. **Prueba los errores**: Las pruebas negativas son importantes
5. **Revisa los logs**: Los servicios muestran información útil en la consola

---

## ? Checklist de Pruebas

- [ ] Todos los servicios están corriendo
- [ ] Puedo registrar un usuario
- [ ] Puedo hacer login y obtener token
- [ ] Puedo crear platos
- [ ] Puedo crear promociones
- [ ] Puedo ver restaurantes y mesas
- [ ] Puedo crear una reserva
- [ ] Puedo generar una factura
- [ ] Puedo marcar una factura como pagada
- [ ] Las pruebas de error funcionan correctamente

---

## ?? ¡Listo!

Ahora tienes todo lo necesario para probar tus microservicios de manera profesional.

**¿Necesitas ayuda?** Revisa los logs de los servicios o las respuestas HTTP para más detalles.

---

**Creado para el proyecto CafeSanJuan** ?
**Arquitectura de Microservicios con gRPC y REST APIs**
