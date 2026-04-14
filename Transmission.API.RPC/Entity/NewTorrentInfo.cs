using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.API.RPC.Entity
{
	/// <summary>
	/// Information of added torrent
	/// </summary>
	public class NewTorrentInfo
	{
		/// <summary>
		/// Torrent ID
		/// </summary>
		[JsonPropertyName("id")]
		public int ID { get; set; }

		/// <summary>
		/// Torrent name
		/// </summary>
		[JsonPropertyName("name")]
		public string Name { get; set; }

		/// <summary>
		/// Torrent Hash
		/// </summary>
		[JsonPropertyName("hashString")]
		public string HashString { get; set; }

	}
}
