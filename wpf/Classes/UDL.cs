using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgilusLogger.Classes
{
    public class UDL
    {
        public string Provider { get; set; }
        public string Password { get; set; }
        public bool PersistSecurityInfo { get; set; }
        public string UserId { get; set; }
        public string InitialCatalog { get; set; }
        public string DataSource { get; set; }
        private string Template { get; set; }
        private string Header { get; set; }

        public UDL(string provider, string password, bool persistSecurityInfo, string userId, string initialCatalog, string dataSource)
        {
            Provider = provider;
            Password = password;
            PersistSecurityInfo = persistSecurityInfo;
            UserId = userId;
            InitialCatalog = initialCatalog;
            DataSource = dataSource;
            GenerateTemplate();
        }

        private void GenerateTemplate()
        {
            Header = $"[oledb]\n; Everything after this line is an OLE DB initstring\nProvider={Provider};";
            var body = $"Password={Password};Persist Security Info={PersistSecurityInfo};User ID={UserId};Initial Catalog={InitialCatalog};Data Source={DataSource};Application Name=Agilus Logger\n";
            Template = Header + body;
        }

        // ReSharper disable once InconsistentNaming
        public UDL GenerateUDL(string path)
        {
            const string fileName = "agilus.udl";
            var file = Path.Combine(path, fileName);
            //File.Create(file).Dispose();
            //File.WriteAllText(file, Template, Encoding.Unicode);

            File.Create(file).Dispose();

            var process = Process.Start(file);
            process?.WaitForExit();
            return this;            

        }
    }
}
