using RestND.MVVM.Model.Orders;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;

public class TicketPrinter
{
    private readonly Order _order;

    private float yPos = 0;
    private readonly float leftMargin = 5;
    private readonly float pageWidth = 280; // ~72mm printable width
    private readonly Font bigfont = new Font("Consolas", 25f);
    private readonly Font font = new Font("Consolas", 10f);
    private readonly Font bold = new Font("Consolas", 10f, FontStyle.Bold);
    private readonly Font bigBold = new Font("Consolas", 12f, FontStyle.Bold);

    private readonly CultureInfo il = new CultureInfo("he-IL");

    public string RestaurantName { get; set; } = "RestND Restaurant";

    // ticket behavior
    public bool ExcludeSoftDrinks { get; set; } = true;
    public string TicketTitle { get; set; } = "KITCHEN ";

    public TicketPrinter(Order order)
    {
        _order = order;
    }

    public void Print()
    {
        PrintDocument pd = new PrintDocument();
        pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", (int)pageWidth, 5000);
        pd.PrintPage += PrintPage;
        pd.Print();
    }

    private void PrintPage(object sender, PrintPageEventArgs e)
    {
        Graphics g = e.Graphics;
        yPos = 10;

        // Header
        DrawCentered(g, RestaurantName, bold, extra: 6);
        yPos += 5;

        // Meta
        string table = _order.Table != null ? _order.Table.Table_Number.ToString() : "-";
        string waiter = _order.assignedEmployee?.Employee_Name ?? "-";

        // NEW: Order ID + People count
        string orderId = GetIntPropAsString(_order, "Order_ID", "OrderId", "ID", "Id");
        string people = GetIntPropAsString(_order, "PeopleCount", "People_Count", "Guests", "GuestsCount", "NumPeople", "NumberOfPeople");

        DrawText(g, $"Date: {DateTime.Now:dd/MM/yyyy HH:mm}");
        DrawText(g, $"Order: {orderId}    People: {people}");
        DrawText(g, $"Table: {table}    Waiter: {waiter}");
        DrawLine(g);

        // Title
        DrawCentered(g, TicketTitle, bigBold, extra: 6);
        DrawLine(g);

        // Items
        var items = _order.DishInOrder
            .Where(x => x != null)
            .Where(x => x.Quantity > 0)
            .Where(x => x.dish != null && !string.IsNullOrWhiteSpace(x.dish.Dish_Name));

        if (ExcludeSoftDrinks)
        {
            items = items.Where(x =>
                !string.Equals(x.dish?.Dish_Type?.DishType_Name, "SoftDrinks", StringComparison.OrdinalIgnoreCase));
        }

        var grouped = items
            .GroupBy(x => x.dish?.Dish_Type?.DishType_Name ?? "Other")
            .OrderBy(gp => gp.Key);

        bool printedAny = false;

        foreach (var group in grouped)
        {
            printedAny = true;

            // Section header (Dish Type)
            DrawText(g, group.Key.ToUpperInvariant(), bold);

            foreach (var item in group)
            {
                string name = item.dish?.Dish_Name ?? "(Unknown)";
                DrawText(g, $"{item.Quantity}  x  {name}", bold);
                yPos += 3;
            }

            yPos += 4;
            DrawLine(g);
        }

        if (!printedAny)
        {
            DrawCentered(g, "No items to print.", bold, extra: 8);
            DrawLine(g);
        }

        yPos += 8;
        DrawCentered(g, $" {DateTime.Now: HH:mm}", bigfont);
    }

    // ✅ Helper: tries multiple property names safely (no compile errors)
    private string GetIntPropAsString(object obj, params string[] possibleNames)
    {
        if (obj == null) return "-";

        var t = obj.GetType();

        foreach (var name in possibleNames)
        {
            var prop = t.GetProperty(name);
            if (prop == null) continue;

            try
            {
                var value = prop.GetValue(obj);
                if (value == null) continue;

                // handle int/short/long/etc.
                if (value is IConvertible)
                    return Convert.ToInt32(value).ToString();
            }
            catch
            {
                // ignore and try next name
            }
        }

        return "-";
    }

    private void DrawText(Graphics g, string text, Font? f = null, bool alignRight = false)
    {
        f ??= font;
        float width = pageWidth - leftMargin * 2;

        var format = new StringFormat
        {
            Alignment = alignRight ? StringAlignment.Far : StringAlignment.Near
        };

        g.DrawString(
            text,
            f,
            Brushes.Black,
            new RectangleF(leftMargin, yPos, width, f.Height + 2),
            format);

        yPos += f.GetHeight(g) + 2;
    }

    private void DrawCentered(Graphics g, string text, Font? f = null, float extra = 0)
    {
        f ??= font;
        float width = pageWidth - leftMargin * 2;

        var format = new StringFormat { Alignment = StringAlignment.Center };

        g.DrawString(
            text,
            f,
            Brushes.Black,
            new RectangleF(leftMargin, yPos, width, f.Height + 2),
            format);

        yPos += f.GetHeight(g) + extra;
    }

    private void DrawLine(Graphics g)
    {
        g.DrawString(new string('-', 42), font, Brushes.Black, leftMargin, yPos);
        yPos += font.GetHeight(g);
    }
}
