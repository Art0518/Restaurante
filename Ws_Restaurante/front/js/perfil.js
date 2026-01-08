// URL base de la API
const API = "http://cafesanjuanr.runasp.net/api";

// Función para verificar si un usuario está logueado
function verificarUsuarioLogueado() {
    const usuario = JSON.parse(localStorage.getItem("usuario"));
    
 if (!usuario) {
      document.getElementById("perfilContainer").style.display = "none";
document.getElementById("loginRequired").style.display = "block";
        return null;
 } else {
        return usuario;
    }
}

// Función para cargar datos del usuario en los campos
function cargarDatosUsuario(usuario) {
    if (usuario) {
     document.getElementById("Email").value = usuario.Email || "";
     document.getElementById("Nombre").value = usuario.Nombre || "";
        document.getElementById("Cedula").value = usuario.Cedula || "";
        document.getElementById("Telefono").value = usuario.Telefono || "";
  document.getElementById("Direccion").value = usuario.Direccion || "";
    }
}

// Cargar usuario desde localStorage
const usuario = verificarUsuarioLogueado();

// Si hay un usuario, cargar sus datos
cargarDatosUsuario(usuario);

// ======================
// FUNCIONES DE NOTIFICACIÓN
// ======================

function mostrarMensajeError(mensaje) {
    const notificaciones = document.getElementById("notificacionesPerfil");
    const mensajeError = document.getElementById("mensajeError");
    const mensajeExito = document.getElementById("mensajeExito");
    
    // Ocultar mensaje de éxito si está visible
    mensajeExito.style.display = "none";
    
  // Mostrar mensaje de error
    mensajeError.textContent = mensaje;
    mensajeError.style.display = "block";
    notificaciones.style.display = "block";
    
    // Scroll hacia arriba para que se vea el mensaje
 document.querySelector('.section').scrollIntoView({ behavior: 'smooth' });
    
  // Auto-ocultar después de 8 segundos
    setTimeout(() => {
        mensajeError.style.display = "none";
        if (mensajeExito.style.display === "none") {
    notificaciones.style.display = "none";
}
    }, 8000);
}

function mostrarMensajeExito(mensaje) {
    const notificaciones = document.getElementById("notificacionesPerfil");
  const mensajeError = document.getElementById("mensajeError");
    const mensajeExito = document.getElementById("mensajeExito");
    
 // Ocultar mensaje de error si está visible
    mensajeError.style.display = "none";
    
    // Mostrar mensaje de éxito
  mensajeExito.textContent = mensaje;
    mensajeExito.style.display = "block";
    notificaciones.style.display = "block";
    
    // Scroll hacia arriba para que se vea el mensaje
    document.querySelector('.section').scrollIntoView({ behavior: 'smooth' });
    
 // Auto-ocultar después de 5 segundos
    setTimeout(() => {
    mensajeExito.style.display = "none";
        if (mensajeError.style.display === "none") {
         notificaciones.style.display = "none";
    }
    }, 5000);
}

function ocultarMensajes() {
    document.getElementById("mensajeError").style.display = "none";
  document.getElementById("mensajeExito").style.display = "none";
    document.getElementById("notificacionesPerfil").style.display = "none";
}

// ======================
// FUNCIONES DE VALIDACIÓN
// ======================

function validarNombreCompleto(nombre) {
    if (!nombre) return { valido: false, mensaje: "El nombre es obligatorio" };
    
    const palabras = nombre.trim().split(' ').filter(p => p.length > 0);
    if (palabras.length < 2) {
     return { valido: false, mensaje: "Debe incluir nombre y apellido" };
    }
  
    if (!/^[a-zA-ZñÑáéíóúÁÉÍÓÚ\s]+$/.test(nombre)) {
      return { valido: false, mensaje: "Solo se permiten letras" };
    }
    
    return { valido: true, mensaje: "✓ Nombre válido" };
}

function validarCedula(cedula) {
    if (!cedula) return { valido: false, mensaje: "La cédula es obligatoria" };
    
    const cedulaLimpia = cedula.replace(/[-\s]/g, '');
    if (!/^\d{11}$/.test(cedulaLimpia)) {
        return { valido: false, mensaje: "Debe tener 11 dígitos (XXX-XXXXXXX-X)" };
 }
    
    return { valido: true, mensaje: "✓ Cédula válida" };
}

function validarEmail(email) {
    if (!email) return { valido: false, mensaje: "El email es obligatorio" };
    
    if (!/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(email)) {
        return { valido: false, mensaje: "Formato de email inválido" };
    }
    
    return { valido: true, mensaje: "✓ Email válido" };
}

