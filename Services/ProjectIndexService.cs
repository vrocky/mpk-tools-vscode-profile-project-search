using System.IO;
using VsCodeProfileCommon.Models;
using VsCodeProfileCommon.Services;
using VsCodeProfileProjectSearch.Models;

namespace VsCodeProfileProjectSearch.Services;

public static class ProjectIndexService
{
    public static List<ProjectSearchItem> BuildIndex(bool includeMissing)
    {
        var settings = SettingsService.Load();
        var scannedProfiles = ProfileScanService.ScanAll(settings.ProfileDirectories);
        var profiles = BuildProfilesIncludingContainerDefault(settings.ProfileDirectories, scannedProfiles);

        var indexed = new List<ProjectSearchItem>();

        foreach (var profile in profiles)
        {
            var projects = RecentProjectsService.ReadFromProfile(profile.UserDataPath, includeMissing);

            foreach (var project in projects)
            {
                if (!includeMissing && !project.Exists)
                {
                    continue;
                }

                indexed.Add(new ProjectSearchItem
                {
                    ProfileName = profile.Name,
                    ProfileContainerDirectory = profile.SourceDirectory,
                    ProfilePath = profile.FullPath,
                    ProjectPath = project.Path,
                    Kind = project.Kind,
                    TimelineRank = project.Index,
                    Exists = project.Exists,
                    Source = project.Source,
                    UserDataPath = profile.UserDataPath,
                    ExtensionsPath = profile.ExtensionsPath,
                    ProfileLastModified = profile.LastModified,
                    ProjectLastAccessedUtc = project.LastAccessedUtc
                });
            }
        }

        // Sort by most recent project timestamp first. Fall back to profile recency and timeline rank.
        return indexed
            .OrderByDescending(item => item.ProjectLastAccessedUtc ?? DateTime.MinValue)
            .ThenByDescending(item => item.ProfileLastModified)
            .ThenBy(item => item.TimelineRank)
            .ThenBy(item => item.ProfileName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.ProjectPath, StringComparer.OrdinalIgnoreCase)
            .DistinctBy(item => $"{item.ProfilePath}|{item.ProjectPath}", StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static List<VsCodeProfile> BuildProfilesIncludingContainerDefault(
        IEnumerable<string> profileContainerDirectories,
        IEnumerable<VsCodeProfile> scannedProfiles)
    {
        var allProfiles = scannedProfiles.ToList();

        foreach (var containerDirectory in profileContainerDirectories.Where(path => !string.IsNullOrWhiteSpace(path)))
        {
            if (!Directory.Exists(containerDirectory))
            {
                continue;
            }

            var defaultProfilePath = containerDirectory;
            var userDataPath = ResolveContainerDefaultUserDataPath(defaultProfilePath);
            var extensionsPath = Path.Combine(defaultProfilePath, "extensions");

            Directory.CreateDirectory(userDataPath);
            Directory.CreateDirectory(extensionsPath);

            allProfiles.Add(new VsCodeProfile
            {
                Name = "Default",
                FullPath = defaultProfilePath,
                SourceDirectory = containerDirectory,
                Initials = "DF",
                AvatarColor = "#007acc",
                UserDataPath = userDataPath,
                ExtensionsPath = extensionsPath,
                LastModified = Directory.GetLastWriteTime(defaultProfilePath)
            });
        }

        return allProfiles
            .DistinctBy(profile => profile.FullPath, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string ResolveContainerDefaultUserDataPath(string containerDirectory)
    {
        var dataPath = Path.Combine(containerDirectory, "data");
        if (Directory.Exists(dataPath))
        {
            return dataPath;
        }

        var userDataPath = Path.Combine(containerDirectory, "user-data");
        return userDataPath;
    }
}
