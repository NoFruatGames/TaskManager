using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferDataTypes.Payloads
{
    public struct PayloadLoginToAccountResult
    {
        public PayloadLoginToAccountResult() { }
        public string SessionKey { get; set; } = string.Empty;
        public LoginToAccountResult Result { get; set; } = LoginToAccountResult.None;
    }
    public enum LoginToAccountResult
    {
        None, Success, WrongPassword, WrongUsername
    }
}
