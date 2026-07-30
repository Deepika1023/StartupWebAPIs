using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StartupWebAPIs.Data;
using StartupWebAPIs.Interfaces;
using StartupWebAPIs.Jobs;
using StartupWebAPIs.Mapping;
using StartupWebAPIs.Middleware;
using StartupWebAPIs.Models;
using StartupWebAPIs.Repositories;
using StartupWebAPIs.Services;
using StartupWebAPIs.Swagger;
using StartupWebAPIs.Validators;
using System.Text;
using StartupWebAPIs.Data;
using System.Threading.RateLimiting;

////Serilog

//Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console()
//    .WriteTo.File("Logs/log-.txt",
//        rollingInterval: RollingInterval.Day)
//    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
});
//builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "StartupWebAPIs:";
});
builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString("DefaultConnection")));
});

builder.Services.AddHangfireServer();
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "PostgreSQL")
    .AddRedis(
        "localhost:6379",
        name: "Redis");
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // options.UseSqlServer(
    //  builder.Configuration.GetConnectionString("DefaultConnection"));

    options.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IExcelReportService, ExcelReportService>();
builder.Services.AddScoped<IPdfReportService, PdfReportService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<BackgroundJobs>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";

    //options.DocInclusionPredicate((docName, apiDesc) =>
    //{
    //    var versions = apiDesc.ActionDescriptor.EndpointMetadata
    //        .OfType<ApiVersionAttribute>()
    //        .SelectMany(v => v.Versions);

    //    return versions.Any(v => $"v{v.ToString()}" == docName);
    //});
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });


    //Optional: Enable X-API - KEY in Swagger

    //To make testing easier, add this to your existing AddSwaggerGen(...) configuration:
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key required in the X-API-KEY header",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-KEY",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "ApiKey"
            }
        },
        Array.Empty<string>()
    }
});
});
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);

    options.AssumeDefaultVersionWhenUnspecified = true;

    options.ReportApiVersions = true;

    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";

    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("ApiPolicy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            Success = false,
            Message = "Rate limit exceeded. Please try again later.",
            StatusCode = 429
        }, cancellationToken);
    };
});
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
var app = builder.Build();
app.MapGet("/test-rate", () => "OK")
   .RequireRateLimiting("ApiPolicy");
app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwaggerUI(options =>
    {

        // options.SwaggerEndpoint("/swagger/v1/swagger.json", "Startup Web APIs V1");
        //options.SwaggerEndpoint("/swagger/v2/swagger.json", "Startup Web APIs V2");
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}


app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseMiddleware<ApiKeyMiddleware>();

app.UseRateLimiter();      // <-- Move here

app.UseAuthentication();

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");
RecurringJob.AddOrUpdate<BackgroundJobs>( 
    "cleanup-job",
    job => job.CleanExpiredApiKeys(),
    Cron.Minutely);  //Register the recurring job

app.MapControllers();

app.MapHealthChecks("/health");
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await DbSeeder.SeedAsync(context);
}
app.Run();


