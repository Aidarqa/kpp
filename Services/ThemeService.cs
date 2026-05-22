using Microsoft.JSInterop;

namespace KppBlazor.Services;

public class ThemeService
{
    private readonly IJSRuntime _js;
    public string Current { get; private set; } = "dark";
    public bool IsDark => Current == "dark";

    public event Action? OnChange;

    public ThemeService(IJSRuntime js) => _js = js;

    public async Task InitializeAsync()
    {
        try
        {
            var saved = await _js.InvokeAsync<string?>("localStorage.getItem", "kpp-theme");
            if (!string.IsNullOrWhiteSpace(saved) && (saved == "dark" || saved == "light"))
                Current = saved;
        }
        catch { /* ignore */ }
        await ApplyAsync();
    }

    public async Task ToggleAsync()
    {
        Current = IsDark ? "light" : "dark";
        await ApplyAsync();
        try { await _js.InvokeVoidAsync("localStorage.setItem", "kpp-theme", Current); }
        catch { /* ignore */ }
        OnChange?.Invoke();
    }

    private async Task ApplyAsync()
    {
        try
        {
            await _js.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", Current);
        }
        catch { /* ignore (pre-render) */ }
    }
}
