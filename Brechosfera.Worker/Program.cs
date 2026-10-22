using Brechosfera.Application;
using Brechosfera.Application.Messages;
using Brechosfera.Infrastructure;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMessageHandlers();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddRabbitMqConsumer<EmailConfirmationMessage>(Queues.EmailConfirmationRequested);
builder.Services.AddRabbitMqConsumer<UserMessage>(Queues.UserCreated);

builder.Build().Run();