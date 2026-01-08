using System.Linq;

namespace Logica.Validaciones
{
    public static class ValidacionMesa
    {
        // Capacidad mínima 1 persona y máxima 20
        public static bool CapacidadValida(int capacidad)
        {
            return capacidad >= 1 && capacidad <= 20;
        }

        // Estado permitido (para validaciones de negocio)
        public static bool EstadoValido(string estado)
        {
            string[] permitidos = { "DISPONIBLE", "OCUPADA", "RESERVADA", "INACTIVA" };
            return !string.IsNullOrEmpty(estado) && permitidos.Contains(estado.ToUpper());
        }
    }
}
