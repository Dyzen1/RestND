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

        [ObservableProperty] private string newRoleName;
        [ObservableProperty] private AuthorizationStatus selectedAuthorization;

        [ObservableProperty]
        private ObservableCollection<AuthorizationStatus> authorizationOptions =
            new(Enum.GetValues(typeof(AuthorizationStatus)).Cast<AuthorizationStatus>());

        [ObservableProperty] private string formErrorMessage;

        public event Action? RequestClose;

        public RoleViewModel()
        {
            SelectedAuthorization = authorizationOptions.FirstOrDefault();
            LoadRoles();
            // Optional: select first row to enable Update/Delete immediately
            SelectedRole = Roles.FirstOrDefault();
        }

        // ?? RE-ENABLED: keep buttons in sync with selection
        partial void OnSelectedRoleChanged(Role value)
        {
            DeleteRoleCommand.NotifyCanExecuteChanged();
            UpdateRoleCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void LoadRoles()
        {
            Roles.Clear();
            foreach (var r in _roleService.GetAll())
                Roles.Add(r);
        }

        [RelayCommand]
        private void AddRole()
        {
            FormErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(NewRoleName))
            {
                FormErrorMessage = "Role name is required.";
                return;
            }

            bool exists = _roleService.GetAll()
                .Any(r => string.Equals(r.Role_Name, NewRoleName, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                FormErrorMessage = "Role name already exists.";
                return;
            }

            var role = new Role
            {
                Role_Name = NewRoleName.Trim(),
                Role_Authorization = SelectedAuthorization,
                Is_Active = true
            };

            if (_roleService.Add(role))
            {
                LoadRoles();
                NewRoleName = string.Empty;
                SelectedAuthorization = authorizationOptions.FirstOrDefault();
                RequestClose?.Invoke(); // if opened as dialog
            }
            else
            {
                FormErrorMessage = "Failed to create role.";
            }
        }

        [RelayCommand(CanExecute = nameof(CanModifyRole))]
        private void UpdateRole()
        {
            FormErrorMessage = string.Empty;

            if (SelectedRole == null) return;

            if (string.IsNullOrWhiteSpace(SelectedRole.Role_Name))
            {
                FormErrorMessage = "Role name is required.";
                return;
            }

            bool exists = _roleService.GetAll()
                .Any(r => r.Role_ID != SelectedRole.Role_ID &&
                          string.Equals(r.Role_Name, SelectedRole.Role_Name, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                FormErrorMessage = "Role name already exists.";
                return;
            }

            if (_roleService.Update(SelectedRole))
            {
                LoadRoles();
            }
            else
            {
                FormErrorMessage = "Failed to update role.";
            }
        }

        [RelayCommand(CanExecute = nameof(CanModifyRole))]
        private void DeleteRole()
        {
            FormErrorMessage = string.Empty;

            if (SelectedRole == null) return;

            if (_roleService.Delete(SelectedRole))
            {
                LoadRoles();
                SelectedRole = null; // triggers CanExecute updates
            }
            else
            {
                FormErrorMessage = "Failed to delete role.";
            }
        }

        private bool CanModifyRole() => SelectedRole != null;
    }
}
