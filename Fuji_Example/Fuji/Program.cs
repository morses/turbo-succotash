using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Fuji.Data;
using Fuji.DAL;
using Fuji.DAL.Abstract;
using Fuji.DAL.Concrete;
using Fuji.Utilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;  // dotnet add package System.Data.SqlClient
using Fuji.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var authenticationConnectionString = builder.Configuration.GetConnectionString("FujiAuthenticationNET6Connection");
SqlConnectionStringBuilder csbAuth = new SqlConnectionStringBuilder(authenticationConnectionString);
// Use Vince's approach to local-cloud connection string nirvana
if(csbAuth.Password == String.Empty)
{
    csbAuth.Password = builder.Configuration["dbpassword"];
}
builder.Services.AddDbContext<AuthenticationDbContext>(options => options.UseSqlServer(csbAuth.ConnectionString));

var fujiConnectionString = builder.Configuration.GetConnectionString("FujiApplicationNET6Connection");
SqlConnectionStringBuilder csbApp = new SqlConnectionStringBuilder(fujiConnectionString);
if(csbApp.Password == String.Empty)
{
    csbApp.Password = builder.Configuration["dbpassword"];
}
builder.Services.AddDbContext<FujiDbContext>(options => options.UseSqlServer(csbApp.ConnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AuthenticationDbContext>();

builder.Services.AddScoped<IAppleRepository, AppleRepository>();
builder.Services.AddScoped<IFujiUserRepository, FujiUserRepository>();

builder.Services.AddControllersWithViews();

/* This should work in .NET 6 to enable MetaDataTypes to be validated, e.g. see Partials.cs in Models.  See: https://github.com/dotnet/runtime/issues/46678
but I can't get it to work. ?
*/
TypeDescriptor.AddProviderTransparent(
                new AssociatedMetadataTypeTypeDescriptionProvider(typeof(Apple), typeof(AppleMetaData)),
                typeof(Apple));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Get the IConfiguration service that allows us to query user-secrets and 
        // the configuration on Azure
        //var config = app.Services.GetRequiredService<IConfiguration>();
        // Set password with the Secret Manager tool, or store in Azure app configuration
        // dotnet user-secrets set SeedUserPW <pw>

        //var testUserPw = config["SeedUserPW"];
        //var adminPw = config["SeedAdminPW"];

        var testUserPw = builder.Configuration["SeedUserPW"];
        var adminPw = builder.Configuration["SeedAdminPW"];

        SeedUsers.Initialize(services, SeedData.UserSeedData, testUserPw).Wait();
        SeedUsers.InitializeAdmin(services, "admin@example.com", "admin", adminPw, "The", "Admin").Wait();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.Run();
