using McuTools.Interfaces;
using System;
using System.Windows.Media.Imaging;

namespace MLaunchers
{
    public class Kitty: Eprog
    {
        public override string Path
        {
            get { return System.IO.Path.Combine(Folders.Application, "\\SOC\\Kitty.exe"); }
        }

        public override string Description
        {
            get { return "Kitty SSH"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MLaunchers.Tool;component/Icons/kitty.png", UriKind.Relative)); }
        }
    }

    public class WinSCP : Eprog
    {
        public override string Path
        {
            get { return System.IO.Path.Combine(Folders.Application, "\\SOC\\WinSCP.exe"); }
        }

        public override string Description
        {
            get { return "WinSCP"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MLaunchers.Tool;component/Icons/winscp.png", UriKind.Relative)); }
        }
    }

    public class Win32DiskImager : Eprog
    {
        public override string Path
        {
            get { return System.IO.Path.Combine(Folders.Application, "\\SOC\\Win32DiskImager.exe"); }
        }

        public override string Description
        {
            get { return "Win32DiskImager"; }
        }

        public override bool AdministratorRequired
        {
            get { return true; }
        } 

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MLaunchers.Tool;component/Icons/win32diskimager.png", UriKind.Relative)); }
        }
    }

    public class TFTPD : Eprog
    {
        public override string Path
        {
            get { return System.IO.Path.Combine(Folders.Application, "\\SOC\\tftpd32.exe"); }
        }

        public override string Description
        {
            get { return "TFTPD32"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MLaunchers.Tool;component/Icons/tftp.png", UriKind.Relative)); }
        }
    }

    public class Npp : Eprog
    {
        public override string Path
        {
            get { return System.IO.Path.Combine(Folders.Application, "\\SOC\\npp\\npp.exe"); }
        }

        public override string Description
        {
            get { return "Notepad++"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MLaunchers.Tool;component/Icons/npp.png", UriKind.Relative)); }
        }
    }

    public class Zip7 : Eprog
    {
        public override string Path
        {
            get { return System.IO.Path.Combine(Folders.Application, "SOC\\7zip\\7zfm.exe"); }
        }

        public override string Description
        {
            get { return "7Zip"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MLaunchers.Tool;component/Icons/7zip.png", UriKind.Relative)); }
        }
    }

    public class RemoteDesk : Eprog
    {
        public override string Path
        {
            get { return @"%windir%\system32\mstsc.exe"; }
        }

        public override string Description
        {
            get { return "Remote Desktop"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MLaunchers.Tool;component/Icons/remotedesktop.png", UriKind.Relative)); }
        }
    }

    public class PowerShell : Eprog
    {
        public override string Path
        {
            get { return @"%SystemRoot%\system32\WindowsPowerShell\v1.0\powershell.exe"; }
        }

        public override string Description
        {
            get { return "PowerShell"; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MLaunchers.Tool;component/Icons/powershell.png", UriKind.Relative)); }
        }
    }

    public class Cmd : Eprog
    {
        public override string Path
        {
            get { return @"%windir%\system32\cmd.exe"; }
        }

        public override string Description
        {
            get { return "Administrator Command Line"; }
        }

        public override bool AdministratorRequired
        {
            get { return true; }
        }

        public override System.Windows.Media.ImageSource Icon
        {
            get { return new BitmapImage(new Uri("/MLaunchers.Tool;component/Icons/cmd.png", UriKind.Relative)); }
        }
    }
}
