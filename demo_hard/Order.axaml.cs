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
        TotalCost.Text = _order.Services.Sum(s => s.CostPerHour ?? 0).ToString("C");
        StatusText.Text = _order.Status ?? "active";
    }

    private void PrintBarcode_Click(object sender, RoutedEventArgs e)
    {
        if (_order.RentTime == null)
        {
            _order.RentTime = 1; 
        }
        
        new BarcodeWindow(_order.OrderId, _order.RentTime.Value).Show();
        Close();
    }
    
    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}