using Microsoft.JSInterop;

namespace KppBlazor.Services;

public class ThemeService
{
    private readonly IJSRuntime _js;
    public string Current { get; private set; } = "dark";
    public bool IsDark => Current == "dark";

    public string Accent { get; private set; } = "cyan";
    public event Action? OnChange;

    public record AccentOption(string Name, string Label, string Primary, string Secondary, string Text, string Glow, string Soft);

    public static readonly AccentOption[] Accents =
    {
        new("cyan",    "Cyan",    "#06b6d4", "#0891b2", "#67e8f9", "rgba(6,182,212,0.25)",  "rgba(6,182,212,0.1)"),
        new("violet",  "Violet",  "#8b5cf6", "#7c3aed", "#c4b5fd", "rgba(139,92,246,0.25)", "rgba(139,92,246,0.1)"),
        new("emerald", "Emerald", "#10b981", "#059669", "#6ee7b7", "rgba(16,185,129,0.25)", "rgba(16,185,129,0.1)"),
        new("rose",    "Rose",    "#f43f5e", "#e11d48", "#fda4af", "rgba(244,63,94,0.25)",  "rgba(244,63,94,0.1)"),
        new("amber",   "Amber",   "#f59e0b", "#d97706", "#fcd34d", "rgba(245,158,11,0.25)", "rgba(245,158,11,0.1)"),
        new("blue",    "Blue",    "#3b82f6", "#2563eb", "#93c5fd", "rgba(59,130,246,0.25)", "rgba(59,130,246,0.1)"),
    };

    public AccentOption CurrentAccent => Accents.First(a => a.Name == Accent);

    public ThemeService(IJSRuntime js) => _js = js;

    public async Task InitializeAsync()
    {
        try
        {
            var saved = await _js.InvokeAsync<string?>("localStorage.getItem", "kpp-theme");
            if (!string.IsNullOrWhiteSpace(saved) && (saved == "dark" || saved == "light"))
                Current = saved;

            var savedAccent = await _js.InvokeAsync<string?>("localStorage.getItem", "kpp-accent");
            if (!string.IsNullOrWhiteSpace(savedAccent) && Accents.Any(a => a.Name == savedAccent))
                Accent = savedAccent;
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

    public async Task SetAccentAsync(string accentName)
    {
        if (!Accents.Any(a => a.Name == accentName)) return;
        Accent = accentName;
        await ApplyAsync();
        try { await _js.InvokeVoidAsync("localStorage.setItem", "kpp-accent", Accent); }
        catch { }
        OnChange?.Invoke();
    }

    private async Task ApplyAsync()
    {
        try
        {
            await _js.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", Current);
            var a = CurrentAccent;
            await _js.InvokeVoidAsync("document.documentElement.style.setProperty", "--accent", a.Primary);
            await _js.InvokeVoidAsync("document.documentElement.style.setProperty", "--accent-2", a.Secondary);
            await _js.InvokeVoidAsync("document.documentElement.style.setProperty", "--accent-text", a.Text);
            await _js.InvokeVoidAsync("document.documentElement.style.setProperty", "--accent-glow", a.Glow);
            await _js.InvokeVoidAsync("document.documentElement.style.setProperty", "--accent-soft", a.Soft);
            await _js.InvokeVoidAsync("document.documentElement.style.setProperty", "--grad-brand", $"linear-gradient(135deg, {a.Primary} 0%, #8b5cf6 100%)");
            await _js.InvokeVoidAsync("document.documentElement.style.setProperty", "--grad-btn", $"linear-gradient(135deg, {a.Primary} 0%, {a.Secondary} 100%)");
            await _js.InvokeVoidAsync("document.documentElement.style.setProperty", "--shadow-glow", $"0 0 40px {a.Glow}");
        }
        catch { /* ignore (pre-render) */ }
    }
}
