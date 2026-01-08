using System;
using System.Data;
using AccesoDatos.DAO;
using GDatos.Entidades;

namespace Logica.Servicios
{
    public class DetalleFacturaLogica
    {
        private readonly DetalleFacturaDAO dao = new DetalleFacturaDAO();

        // ✅ Listar detalles por factura con validaciones
        public DataTable ListarDetallesPorFactura(int idFactura)
        {
            try
            {
                if (idFactura <= 0)
                    throw new ArgumentException("ID de factura no válido");

                return dao.ListarDetallesPorFactura(idFactura);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en lógica al listar detalles por factura: " + ex.Message);
            }
        }

        // ✅ Insertar detalle con validaciones de negocio
        public void InsertarDetalle(int idFactura, int idReserva, string descripcion, int cantidad, decimal precioUnitario)
        {
            try
            {
                // Validaciones de negocio
                if (idFactura <= 0)
                    throw new ArgumentException("ID de factura no válido");

                if (idReserva <= 0)
                    throw new ArgumentException("ID de reserva no válido");

                if (string.IsNullOrWhiteSpace(descripcion))
                    throw new ArgumentException("Descripción no puede estar vacía");

                if (cantidad <= 0)
                    throw new ArgumentException("La cantidad debe ser mayor a cero");

                if (precioUnitario < 0)
                    throw new ArgumentException("El precio unitario no puede ser negativo");

                dao.InsertarDetalle(idFactura, idReserva, descripcion, cantidad, precioUnitario);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en lógica al insertar detalle: " + ex.Message);
            }
        }

        // ✅ NUEVO: Actualizar detalle existente
        public void ActualizarDetalle(int idDetalle, string descripcion, int cantidad, decimal precioUnitario)
        {
            try
            {
                // Validaciones de negocio
                if (idDetalle <= 0)
                    throw new ArgumentException("ID de detalle no válido");

                if (string.IsNullOrWhiteSpace(descripcion))
                    throw new ArgumentException("Descripción no puede estar vacía");

                if (cantidad <= 0)
                    throw new ArgumentException("La cantidad debe ser mayor a cero");

                if (precioUnitario < 0)
                    throw new ArgumentException("El precio unitario no puede ser negativo");

                dao.ActualizarDetalle(idDetalle, descripcion, cantidad, precioUnitario);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en lógica al actualizar detalle: " + ex.Message);
            }
        }

        // ✅ NUEVO: Eliminar detalle
        public void EliminarDetalle(int idDetalle)
        {
            try
            {
                if (idDetalle <= 0)
                    throw new ArgumentException("ID de detalle no válido");

                dao.EliminarDetalle(idDetalle);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en lógica al eliminar detalle: " + ex.Message);
            }
        }

        // ✅ NUEVO: Obtener detalle por ID
        public DataTable ObtenerDetallePorId(int idDetalle)
        {
            try
            {
                if (idDetalle <= 0)
                    throw new ArgumentException("ID de detalle no válido");

                return dao.ObtenerDetallePorId(idDetalle);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en lógica al obtener detalle por ID: " + ex.Message);
            }
        }

        // ✅ NUEVO: Calcular subtotal de factura
        public decimal CalcularSubtotalFactura(int idFactura)
        {
            try
            {
                if (idFactura <= 0)
                    throw new ArgumentException("ID de factura no válido");

                return dao.CalcularSubtotalFactura(idFactura);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en lógica al calcular subtotal: " + ex.Message);
            }
        }

        // ✅ NUEVO: Contar detalles de factura
        public int ContarDetallesFactura(int idFactura)
        {
            try
            {
                if (idFactura <= 0)
                    throw new ArgumentException("ID de factura no válido");

                return dao.ContarDetallesFactura(idFactura);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en lógica al contar detalles: " + ex.Message);
            }
        }

        // ✅ MÉTODO AUXILIAR: Validar consistencia de detalle
        public bool ValidarConsistenciaDetalle(int idFactura, int idReserva)
        {
            try
            {
                if (idFactura <= 0 || idReserva <= 0)
                    return false;

                // Aquí puedes agregar lógicas adicionales de validación
                // Por ejemplo, verificar que la reserva pertenezca al mismo usuario que la factura
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ✅ MÉTODO AUXILIAR: Calcular total con IVA desde detalles
        public decimal CalcularTotalConIVA(int idFactura, decimal porcentajeIVA = 0.12m)
        {
            try
            {
                decimal subtotal = CalcularSubtotalFactura(idFactura);
                decimal iva = subtotal * porcentajeIVA;
                return subtotal + iva;
            }
            catch (Exception ex)
            {
                throw new Exception("Error calculando total con IVA: " + ex.Message);
            }
        }

        // ✅ MÉTODO AUXILIAR: Validar estado de factura para modificaciones
        public bool PuedeModificarDetalle(int idFactura)
        {
            try
            {
                // Lógica para verificar si se puede modificar la factura
                // Por ejemplo, verificar que no esté pagada o anulada
                return true; // Simplificado por ahora
            }
            catch
            {
                return false;
            }
        }

        // ✅ MÉTODO AUXILIAR: Recalcular totales de factura
        public void RecalcularTotalesFactura(int idFactura)
        {
            try
            {
                if (idFactura <= 0)
                    throw new ArgumentException("ID de factura no válido");

                decimal nuevoSubtotal = CalcularSubtotalFactura(idFactura);
                decimal nuevoIVA = nuevoSubtotal * 0.12m;
                decimal nuevoTotal = nuevoSubtotal + nuevoIVA;

                // Aquí podrías llamar a FacturaDAO para actualizar los totales
                // dao.ActualizarTotales(idFactura, nuevoSubtotal, nuevoIVA, nuevoTotal);
            }
            catch (Exception ex)
            {
                throw new Exception("Error recalculando totales: " + ex.Message);
            }
        }
    }
}
