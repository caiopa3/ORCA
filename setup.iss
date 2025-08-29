; ------- Inno Setup script para ORCA -------
#define MyAppName "ORCA"

#ifndef MyAppVersion
  ; fallback caso não seja passado /dMyAppVersion=...
  #define MyAppVersion "0.0.0"
#endif

#define MyAppExeName "ORCA.exe"

[Setup]
; Use um GUID próprio se quiser manter o mesmo AppId entre versões
AppId={{8B7C4D6B-6E6E-4A1D-9D6E-ORCA-000000000001}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=dist
OutputBaseFilename=ORCA_Installer_{#MyAppVersion}
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=lowest
DisableDirPage=no

[Files]
; Inclui os binários publicados pelo dotnet publish
Source: "ORCA\bin\Release\net8.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Executar {#MyAppName}"; Flags: nowait postinstall skipifsilent
