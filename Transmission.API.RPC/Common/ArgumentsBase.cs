using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.API.RPC.Common
{
    /// <summary>
    /// Absract class for arguments
    /// </summary>
    public abstract class ArgumentsBase
    {
        public Dictionary<string, object> ToDictionary()
        {
            var result = new Dictionary<string, object>();

            var type = this.GetType();
            var properties = type.GetRuntimeProperties();

            foreach (var prop in properties)
            {
                var propType = prop.PropertyType;

                var propJsonAttr = prop.CustomAttributes.FirstOrDefault(attr =>
                    attr.AttributeType == typeof(JsonPropertyAttribute));

                if (propJsonAttr == null)
                    continue;

                var propJsonAttrArg = propJsonAttr.ConstructorArguments.FirstOrDefault(arg => arg.Value != null);

                if (propJsonAttrArg.Value == null)
                    continue;

                var argName = propJsonAttrArg.Value as String;
                var argValue = prop.GetValue(this);

                if (argValue == null)
                    continue;

                result.Add(argName, argValue);
            }

            return result;
        }
    }
}
