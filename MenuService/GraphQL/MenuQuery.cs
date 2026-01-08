using MenuService.Data;
using MenuService.Models;
using Microsoft.Extensions.Configuration;

namespace MenuService.GraphQL
{
    public class MenuQuery
    {
        private readonly string _connectionString;

        public MenuQuery(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Queries para Platos
        public List<Plato> GetPlatos()
        {
            var platoDAO = new PlatoDAO(_connectionString);
            return platoDAO.ListarTodosPlatos();
        }

        public Plato GetPlato(int id)
        {
            var platoDAO = new PlatoDAO(_connectionString);
            return platoDAO.ObtenerPlatoById(id);
        }

        public List<Plato> GetPlatosPorCategoria(string categoria)
        {
            var platoDAO = new PlatoDAO(_connectionString);
            return platoDAO.ListarPlatosPorCategoria(categoria);
        }

        // Queries para Promociones
        public List<Promocion> GetPromociones()
        {
            var promocionDAO = new PromocionDAO(_connectionString);
            return promocionDAO.ListarTodasPromociones();
        }

        public Promocion GetPromocion(int id)
        {
            var promocionDAO = new PromocionDAO(_connectionString);
            return promocionDAO.ObtenerPromocionById(id);
        }

        public List<Promocion> GetPromocionesActivas()
        {
            var promocionDAO = new PromocionDAO(_connectionString);
            return promocionDAO.ListarPromocionesActivas();
        }
    }
}
