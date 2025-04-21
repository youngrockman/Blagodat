using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using demo_hard.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Avalonia.Input;
using System.ComponentModel;

namespace demo_hard;

public partial class SallerWindow : Window, INotifyPropertyChanged
{
    private readonly User15Context _db = new();
    public ObservableCollection<Client> Clients { get; } = new();
    public ObservableCollection<Service> Services { get; } = new();
    public ObservableCollection<Service> SelectedServices { get; } = new();

    private Client? _selectedClient;
    public Client? SelectedClient
    {
        get => _selectedClient;
        set
        {
            _selectedClient = value;
            OnPropertyChanged(nameof(SelectedClient));
        }
    }

    private Service? _selectedService;
    public Service? SelectedService
    {
        get => _selectedService;
        set
        {
            _selectedService = value;
            OnPropertyChanged(nameof(SelectedService));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            await _db.Clients.LoadAsync();
            await _db.Services.LoadAsync();
            
            Clients.Clear();
            Services.Clear();
            
            foreach (var client in _db.Clients.Local.ToList())
                Clients.Add(client);
                
            foreach (var service in _db.Services.Local.ToList())
                Services.Add(service);
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Ошибка загрузки данных: {ex.Message}";
        }
    }

    private async void AddClient_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AddClient();
        var newClient = await dialog.ShowDialog<Client?>(this);
        
        if (newClient != null)
        {
            try
            {
                _db.Clients.Add(newClient);
                await _db.SaveChangesAsync();
                Clients.Add(newClient);
                SelectedClient = newClient;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Ошибка добавления клиента: {ex.Message}";
            }
        }
    }

    private void AddService_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedService != null)
        {
            if (!SelectedServices.Contains(SelectedService))
            {
                SelectedServices.Add(SelectedService);
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

    private void RemoveService_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Service service)
        {
            SelectedServices.Remove(service);
        }
    }

    private async void CreateOrder_Click(object sender, RoutedEventArgs e)
    {
        
        if (SelectedClient == null)
        {
            StatusText.Text = "Выберите клиента";
            return;
        }

      
        if (!SelectedServices.Any())
        {
            StatusText.Text = "Добавьте хотя бы одну услугу";
            return;
        }

        try
        {
            
            var order = new Models.Order()
            {
                ClientId = SelectedClient.ClientId,
                Time = TimeOnly.FromDateTime(DateTime.Now),
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                Status = "active",
                Services = SelectedServices.ToList(),
                RentTime = 1
            };

          
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            
            order.OrderCode = GenerateBarcode(order.OrderId, order.RentTime ?? 1);
            await _db.SaveChangesAsync(); 

            
            new BarcodeWindow(order.OrderId, order.RentTime ?? 1).Show();
            
           
            Close();
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Ошибка создания заказа: {ex.Message}";
        }
    }

    private string GenerateBarcode(int orderId, int rentTime)
    {
        var rnd = new Random();
        return $"{orderId}{DateTime.Now:ddMMyyHHmm}{rentTime}{rnd.Next(100000, 999999)}";
    }
}