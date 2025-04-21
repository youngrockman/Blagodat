using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using demo_hard.Models;
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
            Models.Order newOrder = new Models.Order()
            {
                ClientId = SelectedClient.ClientId,
                Time = TimeOnly.FromDateTime(DateTime.Now),
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                Status = "Новая",
                RentTime = SelectedServices.Max(service => service.RentTime),
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
            new BarcodeWindow(newOrder.OrderId, newOrder.RentTime ?? 30).Show();
            Close();
        }
        catch (Exception exception)
        {
            StatusText.Text = $"Ошибка создания заказа: {exception.Message}";
        }
    }

    private void GenerateOrderPdfDocument(Models.Order order)
    {
        string documentsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string pdfFilePath = Path.Combine(documentsDirectoryPath, $"Заказ_{order.OrderCode.Replace("/", "_")}.pdf");

        using (Document document = new Document(PageSize.A4, 40, 40, 40, 40))
        {
            PdfWriter pdfWriter = PdfWriter.GetInstance(document, new FileStream(pdfFilePath, FileMode.Create));
            document.Open();

            Font titleFont = FontFactory.GetFont("Arial", 16, Font.BOLD);
            Font headerFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
            Font normalFont = FontFactory.GetFont("Arial", 12);

            Paragraph titleParagraph = new Paragraph("Квитанция об оплате", titleFont);
            titleParagraph.Alignment = Element.ALIGN_CENTER;
            titleParagraph.SpacingAfter = 20;
            document.Add(titleParagraph);

            PdfPTable orderInfoTable = new PdfPTable(2);
            orderInfoTable.WidthPercentage = 100;
            orderInfoTable.SetWidths(new float[] { 30, 70 });

            AddPdfTableRow(orderInfoTable, "Номер заказа:", order.OrderCode, headerFont, normalFont);
            AddPdfTableRow(orderInfoTable, "Дата и время:", $"{order.StartDate:dd.MM.yyyy} {order.Time:hh\\:mm}", headerFont, normalFont);
            AddPdfTableRow(orderInfoTable, "Клиент:", SelectedClient?.Fio ?? "Не указан", headerFont, normalFont);
            AddPdfTableRow(orderInfoTable, "Статус:", order.Status, headerFont, normalFont);

            document.Add(orderInfoTable);
            document.Add(new Paragraph(" "));

            Paragraph servicesTitleParagraph = new Paragraph("Оказанные услуги:", headerFont);
            servicesTitleParagraph.SpacingAfter = 10;
            document.Add(servicesTitleParagraph);

            PdfPTable servicesTable = new PdfPTable(3);
            servicesTable.WidthPercentage = 100;
            servicesTable.SetWidths(new float[] { 60, 20, 20 });

            servicesTable.AddCell(new Phrase("Наименование услуги", headerFont));
            servicesTable.AddCell(new Phrase("Стоимость/час", headerFont));
            servicesTable.AddCell(new Phrase("Время (мин)", headerFont));

            foreach (ServiceWithRentTime service in SelectedServices)
            {
                servicesTable.AddCell(new Phrase(service.ServiceName, normalFont));
                servicesTable.AddCell(new Phrase($"{service.CostPerHour:N2} ₽", normalFont));
                servicesTable.AddCell(new Phrase(service.RentTime.ToString(), normalFont));
            }

            document.Add(servicesTable);
            document.Add(new Paragraph(" "));

            decimal totalAmount = SelectedServices.Sum(service => (service.CostPerHour ?? 0) * (service.RentTime / 60m));
            Paragraph totalParagraph = new Paragraph($"Итого к оплате: {totalAmount:N2} ₽", headerFont);
            totalParagraph.Alignment = Element.ALIGN_RIGHT;
            document.Add(totalParagraph);
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = pdfFilePath,
                UseShellExecute = true
            });
        }
        catch (Exception exception)
        {
            StatusText.Text = $"Ошибка при открытии PDF: {exception.Message}";
        }
    }

    private void AddPdfTableRow(PdfPTable table, string label, string value, Font labelFont, Font valueFont)
    {
        table.AddCell(new Phrase(label, labelFont));
        table.AddCell(new Phrase(value, valueFont));
    }
}