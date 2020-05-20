using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.API.RPC.Common
{
	/// <summary>
	/// Transmission request 
	/// </summary>
	public class TransmissionRequest : CommunicateBase
	{
		/// <summary>
		/// Name of the method to invoke
		/// </summary>
		[JsonProperty("method")]
		public string Method;

        /// <summary>
        /// Initialize request
        /// </summary>
        /// <param name="method">Method name</param>
        public TransmissionRequest(string method)
        {
            this.Method = method;
        }

        /// <summary>
        /// Initialize request 
        /// </summary>
        /// <param name="method">Method name</param>
        /// <param name="arguments">Arguments</param>
		public TransmissionRequest(string method, ArgumentsBase arguments)
		{
			this.Method = method;
			this.Arguments = arguments.Data;
		}

        /// <summary>
        /// Initialize request 
        /// </summary>
        /// <param name="method">Method name</param>
        /// <param name="arguments">Arguments</param>
        public TransmissionRequest(string method, Dictionary<string, object> arguments)
        {
            this.Method = method;
            this.Arguments = arguments;
        }
	}
}
