using FacturacionService.Services;
using FacturacionService.GraphQL;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configurar puerto dinámico de Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddControllers();

// Configurar CORS - MEJORADO para localhost y Monster
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
  {
      policy.AllowAnyOrigin()
  .AllowAnyMethod()
   .AllowAnyHeader()
   .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
    });

    // Política específica para desarrollo local
    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins(
  "http://localhost:3000",
         "http://localhost:5173", 
    "http://localhost:8080",
     "http://127.0.0.1:3000",
        "http://127.0.0.1:5173",
         "http://127.0.0.1:8080"
  )
 .AllowAnyMethod()
    .AllowAnyHeader()
   .AllowCredentials()
   .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
    });

    // Política para producción (Monster, Railway)
    options.AddPolicy("Production", policy =>
    {
    policy.WithOrigins(
   "http://cafesanjuanr.runasp.net",
      "https://cafesanjuanr.runasp.net",
         "https://ws-restaurante-production.up.railway.app",
          "https://seguridad-production-279b.up.railway.app"
       )
   .AllowAnyMethod()
.AllowAnyHeader()
          .AllowCredentials()
         .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
    });
});

// Obtener connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registrar el servicio gRPC con la connection string
builder.Services.AddSingleton(provider => 
    new FacturacionGrpcService(
        provider.GetRequiredService<ILogger<FacturacionGrpcService>>(),
     connectionString
    )
);

// Configurar GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<FacturacionQuery>()
    .AddMutationType<FacturacionMutation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Usar política más permisiva (AllowAll) para evitar problemas
app.UseCors("AllowAll");

app.MapGrpcService<FacturacionGrpcService>();
app.MapControllers();
app.MapGraphQL("/graphql");
app.MapGet("/", () => "Servicio de Facturación - CafeSanJuan (gRPC + REST + GraphQL) - CORS ENABLED");

app.Run();
