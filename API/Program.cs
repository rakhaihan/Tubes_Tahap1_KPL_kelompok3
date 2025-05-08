using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Tambahkan services untuk controller
builder.Services.AddControllers();

// Tambahkan Swagger untuk dokumentasi API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aktifkan Swagger di development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Optional, bisa dihapus jika tidak pakai HTTPS

app.UseAuthorization();

app.MapControllers(); // Ini penting agar controller bisa berjalan

app.Run();
