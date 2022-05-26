using Microsoft.OpenApi.Models;
using Quartz.Impl;
using SecretSanta_Backend;
using SecretSanta_Backend.Configuration;
using SecretSanta_Backend.Interfaces;
using SecretSanta_Backend.Jobs;
using SecretSanta_Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.ConfigureCors();
builder.Services.ConfigurePostgreSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryWrapper();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
builder.Services.AddAutoMapper(typeof(Program));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
    app.UseHsts();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}");

app.UseCors("CorsPolicy");


//EventNotificationScheduler.Start();
//var reshuffleService = new ReshuffleService();
//await reshuffleService.Reshuffle(new Guid("0ff3d625-af85-4015-a808-e310084ad09b"));

app.Run();