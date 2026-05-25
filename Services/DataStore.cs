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
    private readonly List<AuditLogEntry> _auditLog = new();

    private int _roleSeq = 1;
    private int _userSeq = 1;
    private int _guestSeq = 1;
    private int _auditSeq = 1;

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
                Id = _roleSeq++, Name = RoleNames.Admin, DisplayName = "Администратор",
                CanCreateRequest = true, CanViewHistory = true, CanManageEntry = true,
                IsSystem = true
            },
            new RoleItem
            {
                Id = _roleSeq++, Name = RoleNames.KPP, DisplayName = "Сотрудник КПП",
                CanCreateRequest = true, CanViewHistory = true, CanManageEntry = true,
                IsSystem = true
            },
            new RoleItem
            {
                Id = _roleSeq++, Name = RoleNames.User, DisplayName = "Пользователь",
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
            RoleName = RoleNames.Admin, RoleDisplayName = "Администратор", RoleId = 1,
            PasswordHash = HashPassword("admin123", out var salt1),
            PasswordSalt = salt1, CreatedAt = Iso(DateTime.UtcNow),
            CanCreateRequest = true, CanViewHistory = true, CanManageEntry = true
        },
        new UserItem
        {
            Id = _userSeq++, Username = "kpp", DisplayName = "Исанов КПП",
            RoleName = RoleNames.KPP, RoleDisplayName = "Сотрудник КПП", RoleId = 2,
            PasswordHash = HashPassword("kpp123", out var salt2),
            PasswordSalt = salt2, CreatedAt = Iso(DateTime.UtcNow),
            CanCreateRequest = true, CanViewHistory = true, CanManageEntry = true
        },
        new UserItem
        {
            Id = _userSeq++, Username = "it_user", DisplayName = "Исанов И.И.",
            RoleName = RoleNames.User, RoleDisplayName = "Пользователь", RoleId = 3,
            PasswordHash = HashPassword("it123", out var salt3),
            PasswordSalt = salt3, CreatedAt = Iso(DateTime.UtcNow),
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
                Status = GuestStatus.Inside, EntryTime = Iso(now.AddHours(-1)),
                EntryDt = now.AddHours(-1), PlannedDate = Iso(now),
                CreatedBy = RoleNames.KPP, HasPassportScan = true
            },
            new GuestItem
            {
                Id = _guestSeq++, FullName = "Камчыбекова Мариям Амановна",
                Passport = "4522 654321", Dob = "20.07.1990", Nationality = "КР",
                Purpose = "Собеседование", Host = "HR Отдел",
                Status = GuestStatus.Pending, PlannedDate = Iso(now.AddHours(2)),
                CreatedBy = "it_user"
            },
            new GuestItem
            {
                Id = _guestSeq++, FullName = "Ли Вэй",
                Nationality = "КНР", Purpose = "Переговоры", Host = "Коммерческий директор",
                CarBrand = "Toyota", CarPlate = "А123ВС77",
                Status = GuestStatus.Exited,
                EntryTime = Iso(now.AddHours(-3)), ExitTime = Iso(now.AddHours(-1)),
                EntryDt = now.AddHours(-3), ExitDt = now.AddHours(-1),
                CreatedBy = RoleNames.Admin, HasPermitDoc = true
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
        var user = _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            !u.IsBlocked);

        if (user is null) return null;

        // Verify password with salt
        var hash = HashPassword(password, user.PasswordSalt);
        return hash == user.PasswordHash ? user : null;
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
            PasswordHash = HashPassword(password, out var salt),
            PasswordSalt = salt,
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
        if (newPassword is not null)
        {
            u.PasswordHash = HashPassword(newPassword, u.PasswordSalt);
        }
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
        foreach (var g in _guests.Where(g => g.Status == GuestStatus.Inside))
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

        if (status != GuestStatus.All)
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
                CompletedCount = list.Count(g => g.Status == GuestStatus.Exited),
                OngoingCount = list.Count(g => g.Status == GuestStatus.Inside),
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
            g.Status = GuestStatus.Inside;
            g.EntryDt = now;
            g.EntryTime = Iso(now);
            AddAuditEntry("register", $"Гость зарегистрирован: {g.FullName}", createdBy, g.FullName);
        }
        else
        {
            g.Status = GuestStatus.Pending;
            AddAuditEntry("create", $"Создана заявка: {g.FullName}", createdBy, g.FullName);
        }

        _guests.Add(g);
        return g;
    }

    public bool GuestEntry(int id)
    {
        var g = _guests.FirstOrDefault(x => x.Id == id);
        if (g is null || g.Status != GuestStatus.Pending) return false;
        var now = DateTime.UtcNow;
        g.Status = GuestStatus.Inside;
        g.EntryDt = now;
        g.EntryTime = Iso(now);
        RefreshDuration(g);
        AddAuditEntry("entry", $"Отмечен въезд: {g.FullName}", "system", g.FullName);
        return true;
    }

    public bool GuestExit(int id)
    {
        var g = _guests.FirstOrDefault(x => x.Id == id);
        if (g is null || g.Status != GuestStatus.Inside) return false;
        var now = DateTime.UtcNow;
        g.Status = GuestStatus.Exited;
        g.ExitDt = now;
        g.ExitTime = Iso(now);
        RefreshDuration(g);
        AddAuditEntry("exit", $"Отмечен выезд: {g.FullName}", "system", g.FullName);
        return true;
    }

    public bool UpdateGuest(int id, GuestCreateRequest updates, string updatedBy)
    {
        var g = _guests.FirstOrDefault(x => x.Id == id);
        if (g is null) return false;

        if (!string.IsNullOrWhiteSpace(updates.FullName)) g.FullName = updates.FullName;
        if (updates.Passport != null) g.Passport = updates.Passport;
        if (updates.Dob != null) g.Dob = updates.Dob;
        if (updates.Nationality != null) g.Nationality = updates.Nationality;
        if (!string.IsNullOrWhiteSpace(updates.Purpose)) g.Purpose = updates.Purpose;
        if (!string.IsNullOrWhiteSpace(updates.Host)) g.Host = updates.Host;
        if (updates.CarBrand != null) g.CarBrand = updates.CarBrand;
        if (updates.CarPlate != null) g.CarPlate = updates.CarPlate;
        if (updates.PlannedDate != null) g.PlannedDate = updates.PlannedDate;
        if (updates.PassportScanBase64 != null)
        {
            g.PassportScanBase64 = updates.PassportScanBase64;
            g.HasPassportScan = true;
        }
        if (updates.PermitDocBase64 != null)
        {
            g.PermitDocBase64 = updates.PermitDocBase64;
            g.HasPermitDoc = true;
        }

        AddAuditEntry("update", $"Обновлены данные гостя: {g.FullName}", updatedBy, g.FullName);
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

    /// <summary>
    /// Генерирует соль и хеширует пароль с использованием SHA-256.
    /// Возвращает хеш в виде hex-строки. Соль записывается в out-параметр.
    /// </summary>
    public static string HashPassword(string password, out string salt)
    {
        salt = GenerateSalt();
        return HashWithSalt(password, salt);
    }

    /// <summary>
    /// Хеширует пароль с использованием существующей соли.
    /// Используется при проверке пароля.
    /// </summary>
    public static string HashPassword(string password, string existingSalt)
    {
        return HashWithSalt(password, existingSalt);
    }

    /// <summary>
    /// Генерирует случайную соль (16 байт в hex).
    /// </summary>
    private static string GenerateSalt()
    {
        var saltBytes = new byte[16];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToHexString(saltBytes);
    }

    /// <summary>
    /// SHA-256(salt_bytes + password_bytes) — prevents rainbow table attacks.
    /// </summary>
    private static string HashWithSalt(string password, string salt)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var saltBytes = Convert.FromHexString(salt);
        var passBytes = System.Text.Encoding.UTF8.GetBytes(password);
        var combined = new byte[saltBytes.Length + passBytes.Length];
        Buffer.BlockCopy(saltBytes, 0, combined, 0, saltBytes.Length);
        Buffer.BlockCopy(passBytes, 0, combined, saltBytes.Length, passBytes.Length);
        return Convert.ToHexString(sha.ComputeHash(combined));
    }

    // ── Audit Log ────────────────────────────────────────
    public void AddAuditEntry(string action, string description, string performedBy, string targetName)
    {
        _auditLog.Add(new AuditLogEntry
        {
            Id = _auditSeq++,
            Action = action,
            Description = description,
            PerformedBy = performedBy,
            TargetName = targetName,
            Timestamp = DateTime.UtcNow
        });
    }

    public List<AuditLogEntry> GetAuditLog(int count = 50)
        => _auditLog.OrderByDescending(a => a.Timestamp).Take(count).ToList();

    // ── Dashboard Stats ──────────────────────────────────
    public DashboardStats GetDashboardStats()
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var weekAgo = today.AddDays(-6);

        var allGuests = _guests;
        var insideCount = allGuests.Count(g => g.Status == GuestStatus.Inside);
        var pendingCount = allGuests.Count(g => g.Status == GuestStatus.Pending);

        // Сегодня
        var todayGuests = allGuests.Where(g =>
        {
            var dt = ParseIso(g.EntryTime ?? g.PlannedDate);
            return dt.HasValue && dt.Value.Date == today;
        }).ToList();

        var exitedToday = allGuests.Count(g =>
        {
            var dt = ParseIso(g.ExitTime);
            return dt.HasValue && dt.Value.Date == today;
        });

        // За неделю
        var weekGuests = allGuests.Where(g =>
        {
            var dt = ParseIso(g.EntryTime ?? g.PlannedDate);
            return dt.HasValue && dt.Value.Date >= weekAgo;
        }).ToList();

        // Визиты по дням (последние 7 дней)
        var dayNames = new[] { "Вс", "Пн", "Вт", "Ср", "Чт", "Пт", "Сб" };
        var weeklyVisits = new List<DailyVisit>();
        for (int i = 6; i >= 0; i--)
        {
            var day = today.AddDays(-i);
            var count = allGuests.Count(g =>
            {
                var dt = ParseIso(g.EntryTime ?? g.PlannedDate);
                return dt.HasValue && dt.Value.Date == day;
            });
            weeklyVisits.Add(new DailyVisit
            {
                Date = day.ToString("dd.MM"),
                DayName = dayNames[(int)day.DayOfWeek],
                Count = count
            });
        }

        // Часы пик
        var hourlyPeaks = new List<HourlyPeak>();
        for (int h = 0; h < 24; h++)
        {
            var hour = h;
            var count = allGuests.Count(g =>
            {
                var dt = ParseIso(g.EntryTime);
                return dt.HasValue && dt.Value.Hour == hour;
            });
            if (count > 0)
                hourlyPeaks.Add(new HourlyPeak { Hour = hour, Count = count });
        }

        return new DashboardStats
        {
            InsideCount = insideCount,
            TodayCount = todayGuests.Count,
            WeekCount = weekGuests.Count,
            PendingCount = pendingCount,
            TotalCount = allGuests.Count,
            ExitedTodayCount = exitedToday,
            WeeklyVisits = weeklyVisits,
            HourlyPeaks = hourlyPeaks
        };
    }
}
