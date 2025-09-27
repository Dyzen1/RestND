using System;
using System.Drawing;
using System.Drawing.Printing;
using RestND.MVVM.Model.Orders;

public class BillPrinter
{
    private readonly Order _order;
    private readonly Bill _bill;

    public BillPrinter(Order order, Bill bill)
    {
        _order = order;
        _bill = bill;
    }

    public void Print()
    {
        PrintDocument pd = new PrintDocument();
        pd.PrintPage += PrintPage;
        pd.Print();
    }

    private void PrintPage(object sender, PrintPageEventArgs e)
    {
        float y = 20;
        var font = new Font("Consolas", 10);

        e.Graphics.DrawString("RestND Restaurant", new Font("Arial", 12, FontStyle.Bold), Brushes.Black, 10, y);
        y += 30;

        e.Graphics.DrawString($"Table: {_order.Table.Table_Number}   Employee: {_order.assignedEmployee.Employee_Name}", font, Brushes.Black, 10, y);
        y += 20;

        e.Graphics.DrawString("----------------------------------------", font, Brushes.Black, 10, y);
        y += 20;

        foreach (var d in _order.DishInOrder)
        {
            string line = $"{d.Quantity} x {d.Dish.Dish_Name}   {d.Dish.Dish_Price * d.Quantity:C}";
            e.Graphics.DrawString(line, font, Brushes.Black, 10, y);
            y += 20;
        }

        e.Graphics.DrawString("----------------------------------------", font, Brushes.Black, 10, y);
        y += 20;

        e.Graphics.DrawString($"Total: {_bill.Price:C}", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, 10, y);
        y += 30;

        e.Graphics.DrawString("Thank you for dining with us!", font, Brushes.Black, 10, y);
    }
    //Usage:
    //var printer = new BillPrinter(order, bill);
    //printer.Print();

}
