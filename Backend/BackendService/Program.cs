using CommonLibrary.Interfaces.Listeners;
using CommonLibrary.Interfaces.Senders;
using CommonLibrary.Interfaces.Services;
using BackendService.DataSources;
using BackendService.Listeners;
using BackendService.Middlewares;
using BackendService.Senders;
using BackendService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging();

builder.Services.AddDbContext<BackendContext>();

builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IDevicesService, DevicesService>();
builder.Services.AddScoped<INetworksService, NetworksService>();

builder.Services.AddScoped<IMessageSender, RabbitSender>();
builder.Services.AddScoped<IMessageListenerFactory, RabbitListenerFactory>();

builder.Services.AddScoped<IAuthorizationService, SimpleAuthorizationService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionsMiddleware>();

app.MapControllers();

app.Run();

