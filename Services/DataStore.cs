using KppBlazor.Models;

namespace KppBlazor.Services;

/// <summary>
/// In-memory БД. Регистрируется как AddSingleton в Program.cs.
/// Хранит роли, пользователей и гостей без каких-либо HTTP-запросов.
/// </summary>
public class DataStore
{
    // ── Таблицы ─────────────────────────────────────────
    private readonly List<RoleItem> _roles = new();
    private readonly List<UserItem> _users = new();
    private readonly List<GuestItem> _guests = new();

    private int _roleSeq = 1;
    private int _userSeq = 1;
    private int _guestSeq = 1;

    // ── Seed ────────────────────────────────────────────
    public DataStore()
    {
        SeedRoles();
        SeedUsers();
        SeedGuests();
    }

    private void SeedRoles()
    {
        _roles.AddRange(new[]
        {
            new RoleItem
            {
                Id = _roleSeq++, Name = "admin", DisplayName = "Администратор",
                CanCreateRequest = true, CanViewHistory = true, CanManageEntry = true,
                IsSystem = true
            },
            new RoleItem
            {
                Id = _roleSeq++, Name = "kpp", DisplayName = "Сотрудник КПП",
                CanCreateRequest = true, CanViewHistory = true, CanManageEntry = true,
                IsSystem = true
            },
            new RoleItem
            {
                Id = _roleSeq++, Name = "user", DisplayName = "Пользователь",
                CanCreateRequest = true, CanViewHistory = false, CanManageEntry = false,
                IsSystem = false
            }
        });
    }

    private void SeedUsers()
    {
    _users.AddRange(new[]
    {
        new UserItem
        {
            Id = _userSeq++, Username = "admin", DisplayName = "Администратор",
            RoleName = "admin", RoleDisplayName = "Администратор", RoleId = 1,
            PasswordHash = Hash("admin123"), CreatedAt = Iso(DateTime.UtcNow),
            CanCreateRequest = true, CanViewHistory = true, CanManageEntry = true
        },
        new UserItem
        {
            Id = _userSeq++, Username = "kpp", DisplayName = "Исанов КПП",
            RoleName = "kpp", RoleDisplayName = "Сотрудник КПП", RoleId = 2,
            PasswordHash = Hash("kpp123"), CreatedAt = Iso(DateTime.UtcNow),
            CanCreateRequest = true, CanViewHistory = true, CanManageEntry = true
        },
        new UserItem
        {
            Id = _userSeq++, Username = "it_user", DisplayName = "Исанов И.И.",
            RoleName = "user", RoleDisplayName = "Пользователь", RoleId = 3,
            PasswordHash = Hash("it123"), CreatedAt = Iso(DateTime.UtcNow),
            CanCreateRequest = true, CanViewHistory = false, CanManageEntry = false
        }
        });
    }

    private void SeedGuests()
    {
        var now = DateTime.UtcNow;
        _guests.AddRange(new[]
        {
            new GuestItem
            {
                Id = _guestSeq++, FullName = "Асанов Тилек Петрович",
                Passport = "4521 123456", Dob = "15.03.1985", Nationality = "КР",
                Purpose = "Деловая встреча", Host = "Директор Алмазов Б.С.",
                Status = "inside", EntryTime = Iso(now.AddHours(-1)),
                EntryDt = now.AddHours(-1), PlannedDate = Iso(now),
                CreatedBy = "kpp", HasPassportScan = true
            },
            new GuestItem
            {
                Id = _guestSeq++, FullName = "Камчыбекова Мариям Амановна",
                Passport = "4522 654321", Dob = "20.07.1990", Nationality = "КР",
                Purpose = "Собеседование", Host = "HR Отдел",
                Status = "pending", PlannedDate = Iso(now.AddHours(2)),
                CreatedBy = "it_user"
            },
            new GuestItem
            {
                Id = _guestSeq++, FullName = "Ли Вэй",
                Nationality = "КНР", Purpose = "Переговоры", Host = "Коммерческий директор",
                CarBrand = "Toyota", CarPlate = "А123ВС77",
                Status = "exited",
                EntryTime = Iso(now.AddHours(-3)), ExitTime = Iso(now.AddHours(-1)),
                EntryDt = now.AddHours(-3), ExitDt = now.AddHours(-1),
                CreatedBy = "admin", HasPermitDoc = true
            }
        });

        // пересчитать длительность у seed-записей
        foreach (var g in _guests) RefreshDuration(g);
    }

    // ── Roles CRUD ──────────────────────────────────────
    public List<RoleItem> GetRoles() => _roles.ToList();

    public RoleItem AddRole(string name, string displayName,
        bool canCreate, bool canHistory, bool canEntry)
    {
        var r = new RoleItem
        {
            Id = _roleSeq++,
            Name = name,
            DisplayName = displayName,
            CanCreateRequest = canCreate,
            CanViewHistory = canHistory,
            CanManageEntry = canEntry,
            IsSystem = false
        };
        _roles.Add(r);
        return r;
    }

    public bool UpdateRole(int id, string displayName,
        bool canCreate, bool canHistory, bool canEntry)
    {
        var r = _roles.FirstOrDefault(x => x.Id == id);
        if (r is null || r.IsSystem) return false;
        r.DisplayName = displayName;
        r.CanCreateRequest = canCreate;
        r.CanViewHistory = canHistory;
        r.CanManageEntry = canEntry;
        return true;
    }

    public bool DeleteRole(int id)
    {
        var r = _roles.FirstOrDefault(x => x.Id == id);
        if (r is null || r.IsSystem) return false;
        _roles.Remove(r);
        return true;
    }

    // ── Users CRUD ──────────────────────────────────────
    public List<UserItem> GetUsers() => _users.ToList();

