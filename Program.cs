using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Text;
using WebApplication2.DBContext;
using WebApplication2.Extension;
using WebApplication2.Hubs;
using WebApplication2.Middleware;
using WebApplication2.Repositries;
using WebApplication2.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog with SQL Server sink
var columnOptions = new ColumnOptions
{
    AdditionalDataColumns = new Collection<System.Data.DataColumn>
    {
        new System.Data.DataColumn { DataType = typeof(string), ColumnName = "Application" },
        new System.Data.DataColumn { DataType = typeof(string), ColumnName = "MachineName" }
    }
};

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "My ASP.NET Core App")
    .WriteTo.Console()
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        sinkOptions: new MSSqlServerSinkOptions
        {
            AutoCreateSqlTable = true,
            TableName = "LogEvents"
        },
        restrictedToMinimumLevel: LogEventLevel.Error,
        columnOptions: columnOptions)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ProductHub>();
builder.Services.AddSingleton<ProductChangeNotificationService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register custom services
//builder.Services.AddSingleton<ProductChangeNotificationService>(sp =>
//    new ProductChangeNotificationService(sp.GetRequiredService<IHubContext<ProductHub>>(), connectionString));

builder.Services.AddScoped<IRepoUser, RepoUser>();
builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
        };

        // Read token from the HttpOnly cookie
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["AuthToken"];
                return Task.CompletedTask;
            }
        };
    });

// Enable CORS to allow requests from any origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyMethod()
               .AllowAnyHeader()
               .SetIsOriginAllowed(_ => true)
               .AllowCredentials();
    });
});

// Configure Swagger/OpenAPI for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure Middleware for request pipeline
app.UseMiddleware<GlobalExceptionHandlingMiddleWare>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

// Enable Routing
app.UseRouting();

// Enable Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map SignalR Hubs and Controllers
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ProductHub>("/productHub");
    endpoints.MapControllers();
});
app.UseSqlTableDependency<ProductChangeNotificationService>(connectionString);
app.Run();