function validarTelefono(telefono) {
    if (!telefono) return { valido: false, mensaje: "El teléfono es obligatorio" };
    
    // Remover espacios, guiones, paréntesis y el +1
    const telefonoLimpio = telefono.replace(/[\s\-\(\)\+]/g, '');
    
    // 🔥 Remover el 1 inicial solo si el número tiene más de 10 dígitos
    let telefonoFinal = telefonoLimpio;
    if (telefonoLimpio.length === 11 && telefonoLimpio.startsWith('1')) {
        telefonoFinal = telefonoLimpio.substring(1);
    }
  
    // Validar que tenga exactamente 10 dígitos y comience con 809, 829 o 849
    if (!/^(809|829|849)\d{7}$/.test(telefonoFinal)) {
   return { valido: false, mensaje: "Debe comenzar con 809, 829 o 849 (10 dígitos)" };
 }
    
    return { valido: true, mensaje: "✓ Teléfono válido" };
}

function validarDireccion(direccion) {
    if (!direccion) return { valido: false, mensaje: "La dirección es obligatoria" };
    
    if (direccion.length < 10) {
        return { valido: false, mensaje: "Debe tener al menos 10 caracteres" };
    }
    
    if (!/.*\d.*/.test(direccion) || !/.*[a-zA-Z].*/.test(direccion)) {
        return { valido: false, mensaje: "Debe incluir números y letras" };
    }
    
 return { valido: true, mensaje: "✓ Dirección válida" };
}

function validarContrasena(contrasena) {
    // La contraseña es opcional en actualización
    if (!contrasena) return { valido: true, mensaje: "Contraseña no será cambiada" };
    
    if (contrasena.length < 8) {
        return { valido: false, mensaje: "Debe tener al menos 8 caracteres" };
    }
    
    if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(contrasena)) {
    return { valido: false, mensaje: "Debe tener mayúscula, minúscula y número" };
 }
    
    return { valido: true, mensaje: "✓ Contraseña válida" };
}

// ======================
// MOSTRAR VALIDACIÓN EN TIEMPO REAL
// ======================

function mostrarValidacion(campo, resultado) {
    const input = document.getElementById(campo);
  const validationDiv = document.getElementById(campo + "Validation");
  
    if (resultado.valido) {
  input.classList.remove('error');
        input.classList.add('valid');
     validationDiv.className = 'validation-message success';
  validationDiv.textContent = resultado.mensaje;
     validationDiv.style.display = 'block';
    } else {
        input.classList.remove('valid');
      input.classList.add('error');
  validationDiv.className = 'validation-message error';
        validationDiv.textContent = resultado.mensaje;
   validationDiv.style.display = 'block';
    }
}

// ======================
// FORMATEO AUTOMÁTICO
// ======================

document.addEventListener('DOMContentLoaded', function() {
 // 🔥 INICIALIZAR USUARIO AL CARGAR LA PÁGINA
    const usuario = verificarUsuarioLogueado();
    if (usuario) {
      cargarDatosUsuario(usuario);
    }

    // Auto-formatear cédula
    const cedulaInput = document.getElementById("Cedula");
    if (cedulaInput) {
   cedulaInput.addEventListener('input', function() {
   let valor = this.value.replace(/\D/g, '');
            if (valor.length > 11) valor = valor.substring(0, 11);
     
      if (valor.length > 3 && valor.length <= 10) {
           valor = valor.substring(0, 3) + '-' + valor.substring(3);
      }
        if (valor.length > 11) {
    valor = valor.substring(0, 11) + '-' + valor.substring(11);
   }
          
  this.value = valor;
       mostrarValidacion('Cedula', validarCedula(valor));
        });
    }

    // Auto-formatear teléfono
    const telInput = document.getElementById("Telefono");
    if (telInput) {
   telInput.addEventListener('input', function() {
      let valor = this.value.replace(/\D/g, '');
        if (valor.length > 10) valor = valor.substring(0, 10);
     
    if (valor.length > 3 && valor.length <= 6) {
 valor = valor.substring(0, 3) + '-' + valor.substring(3);
   } else if (valor.length > 6) {
    valor = valor.substring(0, 3) + '-' + valor.substring(3, 6) + '-' + valor.substring(6);
        }
        
       this.value = valor;
     mostrarValidacion('Telefono', validarTelefono(valor));
        });
    }

    // Validación en tiempo real para otros campos
    const nombreInput = document.getElementById("Nombre");
    if (nombreInput) {
     nombreInput.addEventListener('input', function() {
     mostrarValidacion('Nombre', validarNombreCompleto(this.value));
        });
    }

    const emailInput = document.getElementById("Email");
if (emailInput) {
    emailInput.addEventListener('input', function() {
         mostrarValidacion('Email', validarEmail(this.value));
 });
    }

    const direccionInput = document.getElementById("Direccion");
    if (direccionInput) {
 direccionInput.addEventListener('input', function() {
        mostrarValidacion('Direccion', validarDireccion(this.value));
 });
    }

    const contrasenaInput = document.getElementById("Contrasena");
    if (contrasenaInput) {
  contrasenaInput.addEventListener('input', function() {
         mostrarValidacion('Contrasena', validarContrasena(this.value));
        });
    }

    console.log("🔧 Perfil.js inicializado correctamente");
});

