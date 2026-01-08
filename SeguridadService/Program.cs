using SeguridadService.Services;
using SeguridadService.GraphQL;

var builder = WebApplication.CreateBuilder(args);

// Configurar puerto dinámico de Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

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

// Registrar el servicio gRPC
builder.Services.AddSingleton(provider =>
    new SeguridadGrpcService(
        provider.GetRequiredService<ILogger<SeguridadGrpcService>>(),
        connectionString
    )
);

// Configurar GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<SeguridadQuery>()
    .AddMutationType<SeguridadMutation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowAll");

app.MapGrpcService<SeguridadGrpcService>();
app.MapControllers();
app.MapGraphQL("/graphql");
app.MapGet("/", () => "Servicio de Seguridad - CafeSanJuan (gRPC + REST + GraphQL)");

app.Run();
