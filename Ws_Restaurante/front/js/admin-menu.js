// ========================================
// CONFIG
// ========================================
const API_PLATOS = "http://cafesanjuanr.runasp.net/api/platos";
const CLOUD_NAME = "dt7fbonmg";
const UPLOAD_PRESET = "menu_upload";

let platosGlobal = [];
let platoEditando = null;
let guardandoPlato = false; // Variable para prevenir múltiples envíos

// ========================================
// INICIO - VALIDAR ADMIN Y CARGAR PLATOS
// ========================================
document.addEventListener("DOMContentLoaded", () => {
    const usuario = JSON.parse(localStorage.getItem("usuario"));

    if (!usuario || usuario.Rol !== "ADMIN") {
  showNotification("Acceso denegado. Solo administradores.", "error", "Acceso Denegado", () => {
            window.location.href = "index.html";
        });
        return;
    }

    // Esperar a que los elementos del DOM estén disponibles
setTimeout(() => {
    cargarPlatos();
        
        // Agregar event listeners para los filtros
        const busquedaInput = document.getElementById("busquedaPlato");
      if (busquedaInput) {
        busquedaInput.addEventListener("input", aplicarFiltros);
        }
        
 // Agregar event listener para el formulario
    const formPlato = document.getElementById("formPlato");
        if (formPlato) {
            formPlato.addEventListener("submit", guardarPlato);
        }
        
        // Agregar event listener para preview de imagen
        const imagenInput = document.getElementById("imagenPlatoFile");
        if (imagenInput) {
            imagenInput.addEventListener("change", previewImagen);
 }
    }, 100);
});

// ========================================
// CARGAR PLATOS
// ========================================
function cargarPlatos() {
    fetch(API_PLATOS)
    .then(r => r.json())
        .then(data => {
platosGlobal = data;
    renderPlatos(data);
   })
        .catch(err => {
       console.error("Error cargando platos:", err);
      showNotification("No se pudieron cargar los platos. Intenta de nuevo.", "error", "Error de Carga");
     });
}

// ========================================
// RENDERIZAR LISTA DE PLATOS
// ========================================
function renderPlatos(lista) {
    const cont = document.getElementById("gridPlatos");
    const sinResultados = document.getElementById("sinResultados");
    
    if (!cont) {
        console.error("No se encontró el elemento gridPlatos");
        return;
    }
    
    if (!sinResultados) {
        console.error("No se encontró el elemento sinResultados");
    }
    
    cont.innerHTML = "";

    if (!lista || !lista.length) {
   cont.style.display = "none";
        if (sinResultados) {
        sinResultados.style.display = "block";
        }
        return;
    }
    
    cont.style.display = "grid";
    if (sinResultados) {
   sinResultados.style.display = "none";
    }

    lista.forEach(p => {
  const nombre = p.Nombre || p.Plato;

        cont.innerHTML += `
        <div class="plato-card">
            <img src="${p.ImagenURL || "img/placeholder.jpg"}" alt="${nombre}" class="plato-imagen">
            
        <div class="plato-content">
 <h3 class="plato-nombre">${nombre}</h3>
      <p class="plato-descripcion">${p.Descripcion || ''}</p>
              <p style="font-size: 13px; color: #7a7a7a;"><b>Tipo:</b> ${p.Categoria}</p>
        <div class="plato-precio">$${parseFloat(p.Precio).toFixed(2)}</div>
     <span class="plato-estado estado-disponible">Disponible</span>
       
    <div class="plato-acciones">
         <button class="btn-editar" onclick="editarPlato(${p.IdPlato})">Editar</button>
        <button class="btn-eliminar" onclick="eliminarPlato(${p.IdPlato})">Eliminar</button>
        </div>
            </div>
        </div>`;
    });
}

// ========================================
// FILTROS
// ========================================
function aplicarFiltros() {
    const filtroTexto = document.getElementById("busquedaPlato").value.toLowerCase();

    let lista = [...platosGlobal];

    if (filtroTexto) {
        lista = lista.filter(p =>
            (p.Nombre || p.Plato || "").toLowerCase().includes(filtroTexto) ||
        (p.Descripcion || "").toLowerCase().includes(filtroTexto)
   );
    }

    renderPlatos(lista);
}

// ========================================
// SUBIR IMAGEN A CLOUDINARY
// ========================================
async function subirImagenCloudinary(file) {
    const formData = new FormData();
    formData.append("file", file);
    formData.append("upload_preset", UPLOAD_PRESET);

    const url = `https://api.cloudinary.com/v1_1/${CLOUD_NAME}/image/upload`;

    try {
  const response = await fetch(url, {
        method: "POST",
 body: formData
  });

        if (!response.ok) {
          throw new Error("Error al subir la imagen");
   }

        const data = await response.json();
    return data.secure_url;
    } catch (error) {
   console.error("Error subiendo imagen:", error);
        showNotification("Error al subir la imagen a Cloudinary. Verifica tu conexion.", "error", "Error de Red");
        return null;
    }
}

