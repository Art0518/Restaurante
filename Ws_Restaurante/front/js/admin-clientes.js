// =====================================
// VARIABLES
// =====================================
let paginaActual = 1;
let registrosPorPagina = 30;
let usuarios = [];
let usuariosFiltrados = [];
let usuarioEditando = null;
let modo = ""; // "nuevo" o "editar"
let guardandoCliente = false; // 🔥 Prevenir múltiples clics


// =====================================
// INICIO
// =====================================
document.addEventListener("DOMContentLoaded", () => {
    cargarUsuarios();
    document.getElementById("buscador").addEventListener("input", filtrar);
    
    // 🔥 Agregar validación en tiempo real
    agregarValidacionTiempoReal();
});

// =====================================
// CARGAR USUARIOS
// =====================================
function cargarUsuarios() {
    fetch("http://cafesanjuanr.runasp.net/api/usuarios")
        .then(r => r.json())
        .then(data => {
            usuarios = data.Usuarios || data;
            usuariosFiltrados = usuarios;
            paginaActual = 1;
            renderTabla();
            renderPaginacion();
        });
}

// =====================================
// RENDER TABLA
// =====================================
function renderTabla() {
    const inicio = (paginaActual - 1) * registrosPorPagina;
    const fin = inicio + registrosPorPagina;

    const datos = usuariosFiltrados.slice(inicio, fin);
    const tbody = document.querySelector("#tabla-clientes tbody");
    tbody.innerHTML = "";

    datos.forEach(u => {

        const botonAccion = u.Estado === "ACTIVO"
            ? `<button class="btn-eliminar" onclick="cambiarEstado(${u.IdUsuario}, 'INACTIVO')">Inactivar</button>`
            : `<button class="btn-eliminar" style="background:#6b8e23;" onclick="cambiarEstado(${u.IdUsuario}, 'ACTIVO')">Activar</button>`;

        tbody.innerHTML += `
     <tr class="${u.Estado === "INACTIVO" ? "fila-inactiva" : ""}">
  <td>${u.IdUsuario}</td>
            <td>${u.Nombre}</td>
    <td>${u.Email}</td>
    <td>${u.Rol}</td>
         <td style="font-weight:bold; color:${u.Estado === "INACTIVO" ? "red" : "#5b4230"}">
            ${u.Estado}
              </td>
             <td>${u.Telefono}</td>
   <td>
          ${botonAccion}
   </td>
   </tr>
    `;
    });
}

// =====================================
// BUSCADOR
// =====================================
function filtrar() {
    const texto = document.getElementById("buscador").value.toLowerCase();

    if (texto === "") {
        usuariosFiltrados = usuarios;
    } else {
        usuariosFiltrados = usuarios.filter(u =>
            u.Nombre.toLowerCase().includes(texto) ||
            u.Email.toLowerCase().includes(texto)
        );
    }

    paginaActual = 1;
    renderTabla();
    renderPaginacion();
}

// =====================================
// PAGINACIÓN AVANZADA
// =====================================
function cambiarPagina(nueva) {
    const totalPaginas = Math.ceil(usuariosFiltrados.length / registrosPorPagina);

    if (nueva >= 1 && nueva <= totalPaginas) {
        paginaActual = nueva;
        renderTabla();
        renderPaginacion();
    }
}

function renderPaginacion() {
    const totalPaginas = Math.ceil(usuariosFiltrados.length / registrosPorPagina);
    const pagDiv = document.querySelector(".paginacion");

    pagDiv.innerHTML = "";

    pagDiv.innerHTML += `<button onclick="cambiarPagina(1)">«</button>`;
    pagDiv.innerHTML += `<button onclick="cambiarPagina(${paginaActual - 1})"><</button>`;

    let inicio = Math.max(1, paginaActual - 3);
    let fin = Math.min(totalPaginas, paginaActual + 3);

    for (let i = inicio; i <= fin; i++) {
        pagDiv.innerHTML += `
            <button onclick="cambiarPagina(${i})"
                style="${i === paginaActual ? 'background:#2c3e50' : ''}">
                ${i}
            </button>
        `;
    }

    pagDiv.innerHTML += `<button onclick="cambiarPagina(${paginaActual + 1})">></button>`;
    pagDiv.innerHTML += `<button onclick="cambiarPagina(${totalPaginas})">»</button>`;

    pagDiv.innerHTML += `
        <span style="margin-left:10px; font-weight:bold;">
            Página ${paginaActual} de ${totalPaginas}
        </span>`;
}

// =====================================
// EDITAR USUARIO
// =====================================
function editarUsuario(id) {
    modo = "editar";
    usuarioEditando = usuarios.find(u => u.IdUsuario === id);
    guardandoCliente = false; // 🔥 Resetear flag

    document.getElementById("modal-title").innerText = "Editar Cliente";
    document.getElementById("edit-nombre").value = usuarioEditando.Nombre;
    document.getElementById("edit-email").value = usuarioEditando.Email;
    document.getElementById("edit-telefono").value = usuarioEditando.Telefono;
    document.getElementById("edit-direccion").value = usuarioEditando.Direccion;

    // 🔥 Limpiar clases de validación
 limpiarValidacion();

    abrirModal();
}

