namespace VsCodeProfileProjectSearch.Models;

public sealed class ProjectSearchItem
{
    public string ProfileName { get; set; } = "";
    public string ProfileContainerDirectory { get; set; } = "";
    public string ProfilePath { get; set; } = "";
    public string ProjectPath { get; set; } = "";
    public string Kind { get; set; } = "unknown";
    public int TimelineRank { get; set; }
    public bool Exists { get; set; }
    public string Source { get; set; } = "none";
    public string UserDataPath { get; set; } = "";
    public string ExtensionsPath { get; set; } = "";
    public DateTime ProfileLastModified { get; set; }
    public DateTime? ProjectLastAccessedUtc { get; set; }
    public string DisplayName => System.IO.Path.GetFileNameWithoutExtension(ProjectPath);
    public string TimelineLabel => $"#{TimelineRank + 1}";
    public string ExistsLabel => Exists ? "Exists" : "Missing";
}