    public UserItem? GetByCredentials(string username, string password)
    {
        var hash = Hash(password);
        return _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            u.PasswordHash == hash &&
            !u.IsBlocked);
    }

    public bool AddUser(string username, string password, string displayName, int roleId)
    {
        if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            return false;
        var role = _roles.FirstOrDefault(r => r.Id == roleId);
        _users.Add(new UserItem
        {
            Id = _userSeq++,
            Username = username,
            DisplayName = displayName,
            RoleId = roleId,
            RoleName = role?.Name ?? "",
            RoleDisplayName = role?.DisplayName ?? "",
            PasswordHash = Hash(password),
            CreatedAt = Iso(DateTime.UtcNow),
            CanCreateRequest = role?.CanCreateRequest ?? false,
            CanViewHistory   = role?.CanViewHistory   ?? false,
            CanManageEntry   = role?.CanManageEntry    ?? false
        });
        return true;
    }

    public bool UpdateUser(int id, string? newPassword, bool? isBlocked)
    {
        var u = _users.FirstOrDefault(x => x.Id == id);
        if (u is null) return false;
        if (newPassword is not null) u.PasswordHash = Hash(newPassword);
        if (isBlocked is not null) u.IsBlocked = isBlocked.Value;
        return true;
    }

    public bool DeleteUser(int id)
    {
        var u = _users.FirstOrDefault(x => x.Id == id);
        if (u is null) return false;
        _users.Remove(u);
        return true;
    }

    // ── Guests CRUD ──────────────────────────────────────
    public GuestListResponse QueryGuests(
        string search, string status, string dateFrom, string dateTo)
    {
        // обновить длительность у тех, кто "inside"
        foreach (var g in _guests.Where(g => g.Status == "inside"))
            RefreshDuration(g);

        var q = _guests.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            q = q.Where(g =>
                g.FullName.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                (g.Passport?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (g.CarPlate?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                g.Status.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        if (status != "all")
            q = q.Where(g => g.Status == status);

        if (DateTime.TryParse(dateFrom, out var df))
            q = q.Where(g => ParseIso(g.EntryTime ?? g.PlannedDate) >= df);

        if (DateTime.TryParse(dateTo, out var dt))
            q = q.Where(g => ParseIso(g.EntryTime ?? g.PlannedDate) <= dt.AddDays(1));

        var list = q.OrderByDescending(g =>
                ParseIso(g.EntryTime ?? g.PlannedDate) ?? DateTime.MinValue)
            .ToList();

        var totalMin = list
            .Where(g => g.ExitDt.HasValue && g.EntryDt.HasValue)
            .Sum(g => (g.ExitDt!.Value - g.EntryDt!.Value).TotalMinutes);

        return new GuestListResponse
        {
            Items = list,
            Summary = new GuestSummary
            {
                TotalCount = list.Count,
                CompletedCount = list.Count(g => g.Status == "exited"),
                OngoingCount = list.Count(g => g.Status == "inside"),
                TotalDurationFormatted = FmtMinutes((int)totalMin)
            }
        };
    }

    public GuestItem AddGuest(GuestCreateRequest req, string createdBy, bool registerImmediately)
    {
        var now = DateTime.UtcNow;
        var g = new GuestItem
        {
            Id = _guestSeq++,
            FullName = req.FullName,
            Passport = req.Passport,
            Dob = req.Dob,
            Nationality = req.Nationality,
            Purpose = req.Purpose,
            Host = req.Host,
            CarBrand = req.CarBrand,
            CarPlate = req.CarPlate,
            PlannedDate = req.PlannedDate,
            GroupId = req.GroupId,
            CreatedBy = createdBy,
            HasPassportScan = req.PassportScanBase64 is not null,
            HasPermitDoc = req.PermitDocBase64 is not null,
            PassportScanBase64 = req.PassportScanBase64,
            PermitDocBase64 = req.PermitDocBase64
        };

        if (registerImmediately)
        {
            g.Status = "inside";
            g.EntryDt = now;
            g.EntryTime = Iso(now);
        }
        else
        {
            g.Status = "pending";
        }

        _guests.Add(g);
        return g;
    }

    public bool GuestEntry(int id)
    {
        var g = _guests.FirstOrDefault(x => x.Id == id);
        if (g is null || g.Status != "pending") return false;
        var now = DateTime.UtcNow;
        g.Status = "inside";
        g.EntryDt = now;
        g.EntryTime = Iso(now);
        RefreshDuration(g);
        return true;
    }

    public bool GuestExit(int id)
    {
        var g = _guests.FirstOrDefault(x => x.Id == id);
        if (g is null || g.Status != "inside") return false;
        var now = DateTime.UtcNow;
        g.Status = "exited";
        g.ExitDt = now;
        g.ExitTime = Iso(now);
        RefreshDuration(g);
        return true;
    }

    // ── Helpers ─────────────────────────────────────────
    private static void RefreshDuration(GuestItem g)
    {
        if (g.EntryDt is null) return;
        var end = g.ExitDt ?? DateTime.UtcNow;
        var min = (int)(end - g.EntryDt.Value).TotalMinutes;
        g.DurationFormatted = FmtMinutes(min);
    }

    private static string FmtMinutes(int min) =>
        min < 60 ? $"{min} мин" : $"{min / 60} ч {min % 60} мин";

    private static string Iso(DateTime dt) =>
        dt.ToString("yyyy-MM-ddTHH:mm:ss");

    private static DateTime? ParseIso(string? s) =>
        DateTime.TryParse(s, out var dt) ? dt : null;

    public static string Hash(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        return Convert.ToHexString(sha.ComputeHash(bytes));
    }
}