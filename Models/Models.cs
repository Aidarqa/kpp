namespace KppBlazor.Models;

// ── AuthInfo ──────────────────────────────────────────
public class AuthInfo
{
    public string Id { get; set; } = "";
    public string Username { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string RoleName { get; set; } = "";
    public string RoleId { get; set; } = "";
    public bool CanCreateRequest { get; set; }
    public bool CanViewHistory { get; set; }
    public bool CanManageEntry { get; set; }
}

// ── Roles ──────────────────────────────────────────────
public class RoleItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public bool CanCreateRequest { get; set; }
    public bool CanViewHistory { get; set; }
    public bool CanManageEntry { get; set; }
    public bool IsSystem { get; set; }
}

// ── Users ──────────────────────────────────────────────
public class UserItem
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public int RoleId { get; set; }
    public string RoleName { get; set; } = "";
    public string RoleDisplayName { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public bool IsBlocked { get; set; }
    // Дополнительные флаги прав (дублируются из роли, но могут храниться здесь)
    public bool CanCreateRequest { get; set; }
    public bool CanViewHistory { get; set; }
    public bool CanManageEntry { get; set; }
    public string CreatedAt { get; set; } = "";
}

// ── Guests ─────────────────────────────────────────────
public class GuestItem
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string? Passport { get; set; }
    public string? Dob { get; set; }
    public string? Nationality { get; set; }
    public string? Purpose { get; set; }
    public string? Host { get; set; }
    public string? CarBrand { get; set; }
    public string? CarPlate { get; set; }
    public string Status { get; set; } = "pending";
    public string? EntryTime { get; set; }
    public string? ExitTime { get; set; }
    public string? PlannedDate { get; set; }
    public string? DurationFormatted { get; set; }
    public string? GroupId { get; set; }
    public string? CreatedBy { get; set; }
    public bool HasPassportScan { get; set; }
    public bool HasPermitDoc { get; set; }
    public string? PassportScanBase64 { get; set; }
    public string? PermitDocBase64 { get; set; }
    // Внутренние поля для расчёта длительности
    public DateTime? EntryDt { get; set; }
    public DateTime? ExitDt { get; set; }
}

public class GuestCreateRequest
{
    public string FullName { get; set; } = "";
    public string? Dob { get; set; }
    public string? Passport { get; set; }
    public string? Nationality { get; set; }
    public string Purpose { get; set; } = "";
    public string Host { get; set; } = "";
    public string? CarBrand { get; set; }
    public string? CarPlate { get; set; }
    public string? PlannedDate { get; set; }
    public string? GroupId { get; set; }
    public string? PassportScanBase64 { get; set; }
    public string? PermitDocBase64 { get; set; }
}

public class GuestListResponse
{
    public List<GuestItem> Items { get; set; } = new();
    public GuestSummary Summary { get; set; } = new();
}

public class GuestSummary
{
    public int TotalCount { get; set; }
    public int CompletedCount { get; set; }
    public int OngoingCount { get; set; }
    public string TotalDurationFormatted { get; set; } = "0 мин";
}

/// <summary>Одна строка в форме групповой заявки (Register.razor).</summary>
public class MemberForm
{
    public string FullName { get; set; } = "";
    public string Dob { get; set; } = "";
    public string Passport { get; set; } = "";
    public string Nationality { get; set; } = "КР";
}

// ── Audit Log ────────────────────────────────────────
public class AuditLogEntry
{
    public int Id { get; set; }
    public string Action { get; set; } = "";       // entry, exit, create, register, login, role_create, role_update, role_delete, user_create, user_block, user_unblock, user_delete, password_reset
    public string Description { get; set; } = "";  // Человекочитаемое описание
    public string PerformedBy { get; set; } = "";  // Username того, кто выполнил
    public string TargetName { get; set; } = "";   // Имя гостя / пользователя
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// ── Dashboard Stats ─────────────────────────────────
public class DashboardStats
{
    public int InsideCount { get; set; }
    public int TodayCount { get; set; }
    public int WeekCount { get; set; }
    public int PendingCount { get; set; }
    public int TotalCount { get; set; }
    public int ExitedTodayCount { get; set; }
    public List<DailyVisit> WeeklyVisits { get; set; } = new();
    public List<HourlyPeak> HourlyPeaks { get; set; } = new();
}

public class DailyVisit
{
    public string Date { get; set; } = "";  // dd.MM
    public string DayName { get; set; } = ""; // Пн, Вт, ...
    public int Count { get; set; }
}

public class HourlyPeak
{
    public int Hour { get; set; }  // 0-23
    public int Count { get; set; }
}