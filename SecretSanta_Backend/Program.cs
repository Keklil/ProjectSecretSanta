using SecretSanta_Backend.Configuration;
using SecretSanta_Backend.Interfaces;
using SecretSanta_Backend.Jobs;
using SecretSanta_Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.ConfigureCors();
builder.Services.AddAuthentication("Cookies").AddCookie();
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


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}");

app.UseCors("CorsPolicy");

var scope = app.Services.CreateScope();
var repository = scope.ServiceProvider.GetService<IRepositoryWrapper>();
RepositoryTransfer.SetRepository(repository);
EventNotificationScheduler.Start();

app.Run();

public static class RepositoryTransfer
{
    public static IRepositoryWrapper repository;

    public static void SetRepository(IRepositoryWrapper repository)
    {
        RepositoryTransfer.repository = repository;
    }

    public static IRepositoryWrapper GetRepository()
    {
        return repository;
    }
}