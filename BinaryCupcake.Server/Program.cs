using BinaryCupcake.Client.Services;
using BinaryCupcake.Server.Data;
using BinaryCupcake.Server.Repositories;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Starting
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("ConnectionString não encontrada."));
});

builder.Services.AddScoped<IProduto, ProdutoRepository>();
builder.Services.AddScoped<IUsuario, UsuarioRepository>();

//Ending



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
