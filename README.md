# VS Code Profile Project Search

`vscode-profile-project-search` is a Windows WPF app that indexes recent projects across every VS Code profile found under registered profile container directories.

It reads the same registry settings and profile container directories, then scans each profile's VS Code user data to extract recently opened folders/workspaces. This gives you one searchable list across all profiles so you can quickly find a project and open it directly in the matching profile.

## Why this app

When you maintain many isolated VS Code profiles, recent projects are split per profile and become hard to find. This app solves that by aggregating profile-local history into one place.

## Data sources

For each detected profile, the app reads:

1. `user-data\\globalStorage\\storage.json` (`recentlyOpenedPathsList.entries`)
2. fallback: `user-data\\globalStorage\\state.vscdb` (`history.recentlyOpenedPathsList` or related keys)

## Registry contract

The app uses:

- Key: `HKEY_CURRENT_USER\\Software\\VsCodeProfilePicker`
- `ProfileDirectories` (`REG_MULTI_SZ`): one or more profile container directories that contain profile folders
- `VsCodeExePath` (`REG_SZ`): VS Code executable path/command

## Search behavior

- Searches across all registered profile container directories.
- Includes a `Default` profile for each profile container directory (container root with `user-data` and `extensions`).
- Matches project path, profile name, profile path, and container directory path.
- Lists projects by timeline rank (most recent first within each profile history list).

## Settings dialog

The app includes a Settings dialog (like `vscode-profile-picker`) where you can add/remove profile container directories stored in the registry.

## Open behavior

When you open a project from this app, it launches VS Code with the profile's isolated paths:

```text
code --user-data-dir "<profile>\\user-data" --extensions-dir "<profile>\\extensions" "<projectPath>"
```

## Build and run

```powershell
cd C:\Users\ws-user\Documents\project-8\vscode-profile-project-search
dotnet build
dotnet run
```

## Shared library

This app depends on the shared library at:

- `..\\vscode-profile-picker\\Shared\\VsCodeProfileCommon`

The shared library contains settings loading, profile scanning, and recent-project parsing so behavior stays consistent across both apps.

## Glossary

See `docs/glossary.md`.
