@setlocal enableextensions
@set "COMPLUS_ENABLE_64BIT=0" rem  always select x86 for AnyCPU
@set "COMPLUS_Version=v4.0.30319"  rem  force .NET 4.0 for PowerShell 4.0
@set "COMPLUS_LoadFromRemoteSources=1"  rem  load assemblies from network

@set "powershell=%SystemRoot%\syswow64\WindowsPowerShell\v1.0\powershell.exe"
@set "rootPath=%~dp0"
cd %~dp0

@set DefaultTask=Default
@set DockerImage=

@IF NOT DEFINED SWbuildUrl (@set SWbuildUrl="http://dev-artifactory.dev.local/artifactory/sbf/SW.Build.ps1")

@IF "%1" NEQ "" (SET DefaultTask=%1) 
@IF "%2" NEQ "" (SET DockerImage=,'%2') 

@set "arguments=-Version 5.0 -NoProfile -NonInteractive -NoLogo -ExecutionPolicy Unrestricted -Command "

echo -argumentlist '%DefaultTask%'%DockerImage%

@%powershell% %arguments% "&{ $ErrorActionPreference='Stop'; $sb = [scriptblock]::Create((New-Object System.Net.WebClient).DownloadString('%SWbuildUrl%'));Invoke-command $sb -argumentlist '%DefaultTask%'%DockerImage%; exit $LASTEXITCODE; }"

@exit /B %ERRORLEVEL%