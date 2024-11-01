using System.Reflection;
using Keycloak.AuthServices.Authentication;
using Microsoft.OpenApi.Models;
using GettingStarted.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerUI;

// <snippet_UsingOpenApiModels>
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddDbContext<TodoContext>(options => options.UseInMemoryDatabase("Todo"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
////注册SwaggerAPI文档服务
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Getting Started  API",
        Description = "An ASP.NET Core Web API for managing ToDo items"
        
    });
    //
    ///////////////BEGIN 调用Swagger API的时候需要授权/////////////////////////////
    
    var scheme = new OpenApiSecurityScheme()
    {
        Description = "Authorization header. \r\nExample: 'Bearer 12345abcdef'",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Authorization"
        },
        Scheme = "oauth2",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    };
    options.AddSecurityDefinition("Authorization", scheme);
    var requirement = new OpenApiSecurityRequirement
    {
        [scheme] = new List<string>()
    };
    options.AddSecurityRequirement(requirement);
    //
    ///////////////END 调用Swagger API的时候需要授权/////////////////////////////
    // 
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

//Add support for Json Web Token.
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.Audience = builder.Configuration["Authentication:Audience"];
        x.MetadataAddress = builder.Configuration["Authentication:MetaDataAddress"]!;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"]
        };
    });
 

var app = builder.Build();
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
        //页面API文档格式 Full=全部展开， List=只展开列表, None=都不展开
        options.DocExpansion(DocExpansion.List);
    });
}
 
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<TodoContext>();

    context.TodoItems.Add(new TodoItem { Name = "Item #1" });
    context.TodoItems.Add(new TodoItem { Name = "Item #2" });
    context.TodoItems.Add(new TodoItem { Name = "Item #3" });
    context.TodoItems.Add(new TodoItem { Name = "Item #4" });
    await context.SaveChangesAsync();
}

app.Run();
