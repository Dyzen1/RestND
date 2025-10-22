using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;
using RestND.MVVM.Model.Employees;
using RestND.MVVM.Model.Security;
using RestND.utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RestND.MVVM.ViewModel
{
    public partial class RoleViewModel : ObservableObject, IDisposable
    {
        #region Services & Hub

        private readonly RoleServices _roleService = new();
        // ✅ Define the hub so _hub exists
        private readonly HubConnection _hub = App.EmployeeHub;

        #endregion

        #region SignalR Handler Disposable

        // Keep a disposable so we can detach the hub handler when needed
        private IDisposable? _roleHandler;

        #endregion

        #region Observable Properties

        [ObservableProperty] private ObservableCollection<Role> roles = new();

        // Never null — use a placeholder Role when there's no selection
        [ObservableProperty] private Role selectedRole = new();

        [ObservableProperty] private string newRoleName = string.Empty;
        [ObservableProperty] private AppPermission newRolePermissions = AppPermission.None;
        [ObservableProperty] private string formErrorMessage = string.Empty;

        #endregion // <-- this was missing before

        #region Events

        public event Action? RequestClose;

        #endregion

        #region Constructor

        public RoleViewModel()
        {
            LoadRoles();
            // If there are roles, pick the first; otherwise keep the placeholder
            SelectedRole = Roles.FirstOrDefault() ?? new Role();
        }

        #endregion

        #region Hub Registration

        // Call this once after the hub is started (e.g., after App.EmployeeHub.StartAsync())
        public void RegisterHubHandlers()
        {
            // SignalR → local update + Messenger broadcast
            _roleHandler = _hub.On<Role, string>("ReceiveRoleUpdate", (role, action) =>
            {
                var existing = Roles.FirstOrDefault(r => r.Role_ID == role.Role_ID);

                switch (action)
                {
                    case "add":
                        if (existing == null) Roles.Add(role);
                        break;

                    case "update":
                        if (existing != null)
                        {
                            existing.Role_Name = role.Role_Name;
                            existing.Permissions = role.Permissions;
                            existing.Is_Active = role.Is_Active;
                        }
                        break;

                    case "delete":
                        if (existing != null) Roles.Remove(existing);
                        // If we just deleted the currently selected role, move selection
                        if (SelectedRole.Role_ID == role.Role_ID)
                            SelectedRole = Roles.FirstOrDefault() ?? new Role();
                        break;
                }

                // In-process broadcast for other VMs (e.g., EmployeeViewModel)
                WeakReferenceMessenger.Default.Send(new RoleChangedMessage(role, action));
            });
        }

        public void UnregisterHubHandlers()
        {
            _roleHandler?.Dispose();
            _roleHandler = null;
        }

        #endregion

        #region Generated Hooks

        partial void OnSelectedRoleChanged(Role value)
        {
            // WPF DataGrid can push null via TwoWay SelectedItem; coalesce to a placeholder.
            if (value == null)
                SelectedRole = new Role();
        }

        #endregion

        #region Load Commands

        [RelayCommand]
        private void LoadRoles() =>
            Roles = new ObservableCollection<Role>(_roleService.GetAll());

        #endregion

        #region CRUD Commands

        [RelayCommand]
        private async Task AddRole()
        {
            if (string.IsNullOrWhiteSpace(NewRoleName))
            {
                FormErrorMessage = "Role name is required.";
                return;
            }

            if (Roles.Any(r => string.Equals(r.Role_Name, NewRoleName.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                FormErrorMessage = "Role name already exists.";
                return;
            }

            var role = new Role
            {
                Role_Name = NewRoleName.Trim(),
                Permissions = NewRolePermissions,
                Is_Active = true
            };

            if (_roleService.Add(role))
            {
                LoadRoles();

                // Fetch the created role (with its DB id)
                var created = Roles.FirstOrDefault(r =>
                    r.Role_Name.Equals(role.Role_Name, StringComparison.OrdinalIgnoreCase)) ?? role;

                AuthContext.ApplyRoleUpdate(created);

                // Cross-client broadcast (SignalR)
                await _hub.SendAsync("NotifyRoleUpdate", created, "add");
                // In-process broadcast (immediate)
                WeakReferenceMessenger.Default.Send(new RoleChangedMessage(created, "add"));

                // Reset form
                NewRoleName = string.Empty;
                NewRolePermissions = AppPermission.None;
                FormErrorMessage = string.Empty;

                // Optional: select the newly created role
                SelectedRole = created;

                RequestClose?.Invoke();
            }
            else
            {
                FormErrorMessage = "Failed to create role.";
            }
        }

        [RelayCommand(CanExecute = nameof(CanModifyRole))]
        private async Task UpdateRole()
        {
            // With placeholder pattern, SelectedRole is never null.
            if (SelectedRole.Role_ID == 0)
            {
                FormErrorMessage = "Select an existing role to update.";
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedRole.Role_Name))
            {
                FormErrorMessage = "Role name is required.";
                return;
            }

            if (Roles.Any(r => r.Role_ID != SelectedRole.Role_ID &&
                               string.Equals(r.Role_Name, SelectedRole.Role_Name, StringComparison.OrdinalIgnoreCase)))
            {
                FormErrorMessage = "Role name already exists.";
                return;
            }

            if (_roleService.Update(SelectedRole))
            {
                LoadRoles();
                var updated = Roles.FirstOrDefault(r => r.Role_ID == SelectedRole.Role_ID) ?? SelectedRole;
                AuthContext.ApplyRoleUpdate(updated);

                await _hub.SendAsync("NotifyRoleUpdate", updated, "update");
                WeakReferenceMessenger.Default.Send(new RoleChangedMessage(updated, "update"));

                // Keep selection on the updated role snapshot
                SelectedRole = updated;

                FormErrorMessage = string.Empty;
            }
            else
            {
                FormErrorMessage = "Failed to update role.";
            }
        }

        [RelayCommand(CanExecute = nameof(CanModifyRole))]
        private async Task DeleteRole()
        {
            if (SelectedRole.Role_ID == 0)
            {
                FormErrorMessage = "Select an existing role to delete.";
                return;
            }

            var toDelete = SelectedRole;

            if (_roleService.Delete(toDelete))
            {
                // If current user’s role was deleted, clear their role
                AuthContext.ApplyRoleUpdate(new Role
                {
                    Role_ID = toDelete.Role_ID,
                    Role_Name = toDelete.Role_Name,
                    Permissions = AppPermission.None,
                    Is_Active = false
                });

                await _hub.SendAsync("NotifyRoleUpdate", toDelete, "delete");
                WeakReferenceMessenger.Default.Send(new RoleChangedMessage(toDelete, "delete"));

                LoadRoles();
                SelectedRole = Roles.FirstOrDefault() ?? new Role();
            }
            else
            {
                FormErrorMessage = "Failed to delete role.";
            }
        }

        #endregion

        #region Helpers

        private bool CanModifyRole()
        {
            // With placeholder, always non-null; only allow modify when it's a persisted role
            return SelectedRole.Role_ID != 0;
        }

        // Checkbox toggle for *new* role form
        [RelayCommand]
        private void ToggleNewPermission(AppPermission flag)
        {
            if ((NewRolePermissions & flag) == flag) NewRolePermissions &= ~flag;
            else NewRolePermissions |= flag;
        }

        // Checkbox toggle for the selected role (placeholder allowed to toggle, but update is blocked by CanExecute)
        [RelayCommand]
        private void ToggleSelectedPermission(AppPermission flag)
        {
            if ((SelectedRole.Permissions & flag) == flag) SelectedRole.Permissions &= ~flag;
            else SelectedRole.Permissions |= flag;
        }

        #endregion

        #region IDisposable

        public void Dispose() => UnregisterHubHandlers();

        #endregion
    }
}
