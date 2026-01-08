// ============================================================
// ADMIN - GESTIÓN DE RESERVAS CON FACTURACIÓN
// ============================================================

// Variables globales
let todasLasReservas = [];
let reservasFiltradas = [];
let reservaSeleccionada = null;
let usuario = null;

// ============================================================
// INICIALIZACIÓN
// ============================================================
document.addEventListener('DOMContentLoaded', function() {
    // El usuario ya fue verificado en el HTML, solo lo recoveramos
    usuario = JSON.parse(localStorage.getItem('usuario'));
 
    console.log('? Iniciando aplicación admin - Usuario ya verificado:', usuario.Nombre);
    
    // Configurar event listeners
    setupEventListeners();
    
    // Cargar todas las reservas
 cargarTodasReservas();
});

// ============================================================
// EVENT LISTENERS
// ============================================================
function setupEventListeners() {
    // Filtros en tiempo real
    document.getElementById('filtro-estado').addEventListener('change', aplicarFiltros);
}

// ============================================================
// CARGAR TODAS LAS RESERVAS
// ============================================================
function cargarTodasReservas() {
 mostrarCargando(true);
    
    fetch('/api/reservas/admin/todas')
        .then(response => response.json())
  .then(data => {
  mostrarCargando(false);
          if (data.success) {
      todasLasReservas = data.reservas || [];
     reservasFiltradas = [...todasLasReservas];
    mostrarReservas(reservasFiltradas);
     actualizarEstadisticas(todasLasReservas);
     showAlert('Reservas cargadas exitosamente', 'success');
        } else {
    mostrarError(data.message || 'Error al cargar las reservas');
            mostrarSinReservas();
  }
  })
 .catch(error => {
     mostrarCargando(false);
    console.error('Error:', error);
       mostrarError('Error al conectar con el servidor');
mostrarSinReservas();
        });
}

// ============================================================
// MOSTRAR RESERVAS EN LA TABLA
// ============================================================
function mostrarReservas(reservas) {
    const tbody = document.getElementById('reservas-tbody');
    const sinReservas = document.getElementById('sin-reservas');
    
    if (!reservas || reservas.length === 0) {
       mostrarSinReservas();
        return;
    }

    sinReservas.style.display = 'none';
    tbody.innerHTML = '';

  reservas.forEach(reserva => {
        const fila = document.createElement('tr');
        
   const fecha = new Date(reserva.Fecha).toLocaleDateString('es-ES');
        const hora = reserva.Hora || 'No especificada';
  const total = reserva.Total || 0;
   const metodoPago = reserva.MetodoPago || 'Sin especificar';
  const estado = reserva.Estado || 'Desconocido';
        
        // Determinar clases CSS según el estado
        let estadoClass = '';
     let estadoBadge = '';
    switch (estado) {
      case 'HOLD':
       estadoClass = 'table-warning';
    estadoBadge = 'badge bg-warning text-dark';
       break;
 case 'CONFIRMADA':
       estadoClass = 'table-success';
      estadoBadge = 'badge bg-success';
        break;
   default:
        estadoBadge = 'badge bg-secondary';
        }
        
        fila.className = estadoClass;
 
        // Usar funciones seguras para el texto
 const nombreUsuario = window.CharsetFix ? 
         window.CharsetFix.normalizeText(reserva.NombreUsuario || 'Sin nombre') : 
         (reserva.NombreUsuario || 'Sin nombre');
 
        const emailUsuario = window.CharsetFix ? 
window.CharsetFix.normalizeText(reserva.EmailUsuario || 'Sin email') : 
       (reserva.EmailUsuario || 'Sin email');
            
      const tipoMesa = window.CharsetFix ? 
      window.CharsetFix.normalizeText(reserva.TipoMesa || 'Estándar') : 
            (reserva.TipoMesa || 'Estándar');
          
  const metodoPagoNorm = window.CharsetFix ? 
  window.CharsetFix.normalizeText(metodoPago) : 
         metodoPago;
  
        fila.innerHTML = `
<td><strong>#${reserva.IdReserva}</strong></td>
 <td>
  <div>
     <strong>${nombreUsuario}</strong>
  <br>
        <small class="text-muted">${emailUsuario}</small>
 </div>
   </td>
     <td>
     <span class="badge bg-primary">Mesa ${reserva.NumeroMesa}</span>
    <br>
        <small class="text-muted">${tipoMesa}</small>
  </td>
<td>${fecha}</td>
<td>${hora}</td>
 <td>
       <i class="bi bi-people"></i> ${reserva.NumeroPersonas}
  </td>
       <td>
            <strong class="text-success">$${total.toFixed(2)}</strong>
    </td>
   <td>
        <span class="${estadoBadge}">${estado}</span>
  </td>
         <td>
        <span class="badge bg-info">${metodoPagoNorm}</span>
 </td>
  <td>
     <button class="btn btn-sm btn-info" onclick="verDetalleReserva(${reserva.IdReserva})"
  title="Ver Detalles">
    <i class="bi bi-eye"></i> Detalles
            </button>
 </td>
        `;
        
        tbody.appendChild(fila);
        
      // Agregar fila de observaciones si existen
    if (reserva.Observaciones && reserva.Observaciones.trim() !== '') {
    const filaObs = document.createElement('tr');
  filaObs.className = 'table-light';
    
            const observacionesNorm = window.CharsetFix ? 
    window.CharsetFix.normalizeText(reserva.Observaciones) : 
     reserva.Observaciones;
     
    filaObs.innerHTML = `
    <td colspan="10">
     <small><i class="bi bi-chat-text text-muted"></i> 
 <strong>Observaciones:</strong> ${observacionesNorm}</small>
 </td>
`;
     tbody.appendChild(filaObs);
   }
    });
}

