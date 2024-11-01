using GettingStarted.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GettingStarted;

public static class Extentions
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.ConfigureAuthentication(configuration); // <-- MODIFICATION
        services.AddAuthorization();
    }

//MODIFICATION
    static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var keycloakSettings = configuration.GetSection("Authentication:Keycloak");
        var authority = keycloakSettings["Authority"];
        var audience = keycloakSettings["Audience"];

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authority,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true
                };
            });
    }

    public static void ConfigureMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication(); // <-- MODIFICATION
        app.UseAuthorization();
        app.MapControllers();
        using var serviceScope = app.Services.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<TodoContext>();

        context.TodoItems.Add(new TodoItem { Name = "Item #1" });
        context.TodoItems.Add(new TodoItem { Name = "Item #2" });
        context.TodoItems.Add(new TodoItem { Name = "Item #3" });
        context.SaveChanges();
    }
}