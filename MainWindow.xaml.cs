using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using VsCodeProfileCommon.Models;
using VsCodeProfileCommon.Services;
using VsCodeProfileProjectSearch.Models;
using VsCodeProfileProjectSearch.Services;
using VsCodeProfileProjectSearch.Views;

namespace VsCodeProfileProjectSearch;

public partial class MainWindow : Window
{
    private AppSettings _settings = new();
    private List<ProjectSearchItem> _allProjects = [];

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await RefreshIndexAsync();
        SearchBox.Focus();
    }

    private async Task RefreshIndexAsync()
    {
        try
        {
            StatusText.Text = "Scanning profile container directories...";
            LoggingService.Info("Refreshing project index.");

            var includeMissing = IncludeMissingCheck.IsChecked == true;

            var result = await Task.Run(() =>
            {
                var settings = SettingsService.Load();
                var projects = ProjectIndexService.BuildIndex(includeMissing);
                return (settings, projects);
            });

            _settings = result.settings;
            _allProjects = result.projects;

            SubtitleText.Text = _settings.ProfileDirectories.Count == 0
                ? "No profile container directories found in registry. Configure them in Settings."
                : $"{_allProjects.Count} project entries across {_settings.ProfileDirectories.Count} profile container director{(_settings.ProfileDirectories.Count == 1 ? "y" : "ies")}";

            Render(_allProjects);
            LoggingService.Info($"Index refresh complete. Entries={_allProjects.Count}, Containers={_settings.ProfileDirectories.Count}.");
        }
        catch (Exception ex)
        {
            LoggingService.Error("RefreshIndexAsync failed.", ex);
            StatusText.Text = "Error while scanning. See log file in %LocalAppData%\\VsCodeProfileProjectSearch\\logs.";
        }
    }

    private void Render(List<ProjectSearchItem> projects)
    {
        ProjectsList.ItemsSource = null;
        ProjectsList.ItemsSource = projects;

        StatusText.Text = projects.Count == 0
            ? "No projects found from recent history."
            : "Click any existing entry to open it in its original profile.";
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var query = SearchBox.Text.Trim();
        var filtered = string.IsNullOrWhiteSpace(query)
            ? _allProjects
            : _allProjects.Where(project =>
                project.ProjectPath.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                project.ProfileName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                project.ProfilePath.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                project.ProfileContainerDirectory.Contains(query, StringComparison.OrdinalIgnoreCase)
            ).ToList();

        Render(filtered);
    }

    private void OpenProject_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: ProjectSearchItem item })
        {
            return;
        }

        if (!item.Exists)
        {
            StatusText.Text = "Cannot open: path no longer exists.";
            return;
        }

        try
        {
            var arguments =
                $"--user-data-dir \"{item.UserDataPath}\" --extensions-dir \"{item.ExtensionsPath}\" \"{item.ProjectPath}\"";

            Process.Start(new ProcessStartInfo
            {
                FileName = _settings.VsCodeExePath,
                Arguments = arguments,
                UseShellExecute = true
            });

            StatusText.Text = $"Opened with profile {item.ProfileName}";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Failed to open project: {ex.Message}";
        }
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        _ = RefreshIndexAsync();
    }

    private void IncludeMissingCheck_Click(object sender, RoutedEventArgs e)
    {
        _ = RefreshIndexAsync();
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow(_settings) { Owner = this };
        settingsWindow.ShowDialog();

        if (settingsWindow.Saved)
        {
            SearchBox.Clear();
            _ = RefreshIndexAsync();
        }
    }
}
