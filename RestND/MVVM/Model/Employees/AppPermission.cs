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
        OverView = 2,
        Dishes = 4,
        Reports = 8,
        Orders = 16,
        Employees = 32,
        Tables = 64,

        All = Inventory | OverView | Dishes | Reports | Orders | Employees | Tables
    }
}
