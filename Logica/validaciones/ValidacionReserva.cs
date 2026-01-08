using System;

namespace Logica.Validaciones
{
    public static class ValidacionReserva
    {
        // ✅ La fecha no puede ser en el pasado
        public static bool FechaValida(DateTime fecha)
        {
            return fecha.Date >= DateTime.Now.Date;
        }

        // ✅ La hora debe tener formato HH:mm
        public static bool HoraValida(string hora)
        {
            return TimeSpan.TryParse(hora, out _);
        }
    }
}
