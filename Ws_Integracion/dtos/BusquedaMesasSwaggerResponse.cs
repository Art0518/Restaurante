using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ws_GIntegracionBus.DTOS;

namespace Ws_Integracion.dtos
{
    public class BusquedaMesasSwaggerResponse
    {
        public string mensaje { get; set; }
        public int total { get; set; }
        public List<MesaResponse> mesas { get; set; }
        public object _links { get; set; }
    }
}