// ========================================
// PREVIEW DE IMAGEN
// ========================================
function previewImagen() {
    const fileInput = document.getElementById("imagenPlatoFile");
    const preview = document.getElementById("previewImagen");
 
    if (fileInput && fileInput.files && fileInput.files[0]) {
        const reader = new FileReader();
        reader.onload = function(e) {
    if (preview) {
     preview.src = e.target.result;
       preview.style.display = "block";
            }
        };
   reader.readAsDataURL(fileInput.files[0]);
    }
}

// ========================================
// MODAL NUEVO PLATO
// ========================================
function abrirModalNuevo() {
    platoEditando = null;

    document.getElementById("tituloModal").textContent = "Nuevo Plato";
    document.getElementById("nombrePlato").value = "";
    document.getElementById("descripcionPlato").value = "";
    document.getElementById("precioPlato").value = "";
  document.getElementById("categoriaPlato").value = "";
    
    // Limpiar preview y file input
    const fileInput = document.getElementById("imagenPlatoFile");
    const preview = document.getElementById("previewImagen");
    if (fileInput) fileInput.value = "";
    if (preview) {
     preview.src = "";
        preview.style.display = "none";
    }

    document.getElementById("modalPlato").style.display = "block";
}

// ========================================
// MODAL EDITAR PLATO
// ========================================
function editarPlato(idPlato) {
    platoEditando = platosGlobal.find(p => p.IdPlato === idPlato);

    if (!platoEditando) return;

    document.getElementById("tituloModal").textContent = "Editar Plato";
    document.getElementById("nombrePlato").value = platoEditando.Nombre || platoEditando.Plato;
    document.getElementById("descripcionPlato").value = platoEditando.Descripcion || "";
    document.getElementById("precioPlato").value = platoEditando.Precio;
    document.getElementById("categoriaPlato").value = platoEditando.Categoria;
    
 // Mostrar preview de la imagen actual
    const preview = document.getElementById("previewImagen");
    if (preview && platoEditando.ImagenURL) {
        preview.src = platoEditando.ImagenURL;
        preview.style.display = "block";
    }
    
    // Limpiar file input
    const fileInput = document.getElementById("imagenPlatoFile");
    if (fileInput) fileInput.value = "";

    document.getElementById("modalPlato").style.display = "block";
}

// ========================================
// ELIMINAR PLATO
// ========================================
function eliminarPlato(idPlato) {
    const plato = platosGlobal.find(p => p.IdPlato === idPlato);
    if (!plato) return;

    const nombreReal = plato.Nombre || plato.Plato;

    // Usar showConfirm en lugar de confirm nativo
    showConfirm(
        `Estas seguro de que deseas eliminar el plato "${nombreReal}"?`,
        () => {
            // Si el usuario confirma
       const body = {
         IdPlato: plato.IdPlato,
IdRestaurante: 2,
      Nombre: nombreReal,
    Categoria: plato.Categoria,
  TipoComida: plato.TipoComida || plato.Categoria,
     Precio: parseFloat(plato.Precio),
         Descripcion: plato.Descripcion,
   ImagenURL: plato.ImagenURL || "",
     Disponible: true,
  Operacion: "DELETE"
            };

    fetch(`${API_PLATOS}/gestionar`, {
      method: "POST",
headers: { "Content-Type": "application/json" },
 body: JSON.stringify(body)
        })
        .then(response => {
    if (response.ok) {
       showNotification(`El plato "${nombreReal}" ha sido eliminado exitosamente.`, "success", "Plato Eliminado");
     cargarPlatos();
   } else {
        showNotification("No se pudo eliminar el plato. Intenta de nuevo.", "error", "Error al Eliminar");
     }
    })
     .catch(err => {
          console.error("Error eliminando plato:", err);
        showNotification("Ocurrio un error al eliminar el plato.", "error", "Error");
   });
        }
    );
}

