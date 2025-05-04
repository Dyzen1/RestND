using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Employees;
using System.Collections.ObjectModel;

public partial class RoleViewModel: ObservableObject{
    #region Services
    // Service for handling Role database operations
    private readonly RoleServices _roleService;
    #endregion

    #region Observable Properties
    // List of roles displayed in the UI
    [ObservableProperty]
    private ObservableCollection<Role> roles = new();
    // The role currently selected in the UI
    [ObservableProperty]
    private Role selectedRole;
    // Called automatically when SelectedRole changes
    partial void OnSelectedRoleChanged(Role value)
    {
        DeleteRoleCommand.NotifyCanExecuteChanged();
        UpdateRoleCommand.NotifyCanExecuteChanged();
    }
    #endregion


    #region Constructor
    // Constructor: initializes the RoleService and loads roles
    public RoleViewModel()
    {
        _roleService = new RoleServices();
        LoadRoles();
    }
    #endregion


    #region Load Method
    // Loads all roles from the database
    [RelayCommand]
    private void LoadRoles()
    {
        Roles.Clear();
        var dbRoles = _roleService.GetAll();
        foreach (var role in dbRoles)
            Roles.Add(role);
    }
    #endregion

    #region Delete
    // Command for deleting the selected role
    [RelayCommand(CanExecute = nameof(CanModifyRole))]
    private void DeleteRole()
    {
        if (SelectedRole != null)
        {
            bool success = _roleService.Delete(SelectedRole.Role_ID);

            if (success)
            {
                Roles.Remove(SelectedRole);
            }
        }
    }
    #endregion

    #region Update
    // Command for updating the selected role
    [RelayCommand(CanExecute = nameof(CanModifyRole))]
    private void UpdateRole()
    {
        if (SelectedRole != null)
        {
            bool success = _roleService.Update(SelectedRole);

            if (success)
            {
                LoadRoles();
            }
        }
    }
    #endregion

    #region Add
    // Command for adding a new role
    [RelayCommand]
    private void AddRole()
    {
        if (SelectedRole != null)
        {
            bool success = _roleService.Add(SelectedRole);

            if (success)
            {
                LoadRoles();
                SelectedRole = new Role();
            }
        }
    }
    #endregion

    #region CanModifyRole
    // Helper method: checks if a role is selected (used for button enabling)
    private bool CanModifyRole()
    {
        return SelectedRole != null;
    }
    #endregion

   
    
}