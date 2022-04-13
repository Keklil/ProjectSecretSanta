using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using SecretSanta_Backend;
using SecretSanta_Backend.Configuration;
using SecretSanta_Backend.Interfaces;
using SecretSanta_Backend.Repositories;
using SecretSanta_Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.ConfigureCors();
builder.Services.ConfigurePostgreSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryWrapper();
builder.Services.ConfigureIISIntegration();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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



app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.UseCors("CorsPolicy");

var scope = app.Services.CreateScope();
var service = scope.ServiceProvider.GetService<IRepositoryWrapper>();
var b = new MailService(service);
var d = new Guid("0ff3d625-af85-4015-a808-e310084ad09b");
await b.sendEmailsWithDesignatedRecipient(d);
await b.SendEmailsWithDateChanges(d);

app.Run();
