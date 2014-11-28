@echo off
rem Bin & Obj folder cleanup
rem Written by Webmaster442

for /D %%d in (*) do if exist "%%d\bin" echo Y | rmdir /s "%%d\bin"
for /D %%d in (*) do if exist "%%d\obj" echo Y | rmdir /s "%%d\obj"
for /D %%d in (*) do if exist "%%d\bin" echo I | rmdir /s "%%d\bin"
for /D %%d in (*) do if exist "%%d\obj" echo I | rmdir /s "%%d\obj"