using System;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace AgilusLogger
{
    class LoggerInstance
    {
        private ServiceController _service;
        public ServiceController Service
        {
            set
            {
                ServiceNamePortParser(value);
                _service = value;
            }
        }

        public string ServiceFolderPath { get; private set; }
        public string EntityName { get; private set; }
        public int Port { get; private set; }
        
        public LoggerInstance(ServiceController service)
        {
            Service = service;
        }

        private void ServiceNamePortParser(ServiceController service)
        {
            var regex = new Regex(@"(?:Agilus\sLogger\s)(?:-\s)(\w*\s)(?:\(porta:\s)(\d*)");
            var match = regex.Match(service.DisplayName);
            EntityName = match.Groups[1].Value;
            Port = int.Parse(match.Groups[2].Value);
        }

        override public string ToString()
        {
            return $"{EntityName} - {Port}";
        }

    }

}