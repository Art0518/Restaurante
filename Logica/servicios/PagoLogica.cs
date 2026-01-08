using System;
using System.Data;
using AccesoDatos.DAO;
using GDatos.Entidades;
using Logica.Validaciones;

namespace Logica.Servicios
{
    public class PagoLogica
    {
        private readonly PagoDAO dao = new PagoDAO();

        // ✅ Registrar nuevo pago
        public void RegistrarPago(Pago p)
        {
            // 🔍 Validaciones previas
            if (p.IdFactura <= 0)
                throw new Exception("Debe indicar una factura válida.");

            if (!ValidacionPago.MetodoValido(p.MetodoPago))
                throw new Exception("El método de pago no es válido.");

            if (!ValidacionPago.MontoValido(p.Monto))
                throw new Exception("El monto ingresado no es válido.");

            if (string.IsNullOrEmpty(p.TransaccionCodigo))
                throw new Exception("Debe indicar un código de transacción.");

            // ✅ Llamar al DAO
            dao.RegistrarPago(p.IdFactura, p.MetodoPago, p.Monto, p.TransaccionCodigo);
        }

        // ✅ Validar un pago existente
        public DataTable ValidarPago(int idPago)
        {
            if (idPago <= 0)
                throw new Exception("Debe indicar un ID de pago válido.");

            return dao.ValidarPago(idPago);
        }


    }
}
