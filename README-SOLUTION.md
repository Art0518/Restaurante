# ??? CafeSanJuan - Solución Completa

## ?? Estructura de la Solución

La solución `CafeSanJuan.sln` contiene los siguientes proyectos organizados por tipo:

---

## ?? **Frontend**

### **CafeSanJuanVue** (.esproj - Vue 3 + Vite)
- ?? Framework: **Vue 3** con Vite
- ?? Descripción: Aplicación web del restaurante
- ?? Comando: `npm run dev`
- ?? Puerto: `http://localhost:3000`
- ?? Características:
  - SSR con Vike
  - Componentes Vue 3 (Composition API)
  - Estilo personalizado con CSS
  - Integración con APIs REST y GraphQL

---

## ?? **Microservicios (.NET 8)**

### **SeguridadService** (Puerto 5001)
- ?? **Gestión de usuarios y autenticación**
- ? gRPC: Servicio de seguridad
- ? GraphQL: Queries y Mutations de usuarios
- ? REST API: Endpoints de usuarios
- ?? Base de datos: SQL Server

### **ReservasService** (Puerto 5003)
- ?? **Gestión de reservas y mesas**
- ? gRPC: Servicio de reservas
- ? GraphQL: Queries y Mutations de reservas/mesas
- ? REST API: Endpoints de reservas
- ?? Base de datos: SQL Server

### **MenuService** (Puerto 5002)
- ??? **Gestión de platos y promociones**
- ? gRPC: Servicio de menú
- ? GraphQL: Queries y Mutations de platos/promociones
- ? REST API: Endpoints de menú
- ?? Base de datos: SQL Server

### **FacturacionService** (Puerto 5004)
- ?? **Gestión de facturas y carrito**
- ? gRPC: Servicio de facturación
- ? GraphQL: Queries y Mutations de facturas
- ? REST API: Endpoints de facturación
- ?? Base de datos: SQL Server

### **GrpcClients**
- ?? **Cliente gRPC compartido**
- ?? Librería de clientes para consumir los servicios gRPC
- ?? Configuración centralizada de conexiones

---

## ?? **Servicios Web (.NET Framework 4.8)**

### **Ws_Restaurante**
- ?? **Web API principal del restaurante**
- ?? Servicios REST para integración legacy
- ?? Integración con servicios externos

### **Ws_GIntegracionBus**
- ?? **Bus de integración**
- ?? Gestión de transacciones distribuidas
- ?? Comunicación entre servicios legacy

### **WS_GestionBusSOAP**
- ?? **Servicio SOAP de gestión**
- ?? Servicios web tradicionales
- ?? Compatibilidad con sistemas externos

---

## ?? **Capas de Datos y Lógica (.NET Standard 2.0)**

### **AccesoDatos**
- ?? **Capa de acceso a datos**
- ??? DAOs para todas las entidades
- ?? Conexión con SQL Server

### **Logica**
- ?? **Capa de lógica de negocio**
- ?? Servicios de negocio
- ?? Validaciones y reglas

### **GDatos**
- ?? **Gestión de datos compartida**
- ?? Utilidades de datos
- ?? Helpers y extensiones

---

## ??? **Base de Datos**

- **Motor**: SQL Server
- **Nombre**: `RestauranteDB`
- **Scripts**:
  - `init-database.sql` - Inicialización completa
  - `Database/*.sql` - Stored procedures

---

## ?? **Comandos de Ejecución**

### **Iniciar todos los servicios:**
```bash
.\start-all-services.ps1
```

### **Detener todos los servicios:**
```bash
.\stop-all-services.ps1
```

### **Iniciar servicios individualmente:**
```bash
# Backend (.NET 8)
cd SeguridadService && dotnet run
cd ReservasService && dotnet run
cd MenuService && dotnet run
cd FacturacionService && dotnet run

# Frontend (Vue)
cd CafeSanJuanVue && npm run dev
```

---

## ?? **Puertos y Endpoints**

| Servicio | Puerto | REST API | gRPC | GraphQL |
|----------|--------|----------|------|---------|
| **SeguridadService** | 5001 | ? `/api/usuarios` | ? | ? `/graphql` |
| **MenuService** | 5002 | ? `/api/platos` | ? | ? `/graphql` |
| **ReservasService** | 5003 | ? `/api/reservas` | ? | ? `/graphql` |
| **FacturacionService** | 5004 | ? `/api/facturacion` | ? | ? `/graphql` |
| **CafeSanJuanVue** | 3000 | - | - | - |

---

## ?? **Pruebas**

### **Archivos de prueba HTTP:**
- `test-apis.http` - Pruebas REST API
- `test-graphql.http` - Pruebas GraphQL
- `test-graphql-health.http` - Health check GraphQL
- `test-facturacion.http` - Pruebas de facturación

### **Scripts de prueba:**
- `test-db-connection.ps1` - Prueba de conexión a BD
- `test-monster-connections.ps1` - Pruebas de carga

---

## ??? **Tecnologías Utilizadas**

### **Frontend:**
- Vue 3 (Composition API)
- Vite
- Vike (SSR)
- CSS personalizado

### **Backend:**
- .NET 8 (Microservicios)
- .NET Framework 4.8 (Servicios legacy)
- .NET Standard 2.0 (Librerías compartidas)
- gRPC
- GraphQL (HotChocolate)
- REST API (ASP.NET Core)
- Entity Framework
- SQL Server

---

## ?? **Instalación y Configuración**

### **Requisitos:**
- Visual Studio 2022
- .NET 8 SDK
- .NET Framework 4.8
- Node.js 18+
- SQL Server 2019+

### **Pasos:**

1. **Restaurar paquetes .NET:**
```bash
dotnet restore
```

2. **Instalar dependencias Vue:**
```bash
cd CafeSanJuanVue
npm install
```

3. **Configurar base de datos:**
```bash
# Ejecutar init-database.sql en SQL Server
```

4. **Iniciar servicios:**
```bash
.\start-all-services.ps1
```

---

## ?? **Equipo de Desarrollo**

- **Proyecto**: Sistema de Gestión de Restaurante
- **Cliente**: Un Rincón en San Juan
- **Arquitectura**: Microservicios + Frontend Vue

---

## ?? **Licencia**

Proyecto académico - Universidad [Nombre]
