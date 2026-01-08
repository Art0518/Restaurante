// CARRITO DE RESERVAS - JavaScript con Promociones y Descuentos
// ============================================================

// Variables globales
let reservasSeleccionadas = [];
let todasReservas = [];
let promocionSeleccionada = null;
let promocionesDisponibles = [];
let facturaActual = null;
let usuario = null;

// ? NUEVAS VARIABLES PARA RESERVAS CONFIRMADAS
let reservasConfirmadasSeleccionadas = [];
let todasReservasConfirmadas = [];

document.addEventListener('DOMContentLoaded', function () {
    // Verificar autenticaci�n
    usuario = JSON.parse(localStorage.getItem('usuario'));
    if (!usuario) {
        alert("Debes iniciar sesi�n para ver tu carrito.");
        window.location.href = 'index.html';
    return;
    }

    // Cargar promociones y carrito
    cargarPromocionesActivas();
    cargarCarritoUsuario();
 cargarReservasConfirmadas(); // ? NUEVA FUNCI�N

    // Event listeners
    setupEventListeners();
});

// ============================================================
// SETUP EVENT LISTENERS
// ============================================================
function setupEventListeners() {
  // Bot�n "Pagadas Ahora" - Pago Bancario
    document.getElementById('btn-confirmar-carrito').addEventListener('click', function() {
if (reservasSeleccionadas.length === 0) {
        showAlert('Selecciona al menos una reserva para pagar', 'warning');
            return;
        }
      
        // Calcular total a pagar
        let totalAPagar = 0;
        reservasSeleccionadas.forEach(idReserva => {
     const reserva = todasReservas.find(r => r.IdReserva == idReserva);
      if (reserva) {
  totalAPagar += reserva.TotalFinal || 0;
  }
        });
        
        // Mostrar modal de pago bancario
        document.getElementById('monto-pago-bancario').textContent = '$' + totalAPagar.toFixed(2);
        document.getElementById('cedula-cliente').value = '';
        document.getElementById('estado-pago-bancario').style.display = 'none';
        const modal = new bootstrap.Modal(document.getElementById('modalPagoBancario'));
        modal.show();
  });

    // Bot�n vaciar carrito
    document.getElementById('btn-vaciar-carrito').addEventListener('click', function() {
  showConfirm('¿Estás seguro de que deseas eliminar TODAS las reservas del carrito? Esta acción no se puede deshacer.', 
  function() {
 // Usuario confirmó - vaciar carrito
       vaciarCarritoCompleto();
   },
   function() {
        // Usuario canceló - no hacer nada
console.log('Vaciado de carrito cancelado');
  }
  );
    });

    // Confirmar pago bancario
    document.getElementById('confirmar-pago-bancario').addEventListener('click', function() {
  procesarPagoBancario();
    });

    // Bot?n seleccionar todas
    document.getElementById('btn-seleccionar-todas').addEventListener('click', function() {
     seleccionarTodasReservas();
  });

    // Bot�n deseleccionar todas
    document.getElementById('btn-deseleccionar-todas').addEventListener('click', function() {
     deseleccionarTodasReservas();
  });

  // NUEVOS EVENT LISTENERS PARA FACTURACI?N
    // Botón generar factura
    document.getElementById('btn-generar-factura').addEventListener('click', function() {
        if (reservasSeleccionadas.length === 0) {
      showAlert('Selecciona al menos una reserva para generar la factura', 'warning');
     return;
        }
        generarFacturaDesdeCarrito();
  });

    // Botón nueva factura
    document.getElementById('btn-nueva-factura').addEventListener('click', function() {
  ocultarSeccionFactura();
    });

    // Botón descargar factura
    document.getElementById('btn-descargar-factura').addEventListener('click', function() {
  descargarFactura();
    });

  // NUEVOS EVENT LISTENERS PARA RESERVAS CONFIRMADAS
    
    // Botón seleccionar todas las confirmadas
    document.getElementById('btn-seleccionar-todas-confirmadas').addEventListener('click', function() {
        seleccionarTodasReservasConfirmadas();
    });

    // Botón deseleccionar todas las confirmadas
    document.getElementById('btn-deseleccionar-todas-confirmadas').addEventListener('click', function() {
        deseleccionarTodasReservasConfirmadas();
    });

    // Botón generar factura de confirmadas
    document.getElementById('btn-generar-factura-confirmadas').addEventListener('click', function() {
   if (reservasConfirmadasSeleccionadas.length === 0) {
            showAlert('Selecciona al menos una reserva confirmada para generar la factura', 'warning');
 return;
        }
        generarFacturaReservasConfirmadas();
    });
}

// ============================================================
// CARGAR PROMOCIONES V�LIDAS PARA LAS FECHAS DEL CARRITO
// ============================================================
function cargarPromocionesActivas() {
    if (!usuario || !usuario.IdUsuario) {
        mostrarPromocionesSinDisponibles();
        return;
    }

    fetch(`/api/carrito/promociones-validas/${usuario.IdUsuario}`)
        .then(response => response.json())
  .then(data => {
        if (data.success) {
promocionesDisponibles = data.promociones || [];
        mostrarPromociones(promocionesDisponibles);
 } else {
     mostrarPromocionesSinDisponibles();
      }
        })
   .catch(error => {
            console.error('Error cargando promociones:', error);
   mostrarPromocionesSinDisponibles();
        });
}

// ============================================================
// MOSTRAR PROMOCIONES DISPONIBLES
// ============================================================
function mostrarPromociones(promociones) {
    const container = document.getElementById('promociones-container');

    if (!promociones || promociones.length === 0) {
   mostrarPromocionesSinDisponibles();
        return;
    }

    container.innerHTML = '';

    if (promociones.length === 1) {
        const promocion = promociones[0];
        promocionSeleccionada = promocion.IdPromocion;
      container.innerHTML = `
   <div class="alert alert-success mb-0">
      <i class="bi bi-gift"></i>
    <strong>Promoción activa:</strong> ${promocion.Descuento}% de descuento
      <small class="text-muted d-block">Se aplicará automáticamente</small>
    </div>
    `;
    } else {
      container.innerHTML = `
    <p class="mb-2"><i class="bi bi-gift"></i> Selecciona una promoción:</p>
      <div class="row g-2" id="promociones-opciones">
  </div>
    `;

    const opcionesContainer = document.getElementById('promociones-opciones');
     promociones.forEach(promocion => {
   opcionesContainer.innerHTML += `
             <div class="col-md-4">
     <div class="card promocion-card" onclick="seleccionarPromocion(${promocion.IdPromocion}, '${promocion.Descuento}')">
     <div class="card-body text-center p-2">
      <h5 class="card-title text-success mb-1">${promocion.Descuento}%</h5>
    <small class="text-muted">Descuento</small>
        </div>
         </div>
            </div>
       `;
      });
    }

    if (promocionSeleccionada) {
     cargarCarritoUsuarioManteniendoSelecciones();
  }
}

