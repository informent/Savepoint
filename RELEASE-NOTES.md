# Savepoint 1.5.1

Savepoint now searches the real Steam userdata layout when finding game saves, including Steam accounts on the primary installation when a game is installed in a secondary library. Each account remains a separate candidate. The app prefers a game's existing remote folder and rejects linked candidates instead of suggesting a broader userdata folder.

No snapshot or profile format migration is required. Existing backups and profiles remain in place. Verify the selected candidate before creating a profile; some games save outside Steam's cloud folders.

## Downloads

- `Savepoint-v1.5.1-setup.exe`: setup for your Windows account, with the .NET runtime bundled.
- `Savepoint-v1.5.1-win-x64.zip`: portable application and documentation.
- `SHA256SUMS`: SHA-256 values for both packages and the validation report.
- `RELEASE-VALIDATION.json`: source commit and completed build, regression, packaged headless, installer, reinstall, launch, uninstall and user-file preservation checks.

The release workflow verifies the version tag and executable version, builds the packages from that exact commit, and checks all published package hashes before publication. Windows x64 builds are unsigned. These checks do not certify every game's save location or every Steam configuration.
