using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerceptronHost
{
    public class RequestProcessor
    {
        public RequestProcessor(string keyValueSeparator, string attributeSeparator)
        {
            KeyValueSeparator = keyValueSeparator.ToLower();
            AttributeSeparator = attributeSeparator.ToLower();
        }
        public Dictionary<string, string> Recieve(string data)
        {
            string[] attributes = data.ToLower()
                .Split(AttributeSeparator.ToCharArray());
            var keyValues = new Dictionary<string, string>();
            for (int i = 0; i < attributes.Length; i++)
            {
                string[] keyValue = attributes[i].Split(KeyValueSeparator.ToCharArray());
                if (keyValue.Length != 2) 
                    throw new InvalidOperationException($"Должно быть" +
                        $" key{KeyValueSeparator}value");
                if (keyValues.ContainsKey(keyValue[0])) 
                    throw new InvalidOperationException($"Аттрибут {keyValue[0]} " +
                        $"встречается более одного раза");
                keyValues.Add(keyValue[0], keyValue[1]);
            }
            return keyValues;
        }
        public string KeyValueSeparator { get; }
        public string AttributeSeparator { get; }
    }
}
