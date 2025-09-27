using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using RestND.MVVM.Model; 
using RestND.MVVM.Model.Orders;
using RestND.MVVM.Model.VAT;

namespace RestND.Data
{
    public class BillPrinterOptions
    {
        // ── Layout ────────────────────────────────────────────────────────────────
        public int MaxCharsPerLine { get; set; } = 42;     // 58mm ≈ 32–48 chars depending on font
        public int LeftPaddingPx { get; set; } = 10;
        public int TopPaddingPx { get; set; } = 10;
        public int LineHeightPx { get; set; } = 18;

        // ── Font ─────────────────────────────────────────────────────────────────
        public string FontFamily { get; set; } = "Consolas";
        public float FontSize { get; set; } = 9f;
        public float HeaderFontSize { get; set; } = 11f;

        // ── Printer selection ────────────────────────────────────────────────────
        public string? PrinterName { get; set; } = null;

        // ── Header/Footer ────────────────────────────────────────────────────────
        public string RestaurantName { get; set; } = "RestND Restaurant";
        public string? AddressLine { get; set; } = "Handesaim Technion";
        public string? PhoneLine { get; set; } = "04-1234567";
        public string FooterLine { get; set; } = "תודה ולהתראות! / Thank you!";

        // ── Money ────────────────────────────────────────────────────────────────
        public CultureInfo CurrencyCulture { get; set; } = new("he-IL");
        public bool ShowVatLine { get; set; } = true;
        public double VatPercent { get; set; } = 17.0;
        public bool ShowDiscountLine { get; set; } = true;

        // ── Behavior ─────────────────────────────────────────────────────────────
        public bool CutPaperAtEnd { get; set; } = false;
        public bool PrintPreview { get; set; } = false;
    }

    public class BillPrinterService
    {
        private readonly Order _order;
        private readonly Bill _bill;
        private readonly BillPrinterOptions _opt;

        // cache for render
        private Font? _font, _fontHeader, _fontBold;
        private float _y;

        public BillPrinterService(Order order, Bill bill, BillPrinterOptions? options = null)
        {
            _order = order ?? throw new ArgumentNullException(nameof(order));
            _bill = bill ?? throw new ArgumentNullException(nameof(bill));
            _opt = options ?? new BillPrinterOptions();
        }

        public void Print()
        {
            using var pd = new PrintDocument();

            if (!string.IsNullOrWhiteSpace(_opt.PrinterName))
                pd.PrinterSettings.PrinterName = _opt.PrinterName;

            // If you want a tighter paper size (thermal):
            // Many thermal drivers expose "Roll 58mm" paper sizes; otherwise we can set a small custom size.
            pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            pd.PrintPage += OnPrintPage;

            pd.Print();
        }

