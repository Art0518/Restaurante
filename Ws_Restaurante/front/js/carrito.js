// CARRITO DE RESERVAS - JavaScript con Promociones y Descuentos
// ============================================================
// ACTUALIZADO: Ahora soporta usuarios no logueados usando localStorage

// Microservice bases (production). During local dev a proxy can be used instead.
const BASES = {
 SEGURIDAD: 'https://seguridad-production-279b.up.railway.app',
 MENU: 'https://menu-production-279b.up.railway.app',
 RESERVAS: 'https://reserva-production-279b.up.railway.app',
 FACTURACION: 'https://factura-production-279b.up.railway.app'
};

// Variables globales
let reservasSeleccionadas = [];
let todasReservas = [];
let promocionSeleccionada = null;
let promocionesDisponibles = [];
let facturaActual = null;
let usuario = null;
let esUsuarioLogueado = false; // NUEVO: Para distinguir entre usuarios logueados y no logueados

// ? NUEVAS VARIABLES PARA RESERVAS CONFIRMADAS
let reservasConfirmadasSeleccionadas = [];
let todasReservasConfirmadas = [];

document.addEventListener('DOMContentLoaded', function () {
    // Verificar autenticación (NO obligatoria)
    usuario = JSON.parse(localStorage.getItem('usuario'));
    esUsuarioLogueado = usuario !== null;
    
 if (!esUsuarioLogueado) {
        // Usuario no logueado - mostrar mensaje informativo
        mostrarMensajeUsuarioNoLogueado();
   // Cargar carrito temporal desde localStorage
      cargarCarritoTemporal();
    } else {
        // Usuario logueado - cargar desde el servidor
        cargarPromocionesActivas();
        cargarCarritoUsuario();
        cargarReservasConfirmadas();
    }

 // Event listeners
    setupEventListeners();
});

