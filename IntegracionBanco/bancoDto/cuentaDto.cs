using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegracionBanco.bancoDto
{
    public class cuentaDto
    {
        public int cuenta_id { get; set; }
        public string cliente_id { get; set; }
        public string tipo_cuenta { get; set; }
        public decimal saldo { get; set; }
        public DateTime? fecha_creacion { get; set; }
        public object Clientes { get; set; }
        public List<object> Servicios { get; set; }
        public List<object> Transacciones { get; set; }
        public List<object> Transacciones1 { get; set; }
        public object TiposCuentas { get; set; }
        public override string ToString()
        {
            return $"Cuenta ID: {cuenta_id}, Cliente ID: {cliente_id}, Tipo: {tipo_cuenta}, " +
                   $"Saldo: {saldo:C}, Fecha: {fecha_creacion:yyyy-MM-dd HH:mm:ss}";
        }

    }

}
