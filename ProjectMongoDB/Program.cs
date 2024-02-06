using ProjectMongoDB.DbContext;
using ProjectMongoDB.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<DbSettings>( builder.Configuration.GetSection("MyDb"));
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IPassportUserRepository, PassportUserRepository>();
builder.Services.AddTransient<IUserImageRepository, UserImageRepository>();

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