// ======================
// ACTUALIZAR PERFIL CON VALIDACIONES
// ======================

function actualizarPerfil() {
 // 🔥 VERIFICAR USUARIO DE MANERA ROBUSTA
    const usuario = JSON.parse(localStorage.getItem("usuario"));
    if (!usuario) {
    mostrarMensajeError("❌ Debes iniciar sesión para actualizar tu perfil.");
    return;
    }

    // Obtener valores
    const nombre = document.getElementById("Nombre").value.trim();
    const cedula = document.getElementById("Cedula").value.trim();
    const email = document.getElementById("Email").value.trim();
    const telefono = document.getElementById("Telefono").value.trim();
    const direccion = document.getElementById("Direccion").value.trim();
    const contrasena = document.getElementById("Contrasena").value.trim();

 // Validar todos los campos
    const validaciones = [
     { campo: 'Nombre', resultado: validarNombreCompleto(nombre) },
        { campo: 'Cedula', resultado: validarCedula(cedula) },
     { campo: 'Email', resultado: validarEmail(email) },
        { campo: 'Telefono', resultado: validarTelefono(telefono) },
        { campo: 'Direccion', resultado: validarDireccion(direccion) },
        { campo: 'Contrasena', resultado: validarContrasena(contrasena) }
    ];

    // Mostrar todas las validaciones
    let todosValidos = true;
    let erroresGenerales = [];
    
    validaciones.forEach(v => {
        mostrarValidacion(v.campo, v.resultado);
        if (!v.resultado.valido) {
          todosValidos = false;
          erroresGenerales.push(`• ${v.resultado.mensaje}`);
        }
    });

    if (!todosValidos) {
        // 🔥 MOSTRAR ERRORES EN LA PARTE SUPERIOR
        const mensajeCompleto = "Por favor corrige los siguientes errores:\n\n" + erroresGenerales.join('\n');
        mostrarMensajeError(mensajeCompleto);
  return;
    }

    // 🔥 OCULTAR MENSAJES ANTERIORES SI TODO ESTÁ VÁLIDO
    ocultarMensajes();

    // Preparar datos (solo enviar contraseña si no está vacía)
    const data = {
        Nombre: nombre,
        Cedula: cedula,
 Email: email,
        Telefono: telefono,
        Direccion: direccion
    };

    if (contrasena) {
        data.Contrasena = contrasena;
}

    // Deshabilitar botón durante la actualización
    const btnActualizar = document.getElementById("btnActualizar");
    btnActualizar.disabled = true;
    btnActualizar.textContent = "Actualizando...";

  // Enviar al servidor
    fetch(`${API}/usuarios/edit/${usuario.IdUsuario}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    })
    .then(response => {
      // 🔥 MANEJAR RESPUESTAS DE ERROR DEL SERVIDOR
    if (response.ok) {
   return response.json();
     } else {
            return response.text().then(text => {
      try {
       const errorData = JSON.parse(text);
throw new Error(errorData.message || errorData.Message || text);
     } catch {
               throw new Error(text || `Error ${response.status}: ${response.statusText}`);
              }
          });
 }
    })
    .then(resultado => {
        if (resultado.mensaje && resultado.mensaje.includes("correctamente")) {
            // Actualizar localStorage
usuario.Nombre = nombre;
         usuario.Cedula = cedula;
  usuario.Email = email;
     usuario.Telefono = telefono;
     usuario.Direccion = direccion;
    
   localStorage.setItem("usuario", JSON.stringify(usuario));
     
            mostrarMensajeExito("✅ Perfil actualizado correctamente");
  
    // Limpiar campo contraseña por seguridad
      document.getElementById("Contrasena").value = "";
       } else {
          mostrarMensajeError(resultado.mensaje || "Error al actualizar el perfil");
   }
    })
    .catch(error => {
        console.error('Error:', error);
        
        // 🔥 MOSTRAR ERRORES ESPECÍFICOS DEL SERVIDOR
        let errorMessage = error.message || "Error de conexión con el servidor";
        
 // Detectar errores específicos de duplicados
      if (errorMessage.includes("correo") || errorMessage.includes("email") || errorMessage.includes("Email")) {
      mostrarMensajeError("Ya existe otro usuario con este correo electrónico. Por favor, usa otro email.");
        } else if (errorMessage.includes("cédula") || errorMessage.includes("cedula") || errorMessage.includes("Cedula")) {
        mostrarMensajeError("Ya existe otro usuario con esta cédula. Por favor, verifica tu número de cédula.");
     } else {
         mostrarMensajeError(errorMessage);
    }
    })
  .finally(() => {
        // Rehabilitar botón
   btnActualizar.disabled = false;
        btnActualizar.textContent = "Guardar cambios";
    });
}
