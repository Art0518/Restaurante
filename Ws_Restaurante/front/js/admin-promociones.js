// ========================================
// VARIABLES GLOBALES
// ========================================
let promociones = [];
const API_URL = "http://cafesanjuanr.runasp.net/api/promociones";

// ========================================
// CARGAR PROMOCIONES AL INICIO
// ========================================
document.addEventListener("DOMContentLoaded", () => {
    console.log("Cargando pagina de promociones...");
    cargarPromociones();

    // Event listener para el formulario
    document.getElementById("form-promocion").addEventListener("submit", guardarPromocion);
});

// ========================================
// CARGAR PROMOCIONES DESDE LA API
// ========================================
function cargarPromociones() {
    console.log("Solicitando promociones a la API...");
    
    fetch(API_URL)
        .then(response => {
        console.log("Respuesta recibida, status:", response.status);
   if (!response.ok) {
     throw new Error(`HTTP error! status: ${response.status}`);
         }
            return response.json();
        })
        .then(data => {
  console.log("Datos recibidos de la API:", data);

            // La API puede devolver { mensaje, promociones } o directamente un array
 if (data.promociones) {
         promociones = data.promociones;
    } else if (Array.isArray(data)) {
   promociones = data;
    } else {
        promociones = [];
      }

  console.log("Total de promociones:", promociones.length);

     if (promociones.length > 0) {
            mostrarPromociones();
   } else {
      document.getElementById("lista-promociones").innerHTML = `
<tr>
         <td colspan="6" class="mensaje-vacio">
          No hay promociones registradas. Crea la primera!
     </td>
      </tr>
           `;
            }
      })
        .catch(error => {
            console.error("Error al cargar promociones:", error);
  document.getElementById("lista-promociones").innerHTML = `
          <tr>
<td colspan="6" class="mensaje-vacio" style="color: #dc3545;">
     Error al cargar promociones: ${error.message}
        <br><small>Verifica que la API este funcionando y que tengas permisos de administrador.</small>
         </td>
     </tr>
            `;
        });
}

// ========================================
// MOSTRAR PROMOCIONES EN LA TABLA
// ========================================
function mostrarPromociones() {
    const tbody = document.getElementById("lista-promociones");
    tbody.innerHTML = "";

    if (!promociones || promociones.length === 0) {
        tbody.innerHTML = `
     <tr>
            <td colspan="6" class="mensaje-vacio">
            No hay promociones disponibles
            </td>
 </tr>
        `;
        return;
    }

    promociones.forEach(promo => {
        try {
     let estadoClass = "estado-activa";
      if (promo.Estado === "Inactiva") {
         estadoClass = "estado-inactiva";
} else if (promo.Estado === "Programada") {
    estadoClass = "estado-programada";
  }

  const fechaInicio = formatearFecha(promo.FechaInicio);
            const fechaFin = formatearFecha(promo.FechaFin);

       tbody.innerHTML += `
  <tr>
   <td>${promo.IdPromocion || 'N/A'}</td>
    <td>${parseFloat(promo.Descuento || 0).toFixed(2)}%</td>
                <td>${fechaInicio}</td>
         <td>${fechaFin}</td>
    <td>
      <span class="estado-badge ${estadoClass}">
                ${promo.Estado || 'N/A'}
         </span>
         </td>
        <td>
       <div class="acciones">
     <button class="btn-editar" onclick="editarPromocion(${promo.IdPromocion})">
          Editar
   </button>
   <button class="btn-eliminar" onclick="confirmarEliminar(${promo.IdPromocion})">
  Eliminar
      </button>
      </div>
    </td>
     </tr>
    `;
      } catch (error) {
     console.error("Error al mostrar promocion:", promo, error);
        }
    });
}

// ========================================
// FORMATEAR FECHA (YYYY-MM-DD)
// ========================================
function formatearFecha(fecha) {
    if (!fecha) return "N/A";

    // Si la fecha viene con formato ISO, extraer solo la fecha
    const f = new Date(fecha);
    const year = f.getFullYear();
    const month = String(f.getMonth() + 1).padStart(2, '0');
    const day = String(f.getDate()).padStart(2, '0');

    return `${year}-${month}-${day}`;
}

// ========================================
// ABRIR MODAL (NUEVA PROMOCION)
// ========================================
function abrirModal() {
    document.getElementById("modal-promocion").style.display = "flex";
    document.getElementById("titulo-modal").textContent = "Nueva Promocion";
    document.getElementById("form-promocion").reset();
    document.getElementById("id-promocion").value = "0";
}

