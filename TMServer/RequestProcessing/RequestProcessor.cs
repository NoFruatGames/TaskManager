using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.RequestProcessing
{
    internal partial class RequestProcessors
    {
        private class RequestProcessor
        {
            string Session { get; } = string.Empty;
            int ProfileId { get; } = -1;

            public RequestProcessor(string session, int profileId)
            {
                Session = session;
                ProfileId = profileId;
            }
        }
    }
    
}
