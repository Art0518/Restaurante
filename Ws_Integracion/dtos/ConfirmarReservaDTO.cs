using System;

namespace Ws_GIntegracionBus.Dtos
{
    public class ConfirmarReservaDTO
    {
        public DateTime fecha { get; set; }
        public string hora { get; set; }
        public int bookingUserId { get; set; }
        public int idMesa { get; set; }
        public int personas { get; set; }   // ✅ Campo agregado
        public string metodoPago { get; set; }
    }
}
