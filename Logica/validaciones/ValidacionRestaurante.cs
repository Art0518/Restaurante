namespace Logica.Validaciones
{
    public static class ValidacionRestaurante
    {
        public static bool NombreValido(string nombre)
        {
            return !string.IsNullOrEmpty(nombre) && nombre.Length <= 100;
        }
    }
}
