using KppBlazor.Models;

namespace KppBlazor.Services;

public class ApiService
{
    private readonly DataStore _db;
    private AuthInfo? _auth;

    public ApiService(DataStore db) => _db = db;

    public AuthInfo? Auth => _auth;
    public void SetAuth(AuthInfo? auth) => _auth = auth;

    // ── Auth ─────────────────────────────────────────────
    public Task<AuthInfo> LoginAsync(string username, string password)
    {
        var user = _db.GetByCredentials(username, password)
            ?? throw new Exception("Неверный логин или пароль");

        if (user.IsBlocked) throw new Exception("403");

        var role = _db.GetRoles().FirstOrDefault(r => r.Id == user.RoleId) ?? new RoleItem();

        return Task.FromResult(new AuthInfo
        {
            Id = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName,
            RoleName = user.RoleName,
            Token = "local",
            CanCreateRequest = role.CanCreateRequest,
            CanViewHistory = role.CanViewHistory,
            CanManageEntry = role.CanManageEntry
        });
    }

    // ── Guests ───────────────────────────────────────────
    public Task<GuestListResponse> GetGuestsAsync(
        string search, string status, string dateFrom, string dateTo)
    {
        RequireAuth();
        return Task.FromResult(_db.QueryGuests(search, status, dateFrom, dateTo));
    }

    public Task GuestEntryAsync(int id)
    {
        RequireAuth();
        if (!_db.GuestEntry(id)) throw new Exception("Не удалось отметить вход");
        return Task.CompletedTask;
    }

    public Task GuestExitAsync(int id)
    {
        RequireAuth();
        if (!_db.GuestExit(id)) throw new Exception("Не удалось отметить выход");
        return Task.CompletedTask;
    }

    public Task CreateGuestAsync(GuestCreateRequest req)
    {
        RequireAuth();
        bool immediate = _auth!.CanManageEntry && _auth.RoleName != "admin";
        _db.AddGuest(req, _auth.Username, immediate);
        return Task.CompletedTask;
    }

    // ── Roles ─────────────────────────────────────────────
    public Task<List<RoleItem>> GetRolesAsync()
    {
        RequireAuth();
        return Task.FromResult(_db.GetRoles());
    }

    public Task CreateRoleAsync(
        string name, string displayName,
        bool canCreateRequest, bool canViewHistory, bool canManageEntry)
    {
        RequireAdmin();
        _db.AddRole(name, displayName, canCreateRequest, canViewHistory, canManageEntry);
        return Task.CompletedTask;
    }

    public Task UpdateRoleAsync(
        int id, string displayName,
        bool canCreateRequest, bool canViewHistory, bool canManageEntry)
    {
        RequireAdmin();
        if (!_db.UpdateRole(id, displayName, canCreateRequest, canViewHistory, canManageEntry))
            throw new Exception("Роль не найдена или является системной");
        return Task.CompletedTask;
    }

    public Task DeleteRoleAsync(int id)
    {
        RequireAdmin();
        if (!_db.DeleteRole(id)) throw new Exception("Роль не найдена или является системной");
        return Task.CompletedTask;
    }

    // ── Users ─────────────────────────────────────────────
    public Task<List<UserItem>> GetUsersAsync()
    {
        RequireAdmin();
        return Task.FromResult(_db.GetUsers());
    }

    public Task CreateUserAsync(
        string username, string password, string displayName, int roleId)
    {
        RequireAdmin();
        if (!_db.AddUser(username, password, displayName, roleId))
            throw new Exception("Пользователь с таким именем уже существует");
        return Task.CompletedTask;
    }

    public Task UpdatePasswordAsync(int id, string newPassword)
    {
        RequireAdmin();
        if (!_db.UpdateUser(id, newPassword, null))
            throw new Exception("Пользователь не найден");
        return Task.CompletedTask;
    }

    public Task UpdateBlockedAsync(int id, bool isBlocked)
    {
        RequireAdmin();
        if (!_db.UpdateUser(id, null, isBlocked))
            throw new Exception("Пользователь не найден");
        return Task.CompletedTask;
    }

    public Task DeleteUserAsync(int id)
    {
        RequireAdmin();
        if (!_db.DeleteUser(id)) throw new Exception("Пользователь не найден");
        return Task.CompletedTask;
    }

    // ── Guards ────────────────────────────────────────────
    private void RequireAuth()
    {
        if (_auth is null) throw new Exception("Требуется авторизация");
    }

    private void RequireAdmin()
    {
        RequireAuth();
        if (_auth!.RoleName != "admin")
            throw new Exception("Требуются права администратора");
    }
}