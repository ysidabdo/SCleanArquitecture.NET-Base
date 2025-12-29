using Application.Config;
using Infraestructure.Extension;
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//Ayuda a organizar según el rol que tiene en la capa de aplicación, no es necesario moverlo de lugar pero ayuda a la organización del proyecto
builder.Configuration
    .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"Config/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.AddConfiguration();
builder.AgregarCola();

var app = builder.Build();


app.MapGet("/person/{id}", async Task<IResult>(int id, PersonService personService) =>
{
        var person = await personService.GetPersonById(id);
        return Results.Ok(person);
});

app.Run();
