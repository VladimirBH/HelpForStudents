using System.Net;
using System.Text;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApi;
using WebApi.DataAccess.Contracts;
using WebApi.DataAccess.Repositories;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HelpForStudentsContext>(options => options.UseNpgsql(connectionString));

#region Repositories
    builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    builder.Services.AddTransient<IUserRepository, UserRepository>();
    builder.Services.AddTransient<IThemeRepository, ThemeRepository>();
    builder.Services.AddTransient<ISubjectRepository, SubjectRepository>();
    builder.Services.AddTransient<IPaymentRepository, PaymentRepository>();
#endregion
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
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
/*builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
    options.HttpsPort = 5001;
});*/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();

app.UseHsts();

app.UseAuthorization();

app.MapControllers();

app.Run();
