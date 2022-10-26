using System.Net;
using Microsoft.AspNetCore.HttpOverrides;
using WebApi;
using Microsoft.EntityFrameworkCore;
using WebApi.DataAccess.Contracts;
using WebApi.DataAccess.Repositories;

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
    builder.Services.AddTransient<IDocumentRepository, DocumentRepository>();
    builder.Services.AddTransient<IPaymentRepository, PaymentRepository>();
    builder.Services.AddTransient<IFormulaRepository, FormulaRepository>();
#endregion

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