// ============================================================
// NUEVO: MOSTRAR MENSAJE PARA USUARIOS NO LOGUEADOS
// ============================================================
function mostrarMensajeUsuarioNoLogueado() {
    // Crear un banner informativo
    const banner = document.createElement('div');
    banner.className = 'alert alert-info alert-dismissible fade show mx-auto my-3';
    banner.style.maxWidth = '90%';
    banner.innerHTML = `
   <i class="bi bi-info-circle"></i>
        <strong>¡Bienvenido!</strong> Estás navegando como invitado. 
      <a href="#" class="alert-link" onclick="event.preventDefault(); abrirModalAuth();">Inicia sesión o regístrate</a> 
        para guardar tu carrito y confirmar reservas.
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    const container = document.querySelector('.container.my-5');
 if (container) {
   container.insertBefore(banner, container.firstChild);
    }
    
    // Ocultar secciones que requieren autenticación
    ocultarSeccionesAutenticadas();
}

// ============================================================
// NUEVO: OCULTAR SECCIONES QUE REQUIEREN AUTENTICACIÓN
// ============================================================
function ocultarSeccionesAutenticadas() {
    // Ocultar sección de promociones (requiere usuario logueado)
    const promocionesContainer = document.querySelector('.card.border-success');
    if (promocionesContainer) {
        promocionesContainer.style.display = 'none';
    }
    
    // Ocultar sección de reservas confirmadas
    const seccionConfirmadas = document.querySelectorAll('.card.border-success')[1];
    if (seccionConfirmadas) {
 seccionConfirmadas.style.display = 'none';
    }
    
    // Modificar botones para invitar a login
    const btnConfirmar = document.getElementById('btn-confirmar-carrito');
    if (btnConfirmar) {
        btnConfirmar.innerHTML = '<i class="bi bi-lock"></i> Inicia sesión para confirmar';
        btnConfirmar.onclick = function(e) {
   e.preventDefault();
        showAlert('Debes iniciar sesión para confirmar reservas', 'warning');
    abrirModalAuth();
     };
    }
    
    const btnFactura = document.getElementById('btn-generar-factura');
    if (btnFactura) {
        btnFactura.innerHTML = '<i class="bi bi-lock"></i> Inicia sesión para facturar';
        btnFactura.onclick = function(e) {
            e.preventDefault();
            showAlert('Debes iniciar sesión para generar facturas', 'warning');
       abrirModalAuth();
        };
    }
}

// ============================================================
// NUEVO: ABRIR MODAL DE AUTENTICACIÓN
// ============================================================
function abrirModalAuth() {
    // Guardar que venimos del carrito para redirigir después del login
    sessionStorage.setItem('redirectAfterLogin', 'carrito.html');
    
    // Intentar abrir el modal de autenticación
    const authModal = document.querySelector('#authModal, #auth-modal, [id*="auth"]');
if (authModal) {
        const modal = new bootstrap.Modal(authModal);
        modal.show();
  } else {
   // Si no hay modal, redirigir a index
        window.location.href = 'index.html';
 }
}

// ============================================================
// NUEVO: CARGAR CARRITO TEMPORAL DESDE LOCALSTORAGE
// ============================================================
function cargarCarritoTemporal() {
    const carritoTemp = JSON.parse(localStorage.getItem('carritoTemporal')) || [];
    
    if (carritoTemp.length === 0) {
        mostrarCarritoVacio();
return;
    }
    
    // Mostrar las reservas temporales
    todasReservas = carritoTemp;
    mostrarCarritoTemporal(carritoTemp);
}

// ============================================================
// NUEVO: MOSTRAR CARRITO TEMPORAL
// ============================================================
function mostrarCarritoTemporal(reservas) {
    const carritoItems = document.getElementById('carrito-items');
    const carritoVacio = document.getElementById('carrito-vacio');
    const listaCarrito = document.getElementById('lista-carrito');

    if (!reservas || reservas.length === 0) {
        mostrarCarritoVacio();
        return;
    }

    carritoVacio.style.display = 'none';
    listaCarrito.style.display = 'block';
    actualizarBotones(true);

    carritoItems.innerHTML = '';

    reservas.forEach((reserva, index) => {
      const fila = crearFilaReservaTemporal(reserva, index);
        carritoItems.appendChild(fila);
    });

    document.getElementById('total-reservas').textContent = reservas.length;
    actualizarTotalesSeleccionadas();
}

// ============================================================
// NUEVO: CREAR FILA DE RESERVA TEMPORAL
// ============================================================
function crearFilaReservaTemporal(reserva, index) {
    const fila = document.createElement('tr');

    const fecha = new Date(reserva.Fecha).toLocaleDateString('es-ES');
    const hora = reserva.Hora;
    const precioBase = reserva.PrecioBase || 50.00; // Precio por defecto
    const iva = precioBase * 0.115; // IVA 11.5%
    const totalFinal = precioBase + iva;

    fila.innerHTML = `
        <td>
            <div class="form-check">
     <input class="form-check-input" type="checkbox" value="${index}" 
               id="check_temp_${index}" onchange="actualizarSeleccionTemporal(${index}, this.checked)">
     <label class="form-check-label" for="check_temp_${index}">
       Seleccionar
    </label>
            </div>
        </td>
<td>
<strong>Mesa ${reserva.NumeroMesa || 'No asignada'}</strong>
         <br>
            <small class="text-muted">Capacidad: ${reserva.NumeroPersonas || 'N/A'} personas</small>
        </td>
  <td>${fecha}</td>
        <td>${hora}</td>
        <td>
    <i class="bi bi-people"></i> ${reserva.NumeroPersonas || 'N/A'}
   </td>
        <td>
            <div class="text-end">
     <div><small class="text-muted">Subtotal: $${precioBase.toFixed(2)}</small></div>
            <div><small class="text-muted">IVA (11.5%): $${iva.toFixed(2)}</small></div>
       <div><strong class="text-primary">Total: $${totalFinal.toFixed(2)}</strong></div>
            </div>
        </td>
        <td>
       <button class="btn btn-sm btn-outline-danger" onclick="eliminarDelCarritoTemporal(${index})"
           title="Eliminar del carrito">
    <i class="bi bi-trash"></i> Eliminar
            </button>
      </td>
 `;

    if (reserva.Observaciones && reserva.Observaciones.trim() !== '') {
  const obsRow = document.createElement('tr');
        obsRow.className = 'table-light';
        obsRow.innerHTML = `
  <td colspan="7">
                <small><strong>Observaciones:</strong> ${reserva.Observaciones}</small>
    </td>
      `;
        fila.appendChild(obsRow);
    }

    return fila;
}

// ============================================================
// NUEVO: ACTUALIZAR SELECCIÓN TEMPORAL
// ============================================================
function actualizarSeleccionTemporal(index, seleccionado) {
    if (seleccionado) {
        if (!reservasSeleccionadas.includes(index)) {
       reservasSeleccionadas.push(index);
     }
    } else {
      reservasSeleccionadas = reservasSeleccionadas.filter(i => i !== index);
    }

    actualizarInfoSeleccion();
    actualizarTotalesTemporales();
}

// ============================================================
// NUEVO: ACTUALIZAR TOTALES TEMPORALES
// ============================================================
function actualizarTotalesTemporales() {
    let subtotalSeleccionadas = 0;
    let ivaSeleccionadas = 0;
    let totalSeleccionadas = 0;
  let cantidadSeleccionadas = reservasSeleccionadas.length;

    reservasSeleccionadas.forEach(index => {
   const reserva = todasReservas[index];
if (reserva) {
        const precioBase = reserva.PrecioBase || 50.00;
    const iva = precioBase * 0.115;
     subtotalSeleccionadas += precioBase;
         ivaSeleccionadas += iva;
          totalSeleccionadas += precioBase + iva;
        }
    });

    document.getElementById('total-reservas-seleccionadas').textContent = cantidadSeleccionadas;
    document.getElementById('subtotal-carrito').textContent = '$' + subtotalSeleccionadas.toFixed(2);
    document.getElementById('iva-carrito').textContent = '$' + ivaSeleccionadas.toFixed(2);
    document.getElementById('total-carrito').textContent = '$' + totalSeleccionadas.toFixed(2);

    // Ocultar descuento para usuarios no logueados
    const descuentoRow = document.getElementById('descuento-row');
    const subtotalDescuentoRow = document.getElementById('subtotal-descuento-row');
    if (descuentoRow) descuentoRow.style.display = 'none';
    if (subtotalDescuentoRow) subtotalDescuentoRow.style.display = 'none';
}

// ============================================================
// NUEVO: ELIMINAR DEL CARRITO TEMPORAL
// ============================================================
function eliminarDelCarritoTemporal(index) {
    if (!confirm('¿Estás seguro de que deseas eliminar esta reserva del carrito?')) {
        return;
    }

  let carritoTemp = JSON.parse(localStorage.getItem('carritoTemporal')) || [];
    carritoTemp.splice(index, 1);
    localStorage.setItem('carritoTemporal', JSON.stringify(carritoTemp));

    showAlert('Reserva eliminada del carrito', 'success');
    cargarCarritoTemporal();
}

// ============================================================
// NUEVO: MOSTRAR CARRITO VACÍO
// ============================================================
function mostrarCarritoVacio() {
    const carritoVacio = document.getElementById('carrito-vacio');
    const listaCarrito = document.getElementById('lista-carrito');
    
    if (carritoVacio) carritoVacio.style.display = 'block';
    if (listaCarrito) listaCarrito.style.display = 'none';
    
    actualizarBotones(false);
}

// ============================================================
// ACTUALIZAR BOTONES SEGÚN DISPONIBILIDAD
// ============================================================
function actualizarBotones(habilitar) {
    const btnVaciar = document.getElementById('btn-vaciar-carrito');
    const btnSelTodas = document.getElementById('btn-seleccionar-todas');
    const btnDeselTodas = document.getElementById('btn-deseleccionar-todas');
    
    if (btnVaciar) btnVaciar.disabled = !habilitar;
    if (btnSelTodas) btnSelTodas.disabled = !habilitar;
    if (btnDeselTodas) btnDeselTodas.disabled = !habilitar;
    
    // Los botones de confirmar y factura ya están manejados en ocultarSeccionesAutenticadas
}

// ============================================================
// NUEVAS FUNIONES DE FACTURACI�N
// ============================================================

// ============================================================
// GENERAR FACTURA DESDE CARRITO
// ============================================================
function generarFacturaDesdeCarrito() {
    if (reservasSeleccionadas.length === 0) {
        showAlert('Selecciona al menos una reserva para generar la factura', 'warning');
        return;
    }

    showLoading();

    const requestData = {
        IdUsuario: usuario.IdUsuario,
      ReservasIds: reservasSeleccionadas.join(','),
    PromocionId: promocionSeleccionada,
        MetodoPago: null
    };

  // ? Enviar el monto total a 0 para evitar problemas con descuentos
  // ? Las reservas llegan con sus totales ya calculados
    fetch(`${BASES.FACTURACION}/api/facturas/generar-carrito`, {
        method: 'POST',
        headers: {
       'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestData)
 })
    .then(response => response.json())
    .then(data => {
        hideLoading();
        if (data.success || data.Estado === 'SUCCESS') {
    showAlert('Factura generada exitosamente', 'success');
            facturaActual = data;
          mostrarFacturaGenerada(data);

      setTimeout(() => {
          cargarCarritoUsuario();
     reservasSeleccionadas = [];
        }, 1000);
        } else {
            mostrarError(data.Mensaje || data.message || 'Error al generar la factura');
        }
    })
    .catch(error => {
 hideLoading();
        console.error('Error:', error);
     mostrarError('Error al generar la factura');
   });
}

// ============================================================
// MOSTRAR FACTURA GENERADA
// ============================================================
function mostrarFacturaGenerada(factura) {
    document.getElementById('seccion-factura').style.display = 'block';

    document.getElementById('factura-numero').textContent = factura.IdFactura;
    document.getElementById('factura-fecha').textContent = new Date().toLocaleDateString('es-ES');

 const estadoBadge = document.getElementById('factura-estado');
    estadoBadge.textContent = 'Emitida';
    estadoBadge.className = 'badge bg-warning';

    const metodoPago = factura.MetodoPago && factura.MetodoPago !== 'null' && factura.MetodoPago !== '' ? 
        factura.MetodoPago : 'Todavia no realiza el pago';
    document.getElementById('factura-metodo-pago').textContent = metodoPago;

    document.getElementById('factura-cliente').textContent = usuario?.Nombre || 'Cliente';
    document.getElementById('factura-telefono').textContent = usuario?.Telefono || 'No especificado';
    document.getElementById('factura-email').textContent = usuario?.Email || 'No especificado';

    obtenerDetallesFactura(factura.IdFactura);

    // ⚠️ COMENTADO: El botón btn-marcar-pagada no existe en el DOM actual
    // const btnMarcarPagada = document.getElementById('btn-marcar-pagada');
    // if (metodoPago === 'Todavia no realiza el pago') {
    //   btnMarcarPagada.style.display = 'inline-block';
    // } else {
    //   btnMarcarPagada.style.display = 'none';
    // }

    document.getElementById('seccion-factura').scrollIntoView({ behavior: 'smooth' });
}

// ============================================================
// OBTENER DETALLES DE LA FACTURA
// ============================================================
function obtenerDetallesFactura(idFactura) {
    fetch(`${BASES.FACTURACION}/api/facturas/detallada/${idFactura}`)
 .then(response => response.json())
    .then(data => {
        if (data.success && data.factura && data.detalles) {
    mostrarDetallesFactura(data.factura[0], data.detalles);
    mostrarTotalesFactura(data.factura[0]);
        } else {
      console.error('Error obteniendo detalles de factura');
        }
    })
    .catch(error => {
        console.error('Error:', error);
    });
}

// ============================================================
// MARCAR FACTURA COMO PAGADA
// ============================================================
function marcarFacturaComoPagada() {
    const metodoPago = document.getElementById('metodo-pago-factura').value;

    if (!metodoPago) {
        showAlert('Por favor selecciona un m�todo de pago', 'warning');
        return;
    }

    if (!facturaActual || !facturaActual.IdFactura) {
        showAlert('No hay factura seleccionada', 'warning');
        return;
    }

    showLoading();

    const modal = bootstrap.Modal.getInstance(document.getElementById('modalMarcarPagada'));
    modal.hide();

    const requestData = {
 IdFactura: facturaActual.IdFactura,
        MetodoPago: metodoPago
  };

    fetch(`${BASES.FACTURACION}/api/facturas/marcar-pagada`, {
        method: 'POST',
        headers: {
     'Content-Type': 'application/json'
      },
      body: JSON.stringify(requestData)
    })
    .then(response => response.json())
    .then(data => {
 hideLoading();
        if (data.success || data.Estado === 'SUCCESS') {
     showAlert('Factura marcada como pagada exitosamente', 'success');

            const estadoBadge = document.getElementById('factura-estado');
            estadoBadge.textContent = 'Pagada';
          estadoBadge.className = 'badge bg-success';

            document.getElementById('factura-metodo-pago').textContent = metodoPago;
 document.getElementById('btn-marcar-pagada').style.display = 'none';

 facturaActual.Estado = 'Pagada';
 facturaActual.MetodoPago = metodoPago;

        } else {
      mostrarError(data.Mensaje || data.message || 'Error marcando la factura como pagada');
        }
    })
    .catch(error => {
hideLoading();
        console.error('Error:', error);
        mostrarError('Error al marcar la factura como pagada');
    });
}

// ============================================================
// CARGAR RESERVAS CONFIRMADAS DEL USUARIO
// ============================================================
function cargarReservasConfirmadas() {
    if (!usuario || !usuario.IdUsuario) {
 mostrarReservasConfirmadasVacias();
        return;
    }

    fetch(`${BASES.RESERVAS}/api/reservas/confirmadas/${usuario.IdUsuario}`)
 .then(response => response.json())
    .then(data => {
        if (data.success && data.reservas) {
   mostrarReservasConfirmadas(data.reservas);
 } else {
      mostrarReservasConfirmadasVacias();
  }
})
   .catch(error => {
        console.error('Error cargando reservas confirmadas:', error);
   mostrarReservasConfirmadasVacias();
        });
}

// ============================================================
// MOSTRAR RESERVAS CONFIRMADAS EN LA INTERFAZ
// ============================================================
function mostrarReservasConfirmadas(reservas) {
    const reservasConfirmadasVacio = document.getElementById('reservas-confirmadas-vacio');
    const controlesConfirmadas = document.getElementById('controles-confirmadas');
    const tablaConfirmadas = document.getElementById('tabla-confirmadas');

    todasReservasConfirmadas = reservas || [];

    // Verificar que los elementos existan antes de usarlos
    if (!reservasConfirmadasVacio || !controlesConfirmadas || !tablaConfirmadas) {
        console.error('Error: No se encontraron los elementos necesarios para mostrar reservas confirmadas');
      return;
 }

    if (!reservas || reservas.length === 0) {
        reservasConfirmadasVacio.style.display = 'block';
        controlesConfirmadas.style.display = 'none';
        tablaConfirmadas.style.display = 'none';
        const resumenConfirmadas = document.getElementById('resumen-confirmadas');
        if (resumenConfirmadas) {
   resumenConfirmadas.style.display = 'none';
        }
        return;
    }

    reservasConfirmadasVacio.style.display = 'none';
    controlesConfirmadas.style.display = 'flex';
    tablaConfirmadas.style.display = 'block';

    const reservasConfirmadasItems = document.getElementById('reservas-confirmadas-items');
    if (!reservasConfirmadasItems) {
 console.error('Error: No se encontr� el elemento reservas-confirmadas-items');
  return;
    }

    reservasConfirmadasItems.innerHTML = '';

    reservas.forEach(reserva => {
        const fila = crearFilaReservaConfirmada(reserva);
        reservasConfirmadasItems.appendChild(fila);
    });

    actualizarTotalesReservasConfirmadas();
}

// ============================================================
// CREAR FILA DE RESERVA CONFIRMADA
// ============================================================
function crearFilaReservaConfirmada(reserva) {
    const fila = document.createElement('tr');

  const fecha = new Date(reserva.Fecha).toLocaleDateString('es-ES');
    const hora = reserva.Hora;
    
    // ? El Total ya viene con IVA incluido (15%) desde la base de datos
    const total = reserva.Total || 0;
    
  const metodoPago = reserva.MetodoPago || 'No especificado';

    fila.innerHTML = `
   <td>
            <div class="form-check">
<input class="form-check-input" type="checkbox" value="${reserva.IdReserva}" 
   id="check_confirmada_${reserva.IdReserva}" 
      onchange="actualizarSeleccionConfirmada(${reserva.IdReserva}, this.checked)">
 <label class="form-check-label" for="check_confirmada_${reserva.IdReserva}">
  Seleccionar
        </label>
        </div>
        </td>
        <td>
  <strong>Mesa ${reserva.NumeroMesa}</strong>
            <br>
        <small class="text-muted">Capacidad: ${reserva.NumeroPersonas} personas</small>
        </td>
 <td>${fecha}</td>
  <td>${hora}</td>
   <td>
  <i class="bi bi-people"></i> ${reserva.NumeroPersonas}
        </td>
        <td>
   <span class="badge bg-info">${metodoPago}</span>
      </td>
   <td>
 <strong class="text-success">$${total.toFixed(2)}</strong>
     </td>
        <td>
   <span class="badge bg-success">Confirmada</span>
   </td>
    `;

    if (reserva.Observaciones && reserva.Observaciones.trim() !== '') {
        const obsRow = document.createElement('tr');
        obsRow.className = 'table-light';
        obsRow.innerHTML = `
            <td colspan="8">
  <small><strong>Observaciones:</strong> ${reserva.Observaciones}</small>
     </td>
   `;
        fila.appendChild(obsRow);
    }

    return fila;
}

// ============================================================
// MOSTRAR RESERVAS CONFIRMADAS VAC�AS
// ============================================================
function mostrarReservasConfirmadasVacias() {
    const reservasConfirmadasVacio = document.getElementById('reservas-confirmadas-vacio');
    const controlesConfirmadas = document.getElementById('controles-confirmadas');
    const tablaConfirmadas = document.getElementById('tabla-confirmadas');
    const resumenConfirmadas = document.getElementById('resumen-confirmadas');

    // Mostrar mensaje de vac�o y ocultar otros elementos
    if (reservasConfirmadasVacio) {
   reservasConfirmadasVacio.style.display = 'block';
    }
    
    if (controlesConfirmadas) {
        controlesConfirmadas.style.display = 'none';
    }
    
    if (tablaConfirmadas) {
        tablaConfirmadas.style.display = 'none';
    }

    if (resumenConfirmadas) {
    resumenConfirmadas.style.display = 'none';
    }
}

// ============================================================
// MANEJO DE SELECCIÓN DE RESERVAS
// ============================================================
function actualizarSeleccion(idReserva, seleccionado) {
    if (seleccionado) {
        if (!reservasSeleccionadas.includes(idReserva)) {
    reservasSeleccionadas.push(idReserva);
        }
    } else {
  reservasSeleccionadas = reservasSeleccionadas.filter(id => id !== idReserva);
    }

actualizarInfoSeleccion();
    
    // Actualizar totales según tipo de usuario
    if (esUsuarioLogueado) {
  actualizarTotalesSeleccionadas();
    } else {
     actualizarTotalesTemporales();
  }
}

function seleccionarTodasReservas() {
    if (!esUsuarioLogueado) {
   // Usuario no logueado - seleccionar por índice
   todasReservas.forEach((reserva, index) => {
      const checkbox = document.getElementById(`check_temp_${index}`);
      if (checkbox) {
    checkbox.checked = true;
         if (!reservasSeleccionadas.includes(index)) {
      reservasSeleccionadas.push(index);
      }
 }
   });
   } else {
        // Usuario logueado - seleccionar por ID
        todasReservas.forEach(reserva => {
   const checkbox = document.getElementById(`check_${reserva.IdReserva}`);
 if (checkbox) {
  checkbox.checked = true;
   if (!reservasSeleccionadas.includes(reserva.IdReserva)) {
       reservasSeleccionadas.push(reserva.IdReserva);
    }
     }
  });
  }
  
    actualizarInfoSeleccion();
    
    if (esUsuarioLogueado) {
        actualizarTotalesSeleccionadas();
    } else {
  actualizarTotalesTemporales();
  }
}

function deseleccionarTodasReservas() {
    if (!esUsuarioLogueado) {
        // Usuario no logueado
   todasReservas.forEach((reserva, index) => {
    const checkbox = document.getElementById(`check_temp_${index}`);
     if (checkbox) {
    checkbox.checked = false;
   }
  });
    } else {
// Usuario logueado
  todasReservas.forEach(reserva => {
       const checkbox = document.getElementById(`check_${reserva.IdReserva}`);
    if (checkbox) {
      checkbox.checked = false;
        }
 });
    }
    
    reservasSeleccionadas = [];
 actualizarInfoSeleccion();
    
    if (esUsuarioLogueado) {
        actualizarTotalesSeleccionadas();
    } else {
   actualizarTotalesTemporales();
 }
}

function actualizarInfoSeleccion() {
    const infoSeleccion = document.getElementById('info-seleccion');
    if (infoSeleccion) {
        if (reservasSeleccionadas.length > 0) {
      infoSeleccion.innerHTML = `<span class="badge bg-primary">${reservasSeleccionadas.length} reservas seleccionadas</span>`;
    } else {
   infoSeleccion.innerHTML = '<span class="text-muted">No hay reservas seleccionadas</span>';
        }
    }
}

// ============================================================
// FUNCIONES DE UI
// ============================================================
function mostrarError(mensaje) {
    showAlert(mensaje, 'danger');
}

function showAlert(message, type = 'info') {
 const alertContainer = document.createElement('div');
    alertContainer.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
 alertContainer.style.cssText = 'top: 20px; right: 20px; z-index: 9999; max-width: 400px;';

 alertContainer.innerHTML = `
   ${message}
<button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;

    document.body.appendChild(alertContainer);

setTimeout(() => {
        if (alertContainer.parentNode) {
        alertContainer.remove();
      }
 }, 5000);
}

