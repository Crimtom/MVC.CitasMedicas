using API.CitasMedicas.Services;

var builder = WebApplication.CreateBuilder(args);

// HttpClient para consumir API.CitasMedicas (Microservicio)
builder.Services.AddHttpClient<IAppointmentApiService, AppointmentApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MicroservicesSettings:AppointmentsApi"] ?? "http://localhost:5112");
});

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Citas Medicas BFF",
        Version = "v1",
        Description = "Backend For Frontend - Centralizes API calls to microservices"
    });
});

var app = builder.Build();

// Swagger UI (development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
