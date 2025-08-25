using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model.Employees;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RestND.MVVM.ViewModel
{
    public partial class RoleViewModel : ObservableObject
    {
        private readonly RoleServices _roleService = new();

        [ObservableProperty] private ObservableCollection<Role> roles = new();
        [ObservableProperty] private Role selectedRole;
        [ObservableProperty] private string newRoleName = string.Empty;
        [ObservableProperty] private AppPermission newRolePermissions = AppPermission.None;
        [ObservableProperty] private string formErrorMessage;

        public event Action? RequestClose;

        public RoleViewModel()
        {
            LoadRoles();
            SelectedRole = Roles.FirstOrDefault();
        }

        [RelayCommand]
        private void LoadRoles() =>
            Roles = new ObservableCollection<Role>(_roleService.GetAll());

        [RelayCommand]
        private void AddRole()
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

            var role = new Role { Role_Name = NewRoleName.Trim(), Permissions = NewRolePermissions, Is_Active = true };
            if (_roleService.Add(role))
            {
                LoadRoles();
                NewRoleName = string.Empty;
                NewRolePermissions = AppPermission.None;
                RequestClose?.Invoke();
            }
            else FormErrorMessage = "Failed to create role.";
        }

        [RelayCommand(CanExecute = nameof(CanModifyRole))]
        private void UpdateRole()
        {
            if (SelectedRole == null) return;
            if (string.IsNullOrWhiteSpace(SelectedRole.Role_Name))
            {
                FormErrorMessage = "Role name is required."; return;
            }
            if (Roles.Any(r => r.Role_ID != SelectedRole.Role_ID &&
                               string.Equals(r.Role_Name, SelectedRole.Role_Name, StringComparison.OrdinalIgnoreCase)))
            {
                FormErrorMessage = "Role name already exists."; return;
            }
            if (_roleService.Update(SelectedRole)) LoadRoles();
            else FormErrorMessage = "Failed to update role.";
        }

        [RelayCommand(CanExecute = nameof(CanModifyRole))]
        private void DeleteRole()
        {
            if (SelectedRole == null) return;
            if (_roleService.Delete(SelectedRole)) { LoadRoles(); SelectedRole = null; }
            else FormErrorMessage = "Failed to delete role.";
        }

        private bool CanModifyRole() => SelectedRole != null;

        // checkbox toggles
        [RelayCommand]
        private void ToggleNewPermission(AppPermission flag)
        {
            if ((NewRolePermissions & flag) == flag) NewRolePermissions &= ~flag;
            else NewRolePermissions |= flag;
        }

        [RelayCommand]
        private void ToggleSelectedPermission(AppPermission flag)
        {
            if (SelectedRole == null) return;
            if ((SelectedRole.Permissions & flag) == flag) SelectedRole.Permissions &= ~flag;
            else SelectedRole.Permissions |= flag;
        }
    }
}
