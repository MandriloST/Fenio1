using Blazored.LocalStorage;
using Fenio1.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Fenio1.UI.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ---- Logging (vidljivo u browser konzoli i VS Output prozoru) ----
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
builder.Logging.AddFilter("System", LogLevel.Warning);
builder.Logging.AddFilter("Fenio1", LogLevel.Debug); // naš kod - sve poruke

// API base URL - mijenjaj prema deploymentu
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:57398/";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<ApiService>();

// Auth
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
builder.Services.AddScoped<JwtAuthStateProvider>();

await builder.Build().RunAsync();