        private void OnPrintPage(object? sender, PrintPageEventArgs e)
        {
            _font = new Font(_opt.FontFamily, _opt.FontSize, GraphicsUnit.Point);
            _fontBold = new Font(_opt.FontFamily, _opt.FontSize, FontStyle.Bold, GraphicsUnit.Point);
            _fontHeader = new Font(_opt.FontFamily, _opt.HeaderFontSize, FontStyle.Bold, GraphicsUnit.Point);

            _y = _opt.TopPaddingPx;

            // HEADER
            DrawCentered(e, _opt.RestaurantName, _fontHeader);
            if (!string.IsNullOrWhiteSpace(_opt.AddressLine))
                DrawCentered(e, _opt.AddressLine!, _font);
            if (!string.IsNullOrWhiteSpace(_opt.PhoneLine))
                DrawCentered(e, _opt.PhoneLine!, _font);
            DrawRule(e);

            // META (date/table/employee)
            var now = DateTime.Now;
            var empName = _order?.assignedEmployee?.Employee_Name ?? "-";
            var tableNo = _order?.Table?.Table_Number.ToString() ?? "-";
            DrawLine(e, $"Date: {now:yyyy-MM-dd}  Time: {now:HH:mm}");
            DrawLine(e, $"Table: {tableNo}   Waiter: {empName}");
            DrawRule(e);

            // ITEMS
            // Columns: QTY (right 3) | NAME (left variable) | TOTAL (right 9)
            DrawLine(e, PadBoth("Items", '-'));
            var items = _order?.DishInOrder ?? new System.Collections.Generic.List<DishInOrder>();
            foreach (var it in items)
            {
                var qty = Math.Max(1, it?.Quantity ?? 0);
                var name = it?.Dish?.Dish_Name ?? "(item)";
                var price = it?.Dish?.Dish_Price ?? 0;
                var lineTotal = price * qty;

                DrawItemRow(e, qty, name, lineTotal);
            }
            DrawRule(e);

            // SUBTOTAL / DISCOUNT / VAT / TOTAL
            var subTotal = items.Sum(i => (i?.Dish?.Dish_Price ?? 0) * Math.Max(1, i?.Quantity ?? 0));
            double discountAmount = 0;
            string? discountName = _bill?.Discount?.Discount_Name;
            if (_opt.ShowDiscountLine && _bill?.Discount != null && _bill.Discount.Discount_Percentage > 0)
            {
                discountAmount = subTotal * (_bill.Discount.Discount_Percentage / 100.0);
            }
            var afterDiscount = subTotal - discountAmount;

            double vatAmount = 0;
            if (_opt.ShowVatLine && _opt.VatPercent > 0)
            {
                vatAmount = afterDiscount * (_opt.VatPercent / 100.0);
            }

            var grand = afterDiscount + vatAmount;

            MoneyLine(e, "Subtotal", subTotal);
            if (discountAmount > 0)
                MoneyLine(e, $"Discount ({discountName ?? $"{_bill?.Discount?.Discount_Percentage:0.#}%"} )", -discountAmount);
            if (_opt.ShowVatLine)
                MoneyLine(e, $"VAT {_opt.VatPercent:0.#}%", vatAmount);

            DrawRule(e);
            MoneyLine(e, "TOTAL", grand, bold: true);
            DrawRule(e);

            // FOOTER
            DrawCentered(e, _opt.FooterLine, _font);

            // ESC/POS cut is not handled here; many Windows drivers do auto-cut after job.
            e.HasMorePages = false;

            // cleanup fonts created on the fly
            _font?.Dispose();
            _fontBold?.Dispose();
            _fontHeader?.Dispose();
        }

        // ───────────────────────────── Drawing helpers ─────────────────────────────

        private void DrawLine(PrintPageEventArgs e, string text, Font? f = null)
        {
            var font = f ?? _font!;
            var (wrapped, lines) = Wrap(text, _opt.MaxCharsPerLine);
            foreach (var ln in lines)
            {
                e.Graphics.DrawString(ln, font, Brushes.Black, _opt.LeftPaddingPx, _y);
                _y += _opt.LineHeightPx;
            }
        }

        private void DrawCentered(PrintPageEventArgs e, string text, Font f)
        {
            // Centering by characters (monospace assumption)
            var (wrapped, lines) = Wrap(text, _opt.MaxCharsPerLine);
            foreach (var ln in lines)
            {
                var pad = Math.Max(0, (_opt.MaxCharsPerLine - ln.Length) / 2);
                var centered = new string(' ', pad) + ln;
                e.Graphics.DrawString(centered, f, Brushes.Black, _opt.LeftPaddingPx, _y);
                _y += _opt.LineHeightPx;
            }
        }

        private void DrawRule(PrintPageEventArgs e, char ch = '-')
        {
            DrawLine(e, new string(ch, _opt.MaxCharsPerLine));
        }

        private void MoneyLine(PrintPageEventArgs e, string label, double amount, bool bold = false)
        {
            var textAmount = amount.ToString("C", _opt.CurrencyCulture);
            var left = label.Trim();
            var right = textAmount;

            // Keep at least 1 space between label and amount
            var line = PadLeftRight(left, right, _opt.MaxCharsPerLine);
            DrawLine(e, line, bold ? _fontBold : _font);
        }

        private void DrawItemRow(PrintPageEventArgs e, int qty, string name, double lineTotal)
        {
            // Layout targets:
            // [QTY: right 3] + space + [NAME: wraps] + [TOTAL: right 9]
            var qtyStr = qty.ToString().PadLeft(3);
            var totalStr = lineTotal.ToString("C", _opt.CurrencyCulture);

            // We construct the first line with as much name as can fit;
            // subsequent wrapped lines are name-only (indented to align visually).
            var fixedLeft = qtyStr + " ";
            var fixedRight = totalStr;
            var availableForName = _opt.MaxCharsPerLine - fixedLeft.Length - fixedRight.Length - 1; // -1 space between name and amount
            if (availableForName < 6) availableForName = 6; // minimum room

            var (wrapped, lines) = Wrap(name ?? string.Empty, availableForName);

            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                {
                    var first = fixedLeft + lines[i];
                    var line = PadLeftRight(first, fixedRight, _opt.MaxCharsPerLine);
                    DrawLine(e, line);
                }
                else
                {
                    // continuation lines (name only, with indent equal to qty column)
                    var cont = new string(' ', fixedLeft.Length) + lines[i];
                    DrawLine(e, cont);
                }
            }
        }

        private static string PadLeftRight(string left, string right, int width)
        {
            left = left ?? string.Empty;
            right = right ?? string.Empty;
            var room = Math.Max(0, width - left.Length - right.Length);
            return left + new string(' ', room) + right;
        }

        private static string PadBoth(string title, char filler, int minGaps = 2, int width = 42)
        {
            title = $" {title} ";
            var room = Math.Max(0, width - title.Length);
            var left = room / 2;
            var right = room - left;
            return new string(filler, left) + title + new string(filler, right);
        }

        private static (string Wrapped, string[] Lines) Wrap(string text, int max)
        {
            if (string.IsNullOrEmpty(text)) return (string.Empty, new[] { string.Empty });
            var words = text.Replace("\r", "").Split('\n')
                            .SelectMany(p => p.Split(' '))
                            .ToArray();

            var sb = new StringBuilder();
            var current = new StringBuilder();
            foreach (var w in words)
            {
                if (current.Length == 0)
                {
                    current.Append(w);
                }
                else if (current.Length + 1 + w.Length <= max)
                {
                    current.Append(' ').Append(w);
                }
                else
                {
                    sb.AppendLine(current.ToString());
                    current.Clear();
                    current.Append(w);
                }
            }
            if (current.Length > 0) sb.AppendLine(current.ToString());
            var result = sb.ToString().TrimEnd('\n', '\r');
            var lines = result.Length == 0 ? new[] { "" } : result.Split('\n');
            return (result, lines);
        }
    }
}
