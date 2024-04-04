using AutoMapper;
using MatchingJob.API.Email;
using MatchingJob.API.MiddleWare;
using MatchingJob.BLL;
using MatchingJob.BLL.Authentication;
using MatchingJob.BLL.Mappers;
using MatchingJob.BLL.Repositories;
using MatchingJob.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

#region add DBContext
var connectionString = builder.Configuration.GetConnectionString("DBContext") ?? throw new InvalidOperationException("Connection string 'DBContext' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
#endregion

// Add services to the container.
#region set header for bearer
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
            Reference = new OpenApiReference
            {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
#endregion 
#region Mail Sender
builder.Services.AddOptions();                                         // Kích hoạt Options
var mailsettings = configuration.GetSection("MailSettings");  // đọc config
builder.Services.Configure<MailSetting>(mailsettings);
builder.Services.AddScoped<IEmailSender, EmailSender>();
#endregion

#region Authorization
builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("InGenZ", policy =>
    //    policy.Requirements.Add(new MinimumAgeRequirement(21)));.3
});
#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();



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
        ValidateAudience = false,
        ValidateIssuer = false,
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
