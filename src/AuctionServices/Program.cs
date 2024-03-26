using AuctionServices;
using Microsoft.EntityFrameworkCore;

// Entry point for the application
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configures DbContext for EF Core.
// Uses Npgsql to set up the DB connection (Postgres) and the connection 
// string is retrieved
builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
  opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Adds the AutoMapper library to the services collection, simplifying the 
// process of mapping between different object models making it easier to 
// transform between entity models and DTOs.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Buulds the application
var app = builder.Build();

// Configure the HTTP request pipeline (Middeleware).
app.UseAuthorization();
// Maps controllers to routes
app.MapControllers();

// Attepts to initialize the DB using DbInitializer Class and with console log
// any exception that may occur.
try
{
  DbInitializer.InitDb(app);
}
catch (Exception e)
{
  Console.WriteLine(e);
}

app.Run();
