using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AgilusLogger
{
    public static class Node
    {
        private static Process _process;

        public static event EventHandler OnExit;

        private static event EventHandler OnBeginInstall;

        public static async Task ExecuteNodeAsync(NodeAction command, object service)
        {
            string flags;

            switch (command.Id)
            {
                case 1:
                    flags = $"-n {service.name} -p {service.port}";
                    break;

                case 2:
                    flags = $"{service.name}";
                    break;

                case 3:
                    flags = String.Empty;
                    break;

                default:
                    flags = String.Empty;
                    break;
            }

            _process = new Process
            {
                StartInfo =
                {
                    FileName = Path.Combine(),
                    Arguments = $"node {command.Value} {flags} ",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    RedirectStandardOutput = true
                }
            };
            {
                _process.Start();
                OnBeginInstall?.Invoke(null, new EventArgs());
                OnExit?.Invoke(null, EventArgs.Empty);
            }
        }
    }

    public class NodeAction
    {
        private NodeAction(string value, int id)
        {
            Value = value;
            Id = id;
        }

        public int Id { get; set; }
        public static NodeAction Install => new NodeAction("Install", 1);
        public static NodeAction Uninstall => new NodeAction("Uninstall", 2);
        public static NodeAction Update => new NodeAction(@"Update", 3);
        public string Value { get; set; }
    }
}