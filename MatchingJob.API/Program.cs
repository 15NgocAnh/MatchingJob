using MatchingJob.Data;
using Microsoft.EntityFrameworkCore;
using MatchingJob.BLL;
using AutoMapper;
using MatchingJob.BLL.Mappers;
using MatchingJob.BLL.Authentication;
using System.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MatchingJob.BLL.Repositories;

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

//Encryption
builder.Services.AddSingleton<IPasswordHasher, PBKDF2Hasher>();
builder.Services.AddSingleton<ITokenHelper, JWTHelper>();

//Add scoped Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

//set autoMapper
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton(provider => new MapperConfiguration(options =>
    {
        options.AddProfile(new AutoMapperProfile(provider.GetService<IPasswordHasher>()));
    })
.CreateMapper());

//Jwt configuration starts here
var tokenConfiguration = builder.Configuration.GetSection("TokenSettings");
builder.Services.Configure<TokenSettings>(tokenConfiguration);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var tokenSettings = tokenConfiguration.Get<TokenSettings>();
    var symmetricKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenSettings.Secret));
    var keyExpiration = tokenSettings.AccessExpirationInMinutes;

    options.RequireHttpsMetadata = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = symmetricKey,
        RequireSignedTokens = true,
        RequireExpirationTime = true,
        ValidateLifetime = true,
        ValidateAudience = true,
        ValidateIssuer = true,
    };
});
//Jwt configuration ends here

//set CORS
builder.Services.AddCors(opt => opt.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

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
app.UseCors();
app.MapControllers();

app.Run();
