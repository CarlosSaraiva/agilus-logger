using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace AgilusLogger
{
    public class LoggerService : ServiceController
    {
        public ServiceController Logger;

        private ServiceController _service;

        private string _serviceFolderPath;

        public LoggerService(ServiceController service)
        {
            Service = service;
        }

        public ServiceController Service
        {
            get
            {
                return _service;
            }

            private set
            {
                ServiceNamePortParser(value.DisplayName);
                _service = value;
            }
        }

        public string ServiceFolderPath
        {
            get
            {
                return _serviceFolderPath;
            }

            private set
            {

                _serviceFolderPath = value;
            }
        }

        private string EntityName { get; set; }

        private int Port { get; set; }

        override public string ToString()
        {
            return $"{EntityName} - {Port}";
        }

        private void ServiceNamePortParser(string displayName)
        {
            var regex = new Regex(@"(?:Agilus\sLogger\s)(?:-\s)(\w*\s)(?:\(porta:\s)(\d*)");
            var match = regex.Match(displayName);
            EntityName = match.Groups[1].Value;
            Port = int.Parse(match.Groups[2].Value);
            ServiceFolderPath = EntityName;
        }
    }

}