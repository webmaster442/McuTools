Developer documentation
====

#Build

To Build MCUTools you will need:

* Visual studio 2013 Community edition (or newer)
http://www.visualstudio.com/
* Awesomium 1.7.5.0
http://www.awesomium.com/

To Build the installer you will need:
* Inno Setup
* Inno Download plugin

After installing tools just run the CompileRelease.cmd file. This will build the software to the bin\release directory with all modules.

To run the software .NET Framework 4.5 must be installed.

#Project Layout

The software is split into modules. A module is basically an Assembly. The Assembly's name must end with tool.dll. The Assembly loader in MCU tools will load all the objects that are inherited from the following classes:
* Tool
A basic tool. Opens in a new tab
* PopupTool
A tool that opens in a pop-up window.
* ExternalTool
An external tool, mainly used for program execution
* WebTool
A tool that's found on the internet or opens from a browser.

These classes can be found in The MCUTools.Interfaces assembly file.