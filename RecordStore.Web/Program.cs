using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecordStore.Domain.DTO;
using RecordStore.Domain.DTO.Email;
using RecordStore.Domain.Identity;
using RecordStore.Repository;
using RecordStore.Repository.Implementation;
using RecordStore.Repository.Interface;
using RecordStore.Service.Implementation;
using RecordStore.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<RecordStoreApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IRecordService, RecordService>();
builder.Services.AddScoped<IDataFetchService, DataFetchService>();
builder.Services.AddScoped<IRecordLabelService, RecordLabelService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHttpClient();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

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
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<RecordStoreApplicationUser>>();

        string[] roleNames = { "Administrator", "User" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var adminEmail = builder.Configuration["AdminUser:Email"];
        if (adminEmail != null && await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new RecordStoreApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = builder.Configuration["AdminUser:FirstName"],
                LastName = builder.Configuration["AdminUser:LastName"],
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(adminUser, builder.Configuration["AdminUser:Password"]);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Administrator");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();