using RestND.MVVM.Model.Orders;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using RestND.MVVM.Model.VAT;

public class BillPrinter
{
    #region Properties
    private readonly Order _order;
    private readonly Bill? _bill;
    private float yPos = 0;
    private readonly float leftMargin = 5;
    private readonly float pageWidth = 280; // ~72 mm printable width
    private readonly Font font = new Font("Consolas", 10f);
    private readonly Font bold = new Font("Consolas", 10f, FontStyle.Bold);
    private readonly CultureInfo il = new CultureInfo("he-IL");
    public Vat VatPercent { get; set; } = new Vat(18);
    public string RestaurantName { get; set; } = "RestND Restaurant";
    public string Address { get; set; } = "3200003, Haifa";
    public string Phone { get; set; } = "077-877-5555";
    #endregion

    public BillPrinter(Order order, Bill? bill = null)
    {
        _order = order;
        _bill = bill;
        VatPercent = VatPercent; // default 18%
    }

    public void Print()
    {
        PrintDocument pd = new PrintDocument();
        pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", (int)pageWidth, 5000); // height auto
        pd.PrintPage += PrintPage;
        pd.Print();
    }

    private void PrintPage(object sender, PrintPageEventArgs e)
    {
        Graphics g = e.Graphics;
        yPos = 10;

        DrawCentered(g, RestaurantName, bold, 12);
        DrawCentered(g, Address, font);
        DrawCentered(g, $"Tel: {Phone}", font);
        yPos += 5;
        DrawLine(g);

        string table = _order.Table != null ? _order.Table.Table_Number.ToString() : "-";
        string waiter = _order.assignedEmployee?.Employee_Name ?? "-";

        DrawText(g, $"Date: {DateTime.Now:dd/MM/yyyy HH:mm}");
        DrawText(g, $"Table: {table}    Waiter: {waiter}");
        DrawLine(g);

        // Items
        foreach (var item in _order.DishInOrder)
        {
            string name = item.dish?.Dish_Name ?? "(Unknown)";
            double unit = (double)(item.dish?.Dish_Price ?? 0);
            double total = item.TotalDishPrice > 0 ? item.TotalDishPrice : unit * item.Quantity;

            DrawText(g, name, bold);
            DrawText(g, $"{item.Quantity} × {unit.ToString("N2", il)} ₪", alignRight: true);
            DrawText(g, $"{total.ToString("N2", il)} ₪", alignRight: true);
            yPos += 4;
        }

        DrawLine(g);

        double sub = _order.DishInOrder.Sum(x =>
            x.TotalDishPrice > 0 ? (double)x.TotalDishPrice :
            (double)(x.dish?.Dish_Price ?? 0) * x.Quantity);

        double vat = Math.Round(sub * (VatPercent.Percentage / 100.0), 2);
        double totalSum = Math.Round(sub + vat, 2);

        DrawText(g, $"Subtotal: {sub.ToString("N2", il)} ₪", alignRight: true);
        DrawText(g, $"VAT {VatPercent.Percentage:0.#}%: {vat.ToString("N2", il)} ₪", alignRight: true);
        DrawText(g, $"Total: {totalSum.ToString("N2", il)} ₪", bold, alignRight: true);

        yPos += 10;
        DrawCentered(g, "Thank you for dining with us!", font);
    }

    private void DrawText(Graphics g, string text, Font? f = null, bool alignRight = false)
    {
        f ??= font;
        float width = pageWidth - leftMargin * 2;
        var format = new StringFormat
        {
            Alignment = alignRight ? StringAlignment.Far : StringAlignment.Near
        };
        g.DrawString(text, f, Brushes.Black, new RectangleF(leftMargin, yPos, width, f.Height + 2), format);
        yPos += f.GetHeight(g) + 2;
    }

    private void DrawCentered(Graphics g, string text, Font? f = null, float extra = 0)
    {
        f ??= font;
        float width = pageWidth - leftMargin * 2;
        var format = new StringFormat { Alignment = StringAlignment.Center };
        g.DrawString(text, f, Brushes.Black, new RectangleF(leftMargin, yPos, width, f.Height + 2), format);
        yPos += f.GetHeight(g) + extra;
    }

    private void DrawLine(Graphics g)
    {
        g.DrawString(new string('-', 42), font, Brushes.Black, leftMargin, yPos);
        yPos += font.GetHeight(g);
    }
}
