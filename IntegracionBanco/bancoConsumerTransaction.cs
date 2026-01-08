using IntegracionBanco.bancoDto;
using IntegracionBanco.errores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IntegracionBanco
{
    public static partial class bancoConsumer
    {
        public static async Task<string> getCuentaValida(string cedula,decimal? monto)
        {
            var cliente = await getCliente(cedula);
            if (cliente == null)
                return "NA";
            foreach (var cuenta in cliente.Cuentas)
            {
                if (cuenta.saldo >= monto)
                {
                    return cuenta.cuenta_id+"";
                }
            }
            return "NA";
        }
        public static async Task<bool> SaldoSuficiente(int cuenta,decimal? monto)
        {
            cuentaDto c = await getCuenta(cuenta);
            if(c == null)
                return false;
            if(c.saldo>=monto)
            {
                return true;
            }
            return false;
        }
        public static async Task<string> transaccionValida(clienteDto clienteAux, decimal monto,int cuentaDest)
        {
            try
            {
                var cliente = await bancoConsumer.getCliente(clienteAux.cliente_id);
                if (cliente==null)
                {
                    //cliente = await bancoConsumer.generarCliente(clienteAux);//en caso de que si deba crear ya estaria
                    return "cuenta no creada";
                }
                //ahora busco una cuenta que tenga la plata y/o una cuenta
                if (cliente.Cuentas.Count == 0)
                    return "cliente sin cuentas";
                //recorro las cuentas hasta encontrar una que tenga el saldo bueno
                transaccionDto trs=new transaccionDto();
                foreach (cuentaDto cuenta in cliente.Cuentas)
                {
                    if(cuenta.saldo>monto)
                    {
                        trs = new transaccionDto()
                        {
                            cuenta_origen = cuenta.cuenta_id,
                            cuenta_destino = 85,
                            monto = monto
                        };
                        //llegado aca ya deberia estar todo bien y puedo mandar la transaccion
                        //terminar la funcion aqui
                        await transaccion(trs);
                        return "OK";
                    }
                }
                //si llegue hasta aca es porque no hay saldo, no se puede hacer
                return "sin saldo";
            }
            catch (bankException ex) 
            {
                return ex.Message;
            }
        }
        private static async Task<string> transaccion(transaccionDto transaccion)
        {
            var content = new StringContent(JsonConvert.SerializeObject(transaccion), Encoding.UTF8, "application/json");
            var response = await new HttpClient().PostAsync(url + "/api/Transacciones", content);

            if (!response.IsSuccessStatusCode)
            {
                var detalle = await response.Content.ReadAsStringAsync();
                throw new bankException("transaccionar", $"Código {(int)response.StatusCode} {response.ReasonPhrase}. Detalle: {detalle}");
            }

            return await response.Content.ReadAsStringAsync();
        }
        public static async Task<string> transaccionUnitaria(transaccionDto transaccion)
        {
            Console.WriteLine($"transaccion de {transaccion.cuenta_origen} a {transaccion.cuenta_destino} con {transaccion.monto}");
            var content = new StringContent(JsonConvert.SerializeObject(transaccion), Encoding.UTF8, "application/json");
            var response = await new HttpClient().PostAsync(url + "/api/Transacciones", content);

            if (!response.IsSuccessStatusCode)
            {
                var detalle = await response.Content.ReadAsStringAsync();
            }

            string res=await response.Content.ReadAsStringAsync();
            if(res.Contains("correctamente"))
            {
                return "OK";
            }
            return "ERROR" + res;
        }

        public static async Task<clienteDto> generarCliente(clienteDto clienteAux)
        {
            //si cliente no existe lo creo
            string resultadoCliente = await bancoConsumer.crearCliente(clienteAux);
            Console.WriteLine(resultadoCliente);
            //le creo una cuenta
            string resCUENTA = await bancoConsumer.crearCuenta(new cuentaDto()
            {
                cliente_id = clienteAux.cliente_id,
                saldo = 0,//que le costaba que aqui se pudiera algo mas de 0 lptm
                tipo_cuenta = "ahorros",
                fecha_creacion = DateTime.Now
            });
            //porque el pete del sebas chevere no me devuelve el id de la nueva cuenta toca hacer maromas
            //pido el cliente para obtner el id de la cuenta que necesito, la primera
            clienteDto cliente = await bancoConsumer.getCliente(clienteAux.cliente_id);
            //luego mando el id a actaulizar salo para una recarga de 1000 dolares iniciales
            await bancoConsumer.actualizarSaldo(cliente.Cuentas[0].cuenta_id, 1000);
            //luego vuelvo a actualizar para obtener el cliente ya con la cuenta correcta
            cliente = await bancoConsumer.getCliente(clienteAux.cliente_id);
            return cliente;
        }
    }
}
