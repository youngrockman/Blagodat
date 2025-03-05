using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using demo_hard.Models;
using Tmds.DBus.Protocol;

namespace demo_hard;

public partial class HistoryWindow : Window
{
    private ObservableCollection<LastEnter> lastEnter = new();
    public List<LastEnter> Enter = new();
    public bool sort = true;
    public HistoryWindow()
    {
        using var context = new User2Context();
        InitializeComponent();
        

        Enter = context.LastEnters.Select(it => new LastEnter
        {
            EmployeId = it.EmployeId,
            Login = it.Login,
            EnterDatetime = it.EnterDatetime,
            EnterType = it.EnterType,
        }).ToList();
        
        foreach (var e in Enter)
        {
            lastEnter.Add(e);
        }
        
        LastEnterBox.ItemsSource = lastEnter;
        LoginComboBox.ItemsSource = Enter.Select(it=>it.Login);
        SortComboBox.ItemsSource = new List<string> { "По возростанию", "по убыванию"};
        SortLogin();
    }
    


    private void SortLogin()
    {
        var temp = Enter;
        if (LoginComboBox.SelectedItem is string login)
        {
            temp = temp.Where(it => it.Login == login).ToList();
        }

        temp = sort? temp.OrderBy(it => it.Login).ToList(): temp.OrderByDescending(it=>it. Login).ToList();

        lastEnter.Clear();

        foreach (var items in temp)
        {
            lastEnter.Add(items);
        }
    }

    private void LoginCombobox_OnSelectedChanged(object? sender, SelectionChangedEventArgs e)
    {
        sort = false;
        SortLogin();
    }
    
    private void SortButton_OnClick(object? sender, RoutedEventArgs e)
    {
        sort = !sort; 
        SortLogin();
    }

    private void SortDateTime()
    {
        var temp = Enter;
        if (SortComboBox.SelectedItem is string sortOption)
        {
            temp = sortOption == "По возрастанию" ? temp.OrderBy(it=>it.EnterDatetime).ToList() : temp.OrderByDescending(it=>it.EnterDatetime).ToList();
        }
        
        lastEnter.Clear();
        foreach (var datetime in temp)
        {
            lastEnter.Add(datetime);
        }
        
        
        
    }

    private void SortComboBox_OnSelectedChanged(object? sender, SelectionChangedEventArgs e)
    {
        SortDateTime();
    }
}