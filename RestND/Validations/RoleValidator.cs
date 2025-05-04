using RestND.MVVM.Model;
using RestND.MVVM.Model.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RestND.Validations
{
    public class RoleValidator : GeneralValidations
    {
        Role role;
        public RoleValidator(Role role)
        {
            this.role = role;
        }

    }
}
