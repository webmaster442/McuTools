@echo off
title MCU tools release build script
rem ---------------------------------------------------------------------------
rem MCU Tools Release build script v. 1.3
rem Build Requirements:
rem 	.NET Framework 4.5
rem 	Visual Studio 2012/2013
rem 	Awesomium
rem ---------------------------------------------------------------------------

rem ---------------------------------------------------------------------------
rem Prepare
rem ---------------------------------------------------------------------------
if exist bin\release echo Y | rmdir /s bin\release
if exist bin\release echo I | rmdir /s bin\release

rem ---------------------------------------------------------------------------
rem setup
rem ---------------------------------------------------------------------------
set msbuildp=%windir%\Microsoft.NET\Framework\v4.0.30319\
set awe=c:\Program Files (x86)\Awesomium Technologies LLC\Awesomium SDK\1.7.5.0\build\bin\
SET PATH=%PATH%;%msbuildp%;%awe%

rem ---------------------------------------------------------------------------
rem compile
rem ---------------------------------------------------------------------------
msbuild /m McuTools.sln /p:Configuration=Release
cd MCUShell
msbuild /m McuShell.sln /p:Configuration=Release
cd ..

rem ---------------------------------------------------------------------------
rem pack
rem ---------------------------------------------------------------------------
awesomium_pak_utility htmlassets htmlassets bin\release\

rem ---------------------------------------------------------------------------
rem cleanup
rem ---------------------------------------------------------------------------
cd bin\Release
del *.pdb
del *.vshost.exe.config
del *.vshost.exe
del inspector.pak
del Awesomium.Core.xml
del Awesomium.Windows.Controls.xml
del IronPython.xml
del IronPython.Modules.xml
del IronPython.SQLite.xml
del IronPython.Wpf.xml
del Microsoft.Dynamic.xml
del Microsoft.Scripting.xml
del Microsoft.Scripting.AspNet.xml
del Microsoft.Scripting.Metadata.xml
del MathNet.Numerics.IO.xml
del MathNet.Numerics.xml
del MahApps.Metro.xml
del System.Windows.Interactivity.xml
cd shell
del *.pdb
del *.vshost.exe.config
del *.vshost.exe
cd ..
echo Y | rmdir /s de
echo Y | rmdir /s el-GR
echo Y | rmdir /s es
echo Y | rmdir /s fr
echo Y | rmdir /s it
echo Y | rmdir /s ja
echo Y | rmdir /s ko
echo Y | rmdir /s ru
echo Y | rmdir /s zh-Hans
echo Y | rmdir /s zh-Hant
echo Y | rmdir /s de-DE

echo I | rmdir /s de
echo I | rmdir /s el-GR
echo I | rmdir /s es
echo I | rmdir /s fr
echo I | rmdir /s it
echo I | rmdir /s ja
echo I | rmdir /s ko
echo I | rmdir /s ru
echo I | rmdir /s zh-Hans
echo I | rmdir /s zh-Hant
echo I | rmdir /s de-DE

cd ..
cd ..

rem ---------------------------------------------------------------------------
rem end
rem ---------------------------------------------------------------------------
:end
call clean_binobj.cmd
echo "Done"
pause