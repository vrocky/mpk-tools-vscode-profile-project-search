# Glossary

## Profile Container Directory
The parent directory that contains multiple VS Code profile directories.

Example: `C:\VSCodeProfiles`

This is the directory stored in registry value `ProfileDirectories`.

## VS Code Profile Directory
A single child folder under the profile container directory representing one VS Code profile.

Example: `C:\VSCodeProfiles\Work`

Each profile directory includes `user-data` and `extensions`.

## VS Code Profile
A profile folder containing isolated VS Code data:
- `user-data`
- `extensions`

## User Data Directory
The path passed to VS Code with `--user-data-dir`. It stores settings, UI state, and global storage.

## Extensions Directory
The path passed to VS Code with `--extensions-dir`. It stores profile-specific extensions.

## Recent Projects
VS Code recently opened folder/workspace entries, read from `storage.json` or `state.vscdb`.

## Workspace Project
A `.code-workspace` file entry from recent projects.

## Folder Project
A folder URI entry from recent projects.

## Profile-Scoped Open
Opening a project with both `--user-data-dir` and `--extensions-dir` set to a specific profile.
