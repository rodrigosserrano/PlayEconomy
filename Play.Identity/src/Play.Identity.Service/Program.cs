using Play.Common.MongoDB;
using Play.Identity.Service.Entities;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Play.Common.Auth;
using Play.Identity.Service.Controllers;
using Play.Identity.Service.DTOs.Auth;
using Play.Common;

var builder = WebApplication.CreateBuilder(args);
// const string AllowedOriginSetting = "AllowerdOrigin";

// Add services to the container.
builder.Services.AddControllers(
    options => options.SuppressAsyncSuffixInActionNames = false
);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var collectionName = "users";

builder.Services.AddMongo()

                .AddIndex<User>(collectionName, "Nickname")
                .AddIndex<User>(collectionName, "Email")
                .AddMongoRepository<User>(collectionName)

                .AddMongoRepository<Role>("roles")

                .AddMongoRepository<Session>("sessions")

                .AddAuth();

// .AddMassTransitWithRabbitMq();

// Add authentication to Swagger UI
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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