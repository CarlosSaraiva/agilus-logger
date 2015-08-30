using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace AgilusLogger
{
    using static System.IO.Path;

    [System.ComponentModel.DesignerCategory(@"Code")]
    public class LoggerService : ServiceController
    {
        public int Port { get; private set; }

        private string _serviceFolderPath;

        public string ServiceFolderPath
        {
            get
            {
                return _serviceFolderPath;
            }

            private set
            {
                _serviceFolderPath = Combine(MainWindow.LoggerPath, value);
            }
        }

        public string EntityName { get; private set; }

        //Constructor
        public LoggerService(ServiceController service) : base(service.ServiceName)
        {
            ServiceNamePortParser(service.DisplayName);
        }

        //Methods
        private void ServiceNamePortParser(string displayName)
        {
            var regex = new Regex(@"(?:Agilus\sLogger\s)(?:-\s)(\w*\s)(?:\(porta:\s)(\d*)");
            var match = regex.Match(displayName);
            EntityName = match.Groups[1].Value.Trim();
            Port = int.Parse(match.Groups[2].Value);
            ServiceFolderPath = EntityName;
        }

        override public string ToString()
        {
            return $"{EntityName} - {Port}";
        }
    }
}