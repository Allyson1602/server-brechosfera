using Brechosfera.Api.Exceptions;
using Brechosfera.Application;
using Brechosfera.Application.Handlers;
using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Messages;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Services;
using Brechosfera.Application.Validators;
using Brechosfera.Infrastructure;
using Brechosfera.Infrastructure.Data;
using Brechosfera.Infrastructure.Identity;
using Brechosfera.Infrastructure.Security;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<JwtSettings>()
    .Bind(
        builder.Configuration.GetSection(
            JwtSettings.SectionName
        )
    )
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.Issuer),
        "Jwt:Issuer must be configured."
    )
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.Audience),
        "Jwt:Audience must be configured."
    )
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.Key),
        "Jwt:Key must be configured."
    )
    .Validate(
        settings => settings.ExpirationMinutes > 0,
        "Jwt:ExpirationMinutes must be greater than zero."
    )
    .ValidateOnStart();

var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT configuration was not found.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)
            ),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";

    options.SignIn.RequireConfirmedEmail = true;
    
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddSignInManager();

builder.Services.AddAuthorization();

builder.Services.AddScoped<IMessageHandler<UserMessage>, UserHandler>();

builder.Services.AddApplication().AddInfrastructure(builder.Configuration).AddApi();

builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddSecurityConfiguration();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<INotifier, Notifier>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddScoped<IUserCredentialService, UserCredentialService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IStoreReviewService, StoreReviewService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

builder.Services.AddFluentValidationAutoValidation(config =>
{
    config.DisableDataAnnotationsValidation = true;
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();