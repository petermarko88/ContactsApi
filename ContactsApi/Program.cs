using Contracts;
using LoggerService;
using Microsoft.OpenApi.Models;
using MiniValidation;
using NLog;
using System.ComponentModel.DataAnnotations;


var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Contacts API",
        Description = "A .NET 6 Minimal API Demo"
    });
});
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.MapPost("/contact", (ContactPerson contactPerson, IContactService contactService, ILoggerManager logger) =>
    {
        var isValid = MiniValidator.TryValidate(contactPerson, out var errors);
        if (!isValid)
        {
            foreach (var entry in errors)
            {
                foreach (var error in entry.Value)
                {
                    logger.LogError($"Error while creating new contact: {error}");
                }
            }
            
            return Results.ValidationProblem(errors); 
        }

        var contact = contactService.Create(contactPerson);

        logger.LogInfo($"New contact created: {contactPerson.FirstName} {contactPerson.LastName}");

        return Results.Ok(contact);
    });

app.MapGet("/", () => "Hello World!");

app.Run();

public partial class Program { }

public class ContactPerson
{
    [Required, MinLength(2)]
    public string? FirstName { get; set; }

    [Required, MinLength(2)]
    public string? LastName { get; set; }

    [Required, DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [Required, DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }
}

public interface IContactService
{
    string Create(ContactPerson contactPerson);
}

public class ContactService : IContactService
{
    public string Create(ContactPerson contactPerson)
        => $"Contact '{contactPerson.FirstName} {contactPerson.LastName}' created!";
}