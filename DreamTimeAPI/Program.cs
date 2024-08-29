using DreamTeamAPI.Interfaces;
using DreamTeamAPI.Models;
using DreamTeamAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
 builder.Services.AddDbContext<DreamTeamContext>(options => options.UseSqlServer(connectionString));


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
/*builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});*/

var app = builder.Build();

/*if (app.Environment.IsDevelopment())
{*/
    app.UseSwagger();
    app.UseSwaggerUI();
/*}*/

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();
//app.UseCors();  
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TeamsMVC}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
public partial class Program { }
