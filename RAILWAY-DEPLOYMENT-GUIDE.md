# ?? Guía de Despliegue en Railway - CafeSanJuan Microservicios

## ?? Prerequisitos

- Cuenta en [Railway](https://railway.app)
- Repositorio GitHub: `https://github.com/Art0518/Restaurante`
- Base de datos SQL Server ya configurada en: `db31553.public.databaseasp.net`

---

## ?? Pasos para Desplegar cada Microservicio

### 1. **SeguridadService (Autenticación y Usuarios)**

#### En Railway:
1. **Crear Nuevo Proyecto**
   - Ve a [Railway Dashboard](https://railway.app/dashboard)
   - Click en "New Project" ? "Deploy from GitHub repo"
   - Selecciona `Art0518/Restaurante`

2. **Configurar el Servicio**
   - Railway detectará automáticamente el Dockerfile
   - **Root Directory**: `SeguridadService`
   - **Dockerfile Path**: `SeguridadService/Dockerfile`

3. **Variables de Entorno**
   - Railway asigna automáticamente la variable `PORT`
   - Agrega las siguientes variables:
   
 ```env
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=Server=db31553.public.databaseasp.net;Database=db31553;User Id=db31553;Password=0520ARTU;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=60;
   Jwt__Secret=tu_clave_secreta_muy_larga_minimo_32_caracteres_aqui
   Jwt__Issuer=CafeSanJuan
 Jwt__Audience=CafeSanJuanClients
   Jwt__ExpiryMinutes=1440
   ```

4. **Generar Dominio Público**
   - Ve a "Settings" ? "Networking"
   - Click en "Generate Domain"
   - Copia la URL (ejemplo: `seguridadservice-production.up.railway.app`)

---

### 2. **ReservasService (Reservas y Mesas)**

#### En Railway:
1. **Agregar Nuevo Servicio al Proyecto**
   - En el mismo proyecto, click en "+ New"
   - Selecciona "GitHub Repo" ? `Art0518/Restaurante`

2. **Configurar el Servicio**
   - **Root Directory**: `ReservasService`
   - **Dockerfile Path**: `ReservasService/Dockerfile`

3. **Variables de Entorno**
   ```env
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=Server=db31553.public.databaseasp.net;Database=db31553;User Id=db31553;Password=0520ARTU;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=60;
   ```

4. **Generar Dominio Público**
   - Settings ? Networking ? Generate Domain
   - Copia la URL (ejemplo: `reservasservice-production.up.railway.app`)

---

### 3. **MenuService (Platos y Promociones)**

#### En Railway:
1. **Agregar Nuevo Servicio al Proyecto**
   - En el mismo proyecto, click en "+ New"
   - Selecciona "GitHub Repo" ? `Art0518/Restaurante`

2. **Configurar el Servicio**
   - **Root Directory**: `MenuService`
   - **Dockerfile Path**: `MenuService/Dockerfile`

3. **Variables de Entorno**
   ```env
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=Server=db31553.public.databaseasp.net;Database=db31553;User Id=db31553;Password=0520ARTU;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=60;
   ```

4. **Generar Dominio Público**
   - Settings ? Networking ? Generate Domain
   - Copia la URL (ejemplo: `menuservice-production.up.railway.app`)

---

### 4. **FacturacionService (Facturas y Carritos)**

#### En Railway:
1. **Agregar Nuevo Servicio al Proyecto**
   - En el mismo proyecto, click en "+ New"
- Selecciona "GitHub Repo" ? `Art0518/Restaurante`

2. **Configurar el Servicio**
   - **Root Directory**: `FacturacionService`
   - **Dockerfile Path**: `FacturacionService/Dockerfile`

3. **Variables de Entorno**
   ```env
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=Server=db31553.public.databaseasp.net;Database=db31553;User Id=db31553;Password=0520ARTU;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=60;
   ```

4. **Generar Dominio Público**
   - Settings ? Networking ? Generate Domain
   - Copia la URL (ejemplo: `facturacionservice-production.up.railway.app`)

---

## ?? Verificar los Deploys

### Endpoints disponibles en cada servicio:

#### SeguridadService
- **REST**: `https://tu-dominio.up.railway.app/api/usuarios`
- **GraphQL**: `https://tu-dominio.up.railway.app/graphql`
- **Health**: `https://tu-dominio.up.railway.app/`

#### ReservasService
- **REST**: `https://tu-dominio.up.railway.app/api/reservas`
- **GraphQL**: `https://tu-dominio.up.railway.app/graphql`
- **Health**: `https://tu-dominio.up.railway.app/`

#### MenuService
- **REST**: `https://tu-dominio.up.railway.app/api/platos`
- **GraphQL**: `https://tu-dominio.up.railway.app/graphql`
- **Health**: `https://tu-dominio.up.railway.app/`

#### FacturacionService
- **REST**: `https://tu-dominio.up.railway.app/api/facturacion`
- **GraphQL**: `https://tu-dominio.up.railway.app/graphql`
- **Health**: `https://tu-dominio.up.railway.app/`

---

## ?? Actualizar Frontend Vue

Una vez que tengas todas las URLs de Railway, actualiza tu archivo de configuración del frontend:

**CafeSanJuanVue/src/config/api.config.js:**

```javascript
export const API_CONFIG = {
  SEGURIDAD_SERVICE: 'https://seguridadservice-production.up.railway.app',
  RESERVAS_SERVICE: 'https://reservasservice-production.up.railway.app',
  MENU_SERVICE: 'https://menuservice-production.up.railway.app',
  FACTURACION_SERVICE: 'https://facturacionservice-production.up.railway.app',
  
  // Endpoints GraphQL
  GRAPHQL: {
 SEGURIDAD: 'https://seguridadservice-production.up.railway.app/graphql',
    RESERVAS: 'https://reservasservice-production.up.railway.app/graphql',
    MENU: 'https://menuservice-production.up.railway.app/graphql',
    FACTURACION: 'https://facturacionservice-production.up.railway.app/graphql'
  }
}
```

---

## ??? Configuración Avanzada de Railway

### Configurar Build desde Railway Dashboard

Para cada servicio, en la configuración de Railway:

1. **Settings ? Build**
   - Builder: `DOCKERFILE`
   - Dockerfile Path: `[NombreServicio]/Dockerfile`
   - Docker Build Context: `.` (raíz del repositorio)

2. **Settings ? Deploy**
   - Start Command: (dejar vacío, usa el ENTRYPOINT del Dockerfile)
   - Health Check Path: `/` 
   - Health Check Timeout: `300` segundos
   - Restart Policy: `ON_FAILURE`

3. **Settings ? Networking**
   - Generate Domain (para acceso público)
   - Exponer puerto: Railway detecta automáticamente el puerto 8080

---

## ?? Actualizar Servicios Desplegados

Cuando hagas cambios en tu código:

```bash
# 1. Hacer commit de los cambios
git add .
git commit -m "Actualización de [nombre del servicio]"

# 2. Push a GitHub
git push origin main

# 3. Railway detectará el cambio y redesplegará automáticamente
```

---

## ?? Monitoreo

### Ver Logs en Railway:
1. Selecciona el servicio en el Dashboard
2. Click en la pestaña "Deployments"
3. Click en el último deployment
4. Ver "View Logs"

### Métricas disponibles:
- CPU Usage
- Memory Usage
- Network Traffic
- Request Count

---

## ?? Troubleshooting

### Error: "Application failed to start"
- Verifica que las variables de entorno estén configuradas
- Revisa los logs del deployment
- Asegúrate de que la base de datos sea accesible

### Error: "Port already in use"
- Railway maneja esto automáticamente con la variable `PORT`
- No necesitas especificar puertos manualmente

### Error de CORS
- Los microservicios ya tienen CORS configurado con `AllowAll`
- Para producción, considera restringir los orígenes permitidos

### Error de Base de Datos
- Verifica la connection string en las variables de entorno
- Asegúrate de que el servidor SQL permita conexiones externas
- Verifica el firewall del servidor de base de datos

---

## ?? Costos

Railway ofrece:
- **Plan Starter (Gratis)**: $5 USD de crédito mensual
- **Plan Developer**: $20 USD/mes con $20 de crédito incluido
- Uso adicional: ~$0.000231/GB-hour de memoria

**Estimación para 4 microservicios:**
- Cada servicio: ~512MB RAM
- Total estimado: $10-15 USD/mes (con plan Developer)

---

## ?? Recursos Adicionales

- [Railway Docs](https://docs.railway.app/)
- [Railway Discord](https://discord.gg/railway)
- [ASP.NET Core en Docker](https://docs.microsoft.com/aspnet/core/host-and-deploy/docker/)

---

## ? Checklist de Despliegue

- [ ] Crear proyecto en Railway
- [ ] Desplegar SeguridadService
- [ ] Desplegar ReservasService
- [ ] Desplegar MenuService
- [ ] Desplegar FacturacionService
- [ ] Configurar variables de entorno en cada servicio
- [ ] Generar dominios públicos
- [ ] Probar cada endpoint (Health check)
- [ ] Actualizar configuración del frontend Vue
- [ ] Probar la aplicación completa end-to-end

---

**¡Listo! Tus microservicios están desplegados en Railway ??**
