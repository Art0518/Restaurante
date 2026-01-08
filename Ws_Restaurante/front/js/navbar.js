// VARIABLES GLOBALES DEL NAVBAR
const usuarioNav = JSON.parse(localStorage.getItem("usuario"));

// FUNCIÓN GLOBAL PARA ABRIR EL MODAL (si no existe)
if (typeof openAuthModal === 'undefined') {
    window.openAuthModal = function() {
     console.log("🔓 Intentando abrir modal de autenticación...");
 const modal = document.getElementById("auth-modal-overlay");
        if (modal) {
    modal.style.display = "flex";
     showLogin(); // Asegurarse de mostrar login por defecto
            console.log("✅ Modal abierto correctamente");
     } else {
        console.error("❌ No se encontró el modal de autenticación");
    console.log("🔍 Intentando cargar modal...");
 // Intentar cargar el modal si no existe
    loadAuthModal();
   }
    };
}

// FUNCIÓN PARA CARGAR EL MODAL DINÁMICAMENTE SI NO EXISTE
function loadAuthModal() {
    if (!document.getElementById("auth-modal-overlay")) {
       fetch("components/auth-modal.html")
       .then(res => res.text())
  .then(html => {
        const container = document.createElement("div");
             container.innerHTML = html;
    document.body.appendChild(container.firstElementChild);
       console.log("✅ Modal de autenticación cargado dinámicamente");
 })
   .catch(error => {
 console.error("❌ Error cargando modal dinámicamente:", error);
      });
    }
}

// FUNCIÓN GLOBAL PARA CERRAR EL MODAL (si no existe)
if (typeof closeAuthModal === 'undefined') {
    window.closeAuthModal = function() {
        const modal = document.getElementById("auth-modal-overlay");
        if (modal) {
       modal.style.display = "none";
        }
    };
}

// FUNCIÓN GLOBAL PARA LOGOUT (con verificación de disponibilidad de modales)
if (typeof logout === 'undefined') {
    window.logout = function() {
        // Verificar si el sistema de notificaciones modales está disponible
        if (typeof showConfirm === 'function') {
       // Usar modal de confirmación
 showConfirm('¿Estás seguro de que deseas cerrar sesión?',
  function() {
     // Usuario confirmó - cerrar sesión
              localStorage.removeItem("usuario");
         if (typeof showSuccess === 'function') {
             showSuccess("Sesión cerrada correctamente", "Hasta pronto", function() {
            window.location.href = "index.html";
  });
       } else {
  // Fallback si showSuccess no está disponible
    window.location.href = "index.html";
       }
    },
     function() {
                // Usuario canceló - no hacer nada
              console.log('Cierre de sesión cancelado');
                }
          );
     } else {
            // Fallback: usar confirm() nativo si los modales no están disponibles
 if (confirm('¿Estás seguro de que deseas cerrar sesión?')) {
         localStorage.removeItem("usuario");
        window.location.href = "index.html";
      }
        }
    };
}

// INICIALIZACIÓN DEL NAVBAR
const navArea = document.getElementById("nav-user-area");
const misReservasArea = document.getElementById("mis-reservas-area");
const clienteLinksArea = document.getElementById("cliente-links");
const adminLinksArea = document.getElementById("admin-links");

// Debug: Log para ver qué elementos se encontraron
console.log("🔍 Elementos del navbar:", {
    navArea: navArea ? "✅ Encontrado" : "❌ No encontrado",
 misReservasArea: misReservasArea ? "✅ Encontrado" : "❌ No encontrado",
    clienteLinksArea: clienteLinksArea ? "✅ Encontrado" : "❌ No encontrado",
    adminLinksArea: adminLinksArea ? "✅ Encontrado" : "❌ No encontrado"
});

if (!navArea) {
console.error("❌ No se encontró nav-user-area");
} else {

    // Si NO está logueado
    if (!usuarioNav) {
        console.log("👤 Usuario NO logueado - Mostrando enlaces de cliente");
        navArea.innerHTML = `
     <button class="modern-login-btn" onclick="openAuthModal()">
      Iniciar sesión
  </button>
        `;

        // Mostrar enlaces de cliente para usuarios no logueados
      if (clienteLinksArea) {
            clienteLinksArea.classList.add("show");
        }
        // Ocultar enlaces de admin
  if (adminLinksArea) {
 adminLinksArea.classList.remove("show");
        }
        // Ocultar mis reservas
        if (misReservasArea) {
 misReservasArea.classList.remove("show");
   }
    }

    // Si SÍ está logueado
    else {
    const nombre = usuarioNav.Nombre || "Usuario";
   const rol = usuarioNav.Rol || "Cliente";

        console.log("👤 Usuario logueado:", nombre);
        console.log("🎭 Rol del usuario:", rol);

   const isAdmin = (rol === "ADMIN");
  
        // Debug temporal para administrador
        console.log("🔍 Verificación de admin:");
   console.log("- Rol encontrado:", rol);
  console.log("- ¿Es admin?:", isAdmin);
        console.log("- Usuario completo:", usuarioNav);
    
    const adminBadge = isAdmin ? '<span class="modern-admin-badge">Admin</span>' : '';

     navArea.innerHTML = `
        <span class="modern-user-name">
   ${nombre} ${adminBadge}
   </span>

<a href="mi-perfil.html" class="modern-nav-link">Mi Perfil</a>

   <button class="modern-login-btn" style="background: linear-gradient(135deg, #c0392b, #e74c3c);" onclick="logout()">
  Cerrar sesión
</button>
        `;

        // 🔵 LÓGICA SEGÚN EL ROL
        if (isAdmin) {
console.log("👨‍💼 Usuario ADMIN - Mostrando solo enlaces de administración");
  
          // Ocultar enlaces de cliente
            if (clienteLinksArea) {
          clienteLinksArea.classList.remove("show");
  }
            
   // Mostrar enlaces de admin
            if (adminLinksArea) {
    adminLinksArea.classList.add("show");
            }
      
   // Ocultar mis reservas para admin
        if (misReservasArea) {
   misReservasArea.classList.remove("show");
     }
        } else {
        console.log("👤 Usuario CLIENTE - Mostrando enlaces de cliente");
            
    // Mostrar enlaces de cliente
    if (clienteLinksArea) {
   clienteLinksArea.classList.add("show");
         }
         
            // Ocultar enlaces de admin
            if (adminLinksArea) {
        adminLinksArea.classList.remove("show");
     }
      
  // Mostrar mis reservas para clientes
            if (misReservasArea) {
           misReservasArea.classList.add("show");
        }
     }
    
        console.log("🔐 Configuración final del navbar:");
        console.log("  - Enlaces de cliente:", clienteLinksArea ? clienteLinksArea.classList.contains("show") : "N/A");
 console.log("  - Enlaces de admin:", adminLinksArea ? adminLinksArea.classList.contains("show") : "N/A");
        console.log("  - Mis reservas:", misReservasArea ? misReservasArea.classList.contains("show") : "N/A");
    }
}
