using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferDataTypes.Messages
{
    public class DefaultMessage :TMMessage
    {
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
            DefaultMessage result = new DefaultMessage();
            foreach (var p in message.parameters)
            {
                result.setParameter(p.Key, p.Value);
            }
            return result;
        }
    }
}
