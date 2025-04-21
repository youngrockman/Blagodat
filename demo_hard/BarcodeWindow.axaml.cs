
using System;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Avalonia.Interactivity;
using Path = System.IO.Path;
using Rectangle = Avalonia.Controls.Shapes.Rectangle;
using System.Linq;
using System.Text;

namespace demo_hard;

public partial class BarcodeWindow : Window
{
    private readonly Random _random = new();
    public BarcodeWindow()
    {
        InitializeComponent();
    }

    public BarcodeWindow(int orderId, int rentTime) : this()
    {
        string barcodeData = GenerateBarcodeData(orderId, rentTime);
        BarcodeText.Text = barcodeData;
        GenerateBarcodeVisual(barcodeData);
        GeneratePdf(barcodeData, orderId);
        SaveBase64Link(orderId);
    }

    private string GenerateBarcodeData(int orderId, int rentTime)
    {
        string timestamp = DateTime.Now.ToString("yyMMddHHmm");
        string uniqueCode = string.Concat(Enumerable.Range(0, 6).Select(_ => _random.Next(10)));
        return $"{orderId}{timestamp}{rentTime}{uniqueCode}";
    }

    private void GenerateBarcodeVisual(string data)
    {
        const double mmToPx = 3.78;
        double x = 3.63 * mmToPx;
        double height = 22.85 * mmToPx;
        double extendedHeight = height + (1.65 * mmToPx);

        BarcodeCanvas.Children.Clear();

        for (int i = 0; i < data.Length; i++)
        {
            if (!char.IsDigit(data[i])) continue;

            var digit = int.Parse(data[i].ToString());
            double width = digit == 0 ? 1.35 * mmToPx : 0.15 * digit * mmToPx;

            if (digit > 0)
            {
                bool isBoundary = i == 0 || i == data.Length - 1 || i == data.Length / 2;

                var rect = new Rectangle
                {
                    Width = width,
                    Height = isBoundary ? extendedHeight : height,
                    Fill = Brushes.Black,
                };

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, isBoundary ? 0 : (extendedHeight - height));
                BarcodeCanvas.Children.Add(rect);
            }

            x += width + (0.2 * mmToPx);
        }
    }

    private void GeneratePdf(string data, int orderId)
    {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string pdfPath = Path.Combine(documentsPath, $"Barcode_{orderId}.pdf");

        using var doc = new Document(new iTextSharp.text.Rectangle(100f, 50f));
        PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(pdfPath, FileMode.Create));
        doc.Open();

        var barcode = new Barcode128
        {
            CodeType = Barcode128.CODE128,
            Code = data,
            BarHeight = 22.85f,
        };

        var image = barcode.CreateImageWithBarcode(writer.DirectContent, null, null);
        doc.Add(image);
    }

    private void SaveBase64Link(int orderId)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        string orderInfo = $"дата_заказа={timestamp}&номер_заказа={orderId}";
        string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(orderInfo));
        string link = $"https://wsrussia.ru/?data={base64}";

        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string txtPath = Path.Combine(documentsPath, $"OrderLink_{orderId}.txt");

        File.WriteAllText(txtPath, link);
    }

    private void PrintBarcode_Click(object sender, RoutedEventArgs e) => Close();
    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}