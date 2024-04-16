using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Portmoneu.Api.Extensions;
using Portmoneu.Data.Contexts;
using Portmoneu.Models.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllerExtended();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = jwtSettings["Key"];

builder.Services.AddAuthenticationExtended(jwtSettings, key);

builder.Services.AddAuthorization(options => {
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
});

builder.Services.AddTransientExtended();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerExtended();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer
(builder.Configuration.GetConnectionString("IdentityDB")));

builder.Services.AddDbContext<BankAppData>(options =>
options.UseSqlServer
(builder.Configuration.GetConnectionString("BankAppData")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Loading works");

app.Run();
