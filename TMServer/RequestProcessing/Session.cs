using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.RequestProcessing
{
    internal class Session
    {
        public string Key { get; init; } = string.Empty;
        public List<string> ClientIds { get; } = new List<string>();
        public Session(Random random)
        {
            for (int i = 0; i < 10; ++i)
            {
                Key += random.Next(0, 9).ToString();
            }
        }
        public Session(string key)
        {
            Key = key;
        }
    }
}
