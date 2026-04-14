using System.Collections.Generic;
using System.Text.Json.Serialization;

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
		[JsonPropertyName("method")]
		public string Method { get; set; }

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
