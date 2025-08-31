using RestND.MVVM.Model.Employees;  // AppPermission
using RestND.MVVM.Model.Security;   // AuthContext
using RestND.MVVM.View;
using RestND.MVVM.View.Windows;  
using System;
using System.Windows;

namespace RestND.Data
{
    public static class NavigationSideBar
    {
        public static void Navigate(Window owner, string destination)
        {
            switch (destination)
            {
                case "Inventory":
                    if (!AuthContext.Has(AppPermission.Inventory))
                    {
                        MessageBox.Show("You don't have permission to open Inventory.");
                        return;
                    }
                    Open(owner, new ProductWindow());
                    break;

                case "OverView":
                    if (!AuthContext.Has(AppPermission.OverView))
                    {
                        MessageBox.Show("You don't have permission to open OverView.");
                        return;
                    }
                    Open(owner, new OverView());
                    break;

                case "Dishes":
                    if (!AuthContext.Has(AppPermission.Dishes))
                    {
                        MessageBox.Show("You don't have permission to open Dishes.");
                        return;
                    }
                    Open(owner, new DishWindow());
                    break;

                case "Reports":
                    if (!AuthContext.Has(AppPermission.Reports))
                    {
                        MessageBox.Show("You don't have permission to open Reports.");
                        return;
                    }
                    Open(owner, new ReportWindow());
                    break;

                case "Employees":
                    if (!AuthContext.Has(AppPermission.Employees))
                    {
                        MessageBox.Show("You don't have permission to open Employees.");
                        return;
                    }
                    Open(owner, new EmployeesWindow());
                    break;

                case "Orders":
                    Open(owner, new OrderWindow());
                    break;
            }
        }

        private static void Open(Window owner, Window target)
        {
            target.Owner = owner;
            target.WindowState = WindowState.Maximized;
            target.Show();
            owner?.Hide();
        }
    
    }
}
