using PulseHub.Core.Services;
using PulseHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PulseHub.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); 
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();  

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});



builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(); 

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PulseHub API v1.0"));
}

app.UseHttpsRedirection();

app.UseRouting(); 
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); 
});

app.Run();
