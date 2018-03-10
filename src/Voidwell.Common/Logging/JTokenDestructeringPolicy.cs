using Newtonsoft.Json.Linq;
using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;
using System.Linq;

namespace Voidwell.Common.Logging
{
    public class JTokenDestructuringPolicy : IDestructuringPolicy
    {
        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            var jo = value as JObject;
            if (jo != null)
            {
                result = Destructure(jo, propertyValueFactory);
                return true;
            }

            var ja = value as JArray;
            if (ja != null)
            {
                result = Destructure(ja, propertyValueFactory);
                return true;
            }

            var jv = value as JValue;
            if (jv != null)
            {
                result = Destructure(jv, propertyValueFactory);
                return true;
            }

            result = null;
            return false;
        }

        LogEventPropertyValue Destructure(JValue jv, ILogEventPropertyValueFactory propertyValueFactory)
        {
            return propertyValueFactory.CreatePropertyValue(jv.Value, true);
        }

        LogEventPropertyValue Destructure(JArray ja, ILogEventPropertyValueFactory propertyValueFactory)
        {
            var elems = ja.Select(t => propertyValueFactory.CreatePropertyValue(t, true));
            return new SequenceValue(elems);
        }

        LogEventPropertyValue Destructure(JObject jo, ILogEventPropertyValueFactory propertyValueFactory)
        {
            string typeTag = null;
            var props = new List<LogEventProperty>(jo.Count);
            foreach (var prop in jo.Properties())
            {
                if (prop.Name == "$type")
                {
                    var typeVal = prop.Value as JValue;
                    if (typeVal != null && typeVal.Value is string)
                    {
                        typeTag = (string)typeVal.Value;
                        continue;
                    }
                }

                props.Add(new LogEventProperty(prop.Name, propertyValueFactory.CreatePropertyValue(prop.Value, true)));
            }

            return new StructureValue(props, typeTag);
        }
    }
}
