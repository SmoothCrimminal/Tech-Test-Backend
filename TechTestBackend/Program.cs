using Microsoft.EntityFrameworkCore;
using TechTestBackend;
using TechTestBackend.DelegatingHandlers;
using TechTestBackend.Extensions;
using TechTestBackend.Interfaces;
using TechTestBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextFactory<SongsStorageContext>(options => options.UseInMemoryDatabase("SongsStorage"));
builder.Services.AddConfigurations(builder.Configuration);
builder.Services.AddHttpClients();

builder.Services.AddScoped<ITracksService, SpotifyService>();
builder.Services.AddTransient<SpotifyAuthenticationHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();