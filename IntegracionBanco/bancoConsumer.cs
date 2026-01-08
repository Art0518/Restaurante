using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using IntegracionBanco.bancoDto;
using Newtonsoft.Json;
using IntegracionBanco.errores;

namespace IntegracionBanco
{
     public static partial class bancoConsumer
    {
        private static string url {  get=> "http://mibanca.runasp.net";}
        public static async Task<int>obtenerCuenta(string cedula,decimal? monto)
        {
            clienteDto clienteDto = await getCliente(cedula);
            foreach(var cuenta in clienteDto.Cuentas)
            {
                if(cuenta.saldo>=monto)
                {
                    return cuenta.cuenta_id;
                }
            }
            return -1;
        }
        public static async Task<string> crearCuenta(cuentaDto aux)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(aux), Encoding.UTF8, "application/json");
                var response = await new HttpClient().PostAsync(url + "/api/cuentas", content);
                response.EnsureSuccessStatusCode();
                if (!response.IsSuccessStatusCode)
                {
                    var detalle = await response.Content.ReadAsStringAsync();
                    throw new bankException("crear cuenta 1", $"Código {(int)response.StatusCode} {response.ReasonPhrase}. Detalle: {detalle}");
                }
                string res = await response.Content.ReadAsStringAsync();
                return res;
            }
            catch(HttpRequestException ex)
            {
                throw new bankException("crear cuenta 2 ", ex.Message);
            }
        }
        public static async Task<string> crearCliente(clienteDto aux)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(aux), Encoding.UTF8, "application/json");
                var response = await new HttpClient().PostAsync(url + "/api/clientes", content);
                response.EnsureSuccessStatusCode();
                string res = await response.Content.ReadAsStringAsync();
                return res;
            }
            catch (HttpRequestException ex)
            {
                throw new bankException("crear cliente", ex.Message);
            }
        }
        public static async Task<cuentaDto> getCuenta(int id)
        {
            try
            {
                var response = await new HttpClient().GetAsync(url + "/api/cuentas/" + id);
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<cuentaDto>(jsonString);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
        public static async Task<string> actualizarSaldo(int id,decimal monto)
        {
            try
            {
                Console.WriteLine("se recibio id " + id);
                cuentaDto cuenta = await bancoConsumer.getCuenta(id);
                cuenta.saldo=monto;
                var content = new StringContent(JsonConvert.SerializeObject(cuenta), Encoding.UTF8, "application/json");
                var response = await new HttpClient().PutAsync(url + "/api/cuentas/"+id, content);
                response.EnsureSuccessStatusCode();
                string res = await response.Content.ReadAsStringAsync();
                return res;
            }
            catch (HttpRequestException ex)
            {
                throw new bankException("crear cuenta", ex.Message);
            }
        }
        public static async Task<clienteDto> getCliente(string cedula)
        {
            try
            {
                var response = await new HttpClient().GetAsync(url + "/api/clientes/" + cedula);
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                clienteDto res= JsonConvert.DeserializeObject<clienteDto>(jsonString);
                if(res == null)
                {
                    return null;
                }
                var response2 = await new HttpClient().GetAsync(url + "/api/Cuentas/cliente/" + cedula);
                response2.EnsureSuccessStatusCode();
                var listString= await response2.Content.ReadAsStringAsync();
                List<cuentaDto> cuentas = JsonConvert.DeserializeObject<List<cuentaDto>>(listString);
                res.Cuentas = cuentas;
                return res;
            }
            catch (HttpRequestException ex)
            {
                return null;
            }
        }
        public static async Task<List<clienteDto>> getClientes()
        {
            try
            {
                var response = await new HttpClient().GetAsync(url + "/api/clientes");
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<clienteDto>>(jsonString);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                return new List<clienteDto>();
            }
        }
        public  static async Task<List<cuentaDto>> getCuentas()
        {
            try
            {
                var response = await new HttpClient().GetAsync(url+"/api/cuentas");
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<cuentaDto>>(jsonString);
            }
            catch(HttpRequestException ex) 
            {
                Console.WriteLine(ex.Message);
                return new List<cuentaDto>();
            }
        }
    }
}