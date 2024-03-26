using System.Text.Json.Serialization;
using Elevator.WebApi.Controllers;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services), 
        writeToProviders: true);
    
    // Add services to the container.
    builder.Services.AddControllers()
        .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

    builder.Services.AddSingleton<IFloorRequestService, FloorRequestService>();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(ConfigureSwagger());

    var app = builder.Build();
    app.UseSerilogRequestLogging();
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseHttpsRedirection();
    }

    app.UseCors(builder => builder
       .AllowAnyHeader()
       .AllowAnyMethod()
       .AllowAnyOrigin()
    );

    app.UseHttpsRedirection();

    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

Action<SwaggerGenOptions> ConfigureSwagger()
{
    return c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Elevator API",
            Version = "v1",
            Description = "Elevator API"
        });

        // // Add XML comments for better documentation
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);

    };
}

/// <summary>
/// A work-around for the lack of a Program class in .NET 6 minimal APIs.
/// We need it to be able to use the WebApplicationBuilder.
/// </summary>
public partial class Program { } 
