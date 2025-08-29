; ------- Inno Setup script para ORCA -------
#define MyAppName "ORCA"

#ifndef MyAppVersion
  ; fallback caso não seja passado /dMyAppVersion=...
  #define MyAppVersion "0.0.0"
#endif

#define MyAppExeName "ORCA.exe"

[Setup]
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
; Agora busca diretamente na pasta publish criada pelo workflow
Source: "publish\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Executar {#MyAppName}"; Flags: nowait postinstall skipifsilent
