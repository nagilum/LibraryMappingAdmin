using System.Collections.Generic;

namespace DotNetLibraryAdmin.Controllers.Payloads
{
    public class FileEntryGetAllResponsePayload
    {
        public List<PayloadServer> Servers { get; set; }

        public class PayloadServer
        {
            public string ServerName { get; set; }

            public List<string> ServerIps { get; set; }
        }
    }
}