; Arquivo: setup.iss

[Setup]
AppName=ORCA
AppVersion={#MyAppVersion}
DefaultDirName={pf}\ORCA
DefaultGroupName=ORCA
OutputDir=dist
OutputBaseFilename=ORCA_Installer_{#MyAppVersion}
Compression=lzma
SolidCompression=yes

[Files]
Source: "ORCA\bin\Release\net8.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\ORCA"; Filename: "{app}\ORCA.exe"
Name: "{commondesktop}\ORCA"; Filename: "{app}\ORCA.exe"
