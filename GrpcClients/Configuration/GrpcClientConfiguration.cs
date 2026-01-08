namespace GrpcClients.Configuration
{
    public class GrpcClientConfiguration
    {
        public string FacturacionServiceUrl { get; set; } = "https://localhost:7001";
        public string MenuServiceUrl { get; set; } = "https://localhost:7002";
        public string ReservasServiceUrl { get; set; } = "https://localhost:7003";
        public string SeguridadServiceUrl { get; set; } = "https://localhost:7004";
        
        public int TimeoutSeconds { get; set; } = 30;
        public bool EnableRetry { get; set; } = true;
    }
}