// ========================================
// CERRAR MODAL
// ========================================
function cerrarModal() {
    document.getElementById("modal-promocion").style.display = "none";
}

// ========================================
// EDITAR PROMOCION
// ========================================
function editarPromocion(idPromocion) {
    const promo = promociones.find(p => p.IdPromocion == idPromocion);

 if (!promo) {
        alert("Promocion no encontrada");
  return;
    }

    console.log("Editando promocion:", promo);

    // Rellenar formulario (sin campo nombre)
    document.getElementById("id-promocion").value = promo.IdPromocion || 0;
    document.getElementById("descuento").value = parseFloat(promo.Descuento || 0);
    document.getElementById("fecha-inicio").value = formatearFecha(promo.FechaInicio);
    document.getElementById("fecha-fin").value = formatearFecha(promo.FechaFin);
    document.getElementById("estado").value = promo.Estado || "Activa";

    document.getElementById("titulo-modal").textContent = "Editar Promocion";
    document.getElementById("modal-promocion").style.display = "flex";
}

// ========================================
// GUARDAR PROMOCION (CREAR O ACTUALIZAR)
// ========================================
function guardarPromocion(e) {
    e.preventDefault();

    const idPromocion = parseInt(document.getElementById("id-promocion").value);
    const descuento = parseFloat(document.getElementById("descuento").value);
    const fechaInicio = document.getElementById("fecha-inicio").value;
    const fechaFin = document.getElementById("fecha-fin").value;
    const estado = document.getElementById("estado").value;

    // Validaciones
    if (isNaN(descuento) || descuento <= 0 || descuento > 100 || !fechaInicio || !fechaFin) {
   alert("Por favor completa todos los campos correctamente.");
    return;
    }

    if (new Date(fechaInicio) > new Date(fechaFin)) {
        alert("La fecha de inicio no puede ser mayor a la fecha de fin.");
      return;
    }

    // Generar nombre automáticamente basado en el descuento
    const nombreAutomatico = `Descuento ${descuento}%`;

    // Preparar objeto
    const promocion = {
      IdPromocion: idPromocion,
        Nombre: nombreAutomatico,
        Descuento: descuento,
        FechaInicio: fechaInicio,
        FechaFin: fechaFin,
Estado: estado
    };

    console.log("Guardando promocion:", promocion);

    // Enviar a la API
    fetch(`${API_URL}/gestionar`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
    body: JSON.stringify(promocion)
    })
    .then(response => response.json())
    .then(data => {
        console.log("Respuesta guardar:", data);

        if (data.mensaje) {
    alert(data.mensaje);
       cerrarModal();
  cargarPromociones();
     } else {
            alert("Promocion guardada correctamente");
      cerrarModal();
     cargarPromociones();
   }
    })
    .catch(error => {
        console.error("Error al guardar promocion:", error);
        alert("Error al guardar la promocion. Verifica los datos e intenta nuevamente.");
    });
}

// ========================================
// CONFIRMAR ELIMINACION
// ========================================
function confirmarEliminar(idPromocion) {
  const promo = promociones.find(p => p.IdPromocion == idPromocion);

    if (!promo) {
     alert("Promocion no encontrada");
      return;
    }

    if (confirm(`Estas seguro de que deseas eliminar esta promocion?\n\nEsta accion no se puede deshacer.`)) {
        eliminarPromocion(idPromocion);
    }
}

// ========================================
// ELIMINAR PROMOCION (ELIMINACION FISICA)
// ========================================
function eliminarPromocion(idPromocion) {
    console.log("Eliminando promocion:", idPromocion);

    fetch(`${API_URL}/eliminar/${idPromocion}`, {
     method: "DELETE",
        headers: { "Content-Type": "application/json" }
 })
    .then(response => {
      console.log("Respuesta eliminar, status:", response.status);
        return response.json();
})
    .then(data => {
        console.log("Respuesta eliminar:", data);
        if (data.mensaje) {
            alert(data.mensaje);
        } else {
    alert("Promocion eliminada correctamente");
        }
cargarPromociones(); // Recargar lista
    })
    .catch(error => {
        console.error("Error al eliminar promocion:", error);
alert("Error al eliminar la promocion: " + error.message);
    });
}

// ========================================
// CERRAR MODAL AL HACER CLIC FUERA
// ========================================
window.onclick = function(event) {
    const modal = document.getElementById("modal-promocion");
    if (event.target === modal) {
 cerrarModal();
    }
}