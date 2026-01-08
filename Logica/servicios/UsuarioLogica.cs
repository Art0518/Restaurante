using System;
using System.Data;
using AccesoDatos.DAO;
using GDatos.Entidades;
using Logica.Validaciones;

namespace Logica.Servicios
{
    public class UsuarioLogica
    {
        private readonly UsuarioDAO dao = new UsuarioDAO();

        // Login
        public DataTable Login(string email, string contrasena)
        {
            if (!ValidacionUsuario.EmailValido(email))
                throw new Exception("El correo ingresado no es válido.");

            if (string.IsNullOrEmpty(contrasena))
                throw new Exception("Debe ingresar la contraseña.");

            return dao.Login(email, contrasena);
        }

        // Registrar nuevo usuario
        public void Registrar(Usuario u)
        {
// 🔥 VALIDACIONES COMPLETAS
            if (string.IsNullOrEmpty(u.Nombre))
        throw new Exception("El nombre es obligatorio.");
            
   if (!ValidacionUsuario.NombreCompletoValido(u.Nombre))
       throw new Exception("Debe ingresar nombre y apellido válidos (solo letras, mínimo 2 palabras).");

    if (string.IsNullOrEmpty(u.Email))
          throw new Exception("El correo electrónico es obligatorio.");

    if (!ValidacionUsuario.EmailValido(u.Email))
          throw new Exception("El formato del correo electrónico es inválido.");

   if (string.IsNullOrEmpty(u.Cedula))
                throw new Exception("La cédula es obligatoria.");

            if (!ValidacionUsuario.CedulaValida(u.Cedula))
             throw new Exception("La cédula debe tener formato válido (11 dígitos: XXX-XXXXXXX-X).");

   if (string.IsNullOrEmpty(u.Telefono))
    throw new Exception("El teléfono es obligatorio.");

          if (!ValidacionUsuario.TelefonoValido(u.Telefono))
      throw new Exception("El teléfono debe ser válido (809/829/849 + 7 dígitos).");

         if (string.IsNullOrEmpty(u.Direccion))
     throw new Exception("La dirección es obligatoria.");

            if (!ValidacionUsuario.DireccionValida(u.Direccion))
    throw new Exception("La dirección debe ser válida (mínimo 10 caracteres, incluir números y letras).");

        if (string.IsNullOrEmpty(u.Contrasena))
    throw new Exception("La contraseña es obligatoria.");

      if (!ValidacionUsuario.ContrasenaValida(u.Contrasena))
       throw new Exception("La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula y un número.");

       // Asignar valores por defecto si no se especifican
 if (string.IsNullOrEmpty(u.Rol))
      u.Rol = "CLIENTE";
   
         if (string.IsNullOrEmpty(u.Estado))
       u.Estado = "ACTIVO";

     dao.Registrar(u);
        }

        // Listar usuarios
        public DataTable Listar(string rol = null, string estado = null)
        {
            return dao.Listar(rol, estado);
        }

        // Actualizar usuario
        public void Actualizar(Usuario u)
        {
            if (u.IdUsuario <= 0)
                throw new Exception("ID de usuario no válido.");

            // 🔥 VALIDACIONES PARA ACTUALIZACIÓN
      // Solo validar campos que no están vacíos (actualización parcial)
    
   if (!string.IsNullOrEmpty(u.Nombre) && !ValidacionUsuario.NombreCompletoValido(u.Nombre))
  throw new Exception("Debe ingresar nombre y apellido válidos (solo letras, mínimo 2 palabras).");

            if (!string.IsNullOrEmpty(u.Email) && !ValidacionUsuario.EmailValido(u.Email))
    throw new Exception("El formato del correo electrónico es inválido.");

if (!string.IsNullOrEmpty(u.Cedula) && !ValidacionUsuario.CedulaValida(u.Cedula))
       throw new Exception("La cédula debe tener formato válido (11 dígitos: XXX-XXXXXXX-X).");

        if (!string.IsNullOrEmpty(u.Telefono) && !ValidacionUsuario.TelefonoValido(u.Telefono))
   throw new Exception("El teléfono debe ser válido (809/829/849 + 7 dígitos).");

      if (!string.IsNullOrEmpty(u.Direccion) && !ValidacionUsuario.DireccionValida(u.Direccion))
             throw new Exception("La dirección debe ser válida (mínimo 10 caracteres, incluir números y letras).");

            if (!string.IsNullOrEmpty(u.Contrasena) && !ValidacionUsuario.ContrasenaValida(u.Contrasena))
     throw new Exception("La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula y un número.");

    dao.Actualizar(u);
   }

        // Cambiar estado
        public void CambiarEstado(int idUsuario, string nuevoEstado)
        {
            if (string.IsNullOrEmpty(nuevoEstado))
                throw new Exception("Debe especificar un estado.");
            dao.CambiarEstado(idUsuario, nuevoEstado);
        }
    }
}
