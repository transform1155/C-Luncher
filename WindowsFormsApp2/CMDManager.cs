using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public class CMDManager
    {
        public CMDManager(string cmd) 
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = cmd; 
            startInfo.UseShellExecute = false; 
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true; 
            using (Process process = new Process())
            {
                process.StartInfo = startInfo; process.Start(); process.WaitForExit();
            }
        }
    }
}
