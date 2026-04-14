using System.Text.Json.Serialization;

namespace Transmission.API.RPC.Common
{
	/// <summary>
	/// Transmission response
	/// </summary>
	public class TransmissionResponse : CommunicateBase
	{
		/// <summary>
		/// Contains "success" on success, or an error string on failure.
		/// </summary>
		[JsonPropertyName("result")]
		public string Result { get; set; }
	}
}
