using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Identity.Service.Entities;

var builder = WebApplication.CreateBuilder(args);
// const string AllowedOriginSetting = "AllowerdOrigin";

// Add services to the container.
builder.Services.AddControllers(
    options => options.SuppressAsyncSuffixInActionNames = false
);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var collectionName = "users";

builder.Services.AddMongo()
                .AddIndex<User>(collectionName, "Email")
                .AddIndex<User>(collectionName, "Nickname")
                .AddMongoRepository<User>(collectionName);
// .AddMassTransitWithRabbitMq();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();