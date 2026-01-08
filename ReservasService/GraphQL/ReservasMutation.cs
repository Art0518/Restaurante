using ReservasService.Data;
using ReservasService.Models;
using Microsoft.Extensions.Configuration;

namespace ReservasService.GraphQL
{
    public class ReservasMutation
    {
private readonly string _connectionString;

        public ReservasMutation(IConfiguration configuration)
        {
      _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

  // Mutations para Reservas
        public CrearReservaResponse CrearReserva(ReservaInput input)
        {
 var reservaDAO = new ReservaDAO(_connectionString);
    var dt = reservaDAO.CrearReserva(
   input.IdUsuario,
 input.IdMesa,
  input.Fecha,
    input.Hora,
  input.NumeroPersonas,
            input.Observaciones ?? "",
     input.Estado ?? "HOLD",
            input.MetodoPago ?? ""
        );

      if (dt.Rows.Count == 0)
    return new CrearReservaResponse { Success = false, Message = "Error desconocido" };

      var row = dt.Rows[0];
    return new CrearReservaResponse
       {
   Success = row["Estado"].ToString() == "SUCCESS" || row["Estado"].ToString() == "EXITO",
            Message = row["Mensaje"].ToString(),
            IdReserva = row["IdReserva"] != DBNull.Value ? Convert.ToInt32(row["IdReserva"]) : 0
 };
        }

        public bool ActualizarEstadoReserva(int id, string estado, string metodoPago)
{
    var reservaDAO = new ReservaDAO(_connectionString);
       reservaDAO.ActualizarEstado(id, estado, metodoPago ?? "");
  return true;
        }

     // Mutations para Mesas
        public bool CambiarEstadoMesa(int id, string estado)
     {
       var mesaDAO = new MesaDAO(_connectionString);
            mesaDAO.ActualizarEstado(id, estado);
  return true;
        }

        // Mutations para Restaurantes
   public Restaurante CrearRestaurante(RestauranteInput input)
        {
var restauranteDAO = new RestauranteDAO(_connectionString);
          var restaurante = new Restaurante
 {
    Nombre = input.Nombre,
       Direccion = input.Direccion,
    Telefono = input.Telefono,
   HorarioApertura = input.HorarioApertura,
  HorarioCierre = input.HorarioCierre,
     Activo = true,
   FechaCreacion = DateTime.Now
  };
  return restauranteDAO.CrearRestaurante(restaurante);
        }

  public bool ActualizarRestaurante(int id, RestauranteInput input)
  {
    var restauranteDAO = new RestauranteDAO(_connectionString);
       var restaurante = new Restaurante
   {
     IdRestaurante = id,
    Nombre = input.Nombre,
   Direccion = input.Direccion,
  Telefono = input.Telefono,
   HorarioApertura = input.HorarioApertura,
      HorarioCierre = input.HorarioCierre,
 Activo = true,
      FechaCreacion = DateTime.Now
    };
  return restauranteDAO.ActualizarRestaurante(restaurante);
        }
    }

  public class ReservaInput
    {
        public int IdUsuario { get; set; }
   public int IdMesa { get; set; }
        public DateTime Fecha { get; set; }
     public string Hora { get; set; }
        public int NumeroPersonas { get; set; }
public string Observaciones { get; set; }
     public string Estado { get; set; }
  public string MetodoPago { get; set; }
    }

    public class RestauranteInput
  {
        public string Nombre { get; set; }
     public string Direccion { get; set; }
     public string Telefono { get; set; }
  public string HorarioApertura { get; set; }
        public string HorarioCierre { get; set; }
  }

    public class CrearReservaResponse
    {
     public bool Success { get; set; }
  public string Message { get; set; }
  public int IdReserva { get; set; }
    }
}
