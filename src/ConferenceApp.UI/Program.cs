using ConferenceApp.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register HttpClient for API calls with service discovery
builder.Services.AddHttpClient<ApiService>(client =>
{
    // This will be resolved by service discovery when running with Aspire
    client.BaseAddress = new Uri("https://conferenceapp-api");
});

// Register the API service
builder.Services.AddScoped<ApiService>();

var app = builder.Build();

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

// Map default endpoints for health checks
app.MapDefaultEndpoints();

app.Run();
