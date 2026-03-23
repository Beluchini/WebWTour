using Microsoft.EntityFrameworkCore;
using WebWTour.Components;
using WebWTour.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TourContext>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<TourContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<AuthService>(); // Добавляем сервис авторизации

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

using (var ServieScope = app.Services.CreateScope())
{
    var context = ServieScope.ServiceProvider.GetRequiredService<TourContext>();
    
    context.Database.Migrate();
}

app.Run();