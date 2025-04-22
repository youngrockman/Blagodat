using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using demo_hard.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ReactiveUI;
using System.ComponentModel;

namespace demo_hard;

public partial class SallerWindow : Window, INotifyPropertyChanged, IReactiveObject, INotifyPropertyChanging
{
    private readonly User15Context _databaseContext = new User15Context();
    public ObservableCollection<Client> Clients { get; } = new ObservableCollection<Client>();
    public ObservableCollection<Service> Services { get; } = new ObservableCollection<Service>();
    public ObservableCollection<ServiceWithRentTime> SelectedServices { get; } = new ObservableCollection<ServiceWithRentTime>();

    public class ServiceWithRentTime : Service
    {
        public int RentTime { get; set; } = 30;
        public string Status { get; set; } = "Новая";
    }

    private Client _selectedClient;
    public Client SelectedClient
    {
        get { return _selectedClient; }
        set { this.RaiseAndSetIfChanged(ref _selectedClient, value); }
    }

    private Service _selectedService;
    public Service SelectedService
    {
        get { return _selectedService; }
        set { this.RaiseAndSetIfChanged(ref _selectedService, value); }
    }

    private int _selectedRentTime = 30;
    public int SelectedRentTime
    {
        get { return _selectedRentTime; }
        set { this.RaiseAndSetIfChanged(ref _selectedRentTime, value); }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public event PropertyChangingEventHandler PropertyChanging;

    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs eventArgs)
    {
        PropertyChanged?.Invoke(this, eventArgs);
    }

    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs eventArgs)
    {
        PropertyChanging?.Invoke(this, eventArgs);
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnPropertyChanging(string propertyName)
    {
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
    }

    public SallerWindow()
    {
        InitializeComponent();
        DataContext = this;
        LoadData();
        OrderNumberBox.Text = "Будет сгенерирован автоматически";
    }

    private async void LoadData()
    {
        try
        {
            await _databaseContext.Clients.LoadAsync();
            await _databaseContext.Services.LoadAsync();
            
            Clients.Clear();
            Services.Clear();
            
            foreach (Client client in _databaseContext.Clients.Local.ToList())
            {
                Clients.Add(client);
            }
                
            foreach (Service service in _databaseContext.Services.Local.ToList())
            {
                Services.Add(service);
            }
        }
        catch (Exception exception)
        {
            StatusText.Text = $"Ошибка загрузки данных: {exception.Message}";
        }
    }

    private async void AddClientButtonClick(object sender, RoutedEventArgs routedEventArgs)
    {
        AddClient dialogWindow = new AddClient();
        Client newClient = await dialogWindow.ShowDialog<Client>(this);
        
        if (newClient != null)
        {
            try
            {
                _databaseContext.Clients.Add(newClient);
                await _databaseContext.SaveChangesAsync();
                Clients.Add(newClient);
                SelectedClient = newClient;
            }
            catch (Exception exception)
            {
                StatusText.Text = $"Ошибка добавления клиента: {exception.Message}";
            }
        }
    }

    private void AddServiceButtonClick(object sender, RoutedEventArgs routedEventArgs)
    {
        if (SelectedService != null)
        {
            if (!SelectedServices.Any(service => service.ServiceId == SelectedService.ServiceId))
            {
                SelectedServices.Add(new ServiceWithRentTime
                {
                    ServiceId = SelectedService.ServiceId,
                    ServiceName = SelectedService.ServiceName,
                    CostPerHour = SelectedService.CostPerHour,
                    RentTime = SelectedRentTime,
                    Status = "Новая"
                });
            }
            else
            {
                StatusText.Text = "Эта услуга уже добавлена";
            }
        }
        else
        {
            StatusText.Text = "Выберите услугу из списка";
        }
    }

    private void RemoveServiceButtonClick(object sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is Button button && button.CommandParameter is ServiceWithRentTime service)
        {
            SelectedServices.Remove(service);
        }
    }

    private async void CreateOrderButtonClick(object sender, RoutedEventArgs routedEventArgs)
    {
        if (SelectedClient == null)
        {
            StatusText.Text = "Выберите клиента";
            return;
        }

        if (SelectedServices.Count == 0)
        {
            StatusText.Text = "Добавьте хотя бы одну услугу";
            return;
        }

        try
        {
            string orderNumber = $"{new Random().Next(10000000, 99999999)}/{DateTime.Now:dd.MM.yyyy}";
            Model.Order newOrder = new Model.Order()
            {
                ClientId = SelectedClient.ClientId,
                Time = TimeOnly.FromDateTime(DateTime.Now),
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                Status = "Новая",
                OrderCode = orderNumber
            };

            _databaseContext.Orders.Add(newOrder);
            await _databaseContext.SaveChangesAsync();

            foreach (ServiceWithRentTime service in SelectedServices)
            {
                _databaseContext.OrderServices.Add(new OrderService
                {
                    OrderId = newOrder.OrderId,
                    ServiceId = service.ServiceId,
                    RentTime = service.RentTime
                });
            }

            await _databaseContext.SaveChangesAsync();
            GenerateOrderPdfDocument(newOrder);
            
            // Получаем максимальное время аренды из выбранных услуг
            int maxRentTime = SelectedServices.Max(s => s.RentTime);
            new BarcodeWindow(newOrder.OrderId, maxRentTime).Show();
            
            Close();
        }
        catch (Exception exception)
        {
            StatusText.Text = $"Ошибка создания заказа: {exception.Message}";
        }
    }

    private void GenerateOrderPdfDocument(Model.Order order)
{
    
    string documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);  
    string pdfFilePath = Path.Combine(documentsDirectory, $"Order_{order.OrderCode.Replace("/", "_")}.pdf");

    using (Document document = new Document(PageSize.A4, 40, 40, 40, 40))
    {
        PdfWriter.GetInstance(document, new FileStream(pdfFilePath, FileMode.Create));
        document.Open();

      
        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.EMBEDDED);
        Font titleFont  = new Font(bf, 16, Font.BOLD);
        Font headerFont = new Font(bf, 12, Font.BOLD);
        Font normalFont = new Font(bf, 12);

        
        var title = new Paragraph("Payment Receipt", titleFont)
        {
            Alignment    = Element.ALIGN_CENTER,
            SpacingAfter = 20f
        };
        document.Add(title);

       
        PdfPTable infoTable = new PdfPTable(2) { WidthPercentage = 100 };
        infoTable.SetWidths(new float[] { 30f, 70f });
        AddCell(infoTable, "Order Number:", order.OrderCode, headerFont, normalFont);
        AddCell(infoTable, "Date & Time:", $"{order.StartDate:dd.MM.yyyy} {order.Time:hh\\:mm}", headerFont, normalFont);
        AddCell(infoTable, "Customer:", SelectedClient?.Fio ?? "N/A", headerFont, normalFont);
        AddCell(infoTable, "Status:", order.Status, headerFont, normalFont);
        document.Add(infoTable);

        document.Add(new Paragraph(" "));

        
        var svcTitle = new Paragraph("Services Rendered:", headerFont)
        {
            SpacingAfter = 10f
        };
        document.Add(svcTitle);

        PdfPTable svcTable = new PdfPTable(3)
        {
            WidthPercentage = 100,
            SpacingBefore   = 5f,
            SpacingAfter    = 5f
        };
        svcTable.SetWidths(new float[] { 60f, 20f, 20f });

       
        foreach (var hdr in new[] { "Service Name", "Rate/hour", "Duration (min)" })
        {
            svcTable.AddCell(new PdfPCell(new Phrase(hdr, headerFont))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                BackgroundColor     = BaseColor.LIGHT_GRAY,
                Padding             = 5f
            });
        }

      
        foreach (var svc in SelectedServices)
        {
            svcTable.AddCell(new PdfPCell(new Phrase(svc.ServiceName, normalFont)) { Padding = 5f });
            svcTable.AddCell(new PdfPCell(new Phrase($"{svc.CostPerHour:N2} ₽", normalFont))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Padding             = 5f
            });
            svcTable.AddCell(new PdfPCell(new Phrase(svc.RentTime.ToString(), normalFont))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Padding             = 5f
            });
        }

        document.Add(svcTable);

      
        decimal total = SelectedServices.Sum(s => (s.CostPerHour ?? 0) * (s.RentTime / 60m));
        var totalPara = new Paragraph($"Total Due: {total:N2} ₽", headerFont)
        {
            Alignment       = Element.ALIGN_RIGHT,
            SpacingBefore   = 10f
        };
        document.Add(totalPara);
    }

   
    try
    {
        Process.Start(new ProcessStartInfo
        {
            FileName        = pdfFilePath,
            UseShellExecute = true  
        });
    }
    catch (Exception ex)
    {
        StatusText.Text = $"Error opening PDF: {ex.Message}";
    }
}


private void AddCell(PdfPTable table, string label, string value, Font labelFont, Font valueFont)
{
    table.AddCell(new Phrase(label, labelFont));
    table.AddCell(new Phrase(value, valueFont));
}

}