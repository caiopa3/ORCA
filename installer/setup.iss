; installer/setup.iss
#define AppVersion GetDefine('AppVersion', '1.0.0')
#define AppPublisher GetDefine('AppPublisher', 'ORCA')
#define OutputDir "installer\\Output"

[Setup]
; Use um GUID fixo para manter o "mesmo app" em upgrades
AppId={{C14FCEE5-71E0-4781-9A68-A1BDFB53CF4C}
AppName=ORCA
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\ORCA
DefaultGroupName=ORCA
OutputDir={#OutputDir}
OutputBaseFilename=ORCA-Setup-{#AppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin
DisableProgramGroupPage=yes

[Files]
; Publicação feita pelo workflow em dist\ORCA
Source: "..\dist\ORCA\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs

[Icons]
Name: "{group}\ORCA"; Filename: "{app}\ORCA.exe"
Name: "{commondesktop}\ORCA"; Filename: "{app}\ORCA.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Criar atalho na área de trabalho"; GroupDescription: "Atalhos:"; Flags: unchecked

[Run]
; Executa o ORCA.exe após a instalação
Filename: "{app}\ORCA.exe"; Description: "Executar ORCA agora"; Flags: nowait postinstall skipifsilent
