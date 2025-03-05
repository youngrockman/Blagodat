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
    public Order(string orderNumber, Client client, List<Service> selectedServices)
    {
        InitializeComponent();
        
        OrderNumberText.Text=orderNumber;
        ClientCodeText.Text = client.Id.ToString();
        ClientFioText.Text = client.Fio;
        ClientAddressText.Text = client.Address;
        ServicesListBox.ItemsSource = selectedServices.Select(s => s.ServiceName).ToList();
        DateTimeBox.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        decimal totalPrice = selectedServices.Sum(s => 
        {
            decimal serviceCost = decimal.TryParse(s.ServiceCost, out var cost) ? cost : 0;
            return serviceCost;
        });

        TotalCostText.Text = $"${totalPrice:0.00}";
        
        
        
    }
    
    
    
    

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}