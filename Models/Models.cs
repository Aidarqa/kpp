namespace KppBlazor.Models;

// ─── Auth ───────────────────────────────────────────────
public class AuthInfo
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string RoleName { get; set; } = "";
    public string Token { get; set; } = "local";
    public bool CanCreateRequest { get; set; }
    public bool CanViewHistory { get; set; }
    public bool CanManageEntry { get; set; }
    public bool IsBlocked { get; set; }
}

// ─── Roles ──────────────────────────────────────────────
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

// ─── Users ──────────────────────────────────────────────
public class UserItem
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string RoleName { get; set; } = "";
    public string RoleDisplayName { get; set; } = "";
    public int RoleId { get; set; }
    public bool IsBlocked { get; set; }
    public string CreatedAt { get; set; } = "";
    public string PasswordHash { get; set; } = "";
}

// ─── Guests ─────────────────────────────────────────────
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
    // внутренние поля для расчёта длительности
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

// ─── Register form helpers ───────────────────────────────
/// <summary>Одна строка в форме групповой заявки (Register.razor).</summary>
public class MemberForm
{
    public string FullName { get; set; } = "";
    public string Dob { get; set; } = "";
    public string Passport { get; set; } = "";
    public string Nationality { get; set; } = "КР";
}