using MenuService.Data;
using MenuService.Models;
using Microsoft.Extensions.Configuration;

namespace MenuService.GraphQL
{
    public class MenuMutation
    {
      private readonly string _connectionString;

        public MenuMutation(IConfiguration configuration)
      {
      _connectionString = configuration.GetConnectionString("DefaultConnection");
  }

  // Mutations para Platos
        public Plato CrearPlato(PlatoInput input)
      {
    var platoDAO = new PlatoDAO(_connectionString);
       var plato = new Plato
     {
       IdRestaurante = input.IdRestaurante,
 Nombre = input.Nombre,
  Descripcion = input.Descripcion,
   Precio = input.Precio,
     Categoria = input.Categoria,
 TipoComida = input.TipoComida,
    ImagenURL = input.ImagenURL,
            Stock = input.Stock,
Estado = "ACTIVO"
   };
            return platoDAO.CrearPlato(plato);
  }

        public bool ActualizarPlato(int id, PlatoInput input)
    {
    var platoDAO = new PlatoDAO(_connectionString);
  var plato = new Plato
  {
     IdPlato = id,
   IdRestaurante = input.IdRestaurante,
  Nombre = input.Nombre,
Descripcion = input.Descripcion,
        Precio = input.Precio,
   Categoria = input.Categoria,
        TipoComida = input.TipoComida,
     ImagenURL = input.ImagenURL,
  Stock = input.Stock,
    Estado = "ACTIVO"
   };
     return platoDAO.ActualizarPlato(plato);
     }

     public bool EliminarPlato(int id)
  {
    var platoDAO = new PlatoDAO(_connectionString);
       return platoDAO.EliminarPlato(id);
      }

    // Mutations para Promociones
     public Promocion CrearPromocion(PromocionInput input)
        {
 var promocionDAO = new PromocionDAO(_connectionString);
 var promocion = new Promocion
  {
    IdRestaurante = input.IdRestaurante,
  Nombre = input.Nombre,
Descuento = input.Descuento,
     FechaInicio = input.FechaInicio,
     FechaFin = input.FechaFin,
  Estado = "Activa"
 };
            return promocionDAO.CrearPromocion(promocion);
        }

        public bool ActualizarPromocion(int id, PromocionInput input)
        {
    var promocionDAO = new PromocionDAO(_connectionString);
   var promocion = new Promocion
     {
     IdPromocion = id,
            IdRestaurante = input.IdRestaurante,
     Nombre = input.Nombre,
     Descuento = input.Descuento,
   FechaInicio = input.FechaInicio,
   FechaFin = input.FechaFin,
       Estado = "Activa"
  };
  return promocionDAO.ActualizarPromocion(promocion);
        }

   public bool EliminarPromocion(int id)
        {
     var promocionDAO = new PromocionDAO(_connectionString);
         return promocionDAO.EliminarPromocion(id);
  }
 }

    public class PlatoInput
    {
  public int IdRestaurante { get; set; }
  public string Nombre { get; set; }
        public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string Categoria { get; set; }
        public string TipoComida { get; set; }
  public string ImagenURL { get; set; }
        public int Stock { get; set; }
 }

    public class PromocionInput
    {
        public int IdRestaurante { get; set; }
 public string Nombre { get; set; }
    public decimal Descuento { get; set; }
        public DateTime FechaInicio { get; set; }
  public DateTime FechaFin { get; set; }
    }
}
