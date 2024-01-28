using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Play.Common.MongoDB;
using Play.Identity.Service.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Play.Identity.Service.Settings;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Play.Identity.Service.Auth;

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
                .AddIndex<User>(collectionName, "Email")
                .AddIndex<User>(collectionName, "Nickname")
                .AddMongoRepository<User>(collectionName);

// .AddMassTransitWithRabbitMq();

// Identity
// builder.Services.AddIdentity<User, IdentityRole>()
//     .AddSignInManager()
//     .AddRoles<IdentityRole>();

var authSettings = builder.Configuration.GetSection(nameof(AuthSettings)).Get<AuthSettings>()!;

// JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        // ValidateLifetime = true,
        ValidIssuer = authSettings.Issuer,
        ValidAudience = authSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings.Key)),
    };
});

// Resolve DI 
builder.Services.AddScoped(_ => new Token(builder.Configuration));

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