// =====================================
// MODAL
// =====================================
function abrirModal() {
    document.getElementById("modal-editar").style.display = "flex";
}

function cerrarModal() {
    guardandoCliente = false; // 🔥 Resetear flag
    
 // 🔥 Rehabilitar botón si estaba deshabilitado
 const btnGuardar = document.querySelector('.modal-save');
  if (btnGuardar) {
 btnGuardar.disabled = false;
  btnGuardar.textContent = 'Guardar';
    }
    
    document.getElementById("modal-editar").style.display = "none";
    limpiarValidacion();
}

// =====================================
// 🔥 FUNCIONES DE VALIDACIÓN
// =====================================
function limpiarValidacion() {
    ['edit-nombre', 'edit-email', 'edit-telefono', 'edit-direccion'].forEach(id => {
 const campo = document.getElementById(id);
        if (campo) {
campo.classList.remove('error', 'success');
    }
    });
}

function marcarCampo(id, esValido) {
    const campo = document.getElementById(id);
    if (campo) {
     campo.classList.remove('error', 'success');
   campo.classList.add(esValido ? 'success' : 'error');
    }
}

// 🔥 VALIDAR NOMBRE COMPLETO (al menos 2 palabras, solo letras)
function validarNombre(nombre) {
    if (!nombre || nombre.trim().length === 0) {
    return { valido: false, mensaje: "El nombre es obligatorio" };
    }

    const nombreLimpio = nombre.trim();
 const palabras = nombreLimpio.split(/\s+/);

    if (palabras.length < 2) {
 return { valido: false, mensaje: "Debe incluir al menos nombre y apellido" };
    }

    // Solo letras, espacios, ñ y tildes
    const regex = /^[a-zA-ZñÑáéíóúÁÉÍÓÚ\s]+$/;
    if (!regex.test(nombreLimpio)) {
     return { valido: false, mensaje: "Solo se permiten letras, ñ y tildes" };
    }

    // Cada palabra debe tener al menos 2 caracteres
    for (let palabra of palabras) {
        if (palabra.length < 2) {
   return { valido: false, mensaje: "Cada nombre debe tener al menos 2 letras" };
   }
    }

    return { valido: true };
}

// 🔥 VALIDAR EMAIL
function validarEmail(email) {
    if (!email || email.trim().length === 0) {
     return { valido: false, mensaje: "El email es obligatorio" };
    }

    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!regex.test(email)) {
        return { valido: false, mensaje: "Formato de email inválido" };
    }

    return { valido: true };
}

// 🔥 VALIDAR TELÉFONO (formato dominicano)
function validarTelefono(telefono) {
    if (!telefono || telefono.trim().length === 0) {
return { valido: false, mensaje: "El teléfono es obligatorio" };
    }

    // Remover espacios, guiones, paréntesis y el +1
    const telefonoLimpio = telefono.replace(/[\s\-\(\)\+]/g, '');
    
    // 🔥 CAMBIO: Remover el 1 inicial solo si el número tiene más de 10 dígitos
    let telefonoFinal = telefonoLimpio;
    if (telefonoLimpio.length === 11 && telefonoLimpio.startsWith('1')) {
        telefonoFinal = telefonoLimpio.substring(1);
    }

    // Debe tener exactamente 10 dígitos
    if (telefonoFinal.length !== 10) {
     return { valido: false, mensaje: "Debe tener 10 dígitos (809/829/849-XXX-XXXX)" };
  }

    // Debe comenzar con 809, 829 o 849
    const regex = /^(809|829|849)\d{7}$/;
    if (!regex.test(telefonoFinal)) {
        return { valido: false, mensaje: "Debe comenzar con 809, 829 o 849" };
    }

    return { valido: true };
}

// 🔥 VALIDAR DIRECCIÓN
function validarDireccion(direccion) {
    if (!direccion || direccion.trim().length === 0) {
      return { valido: false, mensaje: "La dirección es obligatoria" };
    }

    const direccionLimpia = direccion.trim();

    if (direccionLimpia.length < 10) {
        return { valido: false, mensaje: "La dirección debe tener al menos 10 caracteres" };
    }

    // Debe contener al menos un número (número de casa/calle)
    if (!/\d/.test(direccionLimpia)) {
        return { valido: false, mensaje: "La dirección debe incluir un número" };
    }

    // Debe contener letras
  if (!/[a-zA-Z]/.test(direccionLimpia)) {
  return { valido: false, mensaje: "La dirección debe incluir texto" };
    }

    return { valido: true };
}

