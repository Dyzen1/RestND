using CommunityToolkit.Mvvm.Messaging.Messages;
using RestND.MVVM.Model.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.utilities
{
    public sealed class RoleChangedMessage : ValueChangedMessage<(Role role, string action)>
    {
        public RoleChangedMessage(Role role, string action) : base((role, action)) { }
    }

}
