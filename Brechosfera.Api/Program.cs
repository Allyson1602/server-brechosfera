using Brechosfera.Api;
using Brechosfera.Application;
using Brechosfera.Infrastructure;
using Brechosfera.Infrastructure.Identity;
using Brechosfera.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddAuth(builder.Configuration)
    .AddApi();

var app = builder.Build();

await app.Services.SeedRolesAsync();

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