using Microsoft.AspNetCore.SignalR;
using RestND.MVVM.Model.Employees;

namespace RestNDSignalR.Hubs
{
    public class EmployeeHub : Hub
    {
        #region Employee Update
        public async Task NotifyEmployeeUpdate(Employee emp, string action)
        {
            await Clients.All.SendAsync("ReceiveEmployeeUpdate", emp, action);
        }

        #endregion

    }
}