// ============================================================
// APLICAR FILTROS
// ============================================================
function aplicarFiltros() {
    const filtroEstado = document.getElementById('filtro-estado').value;

  // Si no hay filtros activos, mostrar todas las reservas
    if (!filtroEstado) {
  reservasFiltradas = [...todasLasReservas];
        mostrarReservas(reservasFiltradas);
        actualizarEstadisticas(reservasFiltradas);
   return;
    }

    reservasFiltradas = todasLasReservas.filter(reserva => {
    // Filtro por estado
     if (filtroEstado && reserva.Estado !== filtroEstado) {
      return false;
     }

   return true;
    });

    mostrarReservas(reservasFiltradas);
  actualizarEstadisticas(reservasFiltradas);
}

// ============================================================
// ACTUALIZAR ESTADÍSTICAS
// ============================================================
function actualizarEstadisticas(reservas) {
    const total = reservas.length;
    const enEspera = reservas.filter(r => r.Estado === 'HOLD').length;
    const confirmadas = reservas.filter(r => r.Estado === 'CONFIRMADA').length;
  const ingresosTotales = reservas
.filter(r => r.Estado === 'CONFIRMADA')
     .reduce((sum, r) => sum + (r.Total || 0), 0);

    document.getElementById('stat-total').textContent = total;
    document.getElementById('stat-hold').textContent = enEspera;
  document.getElementById('stat-confirmadas').textContent = confirmadas;
    document.getElementById('stat-ingresos').textContent = '$' + ingresosTotales.toFixed(2);
}

// ============================================================
// VER DETALLE DE RESERVA
// ============================================================
function verDetalleReserva(idReserva) {
 const reserva = todasLasReservas.find(r => r.IdReserva === idReserva);
    
    if (!reserva) {
        showAlert('Reserva no encontrada', 'error');
 return;
 }

  // Normalizar textos si el fix está disponible
 const nombreUsuario = window.CharsetFix ? 
    window.CharsetFix.normalizeText(reserva.NombreUsuario || 'No especificado') : 
   (reserva.NombreUsuario || 'No especificado');
 
  const emailUsuario = window.CharsetFix ? 
     window.CharsetFix.normalizeText(reserva.EmailUsuario || 'No especificado') : 
   (reserva.EmailUsuario || 'No especificado');
 
    const tipoMesa = window.CharsetFix ? 
    window.CharsetFix.normalizeText(reserva.TipoMesa || 'Estándar') : 
    (reserva.TipoMesa || 'Estándar');
   
 const metodoPago = window.CharsetFix ? 
   window.CharsetFix.normalizeText(reserva.MetodoPago || 'Sin especificar') : 
  (reserva.MetodoPago || 'Sin especificar');

    let detalleHtml = `
      <div class="row">
 <div class="col-md-6">
      <h6 class="text-primary">Informacion de Cliente:</h6>
    <p><strong>Nombre:</strong> ${nombreUsuario}</p>
        <p><strong>Email:</strong> ${emailUsuario}</p>
    <p><strong>ID Usuario:</strong> #${reserva.IdUsuario}</p>
 </div>
 <div class="col-md-6">
      <h6 class="text-primary">Detalles de Reserva:</h6>
      <p><strong>ID:</strong> #${reserva.IdReserva}</p>
   <p><strong>Mesa:</strong> Mesa ${reserva.NumeroMesa} (${tipoMesa})</p>
     <p><strong>Fecha:</strong> ${new Date(reserva.Fecha).toLocaleDateString('es-ES')}</p>
    <p><strong>Hora:</strong> ${reserva.Hora || 'No especificada'}</p>
  </div>
 </div>
      <div class="row">
  <div class="col-md-6">
 <h6 class="text-primary">Estado y Pago:</h6>
  <p><strong>Estado:</strong> <span class="badge bg-${reserva.Estado === 'CONFIRMADA' ? 'success' : 'warning'}">${reserva.Estado}</span></p>
  <p><strong>Metodo de Pago:</strong> ${metodoPago}</p>
 </div>
       <div class="col-md-6">
    <h6 class="text-primary">Informacion Adicional:</h6>
  <p><strong>Personas:</strong> ${reserva.NumeroPersonas}</p>
      <p><strong>Total:</strong> <span class="text-success fw-bold">$${(reserva.Total || 0).toFixed(2)}</span></p>
       </div>
  </div>
  `;

 if (reserva.Observaciones && reserva.Observaciones.trim() !== '') {
   const observacionesNorm = window.CharsetFix ? 
  window.CharsetFix.normalizeText(reserva.Observaciones) : 
   reserva.Observaciones;
  
      detalleHtml += `
   <div class="row">
   <div class="col-12">
 <h6 class="text-primary">Observaciones:</h6>
        <div class="bg-light p-3 rounded">
 ${observacionesNorm}
   </div>
    </div>
   </div>
  `;
    }

 showAlert(detalleHtml, 'info', 'Detalle de Reserva #' + reserva.IdReserva, true);
}