function mostrarPromocionesSinDisponibles() {
    const container = document.getElementById('promociones-container');
    container.innerHTML = `
        <div class="text-center text-muted">
    <i class="bi bi-info-circle"></i>
            No hay promociones disponibles en este momento
        </div>
  `;
}

// ============================================================
// SELECCIONAR PROMOCI�N (MANTENER SELECCIONES)
// ============================================================

function seleccionarPromocion(idPromocion, porcentaje) {
    promocionSeleccionada = idPromocion;

    document.querySelectorAll('.promocion-card').forEach(card => {
        card.classList.remove('border-success', 'bg-light');
    });

    event.currentTarget.classList.add('border-success', 'bg-light');
    showAlert(`Promoci�n de ${porcentaje}% seleccionada`, 'success');
    cargarCarritoUsuarioManteniendoSelecciones();
}

// ============================================================
// CARGAR CARRITO MANTENIENDO SELECCIONES ACTUALES
// ============================================================
function cargarCarritoUsuarioManteniendoSelecciones() {
    if (!usuario || !usuario.IdUsuario) {
  mostrarError('No se pudo identificar al usuario');
        return;
    }

    const seleccionesAnteriores = [...reservasSeleccionadas];
    showLoading();

    let url = `/api/carrito/usuario/${usuario.IdUsuario}`;
    if (promocionSeleccionada) {
   url += `?promocionId=${promocionSeleccionada}`;
 }

    fetch(url)
   .then(response => response.json())
        .then(data => {
hideLoading();
 if (data.success) {
     mostrarCarrito(data.reservas, data.resumen);
           setTimeout(() => {
        restaurarSeccionesAnteriores(seleccionesAnteriores);
       }, 100);
         } else {
          mostrarError(data.message || 'Error al cargar el carrito');
     }
        })
        .catch(error => {
    hideLoading();
      console.error('Error:', error);
            mostrarError('Error al conectar con el servidor');
      });
}

// ============================================================
// CARGAR CARRITO DEL USUARIO (PRIMERA VEZ)
// ============================================================
function cargarCarritoUsuario() {
    if (!usuario || !usuario.IdUsuario) {
        mostrarError('No se pudo identificar al usuario');
   return;
    }

    showLoading();

    let url = `/api/carrito/usuario/${usuario.IdUsuario}`;
    if (promocionSeleccionada) {
        url += `?promocionId=${promocionSeleccionada}`;
    }

    fetch(url)
        .then(response => response.json())
        .then(data => {
            hideLoading();
            if (data.success) {
  mostrarCarrito(data.reservas, data.resumen);
       } else {
      mostrarError(data.message || 'Error al cargar el carrito');
}
        })
     .catch(error => {
          hideLoading();
console.error('Error:', error);
    mostrarError('Error al conectar con el servidor');
     });
}

// ============================================================
// RESTAURAR SELECCIONES ANTERIORES
// ============================================================
function restaurarSeccionesAnteriores(seleccionesAnteriores) {
    seleccionesAnteriores.forEach(idReserva => {
     const checkbox = document.getElementById(`check_${idReserva}`);
        if (checkbox) {
        checkbox.checked = true;
            if (!reservasSeleccionadas.includes(idReserva)) {
     reservasSeleccionadas.push(idReserva);
  }
        }
    });

    updateCheckboxStatus(); // ? Actualizar estado de los checkboxes
    actualizarInfoSeleccion();
    actualizarTotalesSeleccionadas();
}

function updateCheckboxStatus() {
    const tbody = document.querySelector('#carrito-items tbody');
    if (!tbody) return;

    const filas = tbody.querySelectorAll('tr');
    filas.forEach(fila => {
        const checkbox = fila.querySelector('input[type="checkbox"]');
        const idReserva = checkbox ? checkbox.value : null;

        if (checkbox && reservasSeleccionadas.includes(idReserva)) {
            checkbox.checked = true;
        } else {
            checkbox.checked = false;
        }
    });
}

// ============================================================
// MOSTRAR CARRITO EN LA INTERFAZ CON PROMOCIONES
// ============================================================
function mostrarCarrito(reservas, resumen) {
    const carritoItems = document.getElementById('carrito-items');
    const carritoVacio = document.getElementById('carrito-vacio');
    const listaCarrito = document.getElementById('lista-carrito');

    todasReservas = reservas || [];

    if (!reservas || reservas.length === 0) {
        carritoVacio.style.display = 'block';
        listaCarrito.style.display = 'none';
        actualizarBotones(false);
        return;
    }

  carritoVacio.style.display = 'none';
    listaCarrito.style.display = 'block';
    actualizarBotones(true);

    carritoItems.innerHTML = '';

    reservas.forEach(reserva => {
        const fila = crearFilaReserva(reserva);
        carritoItems.appendChild(fila);
    });

    if (resumen && resumen.length > 0) {
     document.getElementById('total-reservas').textContent = resumen[0].TotalReservas || 0;
    }

    actualizarTotalesSeleccionadas();
}

