using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using KppBlazor;
using KppBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


// ─── In-memory БД и сервисы ──────────────────────────────
// AddSingleton: один экземпляр на всё время жизни приложения
builder.Services.AddSingleton<DataStore>();   // ← in-memory БД
builder.Services.AddSingleton<ApiService>();  // ← локальный "API"
builder.Services.AddSingleton<AppState>();    // ← состояние сессии

// HttpClient оставляем на случай если понадобится реальный API в будущем
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

await builder.Build().RunAsync();