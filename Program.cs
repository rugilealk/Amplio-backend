using PSI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers(); // Add controllers

builder.Services.AddSingleton<SongService>();
builder.Services.AddSingleton<PlaylistService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); // Map controller routes

app.Run();
