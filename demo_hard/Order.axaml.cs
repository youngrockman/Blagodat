using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using demo_hard.Models;
using Tmds.DBus.Protocol;

namespace demo_hard;

public partial class Order: Window
{

    public Order()
    {
        InitializeComponent();
    }
    public Order(Client selectedClient, List<Service> selectedServices, int duration)
    {
        InitializeComponent();

        OrderNumberText.Text = GenerateOrderNumber(selectedClient);
        ClientCodeText.Text = selectedClient.ClientCode.ToString();
        ClientFioText.Text = selectedClient.Fio;
        ClientAddressText.Text = selectedClient.Address;
        DateTimeBox.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        ServicesListBox.ItemsSource = selectedServices.Select(s => s.ServiceName).ToList();
        
        decimal totalPrice = selectedServices.Sum(s => decimal.TryParse(s.ServiceCost, out var cost) ? cost : 0);
        TotalCostText.Text = $"${totalPrice:0.00}";
    }

    private string GenerateOrderNumber(Client client)
    {
        using var context = new User2Context();
        int lastOrderId = context.Orders.Any() ? context.Orders.Max(o => o.Id) : 0;
        return $"{client.ClientCode}/{DateTime.Now:ddMMyyyy}/{lastOrderId + 1}";
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}