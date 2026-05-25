using KppBlazor.Models;

namespace KppBlazor.Services;

/// <summary>
/// Глобальное состояние приложения.
/// После миграции на Blazor Router: маршрутизация через NavigationManager,
/// AppState отвечает только за авторизацию, статистику и фильтры истории.
/// </summary>
public class AppState
{
    public AuthInfo? Auth { get; private set; }
    public List<RoleItem> RolesCache { get; set; } = new();
    public SidebarStats Stats { get; set; } = new();

    public event Action? OnChange;

    /// <summary>
    /// URL для перехода после авторизации.
    /// Устанавливается в Login/Register, используется в App.razor для редиректа.
    /// </summary>
    public string? RedirectAfterLogin { get; set; }

    public void SetAuth(AuthInfo? auth)
    {
        Auth = auth;
        RedirectAfterLogin = null;
        NotifyStateChanged();
    }

    /// <summary>
    /// Фильтр для страницы истории (передаётся через query-параметр или программно).
    /// </summary>
    public string HistoryFilter { get; set; } = GuestStatus.All;
    public bool HistoryFilterPending { get; set; }

    public void SetHistoryFilter(string filter)
    {
        HistoryFilter = filter;
        HistoryFilterPending = true;
        NotifyStateChanged();
    }

    public void Logout()
    {
        Auth = null;
        RedirectAfterLogin = null;
        Stats = new SidebarStats();
        HistoryFilter = GuestStatus.All;
        HistoryFilterPending = false;
        NotifyStateChanged();
    }

    public void UpdateStats(SidebarStats stats)
    {
        Stats = stats;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}

public class SidebarStats
{
    public int InsideCnt { get; set; }
    public int TodayCnt { get; set; }
    public int PendingCnt { get; set; }
    public int Total { get; set; }
}
