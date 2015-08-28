using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
<<<<<<< HEAD
using System.Net.Sockets;
=======
>>>>>>> b586ab3399de4ef659956f2496229a632fbad652
using System.Text;
using System.Threading.Tasks;

namespace AgilusLogger
{
<<<<<<< HEAD
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
=======
    public enum NodeAction
    {
        Install,
        Uninstall,
        Update
>>>>>>> b586ab3399de4ef659956f2496229a632fbad652
    }

    public static class Node
    {
        private static Process _process;
        public static event EventHandler OnInstallBegin;
<<<<<<< HEAD
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




=======
        public static event EventHandler OnInstallFinished;

        public static void ExecuteNode(NodeAction command)
        {
>>>>>>> b586ab3399de4ef659956f2496229a632fbad652
            _process = new Process
            {
                StartInfo =
                {
                    FileName = "",
<<<<<<< HEAD
                    Arguments = $"node {command} ",
=======
                    Arguments = "",
>>>>>>> b586ab3399de4ef659956f2496229a632fbad652
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    RedirectStandardOutput = true
                }
                
            };
            {
                _process.Start();
<<<<<<< HEAD
                OnInstallBegin?.Invoke(null, new EventArgs());
                _process.WaitForExit();
                OnInstallFinished?.Invoke(null, new EventArgs());
=======
                OnInstallBegin?.Invoke(this, new EventArgs());
                _process.WaitForExit();
                OnInstallFinished?.Invoke(this, new EventArgs());
>>>>>>> b586ab3399de4ef659956f2496229a632fbad652
            }


            
        }


    }
}
