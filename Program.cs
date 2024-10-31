using System.Reflection;
using Keycloak.AuthServices.Authentication;
using Microsoft.OpenApi.Models;
using GettingStarted.Models;
using Microsoft.EntityFrameworkCore;
// <snippet_UsingOpenApiModels>
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseInMemoryDatabase("Todo"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Getting Started  API",
        Description = "An ASP.NET Core Web API for managing ToDo items"
        // TermsOfService = new Uri("https://example.com/terms"),
        // Contact = new OpenApiContact
        // {
        //     Name = "Example Contact",
        //     Url = new Uri("https://example.com/contact")
        // },
        // License = new OpenApiLicense
        // {
        //     Name = "Example License",
        //     Url = new Uri("https://example.com/license")
        // }
    });
    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
// builder.Services.AddAuthorization();
 
var app = builder.Build();
// Configure the HTTP request pipeline.
// <snippet_Middleware>
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}
 
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<TodoContext>();

    context.TodoItems.Add(new TodoItem { Name = "Item #1" });
    await context.SaveChangesAsync();
}

app.Run();
