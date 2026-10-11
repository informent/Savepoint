#ifndef AppVersion
  #error AppVersion must be supplied
#endif
#ifndef PayloadDir
  #error PayloadDir must be supplied
#endif
#ifndef OutputDir
  #error OutputDir must be supplied
#endif
[Setup]
AppId={{66A21867-D272-530D-9E3F-C8D9A17AF833}
AppName=Savepoint
AppVersion={#AppVersion}
AppPublisher=informent
AppPublisherURL=https://github.com/informent/Savepoint
DefaultDirName={localappdata}\Programs\Savepoint
DefaultGroupName=Savepoint
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0.17763
OutputDir={#OutputDir}
OutputBaseFilename=Savepoint-v{#AppVersion}-setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\Savepoint.exe
CloseApplications=yes
CloseApplicationsFilter=Savepoint.exe
RestartApplications=no
SetupLogging=yes
[Tasks]
Name: desktopicon; Description: "Create a desktop shortcut"; GroupDescription: "Shortcuts:"; Flags: unchecked
[Files]
Source: "{#PayloadDir}\*"; DestDir: "{app}"; Excludes: "*.pdb"; Flags: ignoreversion recursesubdirs createallsubdirs
[Icons]
Name: "{autoprograms}\Savepoint"; Filename: "{app}\Savepoint.exe"; WorkingDir: "{app}"
Name: "{autodesktop}\Savepoint"; Filename: "{app}\Savepoint.exe"; WorkingDir: "{app}"; Tasks: desktopicon
[Run]
Filename: "{app}\Savepoint.exe"; Description: "Open Savepoint"; Flags: nowait postinstall skipifsilent unchecked
