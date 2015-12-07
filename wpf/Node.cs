using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace AgilusLogger
{
    using System.Reflection;
    using System.Security.Principal;
    using static Directory;
    using static Path;
    using static Environment;

    public static class Node
    {
        private static Process _process;

        public static event EventHandler<MessageEventArgs> OnExit;
        public static event EventHandler<MessageEventArgs> OnBeginProcess;
        public static event EventHandler<MessageEventArgs> OnNpm;
        public static event EventHandler<MessageEventArgs> OnFailure;
        public static readonly string LoggerPath = Combine(GetFolderPath(SpecialFolder.ApplicationData), "agilus-logger");
        public static readonly string NodePath64 = Combine(GetFolderPath(SpecialFolder.ProgramFiles),"nodejs");
        public static string ServicePath;
        private static NodeAction _command;
        private static string _fileName;
        private static string _flags;
        private static string[] _loggerFiles;
        private static string[] _serviceFiles;

        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static void SetupNode(NodeAction command, string serviceName, string servicePort)
        {
            string message = null;
            _command = command;
            ServicePath = Combine(LoggerPath, serviceName);
            OnBeginProcess?.Invoke(null, new MessageEventArgs("Inicializado o proceeso de instalação..."));
            switch (command.Id)
            {
                case 1:
                    _flags = $"-n {serviceName} -p {servicePort}";
                    _fileName = $"{LoggerPath}\\{_command.Value}";
                    message = $"Serviço {serviceName} inicializado na porta: {servicePort}.";
                    break;

                case 2:
                    _flags = $"delete agiluslogger{serviceName}porta{servicePort}.exe";
                    _fileName = _command.Value;
                    message = $"Serviço {serviceName} inicializado na porta: {servicePort} foi removido do sistema.";
                    break;

                case 3:
                    _flags = string.Empty;
                    break;

                default:
                    _flags = string.Empty;
                    break;
            }

            OnBeginProcess?.Invoke(null, new MessageEventArgs("Inicio da atualização"));
            PrepareInstall();

            if (command.Id == 1)
            {
                if (File.Exists("C:\\Program Files\\nodejs\\npm.cmd"))
                {
                    OnNpm?.Invoke(null, new MessageEventArgs(UpdateNpm("C:\\Program Files\\nodejs\\npm.cmd")));
                }
                else if (File.Exists("C:\\Program Files(x86)\\nodejs\\npm.cmd"))
                {
                    OnNpm?.Invoke(null, new MessageEventArgs(UpdateNpm("C:\\Program Files(x86)\\nodejs\\npm.cmd")));
                }
                else if (File.Exists(GetFolderPath(SpecialFolder.ApplicationData) + "\\npm\\npm.cmd"))
                {
                    OnNpm?.Invoke(null, new MessageEventArgs(UpdateNpm(GetFolderPath(SpecialFolder.ApplicationData) + "\\npm\\npm.cmd")));
                }
                else
                {
                    OnFailure?.Invoke(null, new MessageEventArgs("Comando npm não encontrado."));
                }
                
            }
            StartProcess();
            OnExit?.Invoke(null, new MessageEventArgs(message));
        }

        private static void PrepareInstall()
        {
            if (!Exists(LoggerPath))
            {
                CreateDirectory(LoggerPath);
            }

            if (!Exists(ServicePath))
            {
                CreateDirectory(ServicePath);
            }

            _loggerFiles = GetFiles("dist\\logger");
            _serviceFiles = GetFiles("dist\\service");

            foreach (var s in _loggerFiles)
            {
                if (s == null) continue;
                var destFile = Path.Combine(LoggerPath, Path.GetFileName(s));
                File.Copy(s, destFile, true);
            }

            foreach (var s in _serviceFiles)
            {
                if (s == null) continue;
                var destFile = Path.Combine(ServicePath, Path.GetFileName(s));
                File.Copy(s, destFile, true);
            }
        }

        private static void StartProcess()
        {
            _process = new Process
            {
                StartInfo =
                {
                    FileName = _fileName,
                    Arguments = _flags,
                    UseShellExecute  = true,
                    RedirectStandardOutput = false,
                    Verb = "runas"
                }
            };
            {
                _process.Start();
            }            

        }

        private static string UpdateNpm(string npmPath)
        {
            var update = new Process
            {
                StartInfo =
                {
                    FileName = npmPath,
                    Arguments = $"install --prefix {LoggerPath}",
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = true,
                    RedirectStandardOutput = false,
                    Verb = "runas"
                }
            };
            {                  
                update.Start();
                update.WaitForExit();
            }
            return "NPM esta atualizando modulos da aplicaçao.";
        }
    }

    public class MessageEventArgs : EventArgs
    {
        public string Message { get; set; }

        public MessageEventArgs(string message)
        {
            Message = message;
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
        public static NodeAction Install => new NodeAction("install.cmd", 1);
        public static NodeAction Uninstall => new NodeAction("sc.exe", 2);
        public static NodeAction Update => new NodeAction(@"Update", 3);
        public string Value { get; set; }
    }
}