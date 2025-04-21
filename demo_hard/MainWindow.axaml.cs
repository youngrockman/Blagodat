using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using demo_hard.Models;
using Tmds.DBus.Protocol;

namespace demo_hard;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }



    private void TogglePasswordVisibility(object? sender, RoutedEventArgs e)
    {
        PasswordBox.PasswordChar = PasswordBox.PasswordChar == '*' ? '\0' : '*';
    }

    private void AuthorizeButton(object? sender, RoutedEventArgs e)
    {
        using var context = new User15Context();
        var user = context.Employees.FirstOrDefault(it => it.Login == LoginBox.Text && it.Password == PasswordBox.Text);

        if (user != null)
        {
            var functionWindow = new FunctionWindow(user);
            {
                DataContext = user;
            };
            functionWindow.ShowDialog(this);
        }
        else
        {
            ErrorBlock.Text = "Неверный пароль";
        }
    }
}