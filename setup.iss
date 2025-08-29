#define MyAppName "ORCA"
#define MyAppVersion GetStringDef("MyAppVersion", "0.0.0")
#define MyAppExeName "ORCA.exe"

[Setup]
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={autopf}\{#MyAppName}
OutputDir=dist
OutputBaseFilename=ORCA_Installer_{#MyAppVersion}
Compression=lzma
SolidCompression=yes

[Files]
Source: "ORCA\bin\Release\net8.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
