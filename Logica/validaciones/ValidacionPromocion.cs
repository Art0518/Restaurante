using System;
using System.Linq;

namespace Logica.Validaciones
{
    public static class ValidacionPromocion
    {
        // ✅ Validar fechas coherentes (inicio <= fin y no pasadas)
        public static bool FechasValidas(DateTime inicio, DateTime fin)
        {
            return inicio.Date <= fin.Date && fin.Date >= DateTime.Now.Date;
        }

        // ✅ Validar estado permitido
        public static bool EstadoValido(string estado)
        {
            string[] permitidos = { "ACTIVA", "INACTIVA", "VENCIDA" };
            return !string.IsNullOrEmpty(estado) && permitidos.Contains(estado.ToUpper());
        }
    }
}
