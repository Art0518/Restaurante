namespace FacturacionService.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; } = "";
        public T Data { get; set; }
    }
}
