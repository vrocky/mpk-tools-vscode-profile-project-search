using Microsoft.Win32;
using System.Windows;
using VsCodeProfileCommon.Models;
using VsCodeProfileCommon.Services;

namespace VsCodeProfileProjectSearch.Views;

public partial class SettingsWindow : Window
{
    private readonly AppSettings _settings;
    public bool Saved { get; private set; }

    public SettingsWindow(AppSettings settings)
    {
        InitializeComponent();
        _settings = settings;
        RefreshList();
    }

    private void RefreshList()
    {
        DirectoryList.ItemsSource = null;
        DirectoryList.ItemsSource = _settings.ProfileDirectories.ToList();
    }

    private void Browse_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select a profile container directory",
            Multiselect = false
        };

        if (dialog.ShowDialog(this) == true)
        {
            NewPathBox.Text = dialog.FolderName;
        }
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        var path = NewPathBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        if (_settings.ProfileDirectories.Contains(path, StringComparer.OrdinalIgnoreCase))
        {
            return;
        }

        _settings.ProfileDirectories.Add(path);
        NewPathBox.Clear();
        RefreshList();
    }

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        if (DirectoryList.SelectedItem is string selected)
        {
            _settings.ProfileDirectories.Remove(selected);
            RefreshList();
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        SettingsService.Save(_settings);
        Saved = true;
        Close();
    }
}
