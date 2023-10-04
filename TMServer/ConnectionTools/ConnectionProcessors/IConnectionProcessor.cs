using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.ConnectionTools.ConnectionProcessors
{
    internal interface IConnectionProcessor : IDisposable
    {
        string GetHost();
        int GetPort();
        Task<string?> ReadLineAsync();
        Task WriteLineAsync(string message, bool flush=true);
    }
}
