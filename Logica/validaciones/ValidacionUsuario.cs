using System.Text.RegularExpressions;

namespace Logica.Validaciones
{
    public static class ValidacionUsuario
    {
        // Validar formato de correo
        public static bool EmailValido(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        // 🔥 VALIDAR CÉDULA (formato dominicano: XXX-XXXXXXX-X)
        public static bool CedulaValida(string cedula)
        {
            if (string.IsNullOrEmpty(cedula)) return false;

            // Remover espacios y guiones para validar solo números
            string cedulaLimpia = cedula.Replace("-", "").Replace(" ", "");

            // Debe tener exactamente 11 dígitos
            if (cedulaLimpia.Length != 11) return false;

            // Debe ser solo números
            return Regex.IsMatch(cedulaLimpia, @"^\d{11}$");
        }

        // 🔥 VALIDAR TELÉFONO (formato dominicano)
        public static bool TelefonoValido(string telefono)
        {
            if (string.IsNullOrEmpty(telefono)) return false;

            // Remover espacios, guiones y paréntesis
            string telefonoLimpio = telefono.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "");

            // Debe tener 10 dígitos (809/829/849 + 7 dígitos)
            if (telefonoLimpio.Length != 10) return false;

            // Debe comenzar con 809, 829 o 849
            return Regex.IsMatch(telefonoLimpio, @"^(809|829|849)\d{7}$");
        }

        // 🔥 VALIDAR NOMBRE Y APELLIDO (al menos 2 palabras)
        public static bool NombreCompletoValido(string nombre)
        {
            if (string.IsNullOrEmpty(nombre)) return false;

            // Remover espacios extra
            string nombreLimpio = nombre.Trim();

            // Dividir en palabras
            string[] palabras = nombreLimpio.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            // Debe tener al menos 2 palabras (nombre y apellido)
            if (palabras.Length < 2) return false;

            // Cada palabra debe tener al menos 2 caracteres y solo letras
            foreach (string palabra in palabras)
            {
                if (palabra.Length < 2 || !Regex.IsMatch(palabra, @"^[a-zA-ZñÑáéíóúÁÉÍÓÚ]+$"))
                    return false;
            }

            return true;
        }

        // 🔥 VALIDAR DIRECCIÓN (debe tener contenido mínimo)
        public static bool DireccionValida(string direccion)
        {
            if (string.IsNullOrEmpty(direccion)) return false;

            string direccionLimpia = direccion.Trim();

            // Debe tener al menos 10 caracteres y contener al menos una palabra "calle", "sector", "avenida", etc.
            if (direccionLimpia.Length < 10) return false;

            // Debe contener al menos números y letras (dirección real)
            return Regex.IsMatch(direccionLimpia, @".*\d.*") && Regex.IsMatch(direccionLimpia, @".*[a-zA-Z].*");
        }

        // 🔥 VALIDAR CONTRASEÑA FUERTE
        public static bool ContrasenaValida(string contrasena)
        {
            if (string.IsNullOrEmpty(contrasena)) return false;

            // Al menos 8 caracteres
            if (contrasena.Length < 8) return false;

            // Al menos una mayúscula, una minúscula y un número
            return Regex.IsMatch(contrasena, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$");
        }
    }
}
