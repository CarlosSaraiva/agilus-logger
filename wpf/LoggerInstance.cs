using System;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace AgilusLogger
{
    class LoggerInstance
    {
<<<<<<< HEAD
        private ServiceController _service;
        public ServiceController Service
        {
            set
            {
                ServiceNamePortParser(value);
                _service = value;
            }
        }

=======
        public ServiceController Logger;
>>>>>>> 4d94fbc99207fb19bd2054c5bbe0539c63e8d5a9
        public string ServiceFolderPath { get; private set; }
        public string EntityName { get; private set; }
        public int Port { get; private set; }
        
<<<<<<< HEAD
        public LoggerInstance(ServiceController service)
        {
            Service = service;
        }

        private void ServiceNamePortParser(ServiceController service)
        {
            var regex = new Regex(@"(?:Agilus\sLogger\s)(?:-\s)(\w*\s)(?:\(porta:\s)(\d*)");
            var match = regex.Match(service.DisplayName);
=======
        public LoggerInstance()
        {
            
        }

        private void ServiceNamePortParser(string displayName)
        {
            var regex = new Regex(@"(?:Agilus\sLogger\s)(?:-\s)(\w*\s)(?:\(porta:\s)(\d*)");
            var match = regex.Match(displayName);
>>>>>>> 4d94fbc99207fb19bd2054c5bbe0539c63e8d5a9
            EntityName = match.Groups[1].Value;
            Port = int.Parse(match.Groups[2].Value);
        }

<<<<<<< HEAD
        override public string ToString()
        {
            return $"{EntityName} - {Port}";
        }

=======
>>>>>>> 4d94fbc99207fb19bd2054c5bbe0539c63e8d5a9
    }

}