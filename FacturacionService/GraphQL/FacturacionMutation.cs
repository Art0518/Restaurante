using FacturacionService.Data;
using FacturacionService.Models;
using Microsoft.Extensions.Configuration;

namespace FacturacionService.GraphQL
{
    public class FacturacionMutation
    {
        private readonly string _connectionString;

        public FacturacionMutation(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public GenerarFacturaResponse GenerarFacturaDesdeCarrito(GenerarFacturaInput input)
        {
            var facturaDAO = new FacturaDAO(_connectionString);
            var dt = facturaDAO.GenerarFacturaCarrito(
                input.IdUsuario,
                input.ReservasIds,
                input.PromocionId,
                input.MetodoPago ?? ""
            );

            if (dt.Rows.Count == 0)
                return null;

            var row = dt.Rows[0];
            return new GenerarFacturaResponse
            {
                Estado = row["Estado"].ToString(),
                Mensaje = row["Mensaje"].ToString(),
                IdFactura = Convert.ToInt32(row["IdFactura"]),
                Subtotal = Convert.ToDecimal(row["Subtotal"]),
                IVA = Convert.ToDecimal(row["IVA"]),
                Total = Convert.ToDecimal(row["Total"])
            };
        }

        public MarcarPagadaResponse MarcarFacturaPagada(MarcarPagadaInput input)
        {
            var facturaDAO = new FacturaDAO(_connectionString);
            var dt = facturaDAO.MarcarFacturaPagada(input.IdFactura, input.MetodoPago);

            if (dt.Rows.Count == 0)
                return new MarcarPagadaResponse { Success = false, Message = "Error desconocido" };

            var row = dt.Rows[0];
            return new MarcarPagadaResponse
            {
                Success = row["Estado"].ToString() == "SUCCESS",
                Message = row["Mensaje"].ToString(),
                IdFactura = Convert.ToInt32(row["IdFactura"])
            };
        }

        public bool AnularFactura(int idFactura)
        {
            var facturaDAO = new FacturaDAO(_connectionString);
            facturaDAO.AnularFactura(idFactura);
            return true;
        }
    }

    public class GenerarFacturaInput
    {
        public int IdUsuario { get; set; }
        public string ReservasIds { get; set; }
        public int? PromocionId { get; set; }
        public string MetodoPago { get; set; }
    }

    public class MarcarPagadaInput
    {
        public int IdFactura { get; set; }
        public string MetodoPago { get; set; }
    }

    public class GenerarFacturaResponse
    {
        public string Estado { get; set; }
        public string Mensaje { get; set; }
        public int IdFactura { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
    }

    public class MarcarPagadaResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int IdFactura { get; set; }
    }
}
