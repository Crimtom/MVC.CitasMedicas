using Microsoft.EntityFrameworkCore;
using DataAcces.Models;
using Business;
using Business.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<CitasMedicasContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("cn")));

// Business layer
builder.Services.AddScoped<IAppointmentBusiness, AppointmentBusiness>();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Citas Medicas API",
        Version = "v1",
        Description = "API for managing medical appointments"
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