// =====================================
// GUARDAR CAMBIOS (SOLO EDICIÓN)
// =====================================
function guardarCambios() {
 // 🔥 Prevenir múltiples clics
    if (guardandoCliente) {
   showWarning("Por favor espere, guardando cambios...");
        return;
    }

    // 🔥 Obtener valores del formulario
    const nombre = document.getElementById("edit-nombre").value.trim();
  const email = document.getElementById("edit-email").value.trim();
    const telefono = document.getElementById("edit-telefono").value.trim();
    const direccion = document.getElementById("edit-direccion").value.trim();

 // ===============================
    // 🔥 VALIDACIONES
    // ===============================
    let errores = [];

    // Validar nombre
    const validacionNombre = validarNombre(nombre);
    marcarCampo('edit-nombre', validacionNombre.valido);
    if (!validacionNombre.valido) {
        errores.push(validacionNombre.mensaje);
    }

    // Validar email
    const validacionEmail = validarEmail(email);
  marcarCampo('edit-email', validacionEmail.valido);
    if (!validacionEmail.valido) {
errores.push(validacionEmail.mensaje);
    }

    // Validar teléfono
    const validacionTelefono = validarTelefono(telefono);
    marcarCampo('edit-telefono', validacionTelefono.valido);
if (!validacionTelefono.valido) {
   errores.push(validacionTelefono.mensaje);
    }

    // Validar dirección
    const validacionDireccion = validarDireccion(direccion);
    marcarCampo('edit-direccion', validacionDireccion.valido);
    if (!validacionDireccion.valido) {
 errores.push(validacionDireccion.mensaje);
    }

    // 🔥 Si hay errores, mostrarlos y detener
    if (errores.length > 0) {
        const mensajeError = "Errores de validación:\n\n" + errores.map((e, i) => `${i + 1}. ${e}`).join('\n');
        showError(mensajeError, "Validación de Datos");
  return;
    }

    // ===============================
 // 🔥 INICIAR GUARDADO
    // ===============================
    guardandoCliente = true;
    
    // 🔥 Deshabilitar botón de guardar
 const btnGuardar = document.querySelector('.modal-save');
    const textoOriginal = btnGuardar.textContent;
    btnGuardar.disabled = true;
    btnGuardar.textContent = 'Guardando...';

    const body = {
   Nombre: nombre,
        Email: email,
   Telefono: telefono,
        Direccion: direccion,
Rol: "CLIENTE",
        Contrasena: "123456"
    };

    if (modo === "editar") {
        fetch(`http://cafesanjuanr.runasp.net/api/usuarios/edit/${usuarioEditando.IdUsuario}`, {
            method: "PUT",
         headers: { "Content-Type": "application/json" },
  body: JSON.stringify(body)
 })
      .then(response => {
         if (response.ok) {
   showSuccess("Cliente actualizado correctamente", "Actualización Exitosa", function() {
        cerrarModal();
        cargarUsuarios();
      });
  } else {
       return response.text().then(text => {
   throw new Error(text || "Error al actualizar cliente");
 });
}
            })
 .catch(err => {
                showError("Error al actualizar cliente: " + err.message);
   console.error(err);
      // 🔥 Rehabilitar botón en caso de error
btnGuardar.disabled = false;
     btnGuardar.textContent = textoOriginal;
 })
   .finally(() => {
// 🔥 Liberar el bloqueo después de 2 segundos
 setTimeout(() => {
    guardandoCliente = false;
              if (btnGuardar) {
   btnGuardar.disabled = false;
     btnGuardar.textContent = textoOriginal;
         }
     }, 2000);
   });
    }
}

// =====================================
// ACTIVAR / INACTIVAR USUARIO
// =====================================
function cambiarEstado(id, nuevoEstado) {
    const mensaje = nuevoEstado === "INACTIVO"
   ? "¿Estás seguro de que deseas INACTIVAR este usuario?"
     : "¿Estás seguro de que deseas ACTIVAR este usuario?";

  showConfirm(mensaje,
     function() {
 // Usuario confirmó
 fetch(`http://cafesanjuanr.runasp.net/api/usuarios/${id}/estado`, {
     method: "PUT",
       headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ Estado: nuevoEstado })
   })
    .then(() => {
    const accion = nuevoEstado === "INACTIVO" ? "inactivado" : "activado";
    showSuccess(`Usuario ${accion} correctamente`, "Estado Actualizado", function() {
   cargarUsuarios();
   });
   })
    .catch(err => {
     showError("Error al cambiar el estado del usuario");
   console.error(err);
    });
 },
      function() {
   // Usuario canceló
  console.log('Cambio de estado cancelado');
 }
    );
}


// =====================================
// 🔥 VALIDACIÓN EN TIEMPO REAL
// =====================================
function agregarValidacionTiempoReal() {
    const campos = [
        { id: 'edit-nombre', validador: validarNombre },
        { id: 'edit-email', validador: validarEmail },
 { id: 'edit-telefono', validador: validarTelefono },
        { id: 'edit-direccion', validador: validarDireccion }
    ];

    campos.forEach(({ id, validador }) => {
        const campo = document.getElementById(id);
   if (campo) {
            campo.addEventListener('blur', function() {
     const resultado = validador(this.value);
 marcarCampo(id, resultado.valido);
});
 
            // Limpiar marcas mientras escribe
            campo.addEventListener('input', function() {
   this.classList.remove('error', 'success');
         });
      }
    });
}
