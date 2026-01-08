using FacturacionService.Services;
using FacturacionService.GraphQL;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddControllers();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
     policy.AllowAnyOrigin()
  .AllowAnyMethod()
        .AllowAnyHeader()
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
app.UseCors("AllowAll");

app.MapGrpcService<FacturacionGrpcService>();
app.MapControllers();
app.MapGraphQL("/graphql");
app.MapGet("/", () => "Servicio de Facturación - CafeSanJuan (gRPC + REST + GraphQL)");

app.Run();
