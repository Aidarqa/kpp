using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using KppBlazor;
using KppBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<DataStore>();
builder.Services.AddSingleton<ApiService>();
builder.Services.AddSingleton<AppState>();
builder.Services.AddSingleton<ThemeService>();
builder.Services.AddSingleton<LocalizationService>();

// HttpClient остаётся для возможных внешних вызовов, но ApiService его не использует
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

await builder.Build().RunAsync();