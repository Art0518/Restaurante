// ======================
// ABRIR / CERRAR MODAL
// ======================
function openAuthModal() {
    document.getElementById("auth-modal-overlay").style.display = "flex";
    showLogin();
}

function closeAuthModal() {
    document.getElementById("auth-modal-overlay").style.display = "none";
}


// ======================
// CAMBIAR ENTRE LOGIN Y REGISTRO
// ======================
function showRegister() {
    document.getElementById("form-login").style.display = "none";
    document.getElementById("form-register").style.display = "block";
    document.getElementById("auth-title").innerText = "Crear Cuenta";
}

function showLogin() {
    document.getElementById("form-login").style.display = "block";
    document.getElementById("form-register").style.display = "none";
    document.getElementById("auth-title").innerText = "Iniciar Sesión";
}


// ======================
// LOGIN (AJAX A TU API)
// ======================
function login() {
    const email = document.getElementById("login-email").value.trim();
    const pass = document.getElementById("login-pass").value.trim();

    if (!email || !pass) {
        showWarning("Por favor, completa todos los campos");
  return;
    }

    fetch("http://cafesanjuanr.runasp.net/api/usuarios/login", {
        method: "POST",
  headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ Email: email, Contrasena: pass })
    })
.then(res => res.json())
  .then(data => {
   if (data.mensaje !== "Login exitoso") {
showError("Credenciales incorrectas. Por favor, verifica tu email y contraseña.");
   return;
   }

            // 🔥 NUEVA VALIDACIÓN: usuario inactivo NO puede iniciar
   if (data.usuario.Estado !== "ACTIVO") {
    showError("Tu cuenta está INACTIVA. Por favor, contacta al administrador.");
      return;
   }

     // Guardar sesión solo si está ACTIVO
 localStorage.setItem("usuario", JSON.stringify({
    IdUsuario: data.usuario.IdUsuario,
  Nombre: data.usuario.Nombre,
    Email: data.usuario.Email,
      Cedula: data.usuario.Cedula,  // 🔥 INCLUIR CÉDULA
       Telefono: data.usuario.Telefono,
     Rol: data.usuario.Rol,
      Estado: data.usuario.Estado,
                Direccion: data.usuario.Direccion ?? ""
    }));

   // 🔥 MOSTRAR MENSAJE Y ESPERAR A QUE EL USUARIO PRESIONE ACEPTAR
    showSuccess("¡Bienvenido!", "Inicio de sesión exitoso", function() {
    closeAuthModal();
   location.reload();
      });
        })
        .catch(err => {
  console.log(err);
   showError("Error de conexión. Por favor, verifica tu conexión a internet.");
 });
}



