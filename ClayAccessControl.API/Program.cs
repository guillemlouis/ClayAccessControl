using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using ClayAccessControl.Core.Models;
using ClayAccessControl.Infrastructure.Data;
using ClayAccessControl.Infrastructure.Services;
using System.Text.Json.Serialization;
using ClayAccessControl.API.Middleware;
using FluentValidation.AspNetCore;
using ClayAccessControl.API.Validators;
using ClayAccessControl.Core.Interfaces;
using ClayAccessControl.Infrastructure.Repositories;
using FluentValidation;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
var authSettings = builder.Configuration.GetSection("AuthSettings").Get<AuthSettings>();

// Configure JWT authentication
builder.Services.AddSingleton(authSettings!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = authSettings!.Issuer,
            ValidAudience = authSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.SecretKey)),
            RoleClaimType = ClaimTypes.Role
        };
    });

// Json config
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configure FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Register all validators in the assembly
var assembly = Assembly.GetExecutingAssembly();
builder.Services.AddValidatorsFromAssembly(assembly);

// Register services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<IOfficeRepository, OfficeRepository>();
builder.Services.AddScoped<IOfficeService, OfficeService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IDoorRepository, DoorRepository>();
builder.Services.AddScoped<IDoorService, DoorService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().WithOpenApi();

// Add test data for the app
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    var passwordService = services.GetRequiredService<PasswordService>();
    context.Database.Migrate();
    var seeder = new DatabaseSeeder(context, passwordService);
    await seeder.SeedAsync();
}

// Configure middlewares
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
