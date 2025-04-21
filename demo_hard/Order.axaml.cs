using Avalonia.Controls;
using Avalonia.Interactivity;
using demo_hard.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace demo_hard;

public partial class Order : Window
{
    private readonly Models.Order _order;
    private readonly User15Context _db = new();
    
    public Order(Models.Order order)
    {
        InitializeComponent();
        _order = order;
        LoadOrderData();
    }

    private void LoadOrderData()
    {
        var client = _db.Clients.FirstOrDefault(c => c.ClientId == _order.ClientId);
        
        OrderNumber.Text = _order.OrderId.ToString();
        ClientName.Text = client?.Fio ?? "Не указан";
        OrderDate.Text = $"{_order.StartDate} {_order.Time}";
        
        // Парсим информацию об услугах из OrderCode
        var servicesInfo = ParseServicesInfo(_order);
        TotalCost.Text = servicesInfo.totalCost.ToString("C");
        StatusText.Text = _order.Status ?? "Новая";
    }

    private (decimal totalCost, string servicesText) ParseServicesInfo(Models.Order order)
    {
        decimal total = 0;
        string info = "";
        
        if (!string.IsNullOrEmpty(order.OrderCode))
        {
            var parts = order.OrderCode.Split(';');
            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part)) continue;
                
                var serviceParts = part.Split(':');
                if (serviceParts.Length == 2 && 
                    int.TryParse(serviceParts[0], out var serviceId) &&
                    int.TryParse(serviceParts[1], out var rentTime))
                {
                    var service = _db.Services.Find(serviceId);
                    if (service != null)
                    {
                        var cost = (service.CostPerHour ?? 0) * rentTime;
                        total += cost;
                        info += $"{service.ServiceName} - {rentTime} ч ({cost:C})\n";
                    }
                }
            }
        }
        
        return (total, info);
    }

    private void PrintBarcode_Click(object sender, RoutedEventArgs e)
    {
        new BarcodeWindow(_order.OrderId, _order.RentTime ?? 1).Show();
        Close();
    }
    
    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}