// ============================================================
// FUNCIONES DE UI
// ============================================================
function mostrarCargando(mostrar) {
    const loadingDiv = document.getElementById('loading-reservas');
    const tabla = document.getElementById('tabla-reservas');
  const sinReservas = document.getElementById('sin-reservas');
    
    if (mostrar) {
    loadingDiv.style.display = 'block';
        tabla.style.display = 'none';
        sinReservas.style.display = 'none';
    } else {
        loadingDiv.style.display = 'none';
        tabla.style.display = 'table';
    }
}

function mostrarSinReservas() {
    document.getElementById('loading-reservas').style.display = 'none';
    document.getElementById('tabla-reservas').style.display = 'none';
    document.getElementById('sin-reservas').style.display = 'block';
    
    // Limpiar estadísticas
    actualizarEstadisticas([]);
}

function mostrarError(mensaje) {
    const mensajeNorm = window.CharsetFix ? 
        window.CharsetFix.normalizeText(mensaje) : 
        mensaje;
    showAlert(mensajeNorm, 'danger');
}

function showAlert(message, type = 'info', title = null, isHtml = false) {
    const alertContainer = document.createElement('div');
    alertContainer.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
    alertContainer.style.cssText = 'top: 20px; right: 20px; z-index: 9999; max-width: 500px;';

    const titleNorm = title && window.CharsetFix ? 
window.CharsetFix.normalizeText(title) : 
        title;
        
    const messageNorm = !isHtml && window.CharsetFix ? 
 window.CharsetFix.normalizeText(message) : 
    message;

    const titleHtml = titleNorm ? `<h6 class="alert-heading">${titleNorm}</h6>` : '';
    const messageContent = isHtml ? messageNorm : (window.CharsetFix ? window.CharsetFix.safeEscapeHtml(messageNorm) : escapeHtml(messageNorm));
    
    alertContainer.innerHTML = `
        ${titleHtml}
        ${messageContent}
  <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;

    document.body.appendChild(alertContainer);

 // Auto-dismiss después de 5 segundos (excepto para info con HTML)
    if (!(isHtml && type === 'info')) {
      setTimeout(() => {
            if (alertContainer.parentNode) {
       alertContainer.remove();
   }
    }, 5000);
    }
}

function escapeHtml(text) {
    if (typeof text !== 'string') return text;
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// ============================================================
// FUNCIONES DE UTILIDAD
// ============================================================
function formatearFecha(fecha) {
    return new Date(fecha).toLocaleDateString('es-ES', {
        year: 'numeric',
    month: 'long',
  day: 'numeric'
    });
}

function formatearHora(hora) {
 if (!hora) return 'No especificada';
    
try {
  const [hours, minutes] = hora.split(':');
     return `${hours.padStart(2, '0')}:${minutes.padStart(2, '0')}`;
    } catch (e) {
    return hora;
}
}

// ============================================================
// EXPORTAR FUNCIONES GLOBALES
// ============================================================
window.cargarTodasReservas = cargarTodasReservas;
window.aplicarFiltros = aplicarFiltros;
window.verDetalleReserva = verDetalleReserva;