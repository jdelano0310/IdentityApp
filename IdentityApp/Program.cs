using IdentityApp.Data;
using IdentityApp.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// turned this off during dev/testing   .. then add user roles
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// add user roles

builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 5;

    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = true;
});

// add user authorization rules
builder.Services.AddAuthorization(options =>
{
    // require that the user of the site be registered to use anything but login
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// adding the creator authorization services   addscoped due to making use of entityframecore
builder.Services.AddScoped<IAuthorizationHandler, InvoiceCreatorAuthorizationHandler>();

// use singleton for 1 instance and it isn't using entityframecore in it
builder.Services.AddScoped<IAuthorizationHandler, InvoiceManagerAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, InvoiceAdministratorAuthorizationHandler>();

var app = builder.Build();

// to allow seeding of data 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // if a database doesn't exist where this site is run it will be created also if there any open
    // migrations this will catch them up
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    var seedUserPassword = builder.Configuration.GetValue<string>("seedUserPassword");
    await SeedData.Initialize(services, seedUserPassword);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// this order is important
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
