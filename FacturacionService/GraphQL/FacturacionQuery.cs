using FacturacionService.Data;
using FacturacionService.Models;
using Microsoft.Extensions.Configuration;

namespace FacturacionService.GraphQL
{
    public class FacturacionQuery
    {
        private readonly string _connectionString;

        public FacturacionQuery(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Factura> GetFacturas()
        {
            var facturaDAO = new FacturaDAO(_connectionString);
            return facturaDAO.ListarTodasFacturas();
        }

        public Factura GetFactura(int id)
        {
            var facturaDAO = new FacturaDAO(_connectionString);
            return facturaDAO.ObtenerFacturaById(id);
        }

        public FacturaDetallada GetFacturaDetallada(int id)
        {
            var facturaDAO = new FacturaDAO(_connectionString);
            var dataset = facturaDAO.ObtenerFacturaDetallada(id);

            if (dataset.Tables.Count == 0 || dataset.Tables[0].Rows.Count == 0)
                return null;

            var facturaRow = dataset.Tables[0].Rows[0];
            var factura = new FacturaDetallada
            {
                IdFactura = Convert.ToInt32(facturaRow["IdFactura"]),
                IdUsuario = Convert.ToInt32(facturaRow["IdUsuario"]),
                Nombre = facturaRow["Nombre"].ToString(),
                Email = facturaRow["Email"].ToString(),
                Telefono = facturaRow["Telefono"].ToString(),
                FechaEmision = Convert.ToDateTime(facturaRow["FechaHora"]),
                Subtotal = Convert.ToDecimal(facturaRow["Subtotal"]),
                IVA = Convert.ToDecimal(facturaRow["IVA"]),
                Total = Convert.ToDecimal(facturaRow["Total"]),
                Estado = facturaRow["Estado"].ToString(),
                MetodoPago = facturaRow["MetodoPago"].ToString(),
                Detalles = new List<DetalleFactura>()
            };

            if (dataset.Tables.Count > 1)
            {
                foreach (System.Data.DataRow detalleRow in dataset.Tables[1].Rows)
                {
                    factura.Detalles.Add(new DetalleFactura
                    {
                        IdDetalleFactura = Convert.ToInt32(detalleRow["IdDetalle"]),
                        IdFactura = Convert.ToInt32(detalleRow["IdFactura"]),
                        Descripcion = detalleRow["Descripcion"].ToString(),
                        Cantidad = Convert.ToInt32(detalleRow["Cantidad"]),
                        PrecioUnitario = Convert.ToDecimal(detalleRow["PrecioUnitario"]),
                        Subtotal = Convert.ToDecimal(detalleRow["Subtotal"])
                    });
                }
            }

            return factura;
        }

        public List<Factura> GetFacturasPorUsuario(int idUsuario)
        {
            var facturaDAO = new FacturaDAO(_connectionString);
            var dt = facturaDAO.ListarFacturasUsuario(idUsuario);
            var facturas = new List<Factura>();

            foreach (System.Data.DataRow row in dt.Rows)
            {
                facturas.Add(new Factura
                {
                    IdFactura = Convert.ToInt32(row["IdFactura"]),
                    IdUsuario = idUsuario,
                    FechaEmision = Convert.ToDateTime(row["FechaHora"]),
                    Subtotal = Convert.ToDecimal(row["Subtotal"]),
                    IVA = Convert.ToDecimal(row["IVA"]),
                    Total = Convert.ToDecimal(row["Total"]),
                    Estado = row["Estado"].ToString(),
                    MetodoPago = row["MetodoPago"].ToString()
                });
            }

            return facturas;
        }
    }

    public class FacturaDetallada
    {
        public int IdFactura { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public string MetodoPago { get; set; }
        public List<DetalleFactura> Detalles { get; set; }
    }
}
