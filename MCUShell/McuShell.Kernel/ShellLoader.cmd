@echo off
Title McuTools Shell

SET SHELL=%CD%
SET PATH=%PATH%;%SHELL%

chcp 65001

goto intro

:intro
color 02
cls
echo.
echo " __  __  _____ _    _ _______          _        _____ _          _ _ "
echo "|  \/  |/ ____| |  | |__   __|        | |      / ____| |        | | |"
echo "| \  / | |    | |  | |  | | ___   ___ | |___  | (___ | |__   ___| | |"
echo "| |\/| | |    | |  | |  | |/ _ \ / _ \| / __|  \___ \| '_ \ / _ \ | |"
echo "| |  | | |____| |__| |  | | (_) | (_) | \__ \  ____) | | | |  __/ | |"
echo "|_|  |_|\_____|\____/   |_|\___/ \___/|_|___/ |_____/|_| |_|\___|_|_|"
echo.
echo MCUTools Shell v. 2015-01
echo Shell Path: %SHELL%
echo Extra commands:
echo df wget ls markdown dirsize envvars ntpdate
echo.