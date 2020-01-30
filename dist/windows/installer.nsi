Var uninstallerPath

Section "-hidden"

    ;Search if ChapterTool is already installed.
    FindFirst $0 $1 "$uninstallerPath\uninst.exe"
    FindClose $0
    StrCmp $1 "" done

    ;Run the uninstaller of the previous install.
    DetailPrint $(inst_unist)
    ExecWait '"$uninstallerPath\uninst.exe" /S _?=$uninstallerPath'
    Delete "$uninstallerPath\uninst.exe"
    RMDir "$uninstallerPath"

    done:

SectionEnd


Section $(inst_ct_req) ;"ChapterTool (required)"

  SectionIn RO

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR

  ;Create 'en-US' directory
  CreateDirectory $INSTDIR\en-US

  ; Put file there
  File "ChapterTool.exe"
  File "ChapterTool.exe.config"
  File "libmp4v2.dll"
  File /oname=en-US\ChapterTool.resources.dll "en-US\ChapterTool.resources.dll"


  ; Write the installation path into the registry
  WriteRegStr HKLM "Software\ChapterTool" "InstallLocation" "$INSTDIR"

  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ChapterTool" "DisplayName" "ChapterTool ${PROG_VERSION}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ChapterTool" "UninstallString" '"$INSTDIR\uninst.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ChapterTool" "DisplayIcon" '"$INSTDIR\ChapterTool.exe",0'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ChapterTool" "Publisher" "TautCony"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ChapterTool" "URLInfoAbout" "https://github.com/tautcony/ChapterTool"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ChapterTool" "DisplayVersion" "${PROG_VERSION}"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ChapterTool" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ChapterTool" "NoRepair" 1
  WriteUninstaller "uninst.exe"
  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  IntFmt $0 "0x%08X" $0
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ChapterTool" "EstimatedSize" "$0"

SectionEnd

; Optional section (can be disabled by the user)
Section /o $(inst_dekstop) ;"Create Desktop Shortcut"

  CreateShortCut "$DESKTOP\ChapterTool.lnk" "$INSTDIR\ChapterTool.exe"

SectionEnd

Section $(inst_startmenu) ;"Create Start Menu Shortcut"

  CreateDirectory "$SMPROGRAMS\ChapterTool"
  CreateShortCut "$SMPROGRAMS\ChapterTool\ChapterTool.lnk" "$INSTDIR\ChapterTool.exe"
  CreateShortCut "$SMPROGRAMS\ChapterTool\Uninstall.lnk" "$INSTDIR\uninst.exe"

SectionEnd


;--------------------------------

Function .onInit

  !insertmacro Init "installer"
  !insertmacro MUI_LANGDLL_DISPLAY

  ;Search if ChapterTool is already installed.
  FindFirst $0 $1 "$INSTDIR\uninst.exe"
  FindClose $0
  StrCmp $1 "" done

  ;Copy old value to var so we can call the correct uninstaller
  StrCpy $uninstallerPath $INSTDIR

  ;Inform the user
  MessageBox MB_OKCANCEL|MB_ICONINFORMATION $(inst_uninstall_question) /SD IDOK IDOK done
  Quit

  done:

FunctionEnd


Function PageFinishRun

  !insertmacro UAC_AsUser_ExecShell "" "$INSTDIR\ChapterTool.exe" "" "" ""

FunctionEnd

Function .onInstSuccess
  SetErrorLevel 0
FunctionEnd