using System;
using System.Data;
using AccesoDatos.DAO;
using GDatos.Entidades;
using Logica.Validaciones;

namespace Logica.Servicios
{
    public class RestauranteLogica
    {
        private readonly RestauranteDAO dao = new RestauranteDAO();

        // ✅ Listar todos los restaurantes
        public DataTable ListarRestaurantes()
        {
            return dao.ListarRestaurantes();
        }

        // ✅ Obtener detalle de un restaurante por ID
        public DataTable DetalleRestaurante(int idRestaurante)
        {
            if (idRestaurante <= 0)
                throw new Exception("El ID del restaurante no es válido.");

            return dao.DetalleRestaurante(idRestaurante);
        }

        // ✅ Crear o actualizar restaurante
        public void GestionarRestaurante(Restaurante r)
        {
            // 🧩 Validaciones de negocio antes de guardar
            if (!ValidacionRestaurante.NombreValido(r.Nombre))
                throw new Exception("El nombre del restaurante no es válido.");

            if (string.IsNullOrEmpty(r.Ciudad))
                throw new Exception("Debe especificar la ciudad.");

            if (string.IsNullOrEmpty(r.Direccion))
                throw new Exception("Debe indicar la dirección del restaurante.");

            // Si pasa las validaciones, se envía al DAO
            dao.GestionarRestaurante(r.IdRestaurante, r.Nombre, r.Ciudad, r.Direccion, r.Horario, r.Descripcion);
        }

        // ✅ Eliminar restaurante
        public void EliminarRestaurante(int idRestaurante)
        {
            if (idRestaurante <= 0)
                throw new Exception("El ID del restaurante no es válido.");

            dao.EliminarRestaurante(idRestaurante);
        }
    }
}
