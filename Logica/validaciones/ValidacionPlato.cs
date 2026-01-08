using System.Linq;

namespace Logica.Validaciones
{
    public static class ValidacionPlato
    {
        // Estado permitido: ACTIVO o INACTIVO
        public static bool EstadoValido(string estado)
        {
            string[] permitidos = { "ACTIVO", "INACTIVO" };
            return !string.IsNullOrEmpty(estado) && permitidos.Contains(estado.ToUpper());
        }

        // Validar rango de precio razonable
        public static bool PrecioValido(decimal precio)
        {
            return precio >= 0.50m && precio <= 500.00m;
        }
    }
}
