# Savepoint

Savepoint is a local-first Windows utility for protecting game saves with clear, inspectable backups.

## What it does

- Detects installed Steam games from Steam's library manifests.
- Shows selected game artwork when available.
- Finds common save locations, including Steam userdata and standard Windows folders.
- Creates atomic timestamped snapshots with SHA-256 hashes and a JSON manifest.
- Validates every path, hash, length, duplicate, and destination collision before restoring the first file.
- Keeps data on your computer. No account, telemetry, or cloud service is required.

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

Download `Savepoint-v1.4.1-setup.exe` from Releases and run it.

For unattended deployment:

```powershell
Start-Process .\Savepoint-v1.4.1-setup.exe -ArgumentList '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-' -Wait -PassThru
```

Build from source with the [open-source Inno Setup compiler](https://github.com/jrsoftware/issrc) installed:

```powershell
pwsh -File scripts/Build-Setup.ps1 -Verify
```

The verification option installs into a temporary test folder, checks reinstall and launch, then uninstalls. It runs on the hosted Windows runner during CI. It does not remove existing user settings or backups.
