using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace AgilusLogger
{
    public class LoggerService
    {        
        public string EntityName { get; private set; }
        public string Status { get; private set; }
        public ServiceController Service { get; private set; }
        public string Lifetime { get; private set; }
        public string Port { get; set; }
        public LoggerService(ServiceController serviceController)
        {
            Service = serviceController;            
            Status = Service.Status.ToString();            
            DisplayNameParser(Service.DisplayName);
            if (null != Service.GetLifetimeService())
            {
                Lifetime = Service.GetLifetimeService().ToString();    
            }            
        }
        private void DisplayNameParser(string displayName)
        {
            var regex = new Regex(@"(?:Agilus\sLogger\s)(?:-\s)(\w*\s)(?:\(porta:\s)(\d*)");
            var match = regex.Match(displayName);
            EntityName = match.Groups[1].Value;
            Port = match.Groups[2].Value;
        }
    }
}
