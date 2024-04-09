using CombatService;
using CombatService.BacgroundServices;
using CombatService.DataAccessObject;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Bacground services
builder.Services.AddHostedService<UserService>();
#endregion

// For Entity Framework
builder.Services.AddDbContext<AppDbContex>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLDBConnection")));

#region DAO in other class
builder.Services.AddScoped<ApplicationUserDAO>();
#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
