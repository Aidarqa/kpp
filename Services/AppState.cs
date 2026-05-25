using KppBlazor.Models;

namespace KppBlazor.Services;

public class AppState
{
    public AuthInfo? Auth { get; private set; }
    public string CurrentView { get; set; } = "history";
    public List<RoleItem> RolesCache { get; set; } = new();
    public SidebarStats Stats { get; set; } = new();

    public event Action? OnChange;

    public void SetAuth(AuthInfo? auth)
    {
        Auth = auth;
        if (auth != null)
            CurrentView = auth.CanViewHistory ? "dashboard" : "register";
        NotifyStateChanged();
    }

    public void Navigate(string view)
    {
        CurrentView = view;
        NotifyStateChanged();
    }

    public string HistoryFilter { get; set; } = GuestStatus.All;
    public bool HistoryFilterPending { get; set; } = false;

    public void SetHistoryFilter(string filter)
    {
        HistoryFilter = filter;
        HistoryFilterPending = true;
        NotifyStateChanged();
    }

    public void Logout()
    {
        Auth = null;
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
