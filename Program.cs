using Serilog.Events;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using WebApplication2.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApplication2.Repositries;
using WebApplication2.DBContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Override default levels for specific namespaces
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "My ASP.NET Core App")
    .WriteTo.Console() // Log to the console
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        sinkOptions: new MSSqlServerSinkOptions
        {
            AutoCreateSqlTable = true, // Automatically create the log table if it does not exist
            TableName = "LogEvents"
        },
        restrictedToMinimumLevel: LogEventLevel.Error, // Only log Error and above to SQL
        columnOptions: columnOptions) // Use custom column options
    .CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddScoped<IRepoUser, RepoUser>();
builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandlingMiddleWare>();
app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
