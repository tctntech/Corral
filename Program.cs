using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Get the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("TexasCorralDB");

if (string.IsNullOrEmpty(connectionString))
{
    var logger = LoggerFactory.Create(logging => logging.AddConsole()).CreateLogger("Program");
    logger.LogError("The connection string 'TexasCorralDB' is not configured properly.");
    throw new InvalidOperationException("Database connection string is missing. Check appsettings.json.");
}
// Register HttpClient for ProductVendorViewController or other controllers that need it
builder.Services.AddHttpClient();


// ✅ Add CORS BEFORE builder.Build()
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// ✅ Add session support BEFORE builder.Build()
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Add MVC services BEFORE builder.Build()
builder.Services.AddControllersWithViews();

// ✅ Add Swagger BEFORE builder.Build()
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ NOW Build the Application
var app = builder.Build();

// ✅ Enable Swagger in Development Mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Use middleware in correct order
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();    // FIRST: Routing
app.UseCors("AllowAll");  // THEN: Apply CORS
app.UseSession();  // THEN: Apply Session

// ✅ Enable Authentication (if needed)
// app.UseAuthentication();
// app.UseAuthorization();

// ✅ Enable API Controllers
app.MapControllers();

// ✅ Map default route for MVC

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
