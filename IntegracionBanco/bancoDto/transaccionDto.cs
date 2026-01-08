using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegracionBanco.bancoDto
{
    public class transaccionDto
    {
        public int cuenta_origen {  get; set; }

        public int cuenta_destino {  get; set; }
        public decimal monto {  get; set; }
        public override string ToString()
        {
            return "transaccion :{"+cuenta_origen+" "+cuenta_destino+" "+monto+"}";
        }
        public transaccionDto inversa()
        {
            return new transaccionDto()
            {
                cuenta_destino = cuenta_origen,
                cuenta_origen = cuenta_destino,
                monto = monto,
            };
        }
    }
}
