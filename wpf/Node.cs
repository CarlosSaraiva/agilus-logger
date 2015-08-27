using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AgilusLogger
{
    public class NodeAction
    {
        private NodeAction(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public static NodeAction Install => new NodeAction("Install");

        public static NodeAction Uninstall => new NodeAction("Uninstall");

        public static NodeAction Update => new NodeAction(@"Update");
    }

    public static class Node
    {
        private static Process _process;
        public static event EventHandler OnInstallBegin;
        public static event EventHandler OnInstallFinished
        {
            add { _process.Exited += value; }
            remove { _process.Exited -= value; }
        }

        public static void ExecuteNode(NodeAction command, LoggerSer arguments)
        {
            switch (command)
            {
                case: NodeAction.Install

            }




            _process = new Process
            {
                StartInfo =
                {
                    FileName = "",
                    Arguments = $"node {command} ",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    RedirectStandardOutput = true
                }
                
            };
            {
                _process.Start();
                OnInstallBegin?.Invoke(null, new EventArgs());
                _process.WaitForExit();
                OnInstallFinished?.Invoke(null, new EventArgs());
            }


            
        }


    }
}
