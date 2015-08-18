using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace AgilusLogger
{
    public class LoggerService
    {
        private string status;
        private string service;
        private string name;
        public string Name
        {
            get { return name; }
            private set
            {
                Regex regex = new Regex(@"(Agilus Logger)(?:\s\-\s)(\w*\s)");
                Match match = regex.Match(value);
                name = match.Groups[2].ToString();
            }
        }
        public string Status { get; set; }
        public ServiceController Service { get; set; }

        public LoggerService(ServiceController serviceController)
        {
            Service = serviceController;
            Name = Service.DisplayName;
            Status = Service.Status.ToString();
        }
    }
}
