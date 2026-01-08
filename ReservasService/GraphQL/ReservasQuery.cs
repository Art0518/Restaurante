using ReservasService.Data;
using ReservasService.Models;
using Microsoft.Extensions.Configuration;

namespace ReservasService.GraphQL
{
    public class ReservasQuery
    {
        private readonly string _connectionString;

        public ReservasQuery(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Queries para Reservas
        public List<ReservaInfo> GetReservas()
        {
            var reservaDAO = new ReservaDAO(_connectionString);
            var dt = reservaDAO.ListarReservas();
            var reservas = new List<ReservaInfo>();

            foreach (System.Data.DataRow row in dt.Rows)
            {
                reservas.Add(new ReservaInfo
                {
                    IdReserva = Convert.ToInt32(row["IdReserva"]),
                    IdUsuario = Convert.ToInt32(row["IdUsuario"]),
                    IdMesa = Convert.ToInt32(row["IdMesa"]),
                    Fecha = Convert.ToDateTime(row["Fecha"]),
                    Hora = row["Hora"].ToString(),
                    NumeroPersonas = Convert.ToInt32(row["NumeroPersonas"]),
                    Estado = row["Estado"].ToString()
                });
            }

            return reservas;
        }

        public List<ReservaInfo> GetReservasPorUsuario(int idUsuario)
        {
            var reservaDAO = new ReservaDAO(_connectionString);
            var dt = reservaDAO.ListarTodasReservasAdmin();
            var reservas = new List<ReservaInfo>();

            foreach (System.Data.DataRow row in dt.Rows)
            {
                if (Convert.ToInt32(row["IdUsuario"]) == idUsuario)
                {
                    reservas.Add(new ReservaInfo
                    {
                        IdReserva = Convert.ToInt32(row["IdReserva"]),
                        IdUsuario = Convert.ToInt32(row["IdUsuario"]),
                        IdMesa = Convert.ToInt32(row["IdMesa"]),
                        Fecha = Convert.ToDateTime(row["Fecha"]),
                        Hora = row["Hora"].ToString(),
                        NumeroPersonas = Convert.ToInt32(row["NumeroPersonas"]),
                        Estado = row["Estado"].ToString()
                    });
                }
            }

            return reservas;
        }

        // Queries para Mesas
        public List<Mesa> GetMesas()
        {
            var mesaDAO = new MesaDAO(_connectionString);
            var dt = mesaDAO.ListarMesas();
            var mesas = new List<Mesa>();

            foreach (System.Data.DataRow row in dt.Rows)
            {
                mesas.Add(new Mesa
                {
                    IdMesa = Convert.ToInt32(row["IdMesa"]),
                    IdRestaurante = Convert.ToInt32(row["IdRestaurante"]),
                    NumeroMesa = Convert.ToInt32(row["NumeroMesa"]),
                    TipoMesa = row["TipoMesa"].ToString(),
                    Capacidad = Convert.ToInt32(row["Capacidad"]),
                    Estado = row["Estado"].ToString()
                });
            }

            return mesas;
        }

        public List<Mesa> GetMesasPorRestaurante(int idRestaurante)
        {
            var mesaDAO = new MesaDAO(_connectionString);
            return mesaDAO.ListarMesasPorRestaurante(idRestaurante);
        }

        // Queries para Restaurantes
        public List<Restaurante> GetRestaurantes()
        {
            var restauranteDAO = new RestauranteDAO(_connectionString);
            return restauranteDAO.ListarRestaurantesActivos();
        }

        public Restaurante GetRestaurante(int id)
        {
            var restauranteDAO = new RestauranteDAO(_connectionString);
            return restauranteDAO.ObtenerRestauranteById(id);
        }
    }

    public class ReservaInfo
    {
        public int IdReserva { get; set; }
        public int IdUsuario { get; set; }
        public int IdMesa { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public int NumeroPersonas { get; set; }
        public string Estado { get; set; }
    }
}
