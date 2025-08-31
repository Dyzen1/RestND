using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model.Employees
{
    [Flags]
    public enum AppPermission
    {
        None = 0,
        Inventory = 1,
        Other = 2,
        Dishes = 4,
        Reports = 8,
        Orders = 16,
        Employees = 32,
        Tables = 64,

        All = Inventory | Other | Dishes | Reports | Orders | Employees | Tables
    }
}
