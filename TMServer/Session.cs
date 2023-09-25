using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer
{
    internal class Session
    {
        public string Key { get; init; } = string.Empty;
        public string AccountName {  get; init; } = string.Empty;
        public List<string> ClientIds { get; } = new List<string>();
        public Session(Random random, string accountName) { 
            AccountName = accountName;
            for(int i = 0; i < 10; ++i)
            {
                Key += (random.Next(0, 9)).ToString();
            }
        }
        public Session(string key, string accountName)
        {
            AccountName = accountName;
            Key = key;
        }
    }
}
