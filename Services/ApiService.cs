using KppBlazor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KppBlazor.Services;

public class ApiService
{
    private readonly DataStore _store;

    public AuthInfo? CurrentAuth { get; private set; }

    public ApiService(DataStore store)
    {
        _store = store;
    }

    public void SetAuth(AuthInfo? auth) => CurrentAuth = auth;

    // ── Auth ──────────────────────────────────────────
    public Task<AuthInfo?> LoginAsync(string username, string password)
    {
        var user = _store.GetByCredentials(username, password);
        if (user is null) return Task.FromResult<AuthInfo?>(null);

        var auth = new AuthInfo
        {
            Id = user.Id.ToString(),
            Username = user.Username,
            DisplayName = user.DisplayName,
            RoleName = user.RoleName,
            RoleId = user.RoleId.ToString(),
            CanCreateRequest = user.CanCreateRequest,
            CanViewHistory = user.CanViewHistory,
            CanManageEntry = user.CanManageEntry
        };
        return Task.FromResult<AuthInfo?>(auth);
    }

    // ── Guests ────────────────────────────────────────
    public Task<GuestListResponse> GetGuestsAsync(string search = "", string status = GuestStatus.All,
        string dateFrom = "", string dateTo = "")
    {
        return Task.FromResult(_store.QueryGuests(search, status, dateFrom, dateTo));
    }

    public Task<GuestItem?> GetGuestByIdAsync(int id)
    {
        var list = _store.QueryGuests("", GuestStatus.All, "", "");
        return Task.FromResult(list.Items.FirstOrDefault(g => g.Id == id));
    }

    public Task<GuestItem> CreateGuestAsync(GuestCreateRequest req, string createdBy = "system", bool registerImmediately = false)
    {
        var guest = _store.AddGuest(req, createdBy, registerImmediately);
        return Task.FromResult(guest);
    }

    public Task<bool> GuestEntryAsync(int guestId)
        => Task.FromResult(_store.GuestEntry(guestId));

    public Task<bool> GuestExitAsync(int guestId)
        => Task.FromResult(_store.GuestExit(guestId));

    public Task<bool> UpdateGuestAsync(GuestItem updated)
    {
        var req = new GuestCreateRequest
        {
            FullName = updated.FullName,
            Passport = updated.Passport,
            Dob = updated.Dob,
            Nationality = updated.Nationality,
            Purpose = updated.Purpose,
            Host = updated.Host,
            CarBrand = updated.CarBrand,
            CarPlate = updated.CarPlate,
            PlannedDate = updated.PlannedDate,
            PassportScanBase64 = updated.PassportScanBase64,
            PermitDocBase64 = updated.PermitDocBase64
        };
        var updatedBy = CurrentAuth?.Username ?? "system";
        return Task.FromResult(_store.UpdateGuest(updated.Id, req, updatedBy));
    }

    // ── Roles ──────────────────────────────────────────
    private Task<bool> SaveRoleInternal(RoleItem role)
    {
        if (role.Id == 0)
        {
            var newRole = _store.AddRole(role.Name, role.DisplayName,
                role.CanCreateRequest, role.CanViewHistory, role.CanManageEntry);
            role.Id = newRole.Id;
            return Task.FromResult(true);
        }
        else
        {
            return Task.FromResult(_store.UpdateRole(role.Id, role.DisplayName,
                role.CanCreateRequest, role.CanViewHistory, role.CanManageEntry));
        }
    }

    public Task<bool> SaveRoleAsync(RoleItem role) => SaveRoleInternal(role);

    public Task<bool> CreateRoleAsync(string name, string displayName,
        bool canCreate, bool canViewHistory, bool canManageEntry)
    {
        var role = new RoleItem
        {
            Name = name,
            DisplayName = displayName,
            CanCreateRequest = canCreate,
            CanViewHistory = canViewHistory,
            CanManageEntry = canManageEntry
        };
        return SaveRoleInternal(role);
    }

