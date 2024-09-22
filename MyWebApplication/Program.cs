using Microsoft.EntityFrameworkCore;
using MyWebApplication.Data;
using MyWebApplication.Services;
using MyWebApplication.Services.Implementations;
using MyWebApplication.Services.Implementations.TranslationStrategies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddHttpClient();

builder.Services.AddScoped<ITranslationStrategy, LeetSpeakTranslation>();

builder.Services.AddScoped<ITranslationStrategy, FakeTranslationStrategy>();

builder.Services.AddScoped<ITranslationService, TranslationService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Apply automatic migrations on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DatabaseContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred while migrating the database.");
        Console.WriteLine(ex.Message);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.Run();
