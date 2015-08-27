using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgilusLogger
{
    public enum NodeAction
    {
        Install,
        Uninstall,
        Update
    }

    public static class Node
    {
        private static Process _process;
        public static event EventHandler OnInstallBegin;
        public static event EventHandler OnInstallFinished;

        public static void ExecuteNode(NodeAction command)
        {
            _process = new Process
            {
                StartInfo =
                {
                    FileName = "",
                    Arguments = "",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    RedirectStandardOutput = true
                }
                
            };
            {
                _process.Start();
                OnInstallBegin?.Invoke(this, new EventArgs());
                _process.WaitForExit();
                OnInstallFinished?.Invoke(this, new EventArgs());
            }


            
        }


    }
}
