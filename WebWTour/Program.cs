using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using WebWTour;
using WebWTour.Components;
using WebWTour.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TourContext>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProtectedLocalStorage>();

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

using (var ServiceScope = app.Services.CreateScope())
{
    var context = ServiceScope.ServiceProvider.GetRequiredService<TourContext>();
    
    context.Database.Migrate();
}

app.Run();