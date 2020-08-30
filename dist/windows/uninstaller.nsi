Section "un.$(remove_files)" ;"un.Remove files"
  SectionIn RO

; Remove files and uninstaller
  Delete "$INSTDIR\ChapterTool.exe"
  Delete "$INSTDIR\ChapterTool.exe.config"
  Delete "$INSTDIR\libmp4v2.dll"
  Delete "$INSTDIR\en-US\ChapterTool.resources.dll"
  Delete "$INSTDIR\LICENSE"
  Delete "$INSTDIR\uninst.exe"

  ; Remove directories used
  RMDir /r "$INSTDIR\en-US"
  RMDir "$INSTDIR"
SectionEnd

Section "un.$(remove_shortcuts)" ;"un.Remove shortcuts"
  SectionIn RO
; Remove shortcuts, if any
  RMDir /r "$SMPROGRAMS\ChapterTool"
  Delete "$DESKTOP\ChapterTool.lnk"
SectionEnd


Section "un.$(remove_registry)" ;"un.Remove registry keys"
  SectionIn RO
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ChapterTool"
  DeleteRegKey HKLM "Software\ChapterTool"
  DeleteRegKey HKLM "Software\Classes\ChapterTool"

  System::Call 'Shell32::SHChangeNotify(i ${SHCNE_ASSOCCHANGED}, i ${SHCNF_IDLIST}, i 0, i 0)'
SectionEnd


;--------------------------------
;Uninstaller Functions

Function un.onInit

  !insertmacro Init "uninstaller"
  !insertmacro MUI_UNGETLANGUAGE

FunctionEnd

Function un.onUninstSuccess
  SetErrorLevel 0
FunctionEnd