// ======================
// REGISTRO CON VALIDACIONES COMPLETAS
// ======================
function registrar() {
    // Obtener valores
    const nombre = document.getElementById("reg-nombre").value.trim();
  const cedula = document.getElementById("reg-cedula").value.trim();
    const email = document.getElementById("reg-email").value.trim();
    const contrasena = document.getElementById("reg-pass").value.trim();
    const telefono = document.getElementById("reg-tel").value.trim();
const direccion = document.getElementById("reg-dir").value.trim();

    // 🔥 VALIDACIONES DEL FRONTEND
    if (!nombre || !cedula || !email || !contrasena || !telefono || !direccion) {
        return showWarning("Todos los campos son obligatorios");
    }

    // Validar nombre (al menos 2 palabras)
    const palabras = nombre.split(' ').filter(p => p.length > 0);
    if (palabras.length < 2) {
        return showWarning("Debes ingresar nombre y apellido");
    }
  if (!/^[a-zA-ZñÑáéíóúÁÉÍÓÚ\s]+$/.test(nombre)) {
     return showWarning("El nombre solo puede contener letras");
    }

    // Validar cédula (formato dominicano)
    const cedulaLimpia = cedula.replace(/[-\s]/g, '');
    if (!/^\d{11}$/.test(cedulaLimpia)) {
   return showWarning("La cédula debe tener 11 dígitos (formato: XXX-XXXXXXX-X)");
    }

    // Validar email
    if (!/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(email)) {
        return showWarning("Formato de correo electrónico inválido");
    }

    // Validar contraseña fuerte
    if (contrasena.length < 8) {
      return showWarning("La contraseña debe tener al menos 8 caracteres");
  }
    if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(contrasena)) {
 return showWarning("La contraseña debe tener al menos una mayúscula, una minúscula y un número");
    }

    // Validar teléfono dominicano
    const telefonoLimpio = telefono.replace(/[\s\-\(\)\+]/g, '');
  
    // 🔥 Remover el 1 inicial solo si el número tiene más de 10 dígitos
    let telefonoFinal = telefonoLimpio;
    if (telefonoLimpio.length === 11 && telefonoLimpio.startsWith('1')) {
        telefonoFinal = telefonoLimpio.substring(1);
    }
    
    if (!/^(809|829|849)\d{7}$/.test(telefonoFinal)) {
        return showWarning("Teléfono inválido. Debe comenzar con 809, 829 o 849 y tener 10 dígitos");
    }

    // Validar dirección
    if (direccion.length < 10) {
        return showWarning("La dirección debe tener al menos 10 caracteres");
    }
    if (!/.*\d.*/.test(direccion) || !/.*[a-zA-Z].*/.test(direccion)) {
        return showWarning("La dirección debe incluir números y letras");
    }

    // Crear objeto usuario
   const usuario = {
      Nombre: nombre,
      Cedula: cedula,
  Email: email,
     Contrasena: contrasena,
    Telefono: telefono,
      Direccion: direccion,
      Rol: "CLIENTE",
       Estado: "ACTIVO"
 };

    // Enviar al servidor
   fetch("http://cafesanjuanr.runasp.net/api/usuarios/registrar", {
       method: "POST",
  headers: { "Content-Type": "application/json" },
        body: JSON.stringify(usuario)
    })
        .then(res => {
  // 🔥 MEJORAR MANEJO DE RESPUESTAS
            if (res.ok) {
    return res.json();
   } else {
       // Si hay error HTTP, intentar obtener el mensaje del servidor
     return res.text().then(text => {
    try {
         const errorData = JSON.parse(text);
    throw new Error(errorData.message || errorData.Message || text);
     } catch {
         throw new Error(text || `Error ${res.status}: ${res.statusText}`);
   }
      });
    }
     })
  .then(data => {
    // 🔥 REGISTRO EXITOSO
     if (data.mensaje && data.mensaje.includes("correctamente")) {
     // Limpiar campos
            document.getElementById("reg-nombre").value = "";
   document.getElementById("reg-cedula").value = "";
            document.getElementById("reg-email").value = "";
   document.getElementById("reg-pass").value = "";
document.getElementById("reg-tel").value = "";
      document.getElementById("reg-dir").value = "";

    // Mostrar mensaje y cambiar a login cuando el usuario presione Aceptar
         showSuccess("Usuario registrado correctamente", "¡Registro Exitoso!", function() {
                    showLogin();
    });
      } else {
         // Error del servidor con mensaje específico
  showError(data.mensaje || data.message || "Error en el registro");
   }
})
        .catch(err => {
        console.error('Error de registro:', err);
  
  // 🔥 MOSTRAR ERRORES ESPECÍFICOS DEL SERVIDOR
 let errorMessage = err.message || "Error de conexión";
   
            // Detectar errores específicos de duplicados
   if (errorMessage.includes("correo") || errorMessage.includes("email") || errorMessage.includes("Email")) {
  showError("Ya existe un usuario con este correo electrónico. Por favor, usa otro email.");
   } else if (errorMessage.includes("cédula") || errorMessage.includes("cedula") || errorMessage.includes("Cedula")) {
    showError("Ya existe un usuario con esta cédula. Por favor, verifica tu número de cédula.");
   } else if (errorMessage.includes("conexión") || errorMessage.includes("network") || errorMessage.includes("fetch")) {
  showError("Error de conexión con el servidor. Verifica tu conexión a internet.");
  } else {
   showError(errorMessage);
   }
     });
}

// ======================
// FUNCIONES GLOBALES DE RESPALDO
// ======================

// Asegurar que las funciones estén disponibles globalmente
window.openAuthModal = openAuthModal;
window.closeAuthModal = closeAuthModal;
window.showLogin = showLogin;
window.showRegister = showRegister;
window.login = login;
window.registrar = registrar;

// Función de inicialización cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function() {
  console.log("🔧 Auth.js inicializado correctamente");
    console.log("🔍 Funciones disponibles:", {
     openAuthModal: typeof openAuthModal !== 'undefined',
       closeAuthModal: typeof closeAuthModal !== 'undefined',
        showLogin: typeof showLogin !== 'undefined',
        showRegister: typeof showRegister !== 'undefined',
        login: typeof login !== 'undefined',
        registrar: typeof registrar !== 'undefined'
    });
});

// ======================
// FORMATEO AUTOMÁTICO DE CAMPOS
// ======================
document.addEventListener('DOMContentLoaded', function() {
 // Auto-formatear cédula
   const cedulaInput = document.getElementById("reg-cedula");
   if (cedulaInput) {
      cedulaInput.addEventListener('input', function() {
           let valor = this.value.replace(/\D/g, ''); // Solo números
if (valor.length > 11) valor = valor.substring(0, 11);
     
       // Formatear XXX-XXXXXXX-X
       if (valor.length > 3 && valor.length <= 10) {
   valor = valor.substring(0, 3) + '-' + valor.substring(3);
        }
       if (valor.length > 11) {
  valor = valor.substring(0, 11) + '-' + valor.substring(11);
     }
        
       this.value = valor;
  });
   }

  // Auto-formatear teléfono
    const telInput = document.getElementById("reg-tel");
  if (telInput) {
   telInput.addEventListener('input', function() {
         let valor = this.value.replace(/\D/g, ''); // Solo números
     if (valor.length > 10) valor = valor.substring(0, 10);
  
      // Formatear XXX-XXX-XXXX
       if (valor.length > 3 && valor.length <= 6) {
    valor = valor.substring(0, 3) + '-' + valor.substring(3);
         } else if (valor.length > 6) {
        valor = valor.substring(0, 3) + '-' + valor.substring(3, 6) + '-' + valor.substring(6);
}
           
   this.value = valor;
      });
  }
    
 console.log("🔧 Auth.js inicializado correctamente con validaciones");
});
