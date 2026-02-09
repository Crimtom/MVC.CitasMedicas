using Microsoft.EntityFrameworkCore;
using DataAcces.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<CitasMedicasContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("cn"));
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
