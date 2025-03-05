using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using demo_hard.Models;

namespace demo_hard;

public partial class AddClient : Window
{
    public AddClient()
    {
        InitializeComponent();
    }

    private void AddClient_OnClick(object? sender, RoutedEventArgs e)
    {
        using var context = new User2Context();

        if (string.IsNullOrWhiteSpace(FioBox.Text) ||
            string.IsNullOrWhiteSpace(CodeBox.Text) ||
            string.IsNullOrWhiteSpace(BirthdayBox.Text) ||
            string.IsNullOrWhiteSpace(AddressBox.Text) ||
            string.IsNullOrWhiteSpace(EmailBox.Text) ||
            string.IsNullOrWhiteSpace(PassportBox.Text) ||
            string.IsNullOrWhiteSpace(PasswordBox.Text)
           )
        {
            UserNotAdd.Text = "Please fill all the fields!";
            return;
        }

        try
        {
            CorrectInput();
            var NewClient = new Client
            {
                Id = context.Clients.Count() + 1,
                Fio = FioBox.Text,
                ClientCode = Convert.ToInt32(CodeBox.Text),
                Passport = PassportBox.Text,
                Birthday = DateOnly.TryParse(BirthdayBox.Text, out var birthDate) ? birthDate : null,
                Address = AddressBox.Text,
                Email = EmailBox.Text,
                Password = PasswordBox.Text,
                RoleId = 1
            };

            context.Clients.Add(NewClient);
            context.SaveChanges();

            UserAdd.Text = "Client added successfully!";

            FioBox.Text = "";
            CodeBox.Text = "";
            PassportBox.Text = "";
            BirthdayBox.Text = "";
            AddressBox.Text = "";
            EmailBox.Text = "";
            PasswordBox.Text = "";
            PhoneBox.Text = "";
            
            


        }
        catch
        {
            throw new ArgumentException("Если Марк посмотрит мой код поставьте 2");
        }

    }

    private void CorrectInput()
    {
        if (CodeBox.Text.Length != 8)
        {
            throw new ArgumentException("Символов должно быть ровно 8");
        }

        if (PassportBox.Text.Length != 10)
        {
            throw new ArgumentException("В паспорте 10 цифр");
        }

        if (!EmailBox.Text.Contains("@"))
        {
            throw new ArgumentException("Email must contain '@'!");
        }

        if (PhoneBox.Text.Length != 11)
        {
            throw new ArgumentException("Неккоректно введен номер телефона");
        }
    }

    private void BackOnOrder(object? sender, RoutedEventArgs e)
    {
        new SallerWindow().ShowDialog(this);
    }

    
}