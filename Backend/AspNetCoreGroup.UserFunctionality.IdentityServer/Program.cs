using System.Text;
using AspNetCoreGroup.UserFunctionality.IdentityServer;
using AspNetCoreGroup.UserFunctionality.IdentityServer.Mapping;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins(
                builder.Configuration.GetValue<string>("IdentityServer:ClientUri") ?? "http://127.0.0.1:3000",
                "http://localhost:5111"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
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
        options.Authority = "http://localhost:5111";
        options.RequireHttpsMetadata = false;
        options.Audience = "authorization";
    })
    .AddOpenIdConnect(options =>
    {
        options.ClientId = "react";
        options.Authority = "http://localhost:5111";
        options.RequireHttpsMetadata = false;
    });

builder.Services.Configure<CookieAuthenticationOptions>(IdentityServerConstants.DefaultCookieAuthenticationScheme, options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSameSiteCookiePolicy();


//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("Admin", policy =>
//    {
//        policy.RequireClaim("Admin", "true");
//    });
//});


builder.Services
    .AddIdentity<UserDTO, IdentityRole>()
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseCookiePolicy();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

