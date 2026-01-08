using System.Linq;

namespace Logica.Validaciones
{
    public static class ValidacionFactura
    {
        public static bool EstadoValido(string estado)
        {
            string[] permitidos = { "ACT", "APR", "ANU", "PEN" }; // ACT=Activa, APR=Aprobada, ANU=Anulada, PEN=Pendiente
            return !string.IsNullOrEmpty(estado) && permitidos.Contains(estado.ToUpper());
        }

        public static bool MontosValidos(decimal subtotal, decimal iva, decimal total)
        {
            return subtotal >= 0 && iva >= 0 && total >= 0;
        }
    }
}
