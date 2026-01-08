using ReservasService.Services;
using ReservasService.GraphQL;

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

// Registrar el servicio gRPC
builder.Services.AddSingleton(provider =>
    new ReservasGrpcService(
        provider.GetRequiredService<ILogger<ReservasGrpcService>>(),
        connectionString
    )
);

// Configurar GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<ReservasQuery>()
    .AddMutationType<ReservasMutation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowAll");

app.MapGrpcService<ReservasGrpcService>();
app.MapControllers();
app.MapGraphQL("/graphql");
app.MapGet("/", () => "Servicio de Reservas - CafeSanJuan (gRPC + REST + GraphQL)");

app.Run();
