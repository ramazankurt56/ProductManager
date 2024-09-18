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

// API uç noktalarýný keþfetmek için hizmetleri ekler.
builder.Services.AddEndpointsApiExplorer();

// Swagger jeneratörünü ekler.
builder.Services.AddSwaggerGen();

// Özel bir hata yakalayýcý ekler.
builder.Services.AddExceptionHandler<ExceptionHandler>();

// Problem detaylarý hizmetini ekler.
builder.Services.AddProblemDetails();

#region Serilog
// Serilog'u yapýlandýrýr ve uygulamaya entegre eder.
builder.Host
      .UseSerilog((hostContext, loggerConfiguration) =>
      {
          loggerConfiguration
           .ReadFrom.Configuration(hostContext.Configuration)
           .Enrich.WithProperty("Application", "ProductManager.WebApi");
      });
#endregion

// Swagger için JWT kimlik doðrulamasýný yapýlandýrýr.
builder.Services.AddSwaggerGen(setup =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Aþaðýdaki metin kutusuna **_SADECE_** JWT Bearer tokenýnýzý girin!",

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

// Geliþtirme ortamýnda Swagger'ý etkinleþtirir.
app.UseSwagger();
app.UseSwaggerUI();

// Serilog ile istek kayýtlarýný etkinleþtirir.
app.UseSerilogRequestLogging();

// HTTP isteklerini HTTPS'e yönlendirir.
app.UseHttpsRedirection();

// Hata yakalama iþlemini etkinleþtirir.
app.UseExceptionHandler();

// Kimlik doðrulamayý etkinleþtirir.
app.UseAuthentication();

// Yetkilendirmeyi etkinleþtirir.
app.UseAuthorization();

// Controller'larý haritalandýrýr.
app.MapControllers();

// Uygulamayý çalýþtýrýr.
app.Run();
