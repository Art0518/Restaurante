let mesasGlobal = [];
let mesaEditando = null;
let guardandoMesa = false; // 🔥 Variable para prevenir múltiples clics

// API endpoint for mesas
const API = "http://cafesanjuanr.runasp.net/api/mesas";

// ===============================
// VALIDAR ADMIN
// ===============================
document.addEventListener("DOMContentLoaded", () => {
    const usuario = JSON.parse(localStorage.getItem("usuario"));
    if (!usuario || usuario.Rol !== "ADMIN") {
        showError("Acceso denegado. Solo administradores pueden acceder.", "Acceso Denegado", function() {
        window.location.href = "index.html";
        });
      return;
    }
    cargarMesas();
});

// ===============================
// CARGAR MESAS
// ===============================
function cargarMesas() {
    fetch(API)
        .then(r => r.json())
        .then(data => {
            mesasGlobal = data;
            mostrarMesas(data);
        });
}

// ===============================
// MOSTRAR LISTA
// ===============================
function mostrarMesas(lista) {
    const cont = document.getElementById("lista-mesas");
    cont.innerHTML = "";

    lista.forEach(m => {

        // 🔥 Normalizamos el estado a mayúsculas
        let estado = (m.Estado || "").toUpperCase();

        let colorEstado =
            estado === "DISPONIBLE" ? "green" :
                estado === "OCUPADA" ? "red" :
                    estado === "INACTIVA" ? "gray" :
                        "gray";

        cont.innerHTML += `
        <div class="tarjeta" 
            style="display:flex; flex-direction:row; padding:25px; gap:35px; align-items:flex-start;">

            <!-- FOTO -->
            <div style="width:260px; text-align:center;">
                <img src="${m.ImagenURL}" 
                     style="width:240px; height:150px; object-fit:cover; border-radius:12px; border:1px solid #d8c5a3;">

                <div style="margin-top:12px; display:flex; flex-direction:column; gap:10px;">
                    <button onclick="cambiarFoto(${m.IdMesa})"
                        style="background:#6b8e23; color:white; padding:10px 18px; border-radius:8px; border:none; font-weight:bold;">
                        Cambiar foto
                    </button>

                    <button onclick="eliminarFoto(${m.IdMesa})"
                        style="background:#d9534f; color:white; padding:10px 18px; border-radius:8px; border:none; font-weight:bold;">
                        Eliminar foto
                    </button>
                </div>
            </div>

            <!-- DATOS -->
            <div style="flex:1;">
                <p><b>ID:</b> ${m.IdMesa}</p>
                <p><b>Número Mesa:</b> ${m.NumeroMesa}</p>
                <p><b>Tipo:</b> ${m.TipoMesa}</p>
                <p><b>Capacidad:</b> ${m.Capacidad}</p>
                <p><b>Precio:</b> $${m.Precio ? m.Precio.toFixed(2) : '0.00'}</p>

                <p><b>Estado:</b> 
                    <span style="font-weight:bold; color:${colorEstado};">${estado}</span>
                </p>

                <div style="margin-top:20px; display:flex; gap:15px;">

                    <button class="btn-editar" onclick="editarMesa(${m.IdMesa})">Editar</button>

                    ${estado === "INACTIVA"
                ? `<button class="btn-activar" onclick="activarMesa(${m.IdMesa})">Activar</button>`
                : `<button class="btn-eliminar" onclick="eliminarMesa(${m.IdMesa})">Eliminar</button>`
            }

                </div>
            </div>

        </div>`;
    });
}

// ===============================
// FILTROS
// ===============================
function aplicarFiltros() {
    let id = document.getElementById("filtro-id").value;
    let estadoFiltro = document.getElementById("filtro-estado").value.toUpperCase();

    let lista = mesasGlobal;

    if (id) lista = lista.filter(m => m.IdMesa == id);
    if (estadoFiltro)
        lista = lista.filter(m => (m.Estado || "").toUpperCase() === estadoFiltro);

    mostrarMesas(lista);
}

// ===============================
// SUBIR IMAGEN A CLOUDINARY
// ===============================
async function subirImagenMesa(file) {
    const f = new FormData();
    f.append("file", file);
    f.append("upload_preset", "mesas_upload");

    const res = await fetch("https://api.cloudinary.com/v1_1/dt7fbonmg/image/upload", {
        method: "POST",
        body: f
    });

    return res.json();
}

// ===============================
// PREVIEW
// ===============================
function previewMesa(e) {
    const img = document.getElementById("preview-mesa");
    img.src = URL.createObjectURL(e.target.files[0]);
    img.style.display = "block";
}