// ============================================================
// CREAR FILA DE RESERVA CON PRECIOS CON PROMOCI�N
// ============================================================
function crearFilaReserva(reserva) {
    const fila = document.createElement('tr');

    const fecha = new Date(reserva.Fecha).toLocaleDateString('es-ES');
    const hora = reserva.Hora;

    const subtotal = reserva.Subtotal || 0;
    const descuento = reserva.MontoDescuentoCalculado || 0;
    const subtotalConDescuento = reserva.SubtotalConDescuento || subtotal;
    const iva = reserva.IVA || 0;
    const totalFinal = reserva.TotalFinal || 0;

    fila.innerHTML = `
        <td>
         <div class="form-check">
  <input class="form-check-input" type="checkbox" value="${reserva.IdReserva}" 
     id="check_${reserva.IdReserva}" onchange="actualizarSeleccion(${reserva.IdReserva}, this.checked)">
       <label class="form-check-label" for="check_${reserva.IdReserva}">
  Seleccionar
         </label>
      </div>
    </td>
        <td>
<strong>Mesa ${reserva.NumeroMesa}</strong>
       <br>
          <small class="text-muted">Capacidad: ${reserva.CapacidadMesa} personas</small>
        </td>
        <td>${fecha}</td>
        <td>${hora}</td>
        <td>
            <i class="bi bi-people"></i> ${reserva.NumeroPersonas}
        </td>
        <td>
   <div class="text-end">
      <div><small class="text-muted">Subtotal: $${subtotal.toFixed(2)}</small></div>
    ${descuento > 0 ? `<div><small class="text-success">Descuento: -$${descuento.toFixed(2)}</small></div>` : ''}

     <div><small class="text-muted">IVA (7%): $${iva.toFixed(2)}</small></div>
     <div><strong class="text-success">Total: $${totalFinal.toFixed(2)}</strong></div>
       </div>
   </td>
        <td>
         <button class="btn btn-sm btn-outline-danger" onclick="eliminarDelCarrito(${reserva.IdReserva})"
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
// MANEJO DE SELECCI�N DE RESERVAS
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
    actualizarTotalesSeleccionadas();
}

function seleccionarTodasReservas() {
    todasReservas.forEach(reserva => {
        const checkbox = document.getElementById(`check_${reserva.IdReserva}`);
        if (checkbox) {
    checkbox.checked = true;
            if (!reservasSeleccionadas.includes(reserva.IdReserva)) {
     reservasSeleccionadas.push(reserva.IdReserva);
    }
        }
    });
    actualizarInfoSeleccion();
    actualizarTotalesSeleccionadas();
}

function deseleccionarTodasReservas() {
    todasReservas.forEach(reserva => {
        const checkbox = document.getElementById(`check_${reserva.IdReserva}`);
        if (checkbox) {
      checkbox.checked = false;
        }
    });
    reservasSeleccionadas = [];
    actualizarInfoSeleccion();
    actualizarTotalesSeleccionadas();
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
// CALCULAR TOTALES CON PROMOCIONES
// ============================================================
function actualizarTotalesSeleccionadas() {
    let subtotalSeleccionadas = 0;
    let descuentoSeleccionadas = 0;
    let ivaSeleccionadas = 0;
    let totalSeleccionadas = 0;
    let cantidadSeleccionadas = reservasSeleccionadas.length;

    reservasSeleccionadas.forEach(idReserva => {
   const reserva = todasReservas.find(r => r.IdReserva == idReserva);
        if (reserva) {
    subtotalSeleccionadas += reserva.Subtotal || 0;
            descuentoSeleccionadas += reserva.MontoDescuentoCalculado || 0;
          ivaSeleccionadas += reserva.IVA || 0;
            totalSeleccionadas += reserva.TotalFinal || 0;
        }
    });

    document.getElementById('total-reservas-seleccionadas').textContent = cantidadSeleccionadas;
    document.getElementById('subtotal-carrito').textContent = '$' + subtotalSeleccionadas.toFixed(2);
    document.getElementById('iva-carrito').textContent = '$' + ivaSeleccionadas.toFixed(2);
    document.getElementById('total-carrito').textContent = '$' + totalSeleccionadas.toFixed(2);

    const descuentoRow = document.getElementById('descuento-row');
    const subtotalDescuentoRow = document.getElementById('subtotal-descuento-row');

    if (descuentoSeleccionadas > 0) {
        descuentoRow.style.display = 'flex';
   subtotalDescuentoRow.style.display = 'flex';

 let porcentajeDescuento = 0;
        if (promocionSeleccionada && promocionesDisponibles.length > 0) {
const promocion = promocionesDisponibles.find(p => p.IdPromocion == promocionSeleccionada);
            if (promocion) porcentajeDescuento = promocion.Descuento;
        }

    document.getElementById('porcentaje-descuento').textContent = porcentajeDescuento;
   document.getElementById('monto-descuento').textContent = '-$' + descuentoSeleccionadas.toFixed(2);
        document.getElementById('subtotal-con-descuento').textContent = '$' + (subtotalSeleccionadas - descuentoSeleccionadas).toFixed(2);
    } else {
        descuentoRow.style.display = 'none';
    subtotalDescuentoRow.style.display = 'none';
    }

    const btnConfirmar = document.getElementById('btn-confirmar-carrito');
    btnConfirmar.disabled = cantidadSeleccionadas === 0;

    const btnFactura = document.getElementById('btn-generar-factura');
    btnFactura.disabled = cantidadSeleccionadas === 0;

    if (cantidadSeleccionadas > 0) {
        btnConfirmar.innerHTML = `<i class="bi bi-check-circle"></i> Confirmar ${cantidadSeleccionadas} Reserva${cantidadSeleccionadas > 1 ? 's' : ''}`;
        btnFactura.innerHTML = `<i class="bi bi-receipt"></i> Generar Factura (${cantidadSeleccionadas})`;
    } else {
        btnConfirmar.innerHTML = '<i class="bi bi-check-circle"></i> Confirmar Seleccionadas';
      btnFactura.innerHTML = '<i class="bi bi-receipt"></i> Generar Factura';
    }
}

// ============================================================
// ACTUALIZAR INFO PROMOCI�N EN MODAL
// ============================================================
function actualizarInfoPromocionModal() {
    const promocionInfo = document.getElementById('promocion-aplicada-info');

    if (promocionSeleccionada && promocionesDisponibles.length > 0) {
        const promocion = promocionesDisponibles.find(p => p.IdPromocion == promocionSeleccionada);
        if (promocion) {
     promocionInfo.style.display = 'block';
        document.getElementById('promocion-nombre').textContent = promocion.Nombre || 'Promoci�n especial';
  document.getElementById('promocion-porcentaje').textContent = promocion.Descuento;
        }
    } else {
        promocionInfo.style.display = 'none';
    }
}

// ============================================================
// ELIMINAR RESERVA DEL CARRITO
// ============================================================
function eliminarDelCarrito(idReserva) {
    if (!confirm('�Est�s seguro de que deseas eliminar esta reserva definitivamente del carrito?')) {
        return;
    }

    showLoading();

    fetch(`/api/carrito/eliminar/${usuario.IdUsuario}/${idReserva}`, {
        method: 'DELETE'
  })
    .then(response => response.json())
    .then(data => {
        hideLoading();
      if (data.success) {
            showAlert('Reserva eliminada exitosamente', 'success');
            cargarCarritoUsuario();
   reservasSeleccionadas = reservasSeleccionadas.filter(id => id !== idReserva);
    } else {
    mostrarError(data.message || 'Error eliminando reserva');
 }
    })
  .catch(error => {
        hideLoading();
 console.error('Error:', error);
  mostrarError('Error al eliminar reserva');
    });
}

// ============================================================
// VACIAR CARrito COMPLETO
// ============================================================
function vaciarCarritoCompleto() {
 if (todasReservas.length === 0) return;

    showLoading();

    let eliminacionesCompletas = 0;
    const totalEliminaciones = todasReservas.length;

    todasReservas.forEach(reserva => {
    fetch(`/api/carrito/eliminar/${usuario.IdUsuario}/${reserva.IdReserva}`, {
    method: 'DELETE'
 })
        .then(response => response.json())
        .then(data => {
            eliminacionesCompletas++;
            if (eliminacionesCompletas === totalEliminaciones) {
    hideLoading();
           showAlert('Carrito vaciado exitosamente', 'success');
      cargarCarritoUsuario();
     reservasSeleccionadas = [];
         }
  })
        .catch(error => {
       console.error('Error eliminando reserva:', error);
            eliminacionesCompletas++;
            if (eliminacionesCompletas === totalEliminaciones) {
           hideLoading();
  showAlert('Algunas reservas no se pudieron eliminar', 'warning');
       cargarCarritoUsuario();
            }
    });
    });
}

// ============================================================
// CONFIRMAR RESERVAS SELECCIONADAS
// ============================================================
function confirmarReservasSeleccionadas() {
    const metodoPago = document.getElementById('metodo-pago').value;

    if (!metodoPago) {
        showAlert('Por favor selecciona un m�todo de pago', 'warning');
        return;
    }

    if (reservasSeleccionadas.length === 0) {
        showAlert('No hay reservas seleccionadas para confirmar', 'warning');
        return;
    }

    showLoading();

    const modal = bootstrap.Modal.getInstance(document.getElementById('modalConfirmar'));
    modal.hide();

    const requestData = {
        IdUsuario: usuario.IdUsuario,
  ReservasIds: reservasSeleccionadas.join(','),
   PromocionId: promocionSeleccionada,
  MetodoPago: metodoPago
    };

    fetch('/api/carrito/confirmar', {
  method: 'POST',
 headers: {
      'Content-Type': 'application/json'
},
        body: JSON.stringify(requestData)
    })
    .then(response => response.json())
    .then(data => {
      hideLoading();
      if (data.success) {
// Verificar si se actualiz� una factura existente
        if (data.idFacturaAfectada || data.IdFacturaAfectada) {
         const idFactura = data.idFacturaAfectada || data.IdFacturaAfectada;
       showAlert(`Reservas confirmadas y factura #${idFactura} marcada como pagada exitosamente`, 'success');
 
 // Si hay una factura actual mostr�ndose, actualizar su estado
     if (facturaActual && facturaActual.IdFactura === idFactura) {
       const estadoBadge = document.getElementById('factura-estado');
     if (estadoBadge) {
            estadoBadge.textContent = 'Pagada';
    estadoBadge.className = 'badge bg-success';
    }
       
          // Actualizar m�todo de pago en la factura mostrada
const metodoPagoElement = document.getElementById('factura-metodo-pago');
if (metodoPagoElement) {
       metodoPagoElement.textContent = metodoPago;
    }
   
 // Ocultar bot�n de marcar como pagada
           const btnMarcarPagada = document.getElementById('btn-marcar-pagada');
    if (btnMarcarPagada) {
         btnMarcarPagada.style.display = 'none';
       }
   
       facturaActual.Estado = 'Pagada';
          facturaActual.MetodoPago = metodoPago;
  }
        } else {
  showAlert('Reservas confirmadas exitosamente', 'success');
 }
     
         cargarCarritoUsuario();
    reservasSeleccionadas = [];
     } else {
        mostrarError(data.message || 'Error confirmando las reservas');
}
    })
    .catch(error => {
        hideLoading();
console.error('Error:', error);
      mostrarError('Error al confirmar las reservas');
    });
}

