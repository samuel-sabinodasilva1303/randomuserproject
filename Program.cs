
/// <summary>
/// Autor: Samuel Sabino - 01/10/2025
/// Descrição: Class responsável por configurar serviços, middlewares e iniciar a API.
/// - Registra controllers, DbContext (PostgreSQL), AutoMapper, repositórios e serviços.
/// - Configura CORS, tratamento global de exceções e arquivos estáticos.
/// - Inicializa o banco (EnsureCreated) e mapeia rotas de controllers e fallback.
/// </summary>
/// 
using Microsoft.EntityFrameworkCore;
using RandomUserProject.Data;
using RandomUserProject.Middleware;
using RandomUserProject.Repositories;
using RandomUserProject.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Random User API",
        Version = "v1",
        Description = "API para gerenciamento de usuários com integração Random User Generator"
    });
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddHttpClient<IRandomUserService, RandomUserService>();
builder.Services.AddScoped<IRandomUserService, RandomUserService>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();

app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.Run();