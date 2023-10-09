using Newtonsoft.Json;
using System.Text.Json.Serialization;
using TransferDataTypes.Messages;

namespace TransferDataTypes
{
    public abstract class TMMessage
    {
        [JsonProperty]
        internal List<KeyValueStruct> parameters = new List<KeyValueStruct>();
        [Newtonsoft.Json.JsonIgnore]
        public int ParametersCount { get { return parameters.Count; } }
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
    }
    
}