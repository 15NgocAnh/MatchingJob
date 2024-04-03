using MatchingJob.Data;
using Microsoft.EntityFrameworkCore;
using MatchingJob.BLL;
using AutoMapper;
using MatchingJob.BLL.Mappers;
using MatchingJob.BLL.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MatchingJob.BLL.Repositories;
using MatchingJob.API.MiddleWare;
using MatchingJob.API.Email;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

#region add DBContext
var connectionString = builder.Configuration.GetConnectionString("DBContext") ?? throw new InvalidOperationException("Connection string 'DBContext' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
#endregion

// Add services to the container.

#region Mail Sender
var emailConfig = builder.Configuration.GetSection("MailSettings").Get<MailSetting>();
builder.Services.AddSingleton(emailConfig);
#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Encryption and Token
builder.Services.AddSingleton<IPasswordHasher, PBKDF2Hasher>();
builder.Services.AddSingleton<ITokenHelper, JWTHelper>();
#endregion

#region Scoped Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
#endregion

#region autoMapper
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton(provider => new MapperConfiguration(options =>
    {
        options.AddProfile(new AutoMapperProfile(provider.GetService<IPasswordHasher>()));
    })
.CreateMapper());
#endregion

#region Jwt configuration - Authentication
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
#endregion

#region set CORS
builder.Services.AddCors(opt => opt.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseGlobalExceptionMiddleware();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.MapControllers();

app.Run();