    public Task<bool> UpdateRoleAsync(int id, string name, string displayName,
        bool canCreate, bool canViewHistory, bool canManageEntry)
    {
        var role = new RoleItem
        {
            Id = id,
            Name = name,
            DisplayName = displayName,
            CanCreateRequest = canCreate,
            CanViewHistory = canViewHistory,
            CanManageEntry = canManageEntry
        };
        return SaveRoleInternal(role);
    }

    public Task<bool> UpdateRoleAsync(int id, string name, bool canCreate, bool canViewHistory, bool canManageEntry)
    {
        var role = new RoleItem
        {
            Id = id,
            Name = name,
            DisplayName = name,
            CanCreateRequest = canCreate,
            CanViewHistory = canViewHistory,
            CanManageEntry = canManageEntry
        };
        return SaveRoleInternal(role);
    }

    public Task<bool> UpdateRoleAsync(int id, string name, string displayName,
        bool canCreate, bool canViewHistory)
    {
        var role = new RoleItem
        {
            Id = id,
            Name = name,
            DisplayName = displayName,
            CanCreateRequest = canCreate,
            CanViewHistory = canViewHistory,
            CanManageEntry = false
        };
        return SaveRoleInternal(role);
    }

    /// <summary>
    /// Обновляет только DisplayName и права роли (без изменения системного имени).
    /// Используется при inline-редактировании в таблице ролей.
    /// </summary>
    public Task<bool> UpdateRoleDisplayNameAsync(int id, string displayName,
        bool canCreate, bool canViewHistory, bool canManageEntry)
    {
        return Task.FromResult(_store.UpdateRole(id, displayName, canCreate, canViewHistory, canManageEntry));
    }

    public Task<List<RoleItem>> GetRolesAsync()
        => Task.FromResult(_store.GetRoles());

    public Task<bool> DeleteRoleAsync(int roleId)
        => Task.FromResult(_store.DeleteRole(roleId));

    // ── Users ──────────────────────────────────────────
    public Task<List<UserItem>> GetUsersAsync()
        => Task.FromResult(_store.GetUsers());

    public Task<bool> CreateUserAsync(string username, string password,
        string displayName, int roleId)
    {
        return Task.FromResult(_store.AddUser(username, password, displayName, roleId));
    }

    public Task<bool> CreateUserAsync(UserItem user, string password)
        => Task.FromResult(_store.AddUser(user.Username, password, user.DisplayName, user.RoleId));

    public Task<bool> UpdateUserAsync(UserItem user)
        => Task.FromResult(_store.UpdateUser(user.Id, null, user.IsBlocked));

    public Task<bool> UpdatePasswordAsync(int userId, string newPassword)
        => Task.FromResult(_store.UpdateUser(userId, newPassword, null));

    public Task<bool> UpdateBlockedAsync(int userId, bool isBlocked)
        => Task.FromResult(_store.UpdateUser(userId, null, isBlocked));

    public Task<bool> ToggleUserBlockAsync(int userId)
    {
        var user = _store.GetUsers().FirstOrDefault(u => u.Id == userId);
        if (user is null) return Task.FromResult(false);
        return Task.FromResult(_store.UpdateUser(userId, null, !user.IsBlocked));
    }

    public Task<bool> DeleteUserAsync(int userId)
        => Task.FromResult(_store.DeleteUser(userId));

    // ── Audit Log ─────────────────────────────────────────
    public Task<List<AuditLogEntry>> GetAuditLogAsync(int count = 50)
        => Task.FromResult(_store.GetAuditLog(count));

    public void AddAuditEntry(string action, string description, string performedBy, string targetName)
        => _store.AddAuditEntry(action, description, performedBy, targetName);

    // ── Dashboard ─────────────────────────────────────────
    public Task<DashboardStats> GetDashboardStatsAsync()
        => Task.FromResult(_store.GetDashboardStats());
}
