// CARRITO DE RESERVAS - JavaScript con Promociones y Descuentos
// ============================================================

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
if (reservasSeleccionadas.length ===0) {
        showAlert('Selecciona al menos una reserva para pagar', 'warning');
            return;
        }
      
        // Calcular total a pagar
        let totalAPagar =0;
        reservasSeleccionadas.forEach(idReserva => {
     const reserva = todasReservas.find(r => r.IdReserva == idReserva);
      if (reserva) {
  totalAPagar += reserva.TotalFinal ||0;
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

    // Botón deseleccionar todas
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

    fetch(`${BASES.RESERVAS}/api/carrito/promociones-validas/${usuario.IdUsuario}`)
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

    if (!promociones || promociones.length ===0) {
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

    let url = `${BASES.RESERVAS}/api/carrito/usuario/${usuario.IdUsuario}`;
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

    let url = `${BASES.RESERVAS}/api/carrito/usuario/${usuario.IdUsuario}`;
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
    if (!confirm('¿Estás seguro de que deseas eliminar esta reserva definitivamente del carrito?')) {
        return;
    }

    showLoading();

    fetch(`${BASES.RESERVAS}/api/carrito/eliminar/${usuario.IdUsuario}/${idReserva}`, {
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
 if (todasReservas.length ===0) return;

    showLoading();

    let eliminacionesCompletas =0;
    const totalEliminaciones = todasReservas.length;

    todasReservas.forEach(reserva => {
 fetch(`${BASES.RESERVAS}/api/carrito/eliminar/${usuario.IdUsuario}/${reserva.IdReserva}`, {
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

    fetch(`${BASES.RESERVAS}/api/carrito/confirmar`, {
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

    fetch(`${BASES.FACTURACION}/api/facturas/generar-confirmadas`, {
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
// CONFIRMAR RESERVAS CON PAGO BANCARIO -> RESERVAS service
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

 fetch(`${BASES.RESERVAS}/api/carrito/confirmar`, {
 method: 'POST',
 headers: { 'Content-Type': 'application/json' },
 body: JSON.stringify(requestData)
 })
 .then(response => response.json())
 .then(data => {
 hideLoading();

 if (data.success) {
 const mensajePago = document.getElementById('mensaje-pago-bancario');
 const monto = totalAPagar || resultadoPago?.monto ||0;
 mensajePago.innerHTML = `
 <div class="alert alert-success border-2 border-success">
 <i class="bi bi-check-circle-fill" style="font-size:2rem; color: #198754;"></i>
 <h5 class="mt-2 text-success"><strong>Transaccion Exitosa</strong></h5>
 <p class="mb-2">Tu pago ha sido procesado correctamente.</p>
 <p class="mb-1"><small><strong>Monto transferido:</strong> $${typeof monto === 'number' ? monto.toFixed(2) : monto}</small></p>
 <p class="mb-0"><small><strong>Metodo:</strong> Transferencia Bancaria</small></p>
 <hr>
 <p class="mb-0 text-muted"><strong>Tus reservas han sido confirmadas y apareceran en "Mis Reservas Confirmadas"</strong></p>
 </div>
 `;

 document.getElementById('confirmar-pago-bancario').style.display = 'none';

 setTimeout(() => {
 const modal = bootstrap.Modal.getInstance(document.getElementById('modalPagoBancario'));
 if (modal) modal.hide();

 showAlert('Reservas confirmadas y pagadas exitosamente', 'success');
 cargarCarritoUsuario();
 cargarReservasConfirmadas();
 reservasSeleccionadas = [];

 document.getElementById('confirmar-pago-bancario').style.display = 'block';
 document.getElementById('confirmar-pago-bancario').disabled = false;
 document.getElementById('estado-pago-bancario').style.display = 'none';
 },4000);
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

    fetch(`${BASES.FACTURACION}/api/facturas/generar-confirmadas`, {
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
// CONFIRMAR RESERVAS CON PAGO BANCARIO -> RESERVAS service
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

 fetch(`${BASES.RESERVAS}/api/carrito/confirmar`, {
 method: 'POST',
 headers: { 'Content-Type': 'application/json' },
 body: JSON.stringify(requestData)
 })
 .then(response => response.json())
 .then(data => {
 hideLoading();

 if (data.success) {
 const mensajePago = document.getElementById('mensaje-pago-bancario');
 const monto = totalAPagar || resultadoPago?.monto ||0;
 mensajePago.innerHTML = `
 <div class="alert alert-success border-2 border-success">
 <i class="bi bi-check-circle-fill" style="font-size:2rem; color: #198754;"></i>
 <h5 class="mt-2 text-success"><strong>Transaccion Exitosa</strong></h5>
 <p class="mb-2">Tu pago ha sido procesado correctamente.</p>
 <p class="mb-1"><small><strong>Monto transferido:</strong> $${typeof monto === 'number' ? monto.toFixed(2) : monto}</small></p>
 <p class="mb-0"><small><strong>Metodo:</strong> Transferencia Bancaria</small></p>
 <hr>
 <p class="mb-0 text-muted"><strong>Tus reservas han sido confirmadas y apareceran en "Mis Reservas Confirmadas"</strong></p>
 </div>
 `;

 document.getElementById('confirmar-pago-bancario').style.display = 'none';

 setTimeout(() => {
 const modal = bootstrap.Modal.getInstance(document.getElementById('modalPagoBancario'));
 if (modal) modal.hide();

 showAlert('Reservas confirmadas y pagadas exitosamente', 'success');
 cargarCarritoUsuario();
 cargarReservasConfirmadas();
 reservasSeleccionadas = [];

 document.getElementById('confirmar-pago-bancario').style.display = 'block';
 document.getElementById('confirmar-pago-bancario').disabled = false;
 document.getElementById('estado-pago-bancario').style.display = 'none';
 },4000);
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

