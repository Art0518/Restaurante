using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ws_GIntegracionBus.DTOS
{
    public class UsuarioResponseDTO
    {
        public string mensaje { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public string rol { get; set; }
        public string estado { get; set; }
        public object _links { get; set; }
    }
}

