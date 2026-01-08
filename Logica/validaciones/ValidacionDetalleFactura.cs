namespace Logica.Validaciones
{
    public static class ValidacionDetalleFactura
    {
        public static bool CantidadValida(int cantidad)
        {
            return cantidad > 0 && cantidad <= 50;
        }

        public static bool PrecioValido(decimal precio)
        {
            return precio > 0 && precio <= 1000;
        }
    }
}
