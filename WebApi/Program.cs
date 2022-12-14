using System.Net;
using System.Text;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using WebApi;
using WebApi.DataAccess.Contracts;
using WebApi.DataAccess.Repositories;
using WebApi.Services;


var MainCorsPolicy = "_mainCorsPolicy";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: MainCorsPolicy,
                policy =>
                {
                    policy.WithOrigins("https://localho.st:5001")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                    
                    policy.WithOrigins("https://localho.st:5001/api/token/refreshaccess")
                    .AllowAnyHeader()
                    .WithMethods("GET")
                    .AllowCredentials();
                });
                
        });
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContext<HelpForStudentsContext>(options => options.UseNpgsql(connectionString, o => o.UseNodaTime()));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();
#region Repositories
    builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    builder.Services.AddTransient<IUserRepository, UserRepository>();
    builder.Services.AddTransient<IThemeRepository, ThemeRepository>();
    builder.Services.AddTransient<ISubjectRepository, SubjectRepository>();
    builder.Services.AddTransient<IPaymentRepository, PaymentRepository>();
    builder.Services.AddTransient<IRefreshSessionRepository, RefreshSessionRepository>();
#endregion

builder.Services.AddAuthentication(options => {
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddCookie(/*options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Home/Error";
            }*/)
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new
                    SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes
                    (builder.Configuration["JWT:Key"]))
                };
            });


builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.TryAddSingleton<ICacheService, CacheService>();
builder.Services.TryAddTransient<ICiaccoRandom, CiaccoRandom>();
/*builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
    options.HttpsPort = 5001;
});*/

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();

app.UseHsts();

app.UseCors(MainCorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
