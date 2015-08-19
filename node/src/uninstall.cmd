:: Created by npm, please don't edit manually.
@IF EXIST "%~dp0\node.exe" (
  "%~dp0\node.exe" "%appdata%\agilus-logger\uninstall.js" %*
) ELSE (
  node "%appdata%\agilus-logger\uninstall.js" %*
)