// ===============================
// ABRIR NUEVA MESA
// ===============================
function abrirModal() {
    mesaEditando = null;
    guardandoMesa = false; // 🔥 Resetear el flag al abrir modal

    document.getElementById("titulo-modal").textContent = "Nueva Mesa";
    document.getElementById("campo-imagen").style.display = "block";

    document.getElementById("m-numero").value = "";
    document.getElementById("m-tipo").value = "Interior";
    document.getElementById("m-capacidad").value = "";
    document.getElementById("m-precio").value = "";
    document.getElementById("m-estado").value = "Disponible";
    document.getElementById("m-imagen-file").value = ""; // 🔥 Limpiar input file

    document.getElementById("preview-mesa").style.display = "none";
    document.getElementById("preview-mesa").src = ""; // 🔥 Limpiar preview

    document.getElementById("modal").style.display = "flex";
}

// ===============================
// EDITAR MESA
// ===============================
function editarMesa(id) {
  mesaEditando = mesasGlobal.find(m => m.IdMesa === id);
    guardandoMesa = false; // 🔥 Resetear el flag al editar

    document.getElementById("titulo-modal").textContent = "Editar Mesa";
    document.getElementById("campo-imagen").style.display = "none";

    document.getElementById("m-numero").value = mesaEditando.NumeroMesa;
    document.getElementById("m-tipo").value = mesaEditando.TipoMesa;
    document.getElementById("m-capacidad").value = mesaEditando.Capacidad;
  document.getElementById("m-precio").value = mesaEditando.Precio || 0;
    document.getElementById("m-estado").value = mesaEditando.Estado;

    document.getElementById("modal").style.display = "flex";
}

// ===============================
// GUARDAR MESA
// ===============================
async function guardarMesa() {
    // 🔥 Prevenir múltiples clics
    if (guardandoMesa) {
 showWarning("Por favor espere, guardando mesa...");
  return;
    }

    // 🔥 Obtener valores del formulario
    const numeroMesa = parseInt(document.getElementById("m-numero").value);
    const tipoMesa = document.getElementById("m-tipo").value;
    const capacidad = parseInt(document.getElementById("m-capacidad").value);
    const precio = parseFloat(document.getElementById("m-precio").value);
    const estado = document.getElementById("m-estado").value;

    // ===============================
    // 🔥 VALIDACIONES
    // ===============================

    // Validar número de mesa
    if (!numeroMesa || numeroMesa <= 0) {
        showError("El número de mesa debe ser mayor a 0", "Validación");
   return;
    }

    // Validar capacidad entre 2 y 6
    if (!capacidad || capacidad < 2 || capacidad > 6) {
        showError("La capacidad debe estar entre 2 y 6 personas", "Validación");
        return;
    }

    // Validar precio positivo
    if (!precio || precio <= 0) {
        showError("El precio debe ser mayor a 0", "Validación");
        return;
    }

    // 🔥 Validar número de mesa duplicado
    if (!mesaEditando) {
        const mesaDuplicada = mesasGlobal.find(m => m.NumeroMesa === numeroMesa);
        if (mesaDuplicada) {
          showError(`Ya existe una mesa con el número ${numeroMesa}`, "Número Duplicado");
       return;
 }
    } else {
    // Al editar, verificar que no haya otra mesa con ese número (excepto la actual)
     const mesaDuplicada = mesasGlobal.find(m => 
  m.NumeroMesa === numeroMesa && m.IdMesa !== mesaEditando.IdMesa
        );
    if (mesaDuplicada) {
    showError(`Ya existe otra mesa con el número ${numeroMesa}`, "Número Duplicado");
            return;
        }
    }

    // ===============================
    // 🔥 INICIAR GUARDADO
    // ===============================
    guardandoMesa = true;
    
    // 🔥 Deshabilitar botón de guardar
    const btnGuardar = document.querySelector('.btn-save');
    const textoOriginal = btnGuardar.textContent;
    btnGuardar.disabled = true;
    btnGuardar.textContent = 'Guardando...';

    try {
        let imagenFinal = mesaEditando ? mesaEditando.ImagenURL : "";

    // Solo subir imagen si es una nueva mesa y hay archivo seleccionado
  if (!mesaEditando) {
        const file = document.getElementById("m-imagen-file").files[0];
            if (file) {
           btnGuardar.textContent = 'Subiendo imagen...';
            const cloud = await subirImagenMesa(file);
      imagenFinal = cloud.secure_url;
}
        }

        btnGuardar.textContent = 'Guardando mesa...';

   const data = {
            IdMesa: mesaEditando ? mesaEditando.IdMesa : 0,
            IdRestaurante: 2,
     NumeroMesa: numeroMesa,
    TipoMesa: tipoMesa,
Capacidad: capacidad,
Precio: precio,
            ImagenURL: imagenFinal,
   Estado: estado
        };

 const response = await fetch(`${API}/gestionar`, {
     method: "POST",
      headers: { "Content-Type": "application/json" },
     body: JSON.stringify(data)
      });

 if (response.ok) {
     const result = await response.json();
        showSuccess(
           result.mensaje || "Mesa guardada correctamente", 
     "Éxito", 
      function() {
              cerrarModal();
   cargarMesas();
                }
            );
        } else {
            const error = await response.text();
       showError("Error al guardar la mesa: " + error);
         // 🔥 Rehabilitar botón en caso de error
            btnGuardar.disabled = false;
     btnGuardar.textContent = textoOriginal;
 }

    } catch (error) {
        showError("Error al guardar la mesa: " + error.message);
        console.error(error);
        // 🔥 Rehabilitar botón en caso de error
        btnGuardar.disabled = false;
        btnGuardar.textContent = textoOriginal;
    } finally {
        // 🔥 Liberar el bloqueo después de 2 segundos para prevenir spam
        setTimeout(() => {
     guardandoMesa = false;
   if (btnGuardar) {
   btnGuardar.disabled = false;
           btnGuardar.textContent = textoOriginal;
   }
        }, 2000);
    }
}

