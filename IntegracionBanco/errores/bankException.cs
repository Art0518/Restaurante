using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegracionBanco.errores
{
    public  class bankException:Exception
    {
        public bankException(string msg,string lugar=""):base($"Error bancario  {msg} en {(lugar??"banco")}") { }
    }
}
