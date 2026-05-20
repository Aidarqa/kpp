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
    public Task<GuestListResponse> GetGuestsAsync(string search = "", string status = "all",
        string dateFrom = "", string dateTo = "")
    {
        return Task.FromResult(_store.QueryGuests(search, status, dateFrom, dateTo));
    }

    public Task<GuestItem?> GetGuestByIdAsync(int id)
    {
        var list = _store.QueryGuests("", "all", "", "");
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
        => Task.FromResult(false);

    // ── Roles ──────────────────────────────────────────
    // Основной метод, на который завязаны все перегрузки
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

    // Перегрузки, которые вызываются из Razor страниц

    // CreateRoleAsync (5 аргументов: name, displayName, canCreate, canViewHistory, canManageEntry)
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

    // UpdateRoleAsync (6 аргументов: id, name, displayName, canCreate, canViewHistory, canManageEntry) – вызывается из Roles.razor(230)
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

    // UpdateRoleAsync (5 аргументов без displayName) – вызывается из Roles.razor(230) с 5 параметрами? Оставим для совместимости
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

    // UpdateRoleAsync (5 аргументов без canManageEntry) – на всякий случай
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
}