using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model.Employees;


namespace RestND.MVVM.ViewModel.Employees
{
    public partial class LoginViewModel : ObservableObject
    {
        #region Services
        private readonly EmployeeServices? _employeeService;
        private readonly LoginServices _loginServices;
        #endregion

        #region Constructor
        public LoginViewModel()
        {
            _loginServices = new LoginServices();
        }

        #endregion

        #region Observable Properties

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private Employee employee;

        #endregion

        #region On Change

        //partial void OnSelectedLoginChanged(Employee value)
        //{
            
        //}

        #endregion

        #region Relay Commands

        [RelayCommand]
        private void GetUserPassword()
        {
           var userDBpassword = _loginServices.GetPassword(id);
        }

        #endregion

    }
}
