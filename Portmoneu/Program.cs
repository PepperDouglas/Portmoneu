

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Portmoneu.Core.Interfaces;
using Portmoneu.Core.Services;
using Portmoneu.Data.Contexts;
using Portmoneu.Data.Interfaces;
using Portmoneu.Data.Repos;


//using Portmoneu.Models.Contexts;
using Portmoneu.Models.Identity;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserRepo, UserRepo>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

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