// ============================================================
// ACTUALIZAR BOTONES
// ============================================================
function actualizarBotones(habilitar) {
  document.getElementById('btn-confirmar-carrito').disabled = !habilitar;
    document.getElementById('btn-generar-factura').disabled = !habilitar;
    document.getElementById('btn-vaciar-carrito').disabled = !habilitar;
    document.getElementById('btn-seleccionar-todas').disabled = !habilitar;
    document.getElementById('btn-deseleccionar-todas').disabled = !habilitar;
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
    fetch('/api/facturas/generar-carrito', {
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
    fetch(`/api/facturas/detallada/${idFactura}`)
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
// MOSTRAR DETALLES DE LA FACTURA
// ============================================================
function mostrarDetallesFactura(factura, detalles) {
  const tbody = document.getElementById('factura-detalles');
    tbody.innerHTML = '';

    detalles.forEach(detalle => {
     // Validar valores antes de usar toFixed
 const cantidad = detalle.Cantidad || 0;
        const precioUnitario = detalle.PrecioUnitario || 0;
        const subtotal = detalle.Subtotal || 0;
        const descripcion = detalle.Descripcion || 'Sin descripción';

      const fila = document.createElement('tr');
        fila.innerHTML = `
  <td>${descripcion}</td>
            <td class="text-center">${cantidad}</td>
            <td class="text-end">$${precioUnitario.toFixed(2)}</td>
   <td class="text-end">$${subtotal.toFixed(2)}</td>
        `;
        tbody.appendChild(fila);
  });
}

// ============================================================
// MOSTRAR TOTALES DE LA FACTURA
// ============================================================
function mostrarTotalesFactura(factura) {
    // Validar y asegurar valores num�ricos
    const subtotal = factura.Subtotal || 0;
    const iva = factura.IVA || 0;
    const total = factura.Total || 0;
    const descuento = factura.Descuento || 0;

    document.getElementById('factura-subtotal').textContent = '$' + subtotal.toFixed(2);
    document.getElementById('factura-iva').textContent = '$' + iva.toFixed(2);
    document.getElementById('factura-total').textContent = '$' + total.toFixed(2);

  const descuentoRow = document.getElementById('factura-descuento-row');
    if (descuento && descuento > 0) {
        descuentoRow.style.display = 'table-row';
        document.getElementById('factura-descuento').textContent = '-$' + descuento.toFixed(2);
    } else {
        descuentoRow.style.display = 'none';
    }
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

    fetch('/api/facturas/marcar-pagada', {
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
// OCULTAR SECCI�N DE FACTURA
// ============================================================
function ocultarSeccionFactura() {
    document.getElementById('seccion-factura').style.display = 'none';
  facturaActual = null;

    document.getElementById('factura-numero').textContent = '';
    document.getElementById('factura-fecha').textContent = '';
    document.getElementById('factura-estado').textContent = '';
    document.getElementById('factura-metodo-pago').textContent = '';
    document.getElementById('factura-cliente').textContent = '';
    document.getElementById('factura-telefono').textContent = '';
    document.getElementById('factura-email').textContent = '';
    document.getElementById('factura-detalles').innerHTML = '';
    document.getElementById('factura-subtotal').textContent = '';
    document.getElementById('factura-iva').textContent = '';
    document.getElementById('factura-total').textContent = '';
}

// ============================================================
// DESCARGAR FACTURA
// ============================================================
function descargarFactura() {
    if (!facturaActual || !facturaActual.IdFactura) {
        showAlert('No hay factura para descargar', 'warning');
        return;
    }

    const ventanaImpresion = window.open('', '_blank', 'width=800,height=600');

    const contenidoFactura = `
    <!DOCTYPE html>
    <html lang="es">
  <head>
  <meta charset="UTF-8">
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
        <title>Factura #${facturaActual.IdFactura} - Caf&eacute; San Juan</title>
        <style>
         body { font-family: Arial, sans-serif; margin: 20px; color: #333; }
     .header { text-align: center; border-bottom: 3px solid #5a3e2b; padding-bottom: 20px; margin-bottom: 30px; }
       .logo { max-width: 120px; height: auto; margin-bottom: 15px; }
    .empresa-nombre { font-size: 28px; color: #5a3e2b; font-weight: bold; margin: 10px 0; }
      .factura-titulo { font-size: 20px; color: #666; margin: 5px 0; }
 .fecha { color: #888; font-size: 14px; }
       .info-section { margin: 20px 0; display: flex; justify-content: space-between; }
            .info-box { flex: 1; padding: 15px; background-color: #f9f9f9; border-radius: 5px; margin: 0 10px; }
            .info-box h3 { color: #5a3e2b; border-bottom: 2px solid #5a3e2b; padding-bottom: 8px; margin-top: 0; }
            .info-box p { margin: 8px 0; line-height: 1.6; }
     .table { width: 100%; border-collapse: collapse; margin: 20px 0; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
            .table th, .table td { border: 1px solid #ddd; padding: 12px; text-align: left; }
            .table th { background-color: #5a3e2b; color: white; font-weight: bold; }
   .table tbody tr:nth-child(even) { background-color: #f9f9f9; }
       .table tbody tr:hover { background-color: #f0f0f0; }
     .totals { text-align: right; margin-top: 30px; padding: 20px; background-color: #f9f9f9; border-radius: 5px; }
  .total-line { margin: 8px 0; font-size: 16px; }
            .final-total { font-weight: bold; font-size: 1.4em; color: #5a3e2b; margin-top: 15px; padding-top: 15px; border-top: 2px solid #5a3e2b; }
       .footer { text-align: center; margin-top: 40px; padding-top: 20px; border-top: 1px solid #ddd; color: #888; font-size: 12px; }
         @media print { body { margin: 0; } .no-print { display: none; } }
        </style>
    </head>
    <body>
      <div class="header">
  <img src="img/logo-rincon.png" alt="Caf&eacute; San Juan Logo" class="logo">
 <div class="empresa-nombre">Caf&eacute; San Juan</div>
      <div class="factura-titulo">FACTURA #${facturaActual.IdFactura}</div>
            <div class="fecha">Fecha: ${new Date().toLocaleDateString('es-ES', { day: '2-digit', month: '2-digit', year: 'numeric' })}</div>
        </div>
        
        <div class="info-section">
          <div class="info-box">
     <h3>Cliente</h3>
          <p><strong>Nombre:</strong> ${usuario?.Nombre || 'Cliente'}</p>
     <p><strong>Tel&eacute;fono:</strong> ${usuario?.Telefono || 'No especificado'}</p>
    <p><strong>Email:</strong> ${usuario?.Email || 'No especificado'}</p>
            </div>
    <div class="info-box">
      <h3>Factura</h3>
          <p><strong>N&uacute;mero:</strong> ${facturaActual.IdFactura}</p>
    <p><strong>Estado:</strong> ${document.getElementById('factura-estado').textContent}</p>
              <p><strong>M&eacute;todo de Pago:</strong> ${document.getElementById('factura-metodo-pago').textContent}</p>
   </div>
        </div>
        
<table class="table">
            <thead>
    <tr>
        <th>Descripci&oacute;n</th>
       <th style="width: 100px; text-align: center;">Cantidad</th>
  <th style="width: 120px; text-align: right;">Precio Unit.</th>
         <th style="width: 120px; text-align: right;">Subtotal</th>
  </tr>
            </thead>
    <tbody>
  ${Array.from(document.getElementById('factura-detalles').rows).map(row =>
          `<tr>${Array.from(row.cells).map((cell, index) => {
          const align = index === 0 ? 'left' : index === 1 ? 'center' : 'right';
   return `<td style="text-align: ${align};">${cell.textContent}</td>`;
         }).join('')}</tr>`
              ).join('')}
    </tbody>
        </table>
        
        <div class="totals">
       <div class="total-line">Subtotal: ${document.getElementById('factura-subtotal').textContent}</div>
         ${document.getElementById('factura-descuento-row').style.display !== 'none' ? 
       `<div class="total-line" style="color: #28a745;">Descuento: ${document.getElementById('factura-descuento').textContent}</div>` : ''}
    <div class="total-line">IVA (11.5%): ${document.getElementById('factura-iva').textContent}</div>
  <div class="final-total">Total: ${document.getElementById('factura-total').textContent}</div>
   </div>
        
        <div class="footer">
  <p><strong>Caf&eacute; San Juan</strong> - Sabores del Caribe y tradici&oacute;n puertorrique&ntilde;a</p>
 <p>Gracias por su preferencia</p>
        </div>
        
        <script>
window.onload = function() { 
       setTimeout(function() { window.print(); }, 500);
            }
        </script>
    </body>
    </html>
    `;

    ventanaImpresion.document.write(contenidoFactura);
    ventanaImpresion.document.close();
}

// ============================================================
// ? NUEVAS FUNCIONES PARA RESERVAS CONFIRMADAS
// ============================================================

// ============================================================
// CARGAR RESERVAS CONFIRMADAS DEL USUARIO
// ============================================================
function cargarReservasConfirmadas() {
    if (!usuario || !usuario.IdUsuario) {
 mostrarReservasConfirmadasVacias();
        return;
    }

    fetch(`/api/reservas/confirmadas/${usuario.IdUsuario}`)
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
// MANEJO DE SELECCI�N DE RESERVAS CONFIRMADAS
// ============================================================
function actualizarSeleccionConfirmada(idReserva, seleccionado) {
    if (seleccionado) {
  if (!reservasConfirmadasSeleccionadas.includes(idReserva)) {
    reservasConfirmadasSeleccionadas.push(idReserva);
        }
    } else {
        reservasConfirmadasSeleccionadas = reservasConfirmadasSeleccionadas.filter(id => id !== idReserva);
    }

    actualizarInfoSeleccionConfirmadas();
    actualizarTotalesReservasConfirmadas();
}

function seleccionarTodasReservasConfirmadas() {
    todasReservasConfirmadas.forEach(reserva => {
    const checkbox = document.getElementById(`check_confirmada_${reserva.IdReserva}`);
        if (checkbox) {
   checkbox.checked = true;
      if (!reservasConfirmadasSeleccionadas.includes(reserva.IdReserva)) {
       reservasConfirmadasSeleccionadas.push(reserva.IdReserva);
     }
     }
   });
    actualizarInfoSeleccionConfirmadas();
    actualizarTotalesReservasConfirmadas();
}

function deseleccionarTodasReservasConfirmadas() {
    todasReservasConfirmadas.forEach(reserva => {
   const checkbox = document.getElementById(`check_confirmada_${reserva.IdReserva}`);
        if (checkbox) {
  checkbox.checked = false;
        }
    });
    reservasConfirmadasSeleccionadas = [];
    actualizarInfoSeleccionConfirmadas();
    actualizarTotalesReservasConfirmadas();
}

function actualizarInfoSeleccionConfirmadas() {
 const infoSeleccion = document.getElementById('info-seleccion-confirmadas');
    if (infoSeleccion) {
   if (reservasConfirmadasSeleccionadas.length > 0) {
            infoSeleccion.innerHTML = `<span class="badge bg-success">${reservasConfirmadasSeleccionadas.length} reservas confirmadas seleccionadas</span>`;
        } else {
infoSeleccion.innerHTML = '<span class="text-muted">No hay reservas confirmadas seleccionadas</span>';
        }
    }
}

// ============================================================
// CALCULAR TOTALES DE RESERVAS CONFIRMADAS
// ============================================================
function actualizarTotalesReservasConfirmadas() {
    let totalConfirmadas = 0;
    let cantidadConfirmadas = reservasConfirmadasSeleccionadas.length;

    reservasConfirmadasSeleccionadas.forEach(idReserva => {
        const reserva = todasReservasConfirmadas.find(r => r.IdReserva == idReserva);
        if (reserva) {
            // El Total ya incluye IVA 11.5%
       totalConfirmadas += reserva.Total || 0;
        }
    });

    // Calcular subtotal e IVA desde el total (que ya incluye IVA 11.5%)
    const subtotalConfirmadas = totalConfirmadas / 1.115;
    const ivaConfirmadas = totalConfirmadas - subtotalConfirmadas;

    document.getElementById('total-confirmadas-seleccionadas').textContent = cantidadConfirmadas;
    document.getElementById('subtotal-confirmadas').textContent = '$' + subtotalConfirmadas.toFixed(2);
    document.getElementById('iva-confirmadas').textContent = '$' + ivaConfirmadas.toFixed(2);
 document.getElementById('total-confirmadas').textContent = '$' + totalConfirmadas.toFixed(2);

    const btnGenerarFactura = document.getElementById('btn-generar-factura-confirmadas');
    btnGenerarFactura.disabled = cantidadConfirmadas === 0;

    // Mostrar/ocultar resumen
  const resumenConfirmadas = document.getElementById('resumen-confirmadas');
    if (cantidadConfirmadas > 0) {
        resumenConfirmadas.style.display = 'block';
      btnGenerarFactura.innerHTML = `<i class="bi bi-receipt"></i> Generar Factura (${cantidadConfirmadas} confirmada${cantidadConfirmadas > 1 ? 's' : ''})`;
    } else {
     resumenConfirmadas.style.display = 'none';
btnGenerarFactura.innerHTML = '<i class="bi bi-receipt"></i> Generar Factura de Confirmadas';
    }
}

// ============================================================
// GENERAR FACTURA DE RESERVAS CONFIRMADAS
// ============================================================
function generarFacturaReservasConfirmadas() {
    if (reservasConfirmadasSeleccionadas.length === 0) {
 showAlert('Selecciona al menos una reserva confirmada para generar la factura', 'warning');
   return;
    }

    showLoading();

    const requestData = {
      IdUsuario: usuario.IdUsuario,
      ReservasIds: reservasConfirmadasSeleccionadas.join(','),
        PromocionId: null, // Las reservas confirmadas ya no aplican promociones adicionales
        MetodoPago: null, // Se obtendr� de las reservas confirmadas
  TipoFactura: 'CONFIRMADA' // ? NUEVO: Indicar que es para reservas confirmadas
    };

    fetch('/api/facturas/generar-confirmadas', {
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
   showAlert('Factura de reservas confirmadas generada exitosamente', 'success');
          facturaActual = data;
       mostrarFacturaGeneradaConfirmada(data);

      setTimeout(() => {
cargarReservasConfirmadas();
     reservasConfirmadasSeleccionadas = [];
  }, 1000);
   } else {
            mostrarError(data.Mensaje || data.message || 'Error al generar la factura de confirmadas');
        }
    })
    .catch(error => {
 hideLoading();
        console.error('Error:', error);
     mostrarError('Error al generar la factura de confirmadas');
   });
}

// ============================================================
// MOSTRAR FACTURA GENERADA DE CONFIRMADAS
// ============================================================
function mostrarFacturaGeneradaConfirmada(factura) {
 document.getElementById('seccion-factura').style.display = 'block';

    document.getElementById('factura-numero').textContent = factura.IdFactura;
    document.getElementById('factura-fecha').textContent = new Date().toLocaleDateString('es-ES');

 // ? DIFERENCIA: Estado "Confirmada" en lugar de "Emitida"
 const estadoBadge = document.getElementById('factura-estado');
    estadoBadge.textContent = 'Confirmada';
    estadoBadge.className = 'badge bg-success';

    // ? Obtener m�todo de pago de las reservas confirmadas
  let metodoPago = 'M�todo combinado';
 const metodosUnicos = [...new Set(reservasConfirmadasSeleccionadas.map(id => {
  const reserva = todasReservasConfirmadas.find(r => r.IdReserva == id);
return reserva?.MetodoPago || 'No especificado';
    }))];
    
    if (metodosUnicos.length === 1) {
   metodoPago = metodosUnicos[0];
    } else if (metodosUnicos.length > 1) {
        metodoPago = `M�ltiples (${metodosUnicos.join(', ')})`;
    }

    document.getElementById('factura-metodo-pago').textContent = metodoPago;
    document.getElementById('factura-cliente').textContent = usuario?.Nombre || 'Cliente';
    document.getElementById('factura-telefono').textContent = usuario?.Telefono || 'No especificado';
    document.getElementById('factura-email').textContent = usuario?.Email || 'No especificado';

  obtenerDetallesFactura(factura.IdFactura);

    // ✅ Para reservas confirmadas, NO mostrar botón "Marcar como pagada"
    // ⚠️ COMENTADO: El botón btn-marcar-pagada no existe en el DOM actual
    // const btnMarcarPagada = document.getElementById('btn-marcar-pagada');
    // btnMarcarPagada.style.display = 'none';

    document.getElementById('seccion-factura').scrollIntoView({ behavior: 'smooth' });
}

// ============================================================
// REALIZAR TRANSACCI�N BANCARIA V�A API
// ============================================================
function realizarTransaccionBancaria(monto) {
    return new Promise((resolve, reject) => {
        // C�dulas del sistema
        const CEDULA_CLIENTE = "1750942508";
        const CEDULA_RESTAURANTE = "1700000000";
        const BASE_URL = "http://mibanca.runasp.net";

  // Paso 1: Obtener cliente y sus cuentas
        Promise.all([
     obtenerClienteYCuentas(CEDULA_CLIENTE, BASE_URL),
            obtenerClienteYCuentas(CEDULA_RESTAURANTE, BASE_URL)
        ])
        .then(([clienteOrigen, clienteDestino]) => {
    // Validar que se obtuvieron los clientes
            if (!clienteOrigen || !clienteOrigen.cuenta_id) {
              return reject(new Error("No se pudo obtener la cuenta del cliente origen"));
 }
      if (!clienteDestino || !clienteDestino.cuenta_id) {
     return reject(new Error("No se pudo obtener la cuenta del restaurante"));
      }

     // Verificar saldo
            const saldo = clienteOrigen.saldo || 0;
  if (saldo < monto) {
   resolve({
  ok: false,
        success: false,
      mensaje: `Saldo insuficiente. Disponible: $${saldo.toFixed(2)}`
             });
       return;
   }

            // Paso 2: Crear la transacci�n con los cuenta_id obtenidos
            crearTransaccion(
        clienteOrigen.cuenta_id,
     clienteDestino.cuenta_id,
                monto,
                BASE_URL
)
            .then(resultado => {
         resolve(resultado);
      })
    .catch(error => {
        reject(error);
        });
        })
        .catch(error => {
        reject(error);
   });
    });
}

// ============================================================
// OBTENER CLIENTE Y SUS CUENTAS
// ============================================================
function obtenerClienteYCuentas(cedula, baseUrl) {
    return new Promise((resolve, reject) => {
        // Obtener cliente
        fetch(`${baseUrl}/api/clientes/${cedula}`)
      .then(response => response.json())
        .then(cliente => {
     if (!cliente) {
      resolve(null);
     return;
    }

   // Obtener cuentas del cliente
      fetch(`${baseUrl}/api/Cuentas/cliente/${cedula}`)
        .then(response => response.json())
     .then(cuentas => {
         if (!cuentas || cuentas.length === 0) {
        resolve(null);
  return;
     }

            // Retornar la primera cuenta con saldo suficiente
  let cuentaSeleccionada = null;
          for (let cuenta of cuentas) {
    const saldo = parseFloat(cuenta.saldo || 0);
    if (saldo > 0) {
             cuentaSeleccionada = {
        cuenta_id: cuenta.cuenta_id,
           saldo: saldo
    };
       break;
          }
   }

 if (!cuentaSeleccionada && cuentas.length > 0) {
   // Si no hay cuenta con saldo, usar la primera
  cuentaSeleccionada = {
         cuenta_id: cuentas[0].cuenta_id,
             saldo: parseFloat(cuentas[0].saldo || 0)
     };
       }

   resolve(cuentaSeleccionada);
         })
   .catch(error => {
    console.error(`Error obteniendo cuentas de ${cedula}:`, error);
      reject(error);
          });
     })
            .catch(error => {
      console.error(`Error obteniendo cliente ${cedula}:`, error);
     reject(error);
    });
    });
}

// ============================================================
// CREAR TRANSACCI�N CON CUENTA_ID
// ============================================================
function crearTransaccion(cuentaOrigen, cuentaDestino, monto, baseUrl) {
    return new Promise((resolve, reject) => {
        const payload = {
          cuenta_origen: cuentaOrigen,
            cuenta_destino: cuentaDestino,
            monto: monto
        };

        fetch(`${baseUrl}/api/Transacciones`, {
         method: 'POST',
 headers: {
      'Content-Type': 'application/json'
            },
     body: JSON.stringify(payload)
        })
        .then(response => {
      return response.text().then(text => {
             return {
   ok: response.ok,
      status: response.status,
               text: text
       };
          });
        })
        .then(resultado => {
            let datos = null;
 try {
            datos = JSON.parse(resultado.text);
    } catch (e) {
      datos = { mensaje: resultado.text };
     }

            const mensaje = typeof datos === 'string' 
     ? datos 
    : (datos.mensaje || datos.Mensaje || 'Transacci�n procesada');

    if (resultado.ok && resultado.status === 200) {
                resolve({
     ok: true,
      success: true,
     mensaje: mensaje,
                monto: monto
            });
         } else {
     resolve({
    ok: false,
    success: false,
        mensaje: mensaje || 'Error en la transacci�n bancaria'
        });
  }
        })
     .catch(error => {
        console.error('Error en la transacci�n:', error);
  reject(error);
        });
    });
}

// ============================================================
// PROCESAR PAGO BANCARIO
// ============================================================
function procesarPagoBancario() {
if (reservasSeleccionadas.length === 0) {
      showAlert('No hay reservas seleccionadas para pagar', 'warning');
      return;
    }

    let totalAPagar = 0;
  reservasSeleccionadas.forEach(idReserva => {
   const reserva = todasReservas.find(r => r.IdReserva == idReserva);
        if (reserva) {
          totalAPagar += reserva.TotalFinal || 0;
  }
    });

    document.getElementById('estado-pago-bancario').style.display = 'block';
    const spinner = document.querySelector('#estado-pago-bancario .spinner-border');
    if (spinner) spinner.style.display = 'block';
    document.getElementById('confirmar-pago-bancario').disabled = true;

    const mensajePago = document.getElementById('mensaje-pago-bancario');
    mensajePago.innerHTML = `
        <div class="text-center">
     <div class="spinner-border text-primary mb-3" role="status">
  <span class="visually-hidden">Procesando...</span>
       </div>
<p class="text-muted"><strong>Procesando transferencia bancaria...</strong></p>
        <p class="small text-muted">Por favor espera mientras se procesa tu pago de <strong>$${totalAPagar.toFixed(2)}</strong></p>
        </div>
    `;

    realizarTransaccionBancaria(totalAPagar)
        .then(resultado => {
          console.log('Resultado transacci�n:', resultado);
       
          // Aceptar como exitoso si:
 // 1. ok o success son true, O
       // 2. El mensaje contiene "correctamente"
            const mensaje = (resultado?.mensaje || resultado?.message || '').toLowerCase();
            const esExitoso = (resultado?.ok === true || resultado?.success === true) || 
        mensaje.includes('correctamente');
   
 if (esExitoso) {
    // Pasar el monto total que se va a pagar
          confirmarReservasConPagoBancario(resultado, totalAPagar);
   } else {
    mostrarErrorPagoBancario(resultado?.mensaje || resultado?.message || 'Error en la transacci�n');
         }
        })
        .catch(error => {
    console.error('Error en pago bancario:', error);
     mostrarErrorPagoBancario('Error procesando el pago: ' + error.message);
        });
}

// ============================================================
// MOSTRAR ERROR EN PAGO BANCARIO
// ============================================================
function mostrarErrorPagoBancario(mensaje) {
    const estadoPagoBancario = document.getElementById('estado-pago-bancario');
    const mensajePago = document.getElementById('mensaje-pago-bancario');

    mensajePago.innerHTML = `
        <div class="alert alert-danger border-2 border-danger">
            <i class="bi bi-x-circle-fill" style="font-size: 2rem; color: #dc3545;"></i>
    <h5 class="mt-2 text-danger"><strong>Error en la Transacci�n</strong></h5>
      <p class="mb-0">${mensaje}</p>
        </div>
    `;

    const spinner = document.querySelector('#estado-pago-bancario .spinner-border');
    if (spinner) spinner.style.display = 'none';
    document.getElementById('confirmar-pago-bancario').disabled = false;
}

// ============================================================
// CONFIRMAR RESERVAS CON PAGO BANCARIO
// ============================================================
function confirmarReservasConPagoBancario(resultadoPago, totalAPagar) {
    showLoading();

    const requestData = {
        IdUsuario: usuario.IdUsuario,
ReservasIds: reservasSeleccionadas.join(','),
     PromocionId: promocionSeleccionada,
  MetodoPago: 'Transferencia Bancaria', 
   Monto: totalAPagar
    };

    fetch('/api/carrito/confirmar', {
    method: 'POST',
        headers: {
      'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestData)
    })
    .then(response => response.json())
    .then(data => {
 hideLoading();

if (data.success) {
 // Mostrar mensaje de �xito m�s claro en el modal
    const mensajePago = document.getElementById('mensaje-pago-bancario');
     const monto = totalAPagar || resultadoPago?.monto || 0;
        mensajePago.innerHTML = `
   <div class="alert alert-success border-2 border-success">
     <i class="bi bi-check-circle-fill" style="font-size: 2rem; color: #198754;"></i>
   <h5 class="mt-2 text-success"><strong>Transaccion Exitosa</strong></h5>
      <p class="mb-2">Tu pago ha sido procesado correctamente.</p>
<p class="mb-1"><small><strong>Monto transferido:</strong> $${typeof monto === 'number' ? monto.toFixed(2) : monto}</small></p>
  <p class="mb-0"><small><strong>Metodo:</strong> Transferencia Bancaria</small></p>
    <hr>
     <p class="mb-0 text-muted"><strong>Tus reservas han sido confirmadas y apareceran en "Mis Reservas Confirmadas"</strong></p>
   </div>
       `;

  document.getElementById('confirmar-pago-bancario').style.display = 'none';
    
   // Esperar 4 segundos antes de cerrar el modal
    setTimeout(() => {
 const modal = bootstrap.Modal.getInstance(document.getElementById('modalPagoBancario'));
        if (modal) modal.hide();

 showAlert('Reservas confirmadas y pagadas exitosamente ??', 'success');
    
        // Recargar carrito y reservas confirmadas
cargarCarritoUsuario();
        cargarReservasConfirmadas();
        reservasSeleccionadas = [];
  
  // Restaurar bot�n
        document.getElementById('confirmar-pago-bancario').style.display = 'block';
        document.getElementById('confirmar-pago-bancario').disabled = false;
        document.getElementById('estado-pago-bancario').style.display = 'none';
    }, 4000);
} else {
        mostrarErrorPagoBancario(data.message || 'Error confirmando las reservas');
    }

    })
  .catch(error => {
  hideLoading();
 console.error('Error:', error);
    mostrarErrorPagoBancario('Error al confirmar las reservas: ' + error.message);
    });
}

