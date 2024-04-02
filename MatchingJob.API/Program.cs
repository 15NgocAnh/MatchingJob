using MatchingJob.Data;
using Microsoft.EntityFrameworkCore;
using MatchingJob.BLL;
using AutoMapper;
using MatchingJob.BLL.Mappers;
using MatchingJob.BLL.Authentication;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

//add DBContext
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBContext") ?? throw new InvalidOperationException("Connect string 'DBContext' not found!")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPasswordHasher, PBKDF2Hasher>();

//Add scoped Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenHelper, JWTHelper>();

//set autoMapper
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton(provider => new MapperConfiguration(options =>
{
    options.AddProfile(new AutoMapperProfile(provider.GetService<IPasswordHasher>()));
})
            .CreateMapper());
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
