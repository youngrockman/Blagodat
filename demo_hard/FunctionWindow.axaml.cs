using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using demo_hard.Model;


namespace demo_hard;

public partial class FunctionWindow : Window
{
    private readonly TimeSpan sessionDuration = TimeSpan.FromMinutes(10);
    private readonly TimeSpan warningTime = TimeSpan.FromMinutes(5);
    //private readonly TimeSpan lockoutDuration = TimeSpan.FromMinutes(1);
    
    private DateTime sessionStartTime;
    private bool warningShow = false;
    public FunctionWindow(Employee user)
    {
        InitializeComponent();
        
        DataContext = new ImageEmployee()
        {
            Id = user.Id,
            Fio = user.Fio,
            Login = user.Login,
            Password = user.Password,
            Role = user.Role,
            Photo = user.Photo
        };
        
        
        sessionStartTime = DateTime.Now;
        StartSessionTimer();
        
    }
    
    public FunctionWindow()
    {
        InitializeComponent();
    }

    private async void StartSessionTimer()
    {
        while (true)
        {
            TimeSpan elapsedTime = DateTime.Now - sessionStartTime;
            TimeSpan remainingTime = sessionDuration - elapsedTime;
            
            this.FindControl<TextBlock>("SessionTimer").Text = $"Осталось: {remainingTime.Minutes}:{remainingTime.Seconds}";

            if (!warningShow && remainingTime <= warningTime)
            {
                warningShow = true;
                WarningBlock.Text = "Внимание! Ваш сеанс закончится через 5 минут!";
            }

            if (remainingTime <= TimeSpan.Zero)
            {
                EndSession();
                break;
            }

            await Task.Delay(1000);
        }
    }

    private async void EndSession()
    {
        this.Close();
    }
        
    private void Back_Button(object? sender, RoutedEventArgs e)
    {
        new MainWindow().ShowDialog(this);
    }

    private void Next_Function_Button(object? sender, RoutedEventArgs e)
    {
        new SallerWindow().ShowDialog(this);    
    }
    
    
    
    
    public class ImageEmployee: Employee
    {
        public bool IsRole2 => Role == 2;
        Bitmap? Image
        {
            get
            {
                try
                {
                    string absolutePath = Path.Combine(AppContext.BaseDirectory, Photo);
                    return new Bitmap(absolutePath);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }
    }

    private void History_Button(object? sender, RoutedEventArgs e)
    {
        new HistoryWindow().ShowDialog(this);
    }
}
