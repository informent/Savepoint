# Savepoint

Savepoint is a local-first Windows utility for protecting game saves with clear, inspectable backups.

## What it does

- Detects installed Steam games from Steam's library manifests.
- Shows selected game artwork when available.
- Finds common save locations, including Steam userdata and standard Windows folders.
- Finds a game's Steam cloud files across local Steam accounts, including games installed in a secondary library. When present, the game's `remote` folder is preferred over cloud metadata.

- Creates atomic timestamped snapshots with SHA-256 hashes and a JSON manifest.
- Validates every path, hash, length, duplicate, and destination collision before restoring the first file.
- Keeps data on your computer. No account, telemetry, or cloud service is required.

Steam cloud candidates follow Valve's documented [`Steam\userdata\<account>\<appid>` layout](https://help.steampowered.com/en/faqs/view/68d2-35ab-09a9-7678). Linked folders are excluded. These are candidates, not a complete database of every game's save locations; verify the selected folder before making a profile. Multiple accounts stay separate.

## Use

1. Start Savepoint.
2. Select **Detect Steam games** and choose a game.
3. Select **Find save folders**. If no single safe candidate is found, choose the folder manually.
4. Choose a backup library on a different drive when possible.
5. Create snapshots and restore them from the history panel.

Savepoint never silently overwrites files.

Version 1.2 records size and SHA-256 from the snapshot copy itself, refuses linked files and folders during capture, and checks restore roots and ancestors for reparse points before writing.

Version 1.3 rejects snapshot libraries that resolve inside a game's save folder before creating output, preventing a backup from recursively including its own working directory. Profile names are restricted to a single safe folder name.

## Build and test

```powershell
dotnet run --project tests/SaveEngineTests.csproj
dotnet run --project tests/SteamDiscoveryTests.csproj
dotnet run --project tests/SaveLocationTests.csproj
dotnet build Savepoint.csproj -c Release
```

GitHub Actions runs these checks on Windows. Release packages are published in GitHub Releases.

## Steam notice

Savepoint can read local Steam library manifests to find installed games. It is an independent project and is not affiliated with, endorsed by, or sponsored by Valve Corporation or Steam.

## License

Savepoint is released under the MIT License. See [LICENSE](LICENSE).

## Name change

Savepoint replaces OpenGameSave. New builds use Savepoint.exe and Savepoint.csproj. Existing releases remain available under their original names.

Existing settings and data locations retain the OpenGameSave folder name for compatibility. Keep these folders when upgrading; no automatic move or data format change is required.

## Automatic setup

Windows setup packages install Savepoint for your current account, bundle the .NET runtime, create a Start menu shortcut and register an uninstaller. A desktop shortcut is optional. No administrator password or manual file copying is required. Existing settings and saved data stay in their current locations. The portable ZIP remains available.

Download `Savepoint-v1.5.1-setup.exe` from Releases and run it.

For unattended deployment:

```powershell
Start-Process .\Savepoint-v1.5.1-setup.exe -ArgumentList '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-' -Wait -PassThru
```

Build from source with the [open-source Inno Setup compiler](https://github.com/jrsoftware/issrc) installed:

```powershell
pwsh -File scripts/Build-Setup.ps1 -Verify
```

The verification option installs into a temporary test folder, checks reinstall and launch, then uninstalls. It runs on the hosted Windows runner during CI. It does not remove existing user settings or backups.

## Compare saves and preview restores

Version 1.5 adds headless JSON reports to help inspect a game's save history before restoring. Both commands verify snapshot paths, file sizes and SHA-256 values before writing a report.

```powershell
Start-Process .\Savepoint.exe -ArgumentList '--compare "D:\Saves\Game\OLDER" "D:\Saves\Game\NEWER" --report "D:\Reports\changes.json"' -WindowStyle Hidden -Wait -PassThru
Start-Process .\Savepoint.exe -ArgumentList '--preview-restore "D:\Saves\Game\NEWER" "D:\Restore" --report "D:\Reports\restore.json"' -WindowStyle Hidden -Wait -PassThru
```

Comparison reports classify Added, Removed, Modified and Unchanged files and show the net byte change. Restore previews classify NewFile, Identical, Conflict and Blocked targets. Neither command changes saves or restore destinations. Reports contain local filenames and paths, so keep them private when necessary. The report folder must already exist and the output filename must be new. Exit code 0 means success; code 2 means invalid input, failed integrity checks or an output failure. Refresh previews if files change before restoring.

Capture and restore reject linked ancestors. Directory collisions and files blocking destination folders are checked before copying begins. The root filename `manifest.json` is reserved for snapshot metadata and cannot be captured; nested files with that name are supported. No snapshot format migration is required.
