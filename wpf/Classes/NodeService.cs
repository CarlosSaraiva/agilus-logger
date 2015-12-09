using System;
using System.Diagnostics;
using System.IO;

namespace AgilusLogger.Classes
{
    using static Directory;
    using static Path;
    using static Environment;

    public static class NodeService
    {
        private static Process _process;

        public static event EventHandler<MessageEventArgs> OnExit;
        public static event EventHandler<MessageEventArgs> OnBeginProcess;
        public static event EventHandler<MessageEventArgs> OnUpdate;
        public static event EventHandler<MessageEventArgs> OnFailure;
        public static readonly string LoggerPath = Combine(GetFolderPath(SpecialFolder.ApplicationData), "agilus-logger");
        public static readonly string NodePath64 = Combine(GetFolderPath(SpecialFolder.ProgramFiles),"nodejs");
        public static string ServicePath;
        private static string[] _loggerFiles;
        private static string[] _serviceFiles;
        private static string DbUserId { get; set; }
        private static string DbPassword { get; set; }
        private static string DbName { get; set; }
        private static string DbAddress { get; set; }

        public static void SetupNode(string serviceName, string servicePort)
        {
            ServicePath = Combine(LoggerPath, serviceName);
            OnBeginProcess?.Invoke(null, new MessageEventArgs("Inicializado o proceeso de instalação..."));

            var proccess = $"{LoggerPath}\\install.cmd";
            var flags = $"-n {serviceName} -p {servicePort}";
            
            var message = $"Serviço {serviceName} inicializado na porta: {servicePort}.";

            OnBeginProcess?.Invoke(null, new MessageEventArgs("Inicio da instalação..."));
            PrepareInstall();
            Update();

            StartProcess(proccess, flags, message);
            OnExit?.Invoke(null, new MessageEventArgs(message));
        }

        public static void UninstallService(LoggerService loggerService)
        {
            const string process = "sc";
            var flags = $"delete {loggerService.ServiceName}";
            var message = $"Serviço {loggerService.EntityName} inicializado na porta: {loggerService.Port} foi removido do sistema.";
            StartProcess(process, flags, message);
            
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

            new UDL("SQLOLEDB.1", DbPassword, true, DbUserId, DbName, DbAddress).GenerateUDL("dist\\service");

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

        private static void StartProcess(string process, string flags, string message)
        {
            _process = new Process
            {
                StartInfo =
                {
                    FileName = process,
                    Arguments = flags,
                    UseShellExecute  = true,
                    RedirectStandardOutput = false,
                    Verb = "runas"
                }
            };
            {
                _process.Start();
                _process.WaitForExit();
            }
            OnExit?.Invoke(null, new MessageEventArgs(message));
        }

        private static void Update()
        {
            string npmProccess = null;

            if (File.Exists("C:\\Program Files\\nodejs\\npm.cmd"))
            {
                npmProccess = "C:\\Program Files\\nodejs\\npm.cmd";
            }
            else if (File.Exists("C:\\Program Files(x86)\\nodejs\\npm.cmd"))
            {
                npmProccess = "C:\\Program Files\\nodejs\\npm.cmd";
            }
            else if (File.Exists(GetFolderPath(SpecialFolder.ApplicationData) + "\\npm\\npm.cmd"))
            {
                npmProccess = GetFolderPath(SpecialFolder.ApplicationData) + "\\npm\\npm.cmd";
            }
            else
            {
                OnFailure?.Invoke(null, new MessageEventArgs("Comando npm não encontrado."));
            }

            if(npmProccess != null)
                OnUpdate?.Invoke(null, new MessageEventArgs("Atualizando/Instalando módulos"));
                StartProcess(npmProccess, $"install --prefix {LoggerPath}", "NPM módulos instalados/atualizados.");

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
}