using Newtonsoft.Json;
using System.Text.Json.Serialization;
using TransferDataTypes.Attributes;
using TransferDataTypes.Messages;

namespace TransferDataTypes
{
    public abstract class TMMessage
    {
        [JsonProperty]
        internal List<KeyValueStruct> parameters = new List<KeyValueStruct>();
        [Newtonsoft.Json.JsonIgnore]
        public int ParametersCount { get { return parameters.Count; } }
        [Newtonsoft.Json.JsonIgnore]
        public required abstract bool IsRequest { get; set; }
        public TMMessage() { }
        internal struct KeyValueStruct
        {
            public string Key { get; set; }
            public object Value { get; set; }
        }
        internal void setParameter(string parameterName, object value)
        {
            KeyValueStruct newValue = new KeyValueStruct()
            {
                Key = parameterName,
                Value = value
            };
            for (int i = 0; i < parameters.Count; i++)
            {

                if (parameters[i].Key == parameterName)
                {
                    parameters[i] = newValue;
                    return;
                }
            }
            parameters.Add(newValue);
        }
        internal T? getParameterValue<T>(string parameterName)
        {
            object? value = null;
            for (int i = 0; i < parameters.Count; i++)
            {

                if (parameters[i].Key == parameterName)
                {
                    value = parameters[i].Value;
                    break;
                }
            }
            return DeserializeFromObject<T>(value ?? string.Empty);

        }
        protected string TextSerialize(object objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize);
        }
        protected static T? DeserializeFromObject<T>(object value)
        {
            try
            {
                if (value is T tValue) return tValue;
                T? TObject = JsonConvert.DeserializeObject<T>(value.ToString() ?? string.Empty);
                return TObject;
            }
            catch
            {
                return default;
            }

        }
        public string Serialize()
        {
            return TextSerialize(this);
        }
        public static DefaultMessage? Deserialize(string text)
        {
            return DeserializeFromObject<DefaultMessage>(text);
        }
        public override string ToString()
        {
            string text = string.Empty;
            foreach (var item in parameters)
            {
                text += $"{item.Key}:{item.Value.ToString()}\n";
            }
            return text;
        }

        internal static CheckPropertyResult<V> getPropertyValue<V,T>(string propertyName, string paramName, TMMessage message) where V : notnull
        {
            Type typeCheck = typeof(T);
            V? messageV = message.getParameterValue<V>(paramName);
            var properties = typeCheck.GetProperties();
            foreach (var property in properties)
            {
                if (property.Name == propertyName)
                {
                    if (property.IsDefined(typeof(RequestPropertyAttribute), false))
                    {

                        if (message.IsRequest && ((messageV is string str && string.IsNullOrEmpty(str)) || (messageV is Enum enm && enm.ToString() == "None"))) return new CheckPropertyResult<V>(default, false);
                        else return new CheckPropertyResult<V>(messageV, true);

                    }
                    else if(property.IsDefined(typeof(ResponsePropertyAttribute), false))
                    {
                        if (!message.IsRequest && ((messageV is string str && string.IsNullOrEmpty(str)) || (messageV is Enum enm && enm.ToString() == "None"))) return new CheckPropertyResult<V>(default, false);
                        else return new CheckPropertyResult<V>(messageV, true);
                    }
                    else if (property.IsDefined(typeof(RequestResponsePropertyAttribute), false))
                    {
                        if ((messageV is string str && string.IsNullOrEmpty(str)) || (messageV is Enum enm && enm.ToString() == "None")) return new CheckPropertyResult<V>(default, false);
                        else return new CheckPropertyResult<V>(messageV, true);
                    }
                    else
                    {
                        return new CheckPropertyResult<V>(messageV, true);
                    }
                }

            }
            return new CheckPropertyResult<V>(messageV, true); 
        }
    }
    internal class CheckPropertyResult<T> where T : notnull
    {
        public T? Property { get;} = default;
        public bool Success { get;} = false;
        public CheckPropertyResult(T? property, bool success)
        {
            Property = property; Success = success;
        }
    }
    
}