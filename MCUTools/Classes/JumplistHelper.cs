using McuTools.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Shell;

namespace McuTools
{
    internal class JumplistHelper
    {
        private JumpList _jl;

        public JumplistHelper()
        {
            try
            {
                string _apprdir = System.AppDomain.CurrentDomain.BaseDirectory + "\\SOC\\";
                _jl = new JumpList();
                string[] programs = Directory.GetFiles(_apprdir, "*.exe", SearchOption.AllDirectories);

                foreach (var program in programs)
                {
                    string filename = Path.GetFileName(program);
                    _jl.JumpItems.Add(new JumpTask()
                    {
                        ApplicationPath = program,
                        Description = filename,
                        Title = filename,
                        IconResourcePath = program,
                        IconResourceIndex = 0,
                        CustomCategory = "SOC Tools"
                    });
                }

                JumpList.SetJumpList(App.Current, _jl);
                _jl.Apply();
            }
            catch (DirectoryNotFoundException) { }
            catch (IOException) { }
        }
    }
}