function showConfirm(mensaje, onConfirm, onCancel) {
    // Usar modal de confirmación existente o crear uno simple
    const modalConfirmacion = document.getElementById('confirmationModal') || 
           document.querySelector('[id*="confirm"]');
    
  if (modalConfirmacion) {
        // Si existe un modal de confirmación, usarlo
  const modalBody = modalConfirmacion.querySelector('.modal-body');
        const btnConfirmar = modalConfirmacion.querySelector('.btn-primary, .btn-success');
        const btnCancelar = modalConfirmacion.querySelector('.btn-secondary');
        
        if (modalBody) modalBody.textContent = mensaje;
        
 if (btnConfirmar) {
            btnConfirmar.onclick = function() {
    const modal = bootstrap.Modal.getInstance(modalConfirmacion);
   if (modal) modal.hide();
      if (onConfirm) onConfirm();
    };
      }
        
        if (btnCancelar) {
   btnCancelar.onclick = function() {
       const modal = bootstrap.Modal.getInstance(modalConfirmacion);
        if (modal) modal.hide();
if (onCancel) onCancel();
   };
        }
  
 const modal = new bootstrap.Modal(modalConfirmacion);
   modal.show();
 } else {
  // Fallback a confirm nativo
 if (confirm(mensaje)) {
      if (onConfirm) onConfirm();
   } else {
            if (onCancel) onCancel();
  }
    }
}

function showLoading() {
    const loadingContainer = document.querySelector('.loading-container');
    if (loadingContainer) {
        loadingContainer.style.display = 'flex';
    loadingContainer.style.opacity = '1';
    }
}

function hideLoading() {
  const loadingContainer = document.querySelector('.loading-container');
    if (loadingContainer) {
        loadingContainer.style.opacity = '0';
      setTimeout(() => {
 if (loadingContainer) {
   loadingContainer.style.display = 'none';
   }
   }, 300);
    }
}

//# sourceMappingURL=carrito.js.map