// ========================================
// GUARDAR PLATO (NUEVO O EDITAR)
// ========================================
async function guardarPlato(e) {
    e.preventDefault();

// Prevenir múltiples envíos
    if (guardandoPlato) {
        showNotification("Ya se esta procesando un plato. Por favor espera.", "warning", "Procesando");
return;
  }

    // Validar nombre
    const nombre = document.getElementById("nombrePlato").value.trim();

    // Validación usando charCodeAt para evitar problemas de encoding
    let esNombreValido = true;
for (let i = 0; i < nombre.length; i++) {
  const char = nombre[i];
  const code = nombre.charCodeAt(i);
        
  // Permitir: letras (A-Z, a-z), espacios, ñ (241), Ñ (209), tildes, ü
     const esLetra = (code >= 65 && code <= 90) || (code >= 97 && code <= 122);
   const esEspacio = code === 32;
        const esEnie = code === 241 || code === 209; // ñ, Ñ
   const esTilde = (code >= 224 && code <= 252); // á-ü, À-Ü
    
     if (!esLetra && !esEspacio && !esEnie && !esTilde) {
    esNombreValido = false;
    break;
        }
    }
    
  if (!esNombreValido || nombre.length === 0) {
        showNotification("El nombre solo debe contener letras y espacios.", "error", "Nombre Invalido");
     return;
 }

    // Validar descripción
    const descripcion = document.getElementById("descripcionPlato").value.trim();
    
    let esDescripcionValida = true;
    for (let i = 0; i < descripcion.length; i++) {
   const code = descripcion.charCodeAt(i);
     
        // Permitir: letras, números, espacios, ñ, tildes, puntuación básica
        const esLetra = (code >= 65 && code <= 90) || (code >= 97 && code <= 122);
  const esNumero = code >= 48 && code <= 57;
      const esEspacio = code === 32;
const esEnie = code === 241 || code === 209;
const esTilde = (code >= 224 && code <= 252);
        const esPuntuacion = [44, 46, 59, 58, 40, 41, 191, 161, 33, 63, 45, 34].includes(code);
   
     if (!esLetra && !esNumero && !esEspacio && !esEnie && !esTilde && !esPuntuacion) {
  esDescripcionValida = false;
          break;
        }
    }
    
    if (!esDescripcionValida || descripcion.length === 0) {
        showNotification("La descripcion contiene caracteres no permitidos.", "error", "Descripcion Invalida");
  return;
    }

    // Validar que el precio sea mayor a 0
    const precio = parseFloat(document.getElementById("precioPlato").value || 0);
    if (precio <= 0) {
        showNotification("El precio debe ser mayor a 0.", "error", "Precio Invalido");
      return;
    }

    // Marcar como guardando
  guardandoPlato = true;
    
    // Deshabilitar botón de guardar
    const btnGuardar = document.querySelector(".btn-guardar");
    if (btnGuardar) {
        btnGuardar.disabled = true;
     btnGuardar.textContent = "Guardando...";
    }

    try {
        let imagenURL = platoEditando ? (platoEditando.ImagenURL || "") : "";
    
// Verificar si hay un archivo para subir
     const fileInput = document.getElementById("imagenPlatoFile");
        if (fileInput && fileInput.files && fileInput.files[0]) {
       showNotification("Subiendo imagen a Cloudinary...", "info", "Procesando");
       imagenURL = await subirImagenCloudinary(fileInput.files[0]);
        
if (!imagenURL) {
      showNotification("Error al subir la imagen. Intenta de nuevo.", "error", "Error de Carga");
     guardandoPlato = false;
          if (btnGuardar) {
     btnGuardar.disabled = false;
           btnGuardar.textContent = "Guardar";
       }
   return;
            }
   }

        const body = {
   IdPlato: platoEditando ? platoEditando.IdPlato : 0,
  IdRestaurante: 2,
   Nombre: nombre,
    Categoria: document.getElementById("categoriaPlato").value,
   TipoComida: document.getElementById("categoriaPlato").value,
  Precio: precio,
     Descripcion: descripcion,
       ImagenURL: imagenURL,
   Disponible: true,
     Operacion: platoEditando ? "UPDATE" : "INSERT"
        };

  const res = await fetch(`${API_PLATOS}/gestionar`, {
   method: "POST",
  headers: { "Content-Type": "application/json" },
     body: JSON.stringify(body)
   });

        if (!res.ok) {
     throw new Error("Error al guardar el plato");
        }

        const mensaje = platoEditando 
   ? `El plato "${nombre}" ha sido actualizado exitosamente.`
     : `El plato "${nombre}" ha sido creado exitosamente.`;
        
        showNotification(mensaje, "success", platoEditando ? "Plato Actualizado" : "Plato Creado", () => {
      cerrarModal();
   cargarPlatos();
        });

    } catch (error) {
        console.error("Error guardando plato:", error);
    showNotification("Ocurrio un error al guardar el plato. Intenta de nuevo.", "error", "Error");
    } finally {
    // Siempre liberar el estado de guardando
   guardandoPlato = false;
      if (btnGuardar) {
   btnGuardar.disabled = false;
        btnGuardar.textContent = "Guardar";
        }
    }
}

// ========================================
// CERRAR MODAL
// ========================================
function cerrarModal() {
    document.getElementById("modalPlato").style.display = "none";
  platoEditando = null;
    
    // Restablecer el estado de guardando al cerrar
    guardandoPlato = false;
    const btnGuardar = document.querySelector(".btn-guardar");
    if (btnGuardar) {
    btnGuardar.disabled = false;
    btnGuardar.textContent = "Guardar";
    }
}
