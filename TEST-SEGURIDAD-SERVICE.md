# ?? PRUEBAS RÁPIDAS - SEGURIDAD SERVICE

## ? Estado del Servicio
**SeguridadService** está configurado y listo para probar en: `http://localhost:5001`

---

## ?? GUÍA DE PRUEBAS PASO A PASO

### 1?? Verificar que el Servicio está Activo

```http
GET http://localhost:5001/api/usuarios
```

**Respuesta Esperada:**
```json
{
  "mensaje": "Servicio de Seguridad activo",
  "version": "1.0"
}
```

---

### 2?? Registrar un Usuario Nuevo

```http
POST http://localhost:5001/api/usuarios/registrar
Content-Type: application/json

{
  "nombre": "Juan",
  "apellido": "Pérez",
  "email": "juan.perez@cafe.com",
  "contrasena": "Pass123!",
  "telefono": "809-765-4321",
  "cedula": "001-2345678-9",
  "direccion": "Calle Principal #123, Santo Domingo",
  "rol": "Usuario"
}
```

**Respuesta Esperada:**
```json
{
  "mensaje": "Usuario registrado correctamente"
}
```

---

### 3?? Hacer Login y Obtener Token JWT

```http
POST http://localhost:5001/api/usuarios/login
Content-Type: application/json

{
  "email": "juan.perez@cafe.com",
  "contrasena": "Pass123!"
}
```

**Respuesta Esperada:**
```json
{
  "mensaje": "Login exitoso",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "usuario": {
    "idUsuario": 1,
"nombre": "Juan Pérez",
    "email": "juan.perez@cafe.com",
    "cedula": "001-2345678-9",
 "rol": "Usuario",
    "estado": "ACTIVO",
    "telefono": "809-765-4321",
    "direccion": "Calle Principal #123, Santo Domingo"
  }
}
```

**?? IMPORTANTE:** Guarda el `token` para las siguientes pruebas.

---

### 4?? Listar Todos los Usuarios

```http
GET http://localhost:5001/api/usuarios/listar
```

**Respuesta Esperada:**
```json
[
  {
 "IdUsuario": 1,
    "Nombre": "Juan Pérez",
    "Email": "juan.perez@cafe.com",
    "Cedula": "001-2345678-9",
    "Rol": "Usuario",
    "Estado": "ACTIVO",
    "Telefono": "809-765-4321",
    "Direccion": "Calle Principal #123, Santo Domingo"
  }
]
```

---

### 5?? Obtener Usuario por ID

```http
GET http://localhost:5001/api/usuarios/1
```

**Respuesta Esperada:**
```json
{
  "idUsuario": 1,
  "nombre": "Juan Pérez",
  "email": "juan.perez@cafe.com",
  "cedula": "001-2345678-9",
  "telefono": "809-765-4321",
  "direccion": "Calle Principal #123, Santo Domingo",
  "rol": "Usuario",
  "estado": "ACTIVO",
  "fechaCreacion": "2024-12-29T...",
  "ultimaConexion": "2024-12-29T..."
}
```

---

### 6?? Actualizar Usuario

```http
PUT http://localhost:5001/api/usuarios/1
Content-Type: application/json

{
  "nombre": "Juan Carlos Pérez González",
  "email": "juan.perez@cafe.com",
"telefono": "809-765-4321",
  "cedula": "001-2345678-9",
  "direccion": "Av. Principal #456, Santo Domingo",
  "rol": "Usuario"
}
```

**Respuesta Esperada:**
```json
{
  "mensaje": "Usuario actualizado correctamente"
}
```

---

### 7?? Cambiar Contraseña

```http
POST http://localhost:5001/api/usuarios/1/cambiar-contrasena
Content-Type: application/json

{
  "contrasenaActual": "Pass123!",
  "nuevaContrasena": "NewPassword456!"
}
```

**Respuesta Esperada:**
```json
{
  "mensaje": "Contraseña cambiada correctamente"
}
```

---

### 8?? Validar Token JWT

```http
POST http://localhost:5001/api/usuarios/validar-token
Content-Type: application/json

{
  "token": "TU_TOKEN_AQUI"
}
```

**Respuesta Esperada:**
```json
{
  "mensaje": "Token válido",
  "valido": true,
  "idUsuario": 1,
  "email": "juan.perez@cafe.com",
  "rol": "Usuario"
}
```

---

### 9?? Listar Usuarios por Rol

```http
GET http://localhost:5001/api/usuarios/listar?rol=Usuario
GET http://localhost:5001/api/usuarios/listar?rol=Administrador
```

---

## ?? PRUEBAS DE ERRORES

