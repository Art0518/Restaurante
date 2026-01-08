using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace Ws_GIntegracionBus.DTOS
{
    public class DisponibilidadResponse
    {
        public string mensaje { get; set; }
        public string fecha { get; set; }
        public string hora { get; set; }
        public int numeroPersonas { get; set; }
        public string ciudad { get; set; }
        public bool disponible { get; set; }
        public object _links { get; set; }
    }
}