// ===============================
// CAMBIAR FOTO
// ===============================
async function cambiarFoto(idMesa) {
    const input = document.createElement("input");
    input.type = "file";
    input.accept = "image/*";

    input.onchange = async e => {
        const file = e.target.files[0];
        if (!file) return;

        const cloud = await subirImagenMesa(file);
        const nuevaURL = cloud.secure_url;

        const mesa = mesasGlobal.find(m => m.IdMesa === idMesa);

        fetch(`${API}/gestionar`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                ...mesa,
                ImagenURL: nuevaURL
            })
        }).then(() => cargarMesas());
    };

    input.click();
}

// ===============================
// ELIMINAR FOTO
// ===============================
function eliminarFoto(idMesa) {
    showConfirm("¿Estás seguro de que deseas eliminar la foto de esta mesa?",
        function() {
            // Usuario confirmó - eliminar foto
   const mesa = mesasGlobal.find(m => m.IdMesa === idMesa);

 fetch(`${API}/gestionar`, {
     method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
    ...mesa,
   ImagenURL: ""
     })
   })
 .then(() => {
      showSuccess("Foto eliminada correctamente");
       cargarMesas();
   })
  .catch(err => {
       showError("Error al eliminar la foto");
      console.error(err);
    });
   },
  function() {
     // Usuario canceló - no hacer nada
   console.log('Eliminación de foto cancelada');
        }
    );
}

// ===============================
// ELIMINAR MESA → INACTIVA
// ===============================
function eliminarMesa(idMesa) {
    showConfirm("¿Estás seguro de que deseas INACTIVAR esta mesa?",
 function() {
  // Usuario confirmó - inactivar mesa
   fetch(`${API}/${idMesa}`, {
   method: "DELETE"
  })
             .then(r => r.json())
  .then(data => {
       showSuccess(data.mensaje || "Mesa inactivada correctamente", "Mesa Inactivada", function() {
       cargarMesas();
 });
       })
   .catch(err => {
   showError("Error al inactivar la mesa");
   console.error(err);
        });
 },
        function() {
    // Usuario canceló - no hacer nada
 console.log('Inactivación de mesa cancelada');
 }
    );
}

// ===============================
// ACTIVAR MESA
// ===============================
function activarMesa(idMesa) {
    showConfirm("¿Deseas ACTIVAR esta mesa?",
        function() {
  // Usuario confirmó - activar mesa
  fetch(`${API}/${idMesa}/estado`, {
      method: "PUT",
           headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Estado: "Disponible" })
     })
  .then(r => r.json())
     .then(() => {
    showSuccess("Mesa activada correctamente", "Mesa Activada", function() {
              cargarMesas();
      });
  })
  .catch(err => {
   showError("Error al activar la mesa");
   console.error(err);
 });
        },
  function() {
   // Usuario canceló - no hacer nada
       console.log('Activación de mesa cancelada');
  }
);
}

// ===============================
// CERRAR
// ===============================
function cerrarModal() {
    // 🔥 Resetear flag de guardado
    guardandoMesa = false;
    
    // 🔥 Rehabilitar botón de guardar si estaba deshabilitado
    const btnGuardar = document.querySelector('.btn-save');
    if (btnGuardar) {
        btnGuardar.disabled = false;
        btnGuardar.textContent = 'Guardar';
    }
    
    document.getElementById("modal").style.display = "none";
}
