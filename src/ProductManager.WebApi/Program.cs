using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using ProductManager.Application;
using ProductManager.Infrastructure;
using ProductManager.WebApi.Handlers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Controller hizmetlerini ekler.
builder.Services.AddControllers();

// Infrastructure hizmetlerini ekler.
builder.Services.AddInfrastructure(builder.Configuration);

// Seeder hizmetlerini  ekler.
builder.Services.AddSeeders();

// Application hizmetlerini ekler.
builder.Services.AddApplication();

// API u� noktalar�n� ke�fetmek i�in hizmetleri ekler.
builder.Services.AddEndpointsApiExplorer();

// Swagger jenerat�r�n� ekler.
builder.Services.AddSwaggerGen();

// �zel bir hata yakalay�c� ekler.
builder.Services.AddExceptionHandler<ExceptionHandler>();

// Problem detaylar� hizmetini ekler.
builder.Services.AddProblemDetails();

#region Serilog
// Serilog'u yap�land�r�r ve uygulamaya entegre eder.
builder.Host
      .UseSerilog((hostContext, loggerConfiguration) =>
      {
          loggerConfiguration
           .ReadFrom.Configuration(hostContext.Configuration)
           .Enrich.WithProperty("Application", "ProductManager.WebApi");
      });
#endregion

// Swagger i�in JWT kimlik do�rulamas�n� yap�land�r�r.
builder.Services.AddSwaggerGen(setup =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "A�a��daki metin kutusuna **_SADECE_** JWT Bearer token�n�z� girin!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
});

var app = builder.Build();

// Geli�tirme ortam�nda Swagger'� etkinle�tirir.
app.UseSwagger();
app.UseSwaggerUI();

// Serilog ile istek kay�tlar�n� etkinle�tirir.
app.UseSerilogRequestLogging();

// HTTP isteklerini HTTPS'e y�nlendirir.
app.UseHttpsRedirection();

// Hata yakalama i�lemini etkinle�tirir.
app.UseExceptionHandler();

// Kimlik do�rulamay� etkinle�tirir.
app.UseAuthentication();

// Yetkilendirmeyi etkinle�tirir.
app.UseAuthorization();

// Controller'lar� haritaland�r�r.
app.MapControllers();

// Uygulamay� �al��t�r�r.
app.Run();
