using PSI.Services;
using PSI.Data;
using Microsoft.EntityFrameworkCore; //prideta databaze

var builder = WebApplication.CreateBuilder(args);

//databaze
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });


builder.Services.AddScoped<SongService>();
builder.Services.AddScoped<PlaylistService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); // Map controller routes

app.Run();
