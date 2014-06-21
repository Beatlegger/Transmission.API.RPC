using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transsmission.API.RPC.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class TransmissionArgument
    {
        public Dictionary<string, object> ToArguments()
        {
            var result = new Dictionary<string, object>();

            var type = this.GetType();
            var properties = type.GetProperties();

            foreach (var prop in properties)
            {
                var propType = prop.PropertyType;

                var propJsonAttr = prop.CustomAttributes.FirstOrDefault(attr =>
                    attr.AttributeType == typeof(JsonPropertyAttribute));

                if (propJsonAttr == null)
                    continue;

                var propJsonAttrArg = propJsonAttr.ConstructorArguments.FirstOrDefault(arg => arg.Value != null);

                if (propJsonAttrArg == null)
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