### Error 1: Login con Email Inexistente

```http
POST http://localhost:5001/api/usuarios/login
Content-Type: application/json

{
  "email": "noexiste@cafe.com",
  "contrasena": "cualquierpass"
}
```

**Respuesta Esperada:**
```json
{
  "mensaje": "Credenciales inválidas"
}
```
**HTTP Status:** 401 Unauthorized

---

### Error 2: Registro con Email Duplicado

```http
POST http://localhost:5001/api/usuarios/registrar
Content-Type: application/json

{
  "nombre": "Otro",
  "apellido": "Usuario",
  "email": "juan.perez@cafe.com",
  "contrasena": "Pass123!",
  "telefono": "809-888-8888",
  "cedula": "001-9999999-9",
  "direccion": "Calle Test #1, Santo Domingo",
  "rol": "Usuario"
}
```

**Respuesta Esperada:**
```json
{
  "mensaje": "Ya existe un usuario con este correo electrónico."
}
```
**HTTP Status:** 400 Bad Request

---

### Error 3: Usuario No Encontrado

```http
GET http://localhost:5001/api/usuarios/99999
```

**Respuesta Esperada:**
```json
{
  "mensaje": "Usuario no encontrado"
}
```
**HTTP Status:** 404 Not Found

---

### Error 4: Token Inválido

```http
POST http://localhost:5001/api/usuarios/validar-token
Content-Type: application/json

{
  "token": "token_invalido_xyz123"
}
```

**Respuesta Esperada:**
```json
{
  "mensaje": "Token inválido o expirado"
}
```
**HTTP Status:** 401 Unauthorized

---

## ?? CARACTERÍSTICAS IMPLEMENTADAS

? **Registro de Usuarios** - Sin encriptación de contraseñas  
? **Login con JWT** - Tokens válidos por 24 horas  
? **CRUD Completo** - Crear, Leer, Actualizar, Eliminar  
? **Validación de Tokens** - Verificar autenticidad  
? **Cambio de Contraseña** - Con validación de contraseña actual  
? **Filtros** - Listar usuarios por rol  
? **Stored Procedures** - Usa sp_login_usuario, sp_registrar_usuario_2, etc.  
? **DataTable** - Compatible con Ws_Restaurante  
? **Validaciones** - Email y cédula únicos  

---

## ?? CAMPOS DE LA TABLA USUARIO

| Campo | Tipo | Descripción |
|-------|------|-------------|
| IdUsuario | int | ID autoincrementable |
| Nombre | varchar(200) | Nombre completo |
| Email | varchar(100) | Email único |
| Contrasena | varchar(255) | Contraseña sin encriptar |
| Rol | varchar(50) | Usuario/Administrador |
| Estado | varchar(20) | ACTIVO/INACTIVO |
| Telefono | varchar(20) | Formato: 809-XXX-XXXX |
| Direccion | varchar(255) | Dirección completa |
| Cedula | varchar(20) | Formato: XXX-XXXXXXX-X |
| FechaCreacion | datetime | Fecha de registro |
| UltimaConexion | datetime | Último login |

---

## ?? NOTAS IMPORTANTES

1. **Sin Encriptación**: Las contraseñas se almacenan en texto plano (como Ws_Restaurante)
2. **Token JWT**: Expira en 24 horas
3. **Validaciones**: Email y cédula deben ser únicos
4. **Formato Teléfono**: 809/829/849-XXX-XXXX
5. **Formato Cédula**: XXX-XXXXXXX-X (11 dígitos)
6. **Rol por Defecto**: "Usuario" si no se especifica
7. **Estado por Defecto**: "ACTIVO" al registrar

---

## ?? PRÓXIMOS PASOS

Ahora que el SeguridadService está funcionando, puedes:

1. ? Probar todos los endpoints con `test-apis.http`
2. ?? Configurar MenuService (platos y promociones)
3. ?? Configurar ReservasService (reservas, mesas, restaurantes)
4. ?? Configurar FacturacionService (facturas)

---

## ?? TROUBLESHOOTING

### Problema: "No se pudo conectar al servidor"
**Solución:** Verifica que el servicio esté corriendo en `http://localhost:5001`

### Problema: "Error de conexión a la base de datos"
**Solución:** Verifica la cadena de conexión en `appsettings.json`

### Problema: "Ya existe un usuario con este correo"
**Solución:** Usa un email diferente o elimina el usuario existente

### Problema: "Credenciales inválidas"
**Solución:** Verifica que el email y contraseña sean correctos (sin encriptación)

---

**? ¡El SeguridadService está listo para usar!**
