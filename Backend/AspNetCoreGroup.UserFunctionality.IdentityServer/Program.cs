using System.Text;
using AspNetCoreGroup.UserFunctionality.IdentityServer;
using AspNetCoreGroup.UserFunctionality.IdentityServer.Mapping;
using CommonLibrary.Interfaces.Senders;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitLibrary.Senders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", b =>
    {
        b.WithOrigins(builder.Configuration.GetValue<string>("IdentityServer:ClientUri"));
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("UsersDB"));
});
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "http://localhost:80";
        options.RequireHttpsMetadata = false;
        options.Audience = "authorization";
    })
    .AddOpenIdConnect(options =>
    {
        options.ClientId = "react";
        options.Authority = "http://localhost:80";
        options.RequireHttpsMetadata = false;
    });

builder.Services.Configure<CookieAuthenticationOptions>(IdentityServerConstants.DefaultCookieAuthenticationScheme, options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSameSiteCookiePolicy();

builder.Services
    .AddIdentity<UserDTO, IdentityRole<int>>()
    .AddTokenProvider("IS4", typeof(DataProtectorTokenProvider<UserDTO>))
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services
    .AddIdentityServer()
    .AddAspNetIdentity<UserDTO>()
    .AddInMemoryClients(Config.GetClients(builder))
    .AddInMemoryIdentityResources(Config.GetIdentityResources())
    .AddInMemoryIdentityResources(Config.GetIdentityResources());


builder.Services.AddAutoMapper(typeof(RegisterProfile));
builder.Services.AddAutoMapper(typeof(LoginProfile));

builder.Services.AddScoped<IMessageSender, RabbitSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDeveloperExceptionPage();


app.UseCors("CorsPolicy");

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

