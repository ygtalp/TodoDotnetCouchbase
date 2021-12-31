using System.Text.Json.Serialization;

namespace API.Models
{
	public class ReturnModel
	{
		[JsonPropertyName("useremail")]
		public string UserEmail { get; set; }

		[JsonPropertyName("password")]
		public string Password { get; set; }

        [JsonPropertyName("token")]
		public string Token { get; set; }

		[JsonPropertyName("expiry")]
		public uint Expiry { get; set; }
	}
}