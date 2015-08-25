using System;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace AgilusLogger
{
    class LoggerInstance
    {
        public ServiceController Logger;
        public string ServiceFolderPath { get; private set; }
        public string EntityName { get; private set; }
        public int Port { get; private set; }
        
        public LoggerInstance()
        {
            
        }

        private void ServiceNamePortParser(string displayName)
        {
            var regex = new Regex(@"(?:Agilus\sLogger\s)(?:-\s)(\w*\s)(?:\(porta:\s)(\d*)");
            var match = regex.Match(displayName);
            EntityName = match.Groups[1].Value;
            Port = int.Parse(match.Groups[2].Value);
        }

    }

}