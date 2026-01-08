using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegracionBanco.bancoDto
{
    public class clienteDto
    {
        public object AutenticacionExterna { get; set; }
        public object Autenticacion { get; set; }
        public List<object> PagosServicios { get; set; }
        public List<cuentaDto> Cuentas { get; set; }
        public string cliente_id { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public List<object> DeudasNueva { get; set; }
        public override string ToString()
        {
            var cuentasDetalle = Cuentas != null && Cuentas.Any()
                ? string.Join("\n", Cuentas.Select(c => $" - {c}"))
                : " - Sin cuentas";

            return $"Cliente: {nombre} ({cliente_id})\n" +
                   $"Dirección: {direccion}\n" +
                   $"Teléfono: {telefono}, Email: {email}\n" +
                   $"Cuentas: {Cuentas?.Count ?? 0}\n{cuentasDetalle}\n" +
                   $"PagosServicios: {PagosServicios?.Count ?? 0}, DeudasNueva: {DeudasNueva?.Count ?? 0}";
        }


    }
}
