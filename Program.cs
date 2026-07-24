using DotNetEnv;
using MentalTrack.Data;
using MentalTrack.Models;
using MentalTrack.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

Env.Load();



var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(builder.Environment.EnvironmentName);
Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));

DotNetEnv.Env.Load();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
///fcc




// 🔥 Render / production port fix (Kestrel)
builder.WebHost.ConfigureKestrel(options =>
{
    var port = Environment.GetEnvironmentVariable("PORT");
    if (!string.IsNullOrEmpty(port))
    {
        options.ListenAnyIP(int.Parse(port));
    }
});


builder.Services.AddScoped<StatisticsService>();
// Services
builder.Services.AddScoped<CosineSimilarityService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();


builder.Services.AddHttpClient<EmbeddingService>();
builder.Services.AddHttpClient<SentimentService>();

builder.Services.AddScoped<ChunkJournalEntry>();
builder.Services.AddSingleton<EmbeddingConverter>();
builder.Services.AddSingleton<JsonConverter>();

builder.Services.AddScoped<WorkingWithDates>();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS radši vypnout na Renderu
    // app.UseHsts();
}


app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();