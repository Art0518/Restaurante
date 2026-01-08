using GrpcClients.Clients;
using GrpcClients.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GrpcClients
{
    public class GrpcClientFactory
    {
 private readonly GrpcClientConfiguration _configuration;

        public GrpcClientFactory(GrpcClientConfiguration configuration)
        {
           _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
     }

      // Factory method para crear clientes individuales
     public FacturacionGrpcClient CreateFacturacionClient()
        {
  return new FacturacionGrpcClient(_configuration.FacturacionServiceUrl);
}

    public MenuGrpcClient CreateMenuClient()
    {
  return new MenuGrpcClient(_configuration.MenuServiceUrl);
     }

  public ReservasGrpcClient CreateReservasClient()
        {
        return new ReservasGrpcClient(_configuration.ReservasServiceUrl);
 }

  public SeguridadGrpcClient CreateSeguridadClient()
{
        return new SeguridadGrpcClient(_configuration.SeguridadServiceUrl);
      }
    }

  // Clase estática para métodos de extensión
    public static class GrpcClientExtensions
    {
     // Método para registrar los clientes en DI
     public static void AddGrpcClients(this IServiceCollection services, GrpcClientConfiguration configuration)
{
    if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

  services.AddSingleton(configuration);
      services.AddSingleton<GrpcClientFactory>();

 // Registrar clientes individuales como singletons
 services.AddSingleton(provider =>
  {
      var factory = provider.GetRequiredService<GrpcClientFactory>();
      return factory.CreateFacturacionClient();
     });

 services.AddSingleton(provider =>
 {
var factory = provider.GetRequiredService<GrpcClientFactory>();
    return factory.CreateMenuClient();
        });

   services.AddSingleton(provider =>
        {
 var factory = provider.GetRequiredService<GrpcClientFactory>();
        return factory.CreateReservasClient();
  });

  services.AddSingleton(provider =>
    {
 var factory = provider.GetRequiredService<GrpcClientFactory>();
     return factory.CreateSeguridadClient();
 });
       }
    }
}
