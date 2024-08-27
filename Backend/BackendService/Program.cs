using BackendCommonLibrary.Interfaces.BackgroundServices;
using BackendCommonLibrary.Interfaces.Services;
using BackendService.BackgroundServices;
using BackendService.DataSources;
using BackendService.Middlewares;
using BackendService.Services;
using CommonLibrary.Interfaces.Listeners;
using CommonLibrary.Interfaces.Senders;
using CommonLibrary.Interfaces.Services;
using DefaultRealisationLibrary.Factories;
using DefaultRealisationLibrary.Services;
using RabbitLibrary.Listeners;
using RabbitLibrary.Senders;
// TODO: Заменить на встроенный вариант
using IHttpClientFactory = CommonLibrary.Interfaces.Factories.IHttpClientFactory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging();

builder.Services.AddDbContext<BackendContext>();

builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IDevicesService, DevicesService>();
builder.Services.AddScoped<INetworksService, NetworksService>();
builder.Services.AddScoped<INetworkDevicesService, NetworkDevicesService>();
builder.Services.AddScoped<INetworkUsersService, NetworkUsersService>();
builder.Services.AddScoped<INetworkRulesService, NetworkRulesService>();
builder.Services.AddScoped<IGraphsService, GraphsService>();

builder.Services.AddScoped<IMessageSender, RabbitSender>();
builder.Services.AddScoped<IMessageListenerFactory, RabbitListenerFactory>();
builder.Services.AddScoped<IRequestsService, HttpRequestService>();
builder.Services.AddScoped<IHttpClientFactory, HttpClientFactory>();
builder.Services.AddScoped<IDataEventsService, DataEventsService>();
builder.Services.AddScoped<IDataStorageService, DataStorageService>();

builder.Services.AddSingleton<IHttpClientFactory, HttpClientFactory>();
//builder.Services.AddScoped<IAuthorizationService, HTTPAuthorizationService>();
builder.Services.AddScoped<IAuthorizationService, SimpleAuthorizationService>();

builder.Services.AddHostedService<DataEventsBackgroundService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionsMiddleware>();
app.UseMiddleware<AuthorizationMiddleware>();

app.MapControllers();

app.Run();

