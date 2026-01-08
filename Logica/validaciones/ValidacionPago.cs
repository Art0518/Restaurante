using System.Linq;

namespace Logica.Validaciones
{
    public static class ValidacionPago
    {
        // Métodos de pago permitidos
        public static bool MetodoValido(string metodo)
        {
            string[] permitidos = { "TRANSFERENCIA" };
            return !string.IsNullOrEmpty(metodo) && permitidos.Contains(metodo.ToUpper());
        }

        // Validar monto de pago
        public static bool MontoValido(decimal monto)
        {
            return monto > 0 && monto <= 10000;
        }
    }
}
