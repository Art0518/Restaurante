using System;
using System.Data;
using AccesoDatos.DAO;
using GDatos.Entidades;

namespace Logica.Servicios
{
    public class PlatoLogica
    {
        private readonly PlatoDAO dao = new PlatoDAO();

        // ✅ Listar todos los platos del menú
        public DataTable ListarPlatos()
        {
            return dao.ListarPlatos();
        }

        // ✅ Crear, actualizar o eliminar plato
        public void GestionarPlato(Plato p)
        {
            if (string.IsNullOrEmpty(p.Operacion))
                throw new Exception("Debe especificar la operación (INSERT, UPDATE, DELETE).");

            // Validaciones según operación
            switch (p.Operacion.ToUpper())
            {
                case "INSERT":
                case "UPDATE":
                    if (string.IsNullOrEmpty(p.Nombre))
                        throw new Exception("El nombre del plato es obligatorio.");
                    if (string.IsNullOrEmpty(p.TipoComida))
                        throw new Exception("Debe especificar el tipo de plato.");
                    if (p.Precio <= 0)
                        throw new Exception("El precio debe ser mayor a 0.");
                    break;

                case "DELETE":
                    if (p.IdPlato <= 0)
                        throw new Exception("Debe especificar el Id del plato para eliminar.");
                    break;

                default:
                    throw new Exception("Operación no reconocida. Use INSERT, UPDATE o DELETE.");
            }

            // ✅ Llamar al DAO
            dao.GestionarPlato(
                p.Operacion,
                p.IdPlato,
                p.IdRestaurante,
                p.Nombre,
                p.Categoria,
                p.TipoComida,
                p.Precio,
                p.Descripcion,
                p.ImagenURL
            );
        }
    }
}
