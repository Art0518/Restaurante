using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ws_GIntegracionBus.DTOS
{
    public class UsuarioRequestDTO
    {
        public string nombre { get; set; }
        public string email { get; set; }
        public string contrasena { get; set; }
        public string rol { get; set; }
        public string telefono { get; set; }
        public string direccion { get; set; }
    }
}

