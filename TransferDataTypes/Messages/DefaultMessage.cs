using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferDataTypes.Messages
{
    public class DefaultMessage :TMMessage
    {
        public required override bool IsRequest
        {
            get
            {
                return !string.IsNullOrEmpty(getParameterValue<string>("request"));
            }
            set
            {
                if (value) setParameter("request", string.Empty);
                else setParameter("response", string.Empty);

            }
        }
        public void SetParameter(string parameterName, object value)
        {
            setParameter(parameterName, value);
        }
        public T? GetParameterValue<T>(string parameterName)
        {
            return getParameterValue<T>(parameterName);
        }
        public static DefaultMessage Parse(TMMessage message)
        {
            DefaultMessage result = new DefaultMessage() { IsRequest=false};
            foreach (var p in message.parameters)
            {
                result.setParameter(p.Key, p.Value);
            }
            return result;
        }
    }
}
